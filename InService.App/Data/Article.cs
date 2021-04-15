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
using InService.Lib.Data;
using Newtonsoft.Json;
using SQLite;

namespace InService.App.Data
{
    class Article : IArticle
    {
        [Ignore, JsonIgnore]
        public Course Course { get; set; }
        public async Task LoadCourse()
        {
            Course = await Course.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == CourseID);
        }
        [Ignore, JsonIgnore]
        public Attachment Attachment { get; set; }
        public async Task LoadAttachment()
        {
            Attachment = await Attachment.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == AttachmentID);
        }
        [Ignore, JsonIgnore]
        public Module Module { get; set; }
        public async Task LoadModule()
        {
            Module = await Module.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == ModuleID);
        }

        List<IAttachmentJson> _attachments;
        [Ignore, JsonIgnore]
        public List<IAttachmentJson> Attachments
        {
            get
            {
                if (_attachments == null)
                {
                    if (!string.IsNullOrWhiteSpace(AttachmentsJson))
                        //_attachments = JsonSerializer.Deserialize<List<Attachment>>(AttachmentsJson, JsonHelper.SerializerOptions);
                        _attachments = JsonConvert.DeserializeObject<List<IAttachmentJson>>(AttachmentsJson);
                    else _attachments = new List<IAttachmentJson>();
                }
                return _attachments;
            }
        }

        [Ignore, JsonIgnore]
        public ValueChain ValueChain { get; set; }
        public async Task LoadValueChain()
        {
            ValueChain = await ValueChain.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == ValueChainID);
        }
        //[Ignore, JsonIgnore]
        //public static IDataTable<Article> DB { get; } = new IDataTable<Article>();


        [Ignore, JsonIgnore]
        public static ArticleDatabase DB { get; } = new ArticleDatabase();

        public class ArticleDatabase : IDataTable<Article>
        {
            public ArticleDatabase() : base($"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}/articles.db") { }

            public TableQuery<Article> Articles => Conn.Table<Article>();

            public AsyncTableQuery<Article> ArticlesAsync => AsyncConn.Table<Article>();
        }


    }
}