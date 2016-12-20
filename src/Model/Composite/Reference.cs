using Newtonsoft.Json;
using System;

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

        /// <summary>
        /// Gets or sets the uri (reference) to the resource.
        /// </summary>
        [JsonProperty(PropertyName = "reference")]
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
            Uri = String.Format("{0}/{1}", uri, id);
        }

        public Reference(string value)
            : base()
        {
            Uri = value;
        }
    }
}