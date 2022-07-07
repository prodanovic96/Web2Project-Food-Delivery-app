﻿using System.Collections.Generic;

namespace Web2Project.Models
{
    public class Proizvod
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public float Cena { get; set; }
        public string Sastojci { get; set; }
        public int KategorijaId { get; set; }
        public virtual Kategorija Kategorija { get; set; }
        public string ImagePath { get; set; }
    }
}
