using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Work;
using InService.App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InService.App.Workers
{
    public class DownloadsWorker : Worker
    {
        public DownloadsWorker(Context context, WorkerParameters workerParameters) : base(context, workerParameters)
        {

        }
        public override Result DoWork()
        {
            Download();
            return Result.InvokeSuccess();
        }

        public async void Download()
        {
            AttachmentsDataSync DataSync = new AttachmentsDataSync();
            if (!DataSync.IsBusy)
            {
                await DataSync.Download();
            }
        }
    }
}