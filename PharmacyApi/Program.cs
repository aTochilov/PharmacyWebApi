using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace PharmacyApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel();
                    webBuilder.UseUrls("http://localhost:5000", "http://192.168.0.107:5001");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
