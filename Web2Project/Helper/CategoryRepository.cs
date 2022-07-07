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

        public List<Kategorija> GetAll()
        {
            return _dbcontext.Kategorija.ToList();
        }
    }
}
