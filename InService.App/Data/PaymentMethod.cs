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
    class PaymentMethod : IPaymentMethod
    {
        [Ignore, JsonIgnore]
        public static IDataTable<PaymentMethod> DB { get; } = new IDataTable<PaymentMethod>();
    }
}