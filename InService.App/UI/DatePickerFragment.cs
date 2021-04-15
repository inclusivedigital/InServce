using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InService.App.UI
{
    class DatePickerFragment : Android.Support.V4.App.DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();

        public event EventHandler<DateTime> DateSelected;

        public static readonly string DefaultDateFormat = "dd-MMM-yyyy";

        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public DateTime? DefaultDate { get; set; }
        public EditText DisplayText { get; set; }
        public DateTime? SelectedDate { get; private set; }
        public string DateFormat { get; set; }

        public DatePickerFragment(EditText displayText, Android.Support.V4.App.FragmentManager fragmentManager)
        {
            DisplayText = displayText;
            DisplayText.TextChanged += (o, e) =>
            {
                try
                {
                    String curDateString = DisplayText.Text.Trim();
                    String format = "";
                    if (Regex.IsMatch(curDateString, "\\d{2}(\\/|-)[a-zA-Z]{3}(\\/|-)\\d{4}"))
                        format = "dd/MMM/yyy";
                    else if (Regex.IsMatch(curDateString, "\\d{2}(\\/|-)\\d{2}(\\/|-)\\d{4}"))
                        format = "dd/MM/yyy";
                    else return;
                    DateTime curDate = DateTime.Parse(curDateString);
                    if (SelectedDate != null && curDate == SelectedDate) return;
                    SelectedDate = curDate;
                    DisplayText.Text = curDate.ToString(DefaultDateFormat);
                    DateSelected?.Invoke(this, SelectedDate.Value);
                }
                catch (Exception)
                {
                }
            };
            DisplayText.Click += (o, e) => Show(fragmentManager, TAG);
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime date = DefaultDate ?? DateTime.Now;
            try
            {
                date = DateTime.ParseExact(DisplayText.Text, DateFormat ?? DefaultDateFormat, CultureInfo.CurrentCulture);
            }
            catch
            {
            }
            DatePickerDialog dialog = new DatePickerDialog(Activity, this, date.Year, date.Month - 1, date.Day);
            if (MinDate.HasValue) dialog.DatePicker.MinDate = MinDate.Value.Date.GetJavaTimeFromDate();
            if (MaxDate.HasValue) dialog.DatePicker.MaxDate = MaxDate.Value.Date.GetJavaTimeFromDate();
            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            SelectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            if (DisplayText != null)
            {
                DisplayText.Text = SelectedDate.Value.ToString(DateFormat ?? DefaultDateFormat);
            }
            DateSelected?.Invoke(this, SelectedDate.Value);
        }
    }
}