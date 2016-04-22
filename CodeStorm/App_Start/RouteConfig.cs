// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='RouteConfig.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Code
{
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Route Configuration
    /// </summary>
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Tribe", "Tribe/{tribename}", new { controller = "Tribe", action = "Index" });

            routes.MapRoute("DevOp", "DevOp/{username}", new { controller = "Profile", action = "Index", username = UrlParameter.Optional });
            routes.MapRoute("DevOpAchievements", "DevOp/{username}/Achievements", new { controller = "Profile", action = "Achievements" });

            routes.MapRoute("Profile", "Profile/Edit", new { controller = "Profile", action = "Edit" });


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}