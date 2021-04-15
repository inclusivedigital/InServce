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
using InService.App.Auth;
using InService.Lib.Data;

namespace InService.App.Data
{
    class AttachmentsDataSync : IDataSync
    {
        public AttachmentsDataSync() { ProgressMax = 2; }

        public bool SyncAll { get; set; } = true;

        public override Task Sync()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> Download()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SyncAttachments()
        {
            var remoteInvoices = await Download<Attachment>("attachments", 4);
            if (remoteInvoices == null) return false;
            foreach (var item in remoteInvoices)
            {
                var localattach = await Attachment.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == item.ID);
                if (localattach != null)
                {
                    if (localattach.IsDownloaded) continue;
                }
                else
                {
                    item.IsDownloaded = false;
                    await Attachment.DB.InsertAsync(item);
                }
            }
            SessionManager.SyncFiles = true;
            SessionManager.SyncAttachments = false;
            return true;
        }


        public override Task<bool> Upload()
        {
            throw new NotImplementedException();
        }
    }
}