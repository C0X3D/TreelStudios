using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treel.DataModel.Enums;

namespace TreelGamesDataModel
{
    public class SpinResponseModel
    {
        public SpinResponseModel() { }
        public bool ConnectToPayments { get; set; } = false;
    }

    public class CrashBalanceResponseModel
    {
        public WithdrawDepositStatus Status { get; set; }
        public decimal NewBalance { get; set; }
    }


    public class CrashCashOutResponse
    {
        public double Multiplier { get; set; }
        public string UserId { get; set; }
        public string GameId { get; set; }
        public string SessionToken { get; set; }
    }
}
