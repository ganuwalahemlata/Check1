using System.Web;
using System.Web.Optimization;

namespace KontinuityCRM
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                            "~/Scripts/bootstrap.min.js"
                            ));
            
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                            "~/Scripts/jquery-{version}.js"));
                            

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        //"~/Scripts/jquery.unobtrusive-ajax.min.js",
                        //"~/Scripts/jquery.validate.min.js",
                        //"~/Scripts/jquery.validate.unobtrusive.min.js"
                          "~/Scripts/jquery.validate*"
                          , "~/Scripts/kontinuitycrm.unobtrusive.js"
                          , "~/Scripts/jquery.maskedinput.js"
            ));


            bundles.Add(new ScriptBundle("~/bundles/unicorn").Include(

                          
                            // used by forms
                            "~/Scripts/bootstrap-colorpicker.js", // need for th datepicker don´t ask me why
                            //"~/Scripts/bootstrap-datepicker.js", // only forms
                            "~/Scripts/jquery.icheck.min.js",  // used also by table

                           "~/Scripts/select2.min.js",         // used also by table
                             //"~/Scripts/select21.js",         // used also by table

                            "~/Scripts/jquery.nicescroll.min.js", // used also by table
                            
                            "~/Scripts/unicorn.js", // both
                            "~/Scripts/unicorn.form_common.js", // only by forms

                            "~/Scripts/jquery.dataTables.min.js", // used by table
                            
                            "~/Scripts/unicorn.tables.js" // used by table
                            
                            //"~/Scripts/excanvas.min.js",
                            //"~/Scripts/jquery.sparkline.min.js",
                            //"~/Scripts/fullcalendar.min.js",
                            
                            ));

            bundles.Add(new ScriptBundle("~/bundles/kontinuitycrm").Include(
                            "~/Scripts/kontinuitycrm.js"));




            // Bundles Styles
            bundles.Add(new StyleBundle("~/Content/bootstrapcss").Include(
                            "~/Content/bootstrap.min.css",
                            "~/Content/font-awesome.min.css",
                            
                            "~/Content/jquery.ui.css",
                            //"~/Content/datepicker.css",
                            "~/Content/bootstrap-datepicker.min.css",
                            "~/Content/fullcalendar.css",
                            "~/Content/jquery.jscrollpane.css",
                            "~/Content/icheck/flat/blue.css"
                            ,"~/Content/select2.css"
                           
                            ));

            bundles.Add(new ScriptBundle("~/Content/unicorncss").Include(
                            "~/Content/unicorn.css"
                            ));

            bundles.Add(new StyleBundle("~/Content/kontinuitycrmcss").Include(
                            "~/Content/kontinuitycrm.css"
                            ));
        }
    }
}