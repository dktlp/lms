using System;

using LMS.Model;
using LMS.Model.Resource;

namespace LMS.Data
{
    public class AccountValidator : DataValidator<Account>
    {
        public AccountValidator()
            : base()
        {
        }

        public override DataValidationResult Validate(Account item)
        {
            return base.Validate(item);
        }
    }
}
