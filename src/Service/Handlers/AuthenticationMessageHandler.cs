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

namespace LMS.Service.Handlers
{
    public class AuthenticationMessageHandler : DelegatingHandler
    {
        private static readonly ILog Log = LogManager.GetLogger("Trace");

        const string HEADER_TENANT_IDENTIFIER = "lms.tenant.identifier";

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string tenantIdentifier = (request.Headers.Contains(HEADER_TENANT_IDENTIFIER)) ? request.Headers.GetValues(HEADER_TENANT_IDENTIFIER).First<string>() : null;
            bool requestAuthorized = (tenantIdentifier != null);

            if (requestAuthorized)
            {
                Log.Debug(String.Format("Verify tenant identifier '{0}'", tenantIdentifier));
                
                // Verify tenant identifier
                IRepository<Tenant> repository = RepositoryFactory<Tenant>.Create();
                Tenant tenant = repository.Get(int.Parse(tenantIdentifier));
                if (tenant == null || tenant.Id != int.Parse(tenantIdentifier))
                    requestAuthorized = false;
            }

            // TODO: Verify that the calling app is valid (name, version, api-key)

            if (requestAuthorized)
            {
                Log.Debug(String.Format("Verify JWT token '{0}'", ((request.Headers.Authorization!= null)? request.Headers.Authorization.ToString() : "no_token_provided")));

                // Verify JWT based authentication token
                if (request.Headers.Authorization != null)
                {
                    // TODO: Implement JWT security.





                }
            }

            if (requestAuthorized)
            {
                Log.Info("HTTP AUTH OK");
                return await base.SendAsync(request, cancellationToken);
            }
            else
            {
                Log.Warn("HTTP AUTH ERROR");
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }            
        }
    }
}