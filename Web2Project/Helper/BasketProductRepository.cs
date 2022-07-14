using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2Project.Models;
using Web2Project.Repository;

namespace Web2Project.Helper
{
    public class BasketProductRepository : IBasketProductRepository
    {
        DataBaseContext _dbcontext;

        public BasketProductRepository(DataBaseContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public void Add(KorpaProizvod korpaProizvod)
        {
            _dbcontext.KorpaProizvod.Add(korpaProizvod);
            _dbcontext.SaveChanges();
        }

        public KorpaProizvod Existing(int ProizvodId, int KorpaId)
        {
            KorpaProizvod korpaProizvod = _dbcontext.KorpaProizvod.Where(k => k.ProizvodId == ProizvodId && k.KorpaId == KorpaId).FirstOrDefault();
            return korpaProizvod;
        }

        public List<KorpaProizvod> GetAllProducts(int KorpaId)
        {
            var result = _dbcontext.KorpaProizvod.Where(k => k.KorpaId == KorpaId).ToList();

            return result;
        }

        public void PovecajKolicinu(int id)
        {
            var result = _dbcontext.KorpaProizvod.Where(k => k.Id == id).FirstOrDefault();

            result.Kolicina++;

            _dbcontext.SaveChanges();
        }

        public void SmanjiKolicinu(int id)
        {
            var result = _dbcontext.KorpaProizvod.Where(k => k.Id == id).FirstOrDefault();

            result.Kolicina--;

            if(result.Kolicina == 0)
            {
                _dbcontext.Remove(_dbcontext.KorpaProizvod.Single(k => k.Id == result.Id));
            }

            _dbcontext.SaveChanges();
        }
    }
}
