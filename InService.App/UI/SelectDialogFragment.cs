using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace InService.App.UI
{
    class SelectDialogFragment : Android.Support.V4.App.DialogFragment
    {
        View RootView;
        public RecyclerView RecyclerView { get; private set; }
        public event EventHandler ViewCreated;
        public event EventHandler<Android.Support.V7.Widget.SearchView.QueryTextChangeEventArgs> QueryTextChanged;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(Resource.Layout.searchable_list, container, false);
            RootView.SetBackgroundResource(Resource.Color.colorWindowBackground);
            RecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.gridView_items);
            var toolbar = RootView.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_top);
            toolbar.InflateMenu(Resource.Menu.appcompat_search);
            var searchView = (Android.Support.V7.Widget.SearchView)toolbar.Menu.FindItem(Resource.Id.action_search).ActionView;
            searchView.Iconified = false;
            searchView.QueryTextChange += (o, e) => QueryTextChanged?.Invoke(o, e);
            RecyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            ViewCreated?.Invoke(this, null);
            return RootView;
        }
    }
}