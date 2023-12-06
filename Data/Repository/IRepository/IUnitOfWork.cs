using api_app.Data.Repository.IRepository;
using Rehub.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rehub.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository ApplicationUser { get; }
        IStoreRepository Store { get; }
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        void Save();
    }
}