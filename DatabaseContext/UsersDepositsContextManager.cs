using DatabaseContext.DataLayer;
using DatabaseContext.Interfaces;
using System;
using System.Threading.Tasks;
using Treel.DataModel.Enums;
using Treel.DataModel.TransactionModels; // Ensure this namespace is used
using TreelGamesDataModel;

namespace DatabaseContext
{
    public partial class DatabaseContextManager : _IDatabaseContextManager
    {
        private readonly IBalanceDepositRepository _balanceDepositRepository;

        public async Task<TreelStudios_DepositBalanceRequestModel> StoreSpinDepositRequestAsync(SpinRequestModel spinRequestModel)
        {
            var balanceDeposit = new BalanceDepositMongo
            {
                DateTime = DateTime.UtcNow,
                SpinRequestModel = spinRequestModel,
                Status = WithdrawDepositStatus.PENDING
            };

            await _balanceDepositRepository.InsertAsync(balanceDeposit);

            return MapToDepositBalanceRequestModel(balanceDeposit);
        }

        public async Task<TreelStudios_DepositBalanceRequestModel> RetrieveSpinDepositRequestAsync(Guid spiningId)
        {
            var balanceDeposit = await _balanceDepositRepository.FindByIdAsync(spiningId);

            if (balanceDeposit == null)
            {
                return null; // Handle null scenario as appropriate
            }

            return MapToDepositBalanceRequestModel(balanceDeposit);
        }

        public async Task<TreelStudios_DepositBalanceRequestModel> UpdateSpinDepositRequestAsync(Guid spiningId, WithdrawDepositStatus newStatus)
        {
            var updatedBalanceDeposit = await _balanceDepositRepository.UpdateStatusAsync(spiningId, newStatus);

            if (updatedBalanceDeposit == null)
            {
                return null; // Handle null scenario as appropriate
            }

            return MapToDepositBalanceRequestModel(updatedBalanceDeposit);
        }

        private TreelStudios_DepositBalanceRequestModel MapToDepositBalanceRequestModel(BalanceDepositMongo balanceDeposit)
        {
            return new TreelStudios_DepositBalanceRequestModel
            {
                SessionToken = balanceDeposit.SpinRequestModel.SessionToken,
                PlayerId = balanceDeposit.SpinRequestModel.PlayerId,
                Amount = balanceDeposit.SpinRequestModel.Amount,
                Customer_Curency_Name = balanceDeposit.SpinRequestModel.Customer_Curency_Name,
                GameName = balanceDeposit.SpinRequestModel.GameId,
                Customer_Site_Name = balanceDeposit.SpinRequestModel.Customer_Site_Name,
                TransactionId = balanceDeposit.SpiningId.ToString()
            };
        }
    }
}
