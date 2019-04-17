using Convey.CQRS.Queries.Dispatchers;
using Microsoft.Extensions.DependencyInjection;

namespace Convey.CQRS.Queries
{
    public static class Extensions
    {
        public static IConveyBuilder AddQueryHandlers(this IConveyBuilder builder)
        {
            builder.Services.AddTransient<IQueryDispatcher, QueryDispatcher>();
            builder.Services.Scan(s =>
                s.FromEntryAssembly()
                    .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            return builder;
        }
    }
}