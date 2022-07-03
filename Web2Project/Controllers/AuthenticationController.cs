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

         */

        public AuthenticationController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string korisnickoIme, string lozinka)
        {
            if (korisnickoIme == "" || korisnickoIme == null || lozinka == "" || lozinka == null)
            {
                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Sva polja moraju biti popunjena"));
                return View("Index", "Home");
            }

            if (!_userRepository.Existing(korisnickoIme))
            {
                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Ovo korisnicko ime ne postoji"));
                return View("Index", "Home");
            }

            lozinka = Operations.hashPassword(lozinka);
            Korisnik korisnik = _userRepository.Get(korisnickoIme);

            if(korisnik.Lozinka != lozinka)
            {
                HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Pogresna lozinka"));
                return View("Index", "Home");
            }

            /*
            HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(korisnik));
            Korisnik korisnik = JsonConvert.DeserializeObject<Korisnik>(HttpContext.Session.GetString("UlogovanKorisnik"));
            */

            korisnik.LogIn();
            HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(korisnik));

            HttpContext.Session.SetString("AlertMessage", JsonConvert.SerializeObject("Korisnik: " + korisnickoIme + " uspesno ulogovan!"));

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
            korisnik = new Korisnik();

            HttpContext.Session.SetString("UlogovanKorisnik", JsonConvert.SerializeObject(korisnik));

            return RedirectToAction("Index", "Home");
        }
    }
}
