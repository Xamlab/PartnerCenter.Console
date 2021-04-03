using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PartnerCenter.Console.Configuration;
using PartnerCenter.Console.Services;
using PartnerCenter.Console.Services.Implementation;
using PartnerCenter.Console.Utilities;

namespace PartnerCenter.Console
{
    public static class Bootstrapper
    {
        public static IServiceCollection CreateContainer()
        {
            return new ServiceCollection();
        }

        public static IServiceCollection AddConfiguration(this IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false)
                                .AddUserSecrets(typeof(Program).Assembly)
                                .AddEnvironmentVariables()
                                .Build();
            services.AddSingleton<IConfiguration>(configuration);

            var partnerCenterConfiguration = configuration.GetSection("PartnerCenter").Get<PartnerCenterConfiguration>();
            services.AddSingleton(partnerCenterConfiguration);

            return services;
        }

        public static IServiceCollection AddConsoleServices(this IServiceCollection services)
        {
            services.AddTransient<AuthenticationDelegatingHandler>();
            services.AddTransient<IUpdateService, UpdateService>();
            services.AddHttpClient<ITokenService, TokenService>((serviceProvider, client) =>
                                                                {
                                                                    var configuration = serviceProvider.GetRequiredService<PartnerCenterConfiguration>();
                                                                    client.BaseAddress = new Uri(configuration.TokenBaseUrl);
                                                                });
            services.AddHttpClient<IPartnerCenterService, PartnerCenterService>((serviceProvider, client) =>
                                                                                {
                                                                                    var configuration = serviceProvider.GetRequiredService<PartnerCenterConfiguration>();
                                                                                    client.BaseAddress = new Uri(configuration.ManagementBaseUrl);
                                                                                })
                    .AddHttpMessageHandler<AuthenticationDelegatingHandler>();
            return services;
        }
    }
}