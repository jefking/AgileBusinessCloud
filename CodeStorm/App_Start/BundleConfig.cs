// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='BundleConfig.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Code
{
    using System.Web;
    using System.Web.Optimization;

    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // JavaScript
            bundles.Add(new ScriptBundle("~/bundles/CodeStormPlugins").Include(
                        "~/Scripts/codestorm/Plugins/Tweet.js",
                        "~/Scripts/codestorm/Plugins/Cycle.js",
                        "~/Scripts/codestorm/Plugins/MD5.js",
                        "~/Scripts/codestorm/Membership/Authentication.js"));

            // CSS
            bundles.Add(new StyleBundle("~/Content/css/CodeStorm").Include(
                    "~/Content/css/bootstrap.css",
                //"~/Content/stylesheets/skeleton.css",
                //"~/Content/stylesheets/layout.css",
                //"~/Content/stylesheets/themes/light-blue.css",
                    "~/Content/css/styles.css",
                    "~/Content/css/m-styles.min.css",
                    "~/Content/stylesheets/codestorm.css"));
        }
    }
}