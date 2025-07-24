using Cox.SignalRDataModel.Interfaces;

namespace Cox.SignalRHub
{
    public class ClientManagementService : IClientManagementService
    {
        private readonly Dictionary<string, List<string>> _clientConnections = [];

        public string GetClientById(string connectionId)
        {
            return _clientConnections.FirstOrDefault(x => x.Value.Contains(connectionId)).Key;
        }

        public List<string> GetClients(string userId)
        {
            return _clientConnections.TryGetValue(userId, out List<string>? value) ? value : [];
        }

        public void RegisterClient(string userId, string connectionId)
        {
            if (!_clientConnections.TryGetValue(userId, out List<string>? value))
            {
                value = [];
                _clientConnections[userId] = value;
            }

            value.Add(connectionId);
        }

        public void UnregisterClient(string userId, string connectionId)
        {
            if (_clientConnections.TryGetValue(userId, out List<string>? value))
            {
                value.Remove(connectionId);

                if (value.Count == 0)
                {
                    _clientConnections.Remove(userId);
                }
            }
        }
    }
}
