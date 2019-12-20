using Paho.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Paho.Utility;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Paho.Controllers
{
    [Authorize]
    public class SummaryJMController : ControllerBase
    {
        // GET: SummaryJM
        public ActionResult Index()
        {
            var SummaryViewModel = new SummaryJMViewModel();

            IQueryable<Institution> institutions = null;
            //IQueryable<CatAgeGroupJM> CatAgeGroupQuery = null;
            var user = UserManager.FindById(User.Identity.GetUserId());
            var DoS = DateTime.Now.ToString(getMsg("msgDatePickerConfig"));
            var date_format = getMsg("msgDateFormatDP");

            //CaseViewModel.UsrCtry = user.Institution.CountryID;
            SummaryViewModel.DatePickerConfig = DoS;
            SummaryViewModel.DateFormatDP = date_format;
            SummaryViewModel.DisplayTotalGroup = false;                         // Mostrar totales x Pais/Parish ILI Cases-Visit
            //SummaryViewModel.UsrCtry = user.Institution.CountryID;

            if (user.Institution.AccessLevel == AccessLevel.All)
            {
                SummaryViewModel.DisplayCountries = true;
                SummaryViewModel.DisplayHospitals = true;
            }
            else if (user.Institution is Hospital || user.Institution is AdminInstitution)
            {
                SummaryViewModel.DisplayHospitals = true;

                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    institutions = db.Institutions
                                   //.Where(i => i.CountryID == user.Institution.CountryID && i.AccessLevel == AccessLevel.Area).OrderBy(n => n.FullName);
                                   .Where(i => i.CountryID == user.Institution.CountryID && i.ILI == true).OrderBy(n => n.FullName);
                    SummaryViewModel.DisplayTotalGroup = true;
                }
                else if (user.Institution.AccessLevel == AccessLevel.Area)
                {
                    //institutions = db.Institutions
                    //               .Where(i => i.AreaID == user.Institution.AreaID && i.AccessLevel == AccessLevel.Area).OrderBy(n => n.FullName);
                    institutions = db.Institutions
                                   .Where(i => i.AreaID == user.Institution.AreaID && i.ILI == true).OrderBy(n => n.FullName);
                    SummaryViewModel.DisplayTotalGroup = true;
                }
                else
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.ID == user.Institution.ID).OrderBy(n => n.FullName);
                }
            }

            SummaryViewModel.Countries = db.Countries
                    .Select(c => new CountryView()
                    {
                        Id = c.ID.ToString(),
                        Name = c.Name,
                        Active = c.Active
                    }).ToArray();

            if (institutions != null)
            {
                var institutionsDisplay = institutions.Select(i => new LookupView<Institution>()
                {
                    Id = i.ID.ToString(),
                    Name = i.Name
                }).ToList();

                if (institutionsDisplay.Count() > 1)
                {
                    var all = new LookupView<Institution> { Id = "0", Name = getMsg("msgSelectLabel") };
                    institutionsDisplay.Insert(0, all);
                }

                SummaryViewModel.Institutions = institutionsDisplay;
            };

            var AgeGroupDisplay = db.CatAgeGroupJM.Where(y => y.id_country == user.Institution.CountryID).OrderBy(z => z.id_conf_country).ToList();

            SummaryViewModel.CatAgeGroup = AgeGroupDisplay;

            //**** Link Dashboard
            string dashbUrl = "", dashbTitle = "";
            List<CatDashboardLink> lista = (from tg in db.CatDashboarLinks
                                            where tg.id_country == user.Institution.CountryID
                                            select tg).ToList();
            if (lista.Count > 0)
            {
                dashbUrl = lista[0].link;
                dashbTitle = lista[0].title;
            }

            ViewBag.DashbUrl = dashbUrl;
            ViewBag.DashbTitle = dashbTitle;
            //****
            ViewBag.UsrCtry = user.Institution.CountryID;
            //****
            return View(SummaryViewModel);
        }

        public JsonResult GetSummaryForYearJM(int hospitalId, int epiYear)
        {
            int nSemaActual;
            DateTime dSEHoy;
            List<Dictionary<string, int>> summaryPerYear = new List<Dictionary<string, int>>();
            DateTime dFech = PAHOClassUtilities.fechaInicioPrimeraSemanaEpidemiologica(epiYear);
            if (DateTime.Today.Year == epiYear)
            {
                nSemaActual = PAHOClassUtilities.NumeroActualSE();
                dSEHoy = dFech.AddDays((nSemaActual - 1) * 7).AddDays(6);
            }
            else
            {
                nSemaActual = PAHOClassUtilities.semanasAnioEpidemiologico(epiYear);
                dSEHoy = PAHOClassUtilities.fechaInicioPrimeraSemanaEpidemiologica(epiYear);
                dSEHoy = dSEHoy.AddDays(-1);
            }

            //****
            string hospitalIDs = "";
            IQueryable<Institution> institutions = null;
            var user = UserManager.FindById(User.Identity.GetUserId());

            if (user.Institution is Hospital || user.Institution is AdminInstitution)
            {
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    institutions = db.Institutions
                                   .Where(i => i.CountryID == user.Institution.CountryID && i.ILI == true).OrderBy(n => n.FullName);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Area)      // Jamaica: Parish
                {
                    institutions = db.Institutions
                                   .Where(i => i.AreaID == user.Institution.AreaID && i.ILI == true).OrderBy(n => n.FullName);
                }
                else
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.ID == user.Institution.ID).OrderBy(n => n.FullName);
                }

                foreach (var inst in institutions)
                    hospitalIDs += inst.ID.ToString() + ",";
                hospitalIDs = hospitalIDs.Substring(0, hospitalIDs.Length - 1);
            }

            //****
            Dictionary<string, int> aILICaseTG = new Dictionary<string, int>();
            Dictionary<string, int> aILISampTG = new Dictionary<string, int>();
            Dictionary<string, int> aTotVisiTG = new Dictionary<string, int>();

            int countryId = (int)user.Institution.CountryID;
            int weekFrom = 1;
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("Summary_ILI_TotalByEW", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = epiYear;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.NVarChar).Value = hospitalIDs;
                    command.Parameters.Add("@weekFrom", SqlDbType.Int).Value = weekFrom;
                    command.Parameters.Add("@weekTo", SqlDbType.Int).Value = nSemaActual;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string cAnio = reader.GetValue(0).ToString();
                            string cWE = reader.GetValue(1).ToString();

                            aILICaseTG.Add(cAnio + cWE, (int)reader.GetValue(2));
                            aILISampTG.Add(cAnio + cWE, (int)reader.GetValue(3));
                            aTotVisiTG.Add(cAnio + cWE, (int)reader.GetValue(4));
                        }
                    }

                    command.Parameters.Clear();
                    con.Close();
                }
            }

            //****
            for (int i = nSemaActual; i >= 1; i--)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>();
                var ColILICasesST = 0;
                var ColILISamplesTakenST = 0;
                var ColTotalVisitsST = 0;

                DateTime WeekendDate = DateTime.UtcNow;

                var casesummary = db.CaseSummariesJM.FirstOrDefault(s => s.HospitalId == hospitalId && s.EW == i && s.EpiYear == epiYear);
                if (casesummary == null)
                {
                    DateTime vFeIn = dFech.AddDays(7 * (i - 1));
                    DateTime vFeFi = vFeIn.AddDays(6);

                    if (WeekendDate >= vFeIn && WeekendDate <= vFeFi)
                        WeekendDate = vFeFi;
                    else
                    {
                        if (vFeFi < WeekendDate)
                            WeekendDate = dFech.AddDays(7 * (i - 1) + 6);
                        else
                            WeekendDate = dSEHoy;
                    }
                }
                else
                {
                    var casesummaryDetails = casesummary.CaseSummaryDetailJM.ToArray();
                    WeekendDate = casesummary.WeekendDate;
                    foreach (CaseSummaryDetailJM casesummaryDetail in casesummaryDetails)
                    {
                        ColILICasesST += casesummaryDetail.ILICases;
                        ColILISamplesTakenST += casesummaryDetail.ILISamplesTaken;
                        ColTotalVisitsST += casesummaryDetail.TotalVisits;
                    }
                }
                dictionary.Add("EpiWeek", i);
                dictionary.Add("ColILICasesST", ColILICasesST);
                dictionary.Add("ColILISamplesTakenST", ColILISamplesTakenST);
                dictionary.Add("ColTotalVisitsST", ColTotalVisitsST);
                Int32 unixTimestamp = (Int32)(WeekendDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                dictionary.Add("WeekendDate", unixTimestamp);
                dictionary.Add("EpiYear", epiYear);
                //****
                dictionary.Add("ColILICaseGroup", aILICaseTG[epiYear.ToString() + i.ToString()]);
                dictionary.Add("ColILISampGroup", aILISampTG[epiYear.ToString() + i.ToString()]);
                dictionary.Add("ColTotVisiGroup", aTotVisiTG[epiYear.ToString() + i.ToString()]);

                //****
                summaryPerYear.Add(dictionary);
            }

            //System.Diagnostics.Debug.WriteLine("F2->" + DateTime.Now.ToString());     //=> OKOK
            return Json(summaryPerYear, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSummaryDetailsJM(int hospitalId, string hospitalDate, int EpiWeek, int EpiYear)
        {
            var HospitalDate = DateTime.Parse(hospitalDate);
            //var casesummary =  db.CaseSummaries.FirstOrDefault(s=>s.HosiptalId == hospitalId && s.StartDateOfWeek == HospitalDate);
            var casesummary = db.CaseSummariesJM.FirstOrDefault(s => s.HospitalId == hospitalId && s.EpiYear == EpiYear && s.EW == EpiWeek);
            var user = UserManager.FindById(User.Identity.GetUserId());

            if (casesummary == null)
            {
                casesummary = new CaseSummaryJM(){
                    WeekendDate = HospitalDate,
                    HospitalId = hospitalId,
                    EW = EpiWeek,
                    EpiYear = EpiYear,
                    CaseSummaryDetailJM = new List<CaseSummaryDetailJM>()
                };
                db.Entry(casesummary).State = EntityState.Added;
                var AgeGroupbyCountry = db.CatAgeGroupJM.Where(i => i.id_country == user.Institution.CountryID).ToList();
                foreach (CatAgeGroupJM agegroup in AgeGroupbyCountry)
                {
                    casesummary.CaseSummaryDetailJM.Add(
                        new CaseSummaryDetailJM()
                        {
                            AgeGroup = agegroup.id_conf_country,
                            ILICases = 0,
                            ILISamplesTaken = 0,
                            TotalVisits = 0,
                        }
                    );
                }
                db.SaveChanges();
            }
            var casesummaryDetails = casesummary.CaseSummaryDetailJM.ToArray();

            return Json(casesummaryDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSummaryJM(CaseSummaryDetailJM[] casesummaryDetails)
        {
            var casesummaryid = casesummaryDetails[0].CaseSummaryId;
            var casesummary = db.CaseSummariesJM.FirstOrDefault(s => s.Id == casesummaryid);

            foreach (CaseSummaryDetailJM casesummaryDetail in casesummaryDetails)
            {
                var dbcasesummaryDetail = casesummary.CaseSummaryDetailJM.First(d => d.Id == casesummaryDetail.Id);
                
                dbcasesummaryDetail.ILICases = casesummaryDetail.ILICases;
                dbcasesummaryDetail.ILISamplesTaken = casesummaryDetail.ILISamplesTaken;
                dbcasesummaryDetail.TotalVisits = casesummaryDetail.TotalVisits;
            }
            db.SaveChanges();

            return Json(getMsg("viewValidateSavedRecordSummary"));
        }
      
        public string getMsg(string msgView)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            string searchedMsg = msgView;
            int? countryID = user.Institution.CountryID;
            string countryLang = user.Institution.Country.Language;

            ResourcesM myR = new ResourcesM();
            searchedMsg = myR.getMessage(searchedMsg, countryID, countryLang);

            return searchedMsg;
        }
    }
}