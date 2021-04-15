using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using InService.Lib;
using InService.Lib.Data;
using Newtonsoft.Json;
using SQLite;

namespace InService.App.Data
{
    class Instruction : IInstruction
    {
        [Ignore, JsonIgnore]
        public ExaminationType Type
        {
            get => (ExaminationType)ExamTypeID;
            set => ExamTypeID = (int)value;
        }
        [Ignore, JsonIgnore]
        public static IDataTable<Instruction> DB { get; } = new IDataTable<Instruction>();
    }
}