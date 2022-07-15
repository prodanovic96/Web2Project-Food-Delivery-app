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
    public class DostavljacController : Controller
    {
        IProductRepository _productRepository;
        IBasketRepository _basketRepository;
        IBasketProductRepository _basketProductRepository;
        IUserRepository _userRepository;

        public DostavljacController(IProductRepository productRepository, IBasketRepository basketRepository, IBasketProductRepository basketProductRepository, IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _basketRepository = basketRepository;
            _basketProductRepository = basketProductRepository;
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            Korisnik dostavljac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                dostavljac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (dostavljac == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
                else
                {
                    if (dostavljac.TipKorisnika == Tip.ADMINISTRATOR)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if(dostavljac.TipKorisnika == Tip.POTROSAC)
                    {
                        return RedirectToAction("Index", "Potrosac");
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

            ViewBag.korisnik = dostavljac;
            return View();
        }

        public IActionResult NovePorudzbine()
        {
            Korisnik dostavljac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                dostavljac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (dostavljac == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
                else
                {
                    if (dostavljac.TipKorisnika == Tip.ADMINISTRATOR)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (dostavljac.TipKorisnika == Tip.POTROSAC)
                    {
                        return RedirectToAction("Index", "Potrosac");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Authentication");
            }
            ViewBag.korisnik = dostavljac;

            Korpa aktivna = _basketRepository.GetDostavljacActiveBasket(dostavljac.Id);

            if(aktivna != null)
                return RedirectToAction("TrenutnaPorudzbina");
            

            List<Korpa> porudzbine = _basketRepository.GetDostavljacBasket();

            Dictionary<int, float> cene = new Dictionary<int, float>();
            Dictionary<int, Korisnik> primaoci = new Dictionary<int, Korisnik>();

            List<Stavka> stavke = new List<Stavka>();

            foreach (var p in porudzbine)
            {
                cene.Add(p.Id, p.Cena - 400);
                
                List<KorpaProizvod> proizvodi = _basketProductRepository.GetAllProducts(p.Id);

                Korisnik primalac = _userRepository.Get(p.KorisnikId);
                primaoci.Add(p.Id, primalac);

                foreach (var item in proizvodi)
                {
                    Stavka stavka = new Stavka();
                    stavka.KorpaId = p.Id;

                    stavka.Proizvod = _productRepository.Get(item.ProizvodId);
                    stavka.Kolicina = item.Kolicina;

                    stavke.Add(stavka);
                }
            }

            string message = HttpContext.Session.GetString("AlertMessage");
            if (message != "" && message != null)
            {
                ViewBag.AlertMessage = message;
                ViewBag.Uspesno = JsonConvert.DeserializeObject<bool>(HttpContext.Session.GetString("Uspesno"));

                HttpContext.Session.SetString("AlertMessage", "");
            }


            ViewBag.Stavke = stavke;
            ViewBag.Primaoci = primaoci;
            ViewBag.Cene = cene;
            ViewBag.porudzbine = porudzbine;
            return View();
        }


        public IActionResult PreuzmiPorudzbinu(int id)
        {
            Korisnik dostavljac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                dostavljac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (dostavljac == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
                else
                {
                    if (dostavljac.TipKorisnika == Tip.ADMINISTRATOR)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (dostavljac.TipKorisnika == Tip.POTROSAC)
                    {
                        return RedirectToAction("Index", "Potrosac");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Authentication");
            }
            ViewBag.korisnik = dostavljac;

            Korpa aktivna = _basketRepository.GetDostavljacActiveBasket(dostavljac.Id);
            if (aktivna != null)
            {
                HttpContext.Session.SetString("AlertMessage", "Dostavljac trenutno dostavlja drugu porudzbinu!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                return RedirectToAction("TrenutnaPorudzbina");
            }

            if (!_basketRepository.DostavljacEmpty(id))
            {
                HttpContext.Session.SetString("AlertMessage", "Drugi dostavljac je uzeo ovu porudzbinu pre vas!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                return RedirectToAction("NovePorudzbine");
            }

            Korpa korpa = _basketRepository.GetBasketById(id);
            korpa.Status = Status.DostavljaSe;
            korpa.DostavljacId = dostavljac.Id;

            _basketRepository.DostavljacPreuzima(korpa);

            HttpContext.Session.SetString("AlertMessage", "Uspesno ste preuzeli porudzbinu!");
            HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));

            return RedirectToAction("TrenutnaPorudzbina");
        }

        public JsonResult Gotovo(string userdata)
        {
            Korpa porudzbina = _basketRepository.GetBasketById(int.Parse(userdata));

            HttpContext.Session.SetString("AlertMessage", "Porudzbina uspesno dostavljena!");
            HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));

            porudzbina.Status = Status.Zavrsena;
            _basketRepository.UpdateStatus(porudzbina);

            return Json(1);
        }


        public IActionResult TrenutnaPorudzbina()
        {
            Korisnik dostavljac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                dostavljac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (dostavljac == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
                else
                {
                    if (dostavljac.TipKorisnika == Tip.ADMINISTRATOR)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (dostavljac.TipKorisnika == Tip.POTROSAC)
                    {
                        return RedirectToAction("Index", "Potrosac");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Authentication");
            }
            ViewBag.korisnik = dostavljac;

            Korpa porudzbina = _basketRepository.GetDostavljacActiveBasket(dostavljac.Id);

            if(porudzbina == null)
            {

                return View("NemaDostave");
            }

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

            porudzbina.VremePorucivanja = DateTime.Now;

            ViewBag.Primalac = _userRepository.Get(porudzbina.KorisnikId);
            ViewBag.Korpa = porudzbina;
            ViewBag.Stavke = stavke;
            ViewBag.Cena = porudzbina.Cena - 400;
            return View();
        }

        [HttpPost]
        public JsonResult VratiVreme(string userdata)
        {
            Korpa korpa = _basketRepository.GetBasketById(int.Parse(userdata));

            DateTime broj = korpa.VremePorucivanja.AddSeconds(30);

            TimeSpan span = broj - DateTime.Now;
            
            double broj1 = span.TotalMilliseconds;

            return Json(broj1);
        }



        public IActionResult MojePorudzbine()
        {
            Korisnik dostavljac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                dostavljac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (dostavljac == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
                else
                {
                    if (dostavljac.TipKorisnika == Tip.ADMINISTRATOR)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (dostavljac.TipKorisnika == Tip.POTROSAC)
                    {
                        return RedirectToAction("Index", "Potrosac");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Authentication");
            }
            ViewBag.korisnik = dostavljac;

            List<Korpa> porudzbine = _basketRepository.GetDostavljacFinishedBasket(dostavljac.Id);

            Dictionary<int, float> cene = new Dictionary<int, float>();
            Dictionary<int, Korisnik> primaoci = new Dictionary<int, Korisnik>();

            List<Stavka> stavke = new List<Stavka>();

            foreach (var p in porudzbine)
            {
                cene.Add(p.Id, p.Cena - 400);

                List<KorpaProizvod> proizvodi = _basketProductRepository.GetAllProducts(p.Id);

                Korisnik primalac = _userRepository.Get(p.KorisnikId);
                primaoci.Add(p.Id, primalac);

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
            ViewBag.Primaoci = primaoci;
            ViewBag.Cene = cene;
            ViewBag.porudzbine = porudzbine;
            return View();
        }
    }
}
