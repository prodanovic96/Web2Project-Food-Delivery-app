using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2Project.Models;

namespace Web2Project.Helper
{
    public interface IBasketRepository
    {
        Korpa GetBasketByStatus(int potrosacId, Status status);
        List<Korpa> GetFinishedBasket(int potrosacId);
        List<Korpa> GetDostavljacBasket();
        List<Korpa> GetAdminBasket();
        List<Korpa> GetDostavljacFinishedBasket(int dostavljacId);
        Korpa GetDostavljacActiveBasket(int dostavljacId);
        Korpa GetPotrosacActiveBasket(int potrosacId);
        Korpa GetBasketById(int id);
        void Add(Korpa korpa);
        void UpdateCene(Korpa korpa);
        void UpdateKorpa(Korpa korpa);
        void UpdateStatus(Korpa korpa);
        bool DostavljacEmpty(int id);
        void DostavljacPreuzima(Korpa korpa);
    }
}
