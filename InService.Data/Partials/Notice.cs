using InService.Lib;
using InService.Lib.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class Notice
    {
        public bool IsExpired => DateTime.UtcNow.AddHours((double)(0)) > EndDate;
        public ArticleFlags Status
        {
            get => (ArticleFlags)StatusID;
            set => StatusID = (int)value;
        }
        public NoticeType Type
        {
            get => (NoticeType)TypeID;
            set => TypeID = (int)value;
        }

        List<IAttachmentJson> _attachments;
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

        public INotice INotice => new INotice
        {
            StatusID = StatusID,
            TypeID = TypeID,
            AttachmentsJson = AttachmentsJson,
            CreationDate = CreationDate,
            CreatorID = CreatorID,
            Description = Description,
            EndDate = EndDate,
            Heading = Heading,
            ID = ID,
            StartDate = StartDate,
            AttachmentID = AttachmentID,
        };
    }
}
