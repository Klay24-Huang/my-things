using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;
using System.Configuration;

namespace Web.Controllers
{
    public class BaseSafeController : Controller
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        protected string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        protected string connetStrMirror = ConfigurationManager.ConnectionStrings["IRentMirror"].ConnectionString;

        public void nnlog(object user, string ip)
        {
            logger.Trace(
                    "{ReportName:'TEST'," +
                    "User" + ":'" + user + "'," +
                    "IPAddr" + ":'" + ip + "}"
                    );
        }

    }
}