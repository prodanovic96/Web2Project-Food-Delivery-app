using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Web2Project.Helper;
using Web2Project.Models;
using Web2Project.Services;

namespace Web2Project.Controllers
{
    public class RegistrationController : Controller
    {
        IUserRepository _userRepository;
        IFileUploadService _fileUploadService;
        IEmailSender _emailSender;

        public RegistrationController(IUserRepository userRepository, IFileUploadService fileUploadService, IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _fileUploadService = fileUploadService;
            _emailSender = emailSender;

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

            string message = HttpContext.Session.GetString("AlertMessage");
            if (message != "" && message != null)
            {
                ViewBag.AlertMessage = message;
                ViewBag.Uspesno = JsonConvert.DeserializeObject<bool>(HttpContext.Session.GetString("Uspesno"));

                HttpContext.Session.SetString("AlertMessage", "");
            }

            ViewBag.korisnik = korisnik;
            ViewBag.Message = "";

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
        public ActionResult Add(Korisnik korisnik, string ponovoLozinka, IFormFile ifile)
        {
            if (korisnik.KorisnickoIme == null || korisnik.KorisnickoIme == "" || korisnik.Ime == null || korisnik.Ime == "" || korisnik.Prezime == null || korisnik.Prezime == "" || korisnik.Lozinka == null || korisnik.Lozinka == "" || korisnik.Email == null || korisnik.Email == "" || korisnik.DatumRodjenja > DateTime.Now)
            {
                HttpContext.Session.SetString("AlertMessage", "Sva polja moraju biti popunjena");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                return RedirectToAction("Index");
            }

            if (_userRepository.Existing(korisnik.KorisnickoIme))
            {
                HttpContext.Session.SetString("AlertMessage", "Korisnicko ime je zauzeto");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                return RedirectToAction("Index");
            }

            if(korisnik.Lozinka != ponovoLozinka)
            {
                HttpContext.Session.SetString("AlertMessage", "Lozinke se ne poklapaju!");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                return RedirectToAction("Index");
            }

            if(ifile != null)
            {
                string imgext = Path.GetExtension(ifile.FileName).ToLower();
                if (imgext != ".jpg" && imgext != ".png")
                {
                    HttpContext.Session.SetString("AlertMessage", "Slika mora biti u .jpg ili .png formatu!");
                    HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                    return RedirectToAction("Index");
                }
            }

            bool isEmail = Regex.IsMatch(korisnik.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            if (isEmail)
            {
                if (korisnik.TipKorisnika == Tip.POTROSAC)
                {
                    korisnik.Verifikovan = Zahtev.PRIHVACEN;

                    Message message = new Message(new string[] { "markoprodanovic96@gmail.com" }, "[Web 2] - Projekat", "Uspesno ste se registrovali na sistem, mozete koristiti usluge naseg sistema!");
                    _emailSender.SendEmail(message);

                    HttpContext.Session.SetString("AlertMessage", "Korisnik uspesno registrovan!");
                    HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));
                }
                else
                {
                    korisnik.Verifikovan = Zahtev.PROCESIRA_SE;

                    Message message = new Message(new string[] { "markoprodanovic96@gmail.com" }, "[Web 2] - Projekat", "Vas nalog se procesuira, administrator ce obraditi vas zahtev u najkracem mogucem roku!");
                    _emailSender.SendEmail(message);

                    HttpContext.Session.SetString("AlertMessage", "Korisnik uspesno registrovan, ceka se verifikacija od strane administratora!");
                    HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(true));
                }

                korisnik.Lozinka = Operations.hashPassword(korisnik.Lozinka);
              
                _userRepository.Add(korisnik);

                string imagePath = "";

                if (ifile != null)
                {
                    _fileUploadService.UploadFile(ifile, korisnik.Id.ToString(), "Korisnici");
                    imagePath = "~/Korisnici/" + korisnik.Id + ".jpg";
                }
                else
                {
                    imagePath = "~/Korisnici/unknown.jpg";
                }

                _userRepository.UpdateKorisnik(korisnik, "ImagePath", imagePath);

                
                return RedirectToAction("Index", "Authentication");
            }
            else
            {
                HttpContext.Session.SetString("AlertMessage", "Email nije validan");
                HttpContext.Session.SetString("Uspesno", JsonConvert.SerializeObject(false));

                return RedirectToAction("Index");
            }
        }
    }
}
