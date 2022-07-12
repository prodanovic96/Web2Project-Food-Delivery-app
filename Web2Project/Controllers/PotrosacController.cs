﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2Project.Models;

namespace Web2Project.Controllers
{
    public class PotrosacController : Controller
    {
        public IActionResult Index()
        {
            Korisnik posetilac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                posetilac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (posetilac == null)
                {
                    posetilac = new Korisnik();
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

            return View();
        }
    }
}
