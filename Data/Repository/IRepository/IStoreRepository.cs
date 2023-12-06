using api_app.Data.Repository.IRepository;
using api_app.Models;

namespace api_app.Data.Repository.IRepository
{
    public interface IStoreRepository : IRepository<Store>
    {
        void Update(Store obj);
        void Save();
    }
}