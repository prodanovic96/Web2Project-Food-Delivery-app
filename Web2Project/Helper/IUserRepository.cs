using Web2Project.Models;

namespace Web2Project.Helper
{
    public interface IUserRepository
    {
        void Add(Korisnik korisnik);

        Korisnik Get(string korisnickoIme);

        bool Existing(string korisnickoIme);

        void UpdateKorisnik(Korisnik korisnik, string property, string value);
    }
}
