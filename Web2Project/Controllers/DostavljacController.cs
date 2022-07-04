using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2Project.Models;

namespace Web2Project.Controllers
{
    public class DostavljacController : Controller
    {
        public IActionResult Index()
        {
            Korisnik dostavljac = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                dostavljac = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (dostavljac == null)
                {
                    dostavljac = new Korisnik();
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

            ViewBag.korisnik = dostavljac;
            return View();
        }
    }
}
