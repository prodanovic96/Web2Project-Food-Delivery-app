using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web2Project.Helper;
using Web2Project.Models;

namespace Web2Project.Controllers
{
    public class AuthenticationController : Controller
    {
        IUserRepository _userRepository;

        /*
          
        https://medium.com/c-sharp-progarmming/tutorial-code-first-approach-in-asp-net-core-mvc-with-ef-5baf5af696e9
        https://www.learnentityframeworkcore.com/dbcontext/modifying-data
        https://www.learnentityframeworkcore.com/dbcontext/modifying-data
        https://www.entityframeworktutorial.net/efcore/configure-one-to-one-relationship-using-fluent-api-in-ef-core.aspx
        https://docs.oracle.com/cd/E17952_01/connector-net-en/connector-net-entityframework-core-example.html
        https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
        https://stackoverflow.com/questions/21573550/setting-unique-constraint-with-fluent-api
        https://www.learnentityframeworkcore.com/configuration/fluent-api/hasforeignkey-method

        Bitno
        https://www.learnentityframeworkcore.com/dbcontext/modifying-data
        https://medium.com/c-sharp-progarmming/tutorial-code-first-approach-in-asp-net-core-mvc-with-ef-5baf5af696e9

         */

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
                        return RedirectToAction("Index", "Dostavljac");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Potrosac");
                    }
                } 
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
                return View("Index", "Home");
            }

            if (!_userRepository.Existing(korisnickoIme))
            {
                HttpContext.Session.SetString("AlertMessage", "Ovo korisnicko ime ne postoji");
                return View("Index", "Home");
            }

            lozinka = Operations.hashPassword(lozinka);
            Korisnik korisnik = _userRepository.Get(korisnickoIme);

            if(korisnik.Lozinka != lozinka)
            {
                ViewBag.Ime = korisnickoIme;
                ViewBag.korisnik = new Korisnik();
                ViewBag.Message = "Pogresna lozinka";

                HttpContext.Session.SetString("AlertMessage","Pogresna lozinka");
                return View("Index", "Home");
            }

            /*
            HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(korisnik));
            Korisnik korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));
            */

            korisnik.LogIn();
            HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(korisnik));

            HttpContext.Session.SetString("AlertMessage", ("Korisnik: " + korisnickoIme + " uspesno ulogovan!"));

            if(korisnik.TipKorisnika == Tip.ADMINISTRATOR)
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
            Korisnik korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));

            korisnik.LogOut();

            HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(null));

            return RedirectToAction("Index", "Authentication");
        }
    }
}
