using System.IO;
using Microsoft.Extensions.Logging;

namespace SusSuite.Core.Services
{
    public interface IConfigService
    {
        string PluginName { get; set; }
        LogLevel DefaultLogLevel { get; set; }
        T GetConfig<T>(string fileName) where T : new();
        T GetConfig<T>() where T : new();
        bool TryGetDataFolder(out DirectoryInfo directoryInfo);
    }
}