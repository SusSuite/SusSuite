using Microsoft.Extensions.Logging;
using System;

namespace SusSuite.Core.Services
{
    public interface ILoggerService
    {
        string PluginName { get; set; }

        void Log(LogLevel logLevel, Exception ex, string message, params object[] args);
        void Log(LogLevel logLevel, string message, params object[] args);
        void LogDebug(Exception ex, string message, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogError(Exception ex, string message, params object[] args);
        void LogError(string message, params object[] args);
        void LogInformation(Exception ex, string message, params object[] args);
        void LogInformation(string message, params object[] args);
        void LogTrace(Exception ex, string message, params object[] args);
        void LogTrace(string message, params object[] args);
        void LogWarning(Exception ex, string message, params object[] args);
        void LogWarning(string message, params object[] args);
    }
}