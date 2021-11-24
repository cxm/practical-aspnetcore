using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace PracticalAspNetCore
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // The order is important. If you switch it, it won't work.
            services.AddTransient<RssReader>();
            services.AddHttpClient<IRss, RssReader>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var rss = context.RequestServices.GetService<IRss>();
                var result = await rss.Get("http://scripting.com/rss.xml");

                context.Response.Headers.Add("Content-Type", "application/rss+xml");
                await context.Response.WriteAsync(result);
            });
        }
    }

    public interface IRss
    {
        Task<string> Get(string url);
    }

    public class RssReader : IRss
    {
        readonly HttpClient _client;

        public RssReader(HttpClient client)
        {
            _client = client;
        }

        public Task<string> Get(string url) => _client.GetStringAsync(url);
    }


}