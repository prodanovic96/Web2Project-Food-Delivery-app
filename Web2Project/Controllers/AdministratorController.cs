using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2Project.Helper;
using Web2Project.Models;

namespace Web2Project.Controllers
{
    public class AdministratorController : Controller
    {
        IProductRepository _productRepository;
        ICategoryRepository _categoryRepository;
        IUserRepository _userRepository;
        IEmailSender _emailSender;

        public AdministratorController(IProductRepository productRepository, ICategoryRepository categoryRepository, IUserRepository userRepository, IEmailSender emailSender)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            Korisnik administrator = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                administrator = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (administrator == null)
                {
                    administrator = new Korisnik();
                }
                else
                {
                    if (administrator.TipKorisnika == Tip.POTROSAC)
                    {
                        return RedirectToAction("Index", "Potrosac");
                    }
                    else if (administrator.TipKorisnika == Tip.DOSTAVLJAC)
                    {
                        return RedirectToAction("Index", "Dostavljac");
                    }
                }
            }
            ViewBag.korisnik = administrator;
            return View();
        }

        public IActionResult DodajProizvod()
        {
            Korisnik administrator = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                administrator = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (administrator == null)
                {
                    administrator = new Korisnik();
                }
                else
                {
                    if (administrator.TipKorisnika == Tip.POTROSAC)
                    {
                        return RedirectToAction("Index", "Potrosac");
                    }
                    else if (administrator.TipKorisnika == Tip.DOSTAVLJAC)
                    {
                        return RedirectToAction("Index", "Dostavljac");
                    }
                }
            }
            ViewBag.kategorije = _categoryRepository.GetAll();
            ViewBag.korisnik = administrator;
            return View();
        }

        [HttpPost]
        public IActionResult ProizvodDodat(Proizvod proizvod)
        {
            _productRepository.Add(proizvod);

            return RedirectToAction("Index");
        }

        public IActionResult DodajKategoriju()
        {
            Korisnik administrator = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                administrator = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (administrator == null)
                {
                    administrator = new Korisnik();
                }
                else
                {
                    if (administrator.TipKorisnika == Tip.POTROSAC)
                    {
                        return RedirectToAction("Index", "Potrosac");
                    }
                    else if (administrator.TipKorisnika == Tip.DOSTAVLJAC)
                    {
                        return RedirectToAction("Index", "Dostavljac");
                    }
                }
            }
            ViewBag.korisnik = administrator;
            return View();
        }

        [HttpPost]
        public IActionResult KategorijaDodata(Kategorija kategorija)
        {
            _categoryRepository.Add(kategorija);
            return RedirectToAction("Index");
        }

        public IActionResult Verifikuj()
        {
            Korisnik administrator = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                administrator = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (administrator == null)
                {
                    administrator = new Korisnik();
                }
                else
                {
                    if (administrator.TipKorisnika == Tip.POTROSAC)
                    {
                        return RedirectToAction("Index", "Potrosac");
                    }
                    else if (administrator.TipKorisnika == Tip.DOSTAVLJAC)
                    {
                        return RedirectToAction("Index", "Dostavljac");
                    }
                }
            }

            ViewBag.dostavljaciPrihvaceni = _userRepository.GetDostavljaciPrihvaceni();
            ViewBag.dostavljaci = _userRepository.GetDostavljaci();
            ViewBag.korisnik = administrator;
            return View();
        }

        [HttpPost]
        public IActionResult Odobren(int id)
        {
            // Mejl bi trebao da bude poslat na korisnik.Email
            Message message = new Message(new string[] { "markoprodanovic96@gmail.com" }, "[Web 2] - Projekat", "Vas profil je verifikovan, mozete poceti sa radom!");
            _emailSender.SendEmail(message);

            Korisnik korisnik = _userRepository.Get(id);
            _userRepository.UpdateKorisnik(korisnik, "Verifikovan", Zahtev.PRIHVACEN.ToString());

            return RedirectToAction("Verifikuj");
        }

        [HttpPost]
        public IActionResult Odbijen(int id)
        {
            Message message = new Message(new string[] { "obojenigel@gmail.com" }, "[Web 2] - Projekat", "Vas profil je odbijen od strane administratora, ne mozete koristiti usluge naseg sistema!");
            _emailSender.SendEmail(message);

            Korisnik korisnik = _userRepository.Get(id);
            _userRepository.UpdateKorisnik(korisnik, "Verifikovan", Zahtev.ODBIJEN.ToString());

            return RedirectToAction("Verifikuj");
        }
    }
}
