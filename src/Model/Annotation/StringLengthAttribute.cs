using System;

namespace LMS.Model.Annotation
{
    public class StringLengthAttribute : Attribute
    {
        public int Length { get; set; }

        public StringLengthAttribute(int length)
        {
            Length = length;
        }
    }
}