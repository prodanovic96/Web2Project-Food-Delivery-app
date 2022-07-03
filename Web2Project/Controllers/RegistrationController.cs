using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using Web2Project.Helper;
using Web2Project.Models;

namespace Web2Project.Controllers
{
    public class RegistrationController : Controller
    {
        IUserRepository _userRepository;

        public RegistrationController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
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
        public ActionResult Add(Korisnik korisnik, string ponovoLozinka)
        {
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

                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Korisnik uspesno registrovan!"));
                return RedirectToAction("Index");
            }
            else
            {
                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Email nije validan"));
                return RedirectToAction("Index");
            }
        }
    }
}
