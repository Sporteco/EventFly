using Demo.Db;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;

namespace Demo.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // migrations todo
            using (var st = new TestDbContext())
            {
                try
                {
                    st.Database.Migrate();
                } catch(Exception ex) { }
            }

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
