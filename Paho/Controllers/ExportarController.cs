using Paho.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using Paho.Reports.Entities;
using System.Drawing;
using System.Net;
using System.Data.Entity;
using System.Globalization;
using System.Collections;
using System.Web.Script.Serialization;
using Paho.Utility;
using System.Xml;

namespace Paho.Controllers
{
    [Authorize(Roles = "Admin, Report")]
    public class ExportarController : ControllerBase
    {
        // GET:Exportar
        public ActionResult Index()
        {
            var ExportarViewModel = new ExportarViewModel();
            IQueryable<Institution> institutions = null;
            IQueryable<Paho.Models.Region> regions = null;
            //IQueryable<Paho.Models.Area> areas = null;
            IQueryable<Area> areas = null;                          //#### CAFQ: 180703 
            IQueryable<ReportCountry> ReportsCountries = null;

            var user = UserManager.FindById(User.Identity.GetUserId());
            ExportarViewModel.CountryID = user.Institution.CountryID ?? 0;
            ReportsCountries = db.ReportsCountries.Where(i => i.CountryID == user.Institution.CountryID && i.active == true);

            if (user.Institution.AccessLevel == AccessLevel.All)
            {
                ExportarViewModel.DisplayCountries = true;
                ExportarViewModel.DisplayAreas = true;              //#### CAFQ: 180703   
                ExportarViewModel.DisplayRegionals = true;
                ExportarViewModel.DisplayHospitals = true;
            }
            else if (user.Institution is Hospital || user.Institution is AdminInstitution)
            {
                ExportarViewModel.DisplayHospitals = true;

                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID).OrderBy(n => n.FullName);

                    if (user.type_region == null)
                    {     // Regiones
                        regions = db.Regions.Where(c => c.CountryID == user.Institution.CountryID && c.tipo_region == 1).OrderBy(i => i.Name);
                    }
                    else
                    {
                        // Regiones
                        regions = db.Regions.Where(c => c.CountryID == user.Institution.CountryID && c.tipo_region == user.type_region).OrderBy(i => i.Name);
                    }

                    areas = db.Areas.Where(d => d.CountryID == user.Institution.CountryID).OrderBy(i => i.Name);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Area)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.AreaID == user.Institution.AreaID).OrderBy(n => n.FullName);
                }
                else if (user.Institution is Hospital && user.Institution.AccessLevel != AccessLevel.Service)//línea agregada de la versión en inglés. Según AM, sirve para los servicios
                {

                    institutions = db.Institutions.OfType<Hospital>()
                                .Where(i => i.Father_ID == user.InstitutionID || i.ID == user.InstitutionID);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Regional)
                {
                    if (user.type_region == 1 || user.type_region == null)
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID && i.cod_region_institucional == user.Institution.cod_region_institucional).OrderBy(j => j.FullName);
                    }
                    else if (user.type_region == 2)
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID && i.cod_region_salud == user.Institution.cod_region_salud).OrderBy(j => j.FullName);
                    }
                    else if (user.type_region == 3)
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID && i.cod_region_pais == user.Institution.cod_region_pais).OrderBy(j => j.FullName);
                    }
                }
                else
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.ID == user.Institution.ID);
                }
            }
            else
            {
                ExportarViewModel.DisplayHospitals = true;

                var inst_by_lab = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(j => j.InstitutionToID == user.InstitutionID).Select(i => i.InstitutionParentID).ToList();
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.CountryID == user.Institution.CountryID).OrderBy(j => j.FullName);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Area)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.AreaID == user.Institution.AreaID).OrderBy(j => j.FullName);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Regional)
                {
                    if (user.type_region == 1 || user.type_region == null)
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID && i.cod_region_institucional == user.Institution.cod_region_institucional).OrderBy(j => j.FullName);
                    }
                    else if (user.type_region == 2)
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID && i.cod_region_salud == user.Institution.cod_region_salud).OrderBy(j => j.FullName);
                    }
                    else if (user.type_region == 3)
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID && i.cod_region_pais == user.Institution.cod_region_pais).OrderBy(j => j.FullName);
                    }
                }
                else
                {
                    if (inst_by_lab.Any())
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => inst_by_lab.Contains(i.ID));
                    }
                    else
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.ID == user.Institution.ID);
                    }

                }
            }

            var DoS = DateTime.Now.ToString(getMsg("msgDatePickerConfig"));
            var date_format = getMsg("msgDateFormatDP");

            //CaseViewModel.UsrCtry = user.Institution.CountryID;
            ExportarViewModel.DatePickerConfig = DoS;
            ExportarViewModel.DateFormatDP = date_format;

            ExportarViewModel.Countries = db.Countries
                    .Select(c => new CountryView()
                    {
                        Id = c.ID.ToString(),
                        Name = c.Name,
                        Active = c.Active
                    }).ToArray();

            if (ReportsCountries != null)
            {
                ExportarViewModel.ReportsCountries = ReportsCountries.Select(i => new LookupView<ReportCountry>()
                {
                    Id = i.ID.ToString(),
                    Name = i.description,
                    orden = i.ReportID.ToString()
                }).ToArray();
            }

            //**** 
            if (areas != null)
            {
                var areasDisplay = areas.Select(i => new LookupView<Area>()
                {
                    Id = i.ID.ToString(),
                    Name = i.Name
                }).OrderBy(d => d.Name).ToList();

                if (areasDisplay.Count() > 1)
                {
                    //var all = new LookupView<Region> { Id = "0", Name = "-- Todo(a)s --" };
                    var all = new LookupView<Area> { Id = "0", Name = getMsg("msgGeneralMessageAll") };
                    areasDisplay.Insert(0, all);
                    ExportarViewModel.DisplayAreas = true;
                }

                ExportarViewModel.Areas = areasDisplay;
                //CaseViewModel.Regions = regions;
            }

            if (regions != null)
            {
                var regionsDisplay = regions.Select(i => new LookupView<Paho.Models.Region>()
                {
                    Id = i.orig_country.ToString(),
                    Name = i.Name
                }).ToList();

                if (regionsDisplay.Count() > 1)
                {
                    //var all = new LookupView<Paho.Models.Region> { Id = "0", Name = "-- Todo(a)s --" };
                    var all = new LookupView<Paho.Models.Region> { Id = "0", Name = getMsg("msgGeneralMessageAll") };
                    regionsDisplay.Insert(0, all);
                    ExportarViewModel.DisplayRegionals = true;
                }

                ExportarViewModel.Regions = regionsDisplay;
                //CaseViewModel.Regions = regions;
            }

            if (institutions != null)
            {
                var institutionsDisplay = institutions.Select(i => new LookupView<Paho.Models.Institution>()
                {
                    Id = i.ID.ToString(),
                    Name = i.Name
                }).ToList();

                if (institutionsDisplay.Count() > 1)
                {
                    //var all = new LookupView<Paho.Models.Institution> { Id = "0", Name = "-- Todo(a)s --" };
                    var all = new LookupView<Paho.Models.Institution> { Id = "0", Name = getMsg("msgGeneralMessageAll") };
                    institutionsDisplay.Insert(0, all);
                }

                ExportarViewModel.Institutions = institutionsDisplay;
            };

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

            return View(ExportarViewModel);
        }

        private IQueryable<FluCase> GetQuery(int CountryID, int? HospitalID, int? Year, int? Month, int? SE, DateTime? StartDate, DateTime? EndDate)
        {
            IQueryable<FluCase> query = db.Set<FluCase>();
            // assuming that you return all records when nothing is specified in the filter
            query = query.Where(t =>
                   t.CountryID == CountryID);

            if (HospitalID.HasValue)
                query = query.Where(t =>
                    t.HospitalID == HospitalID);

            if (Year.HasValue)
                query = query.Where(t =>
                    t.HospitalDate.Year == Year.Value);

            if (Month.HasValue)
                query = query.Where(t =>
                    t.HospitalDate.Month == Month.Value);

            if (SE.HasValue)
                query = query.Where(t =>
                    t.HospEW == SE.Value);

            if (StartDate.HasValue)
                query = query.Where(t =>
                   t.HospitalDate >= StartDate.Value);

            if (EndDate.HasValue)
                query = query.Where(t =>
                   t.HospitalDate >= EndDate.Value);

            return query;
        }

        [HttpGet]
        public ActionResult GetExcel(string Report, int CountryID, int? RegionID, int? HospitalID, int? Year, int? Month, int? SE,
            DateTime? StartDate, DateTime? EndDate, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? Inusual,
            string CasosNPHL = "", int? Area = 0, int? Sentinel = null)        //#### CAFQ
        {
            try
            {
                var ms = new MemoryStream();

                if (Surv == 3)
                {
                    Surv = 0;
                    Inusual = true;
                }
                else if (Surv == 0)
                    Inusual = null;

                int AreaID_ = (int)Area;
                var user = UserManager.FindById(User.Identity.GetUserId());
                int CountryID_ = (CountryID >= 0) ? CountryID : (user.Institution.CountryID ?? 0);
                //int? HospitalID_ = (user.Institution.Father_ID > 0 || user.Institution.Father_ID == null) ? HospitalID : Convert.ToInt32(user.Institution.ID);
                //int? HospitalID_ = (HospitalID >= 0) ? HospitalID : Convert.ToInt32(user.Institution.ID);
                int? HospitalID_ = (HospitalID > 0) ? HospitalID : 0;
                int? RegionID_ = (RegionID >= 0) ? RegionID : (user.Institution.cod_region_institucional ?? 0);
                string Languaje_ = user.Institution.Country.Language ?? "SPA";
                string Country_Code = user.Institution.Country.Code;

                var reportCountry = db.ReportsCountries.FirstOrDefault(s => s.ID == ReportCountry);
                int reportID = reportCountry.ReportID;//contiene la FK para obtener el ID del reporte en la tabla Report
                int reportStartCol = reportCountry.startCol;//contiene la startCol de la tabla ReportCountry, pasa saber en que columna se inicia
                int reportStartRow = reportCountry.startRow;//contiene la startRow de la tabla ReportCountry, pasa saber en que fila se inicia
                var report = db.Reports.FirstOrDefault(s => s.ID == reportID);
                string reportTemplate = report.Template;//contiene el nombre del template, que luego se relacionará al archivo template de Excel

                if (user.Institution.AccessLevel == AccessLevel.SelfOnly && HospitalID_ == 0) { HospitalID_ = Convert.ToInt32(user.Institution.ID); }
                if (user.Institution.AccessLevel == AccessLevel.Area && Area == 0) { AreaID_ = Convert.ToInt32(user.Institution.AreaID); }

                if (ReportCountry < 1)
                {
                    ViewBag.Message = "El reporte no se pudo generar, por favor intente de nuevo";
                    return null;
                    ;
                }

                string templateToUse;

                switch (reportTemplate.ToUpper())
                {
                    case "D1":
                        templateToUse = "DenominadoresTemplate";
                        break;
                    case "B1":
                        templateToUse = "BitacoraTemplate";
                        break;
                    case "I1":
                        templateToUse = "IndicadoresDesempenio";            //#### CAFQ en web.config crear la variable 
                        break;
                    case "RE1":
                        templateToUse = "REVELAC-i";                        //#### CAFQ en web.config crear la variable 
                        break;
                    case "ML1":
                        templateToUse = "MuestrasLabTemplate";                        //#### CAFQ en web.config crear la variable 
                        break;
                    case "FM1":
                        templateToUse = "FormSariIliHospDeathTemplate";                        //#### CAFQ en web.config crear la variable 
                        break;
                    case "FLUID":
                        templateToUse = "FluIDTemplate";
                        break;
                    case "CC":
                        templateToUse = "ConsolidadoCargaTemplate";
                        break;
                    case "LRD":
                        templateToUse = "LibroReporteDiario";
                        break;
                    case "CPE":
                        templateToUse = "CondicionesPreExistentes";
                        break;
                    case "CPV":
                        templateToUse = "CasosPositivosConVacuna";
                        break;
                    case "C1":
                        templateToUse = "CONSDATA";
                        break;
                    default:
                        templateToUse = "SariTemplate";
                        break;
                }

                var tempy = ConfigurationManager.AppSettings[templateToUse]
                    .Replace("{report}", reportTemplate)
                    .Replace("{countryId}", CountryID_.ToString());

                var tempy2 = System.IO.File.Exists(tempy);

                using (var fs = System.IO.File.OpenRead(ConfigurationManager.AppSettings[templateToUse]
                    .Replace("{report}", reportTemplate)
                    .Replace("{countryId}", CountryID_.ToString())
                    ))
                {
                    using (var excelPackage = new ExcelPackage(fs))
                    {
                        var excelWorkBook = excelPackage.Workbook;
                        bool insertRow = true;

                        //#### CAFQ: 180204
                        bool bVariosAnios = false;
                        if (YearFrom != null && YearTo != null)
                        {
                            if (YearTo > DateTime.Now.Year) YearTo = DateTime.Now.Year;
                            bVariosAnios = (YearFrom != YearTo) ? true : false;
                        }
                        //#### 

                        if (reportTemplate == "R1" || reportTemplate == "R2" || reportTemplate == "R3" || reportTemplate == "R4" || reportTemplate == "D1" || reportTemplate == "B1" || reportTemplate == "CPE" || reportTemplate == "C1")
                        {
                            insertRow = false;
                        } // R4 = Virus detectados


                        if (reportTemplate == "I1")      //#### CAFQ
                            AppendDataToExcel_IndDes(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook,
                                reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual, AreaID_);        //#### CAFQ
                        else if (reportTemplate == "RE1")
                            AppendDataToExcel_REVELAC(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook,
                                reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual, AreaID_);        //#### CAFQ
                        else if (reportTemplate == "CC")
                            AppendDataToExcel_ConsolidadoCarga(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook,
                                reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual, AreaID_, Sentinel);        //#### CAFQ
                        else if (reportTemplate == "CPV")
                            AppendDataToExcel_CasosPositivosConVacuna(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook,
                                reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual, AreaID_, Sentinel);        //#### CAFQ
                        else if (reportTemplate.ToUpper() == "R6" && bVariosAnios)
                        {
                            AppendDataToExcel_R6_SeveralYears(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook,
                                reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual, AreaID_, Sentinel);        //#### CAFQ
                        }
                        else if (reportTemplate.ToUpper() == "R4")
                        {
                            ////////////////////////////// Aquí comienza R4 - Virus detectados////////////////////
                            var contador = 0;
                            var YearBegin = 0;
                            var YearEnd = 0;

                            if (YearFrom != null && YearTo != null)
                            {
                                YearBegin = (int)YearFrom;
                                YearEnd = (int)YearTo;
                                if (YearEnd > DateTime.Now.Year) YearEnd = DateTime.Now.Year;
                            }
                            else if (Year != null)
                            {
                                YearBegin = (int)Year;
                                YearEnd = (int)Year;
                                if (YearEnd > DateTime.Now.Year)
                                {
                                    YearBegin = DateTime.Now.Year;
                                    YearEnd = DateTime.Now.Year;
                                }
                            }

                            var excelWs_VIRUSES_IRAG = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "Virus" : "Virus"];
                            var excelWs_VIRUSES_Chart = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "Graph Virus" : "Gráficos"];

                            var excelWs_VIRUSES_INF_Geographic = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "Virus_INF_GEO" : "Virus_INF_GEO"];
                            var excelWs_VIRUSES_RSV_Geographic = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "Virus_RSV_GEO" : "Virus_VSR_GEO"];


                            contador = YearEnd - YearBegin;
                            var YearEnd_report = DateTime.Now.Year;

                            for (int i = 0; i <= contador; i++)
                            {

                                YearEnd_report = YearEnd - i;


                                if (i > 0)
                                {
                                    CopyAndPasteRange(excelWorkBook, excelWs_VIRUSES_IRAG.Index, excelWorkBook, excelWs_VIRUSES_IRAG.Index, "A" + Convert.ToString(6 + (52 * i)) + ":BZ" + Convert.ToString(6 + (52 * i)), "A" + Convert.ToString(6 + (52 * (i + 1))) + ":BZ" + Convert.ToString(6 + (52 * (i + 1))));
                                    CopyAndPasteRange(excelWorkBook, excelWs_VIRUSES_IRAG.Index, excelWorkBook, excelWs_VIRUSES_IRAG.Index, "A6:BZ57", "A" + Convert.ToString(6 + (52 * i)) + ":BZ" + Convert.ToString(6 + (52 * i) + 52));

                                    if (excelWs_VIRUSES_INF_Geographic != null)
                                        CopyAndPasteRange(excelWorkBook, excelWs_VIRUSES_INF_Geographic.Index, excelWorkBook, excelWs_VIRUSES_INF_Geographic.Index, "A" + Convert.ToString(5 + (52 * i)) + ":BZ" + Convert.ToString(5 + (52 * i)), "A" + Convert.ToString(5 + (52 * (i + 1))) + ":BZ" + Convert.ToString(5 + (52 * (i + 1))));
                                    if (excelWs_VIRUSES_RSV_Geographic != null)
                                        CopyAndPasteRange(excelWorkBook, excelWs_VIRUSES_RSV_Geographic.Index, excelWorkBook, excelWs_VIRUSES_RSV_Geographic.Index, "A" + Convert.ToString(5 + (52 * i)) + ":BZ" + Convert.ToString(5 + (52 * i)), "A" + Convert.ToString(5 + (52 * (i + 1))) + ":BZ" + Convert.ToString(5 + (52 * (i + 1))));

                                }
                                if (i > 0)
                                {
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_VIRUSES_Chart.Index, excelWs_VIRUSES_IRAG.Index, "CV1", (6 + (52 * i)), (6 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_VIRUSES_Chart.Index, excelWs_VIRUSES_IRAG.Index, "CV2", (6 + (52 * i)), (6 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_VIRUSES_Chart.Index, excelWs_VIRUSES_IRAG.Index, "CV3", (6 + (52 * i)), (6 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_VIRUSES_Chart.Index, excelWs_VIRUSES_IRAG.Index, "CV4", (6 + (52 * i)), (6 + (52 * i)) + 51);
                                    // Configuración de gráfica de Pie
                                    ConfigGraph_Pie(excelWorkBook, excelWs_VIRUSES_Chart.Index, excelWs_VIRUSES_IRAG.Index, "CV5", (7 + (52 * i)) + 51, (7 + (52 * i)) + 51);
                                    ConfigGraph_Pie(excelWorkBook, excelWs_VIRUSES_Chart.Index, excelWs_VIRUSES_IRAG.Index, "CV6", (7 + (52 * i)) + 51, (7 + (52 * i)) + 51);


                                }
                                AppendDataToExcel_R4(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "R4", (i > 0 ? (6 + (52 * i)) : 6), 1, excelWs_VIRUSES_IRAG.Index, false, ReportCountry, YearEnd_report, YearEnd_report, Surv, Inusual, AreaID_, Sentinel);

                            }


                            // Procedimiento para cambiar los rangos del eje X 
                            if (contador > 0)
                            {
                                var RangeStr = " '" + ((user.Institution.Country.Language == "ENG") ? "Virus" : "Virus") + "'!$BY$6:$BZ$57 ";

                                for (int i = 1; i <= contador; i++)
                                {
                                    RangeStr = " '" + ((user.Institution.Country.Language == "ENG") ? "Virus" : "Virus") + "'!$BY$" + (6 + (52 * i)).ToString() + ":$BZ$" + ((6 + (52 * i)) + 51).ToString() + ", " + RangeStr;
                                }

                                RangeStr = " ( " + RangeStr + ")";

                                //Tamaño original de las gráficas
                                // LineChart.SetSize(1375, 700);

                                var graph_name = "CV1";
                                var LineChart = excelWs_VIRUSES_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((150 * contador) + 1375, 700);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "CV2";
                                LineChart = excelWs_VIRUSES_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((150 * contador) + 1375, 700);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "CV3";
                                LineChart = excelWs_VIRUSES_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((150 * contador) + 1375, 700);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "CV4";
                                LineChart = excelWs_VIRUSES_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((150 * contador) + 1375, 700);
                                UpdateRangeXMLPath(LineChart, RangeStr);
                            }

                            if (excelWs_VIRUSES_INF_Geographic != null)
                                AppendDataToExcel_R4(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "R4_complement", 5, 1, excelWs_VIRUSES_INF_Geographic.Index, false, ReportCountry, YearBegin, YearEnd, Surv, Inusual, AreaID_, Sentinel);

                            if (excelWs_VIRUSES_RSV_Geographic != null)
                                AppendDataToExcel_R4(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "R4_complement", 5, 1, excelWs_VIRUSES_RSV_Geographic.Index, false, ReportCountry, YearBegin, YearEnd, Surv, Inusual, AreaID_, Sentinel);

                            var excelWs_Leyendas = excelWorkBook.Worksheets["Leyendas"];
                            ConfigToExcel_FLUID(CountryID, Languaje_, RegionID_, YearEnd, YearBegin, YearEnd, StartDate, EndDate, HospitalID_, Surv, excelWorkBook, "Leyendas", 1, excelWs_Leyendas.Index, false);
                            
                            // Fin R4 - Virus detectados
                        }
                        else if (reportTemplate.ToUpper() == "FLUID")
                        {
                            /////////////////////////////////////// FLUID  ////////////////////////////////////////////////////////////////////////7

                            var contador = 0;
                            var YearBegin = 0;
                            var YearEnd = 0;

                            if (YearFrom != null && YearTo != null)
                            {
                                YearBegin = (int)YearFrom;
                                YearEnd = (int)YearTo;
                                if (YearEnd > DateTime.Now.Year) YearEnd = DateTime.Now.Year;
                            }
                            else if (Year != null)
                            {
                                YearBegin = (int)Year;
                                YearEnd = (int)Year;
                                if (YearEnd > DateTime.Now.Year)
                                {
                                    YearBegin = DateTime.Now.Year;
                                    YearEnd = DateTime.Now.Year;
                                }
                            }

                            /////////////////////////////////////// VIRUS IRAG  ////////////////////////////////////////////////////////////////////////7

                            var excelWs_VIRUSES_IRAG = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "NATIONAL VIRUSES" : "Virus Identificados"];
                            var excelWs_VIRUSES_Chart = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "Graph Virus" : "Gráficos Virus IRAG"];
                            contador = YearEnd - YearBegin;
                            var YearEnd_report = DateTime.Now.Year;

                            var excelWs_VIRUSES_INF_Geographic = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "Virus_INF_GEO" : "Virus_INF_GEO"];
                            var excelWs_VIRUSES_RSV_Geographic = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "Virus_RSV_GEO" : "Virus_VSR_GEO"];


                            for (int i = 0; i <= contador; i++)
                            {

                                YearEnd_report = YearEnd - i;


                                if (i > 0)
                                {
                                    CopyAndPasteRange(excelWorkBook, excelWs_VIRUSES_IRAG.Index, excelWorkBook, excelWs_VIRUSES_IRAG.Index, "A" + Convert.ToString(6 + (52 * i)) + ":BZ" + Convert.ToString(6 + (52 * i)), "A" + Convert.ToString(6 + (52 * (i + 1))) + ":BZ" + Convert.ToString(6 + (52 * (i + 1))));
                                    CopyAndPasteRange(excelWorkBook, excelWs_VIRUSES_IRAG.Index, excelWorkBook, excelWs_VIRUSES_IRAG.Index, "A6:BZ57", "A" + Convert.ToString(6 + (52 * i)) + ":BZ" + Convert.ToString(6 + (52 * i) + 52));

                                    // Totales Virus INF Geo y VIrus RSV Geto
                                    if (excelWs_VIRUSES_INF_Geographic != null)
                                        CopyAndPasteRange(excelWorkBook, excelWs_VIRUSES_INF_Geographic.Index, excelWorkBook, excelWs_VIRUSES_INF_Geographic.Index, "A" + Convert.ToString(5 + (52 * i)) + ":BZ" + Convert.ToString(5 + (52 * i)), "A" + Convert.ToString(5 + (52 * (i + 1))) + ":BZ" + Convert.ToString(5 + (52 * (i + 1))));
                                    if (excelWs_VIRUSES_RSV_Geographic != null)
                                        CopyAndPasteRange(excelWorkBook, excelWs_VIRUSES_RSV_Geographic.Index, excelWorkBook, excelWs_VIRUSES_RSV_Geographic.Index, "A" + Convert.ToString(5 + (52 * i)) + ":BZ" + Convert.ToString(5 + (52 * i)), "A" + Convert.ToString(5 + (52 * (i + 1))) + ":BZ" + Convert.ToString(5 + (52 * (i + 1))));
                                }
                                if (i > 0)
                                {
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_VIRUSES_Chart.Index, excelWs_VIRUSES_IRAG.Index, "CV1", (6 + (52 * i)), (6 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_VIRUSES_Chart.Index, excelWs_VIRUSES_IRAG.Index, "CV2", (6 + (52 * i)), (6 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_VIRUSES_Chart.Index, excelWs_VIRUSES_IRAG.Index, "CV3", (6 + (52 * i)), (6 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_VIRUSES_Chart.Index, excelWs_VIRUSES_IRAG.Index, "CV4", (6 + (52 * i)), (6 + (52 * i)) + 51);
                                    // Configuración de gráfica de Pie
                                    ConfigGraph_Pie(excelWorkBook, excelWs_VIRUSES_Chart.Index, excelWs_VIRUSES_IRAG.Index, "CV5", (7 + (52 * i)) + 51, (7 + (52 * i)) + 51);
                                    ConfigGraph_Pie(excelWorkBook, excelWs_VIRUSES_Chart.Index, excelWs_VIRUSES_IRAG.Index, "CV6", (7 + (52 * i)) + 51, (7 + (52 * i)) + 51);


                                }
                                AppendDataToExcel_R4(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "R4", (i > 0 ? (6 + (52 * i)) : 6), 1, excelWs_VIRUSES_IRAG.Index, false, ReportCountry, YearEnd_report, YearEnd_report, 1, Inusual, AreaID_, Sentinel);

                               if (excelWs_VIRUSES_INF_Geographic != null)
                                    AppendDataToExcel_R4(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "R4_complement", 5, 1, excelWs_VIRUSES_INF_Geographic.Index, false, ReportCountry, YearEnd_report, YearEnd_report, Surv, Inusual, AreaID_, Sentinel);

                                if (excelWs_VIRUSES_RSV_Geographic != null)
                                    AppendDataToExcel_R4(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "R4_complement", 5, 1, excelWs_VIRUSES_RSV_Geographic.Index, false, ReportCountry, YearEnd_report, YearEnd_report, Surv, Inusual, AreaID_, Sentinel);

                            }


                            // Procedimiento para cambiar los rangos del eje X 
                            if (contador > 0)
                            {
                                var RangeStr = " '" + ((user.Institution.Country.Language == "ENG") ? "NATIONAL VIRUSES" : "Virus Identificados") + "'!$BY$6:$BZ$57 ";

                                for (int i = 1; i <= contador; i++)
                                {
                                    RangeStr = " '" + ((user.Institution.Country.Language == "ENG") ? "NATIONAL VIRUSES" : "Virus Identificados") + "'!$BY$" + (6 + (52 * i)).ToString() + ":$BZ$" + ((6 + (52 * i)) + 51).ToString() + ", " + RangeStr;
                                }

                                RangeStr = " ( " + RangeStr + ")";

                                //Tamaño original de las gráficas
                                // LineChart.SetSize(1375, 700);

                                var graph_name = "CV1";
                                var LineChart = excelWs_VIRUSES_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((150 * contador) + 1375, 700);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "CV2";
                                LineChart = excelWs_VIRUSES_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((150 * contador) + 1375, 700);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "CV3";
                                LineChart = excelWs_VIRUSES_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((150 * contador) + 1375, 700);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "CV4";
                                LineChart = excelWs_VIRUSES_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((150 * contador) + 1375, 700);
                                UpdateRangeXMLPath(LineChart, RangeStr);
                            }

                            //var excelWs_VIRUSES_IRAG = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "NATIONAL VIRUSES" : "Virus Identificados"];
                            //AppendDataToExcel_FLUID(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "FLUID_NATIONAL_VIRUSES", 6, 1, excelWs_VIRUSES_IRAG.Index, false, ReportCountry, YearEnd, YearEnd, 1, Inusual, AreaID_, Sentinel);

                            //////////////////////////////////////////////////// IRAG  /////////////////////////////////////////////////////////////////////////////////7777
                            var excelWs_IRAG = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "SARI" : "IRAG"];
                            var excelWs_IRAG_Chart = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "SARI Graphs" : "Gráficos IRAG"];

                            AppendDataToExcel_FLUID(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "FLUID_IRAG", 8, 1, excelWs_IRAG.Index, false, ReportCountry, YearBegin, YearEnd, 1, Inusual, AreaID_, Sentinel);

                            // Ajustar gráficas IRAG
                            for (int i = 0; i <= contador; i++)
                            {

                                YearEnd_report = YearEnd - i;

                                if (i > 0)
                                {
                                    // Copy rangos para los años
                                    CopyAndPasteRange(excelWorkBook, excelWs_IRAG.Index, excelWorkBook, excelWs_IRAG.Index, "BY8:BZ59", "BY" + Convert.ToString(8 + (52 * i)) + ":BZ" + Convert.ToString(8 + (52 * i) + 52));

                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_IRAG_Chart.Index, excelWs_IRAG.Index, "GS1", (11 + (52 * i)), (11 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_IRAG_Chart.Index, excelWs_IRAG.Index, "GS2", (11 + (52 * i)), (11 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_IRAG_Chart.Index, excelWs_IRAG.Index, "GS4", (11 + (52 * i)), (11 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_IRAG_Chart.Index, excelWs_IRAG.Index, "GS5", (8 + (52 * i)), (8 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_IRAG_Chart.Index, excelWs_IRAG.Index, "GS6", (11 + (52 * i)), (11 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_IRAG_Chart.Index, excelWs_IRAG.Index, "GS7", (8 + (52 * i)), (8 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_IRAG_Chart.Index, excelWs_IRAG.Index, "GS8", (11 + (52 * i)), (11 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_IRAG_Chart.Index, excelWs_IRAG.Index, "GS9", (8 + (52 * i)), (8 + (52 * i)) + 51);

                                    // Configuración de gráfica de Pie
                                    //ConfigGraph_Pie(excelWorkBook, excelWs_VIRUSES_Chart.Index, excelWs_VIRUSES_IRAG.Index, "CV5", (7 + (52 * i)) + 51, (7 + (52 * i)) + 51);

                                }

                            }

                            if (contador > 0)
                            {
                                var RangeStr = " '" + ((user.Institution.Country.Language == "ENG") ? "SARI" : "IRAG") + "'!$BY$8:$BZ$59 ";

                                for (int i = 1; i <= contador; i++)
                                {
                                    RangeStr = " '" + ((user.Institution.Country.Language == "ENG") ? "SARI" : "IRAG") + "'!$BY$" + (8 + (52 * i)).ToString() + ":$BZ$" + ((8 + (52 * i)) + 51).ToString() + ", " + RangeStr;
                                }

                                RangeStr = " ( " + RangeStr + ")";

                                //Tamaño original de las gráficas
                                // LineChart.SetSize(1375, 700);

                                var graph_name = "GS1";
                                var LineChart = excelWs_IRAG_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((80 * contador) + 600, 325);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "GS2";
                                LineChart = excelWs_IRAG_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((80 * contador) + 600, 325);
                                UpdateRangeXMLPath(LineChart, RangeStr);
                                graph_name = "GS4";
                                LineChart = excelWs_IRAG_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((80 * contador) + 600, 325);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "GS5";
                                LineChart = excelWs_IRAG_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((80 * contador) + 600, 325);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "GS6";
                                LineChart = excelWs_IRAG_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((80 * contador) + 600, 325);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "GS7";
                                LineChart = excelWs_IRAG_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((80 * contador) + 600, 325);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "GS8";
                                LineChart = excelWs_IRAG_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((80 * contador) + 600, 325);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "GS9";
                                LineChart = excelWs_IRAG_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((80 * contador) + 600, 325);
                                UpdateRangeXMLPath(LineChart, RangeStr);
                            }

                            contador = YearEnd - YearBegin;
                            //if (contador > 0)
                            //{
                            //    //var excelWs_Graph_IRAG = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "SARI Graphs" : "Gráficos IRAG"];
                            //    //var excelWs_IRAG = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "SARI" : "IRAG"];

                            //    for (int i = 1; i <= contador; i++)
                            //    {
                            //        ConfigGraph_FLUID(YearEnd - i, i, excelWorkBook, excelWs_IRAG_Chart.Index, excelWs_IRAG.Index, "GS1", "C", "E");
								 
                            //    }
                            //}

                            /////////////////////////////////////////////////////////////////// DEATHS  /////////////////////////////////////////////////////////
                            var excelWs_DEATHS_IRAG = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "DEATHS Sentinel Sites" : "Fallecidos IRAG"];
                            var excelWs_DEATHS_IRAG_Chart = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "DEATHS Sentinel Sites" : "Fallecidos IRAG"];

                            AppendDataToExcel_FLUID(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "FLUID_DEATHS_IRAG", 8, 1, excelWs_DEATHS_IRAG.Index, false, ReportCountry, YearBegin, YearEnd, 1, Inusual, AreaID_, Sentinel);

                            // DEATHS Chart

                            contador = YearEnd - YearBegin;
                            //if (contador > 0)
                            //{

                            //    for (int i = 1; i <= contador; i++)
                            //    {
                            //        ConfigGraph_FLUID(YearEnd - i, i, excelWorkBook, excelWs_DEATHS_IRAG_Chart.Index, excelWs_DEATHS_IRAG.Index, "CD3", "C", "K");
                            //    }
                            //}

                            for (int i = 0; i <= contador; i++)
                            {

                                YearEnd_report = YearEnd - i;

                                if (i > 0)
                                {
                                    // Copy rangos para los años
                                    CopyAndPasteRange(excelWorkBook, excelWs_DEATHS_IRAG.Index, excelWorkBook, excelWs_DEATHS_IRAG.Index, "BY8:BZ59", "BY" + Convert.ToString(8 + (52 * i)) + ":BZ" + Convert.ToString(8 + (52 * i) + 52));

                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_DEATHS_IRAG_Chart.Index, excelWs_DEATHS_IRAG.Index, "CD1", (8 + (52 * i)), (8 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_DEATHS_IRAG_Chart.Index, excelWs_DEATHS_IRAG.Index, "CD2", (8 + (52 * i)), (8 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_DEATHS_IRAG_Chart.Index, excelWs_DEATHS_IRAG.Index, "CD3", (8 + (52 * i)), (8 + (52 * i)) + 51);

                                }

                            }

                            if (contador > 0)
                            {
                                var RangeStr = " '" + ((user.Institution.Country.Language == "ENG") ? "DEATHS Sentinel Sites" : "Fallecidos IRAG") + "'!$BY$8:$BZ$59 ";

                                for (int i = 1; i <= contador; i++)
                                {
                                    RangeStr = " '" + ((user.Institution.Country.Language == "ENG") ? "DEATHS Sentinel Sites" : "Fallecidos IRAG") + "'!$BY$" + (8 + (52 * i)).ToString() + ":$BZ$" + ((8 + (52 * i)) + 51).ToString() + ", " + RangeStr;
                                }

                                RangeStr = " ( " + RangeStr + ")";

                                //Tamaño original de las gráficas
                                // LineChart.SetSize(1375, 700);

                                var graph_name = "CD1";
                                var LineChart = excelWs_DEATHS_IRAG_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((100 * contador) + 1000, 650);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "CD2";
                                LineChart = excelWs_DEATHS_IRAG_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((100 * contador) + 1000, 650);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "CD3";
                                LineChart = excelWs_DEATHS_IRAG_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((100 * contador) + 1000, 650);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                            }

                            ////////////////////////////////////////////////////////////////// ILI //////////////////////////////////////////////////
                            var excelWs_ILI = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "ILI" : "ETI"];
                            var excelWs_ILI_Chart = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "ILI" : "ETI"];

                            AppendDataToExcel_FLUID(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "FLUID_ETI", 8, 1, excelWs_ILI.Index, false, ReportCountry, YearBegin, YearEnd, 2, Inusual, AreaID_, Sentinel);


                            // Ajustar gráficas ILI
                            for (int i = 0; i <= contador; i++)
                            {

                                YearEnd_report = YearEnd - i;

                                if (i > 0)
                                {
                                    // Copy rangos para los años
                                    CopyAndPasteRange(excelWorkBook, excelWs_ILI.Index, excelWorkBook, excelWs_ILI.Index, "BY8:BZ59", "BY" + Convert.ToString(8 + (52 * i)) + ":BZ" + Convert.ToString(8 + (52 * i) + 52));


                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_ILI_Chart.Index, excelWs_ILI.Index, "CILI1", (8 + (52 * i)), (8 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_ILI_Chart.Index, excelWs_ILI.Index, "CILI3", (8 + (52 * i)), (8 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_ILI_Chart.Index, excelWs_ILI.Index, "CILI4", (8 + (52 * i)), (8 + (52 * i)) + 51);

                                    // Configuración de gráfica de Pie
                                    //ConfigGraph_Pie(excelWorkBook, excelWs_VIRUSES_Chart.Index, excelWs_VIRUSES_IRAG.Index, "CV5", (7 + (52 * i)) + 51, (7 + (52 * i)) + 51);

                                }

                            }

                            if (contador > 0)
                            {
                                var RangeStr = " '" + ((user.Institution.Country.Language == "ENG") ? "ILI" : "ETI") + "'!$BY$8:$BZ$59 ";

                                for (int i = 1; i <= contador; i++)
                                {
                                    RangeStr = " '" + ((user.Institution.Country.Language == "ENG") ? "ILI" : "ETI") + "'!$BY$" + (8 + (52 * i)).ToString() + ":$BZ$" + ((8 + (52 * i)) + 51).ToString() + ", " + RangeStr;
                                }

                                RangeStr = " ( " + RangeStr + ")";

                                //Tamaño original de las gráficas
                                // LineChart.SetSize(1375, 700);

                                var graph_name = "CILI1";
                                var LineChart = excelWs_ILI_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((80 * contador) + 600, 325);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "CILI3";
                                LineChart = excelWs_ILI_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((80 * contador) + 600, 325);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "CILI4";
                                LineChart = excelWs_ILI_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((80 * contador) + 600, 325);
                                UpdateRangeXMLPath(LineChart, RangeStr);
                            }


                            //////////////////////////////////////////////////////////////  VIRUS ILI ////////////////////////////////////////////////////  

                            var excelWs_VIRUSES_ILI = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "ILI VIRUSES - Sentinel" : "Virus ETI Identificados"];
                            var excelWs_VIRUSES_ILI_Chart = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "ILI Graphs Viruses" : "Gráficos Virus ETI"];
                            contador = YearEnd - YearBegin;
                            YearEnd_report = DateTime.Now.Year;
                            //AppendDataToExcel_FLUID(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "FLUID_NATIONAL_VIRUSES", 6, 1, excelWs_VIRUSES_ILI.Index, false, ReportCountry, YearBegin, YearEnd, 2, Inusual, AreaID_, Sentinel);

                            for (int i = 0; i <= contador; i++)
                            {

                                YearEnd_report = YearEnd - i;


                                if (i > 0)
                                {
                                    //Insert_Copy_And_PasteRange(excelWorkBook, excelWs_VIRUSES_IRAG.Index, excelWorkBook, excelWs_VIRUSES_IRAG.Index, "A4:BZ58", "A" + Convert.ToString(4 + (57 * i)) + ":BZ" + Convert.ToString(4 + (57 * i) + 55), (52 * i), 52);
                                    //CopyAndPasteRange(excelWorkBook, excelWs_VIRUSES_IRAG.Index, excelWorkBook, excelWs_VIRUSES_IRAG.Index, "A4:BZ58", "A" + Convert.ToString(4 + (57 * i)) + ":BZ" + Convert.ToString(4 + (57 * i) + 55));
                                    CopyAndPasteRange(excelWorkBook, excelWs_VIRUSES_ILI.Index, excelWorkBook, excelWs_VIRUSES_ILI.Index, "A" + Convert.ToString(6 + (52 * i)) + ":BZ" + Convert.ToString(6 + (52 * i)), "A" + Convert.ToString(6 + (52 * (i + 1))) + ":BZ" + Convert.ToString(6 + (52 * (i + 1))));
                                    CopyAndPasteRange(excelWorkBook, excelWs_VIRUSES_ILI.Index, excelWorkBook, excelWs_VIRUSES_ILI.Index, "A6:BZ57", "A" + Convert.ToString(6 + (52 * i)) + ":BZ" + Convert.ToString(6 + (52 * i) + 52));

                                }
                                if (i > 0)
                                {
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_VIRUSES_ILI_Chart.Index, excelWs_VIRUSES_ILI.Index, "CVILI1", (6 + (52 * i)), (6 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_VIRUSES_ILI_Chart.Index, excelWs_VIRUSES_ILI.Index, "CVILI2", (6 + (52 * i)), (6 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_VIRUSES_ILI_Chart.Index, excelWs_VIRUSES_ILI.Index, "CVILI3", (6 + (52 * i)), (6 + (52 * i)) + 51);
                                    ConfigGraph_Bars_Histogram(YearEnd_report, excelWorkBook, excelWs_VIRUSES_ILI_Chart.Index, excelWs_VIRUSES_ILI.Index, "CVILI4", (6 + (52 * i)), (6 + (52 * i)) + 51);
                                    // Configuración de gráfica de Pie
                                    ConfigGraph_Pie(excelWorkBook, excelWs_VIRUSES_ILI_Chart.Index, excelWs_VIRUSES_ILI.Index, "CVILI5", (7 + (52 * i)) + 51, (7 + (52 * i)) + 51);
                                    ConfigGraph_Pie(excelWorkBook, excelWs_VIRUSES_ILI_Chart.Index, excelWs_VIRUSES_ILI.Index, "CVILI6", (7 + (52 * i)) + 51, (7 + (52 * i)) + 51);


                                }
                                AppendDataToExcel_R4(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "R4", (i > 0 ? (6 + (52 * i)) : 6), 1, excelWs_VIRUSES_ILI.Index, false, ReportCountry, YearEnd_report, YearEnd_report, 2, Inusual, AreaID_, Sentinel);

                            }

                            if (contador > 0)
                            {
                                var RangeStr = " '" + ((user.Institution.Country.Language == "ENG") ? "ILI VIRUSES - Sentinel" : "Virus ETI Identificados") + "'!$BY$6:$BZ$57 ";

                                for (int i = 1; i <= contador; i++)
                                {
                                    RangeStr = " '" + ((user.Institution.Country.Language == "ENG") ? "ILI VIRUSES - Sentinel" : "Virus ETI Identificados") + "'!$BY$" + (6 + (52 * i)).ToString() + ":$BZ$" + ((6 + (52 * i)) + 51).ToString() + ", " + RangeStr;
                                }

                                RangeStr = " ( " + RangeStr + ")";

                                //Tamaño original de las gráficas
                                // LineChart.SetSize(1375, 700);

                                var graph_name = "CVILI1";
                                var LineChart = excelWs_VIRUSES_ILI_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((150 * contador) + 1375, 700);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "CVILI2";
                                LineChart = excelWs_VIRUSES_ILI_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((150 * contador) + 1375, 700);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "CVILI3";
                                LineChart = excelWs_VIRUSES_ILI_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((150 * contador) + 1375, 700);
                                UpdateRangeXMLPath(LineChart, RangeStr);

                                graph_name = "CVILI4";
                                LineChart = excelWs_VIRUSES_ILI_Chart.Drawings[graph_name] as ExcelChart;
                                LineChart.SetSize((150 * contador) + 1375, 700);
                                UpdateRangeXMLPath(LineChart, RangeStr);
                            }

                            // Leyendas
                            var excelWs_Leyendas = excelWorkBook.Worksheets["Leyendas"];
                            ConfigToExcel_FLUID(CountryID, Languaje_, RegionID_, YearEnd, YearBegin, YearEnd, StartDate, EndDate, HospitalID_, Surv, excelWorkBook, "Leyendas_FLUID", 1, excelWs_Leyendas.Index, false);


                        }
                        else if (reportTemplate.ToUpper() == "C1")
                        {
                            var contador = 0;
                            var YearBegin = 0;
                            var YearEnd = 0;
                            var Year_only = 0;

                            if (YearFrom != null && YearTo != null)
                            {
                                YearBegin = (int)YearFrom;
                                YearEnd = (int)YearTo;
                                if (YearEnd > DateTime.Now.Year) YearEnd = DateTime.Now.Year;
                            }
                            else if (Year != null)
                            {
                                YearBegin = (int)Year;
                                YearEnd = (int)Year;
                                if (YearEnd > DateTime.Now.Year)
                                {
                                    YearBegin = DateTime.Now.Year;
                                    YearEnd = DateTime.Now.Year;
                                }
                            }


                            var excelWs_VIRUSESA_IRAG = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "Virus IdentificadosA" : "Virus IdentificadosA"];
                            AppendDataToExcel_FLUID(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "CONSDATA_NATIONAL_VIRUSES", 6, 2, excelWs_VIRUSESA_IRAG.Index, false, ReportCountry, YearBegin, YearEnd, 1, Inusual, AreaID_, Sentinel);

                            var excelWs_VIRUSES_IRAG = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "Virus Identificados" : "Virus Identificados"];
                            AppendDataToExcel_FLUID(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "CONSDATA_NATIONAL_VIRUSES", 6, 2, excelWs_VIRUSES_IRAG.Index, false, ReportCountry, YearEnd, YearEnd, 1, Inusual, AreaID_, Sentinel);

                            var excelWs_Graficos_Gravedad = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "Grafico Gravedad" : "Grafico Gravedad"];
                            AppendDataToExcel_FLUID(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "CONSDATA_GRAVEDAD", 3, 3, excelWs_Graficos_Gravedad.Index, false, ReportCountry, YearEnd, YearEnd, 1, Inusual, AreaID_, Sentinel);


                            var excelWs_Graficos_Edad = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "Graficos EDAD" : "Graficos EDAD"];
                            AppendDataToExcel_FLUID(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "CONSDATA_GRAVEDAD_by_AGE", 3, 3, excelWs_Graficos_Edad.Index, false, ReportCountry, YearEnd, YearEnd, 1, Inusual, AreaID_, Sentinel);

                            var excelWs_Comorbilidades = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "Comorbilidades" : "Comorbilidades"];
                            AppendDataToExcel_CONSDATA(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "CONSDATA_COMORBILIDADES", 3, 2, excelWs_Comorbilidades.Index, false, ReportCountry, YearEnd, YearEnd, 1, Inusual, AreaID_, Sentinel, 0, 0);
                            AppendDataToExcel_CONSDATA(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "CONSDATA_COMORBILIDADES", 3, 13, excelWs_Comorbilidades.Index, false, ReportCountry, YearEnd, YearEnd, 1, Inusual, AreaID_, Sentinel, 1, 0);
                            AppendDataToExcel_CONSDATA(Languaje_, CountryID_, RegionID_, null, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, "CONSDATA_COMORBILIDADES", 3, 24, excelWs_Comorbilidades.Index, false, ReportCountry, YearEnd, YearEnd, 1, Inusual, AreaID_, Sentinel, 0, 1);


                            var excelWs_IRAG = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "IRAG" : "IRAG"];
                            // Del Torax
                            var id_hospital = db.Institutions.Where(z => z.CountryID == 15 && z.FullName.Contains("del Torax")).FirstOrDefault().ID;
                            AppendDataToExcel_CONSDATA(Languaje_, CountryID_, RegionID_, null, Convert.ToInt32(id_hospital), Month, SE, StartDate, EndDate, excelWorkBook, "CONSDATA_IRAG", 6, 1, excelWs_IRAG.Index, false, ReportCountry, YearEnd, YearEnd, 1, Inusual, AreaID_, Sentinel, 0, 0);

                            // SAN PEDRO SULA IHSS
                            id_hospital = db.Institutions.Where(z => z.CountryID == 15 && z.FullName.Contains("SULA IHSS")).FirstOrDefault().ID;
                            AppendDataToExcel_CONSDATA(Languaje_, CountryID_, RegionID_, null, Convert.ToInt32(id_hospital), Month, SE, StartDate, EndDate, excelWorkBook, "CONSDATA_IRAG", 6, 12, excelWs_IRAG.Index, false, ReportCountry, YearEnd, YearEnd, 1, Inusual, AreaID_, Sentinel, 0, 0);

                            // HOSPITAL MARIO CATARINO RIVAS
                            id_hospital = db.Institutions.Where(z => z.CountryID == 15 && z.FullName.Contains("Catarino Rivas")).FirstOrDefault().ID;
                            AppendDataToExcel_CONSDATA(Languaje_, CountryID_, RegionID_, null, Convert.ToInt32(id_hospital), Month, SE, StartDate, EndDate, excelWorkBook, "CONSDATA_IRAG", 6, 23, excelWs_IRAG.Index, false, ReportCountry, YearEnd, YearEnd, 1, Inusual, AreaID_, Sentinel, 0, 0);

                            //HOSPITAL MILITAR
                            id_hospital = db.Institutions.Where(z => z.CountryID == 15 && z.FullName.Contains("Militar")).FirstOrDefault().ID;
                            AppendDataToExcel_CONSDATA(Languaje_, CountryID_, RegionID_, null, Convert.ToInt32(id_hospital), Month, SE, StartDate, EndDate, excelWorkBook, "CONSDATA_IRAG", 6, 34, excelWs_IRAG.Index, false, ReportCountry, YearEnd, YearEnd, 1, Inusual, AreaID_, Sentinel, 0, 0);

                            //IHSS Tegucigalpa
                            id_hospital = db.Institutions.Where(z => z.CountryID == 15 && z.FullName.Contains("ESPECIALIDADES IHSS")).FirstOrDefault().ID;
                            AppendDataToExcel_CONSDATA(Languaje_, CountryID_, RegionID_, null, Convert.ToInt32(id_hospital), Month, SE, StartDate, EndDate, excelWorkBook, "CONSDATA_IRAG", 6, 45, excelWs_IRAG.Index, false, ReportCountry, YearEnd, YearEnd, 1, Inusual, AreaID_, Sentinel, 0, 0);

                            // Nacional
                            //AppendDataToExcel_CONSDATA(Languaje_, CountryID_, RegionID_, null, 0, Month, SE, StartDate, EndDate, excelWorkBook, "CONSDATA_IRAG", 6, 35, excelWs_IRAG.Index, false, ReportCountry, YearEnd, YearEnd, 1, Inusual, AreaID_, Sentinel, 0, 0);

                            // Leyendas
                            var excelWs_Leyendas = excelWorkBook.Worksheets["Leyendas"];
                            ConfigToExcel_FLUID(CountryID, Languaje_, RegionID_, Year, YearBegin, YearEnd, StartDate, EndDate, HospitalID_, Surv,  excelWorkBook, "Leyendas", 1, excelWs_Leyendas.Index, false);

                        }
                        else if (reportTemplate == "FM1")
                            AppendDataToExcel_FormSariIliHospDeath(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual, AreaID_);        //#### CAFQ
                        //else if ((reportTemplate == "R2" || reportTemplate == "R3" || reportTemplate == "R1") && bVariosAnios)
                        else if (reportTemplate == "R2" || reportTemplate == "R3" || reportTemplate == "R1")
                            AppendDataToExcel_R2_SeveralYears(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual, AreaID_);        //#### CAFQ: 180204
                        else
                        {
                            AppendDataToExcel(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual, AreaID_, CasosNPHL, Sentinel);        //#### CAFQ
                        }

                        excelPackage.SaveAs(ms);
                    }
                }

                ms.Position = 0;

                string nombFile = reportCountry.description == "" ? "Exportable_" : Country_Code + "_" + reportCountry.description.ToString().Replace("%", "_").Replace("/"," ").Replace(" ", "_") + "_";            //#### CAFQ

                return new FileStreamResult(ms, "application/xlsx")
                {
                    FileDownloadName = nombFile + DateTime.Now.ToString("yyyy_MM_dd_HH_mm") + ".xlsx"           //#### CAFQ

                };
            }
            catch (Exception e)
            {
                ViewBag.Message = "El reporte no se pudo generar, por favor intente de nuevo: " + e.Message;
            }

            return null;
        }

        private void AppendDataToExcel_FormSariIliHospDeath(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? AreaId)               //#### CAFQ
        {
            var excelWorksheet = excelWorkBook.Worksheets[sheet];
            var row = startRow;
            var column = startColumn;
            string _storedProcedure = "", consString = "";

            var user = UserManager.FindById(User.Identity.GetUserId());
            bool ili = user.Institution.ILI;
            bool sari = user.Institution.SARI;

            if (sari)
            {
                _storedProcedure = "R6";
                consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (var con = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand(_storedProcedure, con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1200 })    //**** CAFQ
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                        command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                        command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                        command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                        command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                        command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                        command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                        command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                        command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                        command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                        command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;                        //#### CAFQ
                        command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;          //#### CAFQ
                        command.Parameters.Add("@Area_ID", SqlDbType.Int).Value = AreaId;                        //#### CAFQ

                        con.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            var col = startColumn;
                            int nFiLe = 0;

                            var n1 = 0;
                            var n2 = 0;
                            var n3 = 0;
                            var n4 = 0;
                            var n5 = 0;
                            var n6 = 0;

                            while (reader.Read())
                            {
                                ++nFiLe;                                // Fila leida
                                //************************************ Hospitalizaciones Denominadores
                                for (int i = 0; i < 53; i++)
                                    n5 += (int)reader.GetValue(i);          // Masculino

                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n5 += (int)reader.GetValue(i);      // Femenino

                                //**** Hospitalizaciones SARI
                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n1 += (int)reader.GetValue(i);      // Masculino

                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n1 += (int)reader.GetValue(i);      // Masculino

                                //************************************ ICU Denominadores
                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n4 += (int)reader.GetValue(i);          // Masculino

                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n4 += (int)reader.GetValue(i);      // Femenino

                                //**** ICU SARI
                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n2 += (int)reader.GetValue(i);      // Masculino

                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n2 += (int)reader.GetValue(i);      // Masculino

                                //************************************ Death Denominadores
                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n6 += (int)reader.GetValue(i);          // Masculino

                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n6 += (int)reader.GetValue(i);      // Femenino

                                //**** Death SARI
                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n3 += (int)reader.GetValue(i);      // Masculino

                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n3 += (int)reader.GetValue(i);      // Masculino

                                //************************************
                                excelWorksheet.Cells[row + 0, col].Value = n1;      // Admissions SARI
                                excelWorksheet.Cells[row + 1, col].Value = n3;      // Deaths SARI
                                excelWorksheet.Cells[row + 2, col].Value = n2;      // ICU admissions SARI

                                excelWorksheet.Cells[row + 3, col].Value = n4;      // ICU Denominadores
                                excelWorksheet.Cells[row + 7, col].Value = n5;      // Hospital admissions Denominadores
                                excelWorksheet.Cells[row + 8, col].Value = n6;      // Deaths Denominadores
                                //****
                                ++col;
                                n1 = 0;
                                n2 = 0;
                                n3 = 0;
                                n4 = 0;
                                n5 = 0;
                                n6 = 0;
                                //****
                            }
                        }

                        command.Parameters.Clear();
                        con.Close();
                    }
                }

                //****
                row = startRow;
                column = startColumn;
                _storedProcedure = "FLUID_IRAG_Total_Muestra_Analizadas_AgeGroup";

                using (var con2 = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand(_storedProcedure, con2) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1200 })    //**** CAFQ
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                        command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                        command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                        command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                        command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                        command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                        command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                        command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                        command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                        command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                        command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;
                        command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;
                        command.Parameters.Add("@Area_ID", SqlDbType.Int).Value = AreaId;                        //#### CAFQ

                        con2.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            var col = startColumn;

                            while (reader.Read())
                            {
                                excelWorksheet.Cells[row + 4, col].Value = (int)reader.GetValue(2);
                                ++col;
                            }
                        }

                        command.Parameters.Clear();
                        con2.Close();
                    }
                }
            }       // End sari

            if (ili)
            {
                row = startRow;
                column = startColumn;
                _storedProcedure = "Summary_ILI_Cases_AgeGroup";

                using (var con2 = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand(_storedProcedure, con2) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1200 })    //**** CAFQ
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                        command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                        command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                        command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                        command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                        command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                        command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                        command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                        command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                        command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                        command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;
                        command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;
                        command.Parameters.Add("@Area_ID", SqlDbType.Int).Value = AreaId;                        //#### CAFQ

                        con2.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            var col = startColumn;
                            int nFiLe = 0;
                            int nGrup = 0;

                            int nCase1 = 0;
                            int nCase2 = 0;
                            int nCase3 = 0;
                            int nSamp1 = 0;
                            int nSamp2 = 0;
                            int nSamp3 = 0;
                            int nVisi1 = 0;
                            int nVisi2 = 0;
                            int nVisi3 = 0;

                            while (reader.Read())
                            {
                                ++nFiLe;
                                ++nGrup;

                                //for (int i = 0; i < 2; i++)
                                //{
                                if (nGrup == 1)
                                {
                                    nCase1 += (int)reader.GetValue(4);
                                    nSamp1 += (int)reader.GetValue(5);
                                    nVisi1 += (int)reader.GetValue(6);
                                }
                                else if (nGrup == 2)
                                {
                                    nCase2 += (int)reader.GetValue(4);
                                    nSamp2 += (int)reader.GetValue(5);
                                    nVisi2 += (int)reader.GetValue(6);
                                }
                                else if (nGrup == 3)
                                {
                                    nCase3 += (int)reader.GetValue(4);
                                    nSamp3 += (int)reader.GetValue(5);
                                    nVisi3 += (int)reader.GetValue(6);
                                }
                                //}

                                if (nFiLe % 3 == 0)
                                {
                                    nGrup = 0;
                                }
                            }
                            //****
                            row = row + 12;
                            excelWorksheet.Cells[row, col + 1].Value = nCase1;
                            excelWorksheet.Cells[row, col + 2].Value = nCase2;
                            excelWorksheet.Cells[row, col + 3].Value = nCase3;
                            ++row;
                            excelWorksheet.Cells[row, col + 1].Value = nSamp1;
                            excelWorksheet.Cells[row, col + 2].Value = nSamp2;
                            excelWorksheet.Cells[row, col + 3].Value = nSamp3;
                            ++row;
                            excelWorksheet.Cells[row, col + 1].Value = nVisi1;
                            excelWorksheet.Cells[row, col + 2].Value = nVisi2;
                            excelWorksheet.Cells[row, col + 3].Value = nVisi3;
                        }

                        command.Parameters.Clear();
                        con2.Close();
                    }
                }
            }       // End ili

            //****
            reportLabels(consString, countryId, languaje_, ReportCountry, hospitalId, year, YearFrom, YearTo, se, startDate, endDate, excelWorkBook, excelWorksheet, AreaId);
            //InsertarImagenLogo(consString, reportTemplate, ReportCountry, excelWorksheet);
        }

        static private void EnableMultilevelAxisLabels(ExcelChart myChartCC)
        {
            var chartXml = myChartCC.ChartXml;
            var nsm = new XmlNamespaceManager(chartXml.NameTable);

            var nsuri = chartXml.DocumentElement.NamespaceURI;
            nsm.AddNamespace("c", nsuri);

            //Get the Series ref and its cat
            var serNode = chartXml.SelectSingleNode("c:chartSpace/c:chart/c:plotArea/c:barChart/c:ser", nsm);
            var catNode = serNode.SelectSingleNode("c:cat", nsm);

            //Get Y axis reference to replace with multi level node
            var numRefNode = catNode.SelectSingleNode("c:numRef", nsm);
            var multiLvlStrRefNode = chartXml.CreateNode(XmlNodeType.Element, "c:multiLvlStrRef", nsuri);

            //Set the multi level flag
            var noMultiLvlLblNode = chartXml.CreateElement("c:noMultiLvlLbl", nsuri);
            var att = chartXml.CreateAttribute("val");
            att.Value = "0";
            noMultiLvlLblNode.Attributes.Append(att);

            var catAxNode = chartXml.SelectSingleNode("c:chartSpace/c:chart/c:plotArea/c:catAx", nsm);
            catAxNode.AppendChild(noMultiLvlLblNode);
        }

        static private void EnableMultilevelAxisLabels(ExcelChart myChartCC, string typeGraph)
        {
            var chartXml = myChartCC.ChartXml;
            var nsm = new XmlNamespaceManager(chartXml.NameTable);

            var nsuri = chartXml.DocumentElement.NamespaceURI;
            nsm.AddNamespace("c", nsuri);

            //Get the Series ref and its cat
            XmlNode serNode = null;
            if (typeGraph == "Line")
            {
                serNode = chartXml.SelectSingleNode("c:chartSpace/c:chart/c:plotArea/c:lineChart/c:ser", nsm);
            }
            else if (typeGraph == "ColumnStacked" || typeGraph == "ColumnStacked100" || typeGraph == "ColumnClustered" || typeGraph == "BarStacked") //  
            {
                serNode = chartXml.SelectSingleNode("c:chartSpace/c:chart/c:plotArea/c:barChart/c:ser", nsm);
            }
            else
            {
                //serNode = chartXml.SelectSingleNode("c:chartSpace/c:chart/c:plotArea/c:barChart/c:ser", nsm);
            }
            var catNode = serNode.SelectSingleNode("c:cat", nsm);

            //Get Y axis reference to replace with multi level node
            var numRefNode = catNode.SelectSingleNode("c:numRef", nsm);
            var multiLvlStrRefNode = chartXml.CreateNode(XmlNodeType.Element, "c:multiLvlStrRef", nsuri);

            //Set the multi level flag
            var noMultiLvlLblNode = chartXml.CreateElement("c:noMultiLvlLbl", nsuri);
            var att = chartXml.CreateAttribute("val");
            att.Value = "0";
            noMultiLvlLblNode.Attributes.Append(att);

            var catAxNode = chartXml.SelectSingleNode("c:chartSpace/c:chart/c:plotArea/c:catAx", nsm);
            catAxNode.AppendChild(noMultiLvlLblNode);
        }

        static private void SetChartPointsColor(ExcelChart chart, int serieNumber, Color color, int typeChart)
        {
            var chartXml = chart.ChartXml;

            var nsa = chart.WorkSheet.Drawings.NameSpaceManager.LookupNamespace("a");
            var nsuri = chartXml.DocumentElement.NamespaceURI;

            var nsm = new XmlNamespaceManager(chartXml.NameTable);
            nsm.AddNamespace("a", nsa);
            nsm.AddNamespace("c", nsuri);

            XmlNode serieNode = null;
            if (typeChart == (int)eChartType.Line)
            {
                serieNode = chart.ChartXml.SelectSingleNode(@"c:chartSpace/c:chart/c:plotArea/c:lineChart/c:ser[c:idx[@val='" + serieNumber + "']]", nsm);
            }
            else
            {
                serieNode = chart.ChartXml.SelectSingleNode(@"c:chartSpace/c:chart/c:plotArea/c:barChart/c:ser[c:idx[@val='" + serieNumber + "']]", nsm);
            }
            //var serieNode = chart.ChartXml.SelectSingleNode(@"c:chartSpace/c:chart/c:plotArea/c:barChart/c:ser[c:idx[@val='" + serieNumber + "']]", nsm);
            var serie = chart.Series[serieNumber];
            var points = serie.Series.Length;

            //Add reference to the color for the legend and data points
            var srgbClr = chartXml.CreateNode(XmlNodeType.Element, "srgbClr", nsa);
            var att = chartXml.CreateAttribute("val");
            att.Value = $"{color.R:X2}{color.G:X2}{color.B:X2}";
            srgbClr.Attributes.Append(att);

            var solidFill = chartXml.CreateNode(XmlNodeType.Element, "solidFill", nsa);
            solidFill.AppendChild(srgbClr);

            var spPr = chartXml.CreateNode(XmlNodeType.Element, "spPr", nsuri);
            spPr.AppendChild(solidFill);

            serieNode.AppendChild(spPr);
        }

        //**** R1: Reporte Numero de casos y %..., R2: Total fallecidos por IRAG y R3: Casos por IRAG y Grupos...
        private static void AppendDataToExcel_R2_SeveralYears(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, 
            int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, 
            int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? AreaId)               //#### CAFQ
        {
            int _YearFrom, _YearTo;
            if (YearFrom != null && YearTo != null)
            {
                _YearFrom = YearFrom.Value;
                _YearTo = YearTo.Value;
            }
            else
            {
                _YearFrom = year.Value;
                _YearTo = year.Value;
            }

            var excelWorksheet = excelWorkBook.Worksheets[sheet];
            int nI = 0;
            int nColumns = 0;           // Columnas en la plantilla
            string label = (string)excelWorksheet.Cells[startRow - 1, startColumn].Value;
            while (label != "" && label != null)
            {
                ++nColumns;
                label = (string)excelWorksheet.Cells[startRow - 1, startColumn + nColumns].Value;
            };
            int nAnios = _YearTo - _YearFrom + 1;
            //int nToSe = 0;                                          // Total semanas del reporte
            List<int> weekByYear = new List<int>();                 // Semanas por anio
            for (nI = _YearFrom; nI <= _YearTo; ++nI)
            {
                //nToSe += 53;            //????
                weekByYear.Add(PAHOClassUtilities.semanasAnioEpidemiologico(nI));
            }

            //var xxx = excelWorksheet.Drawings;                // Coleccion de charts
            //var yyy = excelWorksheet.Drawings[0];             // Un chart especifico
            if (storedProcedure == "R2" || storedProcedure == "R3" || storedProcedure == "R1")
                excelWorksheet.Drawings.Remove(0);              // Eliminando grafico por defecto en plantilla

            //**** Adecuando la plantilla para varios anios
            int row = 0;
            if (storedProcedure == "R1" || storedProcedure == "R3" || storedProcedure == "R2") 
            {                
                for (nI = 1; nI <= nAnios - 1; ++nI)
                {
                    row = startRow + (nI - 1) * 54;
                    excelWorksheet.Cells[row, startColumn, row + 54 - 1, startColumn + nColumns - 1].Copy(excelWorksheet.Cells[row + 54, startColumn]);
                }
            }

            //****
            //string cLabelAxixY = "";
            //if (storedProcedure == "R2")
            //    cLabelAxixY = (string)excelWorksheet.Cells[startRow - 1, startColumn + 1].Value;        // Label eje Y

            row = startRow;
            var column = startColumn;

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            //int nIndice = -1;
            for (nI = _YearFrom; nI <= _YearTo; ++nI)
            {
                //++nIndice;
                if (storedProcedure == "R2xxx")
                {
                    //row = startRow;
                    //var col = startColumn + (nI - _YearFrom) + 1;

                    //if (nI > _YearFrom)
                    //{
                    //    excelWorksheet.Cells[row - 1, startColumn + 1].Copy(excelWorksheet.Cells[row - 1, col]);
                    //    excelWorksheet.Cells[row - 1, col].Value = nI;          // Year

                    //    excelWorksheet.Cells[startRow + 53, startColumn + 1].Copy(excelWorksheet.Cells[startRow + 53, col]);                // Total
                    //    excelWorksheet.Cells[startRow + 53, col].StyleID = excelWorksheet.Cells[startRow + 53, startColumn + 1].StyleID;

                    //    excelWorksheet.Cells[startRow + 52, col].StyleID = excelWorksheet.Cells[startRow + 52, startColumn + 1].StyleID;    // Fila 53
                    //}
                    //else
                    //{
                    //    excelWorksheet.Cells[row - 1, col].Value = nI;          // Year
                    //}
                }
                else if (storedProcedure == "R1" || storedProcedure == "R3" || storedProcedure == "R2")
                {
                    row = startRow + (nI - _YearFrom) * (53 + 1);
                }

                using (var con = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand(storedProcedure, con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1200 })    //**** CAFQ
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                        command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                        command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                        command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                        command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                        command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                        command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                        command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                        command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = nI;
                        command.Parameters.Add("@yearTo", SqlDbType.Int).Value = nI;
                        command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;                        //#### CAFQ
                        command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;          //#### CAFQ
                        con.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            nColumns = (storedProcedure == "R3") ? ((reader.FieldCount - 1) / 2) + 1 : reader.FieldCount;

                            while (reader.Read())
                            {
                                var col = column;
                                var cell = excelWorksheet.Cells[row, col];

                                if (row > startRow && insert_row == true)
                                    excelWorksheet.InsertRow(row, 1);

                                //if (storedProcedure == "R2")
                                //    col = column + (nI - _YearFrom) + ((nI == _YearFrom) ? 0 : 1);

                                for (var i = 0; i < nColumns; i++)
                                {
                                    //if (storedProcedure == "R2")
                                    //{
                                    //    if (nI > _YearFrom && i > 0)
                                    //        --col;
                                    //}

                                    if (reader.GetValue(i) != null)
                                    {
                                        double numberD;
                                        bool isNumber = double.TryParse(reader.GetValue(i).ToString(), out numberD);

                                        DateTime dt;
                                        bool isDate = DateTime.TryParse(reader.GetValue(i).ToString(), out dt);

                                        if (isNumber)
                                        {
                                            excelWorksheet.Cells[row, col].Value = numberD;
                                        }
                                        else
                                        {
                                            if (isDate)
                                            {
                                                excelWorksheet.Cells[row, col].Value = dt;
                                            }
                                            else
                                            {
                                                excelWorksheet.Cells[row, col].Value = reader.GetValue(i).ToString();
                                            }
                                        }
                                        excelWorksheet.Cells[row, col].StyleID = cell.StyleID;
                                    }
                                    col++;
                                }

                                row++;
                            }
                        }

                        command.Parameters.Clear();
                        con.Close();
                    }
                }
            }   // End for

            int nTemp = 0;
            //**** Preparando area de datos para grafico
            row = startRow;
            List<int> aRI = new List<int>();
            List<int> aRF = new List<int>();

            for (nI = 1; nI <= weekByYear.Count; ++nI)
            {
                if(nAnios > 1 && (storedProcedure == "R1" || storedProcedure == "R3" || storedProcedure == "R2"))
                    excelWorksheet.Cells[row, 1].Value = _YearFrom + (nI - 1);      // Anio

                aRI.Add(row);
                row = row + 54 - 2;
                nTemp = weekByYear[nI - 1];
                if (nTemp == 52)
                {
                    if (storedProcedure == "R1" || storedProcedure == "R3" || storedProcedure == "R2")
                        excelWorksheet.DeleteRow(row, 1, true);     // Eliminar desde la fila row 
                    aRF.Add(row - 1);
                    ++row;
                }
                else
                {
                    aRF.Add(row);
                    row = row + 2;
                }
            }

            //**** INSERTAR GRAFICO
            if (storedProcedure == "R2")        // Total fallecidos por IRAG
            {
                //var myChartCC = excelWorksheet.Drawings.AddChart("ChartColumnClustered", eChartType.ColumnClustered);

                //for (nI = _YearFrom; nI <= _YearTo; ++nI)
                //{
                //    int nCol = startColumn + (nI - _YearFrom) + 1;
                //    var seriesCC = myChartCC.Series.Add(ExcelRange.GetAddress(7, nCol, 59, nCol), ExcelRange.GetAddress(7, 2, 59, 2));
                //    seriesCC.Header = nI.ToString();
                //}

                ////myChartCC.Border.Fill.Color = System.Drawing.Color.Red;
                //if (languaje_ == "ENG")
                //    myChartCC.Title.Text = "NUMBERS OF DEATHS BY EPIDEMIOLOGICAL WEEK";
                //else
                //    myChartCC.Title.Text = "NÚMERO DE FALLECIDOS POR SEMANA EPIDEMIOLÓGICA";
                //myChartCC.Title.Font.Bold = true;
                //myChartCC.Legend.Position = eLegendPosition.Bottom;
                //myChartCC.SetSize(920, 400);                    // Ancho, Alto in pixel
                //myChartCC.SetPosition(startRow - 2, 0, (startColumn + (_YearTo - _YearFrom) + 1), 40);             // (int row, int rowoffset in pixel, int col, int coloffset in pixel)

                //myChartCC.YAxis.Title.Text = cLabelAxixY;
                //myChartCC.YAxis.Title.Font.Size = 9;
                //myChartCC.YAxis.Title.Font.Bold = true;

                //myChartCC.XAxis.Title.Text = (string)excelWorksheet.Cells[startRow - 1, startColumn].Value;    // Semana epidemiologica
                //myChartCC.XAxis.Title.Font.Size = 9;
                //myChartCC.XAxis.Title.Font.Bold = true;
                int nCol = startColumn;
                //**** Rangos de las series
                string cvalues = "", cxvalues = "";  //, cvaluesLI = "", cxvaluesLI = "";
                for (nI = 1; nI <= aRI.Count; ++nI)
                {
                    cvalues = cvalues + excelWorksheet.Cells[aRI[nI - 1], 3, aRF[nI - 1], 3].Address + ",";
                    if (nAnios > 1)
                        cxvalues = cxvalues + excelWorksheet.Cells[aRI[nI - 1], 1, aRF[nI - 1], 2].Address + ",";
                    else
                        cxvalues = cxvalues + excelWorksheet.Cells[aRI[nI - 1], 2, aRF[nI - 1], 2].Address + ",";

                    //cvaluesLI = cvaluesLI + excelWorksheet.Cells[aRI[nI - 1], 5, aRF[nI - 1], 5].Address + ",";
                    //if (nAnios > 1)
                    //    cxvaluesLI = cxvaluesLI + excelWorksheet.Cells[aRI[nI - 1], 1, aRF[nI - 1], 2].Address + ",";
                    //else
                    //    cxvaluesLI = cxvaluesLI + excelWorksheet.Cells[aRI[nI - 1], 2, aRF[nI - 1], 2].Address + ",";
                }

                //**** Chart principal
                cvalues = cvalues.Substring(0, cvalues.Length - 1);
                cxvalues = cxvalues.Substring(0, cxvalues.Length - 1);
                ExcelRange values = excelWorksheet.Cells[cvalues];
                ExcelRange xvalues = excelWorksheet.Cells[cxvalues];

                var myChartCC = excelWorksheet.Drawings.AddChart("ColumnStackedLine1", eChartType.ColumnClustered);
                var serieCC = myChartCC.Series.Add(values, xvalues);                            // Valores de la serie, Etiquetas del eje X
                serieCC.Header = (string)excelWorksheet.Cells[startRow - 1, nCol + 1].Value;    // Leyenda

                myChartCC.XAxis.Title.Text = (string)excelWorksheet.Cells[startRow - 1, nCol].Value;
                myChartCC.XAxis.Title.Font.Size = 9;
                myChartCC.XAxis.Title.Font.Bold = true;
                myChartCC.XAxis.MajorTickMark = eAxisTickMark.Out;
                myChartCC.XAxis.MinorTickMark = eAxisTickMark.None;

                myChartCC.YAxis.Title.Text = (string)excelWorksheet.Cells[startRow - 1, nCol + 1].Value;
                myChartCC.YAxis.Title.Font.Size = 9;
                myChartCC.YAxis.Title.Font.Bold = true;

                //**** Chart secundario 
                //cvaluesLI = cvaluesLI.Substring(0, cvaluesLI.Length - 1);
                //cxvaluesLI = cxvaluesLI.Substring(0, cxvaluesLI.Length - 1);
                //ExcelRange valuesLI = excelWorksheet.Cells[cvaluesLI];
                //ExcelRange xvaluesLI = excelWorksheet.Cells[cxvaluesLI];

                //var myChartLI = myChartCC.PlotArea.ChartTypes.Add(eChartType.Line);
                //var serieLI = myChartLI.Series.Add(valuesLI, xvaluesLI);
                //serieLI.Header = (string)excelWorksheet.Cells[startRow - 1, nCol + 3].Value;    // Leyenda;

                //myChartLI.UseSecondaryAxis = true;
                //myChartLI.YAxis.Title.Text = (string)excelWorksheet.Cells[startRow - 1, nCol + 3].Value;
                //myChartLI.YAxis.Title.Font.Size = 9;
                //myChartLI.YAxis.Title.Font.Bold = true;

                //myChartLI.XAxis.MinorUnit = 1;
                ////myChartLI.XAxis.MajorTickMark = eAxisTickMark.Out;
                ////myChartLI.XAxis.MinorTickMark = eAxisTickMark.None;

                //****                
                //myChartCC.Title.Text = SgetMsg("msgReportNumeroCasosIRAGGraphTitle", countryId, languaje_) + ": " + _YearFrom.ToString() + (nAnios == 1 ? "" : " - " + _YearTo.ToString());
                if (languaje_ == "ENG")
                    myChartCC.Title.Text = "NUMBERS OF DEATHS BY EPIDEMIOLOGICAL WEEK";
                else
                    myChartCC.Title.Text = "NÚMERO DE FALLECIDOS POR SEMANA EPIDEMIOLÓGICA";

                myChartCC.Title.Font.Bold = true;
                myChartCC.SetSize(900 + (nAnios - 1) * 300, 450);
                myChartCC.SetPosition(startRow - 2, 0, 3, 40);
                myChartCC.Legend.Position = eLegendPosition.Bottom;
                myChartCC.Legend.Font.Bold = true;

                //**** Habilitando Multi-Level Category Labels
                if (nAnios > 1)
                    EnableMultilevelAxisLabels(myChartCC, "ColumnClustered");
            }
            else if (storedProcedure == "R3")           // CASOS POR IRAG POR GRUPOS DE EDAD Y SE
            {
                int nCol = startColumn;

                var myChartCC = excelWorksheet.Drawings.AddChart("ColumnStacked" + "1", eChartType.ColumnStacked);

                //**** Rangos de las series
                string cvalues = "", cxvalues = ""; //, cvaluesLI = "", cxvaluesLI = "";
                for (int nJ = 1; nJ <= nColumns; ++nJ)
                {
                    //**** Rangos de las series
                    cvalues = "";
                    cxvalues = "";
                    for (nI = 1; nI <= aRI.Count; ++nI)
                    {
                        cvalues = cvalues + excelWorksheet.Cells[aRI[nI - 1], nCol + nJ, aRF[nI - 1], nCol + nJ].Address + ",";
                        if (nAnios > 1)
                            cxvalues = cxvalues + excelWorksheet.Cells[aRI[nI - 1], 1, aRF[nI - 1], 2].Address + ",";
                        else
                            cxvalues = cxvalues + excelWorksheet.Cells[aRI[nI - 1], 2, aRF[nI - 1], 2].Address + ",";
                    }

                    //**** Chart principal
                    cvalues = cvalues.Substring(0, cvalues.Length - 1);
                    cxvalues = cxvalues.Substring(0, cxvalues.Length - 1);
                    ExcelRange values = excelWorksheet.Cells[cvalues];
                    ExcelRange xvalues = excelWorksheet.Cells[cxvalues];

                    //var myChartCC = excelWorksheet.Drawings.AddChart("ColumnStackedLine1", eChartType.ColumnClustered);
                    var serieCC = myChartCC.Series.Add(values, xvalues);                            // Valores de la serie, Etiquetas del eje X
                    serieCC.Header = (string)excelWorksheet.Cells[startRow - 1, nCol + nJ].Value;    // Leyenda
                }

                myChartCC.Title.Text = SgetMsg("msgReportCasosIRAGByAGGraphTitle", countryId, languaje_) + ": " + _YearFrom.ToString() + (nAnios == 1 ? "" : " - " + _YearTo.ToString());
                myChartCC.Title.Font.Bold = true;
                

                myChartCC.SetSize(900 + (nAnios - 1) * 300, 450);      //1090
                myChartCC.SetPosition(startRow + (23 * 0) - 1, 0, nColumns + 1, 40);
                myChartCC.Legend.Position = eLegendPosition.Bottom;

                myChartCC.YAxis.Title.Text = SgetMsg("msgReportNumberSARICases", countryId, languaje_);
                myChartCC.YAxis.Title.Font.Size = 9;
                myChartCC.YAxis.Title.Font.Bold = true;

                myChartCC.XAxis.Title.Text = (string)excelWorksheet.Cells[startRow - 1, startColumn].Value;    // Semana epidemiologica
                myChartCC.XAxis.Title.Font.Size = 9;
                myChartCC.XAxis.Title.Font.Bold = true;
                myChartCC.XAxis.MajorTickMark = eAxisTickMark.Out;
                myChartCC.XAxis.MinorTickMark = eAxisTickMark.None;

                //**** Habilitando Multi-Level Category Labels
                if (nAnios > 1)
                    EnableMultilevelAxisLabels(myChartCC, "ColumnStacked");
            }
            else if (storedProcedure == "R1")       // Número de casos y % de hospitalizaciones por IRAG
            {
                int nCol = startColumn;
                //**** Rangos de las series
                string cvalues="", cxvalues = "", cvaluesLI = "", cxvaluesLI = "";
                for (nI = 1; nI <= aRI.Count; ++nI)
                {
                    cvalues = cvalues + excelWorksheet.Cells[aRI[nI - 1], 3, aRF[nI - 1], 3].Address + ",";
                    if (nAnios > 1)
                        cxvalues = cxvalues + excelWorksheet.Cells[aRI[nI - 1], 1, aRF[nI - 1], 2].Address + ",";
                    else
                        cxvalues = cxvalues + excelWorksheet.Cells[aRI[nI - 1], 2, aRF[nI - 1], 2].Address + ",";

                    cvaluesLI = cvaluesLI + excelWorksheet.Cells[aRI[nI - 1], 5, aRF[nI - 1], 5].Address + ",";
                    if (nAnios > 1)
                        cxvaluesLI = cxvaluesLI + excelWorksheet.Cells[aRI[nI - 1], 1, aRF[nI - 1], 2].Address + ",";
                    else
                        cxvaluesLI = cxvaluesLI + excelWorksheet.Cells[aRI[nI - 1], 2, aRF[nI - 1], 2].Address + ",";
                }

                //**** Chart principal
                cvalues = cvalues.Substring(0, cvalues.Length - 1);
                cxvalues = cxvalues.Substring(0, cxvalues.Length - 1);
                ExcelRange values = excelWorksheet.Cells[cvalues]; 
                ExcelRange xvalues = excelWorksheet.Cells[cxvalues];

                var myChartCC = excelWorksheet.Drawings.AddChart("ColumnStackedLine1", eChartType.ColumnClustered);
                var serieCC = myChartCC.Series.Add(values, xvalues);                            // Valores de la serie, Etiquetas del eje X
                serieCC.Header = (string)excelWorksheet.Cells[startRow - 1, nCol + 1].Value;    // Leyenda

                myChartCC.XAxis.Title.Text = (string)excelWorksheet.Cells[startRow - 1, nCol].Value;
                myChartCC.XAxis.Title.Font.Size = 9;
                myChartCC.XAxis.Title.Font.Bold = true;
                myChartCC.XAxis.MajorTickMark = eAxisTickMark.Out;
                myChartCC.XAxis.MinorTickMark = eAxisTickMark.None;

                myChartCC.YAxis.Title.Text = (string)excelWorksheet.Cells[startRow - 1, nCol + 1].Value;
                myChartCC.YAxis.Title.Font.Size = 9;
                myChartCC.YAxis.Title.Font.Bold = true;

                //**** Chart secundario 
                cvaluesLI = cvaluesLI.Substring(0, cvaluesLI.Length - 1);
                cxvaluesLI = cxvaluesLI.Substring(0, cxvaluesLI.Length - 1);
                ExcelRange valuesLI = excelWorksheet.Cells[cvaluesLI];
                ExcelRange xvaluesLI = excelWorksheet.Cells[cxvaluesLI];

                var myChartLI = myChartCC.PlotArea.ChartTypes.Add(eChartType.Line);
                var serieLI = myChartLI.Series.Add(valuesLI, xvaluesLI);
                serieLI.Header = (string)excelWorksheet.Cells[startRow - 1, nCol + 3].Value;    // Leyenda;

                myChartLI.UseSecondaryAxis = true;
                myChartLI.YAxis.Title.Text = (string)excelWorksheet.Cells[startRow - 1, nCol + 3].Value;
                myChartLI.YAxis.Title.Font.Size = 9;
                myChartLI.YAxis.Title.Font.Bold = true;

                myChartLI.XAxis.MinorUnit = 1;
                //myChartLI.XAxis.MajorTickMark = eAxisTickMark.Out;
                //myChartLI.XAxis.MinorTickMark = eAxisTickMark.None;

                //****                
                myChartCC.Title.Text = SgetMsg("msgReportNumeroCasosIRAGGraphTitle", countryId, languaje_) + ": " + _YearFrom.ToString() + (nAnios == 1 ? "" : " - " + _YearTo.ToString());
                myChartCC.Title.Font.Bold = true;
                myChartCC.SetSize(900 + (nAnios - 1) * 300, 450);
                myChartCC.SetPosition(startRow - 2, 0, 5, 40);
                myChartCC.Legend.Position = eLegendPosition.Bottom;
                myChartCC.Legend.Font.Bold = true;

                //**** Habilitando Multi-Level Category Labels
                if (nAnios > 1)
                    EnableMultilevelAxisLabels(myChartCC, "ColumnClustered");
            }

            //**** inserción de labels y logo en el Excel
            if (ReportCountry != null)
            {
                //inserción de labels
                reportLabels(consString, countryId, languaje_, ReportCountry, hospitalId, year, YearFrom, YearTo, se, startDate, endDate, excelWorkBook, excelWorksheet);

                //inserción de logo
                InsertarLogoExcel(consString, excelWorksheet, ReportCountry);
            }
        }
        
        //**** Reporte epidemiologico
        private static void AppendDataToExcel_R6_SeveralYears(string languaje_, int countryId, int? regionId, int? year,
            int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook,
            string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry,
            int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? AreaId, int? Sentinel = null)
        {
            string cTemp = "";
            int _YearFrom, _YearTo;
            _YearFrom = YearFrom.Value;
            _YearTo = YearTo.Value;

            ExcelWorksheet wsPara = excelWorkBook.Worksheets["Parameters"];
            ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[sheet];
            int nI = 0;
            int nAnios = _YearTo - _YearFrom + 1;
            List<int> weekByYear = new List<int>();                 // Semanas por anio

            //**** Preparando plantillas
            int _startColumn = startColumn;
            int row = startRow;
            int col = _startColumn;
            int nGrEd = PAHOClassUtilities.getNumberAgeGroupCountry(countryId);

            //---- Tabla 1
            int widthT1 = 57;
            for (nI = 1; nI <= nAnios; ++nI)
            {
                int nAnio = _YearFrom + (nI - 1);
                //**** Semanas x anio
                weekByYear.Add(PAHOClassUtilities.semanasAnioEpidemiologico(nAnio));

                //**** Campos data para el anio nI
                col = _startColumn + (nI - 1) * widthT1;
                if (nI > 1)
                {
                    excelWorksheet.Cells[startRow - 3, _startColumn, startRow + 27 * (nGrEd + 1) - 2, _startColumn + widthT1 - 1].Copy(excelWorksheet.Cells[startRow - 3, col]);
                }
                excelWorksheet.Cells[startRow - 4, col].Value = nAnio;
            }
            //---- Tabla 2
            int widthT2 = 57;
            row = (startRow - 1) + (3 * 3 * 3 * (nGrEd + 1)) - 1;           // Ultima fila tabla 1
            int rowIT2 = row + 11 + 1;                                      // Tabla 2: fila inicio 
            int rowFT2 = rowIT2 + (((3 * 4 * nGrEd) + 3) * 14) - 1;

            for (nI = 1; nI <= nAnios; ++nI)
            {
                int nAnio = _YearFrom + (nI - 1);

                //---- Campos data para el anio nI
                col = _startColumn + (nI - 1) * widthT2;
                if (nI > 1)
                {
                    excelWorksheet.Cells[rowIT2 - 3, _startColumn, rowFT2 + 4, _startColumn + widthT2 - 1].Copy(excelWorksheet.Cells[rowIT2 - 3, col]);

                    //---- Ancho de columnas
                    for (int nJ = col; nJ < col + widthT2; nJ++)
                    {
                        if (nJ == col)
                            excelWorksheet.Column(nJ).Width = 6;            // 5.29
                        else if (nJ > col + widthT2 - 5)
                            excelWorksheet.Column(nJ).Width = 12;         // 40 px
                        else
                            excelWorksheet.Column(nJ).Width = 5.8;          // 40 px
                    }
                }
                excelWorksheet.Cells[rowIT2 - 3, col].Value = nAnio;

            }

            //---- Antecedentes
            cTemp = (string)wsPara.Cells[79, 2].Value;
            row = Convert.ToInt32(wsPara.Cells[79, 3].Value);
            col = Convert.ToInt32(wsPara.Cells[79, 4].Value);
            ExcelWorksheet wsTemp = excelWorkBook.Worksheets[cTemp];
            int widthYear_AN = 2 * 3 * 3;

            for (nI = 1; nI <= nAnios; ++nI)
            {
                int nAnio = _YearFrom + (nI - 1);

                //**** Campos data para el anio nI
                int colD = col + (nI - 1) * widthYear_AN;
                if (nI > 1)
                {
                    wsTemp.Cells[row - 5, col, row + 41, col + widthYear_AN - 1].Copy(wsTemp.Cells[row - 5, colD]);
                    for (int nW = 1; nW <= widthYear_AN; nW++)
                        wsTemp.Column(colD + (nW - 1)).Width = 6;
                }
                wsTemp.Cells[row - 5, colD].Value = nAnio;
                //**** Background
                if((nI % 3) == 1)
                {
                    wsTemp.Cells[row - 5, colD, row + 41, colD + widthYear_AN - 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsTemp.Cells[row - 5, colD, row + 41, colD + widthYear_AN - 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(133, 193, 233));
                }                    
                else if((nI % 3) == 2)
                {
                    wsTemp.Cells[row - 5, colD, row + 41, colD + widthYear_AN - 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsTemp.Cells[row - 5, colD, row + 41, colD + widthYear_AN - 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 203, 167));
                }                    
                else if ((nI % 3) == 0)
                {
                    wsTemp.Cells[row - 5, colD, row + 41, colD + widthYear_AN - 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //wsTemp.Cells[row - 5, colD, row + 41, colD + widthYear_AN - 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(171, 235, 198));
                    wsTemp.Cells[row - 5, colD, row + 41, colD + widthYear_AN - 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(163, 228, 215));
                }                    
                //****
            }

            //---- Hoja: T1 % hosp. UCI fall.
            cTemp = (string)wsPara.Cells[73, 2].Value;
            row = Convert.ToInt32(wsPara.Cells[73, 3].Value);
            col = Convert.ToInt32(wsPara.Cells[73, 4].Value);
            wsTemp = excelWorkBook.Worksheets[cTemp];
            int colW = 3;
            int srTA = (startRow - 1) + (27 * nGrEd);
            //int widthYear_TA = 54;
            int widthYear_TA = widthT1;            
            int heightYear_WK = 53;
            int nCoWiCopy = colW;

            R6_ActualizarFormulaCeldasTablas(excelWorksheet, wsPara, wsTemp, nAnios, colW, row, col, srTA, startColumn, 6, 9, startRow - 3, widthYear_TA, 
                heightYear_WK, nCoWiCopy, true);

            //---- T2 SE grav. edad
            cTemp = (string)wsPara.Cells[74, 2].Value;
            row = Convert.ToInt32(wsPara.Cells[74, 3].Value);
            col = Convert.ToInt32(wsPara.Cells[74, 4].Value);
            wsTemp = excelWorkBook.Worksheets[cTemp];
            colW = nGrEd * 3;
            srTA = (startRow - 1);
            //widthYear_TA = 54;
            widthYear_TA = widthT1;
            heightYear_WK = 53;
            nCoWiCopy = colW;

            R6_ActualizarFormulaCeldasTablas(excelWorksheet, wsPara, wsTemp, nAnios, colW, row, col, srTA, startColumn, 6, 9, startRow - 3, widthYear_TA, heightYear_WK, nCoWiCopy, true);

            //---- T4 v.influ SE
            cTemp = (string)wsPara.Cells[76, 2].Value;
            row = Convert.ToInt32(wsPara.Cells[76, 3].Value);
            col = Convert.ToInt32(wsPara.Cells[76, 4].Value);
            wsTemp = excelWorkBook.Worksheets[cTemp];                                   // Worksheet de trabajo (WK)
            colW = 6;                                                                   // Columnas de datos en WK
            srTA = (startRow - 1) + (27 * (nGrEd + 1)) + 11;                            // nGrEd = 6 => 208 (Influenza A(H1N1)pdm09) en TA
            //widthYear_TA = 57;
            widthYear_TA = widthT2;
            int height2_TA = 3 * 4 * nGrEd + 3;
            heightYear_WK = 54;                         // EW + Total   
            nCoWiCopy = colW + 3;

            R6_ActualizarFormulaCeldasTablas(excelWorksheet, wsPara, wsTemp, nAnios, colW, row, col, srTA, startColumn, 0, height2_TA, srTA - 2, widthYear_TA, heightYear_WK, nCoWiCopy, true);

            col = col + 6;                              // Columna Num. Muestras analizadas en WK
            colW = 1;
            srTA = (startRow - 1) + (27 * (nGrEd + 1)) + 11 + (height2_TA * 11);        // nGrEd = 6 => 1033 (# Muestras analizadas)
            R6_ActualizarFormulaCeldasTablas(excelWorksheet, wsPara, wsTemp, nAnios, colW, row, col, srTA, startColumn, 0, height2_TA, srTA - 2, widthYear_TA, heightYear_WK, nCoWiCopy, false);

            //---- T5 VR SE
            cTemp = (string)wsPara.Cells[77, 2].Value;
            row = Convert.ToInt32(wsPara.Cells[77, 3].Value);
            col = Convert.ToInt32(wsPara.Cells[77, 4].Value);
            wsTemp = excelWorkBook.Worksheets[cTemp];
            colW = 13;
            srTA = (startRow - 1) + (27 * (nGrEd + 1)) + 11;
            //widthYear_TA = 57;
            widthYear_TA = widthT2;
            height2_TA = 3 * 4 * nGrEd + 3;
            heightYear_WK = 54;         // EW + Total   
            nCoWiCopy = colW + 2;

            R6_ActualizarFormulaCeldasTablas(excelWorksheet, wsPara, wsTemp, nAnios, colW, row, col, srTA, startColumn, 0, height2_TA, srTA - 2, widthYear_TA, heightYear_WK, nCoWiCopy, true);

            //**** Recuperando data desde SQL Server
            int starRowAntec = 11;
            int starColAntec = 2;
            int colT2 = 0;

            for (nI = 1; nI <= nAnios; ++nI)
            {
                int nAnio = _YearFrom + (nI - 1);
                //col = _startColumn + (nI - 1) * 54;
                //colT2 = _startColumn + (nI - 1) * 57;               // Se enviara a traves del parametro CasosNPHL
                col = _startColumn + (nI - 1) * widthT1;
                colT2 = _startColumn + (nI - 1) * widthT2;               // Se enviara a traves del parametro CasosNPHL
                int colAntec = starColAntec + (nI - 1) * 18;

                AppendDataToExcel(languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, excelWorkBook,
                    storedProcedure, startRow, col, sheet, insert_row, ReportCountry, nAnio, nAnio, Surv, SurvInusual,
                    AreaId, colT2.ToString(), Sentinel, starRowAntec, colAntec);
            }

            //############################# Grafico hoja: "G1 %IRAG"
            Dictionary<string, decimal> aPara = new Dictionary<string, decimal>();
            cTemp = (string)wsPara.Cells[66, 2].Value;
            //ExcelWorksheet wsHoG1 = excelWorkBook.Worksheets["G1 %IRAG"];
            ExcelWorksheet wsHoG1 = excelWorkBook.Worksheets[cTemp];

            //---- Elimando los graficos de plantilla
            for (int nX = wsHoG1.Drawings.Count - 1; nX >= 0; nX--)
            {
                wsHoG1.Drawings.Remove(nX);
            }

            //---- Grafico Resumen (todos los AG)
            int _startRow = startRow - 1;

            aPara.Add("ChartTitleSize", 11);
            aPara.Add("AxisTitleYSize", 11);
            aPara.Add("AxisTitleXSize", 11);
            aPara.Add("AxisYSize", 10);
            aPara.Add("AxisXSize", 10);
            aPara.Add("LegendSize", 11);
            int width0 = 900;
            int hight0 = 520;
            int row0 = 4;
            int col0 = 0;

            string enPersona = "";
            int rowWeek_T1 = _startRow - 2;
            row = _startRow + nGrEd * 27;
            R6_GraficoPorcentajeIRAG_Line(excelWorksheet, wsPara, wsHoG1, row, rowWeek_T1, nAnios, _startColumn, weekByYear,
                "G1_0", width0, hight0, row0, col0, aPara, enPersona);

            //---- Grafico por cada AgeGroup
            aPara["ChartTitleSize"] = 10;
            aPara["AxisTitleYSize"] = 12;
            aPara["AxisTitleXSize"] = 12;
            aPara["AxisYSize"] = 9;
            aPara["AxisXSize"] = 9;
            aPara["LegendSize"] = (decimal)8.5;
            row0 = 14;

            for (nI = 1; nI <= nGrEd; nI++)
            {
                row = _startRow + (nI - 1) * 27;                // Fila inicio del enesimo AgeGroup
                width0 = 450;
                hight0 = 400;

                if (nI % 2 == 0)
                {
                    col0 = 8 + (nAnios - 2) * 2;
                }
                else
                {
                    col0 = 0;
                    row0 = row0 + 25;
                }

                enPersona = (string)wsPara.Cells[61, 2].Value;
                if (nI == 1)
                {
                    if (enPersona.Substring(enPersona.Length - 2) == "de")   // RIGHT
                    {
                        enPersona = enPersona.Substring(0, enPersona.Length - 3);
                    }
                }

                R6_GraficoPorcentajeIRAG_Line(excelWorksheet, wsPara, wsHoG1, row, rowWeek_T1, nAnios, _startColumn, weekByYear,
                    "G1_" + nI.ToString(), width0, hight0, row0, col0, aPara, enPersona);
            }

            //############################# Grafico hoja: "G2 Influenza"
            cTemp = (string)wsPara.Cells[67, 2].Value;
            //ExcelWorksheet wsHoG2 = excelWorkBook.Worksheets["G2 Influenza"];
            ExcelWorksheet wsHoG2 = excelWorkBook.Worksheets[cTemp];
            R6_GraficoVirus_ColumnStacked(excelWorksheet, wsPara, wsHoG2, rowIT2, rowFT2, nGrEd, nAnios, _startColumn, weekByYear, "G2_0");

            //############################# Grafico hoja: "G3 Todos virus"
            cTemp = (string)wsPara.Cells[68, 2].Value;
            //ExcelWorksheet wsHoG3 = excelWorkBook.Worksheets["G3 Todos virus"];
            ExcelWorksheet wsHoG3 = excelWorkBook.Worksheets[cTemp];
            R6_GraficoVirus_ColumnStacked(excelWorksheet, wsPara, wsHoG3, rowIT2, rowFT2, nGrEd, nAnios, _startColumn, weekByYear, "G3_0");


            //############################# Grafico hoja: "G4 Grupos Edad"
            //---- Tabla: T6 Tipo virus edad grav (Age Group)
            cTemp = (string)wsPara.Cells[78, 2].Value;
            ExcelWorksheet wsHoT6 = excelWorkBook.Worksheets[cTemp];         // "T6 Tipo virus edad grav."

            int rowI = Convert.ToInt32(wsPara.Cells[62, 2].Value);
            int colI = Convert.ToInt32(wsPara.Cells[63, 2].Value);
            for (nI = 1; nI <= nAnios; ++nI)
            {
                int nAnio = _YearFrom + (nI - 1);
                if (nI > 1)
                {
                    wsHoT6.Cells[rowI - 1, colI, rowI + 10, colI + nGrEd].Copy(wsHoT6.Cells[rowI - 1, colI + (nI - 1) * (nGrEd + 1)]);

                    int r = rowIT2 + 3;

                    for (int a = 1; a <= nGrEd; a++)
                    {
                        for (int n = 1; n <= 10; n++)
                        {
                            int c = _startColumn + (nI - 1) * 57 + 53;

                            cTemp = "=" + excelWorksheet.Cells[r + (n - 1) * 75 + (a - 1) * 12, c].FullAddress;
                            int aa = rowI + (n - 1);
                            int bb = colI + (nI - 1) * (nGrEd + 1) + (a - 1);
                            wsHoT6.Cells[rowI + (n - 1), colI + (nI - 1) * (nGrEd + 1) + (a - 1)].Formula = cTemp;
                        }
                    }
                }

                wsHoT6.Cells[rowI - 2, colI + (nI - 1) * (nGrEd + 1)].Value = nAnio;
            }

            //----
            Dictionary<string, object> aParam = new Dictionary<string, object>();
            cTemp = (string)wsPara.Cells[69, 2].Value;
            ExcelWorksheet wsHoG4 = excelWorkBook.Worksheets[cTemp];        // G4 Grupos Edad

            aParam.Add("ChartTitle", (string)wsPara.Cells[71, 2].Value);        // Titulo grafico
            aParam.Add("ChartTitleSize", 12);
            aParam.Add("AxisTitleYSize", 11);
            aParam.Add("AxisTitleXSize", 11);
            aParam.Add("AxisYSize", 10);
            aParam.Add("AxisXSize", 10);
            aParam.Add("LegendSize", 11);
            aParam.Add("Width", 900);
            aParam.Add("Hight", 500);
            aParam.Add("RowUpperChart", 4);
            aParam.Add("ColLeftChart", 0);
            aParam.Add("Offset", 10);
            aParam.Add("OffsetRowX", 2);

            R6_GraficoVirus_100StackedColumnAG(wsHoT6, wsPara, wsHoG4, rowI, colI, nGrEd, nAnios, _startColumn, weekByYear, "G4_0", aParam);

            //############################# Grafico hoja: "G5 Gravedad"
            //---- Tabla: T6 Tipo virus edad grav (Hosp., UCI, Def.)
            rowI = Convert.ToInt32(wsPara.Cells[64, 2].Value);
            colI = Convert.ToInt32(wsPara.Cells[65, 2].Value);
            for (nI = 1; nI <= nAnios; ++nI)
            {
                int nAnio = _YearFrom + (nI - 1);
                if (nI > 1)
                {
                    wsHoT6.Cells[rowI - 2, colI, rowI + 10, colI + 2].Copy(wsHoT6.Cells[rowI - 2, colI + (nI - 1) * 3]);
                    for (int a = 1; a <= 3; a++)
                    {
                        int rT = rowIT2 + 4;                // 208 + 4 = 212

                        int colD = colI + (nI - 1) * 3 + (a - 1);
                        for (int n = 1; n <= 10; n++)
                        {
                            int cT = _startColumn + (nI - 1) * 57 + 56;
                            cTemp = "=" + excelWorksheet.Cells[rT + (n - 1) * 75 + (a - 1) * 3, cT].FullAddress;
                            wsHoT6.Cells[rowI + (n - 1), colI + (nI - 1) * 3 + (a - 1)].Formula = cTemp;
                        }
                        //----
                        wsHoT6.Cells[rowI - 2, colD].Formula = wsHoT6.Cells[rowI - 2, colI + (a - 1)].Formula;      // Leyenda
                    }
                }

                wsHoT6.Cells[rowI - 3, colI + (nI - 1) * 3].Value = nAnio;
            }

            Dictionary<string, object> aParamG5 = new Dictionary<string, object>();
            cTemp = (string)wsPara.Cells[70, 2].Value;
            ExcelWorksheet wsHoG5 = excelWorkBook.Worksheets[cTemp];            // G5 Gravedad

            aParamG5.Add("ChartTitle", (string)wsPara.Cells[72, 2].Value);        // Titulo grafico
            aParamG5.Add("ChartTitleSize", 12);
            aParamG5.Add("AxisTitleYSize", 11);
            aParamG5.Add("AxisTitleXSize", 11);
            aParamG5.Add("AxisYSize", 10);
            aParamG5.Add("AxisXSize", 10);
            aParamG5.Add("LegendSize", 11);
            aParamG5.Add("Width", 720);
            aParamG5.Add("Hight", 500);
            aParamG5.Add("RowUpperChart", 4);
            aParamG5.Add("ColLeftChart", 0);
            aParamG5.Add("Offset", 10);
            aParamG5.Add("OffsetRowX", 3);

            R6_GraficoVirus_100StackedColumn(wsHoT6, wsPara, wsHoG5, rowI, colI, nGrEd, nAnios, _startColumn, weekByYear, "G5_0", aParamG5);

            //**** inserción de labels y logo en el Excel
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            if (ReportCountry != null)
            {
                //inserción de labels
                reportLabels(consString, countryId, languaje_, ReportCountry, hospitalId, year, YearFrom, YearTo, se, startDate, endDate, excelWorkBook, excelWorksheet);

                //inserción de logo
                InsertarLogoExcel(consString, excelWorksheet, ReportCountry);
            }
        }

        private static void R6_ActualizarFormulaCeldasTablas(ExcelWorksheet wsTabl, ExcelWorksheet wsPara, ExcelWorksheet wsWork, int nAnios, int nCoWi, int rowS, int colS,
            int rowI_TA, int colI_TA, int salto1, int salto2, int rowEW, int widthYear_TA, int heightYear_WK, int nCoWiCopy, bool bCopy)
        {
            int nI; string cTemp;

            for (nI = 1; nI <= nAnios; ++nI)
            {
                if (nI > 1)
                {
                    if (bCopy)
                        wsWork.Cells[rowS, colS - 2, rowS + heightYear_WK - 1, colS + nCoWiCopy - 1].Copy(wsWork.Cells[rowS + (nI - 1) * heightYear_WK, colS - 2]);

                    int rowIA = rowS + (nI - 1) * heightYear_WK;               // Fila inicio anio en la hoja work

                    //---- Actualizando formula de columnas
                    for (int nK = 1; nK <= nCoWi; nK++)
                    {
                        string addresInicSE = "", addresInicYE = "";
                        int rowPT = rowI_TA;

                        if (nK == 1)
                        {
                            addresInicSE = wsTabl.Cells[rowEW, colI_TA + (nI - 1) * widthYear_TA].FullAddress;
                            addresInicYE = wsTabl.Cells[rowEW - 1, colI_TA + (nI - 1) * widthYear_TA].FullAddress;
                        }

                        rowPT = rowPT + salto1;
                        string addresPorcTotaH = wsTabl.Cells[rowPT + (nK - 1) * salto2, colI_TA + (nI - 1) * widthYear_TA].FullAddress;

                        for (int nJ = 1; nJ <= 53; nJ++)
                        {
                            int rowCurr = rowIA + (nJ - 1);

                            if (nK == 1 && bCopy == true)
                            {
                                cTemp = "=IF(OFFSET(" + addresInicYE + ",0,ROW(B" + rowCurr.ToString() + ")-" + rowIA.ToString() + ")>0, OFFSET(" + addresInicYE + ",0,ROW(B" + rowCurr.ToString() + ")-" + rowIA.ToString() + "), \"\")";
                                wsWork.Cells[rowCurr, colS - 2].Formula = cTemp;

                                cTemp = "=IF(OFFSET(" + addresInicSE + ",0,ROW(B" + rowCurr.ToString() + ")-" + rowIA.ToString() + ")>0, OFFSET(" + addresInicSE + ",0,ROW(B" + rowCurr.ToString() + ")-" + rowIA.ToString() + "), \"\")";
                                wsWork.Cells[rowCurr, colS - 1].Formula = cTemp;
                            }

                            cTemp = "=IF(B" + rowCurr.ToString() + "=\"\", \"\", OFFSET(" + addresPorcTotaH + ",0,ROW(B" + rowCurr.ToString() + ")-" + rowIA.ToString() + "))";
                            wsWork.Cells[rowCurr, colS + (nK - 1)].Formula = cTemp;
                        }
                    }
                }
            }
        }

        private static void R6_GraficoVirus_100StackedColumnAG(ExcelWorksheet excelWorksheet, ExcelWorksheet wsPara, ExcelWorksheet wsHoGr,
            int rowI_TA, int colI_TA, int nGrEd, int nAnios, int _startColumn, List<int> weekByYear, string chart, Dictionary<string, object> aPara)
        {
            int NUM_TIPOS_SUBTIPOS_VIRUS = 10;

            //---- Elimando los graficos de plantilla
            for (int nX = wsHoGr.Drawings.Count - 1; nX >= 0; nX--)
                wsHoGr.Drawings.Remove(nX);

            //---- Rangos de las series
            string cValuesY = "";
            string cValuesX = "";

            int offsetRowX = (int)aPara["OffsetRowX"];
            int col = colI_TA;
            var myChartCC = wsHoGr.Drawings.AddChart(chart, eChartType.ColumnStacked100);

            for (int nJ = 1; nJ <= NUM_TIPOS_SUBTIPOS_VIRUS; nJ++)
            {
                cValuesY = "";
                cValuesX = "";
                int row = rowI_TA + (nJ - 1);

                for (int nI = 1; nI <= nAnios; ++nI)
                {
                    col = colI_TA + (nGrEd + 1) * (nI - 1);

                    cValuesY = cValuesY + excelWorksheet.Cells[row, col, row, col + nGrEd - 1].FullAddress + ",";
                    cValuesX = cValuesX + excelWorksheet.Cells[rowI_TA - offsetRowX, col, rowI_TA - offsetRowX + 1, col + nGrEd - 1].FullAddress + ",";
                }
                cValuesY = cValuesY.Substring(0, cValuesY.Length - 1);
                cValuesX = cValuesX.Substring(0, cValuesX.Length - 1);

                ExcelRange valuesY = excelWorksheet.Cells[cValuesY];
                ExcelRange valuesX = excelWorksheet.Cells[cValuesX];

                var serieCC = myChartCC.Series.Add(valuesY, valuesX);                           // Valores de la serie, Etiquetas del eje X
                serieCC.Header = (string)excelWorksheet.Cells[row, colI_TA - 1].Value;          // Leyenda
            }

            //if(chart == "G4_0")
            //{
            myChartCC.XAxis.Title.Text = (string)wsPara.Cells[2, 3].Value;              // Grupos de edad
            myChartCC.XAxis.Title.Font.Size = (int)aPara["AxisTitleXSize"]; ;
            myChartCC.XAxis.Title.Font.Bold = true;
            myChartCC.XAxis.MajorTickMark = eAxisTickMark.Out;
            myChartCC.XAxis.MinorTickMark = eAxisTickMark.None;
            //}

            myChartCC.YAxis.Title.Text = (string)wsPara.Cells[7, 2].Value;                  // Porcentaje
            myChartCC.YAxis.Title.Font.Size = (int)aPara["AxisTitleYSize"];
            myChartCC.YAxis.Title.Font.Bold = true;

            //---- 
            int colOffset = (int)aPara["Offset"];
            int width0 = (int)aPara["Width"];
            int hight0 = (int)aPara["Hight"];
            int row0 = (int)aPara["RowUpperChart"];
            int col0 = (int)aPara["ColLeftChart"];

            myChartCC.SetSize(width0 + (nAnios - 1) * (width0 / 4), hight0);
            myChartCC.SetPosition(row0, 0, col0, colOffset);

            myChartCC.Title.Text = (string)aPara["ChartTitle"];                         // Titulo grafico
            myChartCC.Title.Font.Size = (int)aPara["ChartTitleSize"];
            myChartCC.Title.Font.Bold = true;

            myChartCC.Legend.Position = eLegendPosition.Bottom;
            myChartCC.Legend.Font.Size = (int)aPara["LegendSize"]; ;
            myChartCC.Legend.Font.Bold = true;

            var color = Color.FromArgb(255, 102, 0);
            SetChartPointsColor(myChartCC, 0, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(255, 255, 0);
            SetChartPointsColor(myChartCC, 1, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(0, 0, 0);
            SetChartPointsColor(myChartCC, 2, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(255, 0, 0);
            SetChartPointsColor(myChartCC, 3, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(0, 204, 255);
            SetChartPointsColor(myChartCC, 4, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(153, 204, 0);
            SetChartPointsColor(myChartCC, 5, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(128, 0, 128);
            SetChartPointsColor(myChartCC, 6, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(55, 96, 146);
            SetChartPointsColor(myChartCC, 7, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(255, 153, 204);
            SetChartPointsColor(myChartCC, 8, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(166, 166, 166);
            SetChartPointsColor(myChartCC, 9, color, (int)eChartType.ColumnStacked);

            //---- Habilitando Multi-Level Category Labels
            EnableMultilevelAxisLabels(myChartCC, "ColumnStacked100");
        }

        private static void R6_GraficoVirus_100StackedColumn(ExcelWorksheet excelWorksheet, ExcelWorksheet wsPara, ExcelWorksheet wsHoGr,
            int rowI_TA, int colI_TA, int nGrEd, int nAnios, int _startColumn, List<int> weekByYear, string chart, Dictionary<string, object> aPara)
        {
            int NUM_TIPOS_SUBTIPOS_VIRUS = 10;

            //---- Elimando los graficos de plantilla
            for (int nX = wsHoGr.Drawings.Count - 1; nX >= 0; nX--)
                wsHoGr.Drawings.Remove(nX);

            //---- Rangos de las series
            string cValuesY = "";
            string cValuesX = "";

            int col = colI_TA;
            var myChartCC = wsHoGr.Drawings.AddChart(chart, eChartType.ColumnStacked100);

            for (int nJ = 1; nJ <= NUM_TIPOS_SUBTIPOS_VIRUS; nJ++)
            {
                int row = rowI_TA + (nJ - 1);
                cValuesY = excelWorksheet.Cells[row, col, row, col + nAnios * 3 - 1].FullAddress;
                cValuesX = excelWorksheet.Cells[rowI_TA - 3, col, rowI_TA - 2, col + nAnios * 3 - 1].FullAddress;

                ExcelRange valuesY = excelWorksheet.Cells[cValuesY];
                ExcelRange valuesX = excelWorksheet.Cells[cValuesX];

                var serieCC = myChartCC.Series.Add(valuesY, valuesX);                   // Valores de la serie, Etiquetas del eje X
                serieCC.Header = (string)excelWorksheet.Cells[row, col - 1].Value;                  // Leyenda
            }

            //myChartCC.XAxis.Title.Text = (string)wsPara.Cells[2, 3].Value;          // 
            //myChartCC.XAxis.Title.Font.Size = (int)aPara["AxisTitleXSize"]; ;
            //myChartCC.XAxis.Title.Font.Bold = true;
            //myChartCC.XAxis.MajorTickMark = eAxisTickMark.Out;
            //myChartCC.XAxis.MinorTickMark = eAxisTickMark.None;

            myChartCC.YAxis.Title.Text = (string)wsPara.Cells[7, 2].Value;          // Porcentaje
            myChartCC.YAxis.Title.Font.Size = (int)aPara["AxisTitleYSize"];
            myChartCC.YAxis.Title.Font.Bold = true;

            //---- 
            int colOffset = (int)aPara["Offset"];
            int width0 = (int)aPara["Width"];
            int hight0 = (int)aPara["Hight"];
            int row0 = (int)aPara["RowUpperChart"];
            int col0 = (int)aPara["ColLeftChart"];

            myChartCC.SetSize(width0 + (nAnios - 1) * (width0 / 4), hight0);
            myChartCC.SetPosition(row0, 0, col0, colOffset);

            //myChartCC.Title.Text = (string)wsPara.Cells[72, 2].Value;               // Titulo grafico
            myChartCC.Title.Text = (string)aPara["ChartTitle"];                         // Titulo grafico
            myChartCC.Title.Font.Size = (int)aPara["ChartTitleSize"];
            myChartCC.Title.Font.Bold = true;

            myChartCC.Legend.Position = eLegendPosition.Bottom;
            myChartCC.Legend.Font.Size = (int)aPara["LegendSize"]; ;
            myChartCC.Legend.Font.Bold = true;

            var color = Color.FromArgb(255, 102, 0);
            SetChartPointsColor(myChartCC, 0, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(255, 255, 0);
            SetChartPointsColor(myChartCC, 1, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(0, 0, 0);
            SetChartPointsColor(myChartCC, 2, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(255, 0, 0);
            SetChartPointsColor(myChartCC, 3, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(0, 204, 255);
            SetChartPointsColor(myChartCC, 4, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(153, 204, 0);
            SetChartPointsColor(myChartCC, 5, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(128, 0, 128);
            SetChartPointsColor(myChartCC, 6, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(55, 96, 146);
            SetChartPointsColor(myChartCC, 7, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(255, 153, 204);
            SetChartPointsColor(myChartCC, 8, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(166, 166, 166);
            SetChartPointsColor(myChartCC, 9, color, (int)eChartType.ColumnStacked);

            //---- Habilitando Multi-Level Category Labels
            EnableMultilevelAxisLabels(myChartCC, "ColumnStacked100");
        }

        private static void R6_GraficoPorcentajeIRAG_Line(ExcelWorksheet excelWorksheet, ExcelWorksheet wsPara, ExcelWorksheet wsHoG1,
            int rowI_T1, int rowWeek_T1, int nAnios, int _startColumn, List<int> weekByYear, string chart, int width0,
            int hight0, int row0, int col0, Dictionary<string, decimal> aPara, string enPersona)
        {
            //---- Rangos de las series
            string cvalues = "", cxvalues = "";
            string cvaluesU = "", cxvaluesU = "";
            string cvaluesD = "", cxvaluesD = "";

            int rowH = rowI_T1 + 6;
            int rowU = rowI_T1 + 15;
            int rowD = rowI_T1 + 24;
            int widthYear = 57;

            for (int nI = 1; nI <= nAnios; ++nI)
            {
                int col = _startColumn + (nI - 1) * widthYear;
                int nSema = weekByYear[nI - 1];

                cvalues = cvalues + excelWorksheet.Cells[rowH, col, rowH, col + nSema - 1].FullAddress + ",";
                cxvalues = cxvalues + excelWorksheet.Cells[rowWeek_T1 - 1, col, rowWeek_T1, col + nSema - 1].FullAddress + ",";

                cvaluesU = cvaluesU + excelWorksheet.Cells[rowU, col, rowU, col + nSema - 1].FullAddress + ",";
                cxvaluesU = cxvaluesU + excelWorksheet.Cells[rowWeek_T1 - 1, col, rowWeek_T1, col + nSema - 1].FullAddress + ",";

                cvaluesD = cvaluesD + excelWorksheet.Cells[rowD, col, rowD, col + nSema - 1].FullAddress + ",";
                cxvaluesD = cxvaluesD + excelWorksheet.Cells[rowWeek_T1 - 1, col, rowWeek_T1, col + nSema - 1].FullAddress + ",";
            }

            cvalues = cvalues.Substring(0, cvalues.Length - 1);
            cxvalues = cxvalues.Substring(0, cxvalues.Length - 1);
            cvaluesU = cvaluesU.Substring(0, cvaluesU.Length - 1);
            cxvaluesU = cxvaluesU.Substring(0, cxvaluesU.Length - 1);
            cvaluesD = cvaluesD.Substring(0, cvaluesD.Length - 1);
            cxvaluesD = cxvaluesD.Substring(0, cxvaluesD.Length - 1);

            ExcelRange values = excelWorksheet.Cells[cvalues];
            ExcelRange xvalues = excelWorksheet.Cells[cxvalues];
            ExcelRange valuesU = excelWorksheet.Cells[cvaluesU];
            ExcelRange xvaluesU = excelWorksheet.Cells[cxvaluesU];
            ExcelRange valuesD = excelWorksheet.Cells[cvaluesD];
            ExcelRange xvaluesD = excelWorksheet.Cells[cxvaluesD];

            //---- Chart principal
            var myChartCC = wsHoG1.Drawings.AddChart(chart, eChartType.Line);

            var serieCC = myChartCC.Series.Add(values, xvalues);                    // H: Valores de la serie, Etiquetas del eje X
            serieCC.Header = (string)wsPara.Cells[54, 2].Value;                     // Leyenda de la linea
            serieCC = myChartCC.Series.Add(valuesU, xvaluesU);                      // U: Valores de la serie, Etiquetas del eje X
            serieCC.Header = (string)wsPara.Cells[55, 2].Value;                     // Leyenda de la linea
            serieCC = myChartCC.Series.Add(valuesD, xvaluesD);                      // D: Valores de la serie, Etiquetas del eje X
            serieCC.Header = (string)wsPara.Cells[56, 2].Value;                     // Leyenda de la linea

            myChartCC.XAxis.Title.Text = (string)wsPara.Cells[3, 2].Value;          // SE
            myChartCC.XAxis.Title.Font.Size = (float)aPara["AxisTitleXSize"]; ;
            myChartCC.XAxis.Title.Font.Bold = true;
            myChartCC.XAxis.MajorTickMark = eAxisTickMark.Out;
            myChartCC.XAxis.MinorTickMark = eAxisTickMark.None;

            myChartCC.YAxis.Title.Text = (string)wsPara.Cells[7, 2].Value;          // Porcentaje
            myChartCC.YAxis.Title.Font.Size = (float)aPara["AxisTitleYSize"];
            myChartCC.YAxis.Title.Font.Bold = true;

            //---- 
            int colOffset = 10;
            myChartCC.SetSize(width0 + (nAnios - 1) * (width0 / 3), hight0);
            myChartCC.SetPosition(row0, 0, col0, colOffset);

            if(enPersona == "")
                myChartCC.Title.Text = (string)wsPara.Cells[58, 2].Value;               // Titulo grafico
            else
                myChartCC.Title.Text = (string)wsPara.Cells[58, 2].Value + " " + enPersona + " " + excelWorksheet.Cells[rowI_T1, _startColumn - 4].Value;               // Titulo grafico

            myChartCC.Title.Font.Size = (float)aPara["ChartTitleSize"];
            myChartCC.Title.Font.Bold = true;

            myChartCC.Legend.Position = eLegendPosition.Bottom;
            myChartCC.Legend.Font.Size = (float)aPara["LegendSize"]; ;
            myChartCC.Legend.Font.Bold = true;

            //---- Habilitando Multi-Level Category Labels
            if (nAnios > 1)
                EnableMultilevelAxisLabels(myChartCC, "Line");
        }

        private static void R6_GraficoVirus_ColumnStacked(ExcelWorksheet excelWorksheet, ExcelWorksheet wsPara, ExcelWorksheet wsHoG2,
            int rowIT2, int rowFT2, int nGrEd, int nAnios, int _startColumn, List<int> weekByYear, string chart)
        {
            //---- Elimando los graficos de plantilla
            for (int nX = wsHoG2.Drawings.Count - 1; nX >= 0; nX--)
            {
                wsHoG2.Drawings.Remove(nX);
            }

            //---- 
            int width0 = 900;
            int hight0 = 520;
            int colOffset = 10;

            //---- Rangos de las series
            string cvalues = "", cxvalues = "";
            string cvaluesSEC = "", cxvaluesSEC = "";

            var myChartG2 = wsHoG2.Drawings.AddChart(chart, eChartType.ColumnStacked);
            int row = rowIT2;
            int col = 0;
            int nNuVi = (chart == "G2_0") ? 6 : 10;     // Nro de virus a mostra en el grafico

            for (int nJ = 1; nJ <= nNuVi; ++nJ)         // Influenza A(H1N1)pdm09, Influenza A No Subtipificada, ..., Influenza B
            {
                cvalues = "";
                cxvalues = "";
                row = rowIT2 + (nJ - 1) * (12 * nGrEd + 3);

                for (int nI = 1; nI <= nAnios; ++nI)
                {
                    col = _startColumn + (nI - 1) * 57;
                    int nSema = weekByYear[nI - 1];

                    cvalues = cvalues + excelWorksheet.Cells[row, col, row, col + nSema - 1].FullAddress + ",";
                    cxvalues = cxvalues + excelWorksheet.Cells[rowIT2 - 3, col, rowIT2 - 2, col + nSema - 1].FullAddress + ",";

                    if (nJ == 1)        // Para chart secundario
                    {
                        if (chart == "G2_0")
                        {
                            cvaluesSEC = cvaluesSEC + excelWorksheet.Cells[rowFT2 + 3, col, rowFT2 + 3, col + nSema - 1].FullAddress + ",";
                        }
                        else
                        {
                            cvaluesSEC = cvaluesSEC + excelWorksheet.Cells[rowFT2 + 4, col, rowFT2 + 4, col + nSema - 1].FullAddress + ",";
                        }
                        cxvaluesSEC = cxvaluesSEC + excelWorksheet.Cells[rowIT2 - 3, col, rowIT2 - 2, col + nSema - 1].FullAddress + ",";
                    }
                }

                cvalues = cvalues.Substring(0, cvalues.Length - 1);
                cxvalues = cxvalues.Substring(0, cxvalues.Length - 1);
                ExcelRange values = excelWorksheet.Cells[cvalues];
                ExcelRange xvalues = excelWorksheet.Cells[cxvalues];

                var serieG2 = myChartG2.Series.Add(values, xvalues);                // Valores de la serie, Etiquetas del eje 

                if (nJ == 6)
                    serieG2.Header = (string)excelWorksheet.Cells[row, 1].Value;            // Leyenda de la serie
                else
                    serieG2.Header = (string)excelWorksheet.Cells[row, 2].Value;            // Leyenda de la serie                
            }

            var color = Color.FromArgb(255, 102, 0);
            SetChartPointsColor(myChartG2, 0, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(255, 255, 0);
            SetChartPointsColor(myChartG2, 1, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(0, 0, 0);
            SetChartPointsColor(myChartG2, 2, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(255, 0, 0);
            SetChartPointsColor(myChartG2, 3, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(0, 204, 255);
            SetChartPointsColor(myChartG2, 4, color, (int)eChartType.ColumnStacked);
            color = Color.FromArgb(153, 204, 0);
            SetChartPointsColor(myChartG2, 5, color, (int)eChartType.ColumnStacked);
            if (chart == "G3_0")
            {
                color = Color.FromArgb(128, 0, 128);
                SetChartPointsColor(myChartG2, 6, color, (int)eChartType.ColumnStacked);
                color = Color.FromArgb(55, 96, 146);
                SetChartPointsColor(myChartG2, 7, color, (int)eChartType.ColumnStacked);
                color = Color.FromArgb(255, 153, 204);
                SetChartPointsColor(myChartG2, 8, color, (int)eChartType.ColumnStacked);
                color = Color.FromArgb(166, 166, 166);
                SetChartPointsColor(myChartG2, 9, color, (int)eChartType.ColumnStacked);
            }

            myChartG2.XAxis.Title.Text = (string)wsPara.Cells[3, 2].Value;                  // SE
            myChartG2.XAxis.Title.Font.Size = 12;
            myChartG2.XAxis.Title.Font.Bold = true;
            myChartG2.XAxis.MajorTickMark = eAxisTickMark.Out;
            myChartG2.XAxis.MinorTickMark = eAxisTickMark.None;

            myChartG2.YAxis.Title.Text = (string)wsPara.Cells[8, 2].Value;                  // Número de casos positivos
            myChartG2.YAxis.Title.Font.Size = 12;
            myChartG2.YAxis.Title.Font.Bold = true;

            //**** Chart secundario 
            cvaluesSEC = cvaluesSEC.Substring(0, cvaluesSEC.Length - 1);
            cxvaluesSEC = cxvaluesSEC.Substring(0, cxvaluesSEC.Length - 1);
            ExcelRange valueSEC = excelWorksheet.Cells[cvaluesSEC];
            ExcelRange xvaluesSEC = excelWorksheet.Cells[cxvaluesSEC];

            var myChartG2SEC = myChartG2.PlotArea.ChartTypes.Add(eChartType.Line);          // Chart secundario
            var serieSEC = myChartG2SEC.Series.Add(valueSEC, xvaluesSEC);
            if (chart == "G2_0")
                serieSEC.Header = (string)excelWorksheet.Cells[rowFT2 + 3, 1].Value;            // Leyenda serie
            else
                serieSEC.Header = (string)excelWorksheet.Cells[rowFT2 + 4, 1].Value;            // Leyenda serie
            //color = Color.FromArgb(192, 0, 0);
            //SetChartPointsColor(myChartG2SEC, 0, color, (int)eChartType.Line);

            myChartG2SEC.UseSecondaryAxis = true;
            if (chart == "G2_0")
                myChartG2SEC.YAxis.Title.Text = (string)excelWorksheet.Cells[rowFT2 + 3, 1].Value;      // % Positivos a Influenza
            else
                myChartG2SEC.YAxis.Title.Text = (string)excelWorksheet.Cells[rowFT2 + 4, 1].Value;      // % Positivos a virus respiratorios
            myChartG2SEC.YAxis.Title.Font.Size = 12;
            myChartG2SEC.YAxis.Title.Font.Bold = true;

            //****   
            width0 = 900;
            hight0 = 520;
            colOffset = 10;
            row = 4;
            col = 0;

            if (chart == "G2_0")
                myChartG2.Title.Text = (string)wsPara.Cells[59, 2].Value;               // Titulo grafico
            else
                myChartG2.Title.Text = (string)wsPara.Cells[60, 2].Value;               // Titulo grafico
            myChartG2.Title.Font.Size = 11;
            myChartG2.Title.Font.Bold = true;
            myChartG2.SetSize(width0 + (nAnios - 1) * (width0 / 3), hight0);
            myChartG2.SetPosition(row, 0, col, colOffset);
            myChartG2.Legend.Position = eLegendPosition.Bottom;
            myChartG2.Legend.Font.Size = 12;
            myChartG2.Legend.Font.Bold = true;

            //**** Habilitando Multi-Level Category Labels
            if (nAnios > 1)
                EnableMultilevelAxisLabels(myChartG2, "ColumnStacked");
        }

        private static void FormatearAreaDatos(ExcelWorksheet excelWorksheet, List<int> weekByYear, int yearFrom, int startRow, int startColumn, int endColumn)
        {
            int nFil, nFilF;
            int nI = 1;
            int nCol = startColumn;
            foreach (int semanas in weekByYear)
            {
                nFil = startRow + semanas * (nI - 1);
                nFilF = nFil + semanas - 1;

                excelWorksheet.Cells[nFil, startColumn, nFil, endColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
                excelWorksheet.Cells[nFil, startColumn, nFil, endColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(250, 191, 143));

                ExcelRange rRang = excelWorksheet.Cells[ExcelRange.GetAddress(nFil, startColumn, nFilF, endColumn)];
                rRang.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                rRang.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                rRang.Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //****
                for (int nY = nFil; nY <= nFilF; ++nY)
                {
                    var semana = excelWorksheet.Cells[nY, nCol].Value;
                    if (semana != null)
                    {
                        if (semana.ToString() != "")
                        {
                            string anio = (yearFrom + (nI - 1)).ToString();
                            excelWorksheet.Cells[nY, nCol].Value = anio.Substring(anio.Length - 2) + "-" + semana.ToString();
                        }
                    }
                }
                ++nI;
            }
        }

        //private static void AppendDataToExcel(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo)
        private static void AppendDataToExcel(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se,
            DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet,
            bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? AreaId, string CasosNPHL = "", 
            int? Sentinel = null, int starRowAntec = 0, int starColAntec = 0)               //#### CAFQ
        {
            var excelWorksheet = excelWorkBook.Worksheets[sheet];
            var row = startRow;
            var column = startColumn;
            string _storedProcedure;
            int excelColTota = 0, nPosiTipo = 0, nInicTip2 = 0, nPoSuViGr = 0;
            Dictionary<string, string> casosNHPLNumber = new Dictionary<string, string>();
            int nGrEd = PAHOClassUtilities.getNumberAgeGroupCountry(countryId);                 // Numero de grupos de edad del pais
            int startColumnT2 = 0;                  // Columna de inicio Tabla para R6
            if (storedProcedure == "R6")
            {
                if (int.TryParse(CasosNPHL, out startColumnT2) == false)
                    startColumnT2 = 6;
            }
            //startColumnT2 = int.Parse(CasosNPHL); 

            //_storedProcedure = (storedProcedure == "ML1") ? "MuestrasLabNPHL" : storedProcedure;      //#### CAFQ
            _storedProcedure = storedProcedure;
            if (storedProcedure == "ML1")
                _storedProcedure = "MuestrasLabNPHL";
            else if (storedProcedure == "LRD")
                _storedProcedure = "R9_LibroReporteDiario";
            else if (storedProcedure == "CPE")
                _storedProcedure = "R10_CondicionesPreexistentes";
            else if (storedProcedure == "CPV")
                _storedProcedure = "R11_CasosPositivosInfluenzaConVacuna";

            if (storedProcedure == "R5")
            {
                if (countryId == 17 || countryId == 119 || countryId == 11)
                {
                    _storedProcedure = "R5_JM";
                    nPosiTipo = 21;                 // Posic. columna "Tipo" (tabla retornada x SP)
                    nInicTip2 = 11;                 // Inicio hospitalizados (tabla retornada x SP)
                    nPoSuViGr = 13;                 // Posic. Excel Sumatoria (Columna "Total hospitalized cases")
                }
                else
                {
                    if (countryId == 25)
                    {
                        _storedProcedure = "R5_2";
                        nPosiTipo = 19;                 // Posic. columna "Tipo" (tabla retornada x SP)
                        nInicTip2 = 10;                 // Inicio hospitalizados (tabla retornada x SP)
                        nPoSuViGr = 12;                 // Posic. Excel Sumatoria (Columna "Total hospitalized cases")
                    }
                    else
                    {
                        nPosiTipo = 15;                 // Posic. columna "Tipo" (tabla retornada x SP)
                        nInicTip2 = 8;                  // Inicio hospitalizados (tabla retornada x SP)
                        nPoSuViGr = 10;                 // Posic. Excel Sumatoria (Columna "Total hospitalized cases")
                    }
                }
            }

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                //using (var command = new SqlCommand(_storedProcedure, con) { CommandType = CommandType.StoredProcedure })     //**** CAFQ
                using (var command = new SqlCommand(_storedProcedure, con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1200 })    //**** CAFQ
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                    command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                    command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                    command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                    command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                    command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;                        //#### CAFQ
                    command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;          //#### CAFQ
                    if (storedProcedure == "ML1")                                                       // Muestras laboratorio NPHL
                    {
                        CasosNPHL = getIdentificarCasos(CasosNPHL, casosNHPLNumber);
                        command.Parameters.Add("@CasosNPHL", SqlDbType.Text).Value = CasosNPHL;
                    }
                    else if (storedProcedure == "CPE" || storedProcedure == "CC")                   // R8_ConsolidadoCarga o R10_CondicionesPreexistentes
                    {
                        command.Parameters.Add("@Area_ID", SqlDbType.Int).Value = AreaId;
                        command.Parameters.Add("@Sentinel", SqlDbType.Int).Value = Sentinel;
                    }

                    con.Open();
                    if ((storedProcedure == "R5"))
                    {
                        string VRS;
                        VRS = (languaje_ == "ENG") ? "RSV" : "VRS";

                        IDictionary<int, string> formulas1 = new Dictionary<int, string>();
                        formulas1[1] = "=" + VRS + "!B{{toreplace}}";
                        formulas1[2] = "=" + VRS + "!C{{toreplace}} + Ad!C{{toreplace}} + Parainfluenza!C{{toreplace}} + 'Inf A'!C{{toreplace}} + 'Inf B'!C{{toreplace}} + Metapnemovirus!C{{toreplace}}";
                        formulas1[3] = "=" + VRS + "!C{{toreplace}}";
                        formulas1[4] = "=Ad!C{{toreplace}}";
                        formulas1[5] = "=Parainfluenza!C{{toreplace}}";
                        formulas1[6] = "='Inf A'!C{{toreplace}}";
                        formulas1[7] = "='Inf B'!C{{toreplace}}";
                        formulas1[8] = "=Metapnemovirus!C{{toreplace}}";

                        if (countryId == 17 || countryId == 119 || countryId == 11)
                        {
                            formulas1[9] = "=" + VRS + "!M{{toreplace}}";
                            formulas1[10] = "=" + VRS + "!N{{toreplace}}+Ad!N{{toreplace}}+Parainfluenza!N{{toreplace}}+'Inf A'!N{{toreplace}}+'Inf B'!N{{toreplace}}+Metapnemovirus!N{{toreplace}}";

                            formulas1[11] = "=" + VRS + "!N{{toreplace}}";
                            formulas1[12] = "=Ad!N{{toreplace}}";
                            formulas1[13] = "=Parainfluenza!N{{toreplace}}";
                            formulas1[14] = "='Inf A'!N{{toreplace}}";
                            formulas1[15] = "='Inf B'!N{{toreplace}}";
                            formulas1[16] = "=Metapnemovirus!N{{toreplace}}";

                            formulas1[17] = "=D{{toreplace}}+E{{toreplace}}+F{{toreplace}}+G{{toreplace}}+H{{toreplace}}+I{{toreplace}}+J{{toreplace}}+K{{toreplace}}+L{{toreplace}}";
                            formulas1[18] = "=O{{toreplace}}+P{{toreplace}}+Q{{toreplace}}+R{{toreplace}}+S{{toreplace}}+T{{toreplace}}+U{{toreplace}}+V{{toreplace}}+W{{toreplace}}";
                        }
                        else
                        {
                            if (countryId == 25)
                            {
                                formulas1[9] = "=" + VRS + "!L{{toreplace}}";
                                formulas1[10] = "=" + VRS + "!M{{toreplace}}+Ad!M{{toreplace}}+Parainfluenza!M{{toreplace}}+'Inf A'!M{{toreplace}}+'Inf B'!M{{toreplace}}+Metapnemovirus!M{{toreplace}}";

                                formulas1[11] = "=" + VRS + "!M{{toreplace}}";
                                formulas1[12] = "=Ad!M{{toreplace}}";
                                formulas1[13] = "=Parainfluenza!M{{toreplace}}";
                                formulas1[14] = "='Inf A'!M{{toreplace}}";
                                formulas1[15] = "='Inf B'!M{{toreplace}}";
                                formulas1[16] = "=Metapnemovirus!M{{toreplace}}";

                                formulas1[17] = "=D{{toreplace}}+E{{toreplace}}+F{{toreplace}}+G{{toreplace}}+H{{toreplace}}+I{{toreplace}}+J{{toreplace}}+K{{toreplace}}";
                                formulas1[18] = "=N{{toreplace}}+O{{toreplace}}+P{{toreplace}}+Q{{toreplace}}+R{{toreplace}}+S{{toreplace}}+T{{toreplace}}+U{{toreplace}}";
                            }
                            else
                            {
                                formulas1[9] = "=" + VRS + "!J{{toreplace}}";
                                formulas1[10] = "=" + VRS + "!K{{toreplace}}+Ad!K{{toreplace}}+Parainfluenza!K{{toreplace}}+'Inf A'!K{{toreplace}}+'Inf B'!K{{toreplace}}+Metapnemovirus!K{{toreplace}}";

                                formulas1[11] = "=" + VRS + "!K{{toreplace}}";
                                formulas1[12] = "=Ad!K{{toreplace}}";
                                formulas1[13] = "=Parainfluenza!K{{toreplace}}";
                                formulas1[14] = "='Inf A'!K{{toreplace}}";
                                formulas1[15] = "='Inf B'!K{{toreplace}}";
                                formulas1[16] = "=Metapnemovirus!K{{toreplace}}";

                                formulas1[17] = "=D{{toreplace}}+E{{toreplace}}+F{{toreplace}}+G{{toreplace}}+H{{toreplace}}+I{{toreplace}}";
                                formulas1[18] = "=L{{toreplace}}+M{{toreplace}}+N{{toreplace}}+O{{toreplace}}+P{{toreplace}}+Q{{toreplace}}";
                            }
                        }

                        for (int i = 1; i <= 7; i++)            // i: Hoja
                        {
                            var virustype = 0;                  // Hoja global
                            switch (i)
                            {
                                case 2:
                                    virustype = 6;
                                    break;
                                case 3:
                                    virustype = 7;
                                    break;
                                case 4:
                                    virustype = 13;
                                    break;
                                case 5:
                                    virustype = 1;
                                    break;
                                case 6:
                                    virustype = 2;
                                    break;
                                case 7:
                                    virustype = 8;
                                    break;
                            }

                            command.Parameters.Clear();
                            command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                            command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                            command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                            command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                            command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                            command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                            command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                            command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                            command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                            command.Parameters.Add("@Virus_type", SqlDbType.Int).Value = virustype;
                            command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                            command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                            command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;                        //#### CAFQ
                            command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;          //#### CAFQ
                            command.Parameters.Add("@Area_ID", SqlDbType.Int).Value = AreaId;
                            command.Parameters.Add("@Sentinel", SqlDbType.Int).Value = Sentinel;

                            var excelWorksheet2 = excelWorkBook.Worksheets[i];

                            using (var reader = command.ExecuteReader())
                            {
                                excelColTota = reader.FieldCount + 1;

                                row = startRow;
                                column = startColumn;
                                var tipo_anterior = 1;

                                while (reader.Read())
                                {
                                    if (Convert.ToInt32(reader.GetValue(nPosiTipo)) != tipo_anterior)
                                    {
                                        row++;
                                    }

                                    tipo_anterior = Convert.ToInt32(reader.GetValue(nPosiTipo));
                                    var col = 1;
                                    var readercont = 0;
                                    int stylerow;
                                    if (tipo_anterior == 1)
                                    {
                                        //stylerow = 2;
                                        stylerow = startRow;
                                    }
                                    else
                                    {
                                        stylerow = row + 1;
                                    }
                                    if(row > startRow)
                                        excelWorksheet2.InsertRow(row, 1);

                                    for (int j = 0; j < excelColTota; j++)                              // Total columnas retornadas x consulta
                                    {
                                        var cell = excelWorksheet2.Cells[stylerow, col + j];
                                        if (tipo_anterior == 2 && j > nInicTip2)            // nInicTip2: Inicio hospitalizados (8 o 10)
                                        {
                                        }
                                        else
                                        {
                                            if (i > 1 || j == 0)
                                            {
                                                if (j == 2)
                                                {
                                                    excelWorksheet2.Cells[row, col + j].Formula = formulas1[17].Replace("{{toreplace}}", row.ToString()); ;
                                                }
                                                else
                                                {
                                                    if (j == nPoSuViGr)                 // Totales fila  de virus hospitalizados
                                                    {
                                                        excelWorksheet2.Cells[row, col + j].Formula = formulas1[18].Replace("{{toreplace}}", row.ToString()); ;
                                                    }
                                                    else
                                                    {
                                                        excelWorksheet2.Cells[row, col + j].Value = reader.GetValue(readercont);
                                                        readercont++;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (j < 17 && formulas1.ContainsKey(j))
                                                {
                                                    excelWorksheet2.Cells[row, col + j].Formula = formulas1[j].Replace("{{toreplace}}", row.ToString()); ;
                                                }
                                            }
                                        }
                                        excelWorksheet2.Cells[row, col + j].StyleID = cell.StyleID;
                                    }
                                    row++;
                                }
                            }
                        }
                    } // END (storedProcedure == "R5")
                    else
                    {
                        if ((storedProcedure == "R6"))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                row = startRow;
                                column = startColumn;
                                //var row = startRow;
                                //var column = startColumn;
                                var contador_salto = 0;
                                while (reader.Read())
                                {
                                    var col = column;

                                    for (int i = 0; i < 53; i++)
                                    {
                                        var cell = excelWorksheet.Cells[startRow, col + i];
                                        excelWorksheet.Cells[row, col + i].Value = reader.GetValue(i);
                                    }
                                    contador_salto++;
                                    if (contador_salto % 4 == 2)
                                    {
                                        row += 2;
                                    }
                                    else
                                    {
                                        if (contador_salto % 4 == 0)
                                        {
                                            row += 5;
                                        }
                                        else
                                        {
                                            row++;
                                        }
                                    }

                                }
                            }
                            /*Inicia llenado de Tabla2. Este reporte, en el mismo tab tiene 2 tablas, con aspectos diferentes entre ellas*/
                            using (var command2 = new SqlCommand(storedProcedure + "_2", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1200 })
                            {
                                command2.Parameters.Clear();
                                command2.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                                command2.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                                command2.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                                command2.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                                command2.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                                command2.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                                command2.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                                command2.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                                command2.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                                command2.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                                command2.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                                command2.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;                        //#### CAFQ
                                command2.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;          //#### CAFQ

                                var con2 = new SqlConnection(consString);
                                con2.Open();

                                using (var reader2 = command2.ExecuteReader())
                                {
                                    int nAnDa = 0;
                                    //if (countryId == 25)
                                    //{
                                    //    row = row - 1 + (9 * 3) + 15;
                                    //    nAnDa = 8 * 8;                  // 8: Nº Age Group
                                    //}
                                    //else
                                    //{
                                    //    if (countryId == 17 || countryId == 119 || countryId == 11)
                                    //    {
                                    //        row = row - 1 + (9 * 3) + 15;
                                    //        nAnDa = 9 * 8;              // 9: Nº Age Group
                                    //    }
                                    //    else
                                    //    {
                                    //        //row = 212;
                                    //        row = row - 1 + (9 * 3) + 15;
                                    //        nAnDa = 6 * 8;              // 6: Nº Age Group
                                    //    }
                                    //}
                                    // nGrEd
                                    row = row - 1 + (9 * 3) + 15;
                                    nAnDa = nGrEd * 8;
                                    column = startColumnT2;

                                    var contador_salto2 = 0;
                                    while (reader2.Read())
                                    {
                                        var col2 = column;

                                        for (int i = 5; i < 58; i++)
                                        {
                                            var cell = excelWorksheet.Cells[startRow, col2 + i - 5];
                                            excelWorksheet.Cells[row, col2 + i - 5].Value = reader2.GetValue(i);
                                        }
                                        contador_salto2++;

                                        if (contador_salto2 % nAnDa == 0)
                                        {
                                            row += 5;
                                        }
                                        else
                                        {
                                            if (contador_salto2 % 2 == 0)
                                            {
                                                row += 2;
                                            }
                                            else
                                            {
                                                row++;
                                            }
                                        }
                                    }
                                }
                                /*Termina llenado de Tabla2*/
                            }
                        }
                        else        // R1, R2, R3, R4 etc   // R4 Virus detectados
                        {
                            bool procesarFila;

                            using (var reader = command.ExecuteReader())
                            {
                                int nColums;
                                nColums = (storedProcedure == "R3") ? ((reader.FieldCount - 1) / 2) + 1 : reader.FieldCount;

                                while (reader.Read())
                                {
                                    var col = column;
                                    if (row > startRow && insert_row == true)
                                        excelWorksheet.InsertRow(row, 1);

                                    procesarFila = true;
                                    if (storedProcedure == "ML1")
                                    {
                                        if (!casosNHPLNumber.ContainsKey((string)reader.GetValue(0)))
                                            procesarFila = false;
                                    }

                                    if (procesarFila)
                                    {
                                        for (var i = 0; i < nColums; i++)
                                        {
                                            var cell = excelWorksheet.Cells[startRow, col];
                                            if (reader.GetValue(i) != null)
                                            {
                                                var datoColu = reader.GetValue(i);

                                                double numberD;
                                                //bool isNumber = double.TryParse(reader.GetValue(i).ToString(), out numberD);
                                                bool isNumber = double.TryParse(datoColu.ToString(), out numberD);

                                                DateTime dt;
                                                //bool isDate = DateTime.TryParse(reader.GetValue(i).ToString(), out dt);
                                                bool isDate = DateTime.TryParse(datoColu.ToString(), out dt);

                                                if (isNumber)
                                                    excelWorksheet.Cells[row, col].Value = numberD;
                                                else
                                                {
                                                    if (isDate)
                                                        excelWorksheet.Cells[row, col].Value = dt;
                                                    else
                                                    {
                                                        //excelWorksheet.Cells[row, col].Style.Numberformat.Format = "@";
                                                        excelWorksheet.Cells[row, col].Value = (string)datoColu.ToString();
                                                    }
                                                }

                                                if (storedProcedure != "CPE")
                                                    excelWorksheet.Cells[row, col].StyleID = cell.StyleID;
                                            }
                                            col++;
                                        }
                                        row++;
                                    }
                                }
                            }
                        }
                    }

                    command.Parameters.Clear();
                    con.Close();
                }
            }

            /*Inserción de tablas en el tab  de antecedentes del reporte R6*/
            if (storedProcedure == "R6")
            {
                using (var con = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand("R6_antecedentes", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1200 })
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                        command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                        command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                        command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                        command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                        command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                        command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                        command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                        command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                        command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                        command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;                        //#### CAFQ
                        command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;          //#### CAFQ
                        con.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if(starRowAntec == 0)
                            {
                                row = 11;
                                column = 2;
                            }
                            else
                            {
                                row = starRowAntec;
                                column = starColAntec;
                            }

                            var contador_salto = 0;
                            var excelWorksheet2 = excelWorkBook.Worksheets[2];
                            while (reader.Read())
                            {
                                var col = column;
                                var colcont = 1;
                                for (int i = 0; i < 6; i++)
                                {
                                    var cell = excelWorksheet2.Cells[startRow, col + colcont];
                                    excelWorksheet2.Cells[row, col + colcont].Value = reader.GetValue(i);
                                    if (i == 1 || i == 3)
                                    {
                                        colcont += 4;
                                    }
                                    else
                                    {
                                        colcont += 2;
                                    }
                                }
                                contador_salto++;
                                if (contador_salto == 1 || contador_salto == 10)
                                {
                                    row += 3;
                                }
                                else
                                {
                                    if (contador_salto == 12 || contador_salto == 16 || contador_salto == 23)
                                    {
                                        row += 2;
                                    }
                                    else
                                    {
                                        row++;
                                    }
                                }
                            }
                        }

                        command.Parameters.Clear();
                        con.Close();
                    }
                }
            }
            /*Fin inserción tablas*/

            /*-----------------------Inserción de los parámetros usados para la generación del reporte al Excel--------------------------------------*/
            //inserción de labels y logo en el Excel
            if (ReportCountry != null)
            {
                //inserción de labels
                reportLabels(consString, countryId, languaje_, ReportCountry, hospitalId, year, YearFrom, YearTo, se, startDate, endDate, excelWorkBook, excelWorksheet, AreaId, Sentinel, storedProcedure);

                //inserción de logo
                InsertarLogoExcel(consString, excelWorksheet, ReportCountry);

                if (storedProcedure == "ML1")
                    setDatosAdicionales(excelWorksheet, startRow, startColumn);
            }

            /*-----------------------Fin de la Inserción de los parámetros usados para la generación del reporte al Excel--------------------------------------*/

            // Apply only if it has a Total row at the end and hast SUM in range, i.e. SUM(A1:A4)
            //excelWorksheet.DeleteRow(row, 2);
        }

        private static string getIdentificarCasos(string data, Dictionary<string, string> aData)
        {
            string cTemp = "";

            var array = data.Split('#');
            foreach (string element in array)
            {
                var array2 = element.Split(':');
                if (cTemp == "")
                    cTemp = array2[0];
                else
                    cTemp += '#' + array2[0];

                if (array2.Length > 1)
                    aData.Add(array2[1], array2[1]);
            }

            return cTemp;
        }

        private static void setDatosAdicionales(ExcelWorksheet excelWorksheet, int startRow, int startColumn)
        {
            excelWorksheet.Cells[startRow - 6, startColumn + 1].Value = DateTime.Today;
            //****
            int nFila = startRow;
            long nID, nPaci = 0;
            Dictionary<long, long> dicID = new Dictionary<long, long>();

            var vID = excelWorksheet.Cells[nFila, startColumn + 3].Value;
            while (vID != null && !(string.Compare(vID.ToString(), "") == 0))
            {
                nID = Convert.ToInt64(excelWorksheet.Cells[nFila, startColumn + 3].Value);
                if (dicID.ContainsKey(nID) == false)
                {
                    ++nPaci;
                    dicID.Add(nID, nID);
                }

                nFila++;
                vID = excelWorksheet.Cells[nFila, startColumn + 3].Value;
            }

            excelWorksheet.Cells[startRow - 5, startColumn + 1].Value = nPaci;
        }

        public ActionResult GetSummaryDetailsExcel(int hospitalId, string hospitalDate, int EpiWeek, int EpiYear)
        {
            var HospitalDate = DateTime.Parse(hospitalDate);
            var user = UserManager.FindById(User.Identity.GetUserId());
            var casesummary = db.CaseSummaries.FirstOrDefault(s => s.HosiptalId == hospitalId && s.StartDateOfWeek == HospitalDate);
            if (casesummary == null)
            {
                casesummary = new CaseSummary()
                {
                    StartDateOfWeek = HospitalDate,
                    HosiptalId = hospitalId,
                    EW = EpiWeek,
                    EpiYear = EpiYear,
                    CaseSummaryDetails = new List<CaseSummaryDetail>()
                };
                db.Entry(casesummary).State = EntityState.Added;
                var AgeGroupbyCountry = db.CatAgeGroup.Where(i => i.id_country == user.Institution.CountryID).ToList();
                foreach (CatAgeGroup agegroup in AgeGroupbyCountry)
                {
                    casesummary.CaseSummaryDetails.Add(
                        new CaseSummaryDetail()
                        {
                            AgeGroup = agegroup.id_conf_country,
                            ETINumFem = 0,
                            ETINumMaso = 0,
                            ETINumST = 0,
                            ETINumEmerST = 0,
                            ETIDenoFem = 0,
                            ETIDenoMaso = 0,
                            ETIDenoST = 0,
                            HospFem = 0,
                            HospMaso = 0,
                            HospST = 0,
                            UCIFem = 0,
                            UCIMaso = 0,
                            UCIST = 0,
                            DefFem = 0,
                            DefMaso = 0,
                            DefST = 0,
                            NeuFem = 0,
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
            var casesummaryDetails = casesummary.CaseSummaryDetails.ToArray();


            try
            {
                var ms = new MemoryStream();
                var tempy = ConfigurationManager.AppSettings["DenominadoresTemplate"];

                var tempy2 = System.IO.File.Exists(tempy);

                using (var fs = System.IO.File.OpenRead(tempy))
                {
                    using (var excelPackage = new ExcelPackage(fs))
                    {
                        var excelWorkBook = excelPackage.Workbook;
                        var excelWorksheet = excelWorkBook.Worksheets[1];
                        var row = 3;
                        var column = 2;
                        foreach (CaseSummaryDetail i in casesummaryDetails)
                        {
                            var col_cont = 0;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.ETINumFem;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.ETINumMaso;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.ETINumST;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.HospFem;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.HospMaso;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.HospST;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.UCIFem;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.UCIMaso;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.UCIST;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.DefFem;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.DefMaso;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.DefST;
                            col_cont++;
                            row++;
                        }
                        //[{"Id":1799,"CaseSummaryId":297,"AgeGroup":1,"ETIDenoFem":0,"ETIDenoMaso":0,"ETIDenoST":0,"ETINumFem":0,"ETINumMaso":0,"ETINumST":0,"ETINumEmerST":0,"HospFem":10,"HospMaso":7,"HospST":17,"UCIFem":6,"UCIMaso":4,"UCIST":10,"DefFem":0,"DefMaso":0,"DefST":0,"NeuFem":0,"NeuMaso":0,"NeuST":0,"CCSARIFem":0,"CCSARIMaso":0,"CCSARIST":0,"VentFem":0,"VentMaso":0,"VentST":0},{"Id":1800,"CaseSummaryId":297,"AgeGroup":2,"ETIDenoFem":0,"ETIDenoMaso":0,"ETIDenoST":0,"ETINumFem":0,"ETINumMaso":0,"ETINumST":0,"ETINumEmerST":0,"HospFem":4,"HospMaso":5,"HospST":9,"UCIFem":0,"UCIMaso":1,"UCIST":1,"DefFem":0,"DefMaso":0,"DefST":0,"NeuFem":0,"NeuMaso":0,"NeuST":0,"CCSARIFem":0,"CCSARIMaso":0,"CCSARIST":0,"VentFem":0,"VentMaso":0,"VentST":0},{"Id":1801,"CaseSummaryId":297,"AgeGroup":3,"ETIDenoFem":0,"ETIDenoMaso":0,"ETIDenoST":0,"ETINumFem":0,"ETINumMaso":0,"ETINumST":0,"ETINumEmerST":0,"HospFem":20,"HospMaso":15,"HospST":35,"UCIFem":0,"UCIMaso":3,"UCIST":3,"DefFem":0,"DefMaso":0,"DefST":0,"NeuFem":0,"NeuMaso":0,"NeuST":0,"CCSARIFem":0,"CCSARIMaso":0,"CCSARIST":0,"VentFem":0,"VentMaso":0,"VentST":0},{"Id":1802,"CaseSummaryId":297,"AgeGroup":4,"ETIDenoFem":0,"ETIDenoMaso":0,"ETIDenoST":0,"ETINumFem":0,"ETINumMaso":0,"ETINumST":0,"ETINumEmerST":0,"HospFem":118,"HospMaso":26,"HospST":144,"UCIFem":5,"UCIMaso":1,"UCIST":6,"DefFem":0,"DefMaso":0,"DefST":0,"NeuFem":0,"NeuMaso":0,"NeuST":0,"CCSARIFem":0,"CCSARIMaso":0,"CCSARIST":0,"VentFem":0,"VentMaso":0,"VentST":0},{"Id":1803,"CaseSummaryId":297,"AgeGroup":5,"ETIDenoFem":0,"ETIDenoMaso":0,"ETIDenoST":0,"ETINumFem":0,"ETINumMaso":0,"ETINumST":0,"ETINumEmerST":0,"HospFem":41,"HospMaso":39,"HospST":80,"UCIFem":5,"UCIMaso":5,"UCIST":10,"DefFem":0,"DefMaso":0,"DefST":0,"NeuFem":0,"NeuMaso":0,"NeuST":0,"CCSARIFem":0,"CCSARIMaso":0,"CCSARIST":0,"VentFem":0,"VentMaso":0,"VentST":0},{"Id":1804,"CaseSummaryId":297,"AgeGroup":6,"ETIDenoFem":0,"ETIDenoMaso":0,"ETIDenoST":0,"ETINumFem":0,"ETINumMaso":0,"ETINumST":0,"ETINumEmerST":0,"HospFem":41,"HospMaso":42,"HospST":83,"UCIFem":6,"UCIMaso":10,"UCIST":16,"DefFem":2,"DefMaso":6,"DefST":8,"NeuFem":0,"NeuMaso":0,"NeuST":0,"CCSARIFem":0,"CCSARIMaso":0,"CCSARIST":0,"VentFem":0,"VentMaso":0,"VentST":0}]



                        excelPackage.SaveAs(ms);
                    }
                }

                ms.Position = 0;
                return new FileStreamResult(ms, "application/xlsx")
                {
                    FileDownloadName = "Denominadores_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm") + ".xlsx"
                };
            }
            catch (Exception e)
            {
                ViewBag.Message = "El reporte no se pudo generar, por favor intente de nuevo" + "\n" + e.Message;
            }

            return null;
        }

        public ActionResult GetDataDictionaryExcel()
        {
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            List<String> tables = new List<String>();
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG=db_name()", con))
                {
                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(reader.GetValue(0).ToString());
                        }
                    }
                    con.Close();
                }
            }

            try
            {
                var ms = new MemoryStream();
                var tempy = ConfigurationManager.AppSettings["DictionaryTemplate"];

                var tempy2 = System.IO.File.Exists(tempy);


                using (var fs = System.IO.File.OpenRead(tempy))
                {
                    using (var excelPackage = new ExcelPackage(fs))
                    {
                        var excelWorkBook = excelPackage.Workbook;
                        foreach (String i in tables)
                        {
                            var row = 2;
                            var column = 1;
                            var excelWorksheet = excelWorkBook.Worksheets.Add(i, excelWorkBook.Worksheets[1]);
                            var col_cont = 0;
                            using (var con = new SqlConnection(consString))
                            {
                                using (var command = new SqlCommand("EXEC sp_columns " + i + ";", con))
                                {
                                    con.Open();
                                    using (var reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            excelWorksheet.Cells[row, column + col_cont].Value = reader.GetValue(3).ToString();
                                            col_cont++;
                                            excelWorksheet.Cells[row, column + col_cont].Value = reader.GetValue(5).ToString();
                                            col_cont++;
                                            excelWorksheet.Cells[row, column + col_cont].Value = reader.GetValue(6).ToString();
                                            col_cont++;
                                            excelWorksheet.Cells[row, column + col_cont].Value = reader.GetValue(7).ToString();
                                            col_cont++;
                                            excelWorksheet.Cells[row, column + col_cont].Value = reader.GetValue(8).ToString();
                                            col_cont++;
                                            excelWorksheet.Cells[row, column + col_cont].Value = reader.GetValue(9).ToString();
                                            col_cont++;
                                            excelWorksheet.Cells[row, column + col_cont].Value = reader.GetValue(17).ToString();
                                            col_cont = 0;
                                            row++;
                                        }
                                    }
                                    con.Close();
                                }
                            }
                        }
                        excelWorkBook.Worksheets.Delete(1);
                        excelPackage.SaveAs(ms);
                    }
                }

                ms.Position = 0;
                return new FileStreamResult(ms, "application/xlsx")
                {
                    FileDownloadName = "Diccionario_de_datos_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm") + ".xlsx"
                };
            }
            catch (Exception e)
            {
                ViewBag.Message = "El reporte no se pudo generar, por favor intente de nuevo" + "\n" + e.Message;
            }

            return null;
        }

        /*public JsonResult GetReportsForCountry(int countryID)
        {
            ////var epiYear = DateTime.Now.Year.ToString();
            ////int intEpiYear = Int32.Parse(epiYear);
            List<Dictionary<string, int>> reportsPerCountry = new List<Dictionary<string, int>>();
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            var reportCountry = db.ReportsCountries.FirstOrDefault(s => s.CountryID == countryID && s.active == true);
            if (reportCountry == null)
            {

            }
            else
            {

                dictionary.Add(reportCountry.description, reportCountry.ID);
            }

                reportsPerCountry.Add(dictionary);

            return Json(reportsPerCountry, JsonRequestBehavior.AllowGet);
        }*/

        private static void AppendDataToExcel_REVELAC(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string reportTemplate, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? AreaId)         //#### CAFQ
        {
            ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets["REVELAC_i"];
            var row = startRow;
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                using (var con = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand("REVELAC_i", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 })
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                        command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                        command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                        command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                        command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                        command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                        command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                        command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                        command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                        command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                        command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;
                        command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;

                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            int excelColTota = reader.FieldCount; 
                            row = 2;

                            while (reader.Read())
                            {

                                var col = 1;
                                var readercont = 0;
                                readercont = 0;

                                for (int j = 0; j < excelColTota; j++)                              // Total columnas retornadas x consulta
                                {
                                    if (reader.GetValue(readercont) != null)
                                    {
                                        var datoColu = reader.GetValue(readercont);

                                        double numberD;
                                        bool isNumber = double.TryParse(datoColu.ToString(), out numberD);

                                        DateTime dt;
                                        bool isDate = DateTime.TryParse(datoColu.ToString(), out dt);

                                        if (isNumber)
                                            excelWorksheet.Cells[row, col + j].Value = numberD;
                                        else
                                        {
                                            if (isDate)
                                                excelWorksheet.Cells[row, col + j].Value = dt;
                                            else
                                            {
                                                excelWorksheet.Cells[row, col + j].Value = (string)datoColu.ToString();
                                            }
                                        }
                                    }

                                    readercont++;
                                }
                                row++;
                            }
                        }

                        command.Parameters.Clear();
                        con.Close();
                    }
                }
            }
            catch (Exception er)
            {
                var msgError = "El reporte no se pudo generar, por favor intente de nuevo: " + er.Message;
            }
        }

        private static void AppendDataToExcel_IndDes(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string reportTemplate, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? AreaId)         //#### CAFQ
        {
            ExcelWorksheet excelWorksheet1 = excelWorkBook.Worksheets["DatosPie"];
            var row = startRow;
            var column = startColumn;

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            decimal[] nDato1 = new decimal[] { 0 };
            decimal[] nDato2 = new decimal[] { 0 };
            string[,] aDato3 = new string[6, 2];       // 6 Filas, 2 Columnas
            decimal[] nDato4 = new decimal[] { 0 };
            decimal[] nDato5 = new decimal[] { 0 };
            decimal[] nDato6 = new decimal[] { 0 };
            string[,] aDato7 = new string[6, 2];
            decimal[] nDato8 = new decimal[] { 0 };
            decimal[] nDato9 = new decimal[] { 0 };
            decimal[] nDato10 = new decimal[] { 0 };
            decimal[] nDato11 = new decimal[] { 0 };
            decimal[] nDato12 = new decimal[] { 0 };
            decimal[] nDato14 = new decimal[] { 0 };

            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 1, nDato1);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 2, nDato2);
            //recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 3, nDato1, aDato3);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 4, nDato4);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 5, nDato5);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 6, nDato6);
            //recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 7, nDato1, aDato7);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 8, nDato8);

            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 9, nDato9);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 10, nDato10);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 11, nDato11);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 12, nDato12);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 14, nDato14);
            //****
            var excelWorksheet2 = excelWorkBook.Worksheets[1];

            //**** Metas
            ArrayList aMetas = new ArrayList();
            ID_recuperarMetas(aMetas, countryId);

            double[] xx = (double[])aMetas[0];                      // Metas
            String[] yy = (String[])aMetas[1];                      // Unidades de la meta

            for (int nI = 0; nI < xx.Length; ++nI)
                excelWorksheet2.Cells[row + nI, column].Value = ID_formatearMeta(xx[nI], yy[nI], countryId, languaje_);

            //**** resultados
            double nTemp = 0;
            row = startRow;

            if (nDato1[0] != 0)
            {
                //nTemp = (double)(nDato4[0] / nDato1[0] * 100);
                nTemp = Math.Round((double)(nDato4[0] / nDato1[0] * 100), 0);
                excelWorksheet2.Cells[row, column + 1].Value = ID_formatearMeta(nTemp, yy[0], countryId, languaje_);
                ID_setResultados(excelWorksheet1, nTemp, xx[0], 2, yy[0]);
            }

            if (nDato8[0] != 0)
            {
                //nTemp = (double)(nDato5[0] / nDato8[0] * 100);
                nTemp = Math.Round((double)(nDato5[0] / nDato8[0] * 100), 0);
                excelWorksheet2.Cells[row + 1, column + 1].Value = ID_formatearMeta(nTemp, yy[1], countryId, languaje_);
                ID_setResultados(excelWorksheet1, nTemp, xx[1], 3, yy[1]);
            }

            if (nDato1[0] != 0)
            {
                //nTemp = (double)(nDato6[0] / nDato1[0] * 100);
                nTemp = Math.Round((double)(nDato6[0] / nDato1[0] * 100), 0);
                excelWorksheet2.Cells[row + 2, column + 1].Value = ID_formatearMeta(nTemp, yy[2], countryId, languaje_);
                ID_setResultados(excelWorksheet1, nTemp, xx[2], 4, yy[2]);
            }

            nTemp = (double)nDato9[0];
            excelWorksheet2.Cells[row + 3, column + 1].Value = ID_formatearMeta(nTemp, yy[3], countryId, languaje_);
            ID_setResultados(excelWorksheet1, nTemp, xx[3], 5, yy[3]);

            nTemp = (double)nDato10[0];
            excelWorksheet2.Cells[row + 4, column + 1].Value = ID_formatearMeta(nTemp, yy[4], countryId, languaje_);
            ID_setResultados(excelWorksheet1, nTemp, xx[4], 6, yy[4]);

            nTemp = (double)nDato11[0];
            excelWorksheet2.Cells[row + 5, column + 1].Value = ID_formatearMeta(nTemp, yy[5], countryId, languaje_);
            ID_setResultados(excelWorksheet1, nTemp, xx[5], 7, yy[5]);

            //nTemp = (double)nDato12[0] * 24.0;
            nTemp = Math.Round((double)nDato12[0], 0) * 24.0;
            excelWorksheet2.Cells[row + 6, column + 1].Value = ID_formatearMeta(nTemp, yy[6], countryId, languaje_);
            ID_setResultados(excelWorksheet1, nTemp, xx[6], 8, yy[6]);

            nTemp = (double)nDato14[0];
            excelWorksheet2.Cells[row + 7, column + 1].Value = ID_formatearMeta(nTemp, yy[7], countryId, languaje_);
            ID_setResultados(excelWorksheet1, nTemp, xx[7], 9, yy[7]);

            //****
            reportLabels(consString, countryId, languaje_, ReportCountry, hospitalId, year, YearFrom, YearTo, se, startDate, endDate, excelWorkBook, excelWorksheet2);
            InsertarImagenLogo(consString, reportTemplate, ReportCountry, excelWorksheet2);
        }

        private static string getDescripcionDatoDesdeID(SqlConnection con, string query, string columnName, int? nID)
        {
            string dato = "";

            //using (var command2 = new SqlCommand("select * from Institution where ID = @InstitutionID", con))
            using (var command2 = new SqlCommand(query, con))
            {
                command2.Parameters.Clear();
                command2.Parameters.Add("@ID", SqlDbType.Int).Value = nID;
                using (var reader2 = command2.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        dato += reader2[columnName];
                    }
                }
                command2.Parameters.Clear();
            }

            return dato;
        }

        private static string getSemanasEpidemiologicasReporte(int? year, int? yearFrom, int? yearTo, int? se, DateTime? startDate, DateTime? endDate, int? countryId, string languaje)
        {
            DateTime dtCurr = DateTime.Today;
            string labelSemanas = "";
            //****
            if (startDate.HasValue && endDate.HasValue)
            {

            }
            else if (year > 0 && se > 0)
            {
                if (year == dtCurr.Year)
                    labelSemanas = se.ToString();
                else
                    //labelSemanas = year.ToString() + " - " + se.ToString();     //
                    labelSemanas = year.ToString() + " " + SgetMsg("msgViewExportarLabelTo", countryId, languaje) + " " + se.ToString();
                //
            }
            else if (yearFrom > 0 && yearTo > 0)
            {
                int nUlSe = 0;

                if (yearTo == dtCurr.Year)
                    nUlSe = PAHOClassUtilities.semanasActualEpidemiologico();
                else
                    nUlSe = PAHOClassUtilities.semanasAnioEpidemiologico((int)yearTo);

                labelSemanas = yearFrom.ToString() + "." + "1" + " " + SgetMsg("msgViewExportarLabelTo", countryId, languaje) + " " + yearTo.ToString() + "." + nUlSe.ToString();
            }
            else if (year > 0)
            {
                if (year == dtCurr.Year)
                {
                    int nSemanas = PAHOClassUtilities.semanasActualEpidemiologico();
                    labelSemanas = "1" + " " + SgetMsg("msgViewExportarLabelTo", countryId, languaje) + " " + nSemanas.ToString();
                }
                else if (year < dtCurr.Year)
                {
                    int nSemanas = PAHOClassUtilities.semanasAnioEpidemiologico((int)year);
                    labelSemanas = year.ToString() + " " + "1" + " " + SgetMsg("msgViewExportarLabelTo", countryId, languaje) + " " + nSemanas.ToString();
                }
            }

            return labelSemanas;
        }

        private static void recuperarDatosIndDes(string consString, string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int opcion, decimal[] nResuOut, string[,] aResuOut = null)         //#### CAFQ
        {
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("IndicDesemp", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                    command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                    command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                    command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                    command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                    command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;
                    command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;      //#### CAFQ
                    command.Parameters.Add("@opcion", SqlDbType.Int).Value = opcion;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        int nFila = 0;
                        while (reader.Read())
                        {
                            if (opcion == 3 || opcion == 7)
                            {
                                aResuOut[nFila, 0] = reader.GetValue(0).ToString();
                                aResuOut[nFila, 1] = reader.GetValue(1).ToString();
                                ++nFila;
                            }
                            else
                            {
                                /*if (reader.GetValue(0).ToString() == "")
                                {
                                    nResuOut[0] = Convert.ToDecimal(0);
                                }
                                else
                                {
                                    nResuOut[0] = Convert.ToDecimal(reader.GetValue(0));
                                }*/
                                nResuOut[0] = (reader.GetValue(0).ToString() == "") ? nResuOut[0] = Convert.ToDecimal(0) : Convert.ToDecimal(reader.GetValue(0));
                            }
                        }
                    }

                    command.Parameters.Clear();
                    con.Close();
                }
            }
        }

        private static void ID_recuperarMetas(ArrayList _aMetas, int _countryId)
        {
            double[] arr1 = new double[0];
            string[] arr2 = new string[0];

            string cMetas = ConfigurationManager.AppSettings["IndicadoresDesempenioMetas_" + _countryId.ToString()];
            if (cMetas == null)
                cMetas = "0:0:0:0:0:0:0:0";                         //"90%:100%:90%:2D:3D:1D:48H:7D"

            string[] aMetas = cMetas.Split(':');
            for (int nI = 0; nI < aMetas.Length; ++nI)
            {
                string cMeta = aMetas[nI].Substring(0, aMetas[nI].Length - 1);
                double nMeta = double.Parse(cMeta);
                string cUnid = aMetas[nI].Substring(aMetas[nI].Length - 1, 1);

                Array.Resize(ref arr1, nI + 1);
                Array.Resize(ref arr2, nI + 1);
                arr1[nI] = nMeta;
                arr2[nI] = cUnid;
            }

            _aMetas.Add(arr1);
            _aMetas.Add(arr2);
        }

        private static string ID_formatearMeta(double _Meta, string _Unid, int countryId, string languaje)
        {
            string cForma = "", cMeta = "";

            if (_Unid == "%")
            {
                cMeta = _Meta.ToString("##0", CultureInfo.InvariantCulture);
                cForma = cMeta + _Unid;
            }

            if (_Unid == "D")
            {
                cMeta = _Meta.ToString("##0", CultureInfo.InvariantCulture);
                cForma = (_Meta == 1) ? cMeta + " " + SgetMsg("msgViewExportarLabelDay", countryId, languaje) : cMeta + " " + SgetMsg("msgViewExportarLabelDays", countryId, languaje);
            }

            if (_Unid == "H")
            {
                cMeta = _Meta.ToString("##0", CultureInfo.InvariantCulture);
                cForma = (_Meta == 1) ? cMeta + " " + SgetMsg("msgViewExportarLabelHour", countryId, languaje) : cMeta + " " + SgetMsg("msgViewExportarLabelHours", countryId, languaje);
            }

            return cForma;
        }

        private static void ID_setResultados(ExcelWorksheet excelWorksheet, double nResu, double nMeta, int nRow, string tipo)
        {
            if (tipo == "%")
            {
                if (nResu < nMeta)
                {
                    excelWorksheet.Cells[nRow, 1].Value = 1;                // Rojo
                    excelWorksheet.Cells[nRow, 2].Value = 0;
                }
                else
                {
                    excelWorksheet.Cells[nRow, 1].Value = 0;
                    excelWorksheet.Cells[nRow, 2].Value = 1;                // Verde
                }
            }
            else        // "D", "H"
            {
                if (nResu <= nMeta)
                {
                    excelWorksheet.Cells[nRow, 1].Value = 0;                // Verde
                    excelWorksheet.Cells[nRow, 2].Value = 1;
                }
                else
                {
                    excelWorksheet.Cells[nRow, 1].Value = 1;
                    excelWorksheet.Cells[nRow, 2].Value = 0;                // Rojo
                }
            }
        }

        public JsonResult ListaCasosPorEnviarNPHL(string Report, int CountryID, int? HospitalID, int? Year, int? Month, int? SE,
            DateTime? StartDate, DateTime? EndDate, int? ReportCountry, int? RegionID, int? YearFrom, int? YearTo, int? Surv, bool? Inusual)
        {
            List<string[]> casoDatosList = new List<string[]>();

            var user = UserManager.FindById(User.Identity.GetUserId());
            var countryId = CountryID;
            var languaje_ = user.Institution.Country.Language;

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("MuestrasLabNPHL", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1200 })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = HospitalID;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = Year;
                    command.Parameters.Add("@Mes_", SqlDbType.Int).Value = Month;
                    command.Parameters.Add("@SE", SqlDbType.Int).Value = SE;
                    command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = StartDate;
                    command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = EndDate;
                    command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = RegionID;
                    command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                    command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;
                    command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = Inusual;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] casoDatos = new string[4];

                            casoDatos[0] = reader.GetValue(3).ToString();       // ID caso
                            casoDatos[1] = reader.GetValue(0).ToString();       // ID NPHL
                            casoDatos[2] = reader.GetValue(2).ToString();       // Nombre
                            casoDatos[3] = reader.GetValue(1).ToString();       // Apellido

                            casoDatosList.Add(casoDatos);
                        }
                    }

                    command.Parameters.Clear();
                    con.Close();
                }
            }

            var jsonSerialiser = new JavaScriptSerializer();
            var jsonDatosLabNPHL = jsonSerialiser.Serialize(casoDatosList);

            return Json(jsonDatosLabNPHL);
        }

        private static void AppendDataToExcel_FLUID(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se,
            DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row,
            int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? AreaId = null, int? Sentinel = null)
        {

            var excelWorksheet = excelWorkBook.Worksheets[sheet];
            var row = startRow;

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand(storedProcedure, con) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                    command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                    command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                    command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                    command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                    command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;
                    command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;
                    command.Parameters.Add("@Area_ID", SqlDbType.Int).Value = AreaId;
                    command.Parameters.Add("@Sentinel", SqlDbType.Int).Value = Sentinel;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //var col = 1;
                            var col = startColumn;
                            //&& insert_row == true
                            if (row > startRow && insert_row == true) excelWorksheet.InsertRow(row, 1);

                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var cell = excelWorksheet.Cells[startRow, col];
                                if (reader.GetValue(i) != null)
                                {
                                    int number;
                                    bool isNumber = int.TryParse(reader.GetValue(i).ToString(), out number);

                                    if (isNumber)
                                    {
                                        excelWorksheet.Cells[row, col].Value = number;
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[row, col].Value = reader.GetValue(i).ToString();
                                    }
                                    excelWorksheet.Cells[row, col].StyleID = cell.StyleID;

                                }
                                col++;
                            }

                            row++;
                        }
                    }
                    command.Parameters.Clear();
                    con.Close();

                }
            }

            // Apply only if it has a Total row at the end and hast SUM in range, i.e. SUM(A1:A4)
            //excelWorksheet.DeleteRow(row, 2);
        }

        private void ConfigToExcel_FLUID(int countryId, string languaje_country, int? regionId, int? year, int? yearBegin, int? yearEnd, DateTime? StartDate, DateTime? EndDate,  int? hospitalId, int? Surv, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int sheet, bool? insert_row)
        {
            var excelWorksheet = excelWorkBook.Worksheets["Leyendas"];
            //var excelWorksheet = excelWorkBook.Worksheets[sheet];

            // Vigilancia

            if (storedProcedure != "Leyendas_FLUID")
            {
                if (Surv == 0)
                {
                    excelWorksheet.Cells[2, 7].Value =  ( languaje_country == "SPA" ) ? "IRAG y ETI" : "SARI and ILI";
                } else if (Surv == 1)
                 {
                    excelWorksheet.Cells[2, 7].Value = (languaje_country == "SPA") ? "IRAG" : "SARI";
                }
                else if (Surv == 2)
                {
                    excelWorksheet.Cells[2, 7].Value = (languaje_country == "SPA") ? "ETI" : "ILI";
                }
            }

            if (year != null)
            {
                excelWorksheet.Cells[2, 1].Value = year.ToString();
            }

            if (yearBegin != null)
            {
                excelWorksheet.Cells[2, 10].Value = yearBegin.ToString();
            }
            if (yearEnd != null)
            {
                excelWorksheet.Cells[2, 11].Value = yearEnd.ToString();
            }

            if (hospitalId != null && hospitalId > 0)
            {
                var Institution = db.Institutions.Find(hospitalId);

                excelWorksheet.Cells[2, 5].Value = Institution.Name.ToString();

            }
            else if (regionId != null && regionId > 0)
            {
                var Region_report = db.Regions.Find(regionId);

                excelWorksheet.Cells[2, 4].Value = Region_report.Name.ToString();
            }
            else
            {
                var Pais = db.Countries.Find(countryId);

                excelWorksheet.Cells[2, 3].Value = Pais.Name.ToString();
            }

            // EW calculate

            if (StartDate != null && EndDate != null)
            {
                if (StartDate.Value.Year == EndDate.Value.Year)
                {
                    excelWorksheet.Cells[2, 13].Value = GetWeekNumber (Convert.ToDateTime(StartDate));
                    excelWorksheet.Cells[2, 14].Value = GetWeekNumber(Convert.ToDateTime(EndDate));
                }
                else if (EndDate != null)
                {
                    excelWorksheet.Cells[2, 13].Value = 1;
                    excelWorksheet.Cells[2, 14].Value = GetWeekNumber(Convert.ToDateTime(EndDate));
                }
            }  else if ( yearEnd != null)
            {
                excelWorksheet.Cells[2, 13].Value = 1;
                if (yearEnd < DateTime.Now.Year )
                {
                    var date_dummy = new DateTime(Convert.ToInt16( yearEnd), 12, 26); 
                    
                    excelWorksheet.Cells[2, 14].Value = GetWeekNumber(date_dummy);
                } else
                {
                    excelWorksheet.Cells[2, 14].Value = GetWeekNumber(DateTime.Now);
                }
                
            } else if (year != null)
            {
                excelWorksheet.Cells[2, 13].Value = 1;
                if (year < DateTime.Now.Year)
                {
                    var date_dummy = new DateTime(Convert.ToInt16(year), 12, 26); ;

                    excelWorksheet.Cells[2, 14].Value = GetWeekNumber(date_dummy);
                }
                else
                {
                    excelWorksheet.Cells[2, 14].Value = GetWeekNumber(DateTime.Now);
                }

            }

        }

        public static int GetWeekNumber( DateTime date)
        {
            int daysToAdd = date.DayOfWeek != DayOfWeek.Sunday ? DayOfWeek.Thursday - date.DayOfWeek : (int)DayOfWeek.Thursday - 7;
            date = date.AddDays(daysToAdd);
            var currentCulture = CultureInfo.CurrentCulture;
            return currentCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private void ConfigGraph_FLUID(int? year, int count, ExcelWorkbook excelWorkBook, int sheet_graph, int sheet_data, string graph_name, string range_x, string range_y)
        {
            var excelWorksheet_graph = excelWorkBook.Worksheets[sheet_graph];
            var excelWorksheet_data = excelWorkBook.Worksheets[sheet_data];

            var LineChart = excelWorksheet_graph.Drawings[graph_name] as ExcelLineChart;
            var RangeStr = range_x + Convert.ToString(8 + (count * 52)) + ":" + range_x + Convert.ToString(59 + (count * 52));
            var RangeStr_2 = range_y + Convert.ToString(8 + (count * 52)) + ":" + range_y + Convert.ToString(59 + (count * 52));
            var Rangedata_X = excelWorksheet_data.Cells[RangeStr];
            var Rangedata_Y = excelWorksheet_data.Cells[RangeStr_2];

            if (LineChart != null)
            {
                var seriesCC = LineChart.Series.Add(Rangedata_Y, Rangedata_X);
                seriesCC.Header = year.ToString();
            }  

        }

        private void ConfigGraph_Bars_Histogram(int? year, ExcelWorkbook excelWorkBook, int sheet_graph, int sheet_data, string graph_name, int range_begin, int range_end)
        {
            var excelWorksheet_graph = excelWorkBook.Worksheets[sheet_graph];
            var excelWorksheet_data = excelWorkBook.Worksheets[sheet_data];

            var LineChart = excelWorksheet_graph.Drawings[graph_name] as ExcelChart;
            //LineChart.UseSecondaryAxis = true;
            var cs = LineChart.Series;
            var Secundary_cs = LineChart.PlotArea.ChartTypes;
            

            var begin_str = "";
            var letter_range_str = "";

            foreach (ExcelChartSerie element in cs)
            {
                begin_str = element.Series.Substring(0, element.Series.IndexOf(":") - (element.Series.Substring(0, element.Series.IndexOf(":")).Length - element.Series.Substring(0, element.Series.IndexOf(":")).LastIndexOf("$") - 1));
                letter_range_str = element.Series.Substring(element.Series.IndexOf("$") + 1 , element.Series.Substring(element.Series.IndexOf("$") + 1).IndexOf("$"));
                element.Series = begin_str + Convert.ToString(range_begin) + ":$" + letter_range_str + Convert.ToString(range_end) + ", " + element.Series ;
            }

            if (graph_name == "CV1" || graph_name == "CV3" || graph_name == "GS4" || graph_name == "GS6" || graph_name == "GS8" || graph_name == "CILI3" || graph_name == "CILI4" || graph_name == "CVILI1" || graph_name == "CVILI3")
            {
                foreach (ExcelChart TypeChart in Secundary_cs)
                {
                    if (TypeChart.GetType().Name == "ExcelLineChart")
                    {
                        var secundary_cs = TypeChart.Series;
                        foreach (ExcelChartSerie element_2 in secundary_cs)
                        {
                            begin_str = element_2.Series.Substring(0, element_2.Series.IndexOf(":") - (element_2.Series.Substring(0, element_2.Series.IndexOf(":")).Length - element_2.Series.Substring(0, element_2.Series.IndexOf(":")).LastIndexOf("$") - 1));
                            letter_range_str = element_2.Series.Substring(element_2.Series.IndexOf("$") + 1, element_2.Series.Substring(element_2.Series.IndexOf("$") + 1).IndexOf("$"));
                            element_2.Series = begin_str + Convert.ToString(range_begin) + ":$" + letter_range_str + Convert.ToString(range_end) + ", " + element_2.Series;

                        }
                    }
                }
            }

        }

        private void ConfigGraph_Pie(ExcelWorkbook excelWorkBook, int sheet_graph, int sheet_data, string graph_name, int range_begin, int range_end)
        {
            var excelWorksheet_graph = excelWorkBook.Worksheets[sheet_graph];
            var excelWorksheet_data = excelWorkBook.Worksheets[sheet_data];

            var LineChart = excelWorksheet_graph.Drawings[graph_name] as ExcelChart;
            //LineChart.UseSecondaryAxis = true;
            var cs = LineChart.Series;
            var Secundary_cs = LineChart.PlotArea.ChartTypes;


            var begin_str = "";
            var letter_range_str = "";
            var range_str = "";

            foreach (ExcelChartSerie element in cs)
            {
                range_str = element.Series.ToString();
                element.Series = range_str.Replace((range_begin - 52).ToString(), (range_begin).ToString());
            }

        }

        private void UpdateRangeXMLPath( ExcelChart char_, string range_)
        {
            var chartXml = char_.ChartXml;
            var nsm = new XmlNamespaceManager(chartXml.NameTable);

            var nsuri = chartXml.DocumentElement.NamespaceURI;
            nsm.AddNamespace("c", nsuri);

            var plotAreaNode = chartXml.SelectNodes("c:chartSpace/c:chart/c:plotArea", nsm);
            //Get the Series ref and its cat
            var serNode = chartXml.SelectNodes("//c:ser", nsm);
            foreach (XmlNode serNode_ind in serNode)
            {
                var multiLvlStrRef_f_Node = serNode_ind.SelectSingleNode("c:cat/c:multiLvlStrRef/c:f", nsm);

                if (multiLvlStrRef_f_Node != null)
                    multiLvlStrRef_f_Node.InnerXml = range_;

            }
        }

        private void CopyAndPasteRange( ExcelWorkbook excelWorkBook_Copy, int sheet_data_copy , ExcelWorkbook excelWorkBook_Paste, int sheet_data_paste, string range_copy, string range_paste)
        {
            var excelWorksheet_copy = excelWorkBook_Copy.Worksheets[sheet_data_copy];
            var excelWorksheet_paste = excelWorkBook_Paste.Worksheets[sheet_data_paste];

            var RangeStr = range_copy;
            var RangeStr_2 = range_paste;
            var Rangedata_copy = excelWorksheet_copy.Cells[RangeStr];
            var Rangedata_Y = excelWorksheet_paste.Cells[RangeStr_2];

            excelWorksheet_copy.Cells[range_copy].Copy(excelWorksheet_paste.Cells[RangeStr_2]);
            //excelWorksheet_paste.Cells.Worksheet.Workbook.Styles.UpdateXml();
        }


        private static void AppendDataToExcel_CONSDATA(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? AreaId = null, int? Sentinel = null, int? UCI = null, int? Fallecidos = null)
        {

            var excelWorksheet = excelWorkBook.Worksheets[sheet];
            var row = startRow;

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand(storedProcedure, con) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                    command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                    command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                    command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                    command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                    command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                    command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;
                    if (UCI > 0)
                        command.Parameters.Add("@UCI", SqlDbType.Int).Value = UCI;
                    if (Fallecidos > 0)
                        command.Parameters.Add("@Fallecidos", SqlDbType.Int).Value = Fallecidos;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //var col = 1;
                            var col = startColumn;
                            //&& insert_row == true
                            if (row > startRow && insert_row == true) excelWorksheet.InsertRow(row, 1);

                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var cell = excelWorksheet.Cells[startRow, col];
                                if (reader.GetValue(i) != null)
                                {
                                    int number;
                                    bool isNumber = int.TryParse(reader.GetValue(i).ToString(), out number);

                                    if (isNumber)
                                    {
                                        excelWorksheet.Cells[row, col].Value = number;
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[row, col].Value = reader.GetValue(i).ToString();
                                    }
                                    excelWorksheet.Cells[row, col].StyleID = cell.StyleID;

                                }
                                col++;
                            }

                            row++;
                        }
                    }
                    command.Parameters.Clear();
                    con.Close();

                }
            }

            // Apply only if it has a Total row at the end and hast SUM in range, i.e. SUM(A1:A4)
            //excelWorksheet.DeleteRow(row, 2);
        }

        private static void AppendDataToExcel_R4(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? AreaId = null, int? Sentinel = null)
        {

            var excelWorksheet = excelWorkBook.Worksheets[sheet];
            var row = startRow;

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand(storedProcedure, con) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                    command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                    command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                    command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                    command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                    command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                    command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;
                    command.Parameters.Add("@Area_ID", SqlDbType.Int).Value = AreaId;
                    command.Parameters.Add("@Sentinel", SqlDbType.Int).Value = Sentinel;

                    if (excelWorksheet.Name.IndexOf("_INF_") > 0)
                        command.Parameters.Add("@VirusType", SqlDbType.Text).Value = "INF";
                    else if (excelWorksheet.Name.IndexOf("_VSR_") > 0 || excelWorksheet.Name.IndexOf("_RSV_") > 0)
                        command.Parameters.Add("@VirusType", SqlDbType.Text).Value = "RSV";

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var col = 1;
                            //&& insert_row == true
                            if (row > startRow && insert_row == true) excelWorksheet.InsertRow(row, 1);

                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var cell = excelWorksheet.Cells[startRow, col];
                                if (reader.GetValue(i) != null)
                                {
                                    int number;
                                    bool isNumber = int.TryParse(reader.GetValue(i).ToString(), out number);

                                    if (isNumber)
                                    {
                                        excelWorksheet.Cells[row, col].Value = number;
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[row, col].Value = reader.GetValue(i).ToString();
                                    }
                                    excelWorksheet.Cells[row, col].StyleID = cell.StyleID;

                                }
                                col++;
                            }

                            row++;
                        }
                    }
                    command.Parameters.Clear();
                    con.Close();

                }
            }

            // Apply only if it has a Total row at the end and hast SUM in range, i.e. SUM(A1:A4)
            //excelWorksheet.DeleteRow(row, 2);
        }

        private void AppendDataToExcel_ConsolidadoCarga(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se,
            DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string reportTemplate, int startRow, int startColumn, int sheet, bool? insert_row,
            int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? AreaId, int? Sentinel)
        {

            //int count = nombres.Count(n => n.EndsWith("o"));
            int numAgeGroup = db.CatAgeGroup.Count(n => n.id_country == countryId);

            ExcelWorksheet excelWorksheet1 = excelWorkBook.Worksheets["Hosp."];
            ExcelWorksheet excelWorksheet2 = excelWorkBook.Worksheets["UCI"];
            ExcelWorksheet excelWorksheet3 = excelWorkBook.Worksheets["Fallecidos"];
            var row = startRow;
            var column = startColumn;
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("R8_ConsolidadoCarga", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                    command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                    command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                    command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                    command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                    command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;
                    command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;
                    command.Parameters.Add("@Area_ID", SqlDbType.Int).Value = AreaId;
                    command.Parameters.Add("@Sentinel", SqlDbType.Int).Value = Sentinel;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        int nFila = 0;
                        while (reader.Read())
                        {
                            // Hospitalizaciones
                            excelWorksheet1.Cells[row + nFila, column + 0].Value = reader.GetValue(2);
                            excelWorksheet1.Cells[row + nFila, column + 1].Value = reader.GetValue(2 + numAgeGroup);

                            excelWorksheet1.Cells[row + nFila, column + 3].Value = reader.GetValue(3);
                            excelWorksheet1.Cells[row + nFila, column + 4].Value = reader.GetValue(3 + numAgeGroup);

                            excelWorksheet1.Cells[row + nFila, column + 6].Value = reader.GetValue(4);
                            excelWorksheet1.Cells[row + nFila, column + 7].Value = reader.GetValue(4 + numAgeGroup);

                            excelWorksheet1.Cells[row + nFila, column + 9].Value = reader.GetValue(5);
                            excelWorksheet1.Cells[row + nFila, column + 10].Value = reader.GetValue(5 + numAgeGroup);

                            excelWorksheet1.Cells[row + nFila, column + 12].Value = reader.GetValue(6);
                            excelWorksheet1.Cells[row + nFila, column + 13].Value = reader.GetValue(6 + numAgeGroup);

                            excelWorksheet1.Cells[row + nFila, column + 15].Value = reader.GetValue(7);
                            excelWorksheet1.Cells[row + nFila, column + 16].Value = reader.GetValue(7 + numAgeGroup);
                            // UCI
                            excelWorksheet2.Cells[row + nFila, column + 0].Value = reader.GetValue(14);
                            excelWorksheet2.Cells[row + nFila, column + 1].Value = reader.GetValue(14 + numAgeGroup);

                            excelWorksheet2.Cells[row + nFila, column + 3].Value = reader.GetValue(15);
                            excelWorksheet2.Cells[row + nFila, column + 4].Value = reader.GetValue(15 + numAgeGroup);

                            excelWorksheet2.Cells[row + nFila, column + 6].Value = reader.GetValue(16);
                            excelWorksheet2.Cells[row + nFila, column + 7].Value = reader.GetValue(16 + numAgeGroup);

                            excelWorksheet2.Cells[row + nFila, column + 9].Value = reader.GetValue(17);
                            excelWorksheet2.Cells[row + nFila, column + 10].Value = reader.GetValue(17 + numAgeGroup);

                            excelWorksheet2.Cells[row + nFila, column + 12].Value = reader.GetValue(18);
                            excelWorksheet2.Cells[row + nFila, column + 13].Value = reader.GetValue(18 + numAgeGroup);

                            excelWorksheet2.Cells[row + nFila, column + 15].Value = reader.GetValue(19);
                            excelWorksheet2.Cells[row + nFila, column + 16].Value = reader.GetValue(19 + numAgeGroup);
                            // Fallecidos
                            excelWorksheet3.Cells[row + nFila, column + 0].Value = reader.GetValue(26);
                            excelWorksheet3.Cells[row + nFila, column + 1].Value = reader.GetValue(26 + numAgeGroup);

                            excelWorksheet3.Cells[row + nFila, column + 3].Value = reader.GetValue(27);
                            excelWorksheet3.Cells[row + nFila, column + 4].Value = reader.GetValue(27 + numAgeGroup);

                            excelWorksheet3.Cells[row + nFila, column + 6].Value = reader.GetValue(28);
                            excelWorksheet3.Cells[row + nFila, column + 7].Value = reader.GetValue(28 + numAgeGroup);

                            excelWorksheet3.Cells[row + nFila, column + 9].Value = reader.GetValue(29);
                            excelWorksheet3.Cells[row + nFila, column + 10].Value = reader.GetValue(29 + numAgeGroup);

                            excelWorksheet3.Cells[row + nFila, column + 12].Value = reader.GetValue(30);
                            excelWorksheet3.Cells[row + nFila, column + 13].Value = reader.GetValue(30 + numAgeGroup);

                            excelWorksheet3.Cells[row + nFila, column + 15].Value = reader.GetValue(31);
                            excelWorksheet3.Cells[row + nFila, column + 16].Value = reader.GetValue(31 + numAgeGroup);
                            //####
                            ++nFila;
                        }
                    }

                    command.Parameters.Clear();
                    con.Close();
                }
            }

            //****
            reportLabels(consString, countryId, languaje_, ReportCountry, hospitalId, year, YearFrom, YearTo, se, startDate, endDate, excelWorkBook,
                excelWorksheet1, AreaId, Sentinel, reportTemplate);
        }

        private void AppendDataToExcel_CasosPositivosConVacuna(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se,
            DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string reportTemplate, int startRow, int startColumn, int sheet, bool? insert_row,
            int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? AreaId, int? Sentinel)
        {
            ExcelWorksheet excelWorksheet1 = excelWorkBook.Worksheets[1];
            var row = startRow;
            var column = startColumn;
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("R11_CasosPositivosInfluenzaConVacuna", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                    command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                    command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                    command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                    command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                    command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;
                    command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;
                    command.Parameters.Add("@Area_ID", SqlDbType.Int).Value = AreaId;
                    command.Parameters.Add("@Sentinel", SqlDbType.Int).Value = Sentinel;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        int nFila = 0;
                        while (reader.Read())
                        {
                            // Hospitalizaciones
                            excelWorksheet1.Cells[row + nFila, column + 1].Value = reader.GetValue(1);
                            excelWorksheet1.Cells[row + nFila, column + 2].Value = reader.GetValue(2);
                            excelWorksheet1.Cells[row + nFila, column + 4].Value = reader.GetValue(3);

                            excelWorksheet1.Cells[row + nFila, column + 6].Value = reader.GetValue(4);
                            excelWorksheet1.Cells[row + nFila, column + 7].Value = reader.GetValue(5);
                            excelWorksheet1.Cells[row + nFila, column + 9].Value = reader.GetValue(6);

                            excelWorksheet1.Cells[row + nFila, column + 11].Value = reader.GetValue(7);
                            excelWorksheet1.Cells[row + nFila, column + 12].Value = reader.GetValue(8);
                            excelWorksheet1.Cells[row + nFila, column + 14].Value = reader.GetValue(9);
                            //####
                            ++nFila;
                        }
                    }

                    command.Parameters.Clear();
                    con.Close();
                }
            }

            //****
            reportLabels(consString, countryId, languaje_, ReportCountry, hospitalId, year, YearFrom, YearTo, se, startDate, endDate, excelWorkBook, excelWorksheet1, AreaId);
        }

        private static void reportLabels(string consString, int countryId, string languaje_, int? ReportCountry, int? hospitalId, int? year, int? YearFrom,
            int? YearTo, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, ExcelWorksheet excelWorksheet, int? AreaId = 0,
            int? Sentinel = null, string ReporteID = "")
        {
            //inserción de labels
            string labelWeekEpid = "";
            labelWeekEpid = getSemanasEpidemiologicasReporte(year, YearFrom, YearTo, se, startDate, endDate, countryId, languaje_);

            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("select * from ReportCountryConfig where ReportCountryID = @ReportCountryID", con))
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@ReportCountryID", SqlDbType.Int).Value = ReportCountry;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int insertrow = (int)reader["row"];
                            int insertcol = (int)reader["col"];
                            int tab = Int32.Parse((string)reader["tab"]);           // Hoja
                            /**************Inicia inserción de labels automáticos*******************/
                            /*Si en la base de datos, el label se encierra dentro de dobles llaves {{parametro}}, el parámetro se cambiará por el parámetro correspondiente de búsqueda ingresado en el formulario*/
                            /*Llenado parámetros que vienen del formulario*/
                            string labelCountry = "";
                            string labelHospital = "";
                            string labelArea = "";
                            string labelYear = "";
                            string labelSE = "";
                            string labelStartDate = "";
                            string labelEndDate = "";
                            string labelCurrDate = "";
                            string labelSentinel = "";
                            string labelVirus = "";

                            //****
                            if (countryId > 0)
                                labelCountry = getDescripcionDatoDesdeID(con, "select * from Country where ID = @ID", "Name", countryId);

                            if (hospitalId > 0)
                                labelHospital = getDescripcionDatoDesdeID(con, "select * from Institution where ID = @ID", "FullName", hospitalId);

                            if (AreaId > 0)
                                labelArea = getDescripcionDatoDesdeID(con, "select * from Area where ID = @ID", "Name", AreaId);

                            if (year > 0)
                                labelYear += year;

                            if (YearFrom > 0)
                                labelYear += SgetMsg("msgViewExportarLabelFromDesde", countryId, languaje_) + YearFrom + " ";

                            if (YearTo > 0)
                                labelYear += SgetMsg("msgViewExportarLabelToHasta", countryId, languaje_) + YearTo + " ";

                            if (se > 0)
                                labelSE += se;

                            if (startDate.HasValue)
                            {
                                labelStartDate += startDate.ToString();
                                DateTime oDate = Convert.ToDateTime(labelStartDate);
                                labelStartDate = oDate.Date.ToString();
                            }

                            if (endDate.HasValue)
                            {
                                labelEndDate += endDate.ToString();
                                DateTime oDate = Convert.ToDateTime(labelEndDate);
                                labelEndDate = oDate.Date.ToString();
                            }

                            if (Sentinel.HasValue)
                                labelSentinel = (Sentinel == 1) ? "Solo Unidades Centinela" : (Sentinel == 0) ? "Unidades No Centinela" : "";
                            else
                                labelSentinel = (ReporteID == "CPE" || ReporteID == "CC") ? "Todos" : "";

                            //****
                            var excelWs = excelWorksheet;

                            //****
                            int nVirus = 0;
                            string cTemp = "";
                            string label = reader["label"].ToString();

                            if (label != "")
                            {
                                //string[] tokens = label.Split(new[] { "{{" }, StringSplitOptions.None);
                                
                                int nNume = label.Split(new[] { "{{" }, StringSplitOptions.None).Length - 1;

                                for (int nI = 1; nI <= nNume; ++nI)
                                {
                                    nVirus = 0;
                                    string cOriginal = label;               //#### CAFQ: 
                                    int nResu1 = label.IndexOf("{{");       //#### CAFQ: 180415
                                    int nResu2 = label.IndexOf("}}");       //#### CAFQ: 180415

                                    if (nResu1 >= 0 && nResu2 >= 0)         //#### CAFQ: 180415
                                    {
                                        label = cOriginal.Substring(nResu1, nResu2 - nResu1 + 2);       //#### CAFQ: 180415
                                        string aaa = label.Substring(0, 7);
                                        if (label.Substring(0, 7) == "{{virus")         // Left 
                                        {
                                            cTemp = label.Substring(8, label.Length - 8 - 2);
                                            if (int.TryParse(cTemp, out nVirus))
                                            {
                                                labelVirus = getDescripcionDatoDesdeID(con, "select * from CatVirusType where ID = @ID", languaje_, nVirus);
                                                label = "{{virus}}";
                                            }
                                        }

                                        switch (label)
                                        {
                                            case "{{country}}":
                                                label = (labelCountry != "" ? (SgetMsg("msgViewExportarLabelCountry", countryId, languaje_) + ": " + labelCountry) : "");
                                                break;
                                            case "{{institution}}":
                                                label = (labelHospital != "" ? (SgetMsg("msgViewExportarLabelInstitutionShort", countryId, languaje_) + ": " + labelHospital) : "");
                                                break;
                                            case "{{Area}}":
                                                label = (labelArea != "" ? (SgetMsg("msgViewExportarLabelArea", countryId, languaje_) + ": " + labelArea) : "");
                                                break;
                                            case "{{year}}":
                                                label = (labelYear != "" ? (SgetMsg("msgViewExportarLabelYear", countryId, languaje_) + ": " + labelYear) : "");
                                                break;
                                            case "{{onlyYear}}":
                                                label = (labelYear != "" ? labelYear : "");
                                                break;
                                            case "{{se}}":
                                                label = (labelSE != "" ? (SgetMsg("msgViewExportarLabelEW", countryId, languaje_) + ": " + labelSE) : "");
                                                break;
                                            case "{{startDate}}":
                                                label = (labelStartDate != "" ? (SgetMsg("msgViewExportarLabelFrom", countryId, languaje_) + ": " + labelStartDate) : "");
                                                break;
                                            case "{{endDate}}":
                                                label = (labelEndDate != "" ? (SgetMsg("msgViewExportarLabelTo", countryId, languaje_) + ": " + labelEndDate) : "");
                                                break;
                                            case "{{fullinstitution}}":
                                                label = (labelCountry != "" ? (SgetMsg("msgViewExportarLabelCountry", countryId, languaje_) + ": " + labelCountry) : "") + " " + (labelHospital != "" ? (SgetMsg("msgViewExportarLabelInstitutionShort", countryId, languaje_) + ": " + labelHospital) : "");
                                                break;
                                            case "{{fulldate}}":
                                                label = (labelYear != "" ? (SgetMsg("msgViewExportarLabelEpidemiologicalYear", countryId, languaje_) + ": " + labelYear) : "") + " " + (labelSE != "" ? (SgetMsg("msgViewExportarLabelEW", countryId, languaje_) + ": " + labelSE) : "") + " " + (labelStartDate != "" ? (SgetMsg("msgViewExportarLabelFrom", countryId, languaje_) + ": " + labelStartDate) : "") + " " + (labelEndDate != "" ? (SgetMsg("msgViewExportarLabelTo", countryId, languaje_) + ": " + labelEndDate) : "");
                                                break;
                                            case "{{currentDate}}":
                                                DateTime oDate = DateTime.Today;
                                                labelCurrDate = String.Format("{0:" + SgetMsg("msgDateFormatReporting", countryId, languaje_) + "}", oDate);
                                                label = labelCurrDate;
                                                break;
                                            case "{{weekEpid}}":
                                                label = (labelWeekEpid != "" ? (SgetMsg("msgViewExportarLabelWeekEpidReporting", countryId, languaje_) + ": " + labelWeekEpid) : "");
                                                break;
                                            case "{{sentinel}}":
                                                label = (labelSentinel != "" ? labelSentinel : "");
                                                break;
                                            case "{{weekEpidOnly}}":
                                                label = "";
                                                if (labelWeekEpid != "")
                                                    label = (ReporteID == "LRD") ? labelWeekEpid.ToUpper() : labelWeekEpid;
                                                break;
                                            case "{{virus}}":
                                                label = (labelVirus != "" ? labelVirus : "");
                                                break;
                                            default:
                                                label = "";
                                                break;
                                        }
                                    }
                                    else
                                        label = "";

                                    label = cOriginal.Substring(0, nResu1) + label + cOriginal.Substring(nResu2 + 2, cOriginal.Length - nResu2 - 2);

                                }   // End For

                                if (tab != 1)
                                    excelWs = excelWorkBook.Worksheets[tab];

                                if (label != "")
                                    excelWs.Cells[insertrow, insertcol].Value = label;
                            }
                        }
                    }
                    command.Parameters.Clear();
                    con.Close();
                }
            }
        }

        private static void InsertarLogoExcel(string consString, ExcelWorksheet excelWorksheet, int? ReportCountry)
        {
            //inserción de logo
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("select * from ReportCountry where ID = @ReportCountryID", con))
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@ReportCountryID", SqlDbType.Int).Value = ReportCountry;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int insertrow = (int)reader["logoRow"];
                            int insertcol = (int)reader["logoCol"];
                            String imagurl = (String)reader["logo"];
                            if (imagurl != "")
                            {
                                try
                                {
                                    WebClient wc = new WebClient();
                                    byte[] bytes = wc.DownloadData(imagurl);
                                    MemoryStream ms = new MemoryStream(bytes);
                                    Bitmap newImage = new Bitmap(System.Drawing.Image.FromStream(ms));
                                    excelWorksheet.Row(insertrow).Height = newImage.Height;
                                    var picture = excelWorksheet.Drawings.AddPicture("reportlogo", newImage);
                                    picture.SetPosition(insertrow, -(newImage.Height), insertcol, -100);
                                    //picture.SetPosition(insertrow, 0, insertcol, 0);
                                }
                                catch (Exception e)
                                {
                                    //ViewBag.Message = "No se insertó el logo, porque no es accesible";
                                }
                            }
                        }
                    }
                    command.Parameters.Clear();
                    con.Close();
                }
            }   // End
        }

        private static void InsertarImagenLogo(string consString, string reportTemplate, int? reportCountry, ExcelWorksheet excelWorksheet)
        {
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("select * from ReportCountry where ID = @ReportCountryID", con))
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@ReportCountryID", SqlDbType.Int).Value = reportCountry;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int insertrow = (int)reader["logoRow"];
                            int insertcol = (int)reader["logoCol"];
                            String imagurl = (String)reader["logo"];
                            if (imagurl != "")
                            {
                                try
                                {
                                    /*
                                    //imagurl = "file:///C:/CAFQSource/PAHO/PAHOFLUCR_CLON/Paho/Content/themes/base/images/logo_pahowho_blue-en.png";     //#### SOLO DESARROLLO: CAFQ
                                    */
                                    WebClient wc = new WebClient();
                                    byte[] bytes = wc.DownloadData(imagurl);
                                    MemoryStream ms = new MemoryStream(bytes);
                                    Bitmap newImage = new Bitmap(System.Drawing.Image.FromStream(ms));
                                    excelWorksheet.Row(insertrow).Height = newImage.Height;
                                    var picture = excelWorksheet.Drawings.AddPicture("reportlogo", newImage);
                                    if (reportTemplate == "I1")
                                        picture.SetPosition(insertrow, -(newImage.Height), insertcol, 2);
                                    else
                                        picture.SetPosition(insertrow, -(newImage.Height), insertcol, -100);
                                    //picture.SetPosition(insertrow, 0, insertcol, 0);
                                }
                                catch (Exception e)
                                {
                                    //ViewBag.Message = "No se insertó el logo, porque no es accesible";
                                }
                            }
                        }
                    }
                    command.Parameters.Clear();
                    con.Close();
                }
            }
        }

        public string getMsg(string msgView)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            string searchedMsg = msgView;
            int? countryID = user.Institution.CountryID;
            string countryLang = user.Institution.Country.Language;

            ResourcesM myR = new ResourcesM();
            searchedMsg = myR.getMessage(searchedMsg, countryID, countryLang);
            //searchedMsg = myR.getMessage(searchedMsg, 0, "ENG");
            return searchedMsg;
        }

        private static string SgetMsg(string msgView, int? countryDisp, string langDisp)
        {
            string searchedMsg = ResourcesM.SgetMessage(msgView, countryDisp, langDisp);
            //searchedMsg = myR.getMessage(searchedMsg, 0, "ENG");
            return searchedMsg;
        }
    }
}
