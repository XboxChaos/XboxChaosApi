using System.Web.Http;
using XboxChaosApi.Helpers;

namespace XboxChaosApi
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services

			// Web API routes
			//config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "{version}/{controller}/{id}",
				defaults: new { version = "1", id = RouteParameter.Optional }
			);

			//config.Formatters.Add(new BrowserJsonFormatter());
		}
	}
}
