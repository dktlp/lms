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
    public enum TransactionType
    {
        [EnumMember(Value = "expense")]
        Expense = 1,
        [EnumMember(Value = "sales")]
        Sales = 2,
        [EnumMember(Value = "advance")]
        Advance = 3,
        [EnumMember(Value = "payment")]
        Payment = 4
    }
}