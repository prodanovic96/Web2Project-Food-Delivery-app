using System.Collections.Generic;

namespace Web2Project.Models
{
    public class Kategorija
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public virtual ICollection<Proizvod> Proizvodi{ get; set; }
    }
}
