using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Workforce_Purple_Parrots.Controllers
{
    public class ComputersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}