using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace $safeprojectname$.App.AppCustomization
{
    public static class AppServices
    {
        public static IServiceCollection ConfigureAppServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // Application services goes there
            serviceCollection.AddSingleton<IApp, AppMain>();

            return serviceCollection;
        }
    }
}
