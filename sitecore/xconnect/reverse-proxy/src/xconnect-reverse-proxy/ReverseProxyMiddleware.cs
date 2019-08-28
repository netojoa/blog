// from https://auth0.com/blog/building-a-reverse-proxy-in-dot-net-core/

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace xConnectReverseProxy
{
    public class ReverseProxyMiddleware
    {
        private readonly HttpClient httpClient;
        private readonly RequestDelegate next;
        private readonly IConfiguration configuration;
        private readonly ILogger<RequestProcessor> log;

        public ReverseProxyMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<RequestProcessor> log)
        {
            this.configuration = configuration;
            this.next = next;
            this.log = log;
        }

        public async Task Invoke(HttpContext context)
        {
            log.LogInformation($"A request was made to {context.Request.Path.Value}");

            try
            {
                await ProcessReverseProxy(context);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "An error occurred.");
                throw ex;
            }

            await next(context);
        }

        protected async Task ProcessReverseProxy(HttpContext context)
        {
            var processor = new RequestProcessor(configuration, log, context);
            await processor.Process();
        }
    }
}