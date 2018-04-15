using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MusicMan
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "Modules",
                url: "templates/{action}/{id}",
                defaults: new { controller = "Templates", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SiteMap",
                url: "page/{*path}",
                defaults: new { controller = "Page", action = "Index", path = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "NewsList",
                url: "blog/{action}/{id}",
                defaults: new { controller = "Blog", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "News",
                url: "blog/{year}/{manth}/{day}/{alias}",
                defaults: new { controller = "Blog", action = "Item" }
            );

            routes.MapRoute(
                name: "Music",
                url: "music",
                defaults: new { controller = "Works", action = "index", type = "music" }
            );

            routes.MapRoute(
                name: "Video",
                url: "video",
                defaults: new { controller = "Works", action = "index", type = "video" }
            );

            routes.MapRoute(
                name: "Like",
                url: "like/{id}",
                defaults: new { controller = "Works", action = "Like", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "DelWorks",
                url: "del/{id}",
                defaults: new { controller = "Works", action = "Delete", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Events",
                url: "events/{action}/{id}",
                defaults: new { controller = "Page", action = "Events", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Contacts",
                 url: "contacts/{action}/{id}",
                defaults: new { controller = "Page", action = "Contacts", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "partners",
                 url: "partners",
                defaults: new { controller = "Page", action = "Partners" }
            );

            routes.MapRoute(
                name: "mission",
                 url: "mission",
                defaults: new { controller = "Page", action = "Mission" }
            );

            routes.MapRoute(
                name: "Producing",
                 url: "producing",
                defaults: new { controller = "Page", action = "Producing" }
            );

            routes.MapRoute(
                name: "regulations",
                 url: "regulations",
                defaults: new { controller = "Page", action = "Regulations" }
            );

            routes.MapRoute(
                name: "Advertising",
                 url: "advertising/{action}/{id}",
                defaults: new { controller = "Page", action = "Advertising", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Helping",
                 url: "helping/{action}/{id}",
                defaults: new { controller = "Page", action = "Helping", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "PeopleList",
                url: "peoples/{group}",
                defaults: new { controller = "Peoples", action = "index", group = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Account",
                url: "Account/{action}/{code}",
                defaults: new { controller = "Account", action = "login", code = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "PeoplePage",
                url: "{UserId}/{action}",
                defaults: new { controller = "Peoples", action = "item", id = UrlParameter.Optional }
            );
        }
    }
}
