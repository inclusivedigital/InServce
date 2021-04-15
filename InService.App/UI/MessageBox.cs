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
    static class MessageBox
    {
        public static void Show(this Context context, string Title, string Message)
        {
            new AlertDialog.Builder(context).SetTitle(Title).SetMessage(Message).SetPositiveButton("OK", (EventHandler<DialogClickEventArgs>)null).Create().Show();
        }
        public static void ShowToast(this Context context, string Message) => Toast.MakeText(context, Message, ToastLength.Long).Show();
    }
}