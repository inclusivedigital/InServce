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
using InService.Lib.Data;
using Newtonsoft.Json;
using SQLite;

namespace InService.App.Data
{
    class Attachment : IAttachment
    {
        //[Ignore, JsonIgnore]
        //public static IDataTable<Attachment> DB { get; } = new IDataTable<Attachment>();

        [Ignore, JsonIgnore]
        public static AttachmentDatabase DB { get; } = new AttachmentDatabase();

        public class AttachmentDatabase : IDataTable<Attachment>
        {
            public AttachmentDatabase() : base($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}/attachments.db") { }

            public TableQuery<Attachment> Attachments => Conn.Table<Attachment>();

            public AsyncTableQuery<Attachment> AttachmentsAsync => AsyncConn.Table<Attachment>();
        }
    }
}