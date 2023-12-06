using api_app.Data.Repository.IRepository;
using api_app.Models;

namespace api_app.Data.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.ProductName = obj.ProductName;
                objFromDb.Price = obj.Price;
                objFromDb.SlashedPrice = obj.SlashedPrice;
                objFromDb.Description = obj.Description;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.StoreId = obj.StoreId;
                if (obj.Image01Url != null)
                {
                    objFromDb.Image01Url = obj.Image01Url;
                }
                if (obj.Image02Url != null)
                {
                    objFromDb.Image02Url = obj.Image02Url;
                }
            }
        }
    }
}