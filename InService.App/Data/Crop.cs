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
    class Crop : ICrop
    {
        [Ignore, JsonIgnore]
        public Attachment Attachment { get; set; }
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
        public CropCategory CropCategory { get; set; }
        public async Task LoadCropCategory()
        {
            CropCategory = await CropCategory.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == CategoryID);
        }
        //[Ignore, JsonIgnore]
        //public static IDataTable<Crop> DB { get; } = new IDataTable<Crop>();
        [Ignore, JsonIgnore]
        public static CropDatabase DB { get; } = new CropDatabase();

        public class CropDatabase : IDataTable<Crop>
        {
            public CropDatabase() : base($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}/crops.db") { }

            public TableQuery<Crop> Crops => Conn.Table<Crop>();

            public AsyncTableQuery<Crop> CropsAsync => AsyncConn.Table<Crop>();
        }
    }
}