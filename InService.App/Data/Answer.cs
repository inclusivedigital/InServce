using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    class Answer : IAnswer
    {
        [Ignore, JsonIgnore]
        public AnswerFlags Flags
        {
            get => (AnswerFlags)FlagsID;
            set => FlagsID = (int)value;
        }
        [Ignore, JsonIgnore]
        public Question Question { get; set; }

        public async Task LoadQuestion()
        {
            Question = await Question.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == QuestionID);
        }

        [Ignore, JsonIgnore]
        public bool IsCorrect => Flags.HasFlag(AnswerFlags.CORRECT);
        [Ignore, JsonIgnore]
        public static IDataTable<Answer> DB { get; } = new IDataTable<Answer>();
    }
}