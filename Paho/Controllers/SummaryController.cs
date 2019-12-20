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

namespace Paho.Controllers
{
    [Authorize]
    public class SummaryController : ControllerBase
    {
        // GET: Summary  
        public ActionResult Index()
        {
            var SummaryViewModel = new SummaryViewModel();
            IQueryable<Institution> institutions = null;
            //IQueryable<CatAgeGroup> CatAgeGroupQuery = null;
            var user = UserManager.FindById(User.Identity.GetUserId());
            var sari = user.Institution.SARI;
            var ili = user.Institution.ILI;
            //var AccessLevel_InstLogin = user.Institution.AccessLevel;
            //var Name_InstLogin = user.Institution.FullName;
            var DoS = DateTime.Now.ToString(getMsg("msgDatePickerConfig"));
            var date_format = getMsg("msgDateFormatDP");

            //CaseViewModel.UsrCtry = user.Institution.CountryID;
            SummaryViewModel.DatePickerConfig = DoS;
            SummaryViewModel.DateFormatDP = date_format;
            //SummaryViewModel.UsrCtry = user.Institution.CountryID;
            SummaryViewModel.SARI = sari;
            SummaryViewModel.ILI = ili;

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
                    institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID && i.Active == true)
                                  .OrderBy(i => i.FullName);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Area)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.AreaID == user.Institution.AreaID && i.Active == true)
                                   .OrderBy(i => i.FullName);
                }
                else
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => (i.ID == user.Institution.ID || i.Father_ID == user.Institution.ID) && i.Active == true)
                                   .OrderBy(i => i.FullName);
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
                    var all = new LookupView<Institution> { Id = "0", Name = getMsg("msgGeneralSelect") };
                    institutionsDisplay.Insert(0, all);
                }

                SummaryViewModel.Institutions = institutionsDisplay;

                //****
                var instWithServ = db.Institutions.OfType<Hospital>()
                                        .Where(i => i.CountryID == user.Institution.CountryID && i.AccessLevel == AccessLevel.Service)
                                        .Select(c => new
                                        {
                                            Id = c.Father_ID
                                        })
                                        .Distinct()
                                        .ToArray();

                SummaryViewModel.InstitutionsWithServices = db.Institutions.OfType<Hospital>()
                                                                .Where(i => i.CountryID == user.Institution.CountryID && i.AccessLevel == AccessLevel.Service)
                                                                .Select(c => new LookupView<Institution>
                                                                {
                                                                    Id = c.Father_ID.ToString()
                                                                })
                                                                .Distinct()
                                                                .ToArray();
            };

            var AgeGroupDisplay = db.CatAgeGroup.Where(y => y.id_country == user.Institution.CountryID).OrderBy(z => z.id_conf_country).ToList();
            //.Select(x => x.AgeGroup)

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

            return View(SummaryViewModel);
        }

        public JsonResult GetSummaryForYear(int hospitalId, int epiYear)
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

            for (int i = nSemaActual; i >= 1; i--)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>();
                var ColETINumST = 0;
                var ColETIDenoST = 0;
                var ColETINumEmerST = 0;
                var ColHospTST = 0;
                var ColUCITST = 0;
                var ColFalleTST = 0;
                var ColHospSARITST = 0;             //#### CAFQ: 181101
                var ColUCISARITST = 0;              //#### CAFQ: 181101
                var ColFalleSARITST = 0;            //#### CAFQ: 181101
                var ColNeuTST = 0;
                var ColCCSARITST = 0;
                var ColVentTST = 0;
                DateTime StartDateOfWeek = DateTime.UtcNow;

                var casesummary = db.CaseSummaries.FirstOrDefault(s => s.HosiptalId == hospitalId && s.EW == i && s.EpiYear == epiYear);
                if (casesummary == null)
                {
                    DateTime vFeIn = dFech.AddDays(7 * (i - 1));
                    DateTime vFeFi = vFeIn.AddDays(6);

                    if (StartDateOfWeek >= vFeIn && StartDateOfWeek <= vFeFi)
                        StartDateOfWeek = vFeFi;
                    else
                    {
                        if (vFeFi < StartDateOfWeek)
                            StartDateOfWeek = dFech.AddDays(7 * (i - 1) + 6);
                        else
                            StartDateOfWeek = dSEHoy;
                    }
                }
                else
                {
                    var casesummaryDetails = casesummary.CaseSummaryDetails.ToArray();
                    StartDateOfWeek = casesummary.StartDateOfWeek;
                    foreach (CaseSummaryDetail casesummaryDetail in casesummaryDetails)
                    {
                        ColETINumST += casesummaryDetail.ETINumST;
                        ColETIDenoST += casesummaryDetail.ETIDenoST;
                        ColETINumEmerST += casesummaryDetail.ETINumEmerST;
                        ColHospTST += casesummaryDetail.HospST;
                        ColUCITST += casesummaryDetail.UCIST;
                        ColFalleTST += casesummaryDetail.DefST;
                        ColHospSARITST += casesummaryDetail.HospSARIST;             //#### CAFQ: 181101
                        ColUCISARITST += casesummaryDetail.UCISARIST;               //#### CAFQ: 181101
                        ColFalleSARITST += casesummaryDetail.DefSARIST;             //#### CAFQ: 181101
                        ColNeuTST += casesummaryDetail.NeuST.HasValue ? casesummaryDetail.NeuST.Value : 0;
                        //ColCCSARITST += casesummaryDetail.CCSARIST.HasValue ? casesummaryDetail.CCSARIST.Value : 0;
                        ColCCSARITST += casesummaryDetail.CCSARIST.Value;
                        ColVentTST += casesummaryDetail.VentST.HasValue ? casesummaryDetail.VentST.Value : 0;
                    }
                }
                dictionary.Add("EpiWeek", i);
                dictionary.Add("ColHospTST", ColHospTST);
                dictionary.Add("ColUCITST", ColUCITST);
                dictionary.Add("ColFalleTST", ColFalleTST);
                dictionary.Add("ColHospSARITST", ColHospSARITST);           //#### CAFQ: 181101
                dictionary.Add("ColUCISARITST", ColUCISARITST);             //#### CAFQ: 181101
                dictionary.Add("ColFalleSARITST", ColFalleSARITST);         //#### CAFQ: 181101
                dictionary.Add("ColETINumST", ColETINumST);
                dictionary.Add("ColETIDenoST", ColETIDenoST);
                dictionary.Add("ColETINumEmerST", ColETINumEmerST);
                dictionary.Add("ColNeuTST", ColNeuTST);
                dictionary.Add("ColCCSARITST", ColCCSARITST);
                dictionary.Add("ColVentTST", ColVentTST);
                Int32 unixTimestamp = (Int32)(StartDateOfWeek.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                dictionary.Add("StartDateOfWeek", unixTimestamp);
                dictionary.Add("EpiYear", epiYear);
                //dictionary.Add("HospitalId", hospitalId);

                summaryPerYear.Add(dictionary);
            }

            return Json(summaryPerYear, JsonRequestBehavior.AllowGet);
        }

        // GET: GetSummaryDetails
        public JsonResult GetSummaryDetails(int hospitalId, string hospitalDate, int EpiWeek, int EpiYear)
        {
            var HospitalDate = DateTime.Parse(hospitalDate);
            //var casesummary =  db.CaseSummaries.FirstOrDefault(s=>s.HosiptalId == hospitalId && s.StartDateOfWeek == HospitalDate);
            var casesummary = db.CaseSummaries.FirstOrDefault(s => s.HosiptalId == hospitalId && s.EpiYear == EpiYear && s.EW == EpiWeek);
            var user = UserManager.FindById(User.Identity.GetUserId());

            if (casesummary == null)
            {
                casesummary  =  new CaseSummary(){
                    StartDateOfWeek = HospitalDate,
                    HosiptalId = hospitalId,
                    EW = EpiWeek,
                    EpiYear = EpiYear,
                    CaseSummaryDetails = new List<CaseSummaryDetail>()
                };
                db.Entry(casesummary).State = EntityState.Added;
                var AgeGroupbyCountry = db.CatAgeGroup.Where(i => i.id_country == user.Institution.CountryID).OrderBy(z => z.id_conf_country).ToList();
                foreach  (CatAgeGroup agegroup in AgeGroupbyCountry )
                {
                    casesummary.CaseSummaryDetails.Add(
                        new  CaseSummaryDetail()
                        {
                            AgeGroup = agegroup.id_conf_country,
                            ETINumFem = 0,
                            ETINumMaso = 0,
                            ETINumST = 0,
                            ETINumEmerFem = 0,
                            ETINumEmerMaso = 0,
                            ETINumEmerST = 0,
                            ETIDenoFem = 0,
                            ETIDenoMaso = 0,
                            ETIDenoST = 0,
                            HospFem = 0,
                            HospMaso = 0,
                            HospST = 0,
                            HospSARIFem = 0,           //#### CAFQ: 181101
                            HospSARIMaso = 0,
                            HospSARIST = 0,
                            UCIFem  = 0,
                            UCIMaso = 0,
                            UCIST = 0 ,
                            UCISARIFem = 0,             //#### CAFQ: 181101
                            UCISARIMaso = 0,
                            UCISARIST = 0,
                            DefFem = 0,
                            DefMaso = 0,
                            DefST = 0 ,
                            DefSARIFem = 0,             //#### CAFQ: 181101
                            DefSARIMaso = 0,
                            DefSARIST = 0,
                            NeuFem = 0 ,
                            NeuMaso = 0,
                            NeuST = 0,
                            CCSARIFem = 0,
                            CCSARIMaso = 0,
                            CCSARIST = 0,
                            VentFem = 0,
                            VentMaso = 0,
                            VentST = 0
                        }            
                    );
                }
                db.SaveChanges();
            }
            var casesummaryDetails = casesummary.CaseSummaryDetails.OrderBy(z => z.AgeGroup).ToArray();


            //     (
            //      from casesummary in db.CaseSummaries as IEnumerable<CaseSummary>
            //      where casesummary
            //      select casesummary.CaseSummaryDetails
            //      );

            //      CaseSummaryDetails = db.FluCases.GroupBy(o=>o.AgeGroup).Select( f => new {
                        
            //      AgeGroup = f.Key,
            //      Hospfem = f.Sum(i => i.Gender == Gender.Female && i.CaseHospital.HospAmDate.HasValue ? 1 : 0),
            //      HospMaso = f.Sum(i => i.Gender == Gender.Male && i.CaseHospital.HospAmDate.HasValue ? 1 : 0),
            //      ICUfem = f.Sum(i => i.Gender == Gender.Female &&  i.CaseHospital.ICUAmDate.HasValue ? 1 : 0),
            //      ICUMaso = f.Sum(i => i.Gender == Gender.Male &&  i.CaseHospital.ICUAmDate.HasValue ? 1 : 0),
            //      Deffem = f.Sum(i => i.Gender == Gender.Female && i.CaseHospital.Destin.Equals("D") ? 1 : 0),
            //      DefMaso = f.Sum(i => i.Gender == Gender.Male && i.CaseHospital.Destin.Equals("D") ? 1 : 0)              
            //      }               
            return Json(casesummaryDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSummary(CaseSummaryDetail[] casesummaryDetails)
        {
            var casesummaryid = casesummaryDetails[0].CaseSummaryId;
            var casesummary = db.CaseSummaries.FirstOrDefault(s => s.Id == casesummaryid);
           
            foreach (CaseSummaryDetail casesummaryDetail in casesummaryDetails)
            {
                var dbcasesummaryDetail = casesummary.CaseSummaryDetails.First(d => d.Id == casesummaryDetail.Id);
                dbcasesummaryDetail.ETINumFem = casesummaryDetail.ETINumFem;
                        System.Diagnostics.Debug.WriteLine(casesummaryDetail.ETINumFem);     //=> OKOK
                dbcasesummaryDetail.ETINumMaso = casesummaryDetail.ETINumMaso;
                        System.Diagnostics.Debug.WriteLine(casesummaryDetail.ETINumMaso);     //=> OKOK
                dbcasesummaryDetail.ETINumST = casesummaryDetail.ETINumST;
                        System.Diagnostics.Debug.WriteLine(casesummaryDetail.ETINumST);     //=> OKOK

                dbcasesummaryDetail.ETINumEmerFem = casesummaryDetail.ETINumEmerFem;
                dbcasesummaryDetail.ETINumEmerMaso = casesummaryDetail.ETINumEmerMaso;
                dbcasesummaryDetail.ETINumEmerST = casesummaryDetail.ETINumEmerST;

                dbcasesummaryDetail.ETIDenoFem = casesummaryDetail.ETIDenoFem;
                        System.Diagnostics.Debug.WriteLine(casesummaryDetail.ETIDenoFem);     //=> OKOK
                dbcasesummaryDetail.ETIDenoMaso = casesummaryDetail.ETIDenoMaso;
                        System.Diagnostics.Debug.WriteLine(casesummaryDetail.ETIDenoMaso);     //=> OKOK
                dbcasesummaryDetail.ETIDenoST = casesummaryDetail.ETIDenoST;
                        System.Diagnostics.Debug.WriteLine(casesummaryDetail.ETIDenoST);     //=> OKOK

                dbcasesummaryDetail.HospFem = casesummaryDetail.HospFem;
                dbcasesummaryDetail.HospMaso = casesummaryDetail.HospMaso;
                dbcasesummaryDetail.HospST = casesummaryDetail.HospST;
                dbcasesummaryDetail.HospSARIFem = casesummaryDetail.HospSARIFem;        //#### CAFQ: 181101
                dbcasesummaryDetail.HospSARIMaso = casesummaryDetail.HospSARIMaso;
                dbcasesummaryDetail.HospSARIST = casesummaryDetail.HospSARIST;
                dbcasesummaryDetail.UCIFem = casesummaryDetail.UCIFem;
                dbcasesummaryDetail.UCIMaso = casesummaryDetail.UCIMaso;
                dbcasesummaryDetail.UCIST = casesummaryDetail.UCIST;
                dbcasesummaryDetail.UCISARIFem = casesummaryDetail.UCISARIFem;          //#### CAFQ: 181101
                dbcasesummaryDetail.UCISARIMaso = casesummaryDetail.UCISARIMaso;
                dbcasesummaryDetail.UCISARIST = casesummaryDetail.UCISARIST;
                dbcasesummaryDetail.DefFem = casesummaryDetail.DefFem;
                dbcasesummaryDetail.DefMaso = casesummaryDetail.DefMaso;
                dbcasesummaryDetail.DefST = casesummaryDetail.DefST;
                dbcasesummaryDetail.DefSARIFem = casesummaryDetail.DefSARIFem;          //#### CAFQ: 181101
                dbcasesummaryDetail.DefSARIMaso = casesummaryDetail.DefSARIMaso;
                dbcasesummaryDetail.DefSARIST = casesummaryDetail.DefSARIST;
                dbcasesummaryDetail.NeuFem = casesummaryDetail.NeuFem;
                dbcasesummaryDetail.NeuMaso = casesummaryDetail.NeuMaso;
                dbcasesummaryDetail.NeuST = casesummaryDetail.NeuST;
                dbcasesummaryDetail.CCSARIFem = casesummaryDetail.CCSARIFem;
                dbcasesummaryDetail.CCSARIMaso = casesummaryDetail.CCSARIMaso;
                dbcasesummaryDetail.CCSARIST = casesummaryDetail.CCSARIST;
                dbcasesummaryDetail.VentFem = casesummaryDetail.VentFem;
                dbcasesummaryDetail.VentMaso = casesummaryDetail.VentMaso;
                dbcasesummaryDetail.VentST = casesummaryDetail.VentST;
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
            searchedMsg = myR.getMessage(searchedMsg,countryID,countryLang);
            //searchedMsg = myR.getMessage(searchedMsg, 0, "ENG");
            return searchedMsg;
        }
    }
}
