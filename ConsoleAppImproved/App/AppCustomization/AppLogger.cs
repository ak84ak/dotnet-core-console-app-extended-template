using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Configuration;
using Serilog;

using $safeprojectname$.Defaults;

namespace $safeprojectname$.App.AppCustomization
{
    public static class AppLogger
    {
        public static LoggerConfiguration ConfigureAppLogger(this LoggerConfiguration loggerConfiguration, IConfiguration appConfiguration)
        {
            // Custom configuration goes there

            loggerConfiguration.AddDefaultLogger(appConfiguration);

            return loggerConfiguration;
        }
    }
}
