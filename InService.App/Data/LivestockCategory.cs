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
    class LivestockCategory : ILivestockCategory
    {
        [Ignore, JsonIgnore]
        public virtual Branch Branch { get; set; }
        public async Task LoadBranch()
        {
            Branch = await Branch.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == BranchID);
        }
        [Ignore, JsonIgnore]
        public List<Livestock> Livestocks { get; set; }
        public async Task LoadLivestocks()
        {
            Livestocks = await Livestock.DB.RowsAsync.Where(c => c.ID == BranchID).ToListAsync();
        }
        [Ignore, JsonIgnore]
        public static IDataTable<LivestockCategory> DB { get; } = new IDataTable<LivestockCategory>();
    }
}