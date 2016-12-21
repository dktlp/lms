using System;

using LMS.Model;
using LMS.Model.Resource;

namespace LMS.Data
{
    public class LabelValidator : DataValidator<Label>
    {
        public LabelValidator()
            : base()
        {
        }

        public override DataValidationResult Validate(Label item)
        {
            return base.Validate(item);
        }
    }
}
