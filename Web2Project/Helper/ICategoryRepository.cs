using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2Project.Models;

namespace Web2Project.Helper
{
    public interface ICategoryRepository
    {
        void Add(Kategorija kategorija);
        Kategorija Get(int id);

        List<Kategorija> GetAll();
    }
}
