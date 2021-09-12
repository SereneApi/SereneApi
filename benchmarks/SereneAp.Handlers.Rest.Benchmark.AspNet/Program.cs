using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SereneAp.Handlers.Rest.Benchmark.AspNet;

namespace SereneApi.Benchmark.AspNet
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}