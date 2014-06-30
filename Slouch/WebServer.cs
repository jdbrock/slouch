using Microsoft.AspNet.SignalR;
using Nancy.TinyIoc;
using Nancy.ViewEngines.Razor;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Slouch
{
    public class WebServer
    {
        // This code configures Web API. The WebServer class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Set up Web API.
            appBuilder.UseWebApi(config);

            // Set up SignalR.
            appBuilder.MapHubs("signalr", new HubConfiguration
            {
                EnableCrossDomain = true,
                EnableJavaScriptProxies = true,
                Resolver = new DefaultDependencyResolver()
            });

            // Set up Nancy.
            appBuilder.UseNancy();
        }
    }
}
