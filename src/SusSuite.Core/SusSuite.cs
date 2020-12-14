using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SusSuite.Core.Services;

namespace SusSuite.Core
{
    public abstract class SusSuiteCore : ISusSuiteCore
    {
        public ILoggerService Logger { get; set; }
        public IConfigService ConfigService { get; set; }
        private LogLevel _configServiceDefaultLogLevel;
        public LogLevel ConfigServiceDefaultLogLevel 
        {
            get
            {
                return _configServiceDefaultLogLevel;
            }
            set
            {
                _configServiceDefaultLogLevel = value;
                ConfigService.DefaultLogLevel = value;
            }
        }

        public string _pluginName;
        public string PluginName
        {
            get => _pluginName;
            set
            {
                _pluginName = value;
                Logger.PluginName = value;
                ConfigService.PluginName = value;
            }
        }

        public SusSuiteCore(ILogger<SusSuiteCore> logger, JsonSerializerOptions jsonSerializerOptions)
        {
            Logger = new LoggerService(logger);
            ConfigService = new ConfigService(Logger, jsonSerializerOptions, ConfigServiceDefaultLogLevel);

            PluginName = "DEFAULT_PLUGIN_NAME";
        }

    }

}
