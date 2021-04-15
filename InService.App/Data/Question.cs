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
using InService.Lib.Data;
using Newtonsoft.Json;
using SQLite;

namespace InService.App.Data
{
    class Question : IQuestion
    {
        [Ignore, JsonIgnore]
        public List<Answer> Answers { get; set; }
        public async Task LoadAnswers()
        {
            Answers = await Answer.DB.RowsAsync.Where(c => c.QuestionID == ID).ToListAsync();
        }
        [Ignore, JsonIgnore]
        public Examination Examination { get; set; }
        public async Task LoadExamination()
        {
            Examination = await Examination.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == ExaminationID);
        }
        //[Ignore, JsonIgnore]
        //public static IDataTable<Question> DB { get; } = new IDataTable<Question>();

        [Ignore, JsonIgnore]
        public static QuestionDatabase DB { get; } = new QuestionDatabase();

        public class QuestionDatabase : IDataTable<Question>
        {
            public QuestionDatabase() : base($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}/questions.db") { }

            public TableQuery<Question> Questions => Conn.Table<Question>();

            public AsyncTableQuery<Question> QuestionsAsync => AsyncConn.Table<Question>();
        }
    }
}