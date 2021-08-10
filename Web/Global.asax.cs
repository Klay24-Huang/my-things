using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Prometheus;//唐加prometheus
using System.Web.Http;//唐加，讓web專案導入webapi專案的套件

namespace Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //唐加，讓web專案導入webapi專案的套件
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //唐加prometheus，要在RouteConfig之前
            AspNetMetricServer.RegisterRoutes(GlobalConfiguration.Configuration);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
