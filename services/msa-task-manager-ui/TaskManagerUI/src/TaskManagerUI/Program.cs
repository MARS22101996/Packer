using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace TaskManagerUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseUrls("http://*:5007")
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
