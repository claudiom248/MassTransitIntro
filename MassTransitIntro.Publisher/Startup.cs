using MassTransit;
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

            var bus = Bus.Factory.CreateUsingAmazonSqs(cfg =>
            {
                cfg.Host(Configuration["AmazonSqs:Region"], host =>
                {
                    host.AccessKey(Configuration["AmazonSqs:AccessKey"]);
                    host.SecretKey(Configuration["AmazonSqs:SecretKey"]);
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
