using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OddJob.Example
{
    public class WebServer : IJob
    {
        private readonly IConfiguration configuration;
        private readonly ILoggerFactory loggerFactory;

        public WebServer(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.loggerFactory = loggerFactory;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var webHostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(this.configuration)
                .UseLoggerFactory(this.loggerFactory)
                .UseStartup<Startup>();

            using (var host = webHostBuilder.Build())
            {
                host.Run(cancellationToken);
            }

            await Task.CompletedTask;
        }

        internal class Startup
        {
            public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            }
        }
    }
}