using System.Web;
using System.Web.Optimization;

namespace Pizzaria.Site
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/cldr.js",
                        "~/Scripts/cldr/event.js",
                        "~/Scripts/cldr/supplemental.js",
                        "~/Scripts/cldr/unresolved.js",
                        "~/Scripts/globalize.js",
                        "~/Scripts/globalize/number.js",
                        "~/Scripts/globalize/date.js",
                        "~/Scripts/globalize/plural.js",
                        "~/Scripts/globalize/currency.js",
                        "~/Scripts/globalize.config.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/jquery-ui-{version}.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/layout").Include(
                      "~/Scripts/bootbox.min.js",
                      "~/Scripts/noty/packaged/jquery.noty.packaged.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                      "~/Scripts/common.js"));

            bundles.Add(new ScriptBundle("~/bundles/fileInput").Include(
                      "~/Scripts/bootstrap-file-input.js"));

            bundles.Add(new ScriptBundle("~/bundles/timePicker").Include(
                      "~/Scripts/bootstrap-timepicker.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                      "~/Scripts/DataTables-1.10.4/jquery.dataTables.js",
                      "~/Scripts/DataTables-1.10.4/dataTables.responsive.js",
                      "~/Scripts/DataTables-1.10.4/dataTables.bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
                      "~/Scripts/bootstrap-datepicker.js",
                      "~/Scripts/locales/bootstrap-datepicker.pt-BR.js"));

            bundles.Add(new ScriptBundle("~/bundles/inputmask").Include(
                      "~/Scripts/jquery.inputmask/inputmask.js",
                      "~/Scripts/jquery.inputmask/jquery.inputmask.js",
                      "~/Scripts/jquery.inputmask/jquery.inputmask.extensions.js",
                      "~/Scripts/jquery.inputmask/jquery.inputmask.date.extensions.js",
                      "~/Scripts/jquery.inputmask/jquery.inputmask.numeric.extensions.js",
                      "~/Scripts/jquery.inputmask/jquery.inputmask.phone.extensions.js",
                      "~/Scripts/jquery.inputmask/jquery.inputmask.regex.extensions.js",
                      "~/Scripts/inputmask.config.js"));

            bundles.Add(new ScriptBundle("~/bundles/select2").Include(
                      "~/Scripts/select2.min.js"));

            bundles.Add(new StyleBundle("~/Content/layout").Include(
                      "~/Content/themes/base/all.css",
                      "~/Content/animate.css",
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-select.css",
                      "~/Content/font-awesome.css",
                      "~/Content/jquery.mCustomScrollbar.css",
                      "~/Content/theme-default.css"));

            bundles.Add(new StyleBundle("~/Content/datepicker").Include(
                      "~/Content/datepicker3.css"));

            BundleTable.EnableOptimizations = false;
        }
    }
}
