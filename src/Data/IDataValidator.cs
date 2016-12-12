using System;

namespace LMS.Data
{
    public interface IDataValidator<T>
    {
        DataValidationResult Validate(T item);
    }
}
