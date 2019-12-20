using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;

namespace Paho
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui-1.10.4.js",
                "~/Scripts/moment.js",
                "~/Scripts/moment-timezone-with-data.js"
                ));

            //bundles.Add(new ScriptBundle("~/bundles/recaptcha").Include(
            //  "https://www.google.com/recaptcha/api.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                "~/Scripts/knockout-{version}.js",
                "~/Scripts/knockout.validation.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/sammy-{version}.js",
                "~/Scripts/date.js",
                "~/Scripts/app/kotoolsfordate.js",
                "~/Scripts/app/common.js",
                "~/Scripts/app/app.datamodel.js",
                "~/Scripts/app/app.viewmodel.js",
                "~/Scripts/app/home.viewmodel.js",
                "~/Scripts/app/contact.viewmodel.js",
                "~/Scripts/app/GEO.viewmodel.js",
                "~/Scripts/app/hospital.viewmodel.js",
                "~/Scripts/app/lab.viewmodel.js",
                "~/Scripts/app/risk.viewmodel.js",
                "~/Scripts/app/printtest.viewmodel.js",
                "~/Scripts/app/_run.js"));

            bundles.Add(new ScriptBundle("~/bundles/catalog-population").Include(
                "~/Scripts/app/catalog.population.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/catalog-institution").Include(
                "~/Scripts/app/catalog.institution.js",
                "~/Scripts/app/common.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/catalog-institution-conf").Include(
                "~/Scripts/app/catalog.institution-conf.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/usersadmin").Include(
                "~/Scripts/app/usersAdmin.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/summary").Include(
                "~/Scripts/app/kotoolsfordate.js",
                "~/Scripts/sammy-{version}.js",
                "~/Scripts/date.js",
                "~/Scripts/app/common.js",
                "~/Scripts/app/app.datamodel.js",
                "~/Scripts/app/app.viewmodel.js",
                "~/Scripts/app/summary.viewmodel.js",
                "~/Scripts/app/_run.js"));

            /*bundles.Add(new ScriptBundle("~/bundles/summaryJM").Include(
                "~/Scripts/app/kotoolsfordate.js",
                "~/Scripts/sammy-{version}.js",
                "~/Scripts/date.js",
                "~/Scripts/app/common.js",
                "~/Scripts/app/app.datamodel.js",
                "~/Scripts/app/app.viewmodel.js",
                "~/Scripts/app/summaryJM.viewmodel.js",
                "~/Scripts/app/_run.js"));*/

            bundles.Add(new ScriptBundle("~/bundles/ticket").Include(
                "~/Scripts/app/kotoolsfordate.js",
                "~/Scripts/sammy-{version}.js",
                "~/Scripts/date.js",
                "~/Scripts/app/common.js",
                "~/Scripts/app/app.datamodel.js",
                "~/Scripts/app/app.viewmodel.js",
                "~/Scripts/app/ticket.viewmodel.js",
                "~/Scripts/app/_run.js"));

            bundles.Add(new ScriptBundle("~/bundles/exportar").Include(
                "~/Scripts/app/kotoolsfordate.js",
                "~/Scripts/sammy-{version}.js",
                "~/Scripts/date.js",
                "~/Scripts/app/common.js",
                "~/Scripts/app/app.datamodel.js",
                "~/Scripts/app/app.viewmodel.js",
                "~/Scripts/app/exportar.viewmodel.js",
                "~/Scripts/app/_run.js"));

            bundles.Add(new ScriptBundle("~/bundles/fluid").Include(
              "~/Scripts/app/kotoolsfordate.js",
              "~/Scripts/sammy-{version}.js",
              "~/Scripts/app/common.js",
              "~/Scripts/app/app.datamodel.js",
              "~/Scripts/app/app.viewmodel.js",
              "~/Scripts/app/fluid.viewmodel.js",
              "~/Scripts/app/_run.js"));

            bundles.Add(new ScriptBundle("~/bundles/principal").Include(
              "~/Scripts/app/kotoolsfordate.js",
              "~/Scripts/sammy-{version}.js",
              "~/Scripts/app/common.js",
              "~/Scripts/app/app.datamodel.js",
              "~/Scripts/app/app.viewmodel.js",
              "~/Scripts/app/principal.viewmodel.js",
              "~/Scripts/app/_run.js"));

            //bundles.Add(new ScriptBundle("~/bundles/d3library").Include(
            //  "~/Scripts/d3.js"));
            bundles.Add(new ScriptBundle("~/bundles/pdfviewer").Include(
            "~/Scripts/pdf.js",
            "~/Scripts/pdf.worker.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/d3library").Include(
            "~/Scripts/d3.js"));

            bundles.Add(new ScriptBundle("~/bundles/amcharts").Include(
            "~/Scripts/amcharts/amcharts.js",
            "~/Scripts/amcharts/serial.js",
            "~/Scripts/amcharts/themes/light.js",
            "~/Scripts/amcharts/plugins/export/export.min.js",
            "~/Scripts/amcharts/plugins/export/export.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/report").Include(
              "~/Scripts/koGrid-2.1.1.js",
              "~/Scripts/app/kotoolsfordate.js",
              "~/Scripts/sammy-{version}.js",
              "~/Scripts/app/common.js",
              "~/Scripts/app/app.datamodel.js",
              "~/Scripts/app/app.viewmodel.js",
              "~/Scripts/app/report.viewmodel.js",
              "~/Scripts/app/_run.js"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/bootstrap-multiselect.js",
                "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                 "~/Content/bootstrap.css",
                 "~/Content/bootstrap-multiselect.css",
                 "~/Content/Site.css",
                 "~/Content/PDFViewer.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}
