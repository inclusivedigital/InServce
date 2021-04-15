using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    class Branch : IBranch
    {
        //[Ignore, JsonIgnore]
        //public static IDataTable<Branch> DB { get; } = new IDataTable<Branch>();
        [Ignore, JsonIgnore]
        public static BranchDatabase DB { get; } = new BranchDatabase();

        public class BranchDatabase : IDataTable<Branch>
        {
            public BranchDatabase() : base($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}/branches.db") { }

            public TableQuery<Branch> Branchs => Conn.Table<Branch>();

            public AsyncTableQuery<Branch> BranchsAsync => AsyncConn.Table<Branch>();
        }
    }
}