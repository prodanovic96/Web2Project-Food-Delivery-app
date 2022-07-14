using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web2Project.Models
{
    public class Stavka
    {
        public int Id { get; set; }
        public Proizvod Proizvod { get; set; }
        public int Kolicina { get; set; }
        public int KorpaId { get; set; }
    }
}
