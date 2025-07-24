using DatabaseContext.Interfaces;
using MongoDB.Driver;
using Treel.DataModel.Enums;

namespace DatabaseContext.Repositories
{
    public class BalanceWithdrawRepository : IBalanceWithdrawRepository
    {
        private readonly IMongoCollection<BalanceWithdrawMongo> _collection;

        public BalanceWithdrawRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<BalanceWithdrawMongo>("Balance_Withdrawals");
        }

        public async Task InsertAsync(BalanceWithdrawMongo balanceWithdraw)
        {
            await _collection.InsertOneAsync(balanceWithdraw);
        }

        public async Task<BalanceWithdrawMongo> FindByIdAsync(Guid spiningId)
        {
            var filter = Builders<BalanceWithdrawMongo>.Filter.Eq(x => x.SpiningId, spiningId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<BalanceWithdrawMongo> UpdateStatusAsync(Guid spiningId, WithdrawDepositStatus newStatus)
        {
            var filter = Builders<BalanceWithdrawMongo>.Filter.Eq(x => x.SpiningId, spiningId);
            var update = Builders<BalanceWithdrawMongo>.Update
                .Set(x => x.Status, newStatus)
                .Set(x => x.DateTime, DateTime.UtcNow);

            return await _collection.FindOneAndUpdateAsync(
                filter,
                update,
                new FindOneAndUpdateOptions<BalanceWithdrawMongo>
                {
                    ReturnDocument = ReturnDocument.After
                }
            );
        }
    }
}
