using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Web2Project.Helper;
using Web2Project.Models;

namespace Web2Project.Controllers
{
    public class PotrosacController : Controller
    {
        IProductRepository _productRepository;
        IBasketRepository _basketRepository;
        IBasketProductRepository _basketProductRepository;
        IUserRepository _userRepository;

        public PotrosacController(IProductRepository productRepository, IBasketRepository basketRepository, IBasketProductRepository basketProductRepository, IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _basketRepository = basketRepository;
            _basketProductRepository = basketProductRepository;
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            Korisnik posetilac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                posetilac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (posetilac == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
                else
                {
                    if (posetilac.TipKorisnika == Tip.ADMINISTRATOR)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (posetilac.TipKorisnika == Tip.DOSTAVLJAC)
                    {
                        return RedirectToAction("Index", "Dostavljac");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Authentication");
            }

            string message = HttpContext.Session.GetString("AlertMessage");
            if (message != "" && message != null)
            {
                ViewBag.AlertMessage = message;
                ViewBag.Uspesno = JsonConvert.DeserializeObject<bool>(HttpContext.Session.GetString("Uspesno"));

                HttpContext.Session.SetString("AlertMessage", "");
            }

            ViewBag.korisnik = posetilac;
            return View();
        }

        public IActionResult NovaPorudzbina()
        {
            Korisnik potrosac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                potrosac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (potrosac == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
                else
                {
                    if (potrosac.TipKorisnika == Tip.ADMINISTRATOR)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (potrosac.TipKorisnika == Tip.DOSTAVLJAC)
                    {
                        return RedirectToAction("Index", "Dostavljac");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Authentication");
            }

            ViewBag.korisnik = potrosac;

            Korpa korpaDostavljaSe = _basketRepository.GetBasketByStatus(potrosac.Id, Status.DostavljaSe);
            if (korpaDostavljaSe != null)
                return RedirectToAction("TrenutnaPorudzbina");

            Korpa korpaCeka = _basketRepository.GetBasketByStatus(potrosac.Id, Status.CekaDostavljaca);

            if (korpaCeka != null)
                return RedirectToAction("PrikaziPorudzbinu");

            string message = HttpContext.Session.GetString("AlertMessage");
            if (message != "" && message != null)
            {
                ViewBag.AlertMessage = message;
                ViewBag.Uspesno = JsonConvert.DeserializeObject<bool>(HttpContext.Session.GetString("Uspesno"));

                HttpContext.Session.SetString("AlertMessage", "");
            }

            List<Proizvod> proizvodi = _productRepository.GetAllProduct();
            proizvodi = proizvodi.OrderBy(p => p.KategorijaId).ToList();
            ViewBag.Proizvodi = proizvodi;

            return View();
        }

        public IActionResult DodajuKorpu(int id)
        {
            Korisnik potrosac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                potrosac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (potrosac == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
                else
                {
                    if (potrosac.TipKorisnika == Tip.ADMINISTRATOR)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (potrosac.TipKorisnika == Tip.DOSTAVLJAC)
                    {
                        return RedirectToAction("Index", "Dostavljac");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Authentication");
            }

            Proizvod proizvod = _productRepository.Get(id);

            if(proizvod == null)
            {
                HttpContext.Session.SetString("AlertMessage", "Ovaj proizvod je u medjuvremenu obrisan!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                return RedirectToAction("NovaPorudzbina");
            }

            Korpa korpa = _basketRepository.GetBasketByStatus(potrosac.Id, Status.FormiraSe);

            if (korpa == null)
            {
                korpa = new Korpa();
                korpa.Status = Status.FormiraSe;
                korpa.KorisnikId = potrosac.Id;
                korpa.AdresaDostave = potrosac.Adresa;

                _basketRepository.Add(korpa);
            }

            KorpaProizvod korpaProizvod = _basketProductRepository.Existing(proizvod.Id, korpa.Id);

            if(korpaProizvod == null)
            {
                korpaProizvod = new KorpaProizvod();

                korpaProizvod.ProizvodId = proizvod.Id;
                korpaProizvod.KorpaId = korpa.Id;

                _basketProductRepository.Add(korpaProizvod);
            }
            else
            {
                korpaProizvod.Kolicina++;
            }


            korpa.Dodaj(proizvod.Cena);
            _basketRepository.UpdateCene(korpa);

            HttpContext.Session.SetString("AlertMessage", "Proizvod uspesno dodat u korpu!");
            HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));

            return RedirectToAction("NovaPorudzbina");
        }

        public IActionResult UkloniIzKorpe(int id)
        {
            Korisnik potrosac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                potrosac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (potrosac == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
                else
                {
                    if (potrosac.TipKorisnika == Tip.ADMINISTRATOR)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (potrosac.TipKorisnika == Tip.DOSTAVLJAC)
                    {
                        return RedirectToAction("Index", "Dostavljac");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Authentication");
            }
            Korpa korpa = _basketRepository.GetBasketByStatus(potrosac.Id, Status.FormiraSe);
            Proizvod proizvod = _productRepository.Get(id);
            KorpaProizvod korpaProizvod = _basketProductRepository.Existing(proizvod.Id, korpa.Id);

            korpa.Oduzmi(proizvod.Cena);
            _basketRepository.UpdateCene(korpa);

            _basketProductRepository.SmanjiKolicinu(korpaProizvod.Id);

            HttpContext.Session.SetString("AlertMessage", "Proizvod uspesno ukljonjen iz korpe!");
            HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));

            return RedirectToAction("MojaKorpa");
        }

        public IActionResult MojaKorpa()
        {
            Korisnik posetilac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                posetilac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (posetilac == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
                else
                {
                    if (posetilac.TipKorisnika == Tip.ADMINISTRATOR)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (posetilac.TipKorisnika == Tip.DOSTAVLJAC)
                    {
                        return RedirectToAction("Index", "Dostavljac");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Authentication");
            }
            ViewBag.korisnik = posetilac;

            Korpa korpaDostavljaSe = _basketRepository.GetBasketByStatus(posetilac.Id, Status.DostavljaSe);
            if (korpaDostavljaSe != null)
                return RedirectToAction("TrenutnaPorudzbina");

            Korpa korpaCeka = _basketRepository.GetBasketByStatus(posetilac.Id, Status.CekaDostavljaca);

            if (korpaCeka != null)
                return RedirectToAction("PrikaziPorudzbinu");

            Korpa korpa = _basketRepository.GetBasketByStatus(posetilac.Id, Status.FormiraSe);

            if(korpa == null)
            {
                korpa = new Korpa();

                ViewBag.Cena = korpa.Cena - 400;
                ViewBag.CenaSaDostavom = korpa.Cena;

                ViewBag.Stavke = new List<Stavka>();
                return View();
            }

            List<KorpaProizvod> proizvodi = _basketProductRepository.GetAllProducts(korpa.Id);

            List<Stavka> stavke = new List<Stavka>();      

            foreach(var item in proizvodi)
            {
                Stavka stavka = new Stavka();
                stavka.Proizvod = _productRepository.Get(item.ProizvodId);
                stavka.Kolicina = item.Kolicina;

                stavke.Add(stavka);
            }

            string message = HttpContext.Session.GetString("AlertMessage");
            if (message != "" && message != null)
            {
                ViewBag.AlertMessage = message;
                ViewBag.Uspesno = JsonConvert.DeserializeObject<bool>(HttpContext.Session.GetString("Uspesno"));

                HttpContext.Session.SetString("AlertMessage", "");
            }

            ViewBag.Cena = korpa.Cena - 400;
            ViewBag.CenaSaDostavom = korpa.Cena;
            ViewBag.Stavke = stavke;
            return View();
        }

        public IActionResult DovrsiNarudzbu()
        {
            Korisnik potrosac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                potrosac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (potrosac == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
                else
                {
                    if (potrosac.TipKorisnika == Tip.ADMINISTRATOR)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (potrosac.TipKorisnika == Tip.DOSTAVLJAC)
                    {
                        return RedirectToAction("Index", "Dostavljac");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Authentication");
            }

            ViewBag.korisnik = potrosac;
            ViewBag.korpa = _basketRepository.GetBasketByStatus(potrosac.Id, Status.FormiraSe);
            return View();
        }

        public IActionResult NarudzbaPotvrdjena(string Adresa, string Komentar)
        {
            Korisnik potrosac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                potrosac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (potrosac == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
                else
                {
                    if (potrosac.TipKorisnika == Tip.ADMINISTRATOR)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (potrosac.TipKorisnika == Tip.DOSTAVLJAC)
                    {
                        return RedirectToAction("Index", "Dostavljac");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Authentication");
            }
            Korpa korpa = _basketRepository.GetBasketByStatus(potrosac.Id, Status.FormiraSe);

            if(Komentar == null)
            {
                Komentar = "";
            }

            korpa.AdresaDostave = Adresa;
            korpa.Komentar = Komentar;
            korpa.VremePorucivanja = DateTime.Now;

            _basketRepository.UpdateKorpa(korpa);

            korpa.Status = Status.CekaDostavljaca;
            _basketRepository.UpdateStatus(korpa);

            HttpContext.Session.SetString("AlertMessage", "Porudzbina uspesno kreirana, ceka se dostavljac da je preuzme!");
            HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));

            return RedirectToAction("MojaKorpa");
        }

        public IActionResult PrikaziPorudzbinu()
        {
            Korisnik posetilac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                posetilac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (posetilac == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
                else
                {
                    if (posetilac.TipKorisnika == Tip.ADMINISTRATOR)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (posetilac.TipKorisnika == Tip.DOSTAVLJAC)
                    {
                        return RedirectToAction("Index", "Dostavljac");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Authentication");
            }
            ViewBag.korisnik = posetilac;

            Korpa korpaCeka = _basketRepository.GetBasketByStatus(posetilac.Id, Status.CekaDostavljaca);

            List<KorpaProizvod> proizvodi = _basketProductRepository.GetAllProducts(korpaCeka.Id);

            List<Stavka> stavke = new List<Stavka>();
            foreach (var item in proizvodi)
            {
                Stavka stavka = new Stavka();
                stavka.Proizvod = _productRepository.Get(item.ProizvodId);
                stavka.Kolicina = item.Kolicina;

                stavke.Add(stavka);
            }

            string message = HttpContext.Session.GetString("AlertMessage");
            if (message != "" && message != null)
            {
                ViewBag.AlertMessage = message;
                ViewBag.Uspesno = JsonConvert.DeserializeObject<bool>(HttpContext.Session.GetString("Uspesno"));

                HttpContext.Session.SetString("AlertMessage", "");
            }


            ViewBag.Korpa = korpaCeka;
            ViewBag.Stavke = stavke;
            ViewBag.Cena = korpaCeka.Cena - 400;           
            return View();
        }

        public IActionResult MojePorudzbine()
        {
            Korisnik posetilac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                posetilac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (posetilac == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
                else
                {
                    if (posetilac.TipKorisnika == Tip.ADMINISTRATOR)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (posetilac.TipKorisnika == Tip.DOSTAVLJAC)
                    {
                        return RedirectToAction("Index", "Dostavljac");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Authentication");
            }
            ViewBag.korisnik = posetilac;

            List<Korpa> porudzbine = _basketRepository.GetFinishedBasket(posetilac.Id);

            Dictionary<int, float> cene = new Dictionary<int, float>();
            Dictionary<int, Korisnik> dostavljaci = new Dictionary<int, Korisnik>();

            List<Stavka> stavke = new List<Stavka>();

            foreach (var p in porudzbine)
            {
                cene.Add(p.Id, p.Cena - 400);

                List<KorpaProizvod> proizvodi = _basketProductRepository.GetAllProducts(p.Id);                

                Korisnik dostavljac = _userRepository.Get(p.DostavljacId);
                if (dostavljac == null)
                {
                    dostavljac = new Korisnik();
                }
                dostavljaci.Add(p.Id, dostavljac);

                foreach (var item in proizvodi)
                {
                    Stavka stavka = new Stavka();
                    stavka.KorpaId = p.Id;

                    stavka.Proizvod = _productRepository.Get(item.ProizvodId);
                    stavka.Kolicina = item.Kolicina;

                    stavke.Add(stavka);
                }
            }

            ViewBag.Stavke = stavke;
            ViewBag.Dostavljaci = dostavljaci;
            ViewBag.Cene = cene;
            ViewBag.porudzbine = porudzbine;
            return View();
        }

        public IActionResult TrenutnaPorudzbina()
        {
            Korisnik posetilac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                posetilac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (posetilac == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
                else
                {
                    if (posetilac.TipKorisnika == Tip.ADMINISTRATOR)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (posetilac.TipKorisnika == Tip.DOSTAVLJAC)
                    {
                        return RedirectToAction("Index", "Dostavljac");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Authentication");
            }
            ViewBag.korisnik = posetilac;

            Korpa porudzbina = _basketRepository.GetPotrosacActiveBasket(posetilac.Id);
            List<KorpaProizvod> proizvodi = _basketProductRepository.GetAllProducts(porudzbina.Id);

            List<Stavka> stavke = new List<Stavka>();
            foreach (var item in proizvodi)
            {
                Stavka stavka = new Stavka();
                stavka.Proizvod = _productRepository.Get(item.ProizvodId);
                stavka.Kolicina = item.Kolicina;

                stavke.Add(stavka);
            }

            string message = HttpContext.Session.GetString("AlertMessage");
            if (message != "" && message != null)
            {
                ViewBag.AlertMessage = message;
                ViewBag.Uspesno = JsonConvert.DeserializeObject<bool>(HttpContext.Session.GetString("Uspesno"));

                HttpContext.Session.SetString("AlertMessage", "");
            }

            ViewBag.Dostavljac = _userRepository.Get(porudzbina.DostavljacId);
            ViewBag.Korpa = porudzbina;
            ViewBag.Stavke = stavke;
            ViewBag.Cena = porudzbina.Cena - 400;
            return View();
        }

        public JsonResult Pocelo(string userdata)
        {
            Korpa porudzbina = _basketRepository.GetBasketById(int.Parse(userdata));

            if(porudzbina.Status == Status.DostavljaSe)
            {
                HttpContext.Session.SetString("AlertMessage", "Dostavljac je preuzeo vasu porudzbinu!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));

                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        public JsonResult Gotovo()
        {
            HttpContext.Session.SetString("AlertMessage", "Porudzbina Vam je uspesno dostavljena!");
            HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));

            return Json(1);
        }
    }
}
