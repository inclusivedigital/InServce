using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.Data
{
    public class IAttachmentJson
    {
        public System.Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Size { get; set; }
        public string Extension { get; set; }
        public System.DateTime UploadDate { get; set; }
    }
}
