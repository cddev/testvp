using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Unity;
using testawsauth.Models;
using testawsauth.Resolver;
using Unity.Lifetime;
using System.Security.Cryptography;

namespace testawsauth
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var container = new UnityContainer();
            container.RegisterType<IUserRepository, UserRepository>(new ContainerControlledLifetimeManager());
          
            config.DependencyResolver = new UnityResolver(container);            

            // Web API routes
            config.MapHttpAttributeRoutes();

            //API will return JSON
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

    }
}
