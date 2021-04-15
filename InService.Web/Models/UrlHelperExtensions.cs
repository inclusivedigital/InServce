using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace InService.Web.Models
{
    public static class UrlHelperExtensions
    {
        public static RouteValueDictionary GetRouteValues(this RequestContext request)
        {
            var excludeList = new string[] { "action", "controller" };
            var data = new RouteValueDictionary();
            foreach (var item in request.RouteData.Values.Where(c => !excludeList.Contains(c.Key.ToLower())))
                data.Add(item.Key, item.Value);
            return data;
        }

        public static string Edit(this UrlHelper Url, object routeValues = null)
        {
            if (routeValues == null) routeValues = Url.RequestContext.GetRouteValues();
            return Url.Edit(Url.RequestContext.RouteData.Values["controller"].ToString(), routeValues);
        }
        public static string Edit(this UrlHelper Url, string controller, object routeValues = null)
        {
            dynamic RouteValues = null;
            if (routeValues is int ID) RouteValues = new { ID };
            else if (routeValues is Guid UID) RouteValues = new { ID = UID };
            else RouteValues = routeValues;
            return Url.Action(nameof(Edit), controller, RouteValues);
        }

        public static string Details(this UrlHelper Url, object routeValues = null)
        {
            if (routeValues == null) routeValues = Url.RequestContext.GetRouteValues();
            return Url.Details(Url.RequestContext.RouteData.Values["controller"].ToString(), routeValues);
        }
        public static string Details(this UrlHelper Url, string controller, object routeValues = null)
        {
            dynamic RouteValues = null;
            if (routeValues is int ID) RouteValues = new { ID };
            else if (routeValues is Guid UID) RouteValues = new { ID = UID };
            else RouteValues = routeValues;
            return Url.Action(nameof(Details), controller, RouteValues);
        }

        public static string Delete(this UrlHelper Url)
            => Url.Action(nameof(Delete), Url.RequestContext.RouteData.Values["controller"].ToString(), Url.RequestContext.GetRouteValues());


        public static string Add(this UrlHelper Url, object routeValues = null)
            => Url.Add(Url.RequestContext.RouteData.Values["controller"].ToString(), routeValues);
        public static string Add(this UrlHelper Url, string controller, object routeValues = null)
        {
            dynamic RouteValues = null;
            if (routeValues is int ID) RouteValues = new { ID };
            else if (routeValues is Guid UID) RouteValues = new { ID = UID };
            else RouteValues = routeValues;
            return Url.Action(nameof(Add), controller, RouteValues);
        }


        public static string Index(this UrlHelper Url, object routeValues = null)
            => Url.Index(Url.RequestContext.RouteData.Values["controller"].ToString(), routeValues);
        public static string Index(this UrlHelper Url, string controller, object routeValues = null)
        {
            dynamic RouteValues = null;
            if (routeValues is int ID) RouteValues = new { ID };
            else if (routeValues is Guid UID) RouteValues = new { ID = UID };
            else RouteValues = routeValues;
            return Url.Action(nameof(Index), controller, RouteValues);
        }

       
        public static string Query(this UrlHelper Url, object routeValues = null)
        {
            var values = Url.RequestContext.GetRouteValues();
            if (routeValues != null)
            {
                foreach (var item in HtmlHelper.AnonymousObjectToHtmlAttributes(routeValues))
                {
                    values.Add(item.Key, item.Value);
                }
            }
            foreach (var item in Url.RequestContext.HttpContext.Request.QueryString.AllKeys)
            {
                if (!values.ContainsKey(item)) values.Add(item, Url.RequestContext.HttpContext.Request.QueryString[item]); 
            }
            return Url.Action(Url.RequestContext.RouteData.Values["action"].ToString(), Url.RequestContext.RouteData.Values["controller"].ToString(), values);
        }

       
    }
}