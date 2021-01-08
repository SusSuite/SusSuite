using System.Text.Json;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SusSuite.Core.Models;

namespace SusSuite.Core.Plugin
{
    public class SusSuiteCorePlugin : IPluginStartup
    {
        public void ConfigureHost(IHostBuilder host)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SusSuiteManager>();
            services.AddSingleton<IEventListener, PluginManagerEventListener>();
        }
    }

    [ImpostorPlugin(
        name: "SusSuite.Core",
        package: "SusSuite",
        author: "Gavin Steinhoff",
        version: "1.0.0")]
    public class ManagerPlugin : PluginBase
    {
        private readonly SusSuiteManager _susSuiteManager;

        public ManagerPlugin(SusSuiteManager susSuiteManager)
        {
            _susSuiteManager = susSuiteManager;
        }

        public override ValueTask EnableAsync()
        {
            _susSuiteManager.PluginManager.RegisterPlugin(_susSuiteManager.ServerPlugin);
            return default;
        }

        public override ValueTask DisableAsync()
        {
            _susSuiteManager.PluginManager.UnRegisterPlugin(_susSuiteManager.ServerPlugin);
            return default;
        }
    }
}
