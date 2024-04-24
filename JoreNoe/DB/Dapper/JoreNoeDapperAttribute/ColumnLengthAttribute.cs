using System;

namespace JoreNoe.DB.Dapper.JoreNoeDapperAttribute
{
    public class ColumnLengthAttribute : Attribute
    {
        public long Length { get; set; }

        public ColumnLengthAttribute(long length = 255)
        {
            Length = length;
        }
    }
}
