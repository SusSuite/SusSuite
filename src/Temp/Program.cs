using System;
using System.Text.Json;
using SusSuite.Core;
using SusSuite.Core.Models;
using SusSuite.Core.Services.Config;
using SusSuite.Core.Services.Logger;
using SusSuite.Core.Services.PluginManager;

namespace Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var susPlugin = new SusSuitePlugin()
            {
                Name = "SusSuite"
            };

            var configService = new ConfigService(new LoggerService(null, susPlugin), susPlugin);

            var validation = new JsonSerializerOptions();
            validation.Converters.Add(new SusSuiteConfigPropertyConverter());

            var serverConfig = configService.GetConfig<SusSuiteConfig>("SusSuiteServer", validation);
        }
    }
}
