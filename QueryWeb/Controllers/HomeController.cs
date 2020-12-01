using Domain.TB.SubScript;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebModel;

namespace QueryWeb.Controllers
{
    public class HomeController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [AllowAnonymous]
        public ActionResult Index(string ReturnUrl)
        {
            if ("" == ReturnUrl)
            {
                ReturnUrl = "Index";
            }
            return base.View(new iRentNUser()
            {
                ReturnUrl = ReturnUrl
            });
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Index(iRentNUser uAccount)
        {
            if (!base.ModelState.IsValid)
            {
                return base.View(uAccount);
            }
            string account = uAccount.Account;
            DateTime now = DateTime.Now;
            DateTime dateTime = DateTime.Now;
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, account, now, dateTime.AddMinutes(10), false, "NUser", FormsAuthentication.FormsCookiePath);
            string encTicket = FormsAuthentication.Encrypt(ticket);
            base.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
            return base.RedirectToAction("QueryList");
        }

        public ActionResult QueryList()
        {
            List<SubScriptionForClient> lstQuery = null;
            SubScriptionRepository subScriptionRepository = new SubScriptionRepository(this.connetStr);
            if (base.User.Identity.IsAuthenticated)
            {
                FormsIdentity id = (FormsIdentity)base.User.Identity;
                if (id != null)
                {
                    FormsAuthenticationTicket ticket = id.Ticket;
                    if (!string.IsNullOrEmpty(ticket.Name))
                    {
                        lstQuery = subScriptionRepository.GetSubScriptionForClients(ticket.Name);
                    }
                }
                else
                {
                    base.Response.Redirect("Index");
                }
            }
            else
            {
                base.Response.Redirect("Index");
            }
            return base.View(lstQuery);
        }
    }
}