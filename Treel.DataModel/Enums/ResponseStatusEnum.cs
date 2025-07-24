using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treel.DataModel.Enums
{
    public enum WithdrawDepositStatus
    {
        PENDING,
        SUCCESS,
        FAILED,
        INSUFICIENT_FUNDS,
        TRANSACTION_ALREADY_EXISTS,
        REFUND_SUCCESS,
    }

    public enum ResponseStatusEnum
    {
        INVALID_SESSION_TOKEN = 501,
        INVALID_AUTH = 600,
        INVALID_AUTH_TOKEN = 601,
        REQUEST_FAILED = 700,
        REQUEST_FAILED_OBJECT_NULL = 701,
        REQUEST_FAILED_OBJECT_PARAMS_NULL = 702,
        UNEXPECTED_ERROR = 800,
        SUCCESS = 900,
        USER_BUSY = 901,
        INSUFICIENT_FUNDS = 902,
    }
}
