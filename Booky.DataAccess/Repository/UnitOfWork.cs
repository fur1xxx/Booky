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
        public IProductRepository Product { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IApplicationUserRepository ApplicationUser {  get; private set; }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            Category = new CategoryRepository(dbContext);
            Product = new ProductRepository(dbContext);
            Company = new CompanyRepository(dbContext);
            ShoppingCart = new ShoppingCartRepository(dbContext);
            ApplicationUser = new ApplicationUserRepository(dbContext);
        }

        public void Save()
        {
            dbContext.SaveChanges();
        }
    }
}
