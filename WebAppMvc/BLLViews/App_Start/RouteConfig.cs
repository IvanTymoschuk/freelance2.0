using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BLLViews
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
            name: "Ticket",
            url: "{controller}/{action}/{id}/{ticket}",
            defaults: new { controller = "Home", action = "Ticket", id = UrlParameter.Optional, ticket = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "SubscribeManager",
               url: "Home/SubscribeManager/{id}/{job_id}",
               defaults: new { controller = "Home", action = "SubscribeManager", id = UrlParameter.Optional, job_id = UrlParameter.Optional }
           );
            routes.MapRoute(
             name: "UpdateResume",
             url: "Home/UpdateResume/{id}/{isTrue}",
             defaults: new { controller = "Home", action = "UpdateResume", id = UrlParameter.Optional, isTrue = UrlParameter.Optional }
         );
        }
    }
}
