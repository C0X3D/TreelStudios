using Treel.Games.CrashApi.DataLayer;

namespace Treel.Games.CrashApi.GameManager.SignalR.Interfaces
{
    public interface ICrashHub
    {
        public Task UserConnectedAsync(object data);
        public Task MultiplierUpdateAsync(double multiplier);
        public Task GameCrashedAsync(double multiplier, string serverSeed, string roundNumber);
        public Task StartInAsync(int seconds);
        public Task LastCallAsync(int seconds);
        public Task ui_UpdateUserBetAsync(object liveBets);

        public Task PlayerCashedOutAsync(object liveBets);
        public Task RecieveMessageAsync(LiveMessage message);
        public Task HandshakeMessageAsync(object message);
        public Task UpdatePlayerBalanceAsync(double model);
    }
}
