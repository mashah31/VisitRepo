using System.Web.Http;

namespace VisitsRepo
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            
            config.Routes.MapHttpRoute(
                name: "StatesAPI",
                routeTemplate: "api/{version}/{controller}/{state}",
                defaults: new { state = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "UsersAPI",
                routeTemplate: "api/{version}/{controller}/{username}",
                defaults: new { username = RouteParameter.Optional },
                constraints: new { username = @"^[a-z]+$" }
            );
        }
    }
}
