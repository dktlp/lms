using System;

using LMS.Model;
using LMS.Model.Resource;
using LMS.Model.Resource.Enums;

namespace LMS.Data
{
    public class TransactionValidator : DataValidator<Transaction>
    {
        public TransactionValidator()
            : base()
        {
        }

        public override DataValidationResult Validate(Transaction item)
        {
            DataValidationResult validationResult = base.Validate(item);
            if (validationResult.IsValid)
            {
                // Validate that attributes are valid according to the TransactionType.
                switch(item.Type)
                {
                    case TransactionType.Expense:
                    case TransactionType.Advance:
                    case TransactionType.Payment:
                        {
                            if (item.Amount >= 0)
                            {
                                validationResult.IsValid = false;
                                validationResult.Message = "For transaction types 'expense/advance/payment' the field 'amount' must be a negative number.";
                            }
                            break;
                        }
                    case TransactionType.Sales:
                        {
                            if (item.Amount <= 0)
                            {
                                validationResult.IsValid = false;
                                validationResult.Message = "For transaction type 'sales' the field 'amount' must be a positive number.";
                            }
                            break;
                        }
                }
            }

            return validationResult;
        }
    }
}
