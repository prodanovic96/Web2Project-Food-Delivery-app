using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Web2Project.Helper;
using Web2Project.Models;
using Web2Project.Services;

namespace Web2Project.Controllers
{
    public class MojNalogController : Controller
    {
        IUserRepository _userRepository;
        IFileUploadService _fileUploadService;

        public MojNalogController(IUserRepository userRepository, IFileUploadService fileUploadService)
        {
            _userRepository = userRepository;
            _fileUploadService = fileUploadService;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                return RedirectToAction("Index", "Authentication");
            }
            else
            {
                Korisnik korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (korisnik == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
            }

            string message = HttpContext.Session.GetString("AlertMessage");
            if (message != "" && message != null)
            {
                ViewBag.AlertMessage = message;
                ViewBag.Uspesno = JsonConvert.DeserializeObject<bool>(HttpContext.Session.GetString("Uspesno"));

                HttpContext.Session.SetString("AlertMessage", "");
            }

            ViewBag.korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));
            return View();
        }

        public ActionResult LicniPodaci()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                return RedirectToAction("Index", "Authentication");
            }
            else
            {
                Korisnik korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (korisnik == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
            }

            string message = HttpContext.Session.GetString("AlertMessage");
            if (message != "" && message != null)
            {
                ViewBag.AlertMessage = message;
                ViewBag.Uspesno = JsonConvert.DeserializeObject<bool>(HttpContext.Session.GetString("Uspesno"));

                HttpContext.Session.SetString("AlertMessage", "");
            }

            ViewBag.korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));
            ViewBag.Message = "";

            return View();
        }

        [HttpPost]
        public ActionResult LicniPodaciPromenjeni(string korisnickoime, string ime, string prezime, string adresa, IFormFile ifile, DateTime DatumRodjenja)
        {
            Korisnik korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

            bool flag = false;

            if (korisnickoime != "" || ime != "" || prezime != "" || adresa != "" || ifile !=null || !DatumRodjenja.ToString().Contains("01-Jan-01"))
            {
                if (ime != null && ime != "" && ime != korisnik.Ime)
                {
                    korisnik.Ime = ime;
                    flag = true;

                    _userRepository.UpdateKorisnik(korisnik, "Ime", ime);
                }
                if (prezime != null && prezime != "" && prezime != korisnik.Prezime)
                {
                    korisnik.Prezime = prezime;
                    flag = true;

                    _userRepository.UpdateKorisnik(korisnik, "Prezime", prezime);
                }
                if (adresa != null && adresa != "" && korisnik.Adresa != adresa)
                {
                    korisnik.Adresa = adresa;
                    flag = true;

                    _userRepository.UpdateKorisnik(korisnik, "Adresa", adresa);
                }
                if(ifile != null)
                {
                    string imgext = Path.GetExtension(ifile.FileName).ToLower();
                    if (imgext != ".jpg" && imgext != ".png")
                    {
                        HttpContext.Session.SetString("AlertMessage", "Slika mora biti u .jpg ili .png formatu!");
                        HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                        return RedirectToAction("LicniPodaci");
                    }

                    flag = true;
                    _fileUploadService.UploadFile(ifile, korisnik.Id.ToString(), "Korisnici");

                    if (korisnik.ImagePath.Contains("unknown.jpg"))
                    {
                        korisnik.ImagePath = "~/Korisnici/" + korisnik.Id + ".jpg";
                        _userRepository.UpdateKorisnik(korisnik, "ImagePath", korisnik.ImagePath);
                    }
                }
                if (!DatumRodjenja.ToString().Contains("01-Jan-01") && DatumRodjenja > DateTime.Now)
                {
                    flag = true;
                    korisnik.DatumRodjenja = DatumRodjenja;

                    _userRepository.UpdateKorisnik(korisnik, "DatumRodjenja", korisnik.DatumRodjenja.ToString());
                }

                if (korisnickoime != null && korisnickoime != "" && korisnickoime != korisnik.KorisnickoIme)
                {
                    if (_userRepository.Existing(korisnickoime))
                    {
                        HttpContext.Session.SetString("AlertMessage", "Korisnicko ime je zauzeto");
                        HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                        return RedirectToAction("LicniPodaci");
                    }
                    else
                    {
                        korisnik.KorisnickoIme = korisnickoime;
                        flag = true;

                        _userRepository.UpdateKorisnik(korisnik, "KorisnickoIme", korisnickoime);
                    }
                }
            }

            if (flag)
            {
                HttpContext.Session.SetString("AlertMessage", "Licni podaci uspesno izmenjeni!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));

                HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(korisnik));
            }
            else
            {
                HttpContext.Session.SetString("AlertMessage", "Licni podaci nisu izmenjeni!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));
            }

            return RedirectToAction("Index");
        }

        public ActionResult Email()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                return RedirectToAction("Index", "Authentication");
            }
            else
            {
                Korisnik korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (korisnik == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
            }

            string message = HttpContext.Session.GetString("AlertMessage");
            if (message != "" && message != null)
            {
                ViewBag.AlertMessage = message;
                ViewBag.Uspesno = JsonConvert.DeserializeObject<bool>(HttpContext.Session.GetString("Uspesno"));

                HttpContext.Session.SetString("AlertMessage", "");
            }

            ViewBag.korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

            return View();
        }

        [HttpPost]
        public ActionResult EmailPromenjen(string email, string lozinka)
        {
            Korisnik korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

            if (email != null && email != "" && email != korisnik.Email)
            {
                bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

                lozinka = Operations.hashPassword(lozinka);

                if (lozinka == korisnik.Lozinka && isEmail)
                {
                    korisnik.Email = email;
                    _userRepository.UpdateKorisnik(korisnik, "Email", email);

                    HttpContext.Session.SetString("AlertMessage", "Email uspesno izmenjen!");
                    HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));

                    HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(korisnik));
                }
                else
                {
                    if (!isEmail)
                    {
                        HttpContext.Session.SetString("AlertMessage", "Email nije validan!");
                        HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));
                    }

                    if (lozinka != korisnik.Lozinka)
                    {
                        HttpContext.Session.SetString("AlertMessage", "Neispravna lozinka!");
                        HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));
                    }

                    ViewBag.korisnik = korisnik;
                    return RedirectToAction("Email");
                }
            }
            ViewBag.korisnik = korisnik;
            return RedirectToAction("Index");
        }

        public ActionResult Lozinka()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                return RedirectToAction("Index", "Authentication");
            }
            else
            {
                Korisnik korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (korisnik == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
            }

            string message = HttpContext.Session.GetString("AlertMessage");
            if (message != "" && message != null)
            {
                ViewBag.AlertMessage = message;
                ViewBag.Uspesno = JsonConvert.DeserializeObject<bool>(HttpContext.Session.GetString("Uspesno"));

                HttpContext.Session.SetString("AlertMessage", "");
            }

            ViewBag.korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik")); 

            return View();
        }

        [HttpPost]
        public ActionResult LozinkaPromenjena(string lozinka, string novalozinka, string novalozinkaponovo)
        {
            Korisnik korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

            if (lozinka != "" && novalozinka != "" && novalozinkaponovo != "")
            {
                lozinka = Operations.hashPassword(lozinka);

                if (korisnik.Lozinka == lozinka && novalozinka == novalozinkaponovo)
                {
                    novalozinka = Operations.hashPassword(novalozinka);
                    korisnik.Lozinka = novalozinka;
                    _userRepository.UpdateKorisnik(korisnik, "Lozinka", novalozinka);

                    HttpContext.Session.SetString("AlertMessage", "Lozinka uspesno izmenjena!");
                    HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));

                    HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(korisnik));
                }
                else
                {
                    if (korisnik.Lozinka != lozinka)
                    {
                        ViewBag.Message1 = "Neispravna lozinka";
                    }

                    if (novalozinka != novalozinkaponovo)
                    {
                        ViewBag.Message2 = "Ponovljena lozinka se ne poklapa";
                    }
                    ViewBag.korisnik = korisnik;
                    return View("Lozinka");
                }
            }

            ViewBag.korisnik = korisnik;
            return RedirectToAction("Index");
        }
    }
}
