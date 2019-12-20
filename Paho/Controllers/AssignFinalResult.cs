using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Paho.Models;
using System.Web.Script.Serialization;
using System.Data.Entity.Validation;
using System.Globalization;
using Microsoft.AspNet.Identity;
using System.Data.Entity.SqlServer;
using System.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace Paho.Controllers
{
    [Authorize]
    public class AssignFinalResultController : ControllerBase
    {

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult AssignFinalResult_Total(
                                        int? SPinstitutionId,
                                        int? PRecordId,
                                        int? PYear,
                                        int PCountry
                                        )
        {
            //IEnumerable<FluCase> flucases;
            IQueryable<FluCase> flucases = null;
            var InstitutionDB = SPinstitutionId;
            var RecordDB = PRecordId;
            var YearDB = PYear;
            var CountryDB = PCountry; 
            var user = UserManager.FindById(User.Identity.GetUserId());
            var UsrCtry = user.Institution.CountryID;
            var UsrAccessLevel = user.Institution.AccessLevel;
            var UsrRegion = user.Institution.cod_region_institucional;
            var UsrArea = user.Institution.AreaID;
            var UsrInstitution = user.Institution.ID;
            string language = user.Institution.Country.Language.ToString();

            flucases = db.FluCases.Where(f => f.CountryID == CountryDB);

            if (RecordDB != null && RecordDB > 0)
                flucases = flucases.Where(x => x.ID == RecordDB);

            if (YearDB != null && YearDB > 0)
                flucases = flucases.Where(y => y.HospitalDate.Year == YearDB);


            if (InstitutionDB != null && InstitutionDB > 0)
                flucases = flucases.Where(x => x.HospitalID == InstitutionDB);

            // prueba

            //flucases = flucases.Where(z => z.ID == 96097);


            var toArrayClone = flucases.ToArray();

            foreach (FluCase Record_ in toArrayClone)
                {
                var List_Test  = Record_.CaseLabTests.OfType<CaseLabTest>()
                                                    .Where(z => z.TestResultID_VirusSubType == Convert.ToString('P') || z.TestResultID_VirusSubType == null || z.TestResultID_VirusSubType_2 == Convert.ToString('P') || z.TestResultID_VirusSubType_2 == null)
                                                    .OrderBy(z=> z.Institution.OrdenPrioritybyLab == null ? 99 : z.Institution.OrdenPrioritybyLab)
                                                    .ThenBy(z=>z.CatTestResult == null ? 99 : z.CatTestResult.orden)
                                                    .ThenBy(x => x.CatTestType == null ? 99 : x.CatTestType.orden )
                                                    .ThenBy(y => y.CatVirusType == null ? 99 : y.CatVirusType.orden )
                                                    .ThenBy(z => z.CatVirusSubType == null ? 99 : z.CatVirusSubType.orden )
                                                    .ThenBy(x=> x.CatVirusSubType_2 == null ? 99 : x.CatVirusSubType_2.orden )
                                                    .ThenBy(y => y.CatVirusLinaje == null ? 99 :  y.CatVirusLinaje.orden )
                                                    .ToList();

                //var List_Save = Record_.CaseLabTests.OfType<CaseLabTest>().ToArray();

                //foreach (CaseLabTest Test_ in List_Save.Where(z => z.inst_conf_end_flow_by_virus == 0)
                //      .OrderBy(x => x.SampleNumber )
                //      .ThenBy(z => z.TestDate)
                //      .ThenBy(y => y.LabID))
                //{
                //    var Institution_Conf_Original = db.InstitutionsConfiguration.Where(i => i.InstitutionToID == Test_.LabID && i.InstitutionParentID == Record_.HospitalID).Any() ? db.InstitutionsConfiguration.Where(i => i.InstitutionToID == Test_.LabID && i.InstitutionParentID == Record_.HospitalID).FirstOrDefault().ID : 0;
                //    var Inst_Conf_End_Flow_By_Virus = Test_.TestResultID == "P" ? db.InstitutionConfEndFlowByVirus.Where(j => j.id_Lab == Test_.LabID && j.id_Cat_TestType == Test_.TestType && j.value_Cat_TestResult == Test_.TestResultID && j.id_Cat_VirusType == Test_.VirusTypeID).Any() ? db.InstitutionConfEndFlowByVirus.Where(j => j.id_Lab == Test_.LabID && j.id_Cat_TestType == Test_.TestType && j.value_Cat_TestResult == Test_.TestResultID && j.id_Cat_VirusType == Test_.VirusTypeID).FirstOrDefault().ID : 0 : db.InstitutionConfEndFlowByVirus.Where(j => j.id_Lab == Test_.LabID && j.id_Cat_TestType == Test_.TestType && j.value_Cat_TestResult == Test_.TestResultID).Any() ? db.InstitutionConfEndFlowByVirus.Where(j => j.id_Lab == Test_.LabID && j.id_Cat_TestType == Test_.TestType && j.value_Cat_TestResult == Test_.TestResultID).FirstOrDefault().ID : db.InstitutionConfEndFlowByVirus.Where(j => j.id_Lab == Test_.LabID && j.id_Cat_TestType == Test_.TestType && j.value_Cat_TestResult == Test_.TestResultID).Any() ? db.InstitutionConfEndFlowByVirus.Where(j => j.id_Lab == Test_.LabID && j.id_Cat_TestType == Test_.TestType && j.value_Cat_TestResult == Test_.TestResultID).FirstOrDefault().ID : 0;

                //        Test_.inst_cnf_orig = Institution_Conf_Original;
                //        Test_.inst_conf_end_flow_by_virus = Inst_Conf_End_Flow_By_Virus;


                //}

                //if (List_Save.Count() > 0)
                //    db.SaveChanges();


                if (List_Test.Count > 0)
                {
                    if (List_Test[0].TestResultID == Convert.ToString('N'))
                    {
                        FluCase c = (from x in db.FluCases
                                     where x.ID == Record_.ID
                                     select x).First();
                        c.FinalResult = Convert.ToString('N');
                        c.FinalResultVirusTypeID = null;
                        c.FinalResultVirusSubTypeID = null;
                        c.FinalResultVirusLineageID = null;

                        c.FinalResult_2 = null;
                        c.FinalResultVirusTypeID_2 = null;
                        c.FinalResultVirusSubTypeID_2 = null;
                        c.FinalResultVirusLineageID_2 = null;

                        c.FinalResult_3 = null;
                        c.FinalResultVirusTypeID_3 = null;
                        c.FinalResultVirusSubTypeID_3 = null;
                        c.FinalResultVirusLineageID_3 = null;
                        db.SaveChanges();
                        //break;
                    }
                    else if (List_Test[0].TestResultID == Convert.ToString('P'))
                    {
                        FluCase c = (from x in db.FluCases
                                     where x.ID == Record_.ID
                                     select x).First();
                        c.FinalResult = List_Test[0].TestResultID;
                        c.FinalResultVirusTypeID = List_Test[0].VirusTypeID;
                        if (List_Test[0].TestResultID_VirusSubType == Convert.ToString('P'))
                        {
                            c.FinalResultVirusSubTypeID = List_Test[0].VirusSubTypeID;
                        }
                        else if (List_Test[0].TestResultID_VirusSubType_2 == Convert.ToString('P'))
                        {
                            c.FinalResultVirusSubTypeID = List_Test[0].VirusSubTypeID_2;
                        }
                        c.FinalResultVirusLineageID = List_Test[0].VirusLineageID;

                        var List_result_2 = List_Test.Where(z => z.VirusTypeID != List_Test[0].VirusTypeID)
                                                    .OrderBy(z => z.Institution.OrdenPrioritybyLab == null ? 99 : z.Institution.OrdenPrioritybyLab)
                                                    .ThenBy(z => z.CatTestResult == null ? 99 : z.CatTestResult.orden)
                                                    .ThenBy(x => x.CatTestType == null ? 99 : x.CatTestType.orden)
                                                    .ThenBy(y => y.CatVirusType == null ? 99 : y.CatVirusType.orden)
                                                    .ThenBy(z => z.CatVirusSubType == null ? 99 : z.CatVirusSubType.orden)
                                                    .ThenBy(x => x.CatVirusSubType_2 == null ? 99 : x.CatVirusSubType_2.orden)
                                                    .ThenBy(y => y.CatVirusLinaje == null ? 99 : y.CatVirusLinaje.orden)
                                                    .ToList();

                        if (List_result_2.Count > 0)
                        {
                            var List_result_2_1 = List_result_2
                                                    .OrderBy(z => z.Institution.OrdenPrioritybyLab == null ? 99 : z.Institution.OrdenPrioritybyLab)
                                                    .ThenBy(z => z.CatTestResult == null ? 99 : z.CatTestResult.orden)
                                                    .ThenBy(x => x.CatTestType == null ? 99 : x.CatTestType.orden)
                                                    .ThenBy(y => y.CatVirusType == null ? 99 : y.CatVirusType.orden)
                                                    .ThenBy(z => z.CatVirusSubType == null ? 99 : z.CatVirusSubType.orden)
                                                    .ThenBy(x => x.CatVirusSubType_2 == null ? 99 : x.CatVirusSubType_2.orden)
                                                    .ThenBy(y => y.CatVirusLinaje == null ? 99 : y.CatVirusLinaje.orden)
                                                    .ToList();

                            var List_result_2_1_1 = List_result_2_1;
                            var List_result_3 = List_result_2_1;

                            if (List_result_2[0].TestResultID == Convert.ToString('N'))
                            {

                                if (List_result_2.Where(z=> z.TestResultID == Convert.ToString('N')).Count() != List_result_2.Count())
                                {
                                    List_result_2_1 = List_result_2.Where(z => z.VirusTypeID != List_result_2[0].VirusTypeID)
                                                                   .OrderBy(z => z.Institution.OrdenPrioritybyLab == null ? 99 : z.Institution.OrdenPrioritybyLab)
                                                                   .ThenBy(z => z.CatTestResult == null ? 99 : z.CatTestResult.orden)
                                                                   .ThenBy(x => x.CatTestType == null ? 99 : x.CatTestType.orden)
                                                                   .ThenBy(y => y.CatVirusType == null ? 99 : y.CatVirusType.orden)
                                                                   .ThenBy(z => z.CatVirusSubType == null ? 99 : z.CatVirusSubType.orden)
                                                                   .ThenBy(x => x.CatVirusSubType_2 == null ? 99 : x.CatVirusSubType_2.orden)
                                                                   .ThenBy(y => y.CatVirusLinaje == null ? 99 : y.CatVirusLinaje.orden)
                                                                   .ToList();
                                    if (List_result_2_1.Count > 0)
                                    {
                                        if (List_result_2_1[0].TestResultID == Convert.ToString('N'))
                                        {
                                            List_result_2_1_1 = List_result_2_1.Where(z => z.VirusTypeID != List_result_2_1[0].VirusTypeID)
                                                                   .OrderBy(z => z.Institution.OrdenPrioritybyLab == null ? 99 : z.Institution.OrdenPrioritybyLab)
                                                                   .ThenBy(z => z.CatTestResult == null ? 99 : z.CatTestResult.orden)
                                                                   .ThenBy(x => x.CatTestType == null ? 99 : x.CatTestType.orden)
                                                                   .ThenBy(y => y.CatVirusType == null ? 99 : y.CatVirusType.orden)
                                                                   .ThenBy(z => z.CatVirusSubType == null ? 99 : z.CatVirusSubType.orden)
                                                                   .ThenBy(x => x.CatVirusSubType_2 == null ? 99 : x.CatVirusSubType_2.orden)
                                                                   .ThenBy(y => y.CatVirusLinaje == null ? 99 : y.CatVirusLinaje.orden)
                                                                   .ToList();
                                        }
                                        else
                                        {
                                            c.FinalResult_2 = List_result_2_1[0].TestResultID;
                                            c.FinalResultVirusTypeID_2 = List_result_2_1[0].VirusTypeID;
                                            if (List_result_2_1[0].TestResultID_VirusSubType == Convert.ToString('P'))
                                            {
                                                c.FinalResultVirusSubTypeID_2 = List_result_2_1[0].VirusSubTypeID;
                                            }
                                            else if (List_result_2_1[0].TestResultID_VirusSubType_2 == Convert.ToString('P'))
                                            {
                                                c.FinalResultVirusSubTypeID_2 = List_result_2_1[0].VirusSubTypeID_2;
                                            } 
                                            c.FinalResultVirusLineageID_2 = List_result_2_1[0].VirusLineageID;

                                            // Aquí empieza el resultado 3

                                            List_result_3 = List_result_2_1.Where(z => z.VirusTypeID != List_result_2_1[0].VirusTypeID)
                                                                   .OrderBy(z => z.Institution.OrdenPrioritybyLab == null ? 99 : z.Institution.OrdenPrioritybyLab)
                                                                   .ThenBy(z => z.CatTestResult == null ? 99 : z.CatTestResult.orden)
                                                                   .ThenBy(x => x.CatTestType == null ? 99 : x.CatTestType.orden)
                                                                   .ThenBy(y => y.CatVirusType == null ? 99 : y.CatVirusType.orden)
                                                                   .ThenBy(z => z.CatVirusSubType == null ? 99 : z.CatVirusSubType.orden)
                                                                   .ThenBy(x => x.CatVirusSubType_2 == null ? 99 : x.CatVirusSubType_2.orden)
                                                                   .ThenBy(y => y.CatVirusLinaje == null ? 99 : y.CatVirusLinaje.orden)
                                                                   .ToList();

                                            if (List_result_3 != null)
                                            {
                                                if (List_result_3[0].TestResultID == Convert.ToString('P'))
                                                {
                                                    c.FinalResult_3 = List_result_3[0].TestResultID;
                                                    c.FinalResultVirusTypeID_3 = List_result_3[0].VirusTypeID;
                                                    if (List_result_3[0].TestResultID_VirusSubType == Convert.ToString('P'))
                                                    {
                                                        c.FinalResultVirusSubTypeID_3 = List_result_3[0].VirusSubTypeID;
                                                    }
                                                    else if (List_result_3[0].TestResultID_VirusSubType_2 == Convert.ToString('P'))
                                                    {
                                                        c.FinalResultVirusSubTypeID_3 = List_result_3[0].VirusSubTypeID_2;
                                                    }
                                                    c.FinalResultVirusLineageID_3 = List_result_3[0].VirusLineageID;
                                                }
                                            }


                                        }
                                    }
                                } else
                                {
                                    c.FinalResult_2 = null;
                                    c.FinalResultVirusTypeID_2 = null;
                                    c.FinalResultVirusSubTypeID_2 = null;
                                    c.FinalResultVirusLineageID_2 = null;

                                    c.FinalResult_3 = null;
                                    c.FinalResultVirusTypeID_3 = null;
                                    c.FinalResultVirusSubTypeID_3 = null;
                                    c.FinalResultVirusLineageID_3 = null;
                                }


                            } else {

                                c.FinalResult = List_result_2[0].TestResultID;
                                c.FinalResultVirusTypeID = List_result_2[0].VirusTypeID;
                                c.FinalResultVirusSubTypeID = List_result_2[0].VirusSubTypeID;
                                c.FinalResultVirusLineageID = List_result_2[0].VirusLineageID; 
                            }
                        }

                        db.SaveChanges();
                    }
                }
            }

            return Json("Success");
        }
        }
}
