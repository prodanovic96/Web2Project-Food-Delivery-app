using Microsoft.AspNetCore.Mvc;
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
            return View();
        }

        [HttpPost]
        public IActionResult Login(string korisnickoIme, string lozinka)
        {
            if (!_userRepository.Existing(korisnickoIme))
            {
                //  ovaj korisnik ne postoji
            }

            lozinka = Operations.hashPassword(lozinka);
            Korisnik korisnik = _userRepository.Get(korisnickoIme);

            if(korisnik.Lozinka != lozinka)
            {
                //  pogresna lozinka
            }

            //  uspesno ulogovan

            return View();
        }

        public IActionResult Logout()
        {
            return View();
        }
    }
}
