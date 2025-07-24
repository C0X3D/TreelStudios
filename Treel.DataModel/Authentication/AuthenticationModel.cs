using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treel.DataModel.Authentication
{
    public class AuthenticationModel : RequestModelBase
    {
        public AuthenticationModel() { }
        public string SessionToken { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Balance { get; set; } = string.Empty;
        public string GameId { get; set; } = string.Empty;
    }
}
