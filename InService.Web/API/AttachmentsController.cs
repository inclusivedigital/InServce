using InService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using InService.Lib.Data;
using System.IO;

namespace InService.Web.API
{
    public class AttachmentsController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<IAttachment> Get()
        {
            string mainDir = Path;
            var attachments = new List<IAttachment>();
            foreach (var item in DB.Attachments.Where(c => c.IsIcon == false).OrderByDescending(c => c.UploadDate))
            {
                var attachment = item.IAttachment;
                //var f = Directory.EnumerateFiles(mainDir).FirstOrDefault(c => c.Contains($"{item.ID}"));
                //if (f != null)
                //{
                //    Byte[] bytes = File.ReadAllBytes(f);
                //    var myString = Convert.ToBase64String(bytes);
                //    attachment.Data = myString;
                //}
                attachments.Add(attachment);
            }
            return attachments;
        }
    }
}
