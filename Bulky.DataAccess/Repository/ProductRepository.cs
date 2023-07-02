using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private AppDbContext _appDb;
        public ProductRepository(AppDbContext appDb) : base(appDb)
        {
            _appDb = appDb;
        }
        public void Update(Product product)
        {
           var objFromDb = _appDb.Products.FirstOrDefault(obj => obj.Id == product.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = product.Title;
                objFromDb.Description = product.Description;
                objFromDb.ISBN = product.ISBN;
                objFromDb.ListPrice = product.ListPrice;
                objFromDb.Price = product.Price;
                objFromDb.Price50 = product.Price50;
                objFromDb.Price100 = product.Price100;
                objFromDb.Author = product.Author;
                objFromDb.CategoryId = product.CategoryId;
                if(product.ImageUrl is not null) 
                {
                    objFromDb.ImageUrl = product.ImageUrl;
                }
            }
        }
    }
}
