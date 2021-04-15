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
    class UserExaminationDetail : IUserExaminationDetail
    {
        [JsonIgnore]
        public bool? IsSynced { get; set; }
        [Ignore, JsonIgnore]
        public Answer Answer { get; set; }
        public async Task LoadAnswer()
        {
            Answer = await Answer.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == AnswerID);
        }
        [Ignore, JsonIgnore]
        public Question Question { get; set; }
        public async Task LoadQuestion()
        {
            Question = await Question.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == QuestionID);
        }
        [Ignore, JsonIgnore]
        public UserExamination UserExamination { get; set; }
        public async Task LoadUserExamination()
        {
            UserExamination = await UserExamination.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == ExaminationID);
        }

        public IUserExaminationDetail IUserExaminationDetail => new IUserExaminationDetail { ExaminationID = ExaminationID, AnswerID = AnswerID, ID = ID, Name = Name, QuestionID = QuestionID };

        //[Ignore, JsonIgnore]
        //public static IDataTable<UserExaminationDetail> DB { get; } = new IDataTable<UserExaminationDetail>();
        [Ignore, JsonIgnore]
        public static UserExaminationDetailDatabase DB { get; } = new UserExaminationDetailDatabase();

        public class UserExaminationDetailDatabase : IDataTable<UserExaminationDetail>
        {
            public UserExaminationDetailDatabase() : base($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}/userexaminationdetails.db") { }

            public TableQuery<UserExaminationDetail> UserExaminationDetails => Conn.Table<UserExaminationDetail>();

            public AsyncTableQuery<UserExaminationDetail> UserExaminationDetailsAsync => AsyncConn.Table<UserExaminationDetail>();
        }
    }
}