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
    class ValueChain : IValueChain
    {
        [Ignore, JsonIgnore]
        public List<Article> Articles { get; set; }
        public async Task LoadArticles()
        {
            Articles = await Article.DB.RowsAsync.Where(c => c.ValueChainID == ID).ToListAsync();
        }
        [Ignore, JsonIgnore]
        public Branch Branch { get; set; }
        public async Task LoadBranch()
        {
            Branch = await Branch.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == BranchID);
        }
        [Ignore, JsonIgnore]
        public List<Course> Courses { get; set; }
        public async Task LoadCourses()
        {
            Courses = await Course.DB.RowsAsync.Where(c => c.ValueChainID == ID).ToListAsync();
        }
        [Ignore, JsonIgnore]
        public List<Examination> Examinations { get; set; }
        public async Task LoadExaminations()
        {
            Examinations = await Examination.DB.RowsAsync.Where(c => c.ValueChainID == ID).ToListAsync();
        }
        //[Ignore, JsonIgnore]
        //public static IDataTable<ValueChain> DB { get; } = new IDataTable<ValueChain>();

        [Ignore, JsonIgnore]
        public static ValueChainDatabase DB { get; } = new ValueChainDatabase();

        public class ValueChainDatabase : IDataTable<ValueChain>
        {
            public ValueChainDatabase() : base($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}/valuechains.db") { }

            public TableQuery<ValueChain> ValueChains => Conn.Table<ValueChain>();

            public AsyncTableQuery<ValueChain> ValueChainsAsync => AsyncConn.Table<ValueChain>();
        }
    }
}