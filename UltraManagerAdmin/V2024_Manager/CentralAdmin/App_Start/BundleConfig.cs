using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace CentralAdmin.App_Start
{
   public class BundleConfig
   {
      // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
      public static void RegisterBundles(BundleCollection bundles)
      {
         bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                   "~/Content/AdminLTE/build/js/jquery.min.js",
                    "~/Content/AdminLTE/build/js/General.js",
                    "~/Scripts/toastr.min.js",
                    "~/Scripts/jquery.inputmask.bundle.min.js",
                    "~/Content/AdminLTE/build/js/toastr.min.js",
                    "~/Scripts/sweetalert.js"
       ));

         bundles.Add(new ScriptBundle("~/bundles/graficos").Include(
     "~/Content/AdminLTE/build/js/gauge.min.js"
));

         bundles.Add(new ScriptBundle("~/bundles/ordenados").Include(
                 "~/Content/AdminLTE/build/js/bootstrap.min.js",
                 "~/Content/AdminLTE/build/js/fastclick.js",
                 "~/Content/AdminLTE/build/js/Chart.min.js",
                 "~/Content/AdminLTE/build/js/bootstrap-progressbar.min.js",
                 "~/Content/AdminLTE/build/js/icheck.min.js",
                 "~/Content/AdminLTE/build/js/jquery.flot.js",
                 "~/Content/AdminLTE/build/js/jquery.flot.pie.js",
                 "~/Content/AdminLTE/build/js/jquery.flot.time.js",
                 "~/Content/AdminLTE/build/js/jquery.flot.stack.js",
                 "~/Content/AdminLTE/build/js/jquery.flot.resize.js",
                 "~/Content/AdminLTE/build/js/jquery.flot.orderBars.js",
                 "~/Content/AdminLTE/build/js/jquery.flot.spline.min.js",
                 "~/Content/AdminLTE/build/js/curvedLines.js",
                 "~/Content/AdminLTE/build/js/moment.min.js",
                 "~/Content/AdminLTE/build/js/daterangepicker.js",
                 "~/Content/AdminLTE/build/js/jquery.smartWizard.js",
                 "~/Content/AdminLTE/build/js/select2.full.min.js",
                 "~/Content/AdminLTE/build/js/custom.min.js",
                 "~/Scripts/sweetalert.js",
                 "~/Content/AdminLTE/build/js/jquery.mask.js",
                 "~/Scripts/jquery.validate.js",
                 "~/Content/AdminLTE/build/js/toastr.min.js"
 ));

         bundles.Add(new StyleBundle("~/Content/AdminLTE/css").Include(
                "~/Content/AdminLTE/build/css/bootstrap.min.css",
                "~/fonts/font-awesome.min.css",
                "~/Content/AdminLTE/build/css/normalize.css",
                "~/Content/AdminLTE/build/css/green.css",
                "~/Content/AdminLTE/build/css/daterangepicker.css",
                "~/Content/AdminLTE/build/css/ion.rangeSlider.css",
                "~/Content/AdminLTE/build/css/ion.rangeSlider.skinFlat.css",
                "~/Content/AdminLTE/build/css/bootstrap-colorpicker.min.css",
                "~/Content/AdminLTE/build/css/custom.min.css",
                "~/Content/AdminLTE/build/css/nprogress.css",
                "~/Content/AdminLTE/build/css/toastr.min.css",
                "~/Scripts/sweetalert.css",
                "~/Content/AdminLTE/build/css/select2.min.css"
       ));

         bundles.Add(new StyleBundle("~/Content/AdminLTE/tablacss").Include(
              "~/Content/AdminLTE/build/Tabla/css/font-awesome.min.css",
              "~/Content/AdminLTE/build/Tabla/css/green.css",
              "~/Content/AdminLTE/build/Tabla/css/dataTables.bootstrap.min.css",
              "~/Content/AdminLTE/build/Tabla/css/buttons.bootstrap.min.css",
              "~/Content/AdminLTE/build/Tabla/css/fixedHeader.bootstrap.min.css",
              "~/Content/AdminLTE/build/Tabla/css/responsive.bootstrap.min.css",
              "~/Content/AdminLTE/build/Tabla/css/scroller.bootstrap.min.css",
              "~/Content/AdminLTE/build/css/toastr.min.css"
));

         bundles.Add(new ScriptBundle("~/bundles/tablas").Include(
             "~/Content/AdminLTE/build/Tabla/js/fastclick.js",
             "~/Content/AdminLTE/build/Tabla/js/nprogress.js",
             "~/Content/AdminLTE/build/Tabla/js/jquery.dataTables.min.js",
             "~/Content/AdminLTE/build/Tabla/js/dataTables.bootstrap.min.js",
             "~/Content/AdminLTE/build/Tabla/js/dataTables.buttons.min.js",
             "~/Content/AdminLTE/build/Tabla/js/buttons.bootstrap.min.js",
             "~/Content/AdminLTE/build/Tabla/js/buttons.flash.min.js",
             "~/Content/AdminLTE/build/Tabla/js/buttons.html5.min.js",
             "~/Content/AdminLTE/build/Tabla/js/buttons.print.min.js",
             "~/Content/AdminLTE/build/Tabla/js/dataTables.fixedHeader.min.js",
             "~/Content/AdminLTE/build/Tabla/js/dataTables.keyTable.min.js",
             "~/Content/AdminLTE/build/Tabla/js/dataTables.responsive.min.js",
             "~/Content/AdminLTE/build/Tabla/js/dataTables.scroller.min.js",
             "~/Content/AdminLTE/build/Tabla/js/jszip.min.js",
             "~/Content/AdminLTE/build/Tabla/js/pdfmake.min.js",
             "~/Content/AdminLTE/build/Tabla/js/vfs_fonts.js"
 ));

      }
   }
}