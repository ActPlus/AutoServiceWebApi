using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebService
{
    public class Program
    {
        /// <summary>
        /// Default startup method.
        /// Runs WebHost.
        /// </summary>
        /// <param name="args"> Arguments passed when launching</param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        
        /// <summary>
        /// Configuration of WebHost.
        /// Adding Logger, configuration from json
        /// Defining RootContainer
        /// </summary>
        /// <param name="args">Arguments from console</param>
        /// <returns>Configured WebHost Builder</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging((hostingContext, logging) =>
            {
                //Loading configuration for Logger
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                //Setting additional configuration for Logger
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventSourceLogger();
            })
            .ConfigureAppConfiguration(
            (WebHostBuilderContext context, IConfigurationBuilder builder) =>
            {
                builder.Sources.Clear();
                builder
                    .AddEnvironmentVariables()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .UseContentRoot(Directory.GetCurrentDirectory()) //Definition required because Docker doesn't have static paths.
            .UseStartup<Startup>();
    }
}
