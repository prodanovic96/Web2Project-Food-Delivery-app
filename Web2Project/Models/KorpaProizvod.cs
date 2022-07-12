using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web2Project.Models
{
    public class KorpaProizvod
    {
        public int Id { get; set; }
        public int Kolicina { get; set; }
        public int ProizvodId { get; set; }
        public int KorpaId { get; set; }
        public virtual Korpa Korpa { get; set; }
        public virtual Proizvod Proizvod { get; set; }

    }
}
