using System;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace LMS.Service
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_BeginRequest()
        {
            if (Request.Headers.GetValues("Origin") != null)
                Response.AppendHeader("Access-Control-Allow-Origin", Request.Headers.GetValues("Origin")[0]);

            Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, Authorization, LMS-Tenant-Identifier, LMS-App-Name, LMS-App-Version");
            Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
            Response.AppendHeader("Access-Control-Allow-Credentials", "true");

            if (Request.HttpMethod == "OPTIONS")
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                Response.End();
            }
        }
    }
}
