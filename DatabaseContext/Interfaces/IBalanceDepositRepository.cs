using DatabaseContext.DataLayer;
using Treel.DataModel.Enums;

namespace DatabaseContext.Interfaces
{
    public interface IBalanceDepositRepository
    {
        Task InsertAsync(BalanceDepositMongo balanceDeposit);
        Task<BalanceDepositMongo> FindByIdAsync(Guid spiningId);
        Task<BalanceDepositMongo> UpdateStatusAsync(Guid spiningId, WithdrawDepositStatus newStatus);
    }
}
