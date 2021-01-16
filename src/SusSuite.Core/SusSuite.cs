using System.Text.Json;
using Microsoft.Extensions.Logging;
using SusSuite.Core.Models;
using SusSuite.Core.Services.Config;
using SusSuite.Core.Services.Logger;
using SusSuite.Core.Services.PluginManager;
using SusSuite.Core.Services.PluginService;

namespace SusSuite.Core
{
    public class SusSuiteManager
    {
        public PluginManager PluginManager { get; }
        private LoggerService LoggerService { get; }
        private ILogger<SusSuiteCore> Logger { get; }
        internal SusSuiteCore SusSuiteCore { get; }
        internal SusSuitePlugin ServerPlugin { get; }

        public SusSuiteManager(ILogger<SusSuiteCore> logger)
        {
            var susPlugin = new SusSuitePlugin()
            {
                Name = "SusSuite",
                PluginType = PluginType.Extra
            };

            Logger = logger;
            LoggerService = new LoggerService(logger, susPlugin);
            PluginManager = new PluginManager(LoggerService);
            SusSuiteCore = GetSusSuiteCore(susPlugin);

            var validation = new JsonSerializerOptions();
            validation.Converters.Add(new SusSuiteConfigPropertyConverter());

            var serverConfig = SusSuiteCore.ConfigService.GetConfig<SusSuiteConfig>("SusSuiteServer", validation);
            ServerPlugin = new SusSuitePlugin()
            {
                Name = serverConfig.ServerName,
                PluginColor = serverConfig.ServerColor,
                PluginType = PluginType.Extra
            };
        }

        public SusSuiteCore GetSusSuiteCore(SusSuitePlugin susSuitePlugin)
        {
            return new SusSuiteCore(Logger, susSuitePlugin, this);
        }
    }

    public class SusSuiteCore
    {
        public PluginService PluginService { get; }
        public ConfigService ConfigService { get; }
        public LoggerService LoggerService { get; }

        public SusSuiteCore(ILogger<SusSuiteCore> logger, SusSuitePlugin susSuitePlugin, SusSuiteManager susSuiteManager)
        {
            LoggerService = new LoggerService(logger, susSuitePlugin);
            ConfigService = new ConfigService(LoggerService, susSuitePlugin);
            PluginService = new PluginService(susSuitePlugin, susSuiteManager);
        }
    }
}
