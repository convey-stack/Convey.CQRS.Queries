using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Convey.CQRS.Queries.Dispatchers
{
    internal sealed class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceScopeFactory _serviceFactory;

        public QueryDispatcher(IServiceScopeFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
        {
            using (var scope = _serviceFactory.CreateScope())
            {
                var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
                dynamic handler = scope.ServiceProvider.GetRequiredService(handlerType);
                
                return handler.HandleAsync((dynamic) query);
            }
        }

        public Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : class, IQuery<TResult>
        {
            using (var scope = _serviceFactory.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();

                return handler.HandleAsync(query);
            }
        }
    }
}