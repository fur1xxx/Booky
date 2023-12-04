using Booky.DataAccess.Data;
using Booky.DataAccess.Repository.IRepository;
using Booky.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booky.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ShoppingCartRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Update(ShoppingCart obj)
        {
            dbContext.ShoppingCarts.Update(obj);
        }
    }
}
