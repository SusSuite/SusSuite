using System;
using Microsoft.Extensions.Logging;
using SusSuite.Core.Models;

namespace SusSuite.Core.Services.Logger
{
    public class LoggerService
    {
        private const string Id = "SusSuite";
        private readonly ILogger _logger;
        private readonly SusSuitePlugin _plugin;

        public LoggerService(ILogger logger, SusSuitePlugin susSuitePlugin)
        {
            _logger = logger;
            _plugin = susSuitePlugin;
        }

        private string MakeMessage(string message)
        {
            return "{Id} => {_pluginName} => " + message;
        }
        private object[] MakeArgs(params object[] args)
        {
            var newArgs = new object[args.Length + 2];
            newArgs[0] = Id;
            newArgs[1] = _plugin.Name;
            args.CopyTo(newArgs, 2);
            return newArgs;
        }

        private void WriteLog(LogLevel logLevel, Exception ex, string message, params object[] args)
        {
            _logger.Log(logLevel, ex, MakeMessage(message), MakeArgs(args));
        }

        private void WriteLog(LogLevel logLevel, string message, params object[] args)
        {
            _logger.Log(logLevel, MakeMessage(message), MakeArgs(args));
        }

        public void LogInformation(string message, params object[] args) => WriteLog(LogLevel.Information, message, args);
        public void LogInformation(Exception ex, string message, params object[] args) => WriteLog(LogLevel.Information, ex, message, args);
        public void LogDebug(string message, params object[] args) => WriteLog(LogLevel.Debug, message, args);
        public void LogDebug(Exception ex, string message, params object[] args) => WriteLog(LogLevel.Debug, ex, message, args);
        public void LogWarning(string message, params object[] args) => WriteLog(LogLevel.Warning, message, args);
        public void LogWarning(Exception ex, string message, params object[] args) => WriteLog(LogLevel.Warning, ex, message, args);
        public void LogError(string message, params object[] args) => WriteLog(LogLevel.Error, message, args);
        public void LogError(Exception ex, string message, params object[] args) => WriteLog(LogLevel.Error, ex, message, args);
        public void LogTrace(string message, params object[] args) => WriteLog(LogLevel.Trace, message, args);
        public void LogTrace(Exception ex, string message, params object[] args) => WriteLog(LogLevel.Trace, ex, message, args);
        public void Log(LogLevel logLevel, string message, params object[] args) => WriteLog(logLevel, message, args);
        public void Log(LogLevel logLevel, Exception ex, string message, params object[] args) => WriteLog(logLevel, ex, message, args);
    }
}
