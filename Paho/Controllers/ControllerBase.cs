using System;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Paho.Models;
using System.Globalization;
using System.Threading;
using System.Linq;

namespace Paho.Controllers
{
    [Authorize]
    public class ControllerBase : Controller
    {
        private ApplicationUserManager _userManager;
        private PahoDbContext _db;
        protected PahoDbContext db
        {
            get
            {
                if (_db == null)
                {
                    _db = (PahoDbContext)HttpContext.GetOwinContext().Environment["AspNet.Identity.Owin:Paho.Models.PahoDbContext, Paho, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"];
                }
                return _db;
            }
        }

        public ControllerBase()
        {
        }

        public ControllerBase(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            //Localization in Base controller:

            string language = (string)RouteData.Values["language"] ?? "es";
            string culture = (string)RouteData.Values["culture"] ?? "ES";

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(string.Format("{0}-{1}", language, culture));
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(string.Format("{0}-{1}", language, culture));


            return base.BeginExecuteCore(callback, state);
        }

        protected override void OnAuthentication(System.Web.Mvc.Filters.AuthenticationContext filterContext) {
            if (User.Identity.IsAuthenticated) {
                var user = UserManager.FindById(User.Identity.GetUserId());
                var institutionType = user.Institution is Hospital ? InstitutionType.Hospital
                    : (user.Institution is Lab ? InstitutionType.Lab : InstitutionType.Admin);
                //var country_user = ;
                ViewBag.UserInstitutionType = (int) institutionType;
                ViewBag.DateofServer = DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("es-GT"));
                ViewBag.SARI = (user.Institution.SARI == true ) ? true : false ;
                ViewBag.ILI = (user.Institution.ILI == true ) ? true : false;
                ViewBag.InstAccesLevel = (int)user.Institution.AccessLevel;
                ViewBag.unusual = (user.Institution.surv_unusual == true) ? true : false;
                ViewBag.AlertDefinitionBegin = (db.ConfAlertCaseDefinition.Where(y => y.id_country == user.Institution.CountryID).Any()) ? db.ConfAlertCaseDefinition.Where(y => y.id_country == user.Institution.CountryID).FirstOrDefault().month_begin : -1;
                ViewBag.AlertDefinitionEnd = (db.ConfAlertCaseDefinition.Where(y => y.id_country == user.Institution.CountryID).Any()) ? db.ConfAlertCaseDefinition.Where(y => y.id_country == user.Institution.CountryID).FirstOrDefault().month_end : -1;
                ViewBag.UsrCtry = user.Institution.CountryID;
                ViewBag.UsrCtryLang = user.Institution.Country.Language;
                ViewBag.UsrInstID = user.InstitutionID;
                ViewBag.UsrInstNPHL = user.Institution.NPHL;
                ViewBag.NPHL_exist = (db.Institutions.OfType<Lab>()
                                   .Where(x => x.NPHL == true && x.CountryID == user.Institution.CountryID).Count() > 0) ;
                ViewBag.UR = (string) (User.IsInRole("Admin") ? "adm" : User.IsInRole("Modify_Epi") ? "mod_epi" : User.IsInRole("Modify_Lab") ? "mod_lab" : User.IsInRole("Close_case") ? "clo_case" : User.IsInRole("Staff") ? "stf" : User.IsInRole("Report") ? "rpt" :   "");
                ViewBag.CmbService = false;

            }

            base.OnAuthentication(filterContext);
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }
    }
}
