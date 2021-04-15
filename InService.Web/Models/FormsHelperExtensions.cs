using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using InService.Lib;

namespace InService.Web.Models
{
    public static class FormsHelperExtensions
    {
        public static HtmlString EditorFor(this HtmlHelper Html, IFormField field, IForm form)
        {
            object value = form?.GetValue(field.ID);
            var htmlAttribs = new Dictionary<string, object>() { { "class", "form-control" + (field.DataType.IsFile() ? "-file" : "") + (field.IsOptions ? " Option" : "") + (field.IsDate ? " datepicker" : "") } };
            if (field.IsMandatory) htmlAttribs.Add("required", "");
            if (field.DataType == DataTypes.MULTI_SELECT_OPTIONS) htmlAttribs.Add("multiple", "");
            if (!string.IsNullOrWhiteSpace(field.Hint)) htmlAttribs.Add("placeholder", field.Hint);

            var @readonly = form != null && form.ID > 0 && !field.IsEditable;
            if (@readonly) htmlAttribs.Add(field.IsOptions || field.IsBoolean ? "disabled" : "readonly", "");

            var validationMsg = Html.ValidationMessage(field.FieldName, new { @class = "text-danger" }).ToHtmlString();
            if (field.DataType.IsString())
            {
                if (field.MinValue.HasValue) htmlAttribs.Add("minlength", field.MinValue.Value);
                if (field.MaxValue.HasValue) htmlAttribs.Add("maxlength", field.MaxValue.Value);
            }
            if (field.IsDate)
            {
                if (field.MinValue.HasValue) htmlAttribs.Add("min", new DateTime(field.MinValue.Value).ToString("yyy-MM-dd"));
                if (field.MaxValue.HasValue) htmlAttribs.Add("max", new DateTime(field.MaxValue.Value).ToString("yyy-MM-dd"));
            }
            if (field.DataType == DataTypes.OPTIONS) return new HtmlString(Html.DropDownList(field.FieldName, new SelectList(field.Options, value), "Select..", htmlAttribs).ToHtmlString() + validationMsg);
            if (field.DataType == DataTypes.MULTI_SELECT_OPTIONS)
            {
                string[] values = null;
                try
                {
                    values = JsonConvert.DeserializeObject<string[]>(value?.ToString());
                }
                catch { }
                return new HtmlString(Html.DropDownList(field.FieldName, new MultiSelectList(field.Options, values), "Select..", htmlAttribs).ToHtmlString() + validationMsg);
            }
            else if (field.DataType == DataTypes.MULTI_LINE_TEXT || field.DataType == DataTypes.STREET_ADDRESS) return new HtmlString(Html.TextArea(field.FieldName, (string)value, htmlAttribs).ToHtmlString() + validationMsg);
            else if (field.DataType == DataTypes.BOOLEAN)
            {
                return new HtmlString($"<input name='{field.FieldName}' type='checkbox' value='true' {((bool?)value == true ? "checked" : "")} {(@readonly ? "readonly" : "")} />");
            }
            if (field.DataType.IsNumeric())
            {
                if (field.MinValue.HasValue) htmlAttribs.Add("min", field.MinValue.Value);
                if (field.MaxValue.HasValue) htmlAttribs.Add("max", field.MaxValue.Value);
            }
            htmlAttribs.Add("type", field.DataType.GetInputType());
            if (field.DataType == DataTypes.THUMBNAIL)
            {
                htmlAttribs.Add("accept", "image/*");
                htmlAttribs.Add("capture", "user");
                validationMsg += "<div class='text-info small'><i class='mdi mdi-alert'></i> Recommended size is 256x256. Larger images will be cropped!</div>";
                value = null;
            }
            return new HtmlString(Html.TextBox(field.FieldName, value, htmlAttribs).ToHtmlString() + validationMsg);
        }

        public static HtmlString ValueDisplay(this HtmlHelper htmlHelper, IForm model, IFormField field, object htmlAttributes = null, object fileDownloadRouteValues = null, bool ignoreInexistantFiles = false)
        {
            if (field.DataType.IsFile())
            {
                var Url = new UrlHelper(htmlHelper.ViewContext.RequestContext);
                var path = HttpContext.Current.Server.MapPath(model.GetFilePath(field.ID));
                if (!ignoreInexistantFiles && !System.IO.File.Exists(path)) return new HtmlString("");
                if (field.DataType == DataTypes.THUMBNAIL)
                {
                    var img = new TagBuilder("img");
                    if (htmlAttributes == null)
                    {
                        img.Attributes.Add("style", "max-width:256px");
                        img.AddCssClass("img-thumbnail shadow-sm");
                    }
                    else img.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
                    if (!img.Attributes.ContainsKey("src"))
                        img.Attributes.Add("src", Url.Action("GetImage", htmlHelper.ViewContext.RouteData.Values["controller"].ToString(), fileDownloadRouteValues));
                    return new HtmlString(img.ToString(TagRenderMode.SelfClosing));
                }
                new HtmlString("");
            }
            var value = model.GetValue(field.ID);
            if (field.IsBoolean)
            {
                var boolValue = (bool?)(value) == true;
                return new HtmlString($"<span class='badge-pill badge badge-{(boolValue ? "success" : "danger")}'><i class='mdi mdi-{(boolValue ? "check" : "close")}'></i></span>");
            }
            if (string.IsNullOrWhiteSpace(value?.ToString())) return new HtmlString("");
            if (field.DataType == DataTypes.MULTI_SELECT_OPTIONS)
            {
                try
                {
                    var values = JsonConvert.DeserializeObject<string[]>(value.ToString());
                    var html = "";
                    foreach (var item in values)
                    {
                        html += $"<span class='badge badge-pill badge-secondary mr-1'>{item}</span>";
                    }
                    return new HtmlString(html);
                }
                catch { }
            }
            if (field.DataType.IsURL()) value = $"<a href='{value}' target='_blank'>{value}</a>";
            if (htmlAttributes == null) return new HtmlString(value.ToString());
            var span = new TagBuilder("span");
            span.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            span.InnerHtml = value.ToString();
            return new HtmlString(span.ToString());
        }


