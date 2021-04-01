using MassTransit;
using MassTransit.ActiveMqTransport;
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

            var bus = Bus.Factory.CreateUsingActiveMq(cfg =>
            {
                var useSsl = bool.Parse(Configuration["ActiveMQ:UseSsl"]);
                cfg.Host(Configuration["ActiveMQ:Host"], host =>
                {
                    host.Username(Configuration["ActiveMQ:Username"]);
                    host.Password(Configuration["ActiveMQ:Password"]);

                    if (useSsl)
                    {
                        host.UseSsl();
                    }
                });

                    cfg.ReceiveEndpoint(Configuration["ActiveMQ:Queue"], endpoint =>
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
