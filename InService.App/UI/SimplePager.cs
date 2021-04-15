using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace InService.App.UI
{
    class SimplePagerAdapter : FragmentPagerAdapter
    {
        public override int Count => Fragments.Count;
        public List<Android.Support.V4.App.Fragment> Fragments { get; }

        public SimplePagerAdapter(Android.Support.V4.App.FragmentManager fm, params Android.Support.V4.App.Fragment[] fragments) : base(fm) { Fragments = fragments.ToList(); }

        public override Android.Support.V4.App.Fragment GetItem(int position) => Fragments[position];

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(Fragments[position].ToString().ToUpper());
        }
    }
}