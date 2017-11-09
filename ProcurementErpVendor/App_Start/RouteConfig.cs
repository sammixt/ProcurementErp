using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProcurementErpVendor
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            "404-PageNotFound",
            "{*url}",
            new { controller = "Error", action = "NotFound" }
            );

            routes.MapRoute(
            "500-Error",
            "{*url}",
            new { controller = "Error", action = "Index" }
                );
        }
    }
}
