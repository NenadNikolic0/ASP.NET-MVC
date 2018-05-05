using System.Web;
using System.Web.Optimization;

namespace Pdf_project
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            //Add all Javascript files into bundle 
            bundles.Add(new ScriptBundle("~/bundles/JS").Include(
                        "~/Scripts/jQuery.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/custom.js"));

            //Add login CSS file into bundle
            bundles.Add(new StyleBundle("~/Content/login").Include(                      
                      "~/Content/login.css"));

            //Add dashboard CSS file into bundle
            bundles.Add(new StyleBundle("~/Content/dashboard").Include(
                      "~/Content/dashboard.css"));

            //Add bootstrap.min CSS file into bundle
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                      "~/Content/bootstrap.min.css"));

            
        }
    }
}
