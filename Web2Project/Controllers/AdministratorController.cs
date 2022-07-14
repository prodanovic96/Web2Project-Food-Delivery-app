using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
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
        IFileUploadService _fileUploadService;
        IBasketRepository _basketRepository;
        IBasketProductRepository _basketProductRepository;

        public AdministratorController(IProductRepository productRepository, ICategoryRepository categoryRepository, IUserRepository userRepository, IEmailSender emailSender, IFileUploadService fileUploadService, IBasketRepository basketRepository, IBasketProductRepository basketProductRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _emailSender = emailSender;
            _fileUploadService = fileUploadService;
            _basketRepository = basketRepository;
            _basketProductRepository = basketProductRepository;
        }

        public IActionResult Index()
        {
            Korisnik administrator = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                administrator = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (administrator == null)
                {
                    return RedirectToAction("Index", "Authentication");
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

            ViewBag.korisnik = administrator;
            return View();
        }


        //  PROIZVOD
        public IActionResult Proizvodi()
        {
            Korisnik administrator = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                administrator = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (administrator == null)
                {
                    return RedirectToAction("Index", "Authentication");
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
            else
            {
                return RedirectToAction("Index", "Authentication");
            }

            ViewBag.korisnik = administrator;

            List<Proizvod> proizvodi = _productRepository.GetAllProduct();
            proizvodi = proizvodi.OrderBy(p => p.KategorijaId).ToList();
            ViewBag.Proizvodi = proizvodi;

            string message = HttpContext.Session.GetString("AlertMessage");
            if (message != "" && message != null)
            {
                ViewBag.AlertMessage = message;
                ViewBag.Uspesno = JsonConvert.DeserializeObject<bool>(HttpContext.Session.GetString("Uspesno"));

                HttpContext.Session.SetString("AlertMessage", "");
            }

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
                    return RedirectToAction("Index", "Authentication");
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
            else
            {
                return RedirectToAction("Index", "Authentication");
            }
            ViewBag.kategorije = _categoryRepository.GetAll();
            ViewBag.korisnik = administrator;

            string message = HttpContext.Session.GetString("AlertMessage");
            if (message != "" && message != null)
            {
                ViewBag.AlertMessage = message;
                ViewBag.Uspesno = JsonConvert.DeserializeObject<bool>(HttpContext.Session.GetString("Uspesno"));

                HttpContext.Session.SetString("AlertMessage", "");
            }

            return View();
        }

        [HttpPost]
        public IActionResult ProizvodDodat(Proizvod proizvod, IFormFile ifile)
        {
            Korisnik administrator = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                administrator = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (administrator == null)
                {
                    return RedirectToAction("Index", "Authentication");
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
            else
            {
                return RedirectToAction("Index", "Authentication");
            }

            if (proizvod.Naziv == null || proizvod.Naziv == "" || proizvod.Sastojci == null || proizvod.Sastojci == "" || proizvod.Cena < 1)
            {
                HttpContext.Session.SetString("AlertMessage", "Sva polja moraju biti pravilno popunjena!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                return RedirectToAction("DodajProizvod");
            }

            if (_productRepository.Existing(proizvod.Naziv))
            {
                HttpContext.Session.SetString("AlertMessage", "Proizvod sa ovim nazivom vec postoji!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                return RedirectToAction("DodajProizvod");
            }

            if (ifile != null)
            {
                string imgext = Path.GetExtension(ifile.FileName).ToLower();
                if (imgext != ".jpg" && imgext != ".png")
                {
                    HttpContext.Session.SetString("AlertMessage", "Slika mora biti u .jpg ili .png formatu!");
                    HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                    return RedirectToAction("DodajProizvod");
                }
            }
            _productRepository.Add(proizvod);

            string imagePath = "";

            if (ifile != null)
            {
                _fileUploadService.UploadFile(ifile, proizvod.Id.ToString(), "Proizvodi");
                imagePath = "~/Proizvodi/" + proizvod.Id + ".jpg";
            }
            else
            {
                imagePath = "~/Proizvodi/unknown.jpg";
            }

            _productRepository.UpdateProperty(proizvod, "ImagePath", imagePath);
            HttpContext.Session.SetString("AlertMessage", "Novi proizvod uspesno dodat!");
            HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));

            return RedirectToAction("Proizvodi");
        }

        public IActionResult ObrisiProizvod(int id)
        {
            if (_productRepository.Existing(id))
            {
                _productRepository.DeleteProduct(id);

                HttpContext.Session.SetString("AlertMessage", "Proizvod uspesno obrisan!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));
            }
            else
            {
                HttpContext.Session.SetString("AlertMessage", "Proizvod je vec obrisan!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));
            }

            return RedirectToAction("Proizvodi");
        }

        public IActionResult IzmeniProizvod(int id)
        {
            Korisnik administrator = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                administrator = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (administrator == null)
                {
                    return RedirectToAction("Index", "Authentication");
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
            else
            {
                return RedirectToAction("Index", "Authentication");
            }
            ViewBag.korisnik = administrator;


            ViewBag.kategorije = _categoryRepository.GetAll();

            Proizvod proizvod = _productRepository.Get(id);
            ViewBag.Kategorija = _categoryRepository.Get(proizvod.KategorijaId);

            ViewBag.Proizvod = proizvod;

            string message = HttpContext.Session.GetString("AlertMessage");
            if (message != "" && message != null)
            {
                ViewBag.AlertMessage = message;
                ViewBag.Uspesno = JsonConvert.DeserializeObject<bool>(HttpContext.Session.GetString("Uspesno"));

                HttpContext.Session.SetString("AlertMessage", "");
            }

            return View();
        }

        [HttpPost]
        public IActionResult ProizvodIzmenjen(int id, Proizvod noviProizvod, IFormFile ifile)
        {
            Korisnik administrator = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                administrator = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));
                if (administrator == null)
                {
                    return RedirectToAction("Index", "Authentication");
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
            else
            {
                return RedirectToAction("Index", "Authentication");
            }

            bool flag = false;

            if (noviProizvod.Naziv != null || noviProizvod.Sastojci != null || noviProizvod.Cena > 0 || noviProizvod.KategorijaId > 0 || ifile != null)
            {
                Proizvod proizvod = _productRepository.Get(id);

                if (proizvod == null)
                {
                    HttpContext.Session.SetString("AlertMessage", "Proizvod je u medjuvremenu obrisan!");
                    HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                    return RedirectToAction("Proizvodi");
                }

                if (noviProizvod.Naziv != null && noviProizvod.Naziv != "" && noviProizvod.Naziv != proizvod.Naziv)
                {
                    proizvod.Naziv = noviProizvod.Naziv;
                    flag = true;

                    _productRepository.UpdateProperty(proizvod, "Naziv", proizvod.Naziv);
                }

                if (noviProizvod.Cena > 0 && noviProizvod.Cena != proizvod.Cena)
                {
                    proizvod.Cena = noviProizvod.Cena;
                    flag = true;

                    _productRepository.UpdateProperty(proizvod, "Cena", proizvod.Cena);
                }

                if (noviProizvod.Sastojci != null && noviProizvod.Sastojci != "" && noviProizvod.Sastojci != proizvod.Sastojci)
                {
                    proizvod.Sastojci = noviProizvod.Sastojci;
                    flag = true;

                    _productRepository.UpdateProperty(proizvod, "Sastojci", proizvod.Sastojci);
                }

                if (noviProizvod.KategorijaId != 0 && noviProizvod.KategorijaId != proizvod.KategorijaId)
                {
                    proizvod.KategorijaId = noviProizvod.KategorijaId;
                    flag = true;

                    _productRepository.UpdateProperty(proizvod, "KategorijaId", proizvod.KategorijaId);
                }

                if (ifile != null)
                {
                    string imgext = Path.GetExtension(ifile.FileName).ToLower();
                    if (imgext != ".jpg" && imgext != ".png")
                    {
                        HttpContext.Session.SetString("AlertMessage", "Slika mora biti u .jpg ili .png formatu!");
                        HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                        return RedirectToAction("Proizvodi");
                    }

                    flag = true;
                    _fileUploadService.UploadFile(ifile, proizvod.Id.ToString(), "Proizvodi");

                    if (proizvod.ImagePath.Contains("unknown.jpg"))
                    {
                        proizvod.ImagePath = "~/Proizvodi/" + proizvod.Id + ".jpg";
                        _productRepository.UpdateProperty(proizvod, "ImagePath", proizvod.ImagePath);
                    }
                }
            }

            if (flag)
            {
                HttpContext.Session.SetString("AlertMessage", "Proizvod uspesno izmenjen!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));
            }
            else
            {
                HttpContext.Session.SetString("AlertMessage", "Proizvod nije izmenjen!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));
            }

            return RedirectToAction("Proizvodi");
        }

        // KATEGORIJE
        public IActionResult Kategorije()
        {
            Korisnik administrator = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                administrator = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (administrator == null)
                {
                    return RedirectToAction("Index", "Authentication");
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
            else
            {
                return RedirectToAction("Index", "Authentication");
            }

            string message = HttpContext.Session.GetString("AlertMessage");
            if(message != "" && message != null)
            {
                ViewBag.AlertMessage = message;
                ViewBag.Uspesno = JsonConvert.DeserializeObject<bool>(HttpContext.Session.GetString("Uspesno"));

                HttpContext.Session.SetString("AlertMessage", "");
            }
            
            ViewBag.korisnik = administrator;
            ViewBag.Kategorije = _categoryRepository.GetAll();

            return View();
        }

        public IActionResult IzmeniKategoriju(int id)
        {
            Korisnik administrator = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                administrator = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (administrator == null)
                {
                    return RedirectToAction("Index", "Authentication");
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
            else
            {
                return RedirectToAction("Index", "Authentication");
            } 

            ViewBag.korisnik = administrator;
            ViewBag.Id = id;

            return View();
        }

        public IActionResult KategorijaIzmenjena(int id, string Naziv)
        {
            if (!_categoryRepository.Existing(id))
            {
                HttpContext.Session.SetString("AlertMessage", "Kategorija je u medjuvremenu obrisana!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                return RedirectToAction("Kategorije");
            }

            if (_categoryRepository.Existing(Naziv))
            {
                HttpContext.Session.SetString("AlertMessage", "Kategorija sa ovakvim nazivom vec postoji!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                return RedirectToAction("Kategorije");
            }

            Kategorija kategorija = _categoryRepository.Get(id);
            _categoryRepository.UpdateProperty(kategorija, Naziv);

            HttpContext.Session.SetString("AlertMessage", "Kategorija uspesno izmenjena!");
            HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));

            return RedirectToAction("Kategorije");
        }

        public IActionResult ObrisiKategoriju(int id)
        {
            if (_categoryRepository.Existing(id))
            {
                _categoryRepository.DeleteCategory(id);

                HttpContext.Session.SetString("AlertMessage", "Kategorija uspesno obrisana!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));
            }
            else
            {
                HttpContext.Session.SetString("AlertMessage", "Kategorija je vec obrisana!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));
            }

            return RedirectToAction("Kategorije");
        }

        public IActionResult DodajKategoriju()
        {
            Korisnik administrator = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                administrator = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (administrator == null)
                {
                    return RedirectToAction("Index", "Authentication");
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

            ViewBag.korisnik = administrator;
            return View();
        }

        [HttpPost]
        public IActionResult KategorijaDodata(Kategorija kategorija)
        {
            Korisnik administrator = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                administrator = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (administrator == null)
                {
                    return RedirectToAction("Index", "Authentication");
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
            else
            {
                return RedirectToAction("Index", "Authentication");
            }

            if (kategorija.Naziv == "" || kategorija.Naziv == null)
            {
                HttpContext.Session.SetString("AlertMessage", "Sva polja moraju biti pravilno popunjena!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));
                return RedirectToAction("DodajKategoriju");
            }

            if (_categoryRepository.Existing(kategorija.Naziv))
            {
                HttpContext.Session.SetString("AlertMessage", "Kategorija sa ovim nazivom vec postoji!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));
                return RedirectToAction("DodajKategoriju");
            }

            _categoryRepository.Add(kategorija);
            HttpContext.Session.SetString("AlertMessage", "Kategorija uspesno dodata!");
            HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));

            return RedirectToAction("Kategorije");
        }


        // VERIFIKACIJA
        public IActionResult Verifikuj()
        {
            Korisnik administrator = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                administrator = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (administrator == null)
                {
                    return RedirectToAction("Index", "Authentication");
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


            ViewBag.dostavljaciPrihvaceni = _userRepository.GetDostavljaciPrihvaceni();
            ViewBag.dostavljaci = _userRepository.GetDostavljaci();
            ViewBag.korisnik = administrator;
            return View();
        }

        [HttpPost]
        public IActionResult Odobren(int id)
        {
            Korisnik korisnik = _userRepository.Get(id);

            if(korisnik.Verifikovan == Zahtev.PROCESIRA_SE)
            {
                _userRepository.UpdateKorisnik(korisnik, "Verifikovan", Zahtev.PRIHVACEN.ToString());

                // Mejl bi trebao da bude poslat na korisnik.Email
                Message message = new Message(new string[] { "markoprodanovic96@gmail.com" }, "[Web 2] - Projekat", "Vas profil je verifikovan, mozete poceti sa radom!");
                _emailSender.SendEmail(message);

                HttpContext.Session.SetString("AlertMessage", "Dostavljac uspesno verifikovan!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));
            }
            else
            {
                HttpContext.Session.SetString("AlertMessage", "Ovaj zahtev je vec obradjen od strane drugog administratora!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));
            }

            return RedirectToAction("Verifikuj");
        }

        [HttpPost]
        public IActionResult Odbijen(int id)
        {
            Korisnik korisnik = _userRepository.Get(id);

            if(korisnik.Verifikovan == Zahtev.PROCESIRA_SE)
            {
                _userRepository.UpdateKorisnik(korisnik, "Verifikovan", Zahtev.ODBIJEN.ToString());

                Message message = new Message(new string[] { "obojenigel@gmail.com" }, "[Web 2] - Projekat", "Vas nalog je odbijen od strane administratora, ne mozete koristiti usluge naseg sistema!");
                _emailSender.SendEmail(message);

                HttpContext.Session.SetString("AlertMessage", "Dostavljac uspesno odbijen!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));
            }
            else
            {
                HttpContext.Session.SetString("AlertMessage", "Ovaj zahtev je vec obradjen od strane drugog administratora!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));
            }

            return RedirectToAction("Verifikuj");
        }


        public IActionResult SvePorudzbine()
        {
            Korisnik administrator = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                administrator = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (administrator == null)
                {
                    return RedirectToAction("Index", "Authentication");
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
            else
            {
                return RedirectToAction("Index", "Authentication");
            }
            ViewBag.korisnik = administrator;

            List<Korpa> porudzbine = _basketRepository.GetAdminBasket();

            Dictionary<int, float> cene = new Dictionary<int, float>();
            Dictionary<int, Korisnik> primaoci = new Dictionary<int, Korisnik>();
            Dictionary<int, Korisnik> dostavljaci = new Dictionary<int, Korisnik>();

            List<Stavka> stavke = new List<Stavka>();

            foreach (var p in porudzbine)
            {
                cene.Add(p.Id, p.Cena - 400);

                List<KorpaProizvod> proizvodi = _basketProductRepository.GetAllProducts(p.Id);

                Korisnik primalac = _userRepository.Get(p.KorisnikId);
                primaoci.Add(p.Id, primalac);

                Korisnik dostavljac = _userRepository.Get(p.DostavljacId);
                if(dostavljac == null)
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
            ViewBag.Primaoci = primaoci;
            ViewBag.Dostavljaci = dostavljaci;
            ViewBag.Cene = cene;
            ViewBag.porudzbine = porudzbine;
            return View();
        }



        // PROVERE NA FRONTU
        public JsonResult CheckCategoryAvailability(string userdata)
        {
            if (_categoryRepository.Existing(userdata))
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        public JsonResult CheckProductAvailability(string userdata)
        {
            if (_productRepository.Existing(userdata))
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
    }
}
