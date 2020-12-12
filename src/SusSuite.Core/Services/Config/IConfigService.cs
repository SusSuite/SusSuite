using System.IO;

namespace SusSuite.Core.Services
{
    public interface IConfigService
    {
        string PluginName { get; set; }

        T GetConfig<T>(string fileName) where T : new();
        bool TryGetDataFolder(out DirectoryInfo directoryInfo);
    }
}