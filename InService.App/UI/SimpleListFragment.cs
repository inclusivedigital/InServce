using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace InService.App.UI
{
    public class SimpleListFragment : Android.Support.V4.App.Fragment
    {
        public string Label { get; set; }
        public bool AllowRefresh { get; set; }
        public bool ShowFloatingActionButton { get; set; }
        public int? FloatingActionButtonResID { get; set; }
        public int? HeaderLayoutResID { get; set; }

        View RootView;
        public RecyclerView RecyclerView { get; private set; }
        public SwipeRefreshLayout SwipeRefreshLayout { get; private set; }
        public ContentLoadingProgressBar LoadingProgressBar { get; private set; }

        public event EventHandler Refreshed;
        public event EventHandler ViewCreated;
        public event EventHandler FloatingActionButtonClick;

        public override string ToString() => Label ?? base.ToString();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (savedInstanceState != null)
            {
                Label = savedInstanceState.GetString(nameof(Label));
                AllowRefresh = savedInstanceState.GetBoolean(nameof(AllowRefresh));
                ShowFloatingActionButton = savedInstanceState.GetBoolean(nameof(ShowFloatingActionButton));
                HeaderLayoutResID = savedInstanceState.GetInt(nameof(HeaderLayoutResID), 0);
                FloatingActionButtonResID = savedInstanceState.GetInt(nameof(FloatingActionButtonResID), 0);

                if (HeaderLayoutResID == 0) HeaderLayoutResID = null;
                if (FloatingActionButtonResID == 0) FloatingActionButtonResID = null;
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString(nameof(Label), Label);
            outState.PutBoolean(nameof(AllowRefresh), AllowRefresh);
            outState.PutBoolean(nameof(ShowFloatingActionButton), ShowFloatingActionButton);
            outState.PutInt(nameof(FloatingActionButtonResID), FloatingActionButtonResID ?? 0);
            outState.PutInt(nameof(HeaderLayoutResID), HeaderLayoutResID ?? 0);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(Resource.Layout.generic_list, container, false);
            RecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.gridView_items);
            RecyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            SwipeRefreshLayout = RootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_refresh_layout);
            LoadingProgressBar = RootView.FindViewById<ContentLoadingProgressBar>(Resource.Id.load_progress_bar);
            var BtnFloatingAction = RootView.FindViewById<ImageButton>(Resource.Id.btn_floating_action);

            SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueBright, Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
            SwipeRefreshLayout.Enabled = AllowRefresh;
            BtnFloatingAction.Visibility = ShowFloatingActionButton ? ViewStates.Visible : ViewStates.Gone;
            BtnFloatingAction.Click += (o, e) => FloatingActionButtonClick?.Invoke(o, e);
            if (FloatingActionButtonResID > 0) BtnFloatingAction.SetImageResource(FloatingActionButtonResID.Value);
            SwipeRefreshLayout.Refresh += (o, e) => OnRefreshed(e);
            ViewCreated?.Invoke(this, new EventArgs());
            if (HeaderLayoutResID.HasValue)
            {
                var headerView = inflater.Inflate(HeaderLayoutResID.Value, RootView.FindViewById<ViewGroup>(Resource.Id.view_list_header));
            }
            return RootView;
        }

        protected void OnRefreshed(EventArgs e) => Refreshed?.Invoke(this, e);
    }
}