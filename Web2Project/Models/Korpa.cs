namespace Web2Project.Models
{
    public class Korpa
    {
        public int Id { get; set; }

        public readonly int CenaDostave = 400;
        public float Cena { get; set; }

        public Korpa()
        {
            Cena = CenaDostave;
        }

        public void IzracunajCenu(float cena)
        {
            Cena += cena;
        }
    }
}
