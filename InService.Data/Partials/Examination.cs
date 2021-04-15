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
    public partial class Examination
    {
        public ExaminationType Type
        {
            get => (ExaminationType)TypeID;
            set => TypeID = (int)value;
        }
        public ExaminationFlags Flags
        {
            get => (ExaminationFlags)FlagsID;
            set => FlagsID = (int)value;
        }

        public ExaminationAudience Audience
        {
            get => (ExaminationAudience)TargetAudienceID;
            set => TargetAudienceID = (int)value;
        }

        public bool IsPremium => Flags.HasFlag(ExaminationFlags.PREMIUM_EXAMINATION);
        public bool IsFree => Flags.HasFlag(ExaminationFlags.FREE_EXAMINATION);
        public QuestionPaperFormat PaperFormat
        {
            get => (QuestionPaperFormat)PaperFormatID;
            set => PaperFormatID = (int)value;
        }

        public TimeSpan TimeToClose => EndDate - DateTime.Now.AddHours(0);

        public bool IsClosed => TimeToClose.TotalSeconds <= 0;
        public bool IsElapsed => DateTime.Now.AddHours(0) > EndDate;
        public bool IsInProgress => !IsElapsed && DateTime.Now.AddHours(0) > StartDate;

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

        public ExaminationPrice CurrentPrice => IsPremium ? ExaminationPrices.OrderByDescending(c => c.CreationDate).FirstOrDefault() : null;
        public IExamination IExamination => new IExamination
        {
            AttachmentsJson = AttachmentsJson,
            CourseID = CourseID,
            CreationDate = CreationDate,
            CreatorID = CreatorID,
            Duration = Duration,
            EndDate = EndDate,
            FlagsID = FlagsID,
            ID = ID,
            MaxAttempts = MaxAttempts,
            ModuleID = ModuleID,
            PaperFormatID = PaperFormatID,
            StartDate = StartDate,
            TargetAudienceID = TargetAudienceID,
            Topic = Topic,
            TypeID = TypeID,
            ValueChainID = ValueChainID,
            Year = Year,
        };
    }
}
