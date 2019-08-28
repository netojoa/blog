namespace xConnectReverseProxy
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;

    public class RequestProcessor
    {
        private HttpClient httpClient;
        private readonly HttpContext context;
        private readonly string xConnectHost = string.Empty;
        private readonly IConfiguration configuration;
        private readonly ILogger<RequestProcessor> logger;

        public RequestProcessor(IConfiguration configuration, ILogger<RequestProcessor> logger, HttpContext context)
        {
            this.httpClient = new HttpClient();
            this.configuration = configuration;
            this.xConnectHost = this.configuration["Settings:XConnectHost"];
            this.context = context;
            this.logger = logger;
        }

        private void LogConfiguration()
        {
            string message = string.Format("Parameters: xConnect host: \"{0}\". ConnectionString \"{1}\".", this.configuration["Settings:XConnectHost"], this.configuration.GetConnectionString("XConnectCertificate"));
            logger.LogInformation(message);
        }

        public async Task Process()
        {

            LogConfiguration();

            AttachClientCertificate();

            var targetUri = BuildTargetUri();
            if (targetUri != null)
            {
                var targetRequestMessage = CreateTargetRequestMessage(context, targetUri);
                await DoRequest(targetRequestMessage);
                return;
            }
        }

        private void AttachClientCertificate()
        {
            HttpClientHandler handler = new HttpClientHandler();
            X509Certificate2 certificate = CertificateHelper.GetCertificateFromStore(this.configuration.GetConnectionString("XConnectCertificate"));
            handler.ClientCertificates.Add(certificate);
            httpClient = new HttpClient(handler);

            logger.LogInformation($"Client \"{certificate.Thumbprint}\" certificate attached");
        }

        private async Task DoRequest(HttpRequestMessage targetRequestMessage)
        {
            using (var responseMessage = await httpClient.SendAsync(targetRequestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
            {
                logger.LogInformation($"Request executed. StatusCode is {responseMessage.StatusCode}");

                context.Response.StatusCode = (int)responseMessage.StatusCode;
                CopyFromTargetResponseHeaders(context, responseMessage);
                await ProcessResponseContent(context, responseMessage);
            }
        }

        private Uri BuildTargetUri()
        {
            Uri targetUri = null;

            if (this.context.Request.Path.StartsWithSegments("/odata", out PathString remainingPath))
            {
                targetUri = new Uri($"https://{xConnectHost}/odata" + remainingPath + this.context.Request.QueryString);
                logger.LogInformation($"Target uri built: {targetUri.ToString()}");
            }

            return targetUri;
        }

        private HttpRequestMessage CreateTargetRequestMessage(HttpContext context, Uri targetUri)
        {
            var requestMessage = new HttpRequestMessage();
            CopyRequestContentAndHeadersFromOriginal(context, requestMessage);
            requestMessage.RequestUri = targetUri;
            requestMessage.Headers.Host = targetUri.Host;
            requestMessage.Method = GetHttpMethod(context.Request.Method);

            logger.LogInformation($"Created target request message: {requestMessage.ToString()}");

            return requestMessage;
        }

        private HttpMethod GetHttpMethod(string method)
        {
            if (HttpMethods.IsDelete(method)) return HttpMethod.Delete;
            if (HttpMethods.IsGet(method)) return HttpMethod.Get;
            if (HttpMethods.IsHead(method)) return HttpMethod.Head;
            if (HttpMethods.IsOptions(method)) return HttpMethod.Options;
            if (HttpMethods.IsPost(method)) return HttpMethod.Post;
            if (HttpMethods.IsPut(method)) return HttpMethod.Put;
            if (HttpMethods.IsTrace(method)) return HttpMethod.Trace;
            return new HttpMethod(method);
        }

        private void CopyRequestContentAndHeadersFromOriginal(HttpContext context, HttpRequestMessage requestMessage)
        {
            var requestMethod = context.Request.Method;

            if (!HttpMethods.IsGet(requestMethod) &&
              !HttpMethods.IsHead(requestMethod) &&
              !HttpMethods.IsDelete(requestMethod) &&
              !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(context.Request.Body);
                requestMessage.Content = streamContent;
            }

            foreach (var header in context.Request.Headers)
            {
                requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }

            logger.LogInformation($"Copied request content and headers from original.");
        }

        private void CopyFromTargetResponseHeaders(HttpContext context, HttpResponseMessage responseMessage)
        {
            foreach (var header in responseMessage.Headers)
            {
                header.Value.ToList().ForEach(s => s = s.Replace(xConnectHost, context.Request.Host.Value));
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                header.Value.ToList().ForEach(s => s = s.Replace(xConnectHost, context.Request.Host.Value));
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            context.Response.Headers.Remove("transfer-encoding");

            logger.LogInformation($"Copied headers from target response.");
        }

        private async Task ProcessResponseContent(HttpContext context, HttpResponseMessage responseMessage)
        {
            var content = await responseMessage.Content.ReadAsByteArrayAsync();

            if (IsContentOfType(responseMessage, "application/json"))
            {
                await ReplaceXConnectHost(content, context);
                logger.LogInformation($"Processed response content with host replacement.");
            }
            else
            {
                await context.Response.Body.WriteAsync(content);
                logger.LogInformation($"Processed response content.");
            }
        }

        private async Task ReplaceXConnectHost(byte[] content, HttpContext context)
        {
            var stringContent = Encoding.UTF8.GetString(content);
            var newContent = stringContent.Replace(xConnectHost, context.Request.Host.Value);
            logger.LogInformation($"Current content-length is {context.Response.Headers["content-length"]}. New content length is {newContent.Length}");

            //context.Response.ContentLength = newContent.Length;
            context.Response.Headers.Remove("content-length");

            await context.Response.WriteAsync(newContent, Encoding.UTF8);
        }

        private bool IsContentOfType(HttpResponseMessage responseMessage, string type)
        {
            var result = false;

            if (responseMessage.Content?.Headers?.ContentType != null)
            {
                result = responseMessage.Content.Headers.ContentType.MediaType == type;
            }

            return result;
        }
    }
}