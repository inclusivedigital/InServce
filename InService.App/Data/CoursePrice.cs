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
    class CoursePrice : ICoursePrice
    {
        [Ignore, JsonIgnore]
        public Course Course { get; set; }
        public async Task LoadCourse()
        {
            Course = await Course.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == CourseID);
        }
        [Ignore, JsonIgnore]
        public Currency Currency { get; set; }
        public async Task LoadCurrency()
        {
            Currency = await Currency.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == CurrencyID);
        }
        [Ignore, JsonIgnore]
        public PaymentMethod PaymentMethod { get; set; }
        public async Task LoadPaymentMethod()
        {
            PaymentMethod = await PaymentMethod.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == PaymentMethodID);
        }
        //[Ignore, JsonIgnore]
        //public static IDataTable<CoursePrice> DB { get; } = new IDataTable<CoursePrice>();
        [Ignore, JsonIgnore]
        public static CoursePriceDatabase DB { get; } = new CoursePriceDatabase();

        public class CoursePriceDatabase : IDataTable<CoursePrice>
        {
            public CoursePriceDatabase() : base($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}/invoices.db") { }

            public TableQuery<CoursePrice> CoursePrices => Conn.Table<CoursePrice>();

            public AsyncTableQuery<CoursePrice> CoursePricesAsync => AsyncConn.Table<CoursePrice>();
        }
    }
}