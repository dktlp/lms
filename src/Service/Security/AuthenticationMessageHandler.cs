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

namespace LMS.Service.Security
{
    // TODO: Javascript MD5 + BASE64:
    // http://www.myersdaily.org/joseph/javascript/md5-text.html
    // http://www.w3schools.com/jsref/met_win_atob.asp

    public class AuthenticationMessageHandler : DelegatingHandler
    {
        const string HEADERNAME_MERCHANT_NAME = "ecommerce.merchant.name";
        const string HEADERNAME_MERCHANT_IDENTIFIER = "ecommerce.merchant.identifier";
        const string HEADERNAME_MERCHANT_KEY = "ecommerce.merchant.key";

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // TODO: Implement authentication check.


            //string headerApiToken = GetHttpHeaderValue(HEADERNAME_API_TOKEN, request);
            //string headerTenantId = GetHttpHeaderValue(HEADERNAME_TENANT_ID, request);
            //string headerSessionToken = GetHttpHeaderValue(HEADERNAME_SESSION_TOKEN, request);
            //string headerSessionHash = GetHttpHeaderValue(HEADERNAME_SESSION_HASH, request);

            //// Check if all headers are delivered in the request.
            //bool proceed = (headerApiToken != null && headerTenantId != null && headerSessionToken != null && headerSessionHash != null);

            //// Validate the SessionHash to check if the HTTP-header values have been tampered with.
            //if (proceed)
            //{
            //    MD5 md5 = MD5.Create();
            //    byte[] hashb = md5.ComputeHash(Encoding.UTF8.GetBytes(headerApiToken + "|" + headerTenantId + "|" + headerSessionToken));
            //    string hash = Convert.ToBase64String(hashb);
            //    if (hash != headerSessionHash)
            //        proceed = false;
            //}

            //// TODO: validate all headers sent from client (api.token, tenant.id, session.token).

            //// Validate the ApiToken
            //// Validate the TenantId
            //// Validate the SessionToken

            //if (proceed)
            //{
            //    Log.Instance.Info("HTTP AUTH OK");
            //    HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            //    return response;
            //}
            //else
            //{
            //    Log.Instance.Warning("HTTP AUTH ERR invalid http header");
            //    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            //}

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            return response;
        }

        private string GetHttpHeaderValue(string headerName, HttpRequestMessage request)
        {
            foreach (KeyValuePair<string, IEnumerable<string>> kp in request.Headers)
            {
                string[] values = kp.Value.ToArray<string>();
                if (kp.Key == headerName)
                    return (values != null && values.Length > 0) ? values[0] : null;
            }

            return null;
        }
    }
}