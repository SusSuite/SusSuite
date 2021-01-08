using SusSuite.Core.Services.Config;
using SusSuite.Core.Services.Logger;

namespace SusSuite.Core.Models
{
    public class SusSuitePlugin
    {

        public string Name { get; init; } = "SusSuite Plugin";
        public string Description { get; init; } = "SusSuite Plugin Desc";
        public string Author { get; init; } = "SusSuite";
        public string Version { get; init; } = "1.0.0";
        public string PluginColor { get; set; } = "[00ff00ff]";
        public PluginType PluginType { get; set; } = PluginType.GameMode;
        public string FormattedPluginName => PluginColor + Name + "[]";

    }

    public enum PluginType
    {
        GameMode,
        Extra
    }
}
