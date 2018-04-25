using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCore.Demo.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
