using System;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using PartnerCenter.Console.Commands;
using PartnerCenter.Console.Configuration;
using PartnerCenter.Console.MicrosoftSample;
using PartnerCenter.Console.Services;

namespace PartnerCenter.Console
{
    internal static class Program
    {
        private static IServiceProvider Services { get; set; } = null!;

        private static async Task Main(string[] args)
        {
            System.Console.WriteLine("Starting up PartnerCenter Console");
            Services = Bootstrapper.CreateContainer()
                                   .AddConfiguration()
                                   .AddConsoleServices()
                                   .BuildServiceProvider();

            // await Parser.Default.ParseArguments<UpdateCommand>(args)
            //             .WithParsedAsync(UpdateAsync);
            Parser.Default.ParseArguments<UpdateCommand>(args)
                  .WithParsed(UpdateThroughMicrosoftSample);
        }

        private static async Task UpdateAsync(UpdateCommand updateCommand)
        {
            await Services.GetRequiredService<IUpdateService>().UpdateFlightAsync(updateCommand.Flight, updateCommand.BundlePath);
        }

        private static void UpdateThroughMicrosoftSample(UpdateCommand updateCommand)
        {
            var partnerCenterConfiguration = Services.GetRequiredService<PartnerCenterConfiguration>();
            var config = new ClientConfiguration
            {
                ApplicationId = partnerCenterConfiguration.ApplicationId,
                FlightId = partnerCenterConfiguration.FlightId,
                ClientId = partnerCenterConfiguration.ClientId,
                ClientSecret = partnerCenterConfiguration.ClientSecret,
                ServiceUrl = "https://manage.devcenter.microsoft.com",
                TokenEndpoint = $"https://login.microsoftonline.com/{partnerCenterConfiguration.Tenant}/oauth2/token",
                Scope = "https://manage.devcenter.microsoft.com",
            };
            var tokenService = Services.GetRequiredService<ITokenService>();
            new FlightSubmissionUpdateSample(config).RunFlightSubmissionUpdateSample(updateCommand.BundlePath, tokenService);
        }
    }
}