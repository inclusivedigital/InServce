using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class Attachment
    {
        //  public string Path => $"~/Attachments/";
        public string Path => @"C:\InService\Articles\Uploads\";

        public IAttachment IAttachment => new IAttachment
        {
            UploadDate = UploadDate,
            Name = Name,
            CreatorID = CreatorID,
            Description = Description,
            Extension = Extension,
            ID = ID,
            Size = Size,
            IsDownloaded = true,
        };
        public IAttachmentJson IAttachmentJson => new IAttachmentJson
        {
            UploadDate = UploadDate,
            Name = Name,
            Description = Description,
            Extension = Extension,
            ID = ID,
            Size = Size,

        };
    }
}
