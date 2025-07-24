using Cox.SignalRDataModel;
using Cox.SignalRDataModel.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Cox.SignalRHub
{
    public abstract class GeneralHub<T>(IClientManagementService? clientManagement) : Hub<T> where T : class
    {
        private readonly IClientManagementService? clientManagement = clientManagement;

        public event EventHandler<ConnectionEventArgs> OnNewClientConnected = null!;

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            string userId = clientManagement!.GetClientById(connectionId);

            clientManagement.UnregisterClient(userId, connectionId);

            await base.OnDisconnectedAsync(exception);
        }

        public void RegisterClient(string userId)
        {
            clientManagement!.RegisterClient(userId, Context.ConnectionId);
            OnNewClientConnected?.Invoke(this, new ConnectionEventArgs() { ConnectionId = Context.ConnectionId });
        }

        public abstract string GetEventName();
    }
}
