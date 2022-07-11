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

        public void DeleteProduct(int id)
        {
            _dbcontext.Remove(_dbcontext.Proizvod.Single(p => p.Id == id));
            _dbcontext.SaveChanges();
        }

        public bool Existing(string naziv)
        {
            var result = _dbcontext.Proizvod.Any(proizvod => proizvod.Naziv == naziv);

            return result;
        }

        public bool Existing(int id)
        {
            var result = _dbcontext.Proizvod.Any(proizvod => proizvod.Id == id);

            return result;
        }

        public Proizvod Get(int id)
        {
            Proizvod proizvod = _dbcontext.Proizvod.Where(k => k.Id == id).FirstOrDefault();
            return proizvod;
        }

        public List<Proizvod> GetAllProduct()
        {
            return _dbcontext.Proizvod.ToList();
        }

        public void UpdateProperty(Proizvod proizvod, string property, string value)
        {
            var result = _dbcontext.Proizvod.Where(p => p.Id == proizvod.Id).FirstOrDefault();        
            result.GetType().GetProperty(property).SetValue(result, value);     

            _dbcontext.SaveChanges();
        }

        public void UpdateProperty(Proizvod proizvod, string property, float value)
        {
            var result = _dbcontext.Proizvod.Where(p => p.Id == proizvod.Id).FirstOrDefault();

            result.GetType().GetProperty(property).SetValue(result, value);

            _dbcontext.SaveChanges();
        }

        public void UpdateProperty(Proizvod proizvod, string property, int value)
        {
            var result = _dbcontext.Proizvod.Where(p => p.Id == proizvod.Id).FirstOrDefault();

            result.GetType().GetProperty(property).SetValue(result, value);

            _dbcontext.SaveChanges();
        }
    }
}
