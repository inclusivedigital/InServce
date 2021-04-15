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
    class Course : ICourse
    {
        [Ignore, JsonIgnore]
        public List<Article> Articles { get; set; }
        public async Task LoadArticles()
        {
            Articles = await Article.DB.RowsAsync.Where(c => c.CourseID == ID).ToListAsync();
        }
        [Ignore, JsonIgnore]
        public List<Examination> AllExercises { get; set; }
        public async Task LoadAllExercises()
        {
            AllExercises = await Examination.DB.RowsAsync.Where(c => c.CourseID == ID).ToListAsync();
        }
        [Ignore, JsonIgnore]
        public List<UserExamination> UserExaminations { get; set; }
        public async Task LoadUserExaminations()
        {
            UserExaminations = await UserExamination.DB.RowsAsync.Where(c => c.CourseID == ID).ToListAsync();
        }
        [Ignore, JsonIgnore]
        public Branch Branch { get; set; }
        public async Task LoadBranch()
        {
            Branch = await Branch.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == BranchID);
        }
        [Ignore, JsonIgnore]
        public NonValueChain NonValueChain { get; set; }
        public async Task LoadNonValueChain()
        {
            NonValueChain = await NonValueChain.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == NonValueChainID);
        }
        [Ignore, JsonIgnore]
        public ValueChain ValueChain { get; set; }
        public async Task LoadValueChain()
        {
            ValueChain = await ValueChain.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == ValueChainID);
        }
        [Ignore, JsonIgnore]
        public List<Crop> Crops { get; set; }
        public async Task LoadCrops()
        {
            Crops = await Crop.DB.RowsAsync.Where(c => c.CourseID == ID).ToListAsync();
        }
        [Ignore, JsonIgnore]
        public List<Examination> Examinations { get; set; }
        public async Task LoadExaminations()
        {
            Examinations = await Examination.DB.RowsAsync.Where(c => c.CourseID == ID).ToListAsync();
        }
        [Ignore, JsonIgnore]
        public List<Livestock> Livestocks { get; set; }
        public async Task LoadLivestocks()
        {
            Livestocks = await Livestock.DB.RowsAsync.Where(c => c.CourseID == ID).ToListAsync();
        }
        [Ignore, JsonIgnore]
        public List<Module> Modules { get; set; }
        public async Task LoadModules()
        {
            Modules = await Module.DB.RowsAsync.Where(c => c.CourseID == ID).ToListAsync();
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
        //public static IDataTable<Course> DB { get; } = new IDataTable<Course>();

        [Ignore, JsonIgnore]
        public static CourseDatabase DB { get; } = new CourseDatabase();

        public class CourseDatabase : IDataTable<Course>
        {
            public CourseDatabase() : base($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}/courses.db") { }

            public TableQuery<Course> Courses => Conn.Table<Course>();

            public AsyncTableQuery<Course> CoursesAsync => AsyncConn.Table<Course>();
        }
    }
}