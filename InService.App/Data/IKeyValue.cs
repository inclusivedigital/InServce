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

namespace InService.App.Data
{
    interface IKeyValuePair
    {
        string Key { get; }
        string Value { get; }
    }

    class GenericKVPair : IKeyValuePair
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public GenericKVPair(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}