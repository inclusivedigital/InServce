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

namespace InService.App
{
    static class ExtMethods
    {
        public static bool HasInternetAccess(this Xamarin.Essentials.NetworkAccess networkAccess)
             => networkAccess == Xamarin.Essentials.NetworkAccess.Internet || networkAccess == Xamarin.Essentials.NetworkAccess.ConstrainedInternet;
    }
}