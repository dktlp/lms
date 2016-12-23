using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System;
using System.Runtime.Serialization;

using LMS;
using LMS.Model;
using LMS.Model.Composite;
using LMS.Model.Annotation;

namespace LMS.Model.Resource.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InvoiceStatus
    {
        [EnumMember(Value = "received")]
        Received = 1,
        [EnumMember(Value = "paid")]
        Paid = 2
    }
}