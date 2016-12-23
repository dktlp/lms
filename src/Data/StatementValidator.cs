using System;

using LMS.Model;
using LMS.Model.Resource;

namespace LMS.Data
{
    public class StatementValidator : DataValidator<Statement>
    {
        public StatementValidator()
            : base()
        {
        }

        public override DataValidationResult Validate(Statement item)
        {
            return base.Validate(item);
        }
    }
}
