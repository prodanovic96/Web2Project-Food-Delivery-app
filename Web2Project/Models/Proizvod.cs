using System.Collections.Generic;

namespace Web2Project.Models
{
    public class Proizvod
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public float Cena { get; set; }
        public ICollection<string> Sastojci { get; set; }
    }
}
