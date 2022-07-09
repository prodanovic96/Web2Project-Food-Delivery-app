using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Web2Project.Helper;
using Web2Project.Models;

namespace Web2Project.Controllers
{
    public class RegistrationController : Controller
    {
        IUserRepository _userRepository;
        IFileUploadService _fileUploadService;

        public RegistrationController(IUserRepository userRepository, IFileUploadService fileUploadService)
        {
            _userRepository = userRepository;
            _fileUploadService = fileUploadService;

        }

        public IActionResult Index()
        {
            Korisnik korisnik = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (korisnik == null)
                {
                    korisnik = new Korisnik();
                }
                else
                {
                    if (korisnik.TipKorisnika == Tip.ADMINISTRATOR)
                    {
                        return RedirectToAction("Index", "Administrator");
                    }
                    else if (korisnik.TipKorisnika == Tip.DOSTAVLJAC)
                    {
                        return RedirectToAction("Index", "Dostavljac");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Potrosac");
                    }
                }
            }

            ViewBag.korisnik = korisnik;
            ViewBag.Message = "";

            return View();
        }
        
        public JsonResult CheckUserAvailability(string userdata)
        {
            if (_userRepository.Existing(userdata))
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        public JsonResult EmailValidation(string email)
        {
            bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            if (isEmail)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        [HttpPost]
        public ActionResult Add(Korisnik korisnik, string ponovoLozinka, IFormFile ifile)
        {
            //korisnik.Lozinka = Operations.hashPassword(korisnik.Lozinka);
            //korisnik.Verifikovan = Zahtev.PRIHVACEN;
            //korisnik.TipKorisnika = Tip.ADMINISTRATOR;
            //korisnik.Google = false;
            //korisnik.ImagePath = "~/Images/unknown.jpg";
            //_userRepository.Add(korisnik);
            //return RedirectToAction("Index");

            if (korisnik.KorisnickoIme == null || korisnik.KorisnickoIme == "" || korisnik.Ime == null || korisnik.Ime == "" || korisnik.Prezime == null || korisnik.Prezime == "" || korisnik.Lozinka == null || korisnik.Lozinka == "" || korisnik.Email == null || korisnik.Email == "" || korisnik.DatumRodjenja > DateTime.Now)
            {
                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Sva polja moraju biti popunjena"));
                return RedirectToAction("Index");
            }

            if (_userRepository.Existing(korisnik.KorisnickoIme))
            {
                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Korisnicko ime je zauzeto"));
                return RedirectToAction("Index");
            }

            if(korisnik.Lozinka != ponovoLozinka)
            {
                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Lozinke se ne poklapaju!"));
                return RedirectToAction("Index");
            }

            if(ifile != null)
            {
                string imgext = Path.GetExtension(ifile.FileName).ToLower();
                if (imgext != ".jpg" && imgext != ".png")
                {
                    ViewBag.Message = "Slika mora biti u .jpg ili .png formatu!";
                    ViewBag.korisnik = new Korisnik();
                    return View("Index");
                }
            }

            bool isEmail = Regex.IsMatch(korisnik.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            if (isEmail)
            {
                if (korisnik.TipKorisnika == Tip.POTROSAC)
                {
                    korisnik.Verifikovan = Zahtev.PRIHVACEN;
                }
                else
                {
                    korisnik.Verifikovan = Zahtev.PROCESIRA_SE;
                }

                korisnik.Lozinka = Operations.hashPassword(korisnik.Lozinka);
              
                _userRepository.Add(korisnik);

                string imagePath = "";

                if (ifile != null)
                {
                    _fileUploadService.UploadFile(ifile, korisnik.Id.ToString());
                    imagePath = "~/Images/" + korisnik.Id + ".jpg";
                }
                else
                {
                    imagePath = "~/Images/unknown.jpg";
                }

                _userRepository.UpdateKorisnik(korisnik, "ImagePath", imagePath);

                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Korisnik uspesno registrovan!"));
                return RedirectToAction("Index", "Authentication");
            }
            else
            {
                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Email nije validan"));
                return RedirectToAction("Index");
            }
        }
    }
}
