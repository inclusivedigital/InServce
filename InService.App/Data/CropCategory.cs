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
    class CropCategory : ICropCategory
    {
        [Ignore, JsonIgnore]
        public Attachment Attachment { get; set; }
        public async Task LoadAttachment()
        {
            Attachment = await Attachment.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == IconID);
        }
        [Ignore, JsonIgnore]
        public Branch Branch { get; set; }
        public async Task LoadBranch()
        {
            Branch = await Branch.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == BranchID);
        }
        [Ignore, JsonIgnore]
        public List<Crop> Crops { get; set; }
        public async Task LoadCrops()
        {
            Crops = await Crop.DB.RowsAsync.Where(c => c.CategoryID == ID).ToListAsync();
        }
        //[Ignore, JsonIgnore]
        //public static IDataTable<CropCategory> DB { get; } = new IDataTable<CropCategory>();
        [Ignore, JsonIgnore]
        public static CropCategoryDatabase DB { get; } = new CropCategoryDatabase();

        public class CropCategoryDatabase : IDataTable<CropCategory>
        {
            public CropCategoryDatabase() : base($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}/cropcategories.db") { }

            public TableQuery<CropCategory> CropCategorys => Conn.Table<CropCategory>();

            public AsyncTableQuery<CropCategory> CropCategorysAsync => AsyncConn.Table<CropCategory>();
        }
    }
}