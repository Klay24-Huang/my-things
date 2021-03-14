using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //定義不要透過 Routing 處理的網址
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            /*這樣設定的話會永遠導去Home/Index，完全進不去Monitor
            //20210218唐加，Adam哥說不要可以直接連結Monitor
            routes.MapRoute(
                name: "Default_Map",
                url: "Monitor/{action}",
                defaults: new { controller = "Home", action = "Index" }
            );
            */

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            
        }
    }
}
