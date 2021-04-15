using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib
{
    public interface IFormField
    {
        int ID { get; set; }
        string Hint { get; set; }
        long? MinValue { get; set; }
        long? MaxValue { get; set; }
        DataTypes DataType { get; }
        FieldFlags Flags { get; set; }
        bool IsMandatory { get; set; }
        bool IsKey { get; set; }
        bool IsEditable { get; set; }
        bool IsUnique { get; set; }
        bool IsPrintable { get; set; }
        bool IsEmail { get; }
        bool IsHidden { get; set; }
        bool IsPhoneNumber { get; }
        bool IsDate { get; }
        bool IsOptions { get; }
        bool IsBoolean { get; }
        bool IsNumber { get; }
        bool IsText { get; }
        DateTime? MinDate { get; }
        DateTime? MaxDate { get; }
        List<string> Options { get; }
        string FieldName { get; }
    }
}
