using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

using $safeprojectname$.Infrastructure;

namespace $safeprojectname$.Defaults
{
    public static class LoggerDefaults
    {
        private static readonly string DefaultFileLogTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3} {ThreadId:D4}]\t{Message:lj}{NewLine}{Exception}";

        public static LoggerConfiguration AddDefaultLogger(this LoggerConfiguration loggerConfiguration, IConfiguration configuration)
        {
            var minLogLevel = configuration.GetValue("Log:MinLevel", LogEventLevel.Information);
            loggerConfiguration.MinimumLevel.Is(minLogLevel);

            loggerConfiguration.Enrich.FromLogContext();
            loggerConfiguration.Enrich.WithThreadId();
            loggerConfiguration.ReadFrom.Configuration(configuration);

            AddConsole(loggerConfiguration, configuration);
            AddTrace(loggerConfiguration, configuration);
            AddFile(loggerConfiguration, configuration);

            return loggerConfiguration;
        }

        private static void AddConsole(LoggerConfiguration loggerConfiguration, IConfiguration configuration)
        {
            if (configuration.GetValue("Log:Console:Enabled", false))
            {
                var minLogLevel = configuration.GetValue("Log:Console", LogEventLevel.Information);
                var excludeRulesArray = configuration.GetSection("Log:Console:ExcludeFilter").Get<string[]>();
                var excludeRules = ParseExcludeRules("Console", excludeRulesArray);
                loggerConfiguration.WriteTo.Logger(subLogger =>
                {
                    subLogger
                        .WriteTo.Console(minLogLevel)
                        .Filter.ByExcluding(GetFilterAction(excludeRules));
                });
            }
        }

        private static void AddTrace(LoggerConfiguration loggerConfiguration, IConfiguration configuration)
        {
            if (configuration.GetValue("Log:Trace:Enabled", false))
            {
                var minLogLevel = configuration.GetValue("Log:Trace", LogEventLevel.Information);
                var excludeRulesArray = configuration.GetSection("Log:Trace:ExcludeFilter").Get<string[]>();
                var excludeRules = ParseExcludeRules("Trace", excludeRulesArray);
                loggerConfiguration.WriteTo.Logger(subLogger =>
                {
                    subLogger
                        .WriteTo.Console(minLogLevel)
                        .Filter.ByExcluding(GetFilterAction(excludeRules));
                });
            }
        }

        private static void AddFile(LoggerConfiguration loggerConfiguration, IConfiguration configuration)
        {
            if (configuration.GetValue("Log:File:Enabled", false))
            {
                var minLogLevel = configuration.GetValue("Log:File", LogEventLevel.Information);
                var excludeRulesArray = configuration.GetSection("Log:File:ExcludeFilter").Get<string[]>();
                var excludeRules = ParseExcludeRules("File", excludeRulesArray);

                var logFilePath = configuration.GetValue("Log:File:Path", @"logs\log.txt");
                logFilePath = Environment.ExpandEnvironmentVariables(logFilePath);

                loggerConfiguration.WriteTo.Logger(subLogger =>
                {
                    subLogger
                        .WriteTo.File(
                            path: logFilePath,
                            restrictedToMinimumLevel: minLogLevel,
                            outputTemplate: configuration.GetValue("Log:File:Template", DefaultFileLogTemplate),
                            shared: configuration.GetValue("Log:File:Shared", false),
                            rollingInterval: configuration.GetValue("Log:File:RollingInterval", RollingInterval.Infinite),
                            fileSizeLimitBytes: configuration.GetValue("Log:File:FileSizeLimit", 10485760),
                            rollOnFileSizeLimit: configuration.GetValue("Log:File:RollOnFileSizeLimit", false),
                            retainedFileCountLimit: configuration.GetValue("Log:File:RetainedFileCountLimit", (int?)31),
                            buffered: configuration.GetValue("Log:File:Buffered", false),
                            flushToDiskInterval: TimeSpan.FromMilliseconds(configuration.GetValue("Log:File:FlushToDiskIntervalMs", 5000)))
                        .Filter.ByExcluding(GetFilterAction(excludeRules));
                });
            }
        }

        private static Func<LogEvent, bool> GetFilterAction(List<SerilogExcludeRule> excludeRules)
        {
            return e =>
            {
                if (excludeRules == null || excludeRules.Count == 0)
                    return false;

                var message = e.RenderMessage();
                if (excludeRules.Any(rule => rule.Regex.IsMatch(message)))
                    return true;

                return false;
            };
        }

        private static List<SerilogExcludeRule> ParseExcludeRules(string logger, string[] rules)
        {
            var result = new List<SerilogExcludeRule>();
            if (rules == null || rules.Length == 0)
                return result;

            foreach (var rule in rules)
            {
                if (rule == null || string.IsNullOrWhiteSpace(rule))
                    continue;

                Regex regex;
                try
                {
                    regex = new Regex(rule, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                }
                catch (ArgumentException e)
                {
                    Log.Logger.Error(e, "Wrong exclude rule: {logger}, {rule}", logger, rule);
                    continue;
                }

                result.Add(new SerilogExcludeRule
                {
                    Regex = regex
                });
            }

            return result;
        }
    }
}
