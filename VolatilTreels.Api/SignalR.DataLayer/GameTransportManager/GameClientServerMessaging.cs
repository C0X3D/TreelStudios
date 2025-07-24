using Cox.SignalRDataModel.Interfaces;
using Cox.SignalRDataModel;
using Cox.SignalRHub;
using Microsoft.AspNetCore.SignalR;
using VolatilTreels.Api.SignalR.DataLayer.Intefaces;

namespace VolatilTreels.Api.SignalR.DataLayer.GameTransportManager;

public class GameClientServerMessaging : GeneralHub<ISpinHub>
{
    public GameClientServerMessaging(IClientManagementService? clientManagement) : base(clientManagement)
    {
        OnNewClientConnected += NotificationsHub_OnNewClientConnected;
    }

    private void NotificationsHub_OnNewClientConnected(object? sender, ConnectionEventArgs e)
    {
        Clients.Clients(e.ConnectionId).SpinResultAsync($"New Connection {e.ConnectionId}.");
    }

    public override string GetEventName()
    {
        return nameof(GameClientServerMessaging);
    }
}

