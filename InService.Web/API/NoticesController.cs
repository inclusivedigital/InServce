using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InService.Web.API
{
    [AllowAnonymous]
    public class NoticesController : SysController
    {
        public IEnumerable<INotice> Get()
        {
            var notices = new List<INotice>();
            string mainDir = Path;
            foreach (var item in DB.Notices.OrderByDescending(c => c.CreationDate))
            {
                var article = item.INotice;
                if (item.AttachmentID.HasValue)
                {
                    var attachment = item.Attachment;
                    var f = Directory.EnumerateFiles(mainDir).FirstOrDefault(c => c.Contains($"{item.Attachment.ID}"));
                    if (f != null)
                    {
                        Byte[] bytes = File.ReadAllBytes(f);
                        var myString = Convert.ToBase64String(bytes);
                        article.Data = myString;
                    }
                }
                notices.Add(article);
            }
            return notices;
        }
    }
}
