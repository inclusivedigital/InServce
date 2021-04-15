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
    class Livestock : ILivestock
    {
        [Ignore, JsonIgnore]
        public virtual Attachment Attachment { get; set; }
        public async Task LoadAttachment()
        {
            Attachment = await Attachment.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == IconID);
        }
        [Ignore, JsonIgnore]
        public Course Course { get; set; }
        public async Task LoadCourse()
        {
            Course = await Course.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == CourseID);
        }
        [Ignore, JsonIgnore]
        public LivestockCategory LivestockCategory { get; set; }
        public async Task LoadLivestockCategory()
        {
            LivestockCategory = await LivestockCategory.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == CategoryID);
        }
        //[Ignore, JsonIgnore]
        //public static IDataTable<Livestock> DB { get; } = new IDataTable<Livestock>();
        [Ignore, JsonIgnore]
        public static LivestockDatabase DB { get; } = new LivestockDatabase();

        public class LivestockDatabase : IDataTable<Livestock>
        {
            public LivestockDatabase() : base($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}/livestock.db") { }

            public TableQuery<Livestock> Livestocks => Conn.Table<Livestock>();

            public AsyncTableQuery<Livestock> LivestocksAsync => AsyncConn.Table<Livestock>();
        }
    }
}