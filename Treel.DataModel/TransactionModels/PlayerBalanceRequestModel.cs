using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treel.DataModel.TransactionModels
{
    public class TreelStudios_WithdrawBalanceRequestModel : RequestModelBase
    {
        public string? SessionToken { get; set; } = string.Empty;
        public string? TransactionId { get; set; } = string.Empty;
        public string? PlayerId { get; set; } = string.Empty;
        public string? GameId { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 0m;
    }

    public class TreelStudios_DepositBalanceRequestModel : RequestModelBase
    {
        public string? SessionToken { get; set; } = string.Empty;
        public string? TransactionId { get; set; } = string.Empty;
        public string? PlayerId { get; set; } = string.Empty;
        public string? GameName { get; set; } = string.Empty;
        public decimal? Amount { get; set; } = 0m;
        public bool IsRefund { get; set; } = false;
    }
}
