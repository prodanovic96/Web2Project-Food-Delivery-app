using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2Project.Models;
using Web2Project.Repository;

namespace Web2Project.Helper
{
    public class BasketRepository : IBasketRepository
    {
        DataBaseContext _dbcontext;

        public BasketRepository(DataBaseContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public void Add(Korpa korpa)
        {
            _dbcontext.Korpa.Add(korpa);
            _dbcontext.SaveChanges();
        }

        public void UpdateKorpa(Korpa korpa)
        {
            var result = _dbcontext.Korpa.Where(k => k.Id == korpa.Id).FirstOrDefault();

            if(result.AdresaDostave != korpa.AdresaDostave)
            {
                result.AdresaDostave = korpa.AdresaDostave;
            }

            result.VremePorucivanja = korpa.VremePorucivanja;
            result.Komentar = korpa.Komentar;

            _dbcontext.SaveChanges();
        }

        public void UpdateCene(Korpa korpa)
        {
            var result = _dbcontext.Korpa.Where(k => k.Id == korpa.Id).FirstOrDefault();

            result.Cena = korpa.Cena;

            _dbcontext.SaveChanges();
        }

        public void UpdateStatus(Korpa korpa)
        {
            var result = _dbcontext.Korpa.Where(k => k.Id == korpa.Id).FirstOrDefault();

            result.Status = korpa.Status;

            _dbcontext.SaveChanges();
        }

        public Korpa GetBasketByStatus(int potrosacId, Status status)
        {
            var result = _dbcontext.Korpa.Where(korpa => korpa.KorisnikId == potrosacId && korpa.Status == status).FirstOrDefault();

            return result;
        }

        public List<Korpa> GetFinishedBasket(int potrosacId)
        {
            var result = _dbcontext.Korpa.Where(k => k.KorisnikId == potrosacId && k.Status == Status.Zavrsena).ToList();

            return result;
        }

        public Korpa GetBasketById(int id)
        {
            var result = _dbcontext.Korpa.Where(korpa => korpa.Id == id).FirstOrDefault();

            return result;
        }

        public List<Korpa> GetAdminBasket()
        {
            var result = _dbcontext.Korpa.Where(k => k.Status == Status.CekaDostavljaca || k.Status == Status.Zavrsena || k.Status==Status.DostavljaSe).ToList();

            return result;
        }

        public List<Korpa> GetDostavljacBasket()
        {
            var result = _dbcontext.Korpa.Where(k => k.Status == Status.CekaDostavljaca).ToList();

            return result;
        }

        public Korpa GetDostavljacActiveBasket(int dostavljacId)
        {
            var result = _dbcontext.Korpa.Where(korpa => korpa.Status == Status.DostavljaSe && korpa.DostavljacId == dostavljacId).FirstOrDefault();

            return result;
        }

        public void DostavljacPreuzima(Korpa korpa)
        {
            var result = _dbcontext.Korpa.Where(k => k.Id == korpa.Id).FirstOrDefault();

            result.DostavljacId = korpa.DostavljacId;
            result.Status = korpa.Status;
            result.VremePorucivanja = DateTime.Now;

            _dbcontext.SaveChanges();
        }

        public bool DostavljacEmpty(int id)
        {
            var result = _dbcontext.Korpa.Where(k => k.Id == id).FirstOrDefault();

            if(result.DostavljacId == 0)
            {
                return true;
            }
            return false;
        }

        public List<Korpa> GetDostavljacFinishedBasket(int dostavljacId)
        {
            var result = _dbcontext.Korpa.Where(k => k.Status == Status.Zavrsena && k.DostavljacId == dostavljacId).ToList();

            return result;
        }

        public Korpa GetPotrosacActiveBasket(int potrosacId)
        {
            var result = _dbcontext.Korpa.Where(korpa => korpa.KorisnikId == potrosacId && korpa.Status == Status.DostavljaSe).FirstOrDefault();

            return result;
        }
    }
}
