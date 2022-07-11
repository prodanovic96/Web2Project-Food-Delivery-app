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

        bool Existing(string naziv);
        bool Existing(int id);

        List<Kategorija> GetAll();

        void DeleteCategory(int id);

        void UpdateProperty(Kategorija kategorija, string value);
    }
}
