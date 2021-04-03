using System;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using PartnerCenter.Console.Commands;
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

            await Parser.Default.ParseArguments<UpdateCommand>(args)
                        .WithParsedAsync(UpdateAsync);
        }

        private static async Task UpdateAsync(UpdateCommand updateCommand)
        {
            await Services.GetRequiredService<IUpdateService>().UpdateFlightAsync(updateCommand.Flight, updateCommand.BundlePath);
        }
        
    }
}