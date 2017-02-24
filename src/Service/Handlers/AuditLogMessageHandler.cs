using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Security.Cryptography;
using System.Text;

using log4net;

using LMS.Data;
using LMS.Data.Cloud;
using LMS.Service.Logging;
using LMS.Model.Resource.Logging;
using LMS.Model.Resource;
using LMS.Model.Composite;
using LMS.Service.Security.JWT;

namespace LMS.Service.Handlers
{
    public class AuditLogMessageHandler : DelegatingHandler
    {
        private static readonly ILog Log = LogManager.GetLogger("Trace");

        const string HEADER_APP_NAME = "LMS-App-Name";
        const string HEADER_APP_VERSION = "LMS-App-Version";

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string applicationName = (request.Headers.Contains(HEADER_APP_NAME)) ? request.Headers.GetValues(HEADER_APP_NAME).First<string>() : null;
            string applicationVersion = (request.Headers.Contains(HEADER_APP_VERSION)) ? request.Headers.GetValues(HEADER_APP_VERSION).First<string>() : null;

            EventAction eventAction = EventAction.Execute;
            switch(HttpContext.Current.Request.HttpMethod)
            {
                case "GET": { eventAction = EventAction.Read; break; }
                case "POST": { eventAction = EventAction.Create; break; }
                case "PUT": { eventAction = EventAction.Update; break; }
                case "DELETE": { eventAction = EventAction.Delete; break; }
            }

            string[] url = HttpContext.Current.Request.RawUrl.Split('/');
            string resourceType = null;
            string resourceRef = null;

            for(int i =0;i<url.Length;++i)
            {
                // Next part of the url defines the resource type.
                if (url[i] == "api" && i + 1 < url.Length)
                {
                    resourceType = url[i + 1];
                    resourceType = char.ToUpper(resourceType[0]) + resourceType.Substring(1);
                    break;
                }
            }

            // Build resource reference
            resourceRef = (Reference.UriPrefix + "/" + resourceType ?? "").ToLower();

            // If last part of url is an integer, append it as the id
            int resourceId = 0;
            if (int.TryParse(url[url.Length - 1], out resourceId))
                resourceRef += "/" + resourceId;

            JsonWebToken token = null;
            if (request.Headers.Authorization != null && request.Headers.Authorization.Scheme == "Bearer")
                token = JsonWebTokenSerializer.Decode(request.Headers.Authorization.Parameter);

            AuditLog.Log(new AuditEvent()
            {
                Source = new Source()
                {
                    Name = applicationName ?? "app_name_undefined",
                    Version = applicationVersion ?? "app_version_undefined"
                },
                Target = new Target()
                {
                    Type = resourceType ?? "resource_undefined",
                    Resource = new Reference(resourceRef)
                },
                User = new User()
                {
                    Username = (token != null) ? token.Data.Username : "username_undefined"
                },
                Event = new Event()
                {
                    Action = eventAction,
                    EffectiveTime = DateTime.Now
                }
            });

            Log.Debug("Event appended to audit log");
            return await base.SendAsync(request, cancellationToken);
        }
    }
}