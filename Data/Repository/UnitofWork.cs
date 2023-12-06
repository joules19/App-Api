
using api_app.Data.Repository.IRepository;
using Rehub.DataAccess.Repository.IRepository;

namespace api_app.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ApplicationUser = new ApplicationUserRepository(_db);
            Store = new StoreRepository(_db);
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
        }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IStoreRepository Store { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }



        public void Save()
        {
            _db.SaveChanges();
        }
    }
}