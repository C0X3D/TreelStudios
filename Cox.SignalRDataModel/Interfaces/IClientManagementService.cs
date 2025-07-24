using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cox.SignalRDataModel.Interfaces
{
    public interface IClientManagementService
    {        
        void RegisterClient(string userId, string connectionId);
        void UnregisterClient(string userId, string connectionId);
        List<string> GetClients(string userId);
        string GetClientById(string connectionId);
    }
}