        public static HtmlString ValueDisplayFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null, object fileDownloadRouteValues = null) where TModel : IForm where TProperty : IFormField
        {
            var model = htmlHelper.ViewData.Model;
            var field = expression.Compile().Invoke(model);
            return htmlHelper.ValueDisplay(model, field, htmlAttributes, fileDownloadRouteValues);
        }

        public static string GetInputType(this DataTypes dataType)
        {
            switch (dataType)
            {
                case DataTypes.INTEGER:
                    return "number";
                case DataTypes.REAL_NUMBER:
                    return "number";
                case DataTypes.DATE:
                    return "text";
                case DataTypes.TIME:
                    return "time";
                case DataTypes.SINGLE_LINE_TEXT:
                case DataTypes.PHONE_NUMBER:
                case DataTypes.MULTI_LINE_TEXT:
                case DataTypes.NAME:
                    return "text";
                case DataTypes.BOOLEAN:
                    return "checkbox";
                case DataTypes.EMAIL:
                    return "email";
                case DataTypes.URL:
                    return "url";
                case DataTypes.OPTIONS:
                    break;
                case DataTypes.THUMBNAIL:
                    return "file";
                default:
                    break;
            }
            return "text";
        }

        public static void Sync(this IForm form, HttpRequestBase Request)
        {
            foreach (var field in form.Fields.Where(c => !c.DataType.IsFile()))
            {
                var value = Request.Form.GetValues(field.FieldName);
                if (value == null) continue;
                form.SetValue(field, value);
            }
        }

        public static void SyncFiles(this IForm form, HttpRequestBase Request)
        {
            foreach (var field in form.Fields.Where(c => c.DataType.IsFile()))
            {
                var file = Request.Files[field.FieldName];
                if (file == null || file.ContentLength == 0) continue;
                var path = HttpContext.Current.Server.MapPath(form.GetFilePath(field.ID));
                if (field.DataType == DataTypes.THUMBNAIL) ProcessThumbnail(file, path);
                else file.SaveAs(path);
            }
        }

        public static MvcForm BeginMultiPartForm(this HtmlHelper Html)
            => Html.BeginForm(Html.ViewContext.RouteData.Values["action"].ToString(), Html.ViewContext.RouteData.Values["controller"].ToString(), FormMethod.Post, new { enctype = "multipart/form-data" });

        public static void ProcessThumbnail(HttpPostedFileBase file, string destPath)
        {
            try
            {
                using (var imgFactory = new ImageProcessor.ImageFactory())
                {
                    imgFactory.Load(file.InputStream);
                    imgFactory.Format(new ImageProcessor.Imaging.Formats.JpegFormat() { Quality = 100 });
                    if (imgFactory.Image.Size.Width > 256) imgFactory.Resize(new System.Drawing.Size(256, imgFactory.Image.Size.Height * 256 / imgFactory.Image.Width));
                    if (imgFactory.Image.Size.Height > imgFactory.Image.Size.Width) imgFactory.Crop(new System.Drawing.Rectangle(0, 0, imgFactory.Image.Size.Width, imgFactory.Image.Size.Width));
                    else if (imgFactory.Image.Size.Width > imgFactory.Image.Height) imgFactory.Crop(new System.Drawing.Rectangle(0, 0, imgFactory.Image.Size.Height, imgFactory.Image.Size.Height));
                    imgFactory.Resize(new System.Drawing.Size(256, 256));
                    imgFactory.Save(destPath);
                }
            }
            catch { }
        }

        public static bool IsFileSet(this IForm form, int fieldID)
        {
            var path = HttpContext.Current.Server.MapPath(form.GetFilePath(fieldID));
            return System.IO.File.Exists(path);
        }

        private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;
            Type underlyingType = Nullable.GetUnderlyingType(realModelType);
            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }
            return realModelType;
        }

        private static readonly SelectListItem[] SingleEmptyItem = new[] { new SelectListItem { Text = "", Value = "" } };
        public static string GetEnumDescription<TEnum>(TEnum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static MvcHtmlString SelectListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression) => SelectListFor(htmlHelper, expression, null);

        public static MvcHtmlString SelectListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            Type enumType = GetNonNullableModelType(metadata);
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();
            var defaultitem = new SelectListItem { Text = "Select...", Value = "0", Selected = true };
            var items = values.Select(c => new SelectListItem { Text = GetEnumDescription(c).ToEnumString(), Value = c.ToString(), Selected = c.Equals(metadata.Model) });// from value in values
            var list = items.ToList();
            list.Insert(0, defaultitem);
            // If the enum is nullable, add an 'empty' item to the collection
            if (metadata.IsNullableValueType) items = SingleEmptyItem.Concat(list);
            return htmlHelper.DropDownListFor(expression, list, htmlAttributes);
        }
    }
}