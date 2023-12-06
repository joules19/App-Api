using api_app.Data.Repository.IRepository;
using api_app.Models;

namespace api_app.Data.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category obj);
        void Save();
    }
}