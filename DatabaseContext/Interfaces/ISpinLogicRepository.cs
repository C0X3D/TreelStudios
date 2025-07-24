using DatabaseContext.DataLayer;

namespace DatabaseContext.Interfaces
{
    public interface ISpinLogicRepository
    {
        Task InsertAsync(string key, SpinLogicDataModelMongo dataModel);
        Task<bool> ExistsAsync(string key);
        Task<List<SpinLogicDataModelMongo>> GetHistoryAsync(string key, int skip, int take);
        Task<SpinLogicDataModelMongo> GetAsync(string key);
        Task<bool> UpdateAsync(string key, SpinLogicDataModelMongo updatedDataModel);
        Task<bool> DeleteAsync(string key);
    }
}
