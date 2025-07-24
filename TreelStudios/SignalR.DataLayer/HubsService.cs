using Cox.SignalRDataModel.Interfaces;
using Microsoft.AspNetCore.SignalR;
using TreelStudios.SignalR.DataLayer.Interfaces;

namespace TreelStudios.SignalR.DataLayer
{
    public class HubsService(IHubContext<SpinHub, ISpinHub> spinHub,
    IClientManagementService clientManagement) : IHubsService
    {
        private readonly IHubContext<SpinHub, ISpinHub> _spinHubContext = spinHub;
        private readonly IClientManagementService _clientManagement = clientManagement;

        public async Task UpdateNotificationsAsync(string userId, object message)
        {
            var clientConnections = _clientManagement.GetClients(userId);
            foreach (var client in clientConnections)
            {
                if (!string.IsNullOrWhiteSpace(client))
                {
                    await _spinHubContext.Clients.Clients(client).UserConnectedAsync(message);
                }
            }
        }

        public async Task SendBalanceRefreshAsync(string userId, object message)
        {
            var clientConnections = _clientManagement.GetClients(userId);
            foreach (var client in clientConnections)
            {
                if (!string.IsNullOrWhiteSpace(client))
                {
                    await _spinHubContext.Clients.Clients(client).BalanceRefreshAsync(message);
                }
            }
        }

        public async Task SendSpinResultAsync(string userId, object message)
        {
            var clientConnections = _clientManagement.GetClients(userId);
            foreach (var client in clientConnections)
            {
                if (!string.IsNullOrWhiteSpace(client))
                {
                    await _spinHubContext.Clients.Clients(client).SpinResultAsync(message);
                }
            }
        }

        public async Task SendSpinOver(string userId, object message)
        {
            var clientConnections = _clientManagement.GetClients(userId);
            foreach (var client in clientConnections)
            {
                if (!string.IsNullOrWhiteSpace(client))
                {
                    await _spinHubContext.Clients.Clients(client).SpinResultAsync(message);
                }
            }
        }

        public async Task SendBalanceRefreshAsync(string userId, string balance)
        {
            var clientConnections = _clientManagement.GetClients(userId);
            foreach (var client in clientConnections)
            {
                if (!string.IsNullOrWhiteSpace(client))
                {
                    await _spinHubContext.Clients.Clients(client).BalanceRefreshAsync(balance);
                }
            }
        }
    }
}

