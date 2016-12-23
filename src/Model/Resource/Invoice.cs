using Newtonsoft.Json;
using System;

using LMS;
using LMS.Model;
using LMS.Model.Composite;
using LMS.Model.Annotation;
using LMS.Model.Resource.Enums;

namespace LMS.Model.Resource
{
    public class Invoice : DomainResource
    {
        // Invoice number
        // EffectiveTime
        // Paid(yes/no)
        // Paypal
        // StatementReference

        /// <summary>
        /// Create an instance of the class.
        /// </summary>
        public Invoice()
        {
        }
    }
}