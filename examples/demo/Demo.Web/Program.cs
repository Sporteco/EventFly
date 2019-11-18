using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Demo.Web
{
    public class Program
    {
        public static void Main(System.String[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(System.String[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
