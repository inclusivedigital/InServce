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
    class Examination : IExamination
    {
        [Ignore, JsonIgnore]
        public Course Course { get; private set; }
        [Ignore, JsonIgnore]
        public Module Module { get; private set; }
        [Ignore, JsonIgnore]
        public string DurationString => (Duration / 60) > 0 ? $"{(Duration / 60).ToString("0.0")} hour(s) {(Duration % 60)} minutes" : $"{Duration % 60} minutes";

        public async Task LoadCourse()
        {
            Course = await Course.DB.RowsAsync.Where(c => c.ID == CourseID).FirstOrDefaultAsync();
        }
        public async Task LoadModule()
        {
            Module = await Module.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == ModuleID);
        }
        [Ignore, JsonIgnore]
        public TimeSpan TimeToClose => EndDate - DateTime.Now.AddHours(0);
        [Ignore, JsonIgnore]
        public bool IsClosed => TimeToClose.TotalSeconds <= 0;
        [Ignore, JsonIgnore]
        public bool IsElapsed => DateTime.Now.AddHours(0) > EndDate;
        [Ignore, JsonIgnore]
        public bool IsInProgress => !IsElapsed && DateTime.Now.AddHours(0) > StartDate;

        [Ignore, JsonIgnore]
        public ValueChain ValueChain { get; set; }
        public async Task LoadValueChain()
        {
            ValueChain = await ValueChain.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == ValueChainID);
        }
        [Ignore, JsonIgnore]
        public List<ExaminationPrice> ExaminationPrices { get; set; }
        public async Task LoadExaminationPrices()
        {
            ExaminationPrices = await ExaminationPrice.DB.RowsAsync.Where(c => c.ExaminationID == ID).ToListAsync();
        }
        [Ignore, JsonIgnore]
        public List<Question> Questions { get;private set; }
        public async Task LoadQuestions()
        {
            Questions = await Question.DB.RowsAsync.Where(c => c.ExaminationID == ID).ToListAsync();
        }

        //[Ignore, JsonIgnore]
        //public static IDataTable<Examination> DB { get; } = new IDataTable<Examination>();

        [Ignore, JsonIgnore]
        public static ExaminationDatabase DB { get; } = new ExaminationDatabase();

        public class ExaminationDatabase : IDataTable<Examination>
        {
            public ExaminationDatabase() : base($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}/examinations.db") { }

            public TableQuery<Examination> Examinations => Conn.Table<Examination>();

            public AsyncTableQuery<Examination> ExaminationsAsync => AsyncConn.Table<Examination>();
        }
    }
}