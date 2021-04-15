using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Script.Serialization;
using InService.Data;
using InService.Lib;
using InService.Lib.Auth;
using System.Text;
using System.Globalization;

namespace InService.Web.Models
{
    public static class HtmlHelperExtensions
    {
        private static InServiceEntities DB = new InServiceEntities();

        public static HtmlString Action(this UrlHelper helper, User user)
        => new HtmlString($"<a class='text-primary' href='{helper.Details("Users", new { user.ID })}'> {user.Name} </a>");

        public static String TrimEnd(this String str, int count)
        {
            return str.Substring(0, str.Length - count);
        }

        public static string BaseUrl(this HttpRequestBase Request) => $"{Request.Url.Scheme}://{Request.Url.Authority + Request.ApplicationPath.TrimEnd('/')}";

        public static string SyncStatus(this DateTime Value)
        {
            var Elapsed = DateTime.Now.Subtract(Value);
            if (Elapsed.TotalSeconds < 60) return string.Format("{0} second{1} ago", Elapsed.Seconds, Elapsed.Seconds == 1 ? "" : "s");
            if (Elapsed.TotalMinutes < 60) return string.Format("{0} minute{1} ago", Elapsed.Minutes, Elapsed.Minutes == 1 ? "" : "s");
            if (Elapsed.TotalHours < 2) return "about an hour ago";
            if (Elapsed.TotalHours < 5) return string.Format("{0} hours ago", Elapsed.Hours);
            if (Value.Date == DateTime.Now.Date) return string.Format("today at {0}", Value.ToString("HH:mm"));
            if (Value.Date == DateTime.Now.AddDays(-1).Date) return string.Format("yesterday at {0}", Value.ToString("HH:mm"));
            if (Value.Year == DateTime.Now.Year) return string.Format("{0:MMM dd} at {0:HH:mm}", Value);
            return $"{Value:dd MMM yyy} at {Value:HH:mm}";
        }

        public static int MediaType(this Data.Attachment attachment)
        {
            var videos = new List<string>() { ".AVI", ".MKV", ".MOV", ".MP4", ".TS", ".WEBM" };
            var audios = new List<string>() { ".MP3", ".WAV", ".AAC", ".WMV", ".WMA", ".3GP" };
            var images = new List<string>() { ".MPEG", ".JPEG", ".JPG ", ".PNG" };
            var docs = new List<string>() { ".PDF", ".XLSX", ".DOC ", ".DOCX", ".TXT", ".HTML", ".OXPS" };
            if (videos.Any(c => c.ToLower() == attachment.Extension.ToLower())) return (int)MediaFileType.VIDEO;
            else if (audios.Any(c => c.ToLower() == attachment.Extension.ToLower())) return (int)MediaFileType.AUDIO;
            else if (images.Any(c => c.ToLower() == attachment.Extension.ToLower())) return (int)MediaFileType.IMAGE;
            else if (docs.Any(c => c.ToLower() == attachment.Extension.ToLower())) return (int)MediaFileType.DOCUMENT;
            else return 0;
        }
        public static int MediaType(this string Extension)
        {
            var videos = new List<string>() { ".AVI", ".MKV", ".MOV", ".MP4", ".TS", ".WEBM" };
            var audios = new List<string>() { ".MP3", ".WAV", ".AAC", ".WMV", ".WMA", ".3GP" };
            var images = new List<string>() { ".MPEG", ".JPEG", ".JPG ", ".PNG" };
            var docs = new List<string>() { ".PDF", ".XLSX", ".DOC ", ".DOCX", ".TXT", ".HTML", ".OXPS" };
            if (videos.Any(c => c.ToLower() == Extension.ToLower())) return (int)MediaFileType.VIDEO;
            else if (audios.Any(c => c.ToLower() == Extension.ToLower())) return (int)MediaFileType.AUDIO;
            else if (images.Any(c => c.ToLower() == Extension.ToLower())) return (int)MediaFileType.IMAGE;
            else if (docs.Any(c => c.ToLower() == Extension.ToLower())) return (int)MediaFileType.DOCUMENT;
            else return 0;
        }

        public static MvcHtmlString ToHTmlContentString(this string expression) => new MvcHtmlString(expression.ReplaceUrlsWithLinks());

        public static MvcHtmlString ToHTmlContentString<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
            => MvcHtmlString.Create(GetContent(htmlHelper, expression).ReplaceUrlsWithLinks());

