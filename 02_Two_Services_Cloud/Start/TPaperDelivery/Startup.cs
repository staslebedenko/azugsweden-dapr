using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace TPaperDelivery
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(options =>
            {
                options.AddFilter("TPaperDelivery", LogLevel.Information);
            });

            services.AddOptions<ProjectOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("ProjectOptions").Bind(settings);
                });

            services.AddControllers();
            services.AddHttpClient();

            var options = new ApplicationInsightsServiceOptions { ConnectionString = "InstrumentationKey=168f30f5-546f-4e1a-8607-df1c93052f2c;IngestionEndpoint=https://northeurope-0.in.applicationinsights.azure.com/" };
            services.AddApplicationInsightsTelemetry(options: options);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
