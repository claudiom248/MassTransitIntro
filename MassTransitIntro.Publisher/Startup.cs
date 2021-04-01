using MassTransit;
using MassTransit.ActiveMqTransport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MassTransitIntro.Publisher
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

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
            });

            services.AddSingleton<IBus>(bus);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            var bus = app.ApplicationServices.GetService<IBus>() as IBusControl;
            applicationLifetime.ApplicationStarted.Register(async () => { await bus.StartAsync(); });
            applicationLifetime.ApplicationStopped.Register(async () => { await bus.StopAsync(); });
        }
    }
}
