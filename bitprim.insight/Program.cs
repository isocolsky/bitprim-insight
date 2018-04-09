﻿using System;
using System.Globalization;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace bitprim.insight
{
    public class Program
    {
        private const int DEFAULT_PORT = 1549;

        public static void Main(string[] args)
        {
            ConfigureLogging();
            try
            {
                var config = GetServerPortFromCommandLine(args);
                var serverPort = config.GetValue<int>("server.port",DEFAULT_PORT);
                var host = new WebHostBuilder()
                    .UseKestrel(options => {
                        options.Listen(IPAddress.Loopback, serverPort);
                    })
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseSerilog()
                    .UseIISIntegration()
                    .UseStartup<Startup>()
                    .Build();

                host.Run();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IConfigurationRoot GetServerPortFromCommandLine(string[] args)
        {
            return new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();
        }

        private static void ConfigureLogging()
        {
            var timeZone = DateTimeOffset.Now.ToString("%K").Replace(CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator, "");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty(LogPropertyNames.TIME_ZONE, timeZone)
                .WriteTo.Console(outputTemplate:
                    "{" + LogPropertyNames.SOURCE_IP + "} " +
                    "{" + LogPropertyNames.USER_ID +  "} {" + LogPropertyNames.USER_NAME  + "} " +
                    "[{Timestamp:dd/MMM/yyyy HH:mm:ss} {" + LogPropertyNames.TIME_ZONE + "}] {Level:u3} " +
                    "\"{" + LogPropertyNames.HTTP_METHOD + "} " +
                    "{" + LogPropertyNames.HTTP_REQUEST_URL + "} " +
                    "{" + LogPropertyNames.HTTP_PROTOCOL_VERSION + "}\" " +
                    "{" + LogPropertyNames.HTTP_RESPONSE_STATUS_CODE + "} " +
                    "{" + LogPropertyNames.HTTP_RESPONSE_LENGTH + "} " +
                    "{Message:lj}{NewLine}{Exception}")
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .CreateLogger();
        }
    }
}
