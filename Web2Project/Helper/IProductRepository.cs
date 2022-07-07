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
    }
}
