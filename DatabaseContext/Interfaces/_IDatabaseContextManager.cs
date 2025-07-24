using DatabaseContext.DataLayer;
using MongoDB.Driver;
using Treel.DataModel.Enums;
using Treel.DataModel.TransactionModels;
using Treel.Games.DataLayer;
using TreelGamesDataModel;

namespace DatabaseContext
{
    public interface _IDatabaseContextManager
    {
        event Action<string, TreelStudios_BalanceResponseModel> OnUserBalanceUpdatedEvent;

        Task<bool> DeleteBusyUser(string key, SpinLogicDataModelMongo updatedDataModel);
        Task<SpinLogicDataModelMongo> GetBusyUser(string playerId);
        Task<List<SpinLogicDataModelMongo>> GetBusyUserHistory(string playerId, int skip, int take);
        Task<bool> IsUserBusy(string playerId);
        Task<TreelStudios_DepositBalanceRequestModel> RetrieveSpinDepositRequestAsync(Guid spiningId);
        Task<TreelStudios_WithdrawBalanceRequestModel> RetrieveSpinWithdrawRequestAsync(Guid spiningId);
        Task<bool> SetUserBusy(string key, SpinLogicDataModelMongo dataModel);
        Task<TreelStudios_DepositBalanceRequestModel> StoreSpinDepositRequestAsync(SpinRequestModel spinRequestModel);
        Task<TreelStudios_WithdrawBalanceRequestModel> StoreSpinWithdrawRequestAsync(SpinRequestModel spinRequestModel);
        Task<bool> UpdateBusyUser(string key, SpinLogicDataModelMongo updatedDataModel);
        Task<TreelStudios_DepositBalanceRequestModel> UpdateSpinDepositRequestAsync(Guid spiningId, WithdrawDepositStatus newStatus);
        Task<TreelStudios_WithdrawBalanceRequestModel> UpdateSpinWithdrawRequestAsync(Guid spiningId, WithdrawDepositStatus newStatus);
    }
}