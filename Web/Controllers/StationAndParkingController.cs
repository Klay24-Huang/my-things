﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class StationAndParkingController : Controller
    {
      [HttpPost]
      public ActionResult StationInfoSetting()
        {
            return View();
        }
    }
}