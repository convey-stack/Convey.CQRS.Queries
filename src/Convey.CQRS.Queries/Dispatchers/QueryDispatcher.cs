using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Convey.CQRS.Queries.Dispatchers
{
    internal sealed class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
                dynamic handler = scope.ServiceProvider.GetService(handlerType);
                ValidateHandler(handler, query);
                
                return handler.HandleAsync((dynamic) query);
            }
        }

        public Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : class, IQuery<TResult>
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<IQueryHandler<TQuery, TResult>>();
                ValidateHandler(handler, query);

                return handler.HandleAsync(query);
            }
        }

        private static void ValidateHandler<T>(object handler, T query)
        {
            if (handler is null)
            {
                throw new InvalidOperationException($"Query handler for: '{query}' was not found.");
            }
        }
    }
}