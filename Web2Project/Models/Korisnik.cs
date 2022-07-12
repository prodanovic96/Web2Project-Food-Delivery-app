using System;
using System.Collections.Generic;

namespace Web2Project.Models
{
    public class Korisnik
    {
        public int Id { get; set; }
        public string KorisnickoIme { get; set; }
        public string Email { get; set; }
        public string Lozinka { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public string Adresa { get; set; }
        public Tip TipKorisnika { get; set; }
        public Zahtev Verifikovan { get; set; }
        public bool LogedIn { get; set; }
        public string ImagePath { get; set; }
        public bool Google { get; set; }
        public virtual ICollection<Korpa> Korpe { get; set; }

        public Korisnik()
        {
            LogedIn = false;
            Google = false;
        }

        public void LogIn()
        {
            LogedIn = true;
        }

        public void LogOut()
        {
            LogedIn = false;
        }
    }
}
