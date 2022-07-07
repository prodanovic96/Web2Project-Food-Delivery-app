namespace Web2Project.Models
{
    public class Porudzbina
    {
        public int Id { get; set; }
        public int Korpa_Id { get; set; } 
        public int Kolicina { get; set; }
        public string Adresa { get; set; }
        public string Komentar { get; set; }
        public float Cena { get; set; }
        public bool Izvrsena { get; set; }
        public int DostavljacId { get; set; }
        public int PotrosacId { get; set; }
        public virtual Korpa Korpa { get; set; }
        public virtual Korisnik Dostavljac { get; set; }
        public virtual Korisnik Potrosac { get; set; }

        public readonly int CenaDostave = 400;


        public Porudzbina()
        {
            Izvrsena = false;
        }

        public void IzracunajCenu()
        {
            // Saberi sve cene i dodaj cenu dostave
        }     
    }
}
