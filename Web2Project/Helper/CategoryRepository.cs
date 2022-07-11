using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2Project.Models;
using Web2Project.Repository;

namespace Web2Project.Helper
{
    public class CategoryRepository : ICategoryRepository
    {
        DataBaseContext _dbcontext;

        public CategoryRepository(DataBaseContext dbcontext)
        {
            _dbcontext = dbcontext;
        }


        public void Add(Kategorija kategorija)
        {
            _dbcontext.Kategorija.Add(kategorija);
            _dbcontext.SaveChanges();
        }

        public Kategorija Get(int id)
        {
            Kategorija kategorija = _dbcontext.Kategorija.Where(k => k.Id == id).FirstOrDefault();
            return kategorija;
        }

        public bool Existing(string naziv)
        {
            var result = _dbcontext.Kategorija.Any(kategorija => kategorija.Naziv == naziv);

            return result;
        }

        public List<Kategorija> GetAll()
        {
            return _dbcontext.Kategorija.ToList();
        }

        public bool Existing(int id)
        {
            var result = _dbcontext.Kategorija.Any(kategorija => kategorija.Id == id);

            return result;
        }

        public void DeleteCategory(int id)
        {
            _dbcontext.Remove(_dbcontext.Kategorija.Single(k => k.Id == id));
            _dbcontext.SaveChanges();
        }

        public void UpdateProperty(Kategorija kategorija, string value)
        {
            var result = _dbcontext.Kategorija.Where(k => k.Id == kategorija.Id).FirstOrDefault();
            result.Naziv = value;

            _dbcontext.SaveChanges();
        }
    }
}
