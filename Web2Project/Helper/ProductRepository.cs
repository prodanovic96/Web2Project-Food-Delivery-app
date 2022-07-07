using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2Project.Models;
using Web2Project.Repository;

namespace Web2Project.Helper
{
    public class ProductRepository : IProductRepository
    {
        DataBaseContext _dbcontext;

        public ProductRepository(DataBaseContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public void Add(Proizvod proizvod)
        {
            _dbcontext.Proizvod.Add(proizvod);
            _dbcontext.SaveChanges();
        }

        public Proizvod Get(int id)
        {
            Proizvod proizvod = _dbcontext.Proizvod.Where(k => k.Id == id).FirstOrDefault();
            return proizvod;
        }
    }
}
