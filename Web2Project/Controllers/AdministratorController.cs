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

        public AdministratorController(IProductRepository productRepository, ICategoryRepository categoryRepository, IUserRepository userRepository, IEmailSender emailSender, IFileUploadService fileUploadService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _emailSender = emailSender;
            _fileUploadService = fileUploadService;
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

            ViewBag.korisnik = administrator;
            return View();
        }

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
                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Sva polja moraju biti pravilno popunjena!"));
                return RedirectToAction("DodajProizvod");
            }

            if (_productRepository.Existing(proizvod.Naziv))
            {
                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Proizvod sa ovim nazivom vec postoji!"));
                return RedirectToAction("DodajProizvod");
            }

            if (ifile != null)
            {
                string imgext = Path.GetExtension(ifile.FileName).ToLower();
                if (imgext != ".jpg" && imgext != ".png")
                {
                    ViewBag.Message = "Slika mora biti u .jpg ili .png formatu!";
                    ViewBag.korisnik = administrator;
                    ViewBag.kategorije = _categoryRepository.GetAll();
                    return View("DodajProizvod");
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
            HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Novi proizvod uspesno dodat!"));

            return RedirectToAction("Proizvodi");
        }

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

            if (!_categoryRepository.Existing(id))
            {
                // Ispis da je kategorija obrisana
                return RedirectToAction("Kategorije");
            }

            ViewBag.korisnik = administrator;
            ViewBag.Id = id;

            return View();
        }

        public IActionResult KategorijaIzmenjena(int id, string Naziv)
        {

            if (!_categoryRepository.Existing(id))
            {
                // Ispis da je kategorija obrisana
                return RedirectToAction("Kategorije");
            }

            Kategorija kategorija = _categoryRepository.Get(id);
            _categoryRepository.UpdateProperty(kategorija, Naziv);

            return RedirectToAction("Kategorije");
        }

        [HttpDelete]
        public async Task<IActionResult> ObrisiKategoriju(int id)
        {
            if (!_categoryRepository.Existing(id))
            {
                return Json(new { success = false, message = "Kategorija neuspesno obrisana!" });
            }

            _categoryRepository.DeleteCategory(id);
            return Json(new { success = true, message = "Kategorija uspesno obirsana" });
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
                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Sva polja moraju biti pravilno popunjena!"));
                return RedirectToAction("DodajKategoriju");
            }

            if (_categoryRepository.Existing(kategorija.Naziv))
            {
                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Kategorija sa ovim nazivom vec postoji!"));
                return RedirectToAction("DodajKategoriju");
            }

            _categoryRepository.Add(kategorija);
            return RedirectToAction("Kategorije");
        }

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
            }
            else
            {
                // Ispis da je zahtev vec obradio neko drugi
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
            }
            else
            {

                // Ispis da je zahtev vec obradio neko drugi
            }

            return RedirectToAction("Verifikuj");
        }


        [HttpDelete]
        public async Task<IActionResult> ObrisiProizvod(int id)
        {
            if (!_productRepository.Existing(id))
            {
                return Json(new { success = false, message = "Proizvod neuspesno obrisan!" });
            }

            _productRepository.DeleteProduct(id);
            return Json(new { success = true, message = "Proizvod uspesno obirsan" });
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

            if (!_productRepository.Existing(id))
            {
                // Ispis da proizvod vise ne postoji
                return RedirectToAction("Proizvodi");
            }

            ViewBag.kategorije = _categoryRepository.GetAll();
            Proizvod proizvod = _productRepository.Get(id);
            ViewBag.Kategorija = _categoryRepository.Get(proizvod.KategorijaId);
            ViewBag.Message = "";
            ViewBag.Proizvod = proizvod;
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

            if (!_productRepository.Existing(id))
            {
                // Ispis da proizvod vise ne postoji
                return RedirectToAction("Proizvodi");
            }

            bool flag = false;

            if(noviProizvod.Naziv != null || noviProizvod.Sastojci != null || noviProizvod.Cena > 0 || noviProizvod.KategorijaId > 0 || ifile != null)
            {
                Proizvod proizvod = _productRepository.Get(id);

                if(proizvod == null)
                {
                    // Ovaj proizvod ne postoji
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

                if(noviProizvod.KategorijaId != 0 && noviProizvod.KategorijaId != proizvod.KategorijaId)
                {
                    proizvod.KategorijaId = noviProizvod.KategorijaId;
                    flag = true;

                    _productRepository.UpdateProperty(proizvod, "KategorijaId", proizvod.KategorijaId);
                }

                if(ifile != null)
                {
                    string imgext = Path.GetExtension(ifile.FileName).ToLower();
                    if (imgext != ".jpg" && imgext != ".png")
                    {
                        ViewBag.Korisnik = administrator;
                        ViewBag.kategorije = _categoryRepository.GetAll();
                        ViewBag.Proizvod = proizvod;
                        ViewBag.Kategorija = _categoryRepository.Get(proizvod.KategorijaId);
                        ViewBag.Message = "Slika mora biti u .jpg ili .png formatu!";
                        return View("IzmeniProizvod");
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
                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Proizvod uspesno izmenjen!"));
            }
            else
            {
                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Proizvod nije izmenjen!"));
            }

            return RedirectToAction("Proizvodi");
        }

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
