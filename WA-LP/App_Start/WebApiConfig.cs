using System.Web.Http;
using System.Web.Http.Cors;
using SharedSecurity.JWT.Handler;

namespace WA_LP
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            
            config.MapHttpAttributeRoutes();

            config.MessageHandlers.Add(new TokenValidationHandler());

            config.Routes.MapHttpRoute(
                name: "SecurityApi",
                routeTemplate: "v2/Security/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ServicesApi",
                routeTemplate: "v2/Services/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
