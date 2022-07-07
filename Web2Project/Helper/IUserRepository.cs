using System.Collections.Generic;
using Web2Project.Models;

namespace Web2Project.Helper
{
    public interface IUserRepository
    {
        void Add(Korisnik korisnik);

        Korisnik Get(string korisnickoIme);

        Korisnik Get(int id);

        bool Existing(string korisnickoIme);

        void UpdateKorisnik(Korisnik korisnik, string property, string value);
        List<Korisnik> GetDostavljaci();
        List<Korisnik> GetDostavljaciPrihvaceni();

        Zahtev VratiZahtev(int id);
    }
}
