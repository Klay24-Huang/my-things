using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Prometheus;//唐加

namespace WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //唐加prometheus
            AspNetMetricServer.RegisterRoutes(GlobalConfiguration.Configuration);
        }
    }
}
