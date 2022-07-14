using System;
using System.Collections.Generic;

namespace Web2Project.Models
{
    public class Korpa
    {
        public int Id { get; set; }
        public float Cena { get; set; }
        public Status Status{ get; set; }
        public string AdresaDostave { get; set; }
        public string Komentar { get; set; }
        public DateTime VremePorucivanja { get; set; }
        public int KorisnikId { get; set; }
        public int DostavljacId { get; set; }
        public Korisnik Korisnik { get; set; }
        public ICollection<KorpaProizvod> KorpeProizvodi { get; set; }

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
