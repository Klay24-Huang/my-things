﻿using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class Foo : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
