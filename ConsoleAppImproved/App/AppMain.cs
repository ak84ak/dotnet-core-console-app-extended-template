using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace $safeprojectname$.App
{
    public class AppMain : IApp
    {
        private readonly ILogger<AppMain> _logger;

        public AppMain(ILogger<AppMain> logger)
        {
            _logger = logger;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Application started");

            await Task.Delay(3000);

            _logger.LogInformation("Application finished");
        }
    }
}
