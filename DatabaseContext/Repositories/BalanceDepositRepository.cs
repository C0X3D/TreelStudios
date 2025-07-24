using DatabaseContext.DataLayer;
using DatabaseContext.Interfaces;
using MongoDB.Driver;
using Treel.DataModel.Enums;

namespace DatabaseContext.Repositories
{
    public class BalanceDepositRepository : IBalanceDepositRepository
    {
        private readonly IMongoCollection<BalanceDepositMongo> _collection;

        public BalanceDepositRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<BalanceDepositMongo>("Balance_Deposits");
        }

        public async Task InsertAsync(BalanceDepositMongo balanceDeposit)
        {
            await _collection.InsertOneAsync(balanceDeposit);
        }

        public async Task<BalanceDepositMongo> FindByIdAsync(Guid spiningId)
        {
            var filter = Builders<BalanceDepositMongo>.Filter.Eq(x => x.SpiningId, spiningId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<BalanceDepositMongo> UpdateStatusAsync(Guid spiningId, WithdrawDepositStatus newStatus)
        {
            var filter = Builders<BalanceDepositMongo>.Filter.Eq(x => x.SpiningId, spiningId);
            var update = Builders<BalanceDepositMongo>.Update
                .Set(x => x.Status, newStatus)
                .Set(x => x.DateTime, DateTime.UtcNow);

            return await _collection.FindOneAndUpdateAsync(
                filter,
                update,
                new FindOneAndUpdateOptions<BalanceDepositMongo>
                {
                    ReturnDocument = ReturnDocument.After
                }
            );
        }
    }
}
