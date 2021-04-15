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

namespace InService.App.Data
{
    class ExaminationsDataSync : IDataSync
    {
        public override Task<bool> Download()
        {
            throw new NotImplementedException();
        }

        public async override Task Sync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ProgressMax = 2;
            if (!await SyncExaminations()) return;
            OnDataSynCompleted(null);
        }

        public async Task<bool> SyncExaminations()
        {
            if (!await Upload()) return false;
            var remoteExaminations = await Download<Examination>("examinations", 2);
            if (remoteExaminations == null) return false;
            var examinations = await Examination.DB.RowsAsync.ToListAsync();
            foreach (var item in remoteExaminations.Where(r => !examinations.Any(c => c.ID == r.ID)))
            {
                Examination.DB.Insert(item);
            }
            return true;
        }

        public override async Task<bool> Upload()
        {
            var examinations = await Examination.DB.RowsAsync.Where(c => c.ID == 0).ToListAsync();
            if (examinations.Any())
            {
                var (success, rExaminations) = await Upload(examinations.ToList(), "examinations", 1);
                if (!success) return false;
                for (int i = 0; i < examinations.Count; i++) rExaminations[i].ID = examinations[i].ID;
                await Examination.DB.AsyncConn.UpdateAllAsync(rExaminations);
            }
            return true;
        }
    }
}