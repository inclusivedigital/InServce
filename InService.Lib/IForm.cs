using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib
{
    public interface IForm
    {
        int ID { get; set; }

        object GetValue(int FieldID);

        void SetValue(IFormField field, string[] value);

        IEnumerable<IFormField> Fields { get; }

        string FilesBasePath { get; }

        string GetFilePath(int FieldID);

        bool HasValue(int fieldID);
    }
}
