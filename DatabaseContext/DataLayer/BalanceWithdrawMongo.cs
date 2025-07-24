using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Treel.DataModel.Enums;
using TreelGamesDataModel;

namespace DatabaseContext.Repositories
{
    public class BalanceWithdrawMongo
    {
        public BalanceWithdrawMongo() { }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid SpiningId { get; set; }

        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        public SpinRequestModel SpinRequestModel { get; set; }

        [BsonRepresentation(BsonType.String)]
        public WithdrawDepositStatus Status { get; set; }
    }
}
