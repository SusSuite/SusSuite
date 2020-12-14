using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace SusSuite.Core.Services
{
    public class ConfigService : IConfigService
    {
        public string PluginName { get; set; }
        public LogLevel DefaultLogLevel { get; set; }

        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILoggerService _loggerService;

        public ConfigService(ILoggerService loggerService, JsonSerializerOptions jsonSerializerOptions, LogLevel defaultLogLevel)
        {
            _loggerService = loggerService;
            _jsonSerializerOptions = jsonSerializerOptions;
            DefaultLogLevel = defaultLogLevel;
        }

        public bool TryGetDataFolder(out DirectoryInfo directoryInfo)
        {
            directoryInfo = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "plugins", PluginName));
            if (directoryInfo.Exists)
            {
                return true;
            }
            else
            {
                if (TryCreateDirectory(directoryInfo, out var directory))
                {
                    directoryInfo = directory;
                    return true;
                }
                else
                {
                    directoryInfo = null;
                    return false;
                }
            }
        }

        public T GetConfig<T>(string fileName) where T : new()
        {
            if (TryGetDataFolder(out var directoryInfo))
            {
                FileInfo file = new FileInfo(Path.Combine(directoryInfo.FullName, fileName + ".json"));

                if (file.Exists)
                {
                    if (TryReadFile(file, out T userConfig))
                    {
                        return userConfig;
                    }
                    else
                    {
                        TryCreateBrokenFile(file);
                        TryCreateFile(file, out T defaultConfig);
                        return defaultConfig;
                    }
                }
                else
                {
                    TryCreateFile(file, out T defaultConfig);
                    return defaultConfig;
                }
            }
            else
            {
                return new T();
            }
        }

        public T GetConfig<T>() where T : new()
        {
            return GetConfig<T>(PluginName);
        }

        private bool TryCreateDirectory(DirectoryInfo directoryInfo, out DirectoryInfo directory)
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

        private bool TryCreateFile<T>(FileInfo fileInfo, out T config) where T : new()
        {
            config = new T();
            try
            {
                _loggerService.Log(DefaultLogLevel, $"Creating config file: {fileInfo.FullName}");
                fileInfo.Directory.Create();
                File.WriteAllText(fileInfo.FullName, JsonSerializer.Serialize(config, _jsonSerializerOptions));
                File.WriteAllText(fileInfo.FullName, JsonSerializer.Serialize(config));
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
            var brokenFileName = $"{Path.GetFileNameWithoutExtension(fileInfo.Name)}-{DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt")}.broken.json";
            var brokenFileFullName = Path.Combine(fileInfo.Directory.FullName, brokenFileName);
            try
            {
                _loggerService.Log(DefaultLogLevel, $"Config file broken, saving copy at {brokenFileFullName}");
                fileInfo.CopyTo(brokenFileFullName, true);
                return true;
            }
            catch (Exception ex)
            {
                _loggerService.LogWarning(ex, "Could not create broken config file.");
                return false;
            }
        }

        private bool TryReadFile<T>(FileInfo fileInfo, out T config)
        {
            _loggerService.Log(DefaultLogLevel, $"Reading Config File: {fileInfo.FullName}");
            try
            {
                using (StreamReader r = new StreamReader(fileInfo.FullName))
                {
                    string json = r.ReadToEnd();
                    config = JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
                    config = JsonSerializer.Deserialize<T>(json);
                    return true;
                }
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
