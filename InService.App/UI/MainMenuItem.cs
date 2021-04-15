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

namespace InService.App.UI
{
    class MainMenuItem
    {
        public string Label { get; set; }
        public int? IconResID { get; set; }
        public int? BackgroundResID { get; set; }
        public int? BackgroundID { get; set; }
        public string Count { get; set; }
        public int? IconTint { get; set; }
        public bool IsEnabled { get; set; }
        public MainMenuItem() => IsEnabled = true;
    }
}