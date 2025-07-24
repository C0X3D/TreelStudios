using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Treel.Games.DataLayer;
using TreelGamesDataModel;

namespace DatabaseContext.DataLayer
{
    public class SpinLogicDataModelMongo
    {
        [BsonElement("SpinRequestModel")]
        public SpinRequestModel SpinRequestModel { get; set; }

        [BsonElement("DataModel")]
        public SpinLogicDataModel DataModel { get; set; }

        public SpinLogicDataModelMongo()
        {
            
        }

        public SpinLogicDataModelMongo(SpinRequestModel spinRequestModel, SpinLogicDataModel dataModel)
            : this()
        {            
            SpinRequestModel = spinRequestModel ?? throw new ArgumentNullException(nameof(spinRequestModel));
            DataModel = dataModel ?? throw new ArgumentNullException(nameof(dataModel));
        }
    }
}
