using System.Linq;
using System.Net.Http;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NSwag;
using NSwag.Generation.Processors.Security;
using TestApi.Common.Configuration;
using TestApi.Common.Configuration.Helpers;
using TestApi.Common.Security;
using TestApi.DAL.Commands;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Queries.Core;
using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Services.Clients.UserApiClient;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Services.Services;
using TestApi.Swagger;
using TestApi.Telemetry;

namespace TestApi
{
    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddOpenApiDocument((document, serviceProvider) =>
            {
                document.AddSecurity("JWT", Enumerable.Empty<string>(),
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Type into the textfield: Bearer {your JWT token}.",
                        Scheme = "bearer"
                    });
                document.Title = "Test Api";
                document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                document.OperationProcessors.Add(new AuthResponseOperationProcessor());
            });
            return services;
        }

        public static IServiceCollection AddCustomTypes(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddScoped<ILoggingDataExtractor, LoggingDataExtractor>();

            services.AddScoped<ITokenProvider, AzureTokenProvider>();
            services.AddSingleton<ITelemetryInitializer, BadRequestTelemetry>();

            services.AddTransient<BookingsApiTokenHandler>();
            services.AddTransient<UserApiTokenHandler>();
            services.AddTransient<VideoApiTokenHandler>();

            services.AddScoped<IQueryHandlerFactory, QueryHandlerFactory>();
            services.AddScoped<IQueryHandler, QueryHandler>();

            services.AddScoped<ICommandHandlerFactory, CommandHandlerFactory>();
            services.AddScoped<ICommandHandler, CommandHandler>();

            services.AddScoped<IAllocationService, AllocationService>();
            services.AddScoped<IBookingsApiService, BookingsApiService>();
            services.AddScoped<IUserApiService, UserApiService>();
            services.AddScoped<IVideoApiService, VideoApiService>();

            RegisterCommandHandlers(services);
            RegisterQueryHandlers(services);

            var container = services.BuildServiceProvider();
            var servicesConfiguration = container.GetService<IOptions<ServicesConfiguration>>().Value;

            services.AddHttpClient<IBookingsApiClient, BookingsApiClient>()
                .AddHttpMessageHandler<BookingsApiTokenHandler>()
                .AddTypedClient(httpClient => BuildBookingsApiClient(httpClient, servicesConfiguration));

            services.AddHttpClient<IUserApiClient, UserApiClient>()
                .AddHttpMessageHandler<UserApiTokenHandler>()
                .AddTypedClient(httpClient => BuildUserApiClient(httpClient, servicesConfiguration));

            services.AddHttpClient<IVideoApiClient, VideoApiClient>()
                .AddHttpMessageHandler<VideoApiTokenHandler>()
                .AddTypedClient(httpClient => BuildVideoApiClient(httpClient, servicesConfiguration));

            return services;
        }

        private static void RegisterCommandHandlers(IServiceCollection serviceCollection)
        {
            var commandHandlers = typeof(ICommand).Assembly.GetTypes().Where(t =>
                t.GetInterfaces().Any(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(ICommandHandler<>)));

            foreach (var queryHandler in commandHandlers)
            {
                var serviceType = queryHandler.GetInterfaces()[0];
                serviceCollection.AddScoped(serviceType, queryHandler);
            }
        }

        private static void RegisterQueryHandlers(IServiceCollection serviceCollection)
        {
            var queryHandlers = typeof(IQuery).Assembly.GetTypes().Where(t =>
                t.GetInterfaces().Any(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));

            foreach (var queryHandler in queryHandlers)
            {
                var serviceType = queryHandler.GetInterfaces()[0];
                serviceCollection.AddScoped(serviceType, queryHandler);
            }
        }

        public static IServiceCollection AddJsonOptions(this IServiceCollection serviceCollection)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            serviceCollection.AddMvc()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = contractResolver;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            return serviceCollection;
        }

        private static IBookingsApiClient BuildBookingsApiClient(HttpClient httpClient,
            ServicesConfiguration serviceSettings)
        {
            return new BookingsApiClient(httpClient)
                {BaseUrl = serviceSettings.BookingsApiUrl, ReadResponseAsString = true};
        }

        private static IUserApiClient BuildUserApiClient(HttpClient httpClient,
            ServicesConfiguration serviceSettings)
        {
            return new UserApiClient(httpClient) {BaseUrl = serviceSettings.UserApiUrl, ReadResponseAsString = true};
        }

        private static IVideoApiClient BuildVideoApiClient(HttpClient httpClient,
            ServicesConfiguration serviceSettings)
        {
            return new VideoApiClient(httpClient) {BaseUrl = serviceSettings.VideoApiUrl, ReadResponseAsString = true};
        }
    }
}