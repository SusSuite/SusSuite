using System;
using System.IO;
using System.Text.Json;
using SusSuite.Core.Models;
using SusSuite.Core.Services.Logger;

namespace SusSuite.Core.Services.Config
{
    public class ConfigService
    {
        private readonly LoggerService _loggerService;
        private readonly SusSuitePlugin _plugin;

        public ConfigService(LoggerService loggerService, SusSuitePlugin plugin)
        {
            _loggerService = loggerService;
            _plugin = plugin;
        }

        public bool TryGetDataFolder(out DirectoryInfo directoryInfo)
        {
            directoryInfo = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "plugins", _plugin.Name));
            if (directoryInfo.Exists)
            {
                return true;
            }

            if (TryCreateDirectory(directoryInfo, out var directory))
            {
                directoryInfo = directory;
                return true;
            }

            directoryInfo = null;
            return false;
        }

        public T GetConfig<T>(string fileName, JsonSerializerOptions jsonSerializerOptions = null) where T : new()
        {
            if (!TryGetDataFolder(out var directoryInfo)) return new T();

            var file = new FileInfo(Path.Combine(directoryInfo.FullName, fileName + ".json"));

            if (file.Exists)
            {
                jsonSerializerOptions ??= new JsonSerializerOptions();
                if (TryReadFile(file, jsonSerializerOptions, out T userConfig))
                {
                    _loggerService.LogDebug("Read config file");
                    return userConfig;
                }

                if (TryCreateBrokenFile(file))
                {
                    _loggerService.LogDebug($"Config file broken, saving copy at {file.Name}");
                }

                if (TryCreateFile(file, jsonSerializerOptions, out T defaultConfig))
                {
                    _loggerService.LogDebug("Returning Default Config.");
                }

                return defaultConfig;
            }
            else
            {
                if (TryCreateFile(file, jsonSerializerOptions, out T defaultConfig))
                {
                    _loggerService.LogDebug("Creating config file");
                }
                return defaultConfig;
            }

        }

        public T GetConfig<T>(JsonSerializerOptions jsonSerializerOptions = null) where T : new()
        {
            return GetConfig<T>(_plugin.Name, jsonSerializerOptions);
        }

        private bool TryCreateDirectory(FileSystemInfo directoryInfo, out DirectoryInfo directory)
        {
            try
            {
                directory = Directory.CreateDirectory(directoryInfo.FullName);
                return true;
            }
            catch (Exception ex)
            {
                _loggerService.LogWarning(ex, "Could not create data folder.");
                directory = null;
                return false;
            }
        }

        private bool TryCreateFile<T>(FileInfo fileInfo, JsonSerializerOptions jsonSerializerOptions, out T config) where T : new()
        {
            config = new T();
            try
            {
                fileInfo.Directory?.Create();
                File.WriteAllText(fileInfo.FullName, JsonSerializer.Serialize(config,jsonSerializerOptions));
                return true;
            }
            catch (Exception ex)
            {
                _loggerService.LogWarning(ex, "Could not create config file.");
                return false;
            }
        }

        private bool TryCreateBrokenFile(FileInfo fileInfo)
        {
            var brokenFileName = $"{Path.GetFileNameWithoutExtension(fileInfo.Name)}-{DateTime.Now:yyyy-MM-dd_hh-mm-ss-tt}.broken.json";
            if (fileInfo.Directory == null) return false;
            var brokenFileFullName = Path.Combine(fileInfo.Directory.FullName, brokenFileName);
            try
            {
                fileInfo.CopyTo(brokenFileFullName, true);
                return true;
            }
            catch (Exception ex)
            {
                _loggerService.LogWarning(ex, "Could not create broken config file.");
                return false;
            }
        }

        private bool TryReadFile<T>(FileSystemInfo fileInfo, JsonSerializerOptions jsonSerializerOptions, out T config)
        {
            try
            {
                using var r = new StreamReader(fileInfo.FullName);
                var json = r.ReadToEnd();
                config = JsonSerializer.Deserialize<T>(json, jsonSerializerOptions);
                return true;
            }
            catch (Exception ex)
            {
                _loggerService.LogWarning(ex, "Could not read config file.");
                config = default(T);
                return false;
            }
        }
    }
}
