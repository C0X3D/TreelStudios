using Cox.SignalRDataModel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Treel.Games.CrashApi.DataLayer;
using Treel.Games.CrashApi.GameManager.SignalR.Interfaces;
using TreelGamesDataModel;

namespace Treel.Games.CrashApi.GameManager.SignalR
{
    public class CrashHubsService(IHubContext<CrashHub, ICrashHub> crashHub,
IClientManagementService clientManagement) : ICrashHubService
    {
        public static string GameId = "treel-games-crashapi";
        public static double Multiplier = 0.0;
        public static bool GameRunning = false;
        private static string ServerSeed = "random_server_seed";
        private static int RoundNumber = 1;
        private static readonly object LockObject = new object();
        private readonly IHubContext<CrashHub, ICrashHub> _crashHub = crashHub;
        private readonly IClientManagementService _clientManagement = clientManagement;
        private ConcurrentDictionary<string, double> Bets { get ; } = new ConcurrentDictionary<string, double>();
        
        ConcurrentDictionary<string, double> ICrashHubService.Bets => throw new NotImplementedException();

        public async Task StartGame()
        {            
            lock (LockObject)
            {
                if (GameRunning)
                    return;
                GameRunning = true;
            }

            ServerSeed = Guid.NewGuid().ToString();
            Multiplier = 1.0;
            double crashPoint = GetCrashMultiplier(ServerSeed, RoundNumber);
            RoundNumber++;          

            while (Multiplier < crashPoint)
            {
                await Task.Delay(100);
                Multiplier += Math.Clamp(Multiplier * 0.005, 0.01, 10);
                await _crashHub.Clients.All.MultiplierUpdateAsync(Multiplier);
            }
            Bets.Clear();
            await _crashHub.Clients.All.GameCrashedAsync(Multiplier, ServerSeed, RoundNumber.ToString());            
            // Start a new game after 10 seconds
            _ = Task.Run(async () =>
            {
                Console.WriteLine("Crash Done " + Multiplier);
                
                await _crashHub.Clients.All.StartInAsync(10);
                await Task.Delay(10000);
                await _crashHub.Clients.All.LastCallAsync(3);
                await Task.Delay(3000);
                GameRunning = false;
                await StartGame();
            });

            GameRunning = false;
        }


        private double GetCrashMultiplier(string serverSeed, int roundNumber)
        {
            double rounder = 100000;
            double rounderStealer = rounder + 1;
            double biasFactor = 1.02; // Factor de ajustare pentru a favoriza numere mai mici

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(serverSeed)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(roundNumber.ToString()));
                int result = BitConverter.ToInt32(hash, 0) & int.MaxValue;

                // Aplicăm bias-ul pentru a favoriza valori mai mici
                double adjustedResult = result % (rounder * biasFactor);
                return Math.Max(1.00, Math.Min(10000.0, rounder / (rounderStealer - adjustedResult)));
            }
        }

        public Task SendToAll(string message)
        {
            throw new NotImplementedException();
        }

        public async Task<CrashBetResponse> PlaceBetAsync(CrashBetRequest betRequest)
        {
            if (!GameRunning)
            {
                if (Bets.ContainsKey(betRequest.UserId))
                {
                    return new CrashBetResponse() { Success = CrashBetResponseStatus.DuplicateBet };
                }
                if (Bets.TryAdd(betRequest.UserId, betRequest.Amount))
                {
                    await _crashHub.Clients.All.ui_UpdateUserBetAsync(new
                    {
                        amount = betRequest.Amount,
                        userName = betRequest.UserName
                    });
                    return new CrashBetResponse() { Success = CrashBetResponseStatus.Success };
                }
            }

            return new CrashBetResponse() { Success = CrashBetResponseStatus.Failed };
        }

        public async Task<CrashCashOutResponse> CashOutAsync(CrashCashOutRequest cashOutRequest)
        {
            if (Bets.ContainsKey(cashOutRequest.SessionToken) && GameRunning)
            {
                double cashOutAmount = Bets[cashOutRequest.SessionToken] * Multiplier;
                Bets.TryRemove(cashOutRequest.SessionToken, out _);
                await _crashHub.Clients.All.PlayerCashedOutAsync(new { userName = cashOutRequest.UserName, amount = cashOutAmount });
                return new CrashCashOutResponse()
                {
                    GameId = GameId,
                    UserId = cashOutRequest.SessionToken,
                    Multiplier = cashOutAmount,                    
                };
            }
            //await CrashHub.SendToAll(cashOutRequest);
            return new CrashCashOutResponse()
            {
                Multiplier = 0,
            };
        }

        public object ProvablyFair(string serverSeed, int roundNumber)
        {
            return GetCrashMultiplier(serverSeed, roundNumber);
        }

        public async Task RecieveMessage(LiveMessage liveMessage)
        {
            await _crashHub.Clients.All.RecieveMessageAsync(liveMessage);
        }
    }
}
