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
    class Payment : IPayment
    {
        [Ignore, JsonIgnore]
        public virtual Currency Currency { get; set; }
        public async Task LoadCurrency()
        {
            Currency = await Currency.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == CurrencyID);
        }
        [Ignore, JsonIgnore]
        public virtual PaymentMethod PaymentMethod { get; set; }
        public async Task LoadPaymentMethod()
        {
            PaymentMethod = await PaymentMethod.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == PaymentMethodID);
        }

        [Ignore, JsonIgnore]
        public static IDataTable<Payment> DB { get; } = new IDataTable<Payment>();
    }
}