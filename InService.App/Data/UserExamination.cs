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
    class UserExamination : IUserExamination
    {
        [JsonIgnore]
        public bool? IsSynced { get; set; }
        [Ignore, JsonIgnore]
        public Examination Examination { get; set; }
        public async Task LoadExamination()
        {
            Examination = await Examination.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == ExaminationID);
            if (Examination == null) Examination = await Examination.DB.RowsAsync.FirstOrDefaultAsync(c => c.LocalID == LocalExamID);
        }
        [Ignore, JsonIgnore]
        public List<UserExaminationDetail> UserExaminationDetails { get; set; }
        public async Task LoadUserExaminationDetails()
        {
            UserExaminationDetails = await UserExaminationDetail.DB.RowsAsync.Where(c => c.ExaminationID == ID).ToListAsync();
        }

        [Ignore, JsonIgnore]
        public IUserExamination IUserExamination => new IUserExamination { ID = ID, CreationDate = CreationDate, AttachmentsJson = AttachmentsJson, DetailsJson = DetailsJson, EndTime = EndTime, ExaminationID = ExaminationID, Latitude = Latitude, Longitude = Longitude, StartTime = StartTime, UserID = UserID };

        List<IUserExaminationDetail> _attachments;
        [Ignore, JsonIgnore]
        public List<IUserExaminationDetail> Details
        {
            get
            {
                if (_attachments == null)
                {
                    if (!string.IsNullOrWhiteSpace(DetailsJson))
                        _attachments = JsonConvert.DeserializeObject<List<IUserExaminationDetail>>(DetailsJson);
                    else _attachments = new List<IUserExaminationDetail>();
                }
                return _attachments;
            }
        }

        public UserExamination() { }

        public UserExamination(IUserExamination examination)
        {
            ID = examination.ID;
            ExaminationID = examination.ExaminationID;
            CreationDate = examination.CreationDate;
            AttachmentsJson = examination.AttachmentsJson;
            DetailsJson = examination.DetailsJson;
            EndTime = examination.EndTime;
            ExaminationID = examination.ExaminationID;
            Latitude = examination.Latitude;
            Longitude = examination.Longitude;
            StartTime = examination.StartTime;
            UserID = examination.UserID;
        }


        //[Ignore, JsonIgnore]
        //public static IDataTable<UserExamination> DB { get; } = new IDataTable<UserExamination>();

        [Ignore, JsonIgnore]
        public static UserExaminationDatabase DB { get; } = new UserExaminationDatabase();

        public class UserExaminationDatabase : IDataTable<UserExamination>
        {
            public UserExaminationDatabase() : base($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}/userexaminations.db") { }

            public TableQuery<UserExamination> UserExaminations => Conn.Table<UserExamination>();

            public AsyncTableQuery<UserExamination> UserExaminationsAsync => AsyncConn.Table<UserExamination>();
        }
    }
}