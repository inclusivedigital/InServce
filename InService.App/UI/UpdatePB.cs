using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace InService.App.UI
{
    public class UpdatePB : AsyncTask<int, int, string>
    {
        Context mcontext;
        ProgressBar mpb;
        TextView mtv;
        int Count;
        public UpdatePB(Context context, ProgressBar pb, TextView tv, int Count)
        {
            this.mcontext = context;
            this.mpb = pb;
            this.mtv = tv;
            this.Count = Count;
        }
        protected override string RunInBackground(params int[] @params)
        {
            // TODO Auto-generated method stub
            for (int i = 1; i <= Count; i++)
            {
                try
                {
                    Thread.Sleep(3000);
                }
                catch (Java.Lang.InterruptedException e)
                {
                    // TODO Auto-generated catch block
                    Android.Util.Log.Error("lv", e.Message);
                }
                mpb.IncrementProgressBy(25);
                PublishProgress(i * 25);
            }
            return "finish";
        }

        protected override void OnProgressUpdate(params int[] values)
        {
            mtv.Text = Java.Lang.String.ValueOf(values[0]);
            Android.Util.Log.Error("lv==", values[0] + "");
        }
        protected override void OnPostExecute(string result)
        {
            //  mcontext.Title = result;
        }


    }

}