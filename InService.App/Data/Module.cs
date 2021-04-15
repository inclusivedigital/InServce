using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using InService.App.UI;
using InService.Lib.Data;
using Newtonsoft.Json;
using SQLite;

namespace InService.App.Data
{
    class Module : IModule
    {
        [Ignore, JsonIgnore]
        public List<Article> Articles { get; set; }
        public async Task LoadArticles()
        {
            Articles = await Article.DB.RowsAsync.Where(c => c.ModuleID == ID).ToListAsync();
        }
        [Ignore, JsonIgnore]
        public Course Course { get; set; }
        public async Task LoadCourse()
        {
            Course = await Course.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == CourseID);
        }
        [Ignore, JsonIgnore]
        public List<Examination> Examinations { get; set; }
        public async Task LoadExaminations()
        {
            Examinations = await Examination.DB.RowsAsync.Where(c => c.ModuleID == ID).ToListAsync();
        }
        [Ignore, JsonIgnore]
        public Attachment Attachment { get; set; }
        public async Task LoadAttachment()
        {
            Attachment = await Attachment.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == IconID);
        }
        [Ignore, JsonIgnore]
        public Bitmap Thumbnail { get; private set; }

        public async Task<bool> LoadThumbnail(Context context)
        {
            BitmapFactory.Options Opts = new BitmapFactory.Options
            {
                InSampleSize = 2
            };
            FileInfo imgFile = new FileInfo($"{context.GetExternalFilesDir(null)}/images/{Attachment.ID}{Attachment.Extension}");
            if (imgFile.Exists)
            {
                var bmp = await BitmapFactory.DecodeFileAsync(imgFile.FullName, Opts);
                if (bmp != null) Thumbnail = bmp.ToRoundedCornerBitmap(12);
            }
            else Thumbnail = null;
            return Thumbnail != null;
        }



        //[Ignore, JsonIgnore]
        //public static IDataTable<Module> DB { get; } = new IDataTable<Module>();

        [Ignore, JsonIgnore]
        public static ModuleDatabase DB { get; } = new ModuleDatabase();

        public class ModuleDatabase : IDataTable<Module>
        {
            public ModuleDatabase() : base($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}/modules.db") { }

            public TableQuery<Module> Modules => Conn.Table<Module>();

            public AsyncTableQuery<Module> ModulesAsync => AsyncConn.Table<Module>();
        }
    }
}