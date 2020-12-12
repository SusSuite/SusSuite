using SusSuite.Core.Services;

namespace SusSuite.Core
{
    public interface ISusSuiteCore
    {
        ILoggerService Logger { get; set; }
        IConfigService ConfigService { get; set; }
        string PluginName { get; set; }
    }
}