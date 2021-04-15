using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using InService.Lib.Data;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InService.App.Data
{
    class ExtensionOfficer : IExtensionOfficer
    {
        [Ignore, JsonIgnore]
        public static IDataTable<ExtensionOfficer> DB { get; } = new IDataTable<ExtensionOfficer>();
    }
}