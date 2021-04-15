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
    public class ArticlesController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<IArticle> Get()
        {
            var articles = new List<IArticle>();
            string mainDir = Path;
            foreach (var item in DB.Articles.OrderByDescending(c => c.CreationDate))
            {
                var article = item.IArticle;
                //if (item.AttachmentID.HasValue)
                //{
                //    var attachment = item.Attachment;
                //    var f = Directory.EnumerateFiles(mainDir).FirstOrDefault(c => c.Contains($"{item.Attachment.ID}"));
                //    if (f != null)
                //    {
                //        Byte[] bytes = File.ReadAllBytes(f);
                //        var myString = Convert.ToBase64String(bytes);
                //        article.Data = myString;
                //    }
                //}
                articles.Add(article);
            }
            return articles;
        }
    }
}
