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
    public partial class Article
    {
        public ArticleFlags Flags
        {
            get => (ArticleFlags)FlagsID;
            set => FlagsID = (int)value;
        }

        public IArticle IArticle => new IArticle
        {
            CourseID = CourseID,
            CreationDate = CreationDate,
            ValueChainID = ValueChainID,
            ID = ID,
            CreatorID = CreatorID,
            Description = Description,
            FlagsID = FlagsID,
            IsDefault = IsDefault,
            ModuleID = ModuleID,
            Name = Name,
            AttachmentsJson = AttachmentsJson,
            AttachmentID = AttachmentID,
        };

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
    }
}
