using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Configuration;

namespace $safeprojectname$.App.AppCustomization
{
    public static class AppConfiguration
    {
        public static IConfigurationBuilder AddAppConfiguration(this IConfigurationBuilder configuration)
        {
            // Add custom configuration there

            return configuration;
        }
    }
}
