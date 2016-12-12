using System;

namespace LMS.Model.Annotation
{
    public class ArrayStringLengthAttribute : Attribute
    {
        public int Length { get; set; }

        public ArrayStringLengthAttribute(int length)
        {
            Length = length;
        }
    }
}