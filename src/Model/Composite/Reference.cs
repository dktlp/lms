using Newtonsoft.Json;

using System;
using System.Web;

using LMS.Model;
using LMS.Model.Annotation;

namespace LMS.Model.Composite
{
    public class Reference : CompositeDataType
    {
        public const string UriPrefix = "/api";
        public const string ArtistUri = UriPrefix + "/artist";
        public const string AccountUri = UriPrefix + "/account";
        public const string LabelUri = UriPrefix + "/label";
        public const string StatementUri = UriPrefix + "/statement";
        public const string InvoiceUri = UriPrefix + "/invoice";

        /// <summary>
        /// Gets or sets the uri (reference) to the resource.
        /// </summary>
        [JsonProperty(PropertyName = "uri")]
        [Required]
        public string Uri { get; set; }

        public int GetId()
        {
            string[] uri = Uri.Split('/');
            return int.Parse(uri[uri.Length - 1]);
        }

        public Reference()
        {
        }

        public Reference(string uri, int id)
            : base()
        {
            string server = null;
            if (HttpContext.Current != null)
                server = String.Format("http://{0}:{1}", HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port);

            Uri = String.Format("{0}{1}/{2}", server ?? "", uri, id);
        }

        public Reference(string value)
            : base()
        {
            Uri = value;
        }
    }
}