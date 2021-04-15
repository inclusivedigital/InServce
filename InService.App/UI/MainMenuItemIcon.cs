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
    class MainMenuItemIcon
    {
        public string Base64String { get; set; }
        public int ID { get; set; }
        public string Title { get; set; }
        public bool IsEnabled { get; set; }
        public MainMenuItemIcon() => IsEnabled = true;
    }
}