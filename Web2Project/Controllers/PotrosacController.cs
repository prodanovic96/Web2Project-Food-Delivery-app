﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web2Project.Controllers
{
    public class PotrosacController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
