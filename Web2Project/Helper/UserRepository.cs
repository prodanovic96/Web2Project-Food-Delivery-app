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
    }
}
