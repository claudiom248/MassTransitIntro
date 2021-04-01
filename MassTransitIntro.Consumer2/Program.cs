﻿using MassTransit;
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

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(Configuration["RabbitMQ:Host"], host =>
                {
                    host.Username(Configuration["RabbitMQ:Username"]);
                    host.Password(Configuration["RabbitMQ:Password"]);
                });

                cfg.ReceiveEndpoint(Configuration["RabbitMQ:Queue"], endpoint =>
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
