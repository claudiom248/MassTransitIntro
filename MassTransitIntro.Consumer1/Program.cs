using MassTransit;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MassTransitIntro.Consumer1
{
    public class Program
    {
        private static readonly IConfiguration Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName)
            .AddJsonFile("appsettings.json")
            .Build();

        static async Task Main()
        {
            var emailSender = new FakeEmailSender();
            
            var bus = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
            {
                cfg.Host(Configuration["AzureServiceBus:ConnectionString"]);

                cfg.ReceiveEndpoint(Configuration["AzureServiceBus:Queue"], endpoint =>
                {
                    endpoint.Consumer(() => new MeetingCreatedMessageConsumer(emailSender));
                });
            });

            await bus.StartAsync();
            Console.ReadKey();
            await bus.StopAsync();
        }
    }
}