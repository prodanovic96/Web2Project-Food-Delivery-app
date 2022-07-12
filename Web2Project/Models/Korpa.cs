using System.Collections.Generic;

namespace Web2Project.Models
{
    public class Korpa
    {
        public int Id { get; set; }
        public float Cena { get; set; }
        public int KorisnikId { get; set; }
        public virtual Korisnik Korisnik { get; set; }
        public virtual ICollection<KorpaProizvod> KorpeProizvodi { get; set; }

        public Korpa()
        {
            Cena = 400;
        }

        public void Dodaj(float iznos)
        {
            Cena += iznos;
        }

        public void Oduzmi(float iznos)
        {
            Cena -= iznos;
        }
    }
}
