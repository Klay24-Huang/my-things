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

        public void nnlog(object user, object account, string ip, string action)
        {
            logger.Trace(
                    "{ReportName:'程式使用紀錄'," +
                    "User" + ":'" + user + "'," +
                    "Account" + ":'" + account + "'," +
                    "ActionName" + ":'" + action + "'," +
                    "IPAddr" + ":'" + ip + "}"
                    );
        }

    }
}