using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SusSuite.Core.Services;

namespace SusSuite.Core
{
    public class SusSuiteCore : ISusSuiteCore
    {
        public ILoggerService Logger { get; set; }
        public IConfigService ConfigService { get; set; }

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
            ConfigService = new ConfigService(Logger, jsonSerializerOptions);

            PluginName = "DEFAULT_PLUGIN_NAME";
        }

    }
}
