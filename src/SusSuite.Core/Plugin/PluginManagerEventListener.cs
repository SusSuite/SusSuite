using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using Impostor.Api.Innersloth;

namespace SusSuite.Core.Plugin
{
    public class PluginManagerEventListener : IEventListener
    {
        private readonly SusSuiteManager _susSuiteManager;
        private readonly SusSuiteCore _susSuiteCore;

        public PluginManagerEventListener(SusSuiteManager susSuiteManager)
        {
            _susSuiteManager = susSuiteManager;
            _susSuiteCore = susSuiteManager.GetSusSuiteCore(_susSuiteManager.ServerPlugin);
        }

        [EventListener(EventPriority.Highest)]
        public void OnGameCreated(IGameCreatedEvent e)
        {
            _susSuiteManager.PluginManager.CreateGame(e.Game.Code);
        }

        [EventListener(EventPriority.Highest)]
        public void OnGameDestroyedEvent(IGameDestroyedEvent e)
        {
            _susSuiteManager.PluginManager.RemoveGame(e.Game.Code);
        }

        [EventListener(EventPriority.Highest)]
        public void OnPlayerChat(IPlayerChatEvent e)
        {
            new Task(async () =>
            {
                if (e.Game.GameState != GameStates.NotStarted || !e.ClientPlayer.IsHost)
                {
                    return;
                }
                var command = e.Message.Split(' ');
                switch (command[0])
                {
                    case "/gamemode":
                        switch (command.Length)
                        {
                            case 1:
                                if (_susSuiteManager.PluginManager.TryGetGame(e.Game.Code, out var susSuiteGame))
                                {
                                    var msg1 = new string[4];
                                    msg1[0] = $"Current Game Mode: {susSuiteGame.SusSuitePlugin.Name}";
                                    msg1[1] = $"{susSuiteGame.SusSuitePlugin.Description}";
                                    msg1[2] = $"Made by: {susSuiteGame.SusSuitePlugin.Author}";
                                    msg1[3] = $"Version: {susSuiteGame.SusSuitePlugin.Version}";
                                    await _susSuiteCore.PluginService.SendMessageAsync(e.Game, msg1);
                                }
                                break;

                            case 2:
                                var plugin = command[1];
                                if (_susSuiteManager.PluginManager.TryGetPlugin(plugin, out var susSuitePlugin))
                                {
                                    _susSuiteManager.PluginManager.EnablePlugin(susSuitePlugin, e.Game.Code);
                                    var msg2 = new string[4];
                                    msg2[0] = $"Game Mode Changed to: {susSuitePlugin.FormattedPluginName}";
                                    msg2[1] = $"{susSuitePlugin.Description}";
                                    msg2[2] = $"Made by: {susSuitePlugin.Author}";
                                    msg2[3] = $"Version: {susSuitePlugin.Version}";
                                    await _susSuiteCore.PluginService.SendMessageAsync(e.Game, msg2);
                                }
                                else
                                {
                                    await _susSuiteCore.PluginService.SendPrivateMessageAsync(e.ClientPlayer, "Invalid Game Mode.", "Valid Game Modes: " + _susSuiteManager.PluginManager.GetGameModeList());
                                }
                                break;

                            default:

                                await _susSuiteCore.PluginService.SendPrivateMessageAsync(e.ClientPlayer, "Too Many Arguments.");
                                break;
                        }
                        break;

                    case "/gamemodes":
                        var msg = new string[1];
                        msg[0] = $"Game Modes: {_susSuiteManager.PluginManager.GetGameModeList()}";
                        await _susSuiteCore.PluginService.SendMessageAsync(e.Game, msg);
                        break;
                }
            }).Start();
        }
    }
}