using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web2Project.Models
{
    public class Proizvod_Korpa
    {
        public int Id { get; set; }
        public int Proizvod_Id { get; set; }
        public int Korpa_Id { get; set; }
        public int Proizvodi_Kolicina { get; set; }

    }
}
