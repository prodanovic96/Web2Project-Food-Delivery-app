using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2Project.Models;
using Web2Project.Repository;

namespace Web2Project.Helper
{
    public class UserRepository : IUserRepository
    {
        DataBaseContext _dbcontext;

        public UserRepository(DataBaseContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public void Add(Korisnik korisnik)
        {
            _dbcontext.Korisnik.Add(korisnik);
            _dbcontext.SaveChanges();
        }

        public bool Existing(string korisnickoIme)
        {
            var result = _dbcontext.Korisnik.Any(korisnik => korisnik.KorisnickoIme == korisnickoIme);

            return result;
        }

        public Korisnik Get(string korisnickoIme)
        {
            var result = _dbcontext.Korisnik.Where(korisnik => korisnik.KorisnickoIme == korisnickoIme).FirstOrDefault();

            return result;
        }

        public Korisnik Get(int id)
        {
            var result = _dbcontext.Korisnik.Where(korisnik => korisnik.Id == id).FirstOrDefault();

            return result;
        }

        public List<Korisnik> GetDostavljaci()
        {
            return _dbcontext.Korisnik.Where(k => k.Verifikovan == Zahtev.PROCESIRA_SE && k.TipKorisnika == Tip.DOSTAVLJAC).ToList();
        }

        public List<Korisnik> GetDostavljaciPrihvaceni()
        {
            return _dbcontext.Korisnik.Where(k => k.Verifikovan == Zahtev.PRIHVACEN && k.TipKorisnika == Tip.DOSTAVLJAC).ToList();
        }

        public Zahtev VratiZahtev(int id)
        {
            var result = _dbcontext.Korisnik.Where(korisnik => korisnik.Id == id).FirstOrDefault();

            return result.Verifikovan;
        }

        public void UpdateKorisnik(Korisnik korisnik, string property, string value)
        {
            var result = _dbcontext.Korisnik.Where(k => k.Id == korisnik.Id).FirstOrDefault();

            if(property == "Verifikovan")
            {
                Enum.TryParse(value, out Zahtev zahtev);
                result.GetType().GetProperty(property).SetValue(result, zahtev);
            }
            else
            {
                result.GetType().GetProperty(property).SetValue(result, value);
            }
            
            _dbcontext.SaveChanges();
        }
    }
}
 