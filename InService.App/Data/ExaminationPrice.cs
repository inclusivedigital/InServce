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
    class ExaminationPrice : IExaminationPrice
    {
        [Ignore, JsonIgnore]
        public Currency Currency { get; set; }
        public async Task LoadCurrency()
        {
            Currency = await Currency.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == ExaminationID);
        }
        [Ignore, JsonIgnore]
        public Examination Examination { get; set; }
        public async Task LoadExamination()
        {
            Examination = await Examination.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == ExaminationID);
        }
        [Ignore, JsonIgnore]
        public PaymentMethod PaymentMethod { get; set; }
        public async Task LoadPaymentMethod()
        {
            PaymentMethod = await PaymentMethod.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == ExaminationID);
        }
        [Ignore, JsonIgnore]
        public static IDataTable<ExaminationPrice> DB { get; } = new IDataTable<ExaminationPrice>();
    }
}