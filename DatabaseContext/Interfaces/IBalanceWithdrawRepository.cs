using DatabaseContext.Repositories;
using Treel.DataModel.Enums;

namespace DatabaseContext.Interfaces
{
    public interface IBalanceWithdrawRepository
    {
        Task InsertAsync(BalanceWithdrawMongo balanceWithdraw);
        Task<BalanceWithdrawMongo> FindByIdAsync(Guid spiningId);
        Task<BalanceWithdrawMongo> UpdateStatusAsync(Guid spiningId, WithdrawDepositStatus newStatus);
    }
}
