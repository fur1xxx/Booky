using Booky.DataAccess.Data;
using Booky.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booky.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            Category = new CategoryRepository(dbContext);
            Product = new ProductRepository(dbContext);
            Company = new CompanyRepository(dbContext);
        }

        public IProductRepository Product { get; private set; }
        public ICategoryRepository Category {get; private set;}

        public ICompanyRepository Company { get; private set; }

        public void Save()
        {
            dbContext.SaveChanges();
        }
    }
}
