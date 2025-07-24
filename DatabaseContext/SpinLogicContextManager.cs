using DatabaseContext.DataLayer;
using DatabaseContext.Interfaces;
using Treel.DataModel.Enums;
using Treel.DataModel.TransactionModels;
using Treel.SendEndpointRequests;
using TreelGamesDataModel;

namespace DatabaseContext
{
    public partial class DatabaseContextManager : _IDatabaseContextManager
    {
        private readonly ISpinLogicRepository _spinLogicRepository;

        public async Task<bool> SetUserBusy(string key, SpinLogicDataModelMongo dataModel)
        {
            await _spinLogicRepository.InsertAsync(key, dataModel);
            return await _spinLogicRepository.ExistsAsync(key);
        }

        public async Task<bool> IsUserBusy(string playerId)
        {
            return await _spinLogicRepository.ExistsAsync(playerId);
        }

        public async Task<List<SpinLogicDataModelMongo>> GetBusyUserHistory(string playerId, int skip, int take)
        {
            return await _spinLogicRepository.GetHistoryAsync(playerId, skip, take);
        }

        public async Task<SpinLogicDataModelMongo> GetBusyUser(string playerId)
        {
            return await _spinLogicRepository.GetAsync(playerId);
        }

        public async Task<bool> DeleteBusyUser(string key, SpinLogicDataModelMongo updatedDataModel)
        {
            var existingUser = await _spinLogicRepository.GetAsync(key);
            if (existingUser == null)
                return false;

            await HandleBalanceUpdate(existingUser, updatedDataModel);
            return await _spinLogicRepository.DeleteAsync(key);
        }

        public async Task<bool> UpdateBusyUser(string key, SpinLogicDataModelMongo updatedDataModel)
        {
            if (updatedDataModel == null)
                return false;

            if (IsDataModelEmpty(updatedDataModel))
                return await DeleteBusyUser(key, updatedDataModel);

            return await _spinLogicRepository.UpdateAsync(key, updatedDataModel);
        }

        private bool IsDataModelEmpty(SpinLogicDataModelMongo dataModel)
        {
            return (dataModel.DataModel?.BaseSpinDataModel?.Count ?? 0) <= 0 &&
                   (dataModel.DataModel?.BonusSpinDataModel?.BonusSpinSteps?.Count ?? 0) <= 0;
        }

        private async Task HandleBalanceUpdate(SpinLogicDataModelMongo existingData, SpinLogicDataModelMongo updatedData)
        {
            CustomerRequestSender sender = new();
            var depositBalanceHelp = await StoreSpinDepositRequestAsync(existingData.SpinRequestModel);
            depositBalanceHelp.Amount = existingData.DataModel?.SpinTotalPayout ?? 0;
            depositBalanceHelp.Amount *= existingData.SpinRequestModel?.Amount;
            var responseModel = await sender.SendPlayerBalanceDepositRequestAsync(new CustomerSettings().Customer, depositBalanceHelp);

            if (responseModel.Status == WithdrawDepositStatus.SUCCESS)
            {
                OnUserBalanceUpdatedEvent?.Invoke($"{updatedData.SpinRequestModel?.PlayerId}_{updatedData.SpinRequestModel?.GameId}", responseModel);
                if (Guid.TryParse(depositBalanceHelp.TransactionId, out var transactionId))
                {
                    await UpdateSpinDepositRequestAsync(transactionId, WithdrawDepositStatus.SUCCESS);
                }
            }
        }
    }
}
