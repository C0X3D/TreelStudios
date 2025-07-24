using Cox.SignalRDataModel;
using Cox.SignalRDataModel.Interfaces;
using Cox.SignalRHub;
using Microsoft.AspNetCore.SignalR;
using Treel.Games.CrashApi.DataLayer;
using Treel.Games.CrashApi.GameManager.SignalR.Interfaces;

namespace Treel.Games.CrashApi.GameManager.SignalR
{

    public class CrashHub : GeneralHub<ICrashHub>
    {
        public CrashHub(IClientManagementService? clientManagement) : base(clientManagement)
        {
            OnNewClientConnected += NotificationsHub_OnNewClientConnected;
        }

        public async Task UpdateUsersBets(object userBet)
        {
            await Clients.All.ui_UpdateUserBetAsync(userBet);
        }

        private async void NotificationsHub_OnNewClientConnected(object? sender, ConnectionEventArgs e)
        {
           await Clients.All.UserConnectedAsync($"New Connection {e.ConnectionId}.");
        }

        public async Task BroadcastMessageAsync(LiveMessage liveMessage)
        {
            await Clients.All.RecieveMessageAsync(liveMessage);
        }

        public async Task HandshakeMessageAsync(object liveMessage)
        {
            await Clients.All.HandshakeMessageAsync(liveMessage);
        }

        public override string GetEventName()
        {
           return nameof(CrashHub);
        }
    }
}
