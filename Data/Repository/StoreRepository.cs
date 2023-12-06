using api_app.Data.Repository.IRepository;
using api_app.Models;

namespace api_app.Data.Repository
{
    public class StoreRepository : Repository<Store>, IStoreRepository
    {
        private ApplicationDbContext _db;

        public StoreRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Store obj)
        {
            var objFromDb = _db.Store.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.StoreName = obj.StoreName;
                objFromDb.Description = obj.Description;
                objFromDb.Location = obj.Location;
                objFromDb.ApplicationUserId = obj.ApplicationUserId;
            }
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}