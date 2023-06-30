using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private AppDbContext _appDb;
        public CategoryRepository(AppDbContext appDb) : base(appDb) 
        {
            _appDb = appDb;
        }
        

        public void Update(Category category)
        {
            _appDb.Update(category);
        }
    }
}
