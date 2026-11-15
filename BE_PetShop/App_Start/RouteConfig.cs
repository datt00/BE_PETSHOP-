using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BE_PetShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Route mặc định: mở "/" sẽ chạy StoreController.Index()
            routes.MapRoute(
                 name: "Default", // tên route
                 url: "{controller}/{action}/{id}", // mẫu URL /Controller/Action/Id
                 defaults: new { controller = "The Pet Viet Nam", action = "Index", id = UrlParameter.Optional } // Controller và Action mặc định
             );

        }
    }
}
