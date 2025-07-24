using DatabaseContext.Interfaces;
using DatabaseContext.Repositories;
using System;
using System.Threading.Tasks;
using Treel.DataModel.Enums;
using Treel.DataModel.TransactionModels;
using TreelGamesDataModel;

namespace DatabaseContext
{
    public partial class DatabaseContextManager : _IDatabaseContextManager
    {
        private readonly IBalanceWithdrawRepository _balanceWithdrawRepository;
        public async Task<TreelStudios_WithdrawBalanceRequestModel> StoreSpinWithdrawRequestAsync(SpinRequestModel spinRequestModel)
        {
            var balanceWithdraw = new BalanceWithdrawMongo
            {
                DateTime = DateTime.UtcNow,
                SpinRequestModel = spinRequestModel,
                Status = WithdrawDepositStatus.PENDING,
            };

            await _balanceWithdrawRepository.InsertAsync(balanceWithdraw);

            return MapToWithdrawBalanceRequestModel(balanceWithdraw);
        }

        public async Task<TreelStudios_WithdrawBalanceRequestModel> RetrieveSpinWithdrawRequestAsync(Guid spiningId)
        {
            var balanceWithdraw = await _balanceWithdrawRepository.FindByIdAsync(spiningId);

            if (balanceWithdraw == null)
            {
                return null; // Handle null scenario as appropriate
            }

            return MapToWithdrawBalanceRequestModel(balanceWithdraw);
        }

        public async Task<TreelStudios_WithdrawBalanceRequestModel> UpdateSpinWithdrawRequestAsync(Guid spiningId, WithdrawDepositStatus newStatus)
        {
            var updatedBalanceWithdraw = await _balanceWithdrawRepository.UpdateStatusAsync(spiningId, newStatus);

            if (updatedBalanceWithdraw == null)
            {
                return null; // Handle null scenario as appropriate
            }

            return MapToWithdrawBalanceRequestModel(updatedBalanceWithdraw);
        }

        private TreelStudios_WithdrawBalanceRequestModel MapToWithdrawBalanceRequestModel(BalanceWithdrawMongo balanceWithdraw)
        {
            return new TreelStudios_WithdrawBalanceRequestModel
            {
                SessionToken = balanceWithdraw.SpinRequestModel.SessionToken,
                PlayerId = balanceWithdraw.SpinRequestModel.PlayerId,
                Amount = balanceWithdraw.SpinRequestModel.Amount,
                Customer_Curency_Name = balanceWithdraw.SpinRequestModel.Customer_Curency_Name,
                GameId = balanceWithdraw.SpinRequestModel.GameId,
                Customer_Site_Name = balanceWithdraw.SpinRequestModel.Customer_Site_Name,
                TransactionId = balanceWithdraw.SpiningId.ToString()
            };
        }
    }
}
