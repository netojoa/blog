using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace xConnectReverseProxy
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly ILogger logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            this.logger.LogInformation("Configured Services");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/log-{Date}.txt");

            if (env.IsDevelopment())
            {
                this.logger.LogInformation("In Development environment");
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseMiddleware<ReverseProxyMiddleware>();

            app.Run(async (context) =>
            {
                if (!context.Request.Path.Value.Contains("odata"))
                    await context.Response.WriteAsync("Status: OK");
            });
        }
    }
}