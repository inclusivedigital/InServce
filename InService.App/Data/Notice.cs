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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InService.App.Data
{
    class Notice : INotice
    {
        [Ignore, JsonIgnore]
        public bool IsExpired => DateTime.UtcNow.AddHours((double)(0)) > EndDate;
        [Ignore, JsonIgnore]
        public ArticleFlags Status
        {
            get => (ArticleFlags)StatusID;
            set => StatusID = (int)value;
        }
        [Ignore, JsonIgnore]
        public NoticeType Type
        {
            get => (NoticeType)TypeID;
            set => TypeID = (int)value;
        }
        //[Ignore, JsonIgnore]
        //public static IDataTable<Notice> DB { get; } = new IDataTable<Notice>();
        [Ignore, JsonIgnore]
        public static NoticeDatabase DB { get; } = new NoticeDatabase();

        public class NoticeDatabase : IDataTable<Notice>
        {
            public NoticeDatabase() : base($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}/notices.db") { }

            public TableQuery<Notice> Notices => Conn.Table<Notice>();

            public AsyncTableQuery<Notice> NoticesAsync => AsyncConn.Table<Notice>();
        }
    }
}