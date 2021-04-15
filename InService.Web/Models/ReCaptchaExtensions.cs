using InService.Lib.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace InService.Web.Models
{
    public static class ReCaptchaExtensions
    {
        public static HtmlString RecaptchaScript(this HtmlHelper html, string action)
        {
            return new HtmlString($"<script src='{Lib.API.ReCaptcha.ScriptURL}'></script>" +
                $"<script>" +
                "grecaptcha.ready(function() {" +
                $"grecaptcha.execute('{Lib.API.ReCaptcha.SiteKey}', {{action: '{action}'}}).then(function(token) {{" +
                $"$('#{nameof(ReCaptcha)}').val(token);" +
                "});" +
                "});" +
                "</script>");
        }

        public static HtmlString ReCaptcha(this HtmlHelper html)
            => new HtmlString($"<input type='hidden' name='{nameof(ReCaptcha)}' id='{nameof(ReCaptcha)}' />");

        public static async Task<ReCaptcha.Response> ValidateCaptchaV3(this Controller controller)
        {
            var token = controller.Request.Form[nameof(ReCaptcha)]?.ToString();
            return await new ReCaptcha("6LegPs0ZAAAAAKRIrijST2g_ynusrLydXvscnyo4").VerifyToken(token, controller.Request.UserHostAddress);
        }

        public static async Task<bool?> ValidateCaptchaV2(this Controller controller)
        {
            try
            {
                string userResponse = controller.Request.Params["g-recaptcha-response"];
                return await hbehr.recaptcha.ReCaptcha.ValidateCaptchaAsync(userResponse);
            }
            catch { return null; }
        }
    }
}