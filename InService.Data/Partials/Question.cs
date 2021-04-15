using InService.Data.Helpers;
using InService.Lib.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class Question
    {
        List<Attachment> _attachments;
        public List<Attachment> Attachments
        {
            get
            {
                if (_attachments == null)
                {
                    if (!string.IsNullOrWhiteSpace(AttachmentsJson))
                        //_attachments = JsonSerializer.Deserialize<List<Attachment>>(AttachmentsJson, JsonHelper.SerializerOptions);
                        _attachments = JsonConvert.DeserializeObject<List<Attachment>>(AttachmentsJson);
                    else _attachments = new List<Attachment>();
                }
                return _attachments;
            }
        }

        public IQuestion IQuestion => new IQuestion
        {
            AttachmentsJson = AttachmentsJson,
            CreationDate = CreationDate,
            CreatorID = CreatorID,
            ExaminationID = ExaminationID,
            ID = ID,
            Name = Name,
            Score = Score,
        };
    }
}
