﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Web2Project.Helper;
using Web2Project.Models;

namespace Web2Project.Controllers
{
    public class MojNalogController : Controller
    {
        IUserRepository _userRepository;

        public MojNalogController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            ViewBag.korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));
            return View();
        }

        public ActionResult LicniPodaci()
        {
            Korisnik korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));
            
            ViewBag.korisnik = korisnik;

            return View();
        }

        [HttpPost]
        public ActionResult LicniPodaciPromenjeni(string korisnickoime, string ime, string prezime, string adresa)
        {
            Korisnik korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

            bool flag = false;

            if (korisnickoime != "" || ime != "" || prezime != "" || adresa != "")
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

                if (korisnickoime != null && korisnickoime != "" && korisnickoime != korisnik.KorisnickoIme)
                {
                    if (_userRepository.Existing(korisnickoime))
                    {
                        HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Korisnicko ime je zauzeto"));
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
                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Licni podaci uspesno izmenjeni!"));
                HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(korisnik));
            }
            else
            {
                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Licni podaci nisu izmenjeni!"));
            }

            return RedirectToAction("Index");
        }

        public ActionResult Email()
        {
            Korisnik korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));
            
            ViewBag.korisnik = korisnik;

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

                    HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Email uspesno izmenjen!"));
                }
                else
                {
                    if (!isEmail)
                    {
                        HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Email nije validan!"));
                    }

                    if (lozinka != korisnik.Lozinka)
                    {
                        HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Neispravna lozinka!"));
                    }

                    HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(korisnik));
                    return View("Email");
                }
            }
            HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(korisnik));
            return RedirectToAction("Index");
        }

        public ActionResult Lozinka()
        {
            Korisnik korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

            ViewBag.korisnik = korisnik;

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

                    HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Lozinka uspesno izmenjena!"));
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