using System;
using System.Linq;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Meeting;
using Impostor.Api.Events.Player;
using SusSuite.Core;
using SusSuite.Core.Models;
using SusSuite.Core.Services.PluginService;
using SusSuite.Plugins.Jester.Models;

namespace SusSuite.Plugins.Jester
{
    public class JesterEventListener : IEventListener
    {
        private readonly SusSuiteManager _susSuiteManager;
        private readonly SusSuitePlugin _susSuiteCorePlugin;
        private readonly SusSuiteCore _susSuiteCore;

        public JesterEventListener(SusSuiteManager susSuiteManager)
        {
            _susSuiteManager = susSuiteManager;
            _susSuiteCorePlugin = _susSuiteManager.PluginManager.GetPlugin("Jester");
            _susSuiteCore = susSuiteManager.GetSusSuiteCore(_susSuiteCorePlugin);
        }

        [EventListener]
        public void OnGameStarted(IGameStartedEvent e)
        {
            if (!_susSuiteManager.PluginManager.IsGameModeEnabled(_susSuiteCorePlugin, e.Game.Code)) return;

            var r = new Random();

            var jesterData = new JesterData();

            _susSuiteManager.PluginManager.TryGetGame(e.Game.Code, out var game);

            var crewMates = e.Game.Players.Where(p => p.Character?.PlayerInfo.IsImpostor == false).ToList();

            jesterData.JesterId = crewMates.ElementAt(r.Next(0, crewMates.Count)).Client.Id;

            game.SetData(jesterData);
        }

        [EventListener]
        public void OnMeetingStarted(IMeetingStartedEvent e)
        {
            if (!_susSuiteManager.PluginManager.IsGameModeEnabled(_susSuiteCorePlugin, e.Game.Code)) return;

            _susSuiteManager.PluginManager.TryGetGame(e.Game.Code, out var game);
            game.TryGetData<JesterData>(out var jesterData);

            if (jesterData.BeenNotified) return;
            jesterData.BeenNotified = true;
            game.SetData(jesterData);

            var jester = e.Game.Players.First(p => p.Client.Id == jesterData.JesterId);

            new Task(async () =>
            {
                System.Threading.Thread.Sleep(5000);
                await _susSuiteCore.PluginService.SendPrivateMessageAsync(e.Game, jester, "pssst...", "You are the Jester", "Get Voted Out ;)");
            }).Start();
        }

        [EventListener]
        public void OnPlayerExiled(IPlayerExileEvent e)
        {
            if (!_susSuiteManager.PluginManager.IsGameModeEnabled(_susSuiteCorePlugin, e.Game.Code)) return;

            _susSuiteManager.PluginManager.TryGetGame(e.Game.Code, out var game);
            game.TryGetData<JesterData>(out var jesterData);

            if (jesterData.JesterId == e.PlayerControl.OwnerId)
            {
                _susSuiteCore.PluginService.EndGame(e.Game, "Jester Wins!", 11000, PluginService.WinType.Impostor);
            }
        }
    }
}