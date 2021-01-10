using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Impostor.Api.Games;
using Microsoft.Extensions.Logging;
using SusSuite.Core.Models;
using SusSuite.Core.Services.Config;
using SusSuite.Core.Services.Logger;

namespace SusSuite.Core.Services.PluginManager
{
    public class PluginManager
    {
        public List<SusSuitePlugin> SusSuitePlugins { get; } = new();
        public List<SusSuiteGame> SusSuiteGames { get; } = new();
        public LoggerService LoggerService { get; }

        public PluginManager(LoggerService loggerService)
        {
            LoggerService = loggerService;

            SusSuitePlugins.Add(new SusSuitePlugin()
            {
                Name = "Normal",
                Description = "Just a normal game",
                Author = "Innersloth",
                Version = "Normal",
                PluginType = PluginType.GameMode
            });
        }

        public bool RegisterPlugin(SusSuitePlugin susSuitePlugin, JsonSerializerOptions jsonSerializerOptions = null)
        {
            if (TryGetPlugin(susSuitePlugin.Name, out _))
            {
                LoggerService.LogWarning("Could not register {0} | {1}.", susSuitePlugin.Name, "Duplicate Name");
                return false;
            }

            if (susSuitePlugin.Name.Any(char.IsWhiteSpace))
            {
                LoggerService.LogWarning("Could not register {0} | {1}.", susSuitePlugin.Name, "White Space in Name");
                return false;
            }

            SusSuitePlugins.Add(susSuitePlugin);

            LoggerService.LogInformation("Registered {0}.", susSuitePlugin.Name);
            return true;
        }

        public bool UnRegisterPlugin(SusSuitePlugin susSuitePlugin)
        {
            if (TryGetPlugin(susSuitePlugin.Name, out var FoundSusSuitePlugin))
            {
                LoggerService.LogInformation("Un-Registered {0}.", susSuitePlugin.Name);
                SusSuitePlugins.Remove(susSuitePlugin);
                return true;
            }

            LoggerService.LogWarning("Could not un-register {0} | {1}.", FoundSusSuitePlugin.Name, "Not Found");
            return false;
        }

        internal void EnablePlugin(SusSuitePlugin susSuitePlugin, string gameCode)
        {
            if (TryGetGame(gameCode, out var susSuiteGame))
            {
                susSuiteGame.SusSuitePlugin = susSuitePlugin;
            }
            else
            {
                SusSuiteGames.Add(new SusSuiteGame()
                {
                    GameCode = gameCode,
                    SusSuitePlugin = susSuitePlugin
                });
            }
        }

        internal void CreateGame(string gameCode)
        {
            TryGetPlugin("Normal", out var susSuitePlugin);
            SusSuiteGames.Add(new SusSuiteGame()
            {
                GameCode = gameCode,
                SusSuitePlugin = susSuitePlugin
            });
        }

        internal void RemoveGame(string gameCode)
        {
            if (TryGetGame(gameCode, out var susSuiteGame))
            {
                SusSuiteGames.Remove(susSuiteGame);
            }
        }

        public bool TryGetPlugin(string pluginName, out SusSuitePlugin susSuitePlugin)
        {
            if (SusSuitePlugins.Any(p => string.Equals(p.Name, pluginName, StringComparison.CurrentCultureIgnoreCase)))
            {
                susSuitePlugin = SusSuitePlugins.First(p => string.Equals(p.Name, pluginName, StringComparison.CurrentCultureIgnoreCase));
                return true;
            }
            susSuitePlugin = null;
            return false;
        }

        public SusSuitePlugin GetPlugin(string pluginName)
        {
            TryGetPlugin(pluginName, out var susSuitePlugin);
            return susSuitePlugin;
        }

        public bool TryGetGame(string gameCode, out SusSuiteGame susSuiteGame)
        {

            if (SusSuiteGames.Any(g => string.Equals(g.GameCode, gameCode, StringComparison.CurrentCultureIgnoreCase)))
            {
                susSuiteGame = SusSuiteGames.First(g => string.Equals(g.GameCode, gameCode, StringComparison.CurrentCultureIgnoreCase));
                return true;
            }
            susSuiteGame = null;
            return false;
        }

        public SusSuiteGame GetGame(string gameCode)
        {
            TryGetGame(gameCode, out var game);
            return game;
        }

        public bool IsGameModeEnabled(SusSuitePlugin susSuitePlugin, IGame game)
        {
            if (TryGetGame(game.Code, out var susSuiteGame))
            {
                return susSuiteGame.SusSuitePlugin == susSuitePlugin;
            }
            return false;
        }

        internal string GetGameModeList()
        {
            return SusSuitePlugins.Where(p => p.PluginType == PluginType.GameMode).Select(p => p.Name).Aggregate((i, j) => i + "\n" + j);
        }
    }
}
