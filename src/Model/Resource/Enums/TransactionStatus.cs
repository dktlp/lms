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
    public enum TransactionStatus
    {
        [EnumMember(Value = "committed")]
        Committed = 1,
        [EnumMember(Value = "draft")]
        Draft = 2,
        [EnumMember(Value = "failed")]
        Failed = 3
    }
}