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
    public enum AccountStatus
    {
        [EnumMember(Value = "open")]
        Open = 1,
        [EnumMember(Value = "closed")]
        Closed = 2
    }
}