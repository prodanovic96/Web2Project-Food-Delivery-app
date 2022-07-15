﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Web2Project.Helper;
using Web2Project.Models;

namespace Web2Project.Controllers
{
    public class AuthenticationController : Controller
    {
        IUserRepository _userRepository;

        public AuthenticationController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
                        korisnik.Verifikovan = _userRepository.VratiZahtev(korisnik.Id);
                        HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(korisnik));

                        return RedirectToAction("Index", "Dostavljac");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Potrosac");
                    }
                } 
            }

            string message = HttpContext.Session.GetString("AlertMessage");
            if (message != "" && message != null)
            {
                ViewBag.AlertMessage = message;
                ViewBag.Uspesno = JsonConvert.DeserializeObject<bool>(HttpContext.Session.GetString("Uspesno"));

                HttpContext.Session.SetString("AlertMessage", "");
            }

            

            ViewBag.Ime = "";
            ViewBag.Message = "";

            ViewBag.korisnik = korisnik;
            return View();
        }

        [HttpPost]
        public IActionResult Login(string korisnickoIme, string lozinka)
        {
            if (korisnickoIme == "" || korisnickoIme == null || lozinka == "" || lozinka == null)
            {
                HttpContext.Session.SetString("AlertMessage", "Sva polja moraju biti popunjena");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                return RedirectToAction("Index");
            }

            if (!_userRepository.Existing(korisnickoIme))
            {
                HttpContext.Session.SetString("AlertMessage", "Ovo korisnicko ime ne postoji");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                return RedirectToAction("Index");
            }

            lozinka = Operations.hashPassword(lozinka);
            Korisnik korisnik = _userRepository.Get(korisnickoIme);

            if(korisnik.Lozinka != lozinka)
            {
                ViewBag.Ime = korisnickoIme;
                ViewBag.korisnik = new Korisnik();
                ViewBag.Message = "Pogresna lozinka";

                return View("Index");
            }

            korisnik.LogIn();
            HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(korisnik));
            

            HttpContext.Session.SetString("AlertMessage", ("Korisnik " + korisnickoIme + " uspesno ulogovan!"));
            HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));

            if (korisnik.TipKorisnika == Tip.ADMINISTRATOR)
            {
                return RedirectToAction("Index", "Administrator");
            }
            else if (korisnik.TipKorisnika == Tip.DOSTAVLJAC)
            {
                return RedirectToAction("Index", "Dostavljac");
            }
            return RedirectToAction("Index", "Potrosac");            
        }

        public IActionResult Logout()
        {
            Korisnik korisnik = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (korisnik == null)
                {
                    return RedirectToAction("Index", "Authentication");
                }
            }
            else
            {
                return RedirectToAction("Index", "Authentication");
            }

            korisnik.LogOut();
            HttpContext.SignOutAsync();

            HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(null));

            HttpContext.Session.SetString("AlertMessage", ("Korisnik " + korisnik.KorisnickoIme + " uspesno odjavljen!"));
            HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));
            return RedirectToAction("Index");
        }

        public async Task LogInGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }

        public IActionResult DopuniPodatke()
        {
            ViewBag.korisnik = new Korisnik();
            return View();
        }

        [HttpPost]
        public IActionResult PodaciDopunjeni(string Adresa, DateTime DatumRodjenja)
        {
            if(Adresa == "")
            {
                HttpContext.Session.SetString("AlertMessage", "Sva polja moraju biti popunjena!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                return RedirectToAction("Index");
            }

            Korisnik korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("PrivremeniKorisnik"));

            korisnik.Adresa = Adresa;
            korisnik.DatumRodjenja = DatumRodjenja;
            korisnik.LogedIn = true;

            _userRepository.Add(korisnik);

            HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(korisnik));

            HttpContext.Session.SetString("AlertMessage", "Korisnik uspesno registrovan!");
            HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));

            return RedirectToAction("Index", "Potrosac");
        }


        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities
                .FirstOrDefault().Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });

            string korisnickoIme = claims.FirstOrDefault().Value;

            Korisnik korisnikSaEmail = _userRepository.GetKorisnikWithSameEmail(claims.Where(e => e.Type.Contains("emailaddress")).FirstOrDefault().Value);

            if(korisnikSaEmail != null)
            {
                korisnikSaEmail.LogedIn = true;

                HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(korisnikSaEmail));

                return RedirectToAction("Index", "Potrosac");
            }

            Korisnik korisnik = _userRepository.Get(korisnickoIme);

            if (korisnik == null)
            {
                korisnik = new Korisnik();
                korisnik.KorisnickoIme = korisnickoIme;
                korisnik.Ime = claims.Where(e => e.Type.Contains("givenname")).FirstOrDefault().Value;
                korisnik.Prezime = claims.Where(e => e.Type.Contains("surname")).FirstOrDefault().Value;
                korisnik.Email = claims.Where(e => e.Type.Contains("emailaddress")).FirstOrDefault().Value;
                korisnik.TipKorisnika = Tip.POTROSAC;
                korisnik.Verifikovan = Zahtev.PRIHVACEN;
                korisnik.ImagePath = claims.Where(e => e.Type.Contains("picture")).FirstOrDefault().Value;
                korisnik.Google = true;

                HttpContext.Session.SetString("PrivremeniKorisnik", JsonConvert.SerializeObject(korisnik));

                return RedirectToAction("DopuniPodatke");
            }

            korisnik.LogedIn = true;
            HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(korisnik));

            return RedirectToAction("Index", "Potrosac");
        }
    }
}
