﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net;
using SusSuite.Core.Models;
using SusSuite.Core.Services.Config;

namespace SusSuite.Core.Services.PluginService
{
    public class PluginService
    {

        private readonly SusSuitePlugin SusSuitePlugin;

        public PluginService(SusSuitePlugin susSuitePlugin)
        {
            SusSuitePlugin = susSuitePlugin;
        }

        public async Task SendMessageAsync(IGame game, params string[] messages)
        {
            if (game.Host.Character != null)
            {
                var name = game.Host.Character.PlayerInfo.PlayerName;
                var message = string.Join("\n\n", messages);
                await game.Host.Character.SetNameAsync(SusSuitePlugin.FormattedPluginName);
                await game.Host.Character.SendChatAsync(message);
                await game.Host.Character.SetNameAsync(name);
            }
        }

        public async Task SendPrivateMessageAsync(IGame game, IClientPlayer toPlayer, params string[] messages)
        {
            if (toPlayer.Character != null)
            {
                var name = toPlayer.Character.PlayerInfo.PlayerName;
                var message = string.Join("\n\n", messages);
                await toPlayer.Character.SetNameAsync(SusSuitePlugin.FormattedPluginName);
                await toPlayer.Character.SendChatToPlayerAsync(message);
                await toPlayer.Character.SetNameAsync(name);
            }
        }

        public void EndGame(IGame game, string message, int timeToWait, WinType winType = WinType.CrewMate)
        {
            var aliveCrewMates = game.Players.Where(p => p.Character?.PlayerInfo.IsImpostor == false && !p.Character.PlayerInfo.IsDead).ToList();
            var aliveImpostors = game.Players.Where(p => p.Character?.PlayerInfo.IsImpostor == true && !p.Character.PlayerInfo.IsDead).ToList();

            new Task(() =>
            {
                Thread.Sleep(timeToWait);

                game.Players.ToList().ForEach(async p =>
                {
                    if (p.Character == null) return;
                    await p.Character.SetNameAsync(message);
                    await p.Character.SetHatAsync(HatType.PartyHat);
                });

                var r = new Random();
                for (var i = 0; i < 10; i++)
                {
                    game.Players.ToList().ForEach(async p =>
                    {
                        if (p.Character != null) await p.Character.SetColorAsync((ColorType) r.Next(0, 6));
                    });
                    Thread.Sleep(1000);
                }

                switch (winType)
                {
                    case WinType.CrewMate:
                        aliveImpostors.ForEach(c => c.Character?.SetMurderedByAsync(aliveImpostors.Last()));
                        break;
                    case WinType.Impostor:
                        aliveCrewMates.ForEach(c => c.Character?.SetMurderedByAsync(aliveImpostors.Last()));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(winType), winType, null);
                }
            }).Start();
        }

        public enum WinType
        {
            CrewMate,
            Impostor
        }

    }
}