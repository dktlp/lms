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
    public enum StatementStatus
    {
        [EnumMember(Value = "pending")]
        Pending = 1,
        [EnumMember(Value = "sent")]
        Sent = 2,
        [EnumMember(Value = "invoiced")]
        Invoiced = 3,
        [EnumMember(Value = "cancelled")]
        Cancelled = 4,
        [EnumMember(Value = "paid")]
        Paid = 5
    }
}