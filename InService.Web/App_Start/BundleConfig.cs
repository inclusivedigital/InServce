using System.Web;
using System.Web.Optimization;

namespace InService.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui.js",
                        "~/Scripts/jquery.Jcrop.js",
                         "~/Scripts/knockout-{version}.js",
                         "~/Scripts/umd/popper.js",
                         "~/Scripts/bootstrap.js",
                         "~/Scripts/respond.js",
                          "~/Scripts/select2.js",
                          "~/Scripts/jquery.unobtrusive-ajax.min.js",
                          "~/Scripts/canvasjs.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/bundles/styles").Include(
                      "~/Content/bootstrap.css",
                       "~/Content/PagedList.css",
                      "~/Content/css/materialdesignicons.min.css",
                      "~/Scripts/font-awesome/css/all.min.css",
                      "~/Content/jquery-ui.css", "~/Content/jquery.Jcrop.css",
                      "~/Content/site.css", "~/Content/css/select2.css", "~/Content/select2-bootstrap4.css"
                      ));
        }
    }
}
