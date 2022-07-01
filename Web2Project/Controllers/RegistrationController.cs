using Microsoft.AspNetCore.Mvc;
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
        public ActionResult Add(Korisnik korisnik)
        {
            bool isEmail = Regex.IsMatch(korisnik.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            if (korisnik.KorisnickoIme == null || korisnik.KorisnickoIme == "" || korisnik.Ime == null || korisnik.Ime == "" || korisnik.Prezime == null || korisnik.Prezime == "" || korisnik.Lozinka == null || korisnik.Lozinka == "" || korisnik.Email == null || korisnik.Email == "" || korisnik.DatumRodjenja > DateTime.Now || !isEmail)
            {
                // Sva polja moraju biti pravilno popunjena
                return RedirectToAction("Index");
            }

            // Da li vec postoji u bazi takvo username
            if (!_userRepository.Existing(korisnik.KorisnickoIme))
            {
                if(korisnik.TipKorisnika == Tip.POTROSAC)
                {
                    korisnik.Verifikovan = Zahtev.PRIHVACEN;
                }
                else
                {
                    korisnik.Verifikovan = Zahtev.PROCESIRA_SE;
                }

                korisnik.Lozinka = Operations.hashPassword(korisnik.Lozinka);
                _userRepository.Add(korisnik);
            }

            return RedirectToAction("Index","Home");
        }
    }
}
