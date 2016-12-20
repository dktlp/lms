using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System;
using System.Runtime.Serialization;

using LMS;
using LMS.Model;
using LMS.Model.Composite;
using LMS.Model.Annotation;

namespace LMS.Model.Resource.Logging
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventAction
    {
        [EnumMember(Value = "create")]
        Create = 1,
        [EnumMember(Value = "read")]
        Read = 2,
        [EnumMember(Value = "update")]
        Update = 3,
        [EnumMember(Value = "delete")]
        Delete = 4,
        [EnumMember(Value = "execute")]
        Execute = 5
    }
}