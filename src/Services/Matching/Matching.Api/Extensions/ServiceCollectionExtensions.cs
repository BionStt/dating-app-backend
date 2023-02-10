using Application.Shared.Pipes;
using EventBus.Messages.Common;
using EventBus.Messages.Publisher;
using MassTransit;
using Matching.Application.Common.Interfaces;
using Matching.Application.Domain.Factories;
using Matching.Application.Features.Matches.EventHandlers;
using Matching.Application.Features.Swipes.EventHandlers;
using Matching.Application.Infrastructure.Cache;
using Matching.Application.Infrastructure.Dapper;
using Matching.Application.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Utils.Time;

namespace Matching.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MatchingApplication");
            services.AddDbContext<MatchingDbContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IDapperContext, DapperContext>();
        }

        public static void AddDomainFactories(this IServiceCollection services)
        {
            services.AddTransient<ISwipeIdFactory, SwipeIdFactory>();
            services.AddTransient<IMatchIdFactory, MatchIdFactory>();
        }

        public static void AddPipes(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }

        public static void AddUtils(this IServiceCollection services)
        {
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        }

        public static void AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(opt => opt.Configuration = configuration["CacheSettings:ConnectionString"]);
            services.AddScoped<ISwipesCacheRepository, SwipesCacheRepository>();
        }

        public static void AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(config =>
            {
                config.AddConsumer<LeftSwipeEventHandler>();
                config.AddConsumer<RightSwipeEventHandler>();
                config.AddConsumer<MatchEventHandler>();

                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(configuration["EventBusSettings:HostAddress"]);

                    cfg.ReceiveEndpoint(EventBusConstants.LeftSwipeEventQueue, c =>
                    {
                        c.ConfigureConsumer<LeftSwipeEventHandler>(ctx);
                    });


                    cfg.ReceiveEndpoint(EventBusConstants.RightSwipeEventQueue, c =>
                    {
                        c.ConfigureConsumer<RightSwipeEventHandler>(ctx);
                    });


                    cfg.ReceiveEndpoint(EventBusConstants.MatchEventQueue, c =>
                    {
                        c.ConfigureConsumer<MatchEventHandler>(ctx);
                    });
                });

            });

            services.AddScoped<IEventPublisher, EventPublisher>();
        }

    }
}
