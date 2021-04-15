using InService.Data.Helpers;
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
    public partial class Answer
    {
        public AnswerFlags Flags
        {
            get => (AnswerFlags)FlagsID;
            set => FlagsID = (int)value;
        }

        public bool IsCorrect => Flags.HasFlag(AnswerFlags.CORRECT);

        List<Attachment> _attachments;
        public List<Attachment> Attachments
        {
            get
            {
                if (_attachments == null)
                {
                    if (!string.IsNullOrWhiteSpace(AttachmentsJson))
                       // _attachments = JsonSerializer.Deserialize<List<Attachment>>(AttachmentsJson, JsonHelper.SerializerOptions);
                    _attachments = JsonConvert.DeserializeObject<List<Attachment>>(AttachmentsJson);
                    else _attachments = new List<Attachment>();
                }
                return _attachments;
            }
        }

        public IAnswer IAnswer => new IAnswer
        {
            AttachmentsJson = AttachmentsJson,
            Comments = Comments,
            CreationDate = CreationDate,
            CreatorID = CreatorID,
            FlagsID = FlagsID,
            ID = ID,
            Name = Name,
            QuestionID = QuestionID,
        };
    }
}
