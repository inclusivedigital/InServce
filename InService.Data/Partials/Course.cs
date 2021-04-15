using InService.Lib.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class Course
    {
        public ICourse ICourse => new ICourse
        {
            BranchID = BranchID,
            Code = Code,
            CreationDate = CreationDate,
            CreatorID = CreatorID,
            Description = Description,
            ID = ID,
            Name = Name,
            IconID = IconID,
            FinalExamQuestions = FinalExamQuestions
        };

        public CoursePrice CurrentPrice => CoursePrices.OrderByDescending(c => c.RevisionDate).FirstOrDefault();

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
    }
}
