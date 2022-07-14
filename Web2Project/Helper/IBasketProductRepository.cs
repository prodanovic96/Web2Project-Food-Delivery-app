using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2Project.Models;

namespace Web2Project.Helper
{
    public interface IBasketProductRepository
    {
        KorpaProizvod Existing(int ProizvodId, int KorpaId);
        void PovecajKolicinu(int id);
        void SmanjiKolicinu(int id);
        void Add(KorpaProizvod korpaProizvod);
        List<KorpaProizvod> GetAllProducts(int KorpaId);
    }
}
