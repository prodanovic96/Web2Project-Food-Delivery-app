using System.Collections.Generic;

namespace Web2Project.Models
{
    public class Proizvod
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public float Cena { get; set; }
        public string Sastojci { get; set; }
        public int KategorijaId { get; set; }
        public Kategorija Kategorija { get; set; }
        public ICollection<KorpaProizvod> KorpeProizvodi { get; set; }
        public string ImagePath { get; set; }
    }
}
