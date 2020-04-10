using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BackgardenBlazor
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        public static int Main(string[] args)
        {
            IHost host = null;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.WithProperty("Backgarden", "FlintSoft Backgarden Sprinklers")
                .WriteTo.Async(a => a.RollingFile("log/backg-{Date}.txt"))
                .WriteTo.ColoredConsole()
                .CreateLogger();

#if Windows
            Console.WriteLine("Windows");
#endif
#if Linux
            Console.WriteLine("Linux");
#endif
#if OSX
            Console.WriteLine("OSX");
#endif

            try
            {
                Log.Information("Building web host...");
                host = CreateHostBuilder(args).Build();

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            return 0;

            //CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
