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
    class NonValueChain : INonValueChain
    {
        [Ignore, JsonIgnore]
        public virtual Branch Branch { get; set; }
        public async Task LoadBranch()
        {
            Branch = await Branch.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == BranchID);
        }
        [Ignore, JsonIgnore]
        public List<Course> Courses { get; set; }
        public async Task LoadCourses()
        {
            Courses = await Course.DB.RowsAsync.Where(c => c.NonValueChainID == ID).ToListAsync();
        }
        [Ignore, JsonIgnore]
        public static IDataTable<NonValueChain> DB { get; } = new IDataTable<NonValueChain>();
    }
}