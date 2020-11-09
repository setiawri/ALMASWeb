using System.Web;
using System.Web.Optimization;

namespace ALMASWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //causes errors in console

            //bundles.Add(new StyleBundle("~/bundles/globalStylesheets").IncludeDirectory("~/assets/css/", "*.css", true));

            //bundles.Add(new ScriptBundle("~/bundles/coreJSfiles").Include(
            //        "~/assets/js/main/jquery.min.js",
            //        "~/assets/js/main/bootstrap.bundle.min.js",
            //        "~/assets/js/plugins/loaders/blockui.min.js"
            //    ));
            ////doesn't work
            ////bundles.Add(new ScriptBundle("~/bundles/assets/js/main").IncludeDirectory("~/assets/js/main/", "*.js", true));

            //bundles.Add(new ScriptBundle("~/bundles/themeJSfiles").Include(
            //        "~/assets/js/plugins/ui/moment/moment.min.js",
            //        "~/assets/js/plugins/pickers/anytime.min.js",
            //        "~/assets/js/plugins/pickers/daterangepicker.js",
            //        "~/assets/js/plugins/pickers/pickadate/picker.js",
            //        "~/assets/js/plugins/pickers/pickadate/picker.date.js",
            //        "~/assets/js/plugins/pickers/pickadate/picker.time.js",
            //        "~/assets/js/plugins/forms/selects/select2.min.js",
            //        "~/assets/js/plugins/forms/styling/uniform.min.js",
            //        "~/assets/js/plugins/forms/styling/switchery.min.js",
            //        "~/assets/js/plugins/forms/styling/switch.min.js",
            //        "~/assets/js/plugins/tables/datatables/datatables.min.js",
            //        "~/assets/js/plugins/notifications/bootbox.min.js",
            //        "~/assets/js/plugins/forms/number/jquery.number.min.js",
            //        "~/assets/js/app.js"
            //    ));

            //BundleTable.EnableOptimizations = true;
        }
    }
}
