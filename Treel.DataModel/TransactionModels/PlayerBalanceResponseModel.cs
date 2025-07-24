using Treel.DataModel.Enums;

namespace Treel.DataModel.TransactionModels
{
    public class TreelStudios_BalanceResponseModel : ResponseModelBase
    {
        public TreelStudios_BalanceResponseModel()
        {
        }

        public TreelStudios_BalanceResponseModel(decimal balance, WithdrawDepositStatus status)
        {
            Balance = balance;
            Status = status;
        }

        public TreelStudios_BalanceResponseModel(WithdrawDepositStatus status)
        {
            Status = status;
        }

        public decimal Balance { get; set; }
        public WithdrawDepositStatus Status { get; set; } = WithdrawDepositStatus.FAILED;
    }
}
