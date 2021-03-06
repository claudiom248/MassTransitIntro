using MassTransit;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MassTransitIntro.Consumer2
{
    public class Program
    {
        private static readonly IConfiguration Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName)
            .AddJsonFile("appsettings.json")
            .Build();

        static async Task Main()
        {
            var smsSender = new FakeSmsSender();

            var bus = Bus.Factory.CreateUsingAmazonSqs(cfg =>
            {
                cfg.Host(Configuration["AmazonSqs:Region"], host =>
                {
                    host.AccessKey(Configuration["AmazonSqs:AccessKey"]);
                    host.SecretKey(Configuration["AmazonSqs:SecretKey"]);
                });

                cfg.ReceiveEndpoint(Configuration["AmazonSqs:Queue"], endpoint =>
                {
                    endpoint.Consumer(() => new MeetingCreatedMessageConsumer(smsSender));
                });
            });

            await bus.StartAsync();
            Console.ReadKey();
            await bus.StopAsync();
        }
    }
}