        private static string GetContent<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            Func<TModel, TProperty> func = expression.Compile();
            return func(htmlHelper.ViewData.Model).ToString();
        }

        public static string ToJSONString(this object obj)
         => JsonConvert.SerializeObject(obj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore , ReferenceLoopHandling = ReferenceLoopHandling.Ignore , PreserveReferencesHandling = PreserveReferencesHandling.Objects });


        public static MvcHtmlString CustomValidationSummary(this HtmlHelper htmlHelper, bool excludePropertyErrors, string message)
        {
            var formContext = htmlHelper.ViewContext.ClientValidationEnabled ? htmlHelper.ViewContext.FormContext : null;
            if (formContext == null && htmlHelper.ViewData.ModelState.IsValid) return null;

            string messageSpan;
            if (!string.IsNullOrEmpty(message))
            {
                TagBuilder spanTag = new TagBuilder("span");
                spanTag.SetInnerText(message);
                messageSpan = spanTag.ToString(TagRenderMode.Normal) + Environment.NewLine;
            }
            else messageSpan = null;

            var htmlSummary = new StringBuilder();
            TagBuilder unorderedList = new TagBuilder("ul");
            unorderedList.AddCssClass("text-danger");
            IEnumerable<ModelState> modelStates = null;
            if (excludePropertyErrors)
            {
                ModelState ms;
                htmlHelper.ViewData.ModelState.TryGetValue(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix, out ms);
                if (ms != null) modelStates = new ModelState[] { ms };
            }
            else modelStates = htmlHelper.ViewData.ModelState.Values;
            if (modelStates != null)
            {
                foreach (ModelState modelState in modelStates)
                {
                    foreach (ModelError modelError in modelState.Errors)
                    {
                        string errorText = GetUserErrorMessageOrDefault(htmlHelper.ViewContext.HttpContext, modelError, null);
                        if (!String.IsNullOrEmpty(errorText))
                        {
                            TagBuilder listItem = new TagBuilder("li");
                            listItem.InnerHtml = errorText;
                            htmlSummary.AppendLine(listItem.ToString(TagRenderMode.Normal));
                        }
                    }
                }
            }

            if (htmlSummary.Length == 0) htmlSummary.AppendLine(@"<li style=""display:none""></li>");

            unorderedList.InnerHtml = htmlSummary.ToString();
            TagBuilder divBuilder = new TagBuilder("div");
            divBuilder.AddCssClass((htmlHelper.ViewData.ModelState.IsValid) ? HtmlHelper.ValidationSummaryValidCssClassName : HtmlHelper.ValidationSummaryCssClassName);
            divBuilder.InnerHtml = messageSpan + unorderedList.ToString(TagRenderMode.Normal);

            if (formContext != null)
            {
                divBuilder.GenerateId("validationSummary");
                formContext.ValidationSummaryId = divBuilder.Attributes["id"];
                formContext.ReplaceValidationSummary = !excludePropertyErrors;
            }
            return MvcHtmlString.Create(divBuilder.ToString());
        }

        private static string GetUserErrorMessageOrDefault(HttpContextBase httpContext, ModelError error, ModelState modelState)
        {
            if (!String.IsNullOrEmpty(error.ErrorMessage)) return error.ErrorMessage;
            if (modelState == null) return null;
            string attemptedValue = (modelState.Value != null) ? modelState.Value.AttemptedValue : null;
            return String.Format(CultureInfo.CurrentCulture, "The value {0} is invalid.", attemptedValue);
        }




        public static HtmlString ToHTML(this UserStatus status)
        {
            switch (status)
            {
                case UserStatus.BLOCKED:
                    return new HtmlString($"<i class='mdi mdi-account-minus text-danger'></i> {status.ToEnumString()}");
                case UserStatus.ACTIVE:
                    return new HtmlString($"<i class='mdi mdi-account-check text-primary'></i> {status.ToEnumString()}");
                case UserStatus.UNAUTHORIZED:
                    return new HtmlString($"<i class='mdi mdi-account-minus'></i> {status.ToEnumString()}");
                default:
                    break;
            }
            return new HtmlString("");
        }

        public static HtmlString ToAddressHTML(this string address)
        {
            var addressParts = new string[] { address };
            var fullAddress = String.Join(", ", addressParts.Where(c => !String.IsNullOrWhiteSpace(c)).ToArray());
            return new HtmlString($"<address><strong><i class='mdi mdi-map-marker'></i> Address </strong><br />{fullAddress}</address>");
        }

    }
}


