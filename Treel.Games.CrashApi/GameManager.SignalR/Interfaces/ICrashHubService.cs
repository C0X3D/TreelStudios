using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using Treel.Games.CrashApi.DataLayer;
using TreelGamesDataModel;

namespace Treel.Games.CrashApi.GameManager.SignalR.Interfaces
{
    public interface ICrashHubService
    {
        Task StartGame();
        Task SendToAll(string message);
        Task<CrashBetResponse> PlaceBetAsync(CrashBetRequest betRequest);
        Task<CrashCashOutResponse> CashOutAsync(CrashCashOutRequest cashOutRequest);
        object ProvablyFair(string serverSeed, int roundNumber);

        Task RecieveMessage(LiveMessage liveMessage);

        ConcurrentDictionary<string, double> Bets { get; }
    }
}
