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
    public class AdministratorController : Controller
    {
        public IActionResult Index()
        {
            Korisnik administrator = new Korisnik();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UlogovanKorisnik")))
            {
                administrator = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

                if (administrator == null)
                {
                    administrator = new Korisnik();
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

            ViewBag.korisnik = administrator;
            return View();
        }
    }
}
