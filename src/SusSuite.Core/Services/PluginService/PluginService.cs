﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net;
using SusSuite.Core.Models;

namespace SusSuite.Core.Services.PluginService
{
    public class PluginService
    {
        private readonly SusSuitePlugin _susSuitePlugin;
        private readonly SusSuiteManager _susSuiteManager;
        private readonly Dictionary<string, object> _data;

        public PluginService(SusSuitePlugin susSuitePlugin, SusSuiteManager susSuiteManager)
        {
            _susSuitePlugin = susSuitePlugin;
            _susSuiteManager = susSuiteManager;
            _data = new Dictionary<string, object>();
        }

        public bool IsGameModeEnabled(IGame game)
        {
            return _susSuiteManager.PluginManager.IsGameModeEnabled(_susSuitePlugin, game);
        }

        public bool TryGetData<T>(IGame game, out T data)
        {
            if (_susSuiteManager.PluginManager.TryGetGameData<T>(game, out var output))
            {
                data = output;
                return true;
            }
            data = default;
            return false;
        }

        public void SetData<T>(IGame game, T data)
        {
            _susSuiteManager.PluginManager.SetGameData(game, data);
        }

        /// <summary>
        /// Sends a public message with in-game chat to a game
        /// </summary>
        /// <param name="game">The game you want to send the message to</param>
        /// <param name="messages">The message to send</param>
         /// <returns></returns>
        public async Task SendMessageAsync(IGame game, params string[] messages)
        {
            if (game.Host.Character != null)
            {
                var name = game.Host.Character.PlayerInfo.PlayerName;
                var message = string.Join("\n\n", messages);
                await game.Host.Character.SetNameAsync(_susSuitePlugin.FormattedPluginName);
                await game.Host.Character.SendChatAsync(message);
                await game.Host.Character.SetNameAsync(name);
            }
        }

        /// <summary>
        /// Sends a private message to a player with in-game chat
        /// </summary>
        /// <param name="toPlayer">The Player to receive the message</param>
        /// <param name="messages">The message to send</param>
        /// <returns></returns>
        public async Task SendPrivateMessageAsync(IClientPlayer toPlayer, params string[] messages)
        {
            if (toPlayer.Character != null)
            {
                var name = toPlayer.Character.PlayerInfo.PlayerName;
                var message = string.Join("\n\n", messages);
                await toPlayer.Character.SetNameAsync(_susSuitePlugin.FormattedPluginName);
                await toPlayer.Character.SendChatToPlayerAsync(message);
                await toPlayer.Character.SetNameAsync(name);
            }
        }

        /// <summary>
        /// Provides a fun way to kill everyone and end the game.
        /// </summary>
        /// <param name="game">The IGame you want to end</param>
        /// <param name="message">The win message</param>
        /// <param name="timeToWait">How many milliseconds to wait before the celebration starts</param>
        /// <param name="winType">Choose the default among us game over screen</param>
        public void EndGame(IGame game, string message, int timeToWait, WinType winType = WinType.CrewMate, Vector2 snapTo = default)
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
                    if (snapTo != default)
                    {
                        await p.Character.NetworkTransform.SnapToAsync(snapTo);
                    }
                });

                var r = new Random();
                for (var i = 0; i < 10; i++)
                {
                    game.Players.ToList().ForEach(async p =>
                    {
                        if (p.Character != null) await p.Character.SetColorAsync((ColorType)r.Next(0, 6));
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
