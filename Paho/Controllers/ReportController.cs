using Paho.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Paho.Controllers
{
    [Authorize]
    public class ReportController : ControllerBase
    {  
        // GET:Exportar  
        public ActionResult Index()
        {
            var ReportViewModel = new ReportViewModel();
            ReportViewModel.Countries = db.Countries.Where(c=>c.Active == true)
                    .Select(c => new CountryView()
                    {
                        Id = c.ID.ToString(),
                        Name = c.Name     
                    }).ToArray();
            IQueryable<Institution> institutions = null;
            var user = UserManager.FindById(User.Identity.GetUserId());
            ReportViewModel.CountryID = user.Institution.CountryID ?? 0;
            if (user.Institution.AccessLevel == AccessLevel.All)
            {
                ReportViewModel.DisplayCountries = true;
                ReportViewModel.DisplayHospitals = true;
            }
            else if (user.Institution is Hospital || user.Institution is AdminInstitution)
            {
                ReportViewModel.DisplayHospitals = true;

                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID).OrderBy(n => n.FullName);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Area)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.AreaID == user.Institution.AreaID).OrderBy(n => n.FullName);
                }
                else
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.ID == user.Institution.ID).OrderBy(n => n.FullName);
                }
            }
            else
            {
                ReportViewModel.DisplayLabs = true;

                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    institutions = db.Institutions.OfType<Lab>()
                                   .Where(i => i.CountryID == user.Institution.CountryID);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Area)
                {
                    institutions = db.Institutions.OfType<Lab>()
                                   .Where(i => i.AreaID == user.Institution.AreaID);
                }
                else
                {
                    institutions = db.Institutions.OfType<Lab>()
                                   .Where(i => i.ID == user.Institution.ID);
                }
            }

            if (institutions != null)
            {
                ReportViewModel.hospitals = institutions.OfType<Hospital>().Select(i => new LookupView<Hospital>()
                {
                    Id = i.ID.ToString(),
                    Name = i.Name
                }).ToArray();      

                ReportViewModel.labs = (from institution in db.Institutions.OfType<Lab>()
                                  select new LookupView<Lab>()
                                  {
                                      Id = institution.ID.ToString(),
                                      Name = institution.Name
                                  }).ToArray();
            };
            return View(ReportViewModel);
        }

        // POST: GetCasesInExcel
        [HttpPost]
        public JsonResult GetFluCases(int CountryID,
                                      string Name,
                                      String NationalID,
                                      string  CaseStatus,
                                      long? HospitalID,                              
                                      DateTime? RStartDate,
                                      DateTime? REndDate,
                                      DateTime? HStartDate,
                                      DateTime? HEndDate
            )
        {
            IQueryable<FluCaseReportViewModel> query = GetQuery(
                CountryID,
                Name,
                NationalID,
                CaseStatus,
                HospitalID,
                RStartDate,
                REndDate,
                HStartDate,
                HEndDate
                );
            FluCaseReportViewModel[] result = query.Select(f => f).ToArray() ;
             //We return the XML from the memory as a .xls file
            //var result = db.Database.SqlQuery<Example>("SELECT TOP 10 [ID] as ColumnThree ,[HospitalDate] as ColumnFour ,[FName1] as ColumnOne ,[FName2] as ColumTwo FROM[PahoFlu].[dbo].[FluCase]", null).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private IQueryable<FluCaseReportViewModel> GetQuery(int CountryID,
                                      string Name,
                                      String NationalID,
                                      string  CaseStatus,
                                      long? HospitalID,                                  
                                      DateTime? RStartDate,
                                      DateTime? REndDate,
                                      DateTime? HStartDate,
                                      DateTime? HEndDate
            )
        {
            string countryCode = db.Countries.Find(CountryID).Code;
            IQueryable<FluCaseReportViewModel> query = db.FluCases.Select(f => new FluCaseReportViewModel()
            {
                CountryCode = countryCode,
                ID = f.ID,
                HospitalID = f.HospitalID,
                HospitalDate = f.HospitalDate,
                RegDate = f.RegDate,
                Name = string.Concat(f.FName1, string.IsNullOrEmpty(f.FName2) ? " " : string.Concat(" ", f.FName2), f.LName1, string.IsNullOrEmpty(f.LName2) ? "" : string.Concat(" ", f.LName2)),
                NationalID = f.NationalId,
                CaseStatus = ""
            });
           
            // assuming that you return all records when nothing is specified in the filter
            if (HospitalID.HasValue)
                query = query.Where(t =>
                    t.HospitalID == HospitalID.Value);
            else 
                   query = query.Where(t =>
                   t.CountryCode == countryCode);

            if (!string.IsNullOrEmpty(Name))
                query = query.Where(t =>
                    t.Name.Contains(Name));

            if (!string.IsNullOrEmpty(NationalID))
                query = query.Where(t =>
                    t.NationalID.Contains(NationalID));
            
            if (HStartDate.HasValue)
                query = query.Where(t =>
                    t.HospitalDate >= HStartDate.Value);
            
            if (HEndDate.HasValue)
                query = query.Where(t =>
                    t.HospitalDate <= HEndDate.Value);

            if (RStartDate.HasValue)
                query = query.Where(t =>
                    t.RegDate >= RStartDate.Value);

            if (REndDate.HasValue)
                query = query.Where(t =>
                    t.RegDate <= REndDate.Value);
       
            return query;
        }

        //private int GetWeek(DateTime date)
        //{
        //    DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;            
        //    Calendar cal = dfi.Calendar;
        //    return cal.GetWeekOfYear(date, dfi.CalendarWeekRule,
        //                                   dfi.FirstDayOfWeek);
        //}

    }
}
