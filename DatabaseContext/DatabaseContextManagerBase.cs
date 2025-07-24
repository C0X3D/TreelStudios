using DatabaseContext.Interfaces;
using DatabaseContext.Repositories;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Treel.DataModel.TransactionModels;
using Treel.Games.DataLayer;
using TreelGamesDataModel;


namespace DatabaseContext
{
    public partial class DatabaseContextManager : _IDatabaseContextManager
    {
        private readonly IMongoDatabase _database;
        public DatabaseContextManager(IMongoClient mongoClient,
            IBalanceWithdrawRepository balanceWithdrawRepository,
            IBalanceDepositRepository balanceDepositRepository,
            ISpinLogicRepository spinLogicRepository)
        {
            _database = mongoClient.GetDatabase("mongodb");
            _spinLogicRepository = spinLogicRepository;
            _balanceDepositRepository = balanceDepositRepository;
            _balanceWithdrawRepository = balanceWithdrawRepository;
        }

        public event Action<string, TreelStudios_BalanceResponseModel> OnUserBalanceUpdatedEvent;
    }
}
