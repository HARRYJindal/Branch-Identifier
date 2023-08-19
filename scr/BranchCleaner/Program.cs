using System;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace BranchCleaner
{
    public class Program
    {
        public static void Main(String[] args)
        {
            Console.WriteLine("Starting of your application");
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            CreateHostBuilder(args).Build().Run();
        }

        //  create and configure web host with startup class methods also 
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults((WebHostBuilder) =>
            {
                WebHostBuilder.UseStartup<Startup>();
            }).ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            })
      .UseNLog();
        }
    }
}