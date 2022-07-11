using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2Project.Models;

namespace Web2Project.Helper
{
    public interface IProductRepository
    {
        void Add(Proizvod proizvod);
        Proizvod Get(int id);
        bool Existing(string naziv);
        bool Existing(int id);
        void UpdateProperty(Proizvod proizvod, string property, string value);
        void UpdateProperty(Proizvod proizvod, string property, float value);
        void UpdateProperty(Proizvod proizvod, string property, int value);
        List<Proizvod> GetAllProduct();
        void DeleteProduct(int id);
    }
}
