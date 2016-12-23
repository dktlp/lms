using System;

using LMS.Model;
using LMS.Model.Resource;

namespace LMS.Data
{
    public class InvoiceValidator : DataValidator<Invoice>
    {
        public InvoiceValidator()
            : base()
        {
        }

        public override DataValidationResult Validate(Invoice item)
        {
            return base.Validate(item);
        }
    }
}
