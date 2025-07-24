using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cox.SignalRDataModel
{
    public class ConnectionEventArgs : EventArgs
    {
        public string ConnectionId { get; set; } = string.Empty;
    }
}
