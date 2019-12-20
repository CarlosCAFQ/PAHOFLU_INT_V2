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
// Librería XML
using System.Xml.Linq;

namespace Paho.Controllers
{
    [Authorize]
    public class CasesController : ControllerBase
    {

        public void HistoryRecord(int? RecordId, int Action_history, int? flow, int? state)
        {
            RecordHistory history;
            //IQueryableToXML(RecordId);
            history = new RecordHistory(); 
            history.Action = Action_history;
            history.Recordid = RecordId;
            history.Userid = User.Identity.GetUserId();
            history.flow = flow;
            history.state = state;
            history.DateAction = DateTime.Now;
            db.Entry(history).State = EntityState.Added;
            db.SaveChanges();
        }

        public ActionResult GetCountries()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            var countries = db.Countries
                .Select(c => new
                {
                    Id = c.ID.ToString(),
                    Name = (user.Institution.Country.Language == "SPA") ? c.Name : c.ENG,
                    Active = c.Active
                })
                .OrderBy(c => c.Name)
                .ToArray();

            return Json(countries, JsonRequestBehavior.AllowGet);
        }

        private IQueryable<Institution> GetInstitutionByRegion(long institutionId)
        {
            var institutionsConfiguration = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>()
                    .Where(i => i.InstitutionFromID == institutionId && i.Priority == 1);

            var institutionLabs = institutionsConfiguration.Select(i => i.InstitutionTo);
            if (!institutionLabs.Any())
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                institutionLabs = db.Institutions.OfType<Lab>().Where(i => i.CountryID == user.Institution.CountryID);
            }

            return institutionLabs;
        }

        // GET: Regiones
        public JsonResult GetRegions(int CountryID, int? AreaID, int? RegionID)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var UsrCtry = CountryID > 0 ? CountryID : user.Institution.CountryID;
            //Refrescar institucion en el combo segun la region a la que pertenecen
            var regions =
             (
              from Region in db.Regions as IQueryable<Region>
              where Region.CountryID == UsrCtry && (Region.tipo_region == 1 || Region.tipo_region == null)
              select new
              {
                  Id = Region.ID,
                  //Id = Region.orig_country,
                  Name = Region.Name
                  }).ToArray();

            if (user.type_region != 1 && user.type_region != null)
            {
                regions =
             (
              from Region in db.Regions as IQueryable<Region>
              where Region.CountryID == UsrCtry && (Region.tipo_region == user.type_region )
              select new
              {
                  Id = Region.ID,
                  //Id = Region.orig_country,
                  Name = Region.Name
              }).ToArray();
            }


                return Json(regions, JsonRequestBehavior.AllowGet);
        }

        // GET: GetHospitals
        public JsonResult GetInstitutions(int? CountryID, int? AreaID, int? RegionID)
        {

            var user = UserManager.FindById(User.Identity.GetUserId());
            var UsrCtry = user.Institution.CountryID;

            if (AreaID != null && AreaID > 0)
            {
                var institutions =
                 (
                  from institution in db.Institutions as IQueryable<Institution>
                  where institution.CountryID == UsrCtry && institution.AreaID == AreaID && institution is Hospital
                  select new
                  {
                      Id = institution.ID,
                      Name = institution.Name,
                      InstitutionType = institution is Hospital ? InstitutionType.Hospital : InstitutionType.Lab
                      //InstitutionType = InstitutionType.Hospital
                  }).ToArray();


                var institutionsDisplay = institutions.Select(i => new LookupView<Institution>()
                {
                    Id = i.Id.ToString(),
                    Name = i.Name
                }).ToList();

                if (institutions.Count() > 1)
                {
                    var all = new LookupView<Institution> { Id = "0", Name = getMsg("msgGeneralMessageAll") };
                    institutionsDisplay.Insert(0, all);
                }
                else if (institutions.Count() == 0)
                {
                    var all = new LookupView<Institution> { Id = "0", Name = "-- Sin unidades centinela --" };
                    institutionsDisplay.Insert(0, all);
                }
                return Json(institutionsDisplay, JsonRequestBehavior.AllowGet);

            }
            else if (RegionID != null  && RegionID > 0)
            {

                //Refrescar institucion en el combo segun la region a la que pertenecen

                var institutions =
                 (
                  from institution in db.Institutions as IQueryable<Institution>
                  where institution.CountryID == UsrCtry && institution is Hospital
                  select new
                  {
                      Id = institution.ID,
                      Name = institution.Name,
                      InstitutionType = institution is Hospital ? InstitutionType.Hospital : InstitutionType.Lab
                      //InstitutionType = InstitutionType.Hospital
                  }).ToArray();

                if ((user.type_region == 1 || user.type_region == null) && RegionID > 0)
                {
                    institutions =
                 (
                  from institution in db.Institutions as IQueryable<Institution>
                  where institution.CountryID == UsrCtry && institution.cod_region_institucional == RegionID && institution is Hospital
                  select new
                  {
                      Id = institution.ID,
                      Name = institution.Name,
                      InstitutionType = institution is Hospital ? InstitutionType.Hospital : InstitutionType.Lab
                      //InstitutionType = InstitutionType.Hospital
                  }).ToArray();
                }
                

                if ( user.type_region == 2)
                {
                    institutions =
                 (
                  from institution in db.Institutions as IQueryable<Institution>
                  where institution.CountryID == UsrCtry && institution.cod_region_salud == RegionID && institution is Hospital
                  select new
                  {
                      Id = institution.ID,
                      Name = institution.Name,
                      InstitutionType = institution is Hospital ? InstitutionType.Hospital : InstitutionType.Lab
                      //InstitutionType = InstitutionType.Hospital
                  }).ToArray();
                }
                else if (user.type_region == 3)
                {
                    institutions =
                 (
                  from institution in db.Institutions as IQueryable<Institution>
                  where institution.CountryID == UsrCtry && institution.cod_region_pais == RegionID && institution is Hospital
                  select new
                  {
                      Id = institution.ID,
                      Name = institution.Name,
                      InstitutionType = institution is Hospital ? InstitutionType.Hospital : InstitutionType.Lab
                      //InstitutionType = InstitutionType.Hospital
                  }).ToArray();
                }

                var institutionsDisplay = institutions.Select(i => new LookupView<Institution>()
                {
                    Id = i.Id.ToString(),
                    Name = i.Name
                }).ToList();

                if (institutions.Count() > 1)
                {
                    var all = new LookupView<Institution> { Id = "0", Name = getMsg("msgGeneralMessageAll") };
                    institutionsDisplay.Insert(0, all);
                } else if (institutions.Count() == 0)
                {
                    var all = new LookupView<Institution> { Id = "0", Name = "-- Sin unidades centinela --" };
                    institutionsDisplay.Insert(0, all);
                }
                return Json(institutionsDisplay, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var user = UserManager.FindById(User.Identity.GetUserId());
                //var UsrCtry = user.Institution.CountryID;

                //CountryID = CountryID != null ? CountryID : UsrCtry;

                var institutions =
                 (
                  from institution in db.Institutions as IQueryable<Institution>
                  where institution.CountryID == CountryID && institution is Hospital
                  select new
                  {
                      Id = institution.ID,
                      Name = institution.Name,
                      InstitutionType = institution is Lab ? InstitutionType.Lab : InstitutionType.Hospital
                      //InstitutionType = InstitutionType.Hospital
                  }).ToArray();

                var institutionsDisplay = institutions.Select(i => new LookupView<Institution>()
                {
                    Id = i.Id.ToString(),
                    Name = i.Name
                }).ToList();

                if (institutions.Count() > 1)
                {
                    var all = new LookupView<Institution> { Id = "0", Name = getMsg("msgGeneralMessageAll") };
                    institutionsDisplay.Insert(0, all);
                }
                else if (institutions.Count() == 0)
                {
                    var all = new LookupView<Institution> { Id = "0", Name = "-- Sin unidades centinela --" };
                    institutionsDisplay.Insert(0, all);
                }

                return Json(institutionsDisplay, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAreas(int CountryID)
        {
            var areas =
                     (
                      from area in db.Areas as IQueryable<Area>
                      where area.CountryID == CountryID
                      orderby area.Name
                      select new
                      {
                          Id = area.ID,
                          Name = area.Name
                      }).ToArray();

            return Json(areas, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetConfAlertCaseDefinition(int CountryID)
        {
            var ageAlertCaseDefinition =
                     (
                      from ConfAlertCaseDefinition in db.ConfAlertCaseDefinition as IQueryable<ConfAlertCaseDefinition>
                      where ConfAlertCaseDefinition.id_country == CountryID
                      orderby ConfAlertCaseDefinition.month_begin
                      select new
                      {
                          month_begin = ConfAlertCaseDefinition.month_begin,
                          month = ConfAlertCaseDefinition.month_end
                      }).ToArray();

            return Json(ageAlertCaseDefinition, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStates(int AreaID)
        {
            var states =
                   (
                    from state in db.States as IQueryable<State>
                    where state.AreaID == AreaID
                    select new
                    {
                        Id = state.ID,
                        Name = state.Name
                    })
                    .OrderBy(c => c.Name)
                    .ToArray();

            return Json(states, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetParishPostOffice(int AreaID)
        {
            var ParisPostOffice =
                   (
                    from PPOff in db.CatParishPostOfficeJM as IQueryable<CatParishPostOfficeJM>
                    where PPOff.AreaID == AreaID
                    select new
                    {
                        Id = PPOff.ID,
                        Name = PPOff.PostOffice_PostalAgency + " - " + PPOff.orig_country
                    })
                    .OrderBy(c => c.Name)
                    .ToArray();

            return Json(ParisPostOffice, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocals(int StateID)
        {
            var locals =
                (
                 from local in db.Locals as IQueryable<Local>
                 where local.StateID == StateID
                 select new
                 {
                     Id = local.ID,
                     Name = local.Code
                 }).ToArray();

            return Json(locals, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNeighborhoods(int StateID)
        {
            var neighborhoods =
                (
                 from neighborhood in db.Neighborhoods as IQueryable<Neighborhood>
                 where neighborhood.StateID == StateID
                 select new
                 {
                     Id = neighborhood.ID,
                     Name = neighborhood.Name
                 })
                 .OrderBy (d=> d.Name)
                 .ToArray();

            return Json(neighborhoods, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHamlets(int NeighborhoodID)                  //#### CAFQ: 181018
        {
            var hamlets =
                (
                 from hamlet in db.Hamlets as IQueryable<Hamlet>
                 where hamlet.NeighborhoodID == NeighborhoodID
                 select new
                 {
                     Id = hamlet.ID,
                     Name = hamlet.Name
                 })
                 .OrderBy(d => d.Name)
                 .ToArray();

            return Json(hamlets, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColonies(int HamletID)                       //#### CAFQ: 181018
        {
            var colonies =
                   (
                    from colony in db.Colonies as IQueryable<Colony>
                    where colony.HamletID == HamletID
                    select new
                    {
                        Id = colony.ID,
                        Name = colony.Name
                    })
                    .OrderBy(c => c.Name)
                    .ToArray();

            return Json(colonies, JsonRequestBehavior.AllowGet);
        }

        // GET: FluCases
        public ActionResult GetFluCases(string sidx, string sord, int page, int rows,
                                        int institutionId,
                                        int? areaId,
                                        int? regionId,
                                        string ScaseNo,
                                        string ShospReg,
                                        string Sstatus,
                                        string SName,
                                        string SLastName,
                                        int? SRecordId,
                                        string SNotiDateS,
                                        string SNotiDateE,
                                        bool SPend,
                                        int ITy
                                        )
        {
            //IEnumerable<FluCase> flucases;
            IQueryable<FluCase> flucases = null;
            var InstitutionDB = institutionId;
            var user = UserManager.FindById(User.Identity.GetUserId());
            var UsrCtry = user.Institution.CountryID;
            var RecordId = SRecordId;
            var UsrAccessLevel = user.Institution.AccessLevel;
            var UsrRegion = user.Institution.cod_region_institucional;
            var UsrArea = user.Institution.AreaID;
            var UsrInstitution = user.Institution.ID;
            string language = user.Institution.Country.Language.ToString();     //#### CAFQ: 180604 - Jamaica Universal

            var Access_Lever_IT = false;

            if (InstitutionDB > 0)
                Access_Lever_IT = db.Institutions.Where(f => f.ID == InstitutionDB).FirstOrDefault().AccessLevel == AccessLevel.Service ? true : false;

            if ((InstitutionDB == 0 && user.Institution is Hospital) || (InstitutionDB > 0 && Access_Lever_IT && user.Institution is Hospital))
            {
                if (InstitutionDB == 0)
                    InstitutionDB = Convert.ToInt32(user.InstitutionID);

                var institution_type = db.Institutions.Where(f => f.ID == InstitutionDB).ToList();

                if (institution_type.Any())
                    if (institution_type[0].Father_ID.ToString() != "" && institution_type[0].Father_ID.ToString() != "0")
                        Int32.TryParse(institution_type[0].Father_ID.ToString(), out InstitutionDB);

                if (institution_type[0] is Hospital && institution_type[0].AccessLevel != AccessLevel.Service)
                {
                    var institutions_service = db.Institutions.OfType<Hospital>()
                                      .Where(i => i.Father_ID == InstitutionDB || i.ID == InstitutionDB).Select(t => t.ID).ToList();
                    if (institutions_service.Any())
                    {
                        flucases = db.FluCases.Where(y => institutions_service.Contains(y.HospitalID));
                    }
                }
            } else if (InstitutionDB == 0 ) {
                flucases = db.FluCases.Where(f => f.CountryID == UsrCtry);
            } else {
                flucases = db.FluCases.Where(f => f.HospitalID == InstitutionDB);
            }

            if (user.Institution.CountryID == 11)               //#### CAFQ: 181008
            {
                var regions = db.Regions.Where(c => c.CountryID == user.Institution.CountryID && c.tipo_region == 1).OrderBy(i => i.Name);

                var regionsDisplay = regions.Select(i => new LookupView<Paho.Models.Region>()
                {
                    Id = i.orig_country.ToString(),
                    //Id = i.ID.ToString(),
                    Name = i.Name
                }).ToList();

                if (regionsDisplay.Count() > 1)
                {
                    var all = new LookupView<Paho.Models.Region> { Id = "0", Name = getMsg("msgSelectLabel") };
                    regionsDisplay.Insert(0, all);
                }
            }
            else if ((regionId > 0 || UsrAccessLevel == AccessLevel.Regional) && institutionId == 0)
            {
                var RegionId_ = (UsrAccessLevel == AccessLevel.Regional) ? UsrRegion : regionId;
                var Regions = db.Institutions.Where(l => l.cod_region_institucional == RegionId_);
                var list_regions = Regions.Select(i => i.ID).ToList();

                if (list_regions.Any())
                {
                    flucases = flucases.Where(k => list_regions.Contains(k.HospitalID));
                }
            }

            if ((areaId > 0 || UsrAccessLevel == AccessLevel.Area) && institutionId == 0)
            {
                var AreaId_ = (UsrAccessLevel == AccessLevel.Area) ? UsrArea : areaId;
                var Areas = db.Institutions.Where(l => l.AreaID == AreaId_);
                var list_areas = Areas.Select(i => i.ID).ToList();

                if (list_areas.Any())
                {
                    flucases = flucases.Where(k => list_areas.Contains(k.HospitalID));
                } else
                {
                    flucases = flucases.Where(k => k.Hospital.AreaID == AreaId_);
                }

            }

            var pageIndex = Convert.ToInt32(page) - 1;
            var pageSize = rows;

            if (ITy == 2)
            {
                var labs = db.Institutions.Where(f => f.ID == institutionId);
                var list_labs = labs.ToList();
                var institutionsConfiguration = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>()
                                        .Where(i => i.InstitutionToID == user.Institution.ID);

                if (InstitutionDB == 0 && institutionsConfiguration.Any())
                {
                    var Listlabs = institutionsConfiguration.Select(i => i.InstitutionParentID).ToList();
                    flucases = flucases.Where(f => Listlabs.Contains(f.HospitalID));
                }

                if (!institutionsConfiguration.Any())
                {
                    flucases = flucases.Where(f => f.IsSample == true);
                    //InstitutionDB = labs;

                    if (list_labs.Count() > 0) {
                        InstitutionDB = int.Parse(list_labs[0].Father_ID.ToString());
                        flucases = flucases.Where(f => f.HospitalID == InstitutionDB);
                    }
                }

                if (SPend == true)
                {
                    flucases = flucases.Where(f => f.CaseStatus == null);
                    if (institutionsConfiguration.Any())
                    {

                        if (institutionsConfiguration.Where(c => c.InstitutionToID == user.InstitutionID && c.InstitutionFrom.NPHL == true).Any())
                        {
                            flucases = flucases.Where(d => d.IsSample == true && ((d.SampleDate != null && d.NPHL_Processed != false) || (d.SampleDate2 != null && d.NPHL_Processed_2 != false)));
                        }

                        if (user.Institution.CountryID == 15)
                        {
                            flucases = flucases.Where(h => (h.IsSample == true && (h.Processed != false || h.Processed_National != false)) && (((h.flow == (institutionsConfiguration.Where(i => i.InstitutionParentID == h.HospitalID && i.InstitutionToID == user.Institution.ID).Select(j => j.Priority).ToList().FirstOrDefault() - 1)) && (h.statement == 2 || h.statement == null)) || ((h.flow == (institutionsConfiguration.Where(i => i.InstitutionParentID == h.HospitalID && i.InstitutionToID == user.Institution.ID).Select(j => j.Priority).ToList().FirstOrDefault())) && (h.statement == 1 || h.statement == null))));
                        }
                        else
                        {
                            flucases = flucases.Where(h => (h.IsSample == true && h.Processed != false) && (((h.flow == (institutionsConfiguration.Where(i => i.InstitutionParentID == h.HospitalID && i.InstitutionToID == user.Institution.ID).Select(j => j.Priority).ToList().FirstOrDefault() - 1)) && (h.statement == 2 || h.statement == null)) || ((h.flow == (institutionsConfiguration.Where(i => i.InstitutionParentID == h.HospitalID && i.InstitutionToID == user.Institution.ID).Select(j => j.Priority).ToList().FirstOrDefault())) && (h.statement == 1 || h.statement == null))));
                        }


                        //Es la escepción únicamente para AZP  -- tengo que mejorar el query para hacerlo automatico
                        if (user.Institution.CountryID != 25)
                            flucases = flucases.Where(i => i.CaseLabTests.Where(x => (x.inst_conf_end_flow_by_virus == 0 || x.inst_conf_end_flow_by_virus == null)  || x.statement_test == 1).Any() || i.CaseLabTests.Count == 0);

                    }
                    else
                    {
                        flucases = flucases.Where(f => f.IsSample == true && (f.Processed != false || f.NPHL_Processed != false) && f.FinalResult == null);
                        flucases = flucases.Where(h => h.flow == 0);
                    }

                }


            }

            if (RecordId > 0)
            {
                flucases = flucases.Where(t => t.ID == RecordId);
            }

            if (SPend == true && ITy != 2)
            {
                flucases = flucases.Where(f => (f.Surv== 1 && (f.Destin == "" || f.Destin == null)) || (f.CaseStatus != 3 && f.CaseStatus != 2 )  );
            }

            if (SNotiDateS != "" && SNotiDateS != "undefined")
            {
                DateTime NotiDateS;

                NotiDateS = DateTime.ParseExact(SNotiDateS, "d", CultureInfo.CreateSpecificCulture("es-GT"));
                flucases = flucases.Where(i => i.RegDate >= NotiDateS);
            }

            if (Sstatus != "" && Sstatus != "undefined")
            {
                flucases = flucases.Where(f => f.Destin == Sstatus);
            }


            if (ShospReg != "" && ShospReg != "undefined") {
                flucases = flucases.Where(g => g.NationalId == ShospReg);
            }

            if (SName != "" && SName != "undefined")
            {
                flucases = flucases.Where(z => string.Concat(z.FName1.Trim().ToUpper(), " ", z.FName2.Trim().ToUpper()).Contains(SName.Trim().ToUpper()));
            }

            if (SLastName != "" && SLastName != "undefined")
            {
                flucases = flucases.Where(z => string.Concat(z.LName1.Trim().ToUpper(), " ", z.LName2.Trim().ToUpper()).Contains(SLastName.Trim().ToUpper()));

            }

            if (ScaseNo != "")
            {
                flucases = flucases.Where(h => h.NoExpediente == ScaseNo.ToUpper());
            }

            flucases = flucases.OrderByDescending(s => s.HospitalDate);

            var totalRecords = flucases.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            flucases = flucases.Skip(pageIndex * pageSize).Take(1500).Take(pageSize);

            var jsondata = new List<Object>();

            string commentsHtml = " <img src='/Content/themes/base/images/Comentario2.png' alt='" + getMsg("msgFlucasesMessageReadytoClose") + "'/>";
            string readyCloseHtml = "<img src='/Content/themes/base/images/ReadyClose.png' alt='" + getMsg("msgFlucasesMessageReadytoClose") + "'/>";
            string MissingDischargeHtml = " <img src='/Content/themes/base/images/MissingDischarge.png' alt='" + getMsg("msgFlucasesMessageReadytoClose") + "'/>";
            string openHtml = "<img src='/Content/themes/base/images/open.png' alt='" + getMsg("msgFlucasesMessageNoStatus") + "'/>" + getMsg("msgFlucasesMessageNoStatus");

            var  Arrayrows = (from flucase in flucases
                                 select new
                                 {
                                     surv_ID = flucase.Surv,
                                     surv_IDInusual = flucase.SurvInusual,    //#### CAFQ: 180604 - Jamaica Universal
                                     ready_close = ( // revisar si ya existe un resultado con el flujo y existe un laboratorio con el campo conclusion que ya ingreso los resultados
                                                    (flucase.flow == db.InstitutionsConfiguration.Where(i => i.InstitutionParentID == flucase.HospitalID && i.Conclusion == true).OrderBy(x => x.Priority).FirstOrDefault().Priority && flucase.statement == 2) 
                                                     // si la muestra no fue tomada
                                                    || (flucase.IsSample == false)
                                                    // Para honduras si el laboratorio regional no proceso la muestra
                                                    || ((user.Institution.CountryID == 15) ? (flucase.Processed_National == false && flucase.Processed == false) : false)
                                                    || ((user.Institution.CountryID == 17) ? (flucase.NPHL_Processed == false) : false)
                                                    || ((user.Institution.CountryID != 15) && (user.Institution.CountryID != 17) && flucase.Processed == false)
                                                    // Para Jamaica si el NPHL no proceso la muestra
                                                    //|| ((user.Institution.CountryID == 17 && user.Institution.CountryID != 15) ? (flucase.NPHL_Processed == false) : (flucase.Processed == false))
                                                    //|| (flucase.Processed == false)
                                                    // La segunda y tercera muestra tampoco fueron tomadas
                                                    || (flucase.Processed2 == false) 
                                                    || (flucase.Processed3 == false)) ? 1 : 0,
                                     ready_close2 = ( // revisar si ya existe un resultado con el flujo
                                                    (flucase.flow == db.InstitutionsConfiguration.Where(i => i.InstitutionParentID == flucase.HospitalID).OrderByDescending(x => x.Priority).FirstOrDefault().Priority && flucase.statement == 2)
                                                    // si la muestra no fue tomada
                                                    || (flucase.IsSample == false)
                                                    // Para honduras si el laboratorio regional no proceso la muestra y Para Jamaica si el NPHL no proceso la muestra
                                                    || ((user.Institution.CountryID == 15) ? ( flucase.Processed_National == false && flucase.Processed == false) : (user.Institution.CountryID == 17 && user.Institution.CountryID != 15) ? (flucase.NPHL_Processed == false) : (flucase.Processed == false))
                                                    // 
                                                    || (flucase.Processed2 == false)
                                                    // La segunda y tercera muestra tampoco fueron tomadas
                                                    || (flucase.Processed2 == false) || (flucase.Processed3 == false)) ? 1 : 0,
                                     ready_close_missing_Discharge = ((flucase.Destin == null || flucase.Destin == "" ) ? 1 : 0),
                                     ready_close_missing_DateEx = ((flucase.HospExDate == null) ? 1 : 0),
                                     id_D = flucase.ID,
                                     H_D = flucase.HospitalDate,
                                     LN_D = flucase.LName1 + " " + flucase.LName2 ?? "",
                                     FN_D = flucase.FName1 + " " + flucase.FName2 ?? "",
                                     NE_D = flucase.NoExpediente ?? "",
                                     IS_D = flucase.IsSample,
                                     FR_ID = flucase.FinalResult,
                                     FR_D_C = flucase.CatVirusType_FR1,
                                     D_D = flucase.Destin,
                                     FRVT_ID = flucase.FinalResultVirusTypeID,
                                     P_D = flucase.Processed,
                                     P_D_N = flucase.Processed_National,
                                     P_D_NPHL = flucase.NPHL_Processed,
                                     CS_D = flucase.CaseStatus,
                                     CS_D_Cat = flucase.CatStatusCase,
                                     VR_IF_D = flucase.CaseLabTests.Where(e => e.TestType == 1 && e.Processed != null).OrderBy(y => y.CatVirusType.orden).ThenBy(d => d.SampleNumber).ThenBy(u => u.TestDate).FirstOrDefault(),
                                     VR_PCR_D = flucase.CaseLabTests.Where(e => e.TestType == 2 && e.Processed != null)
                                                .OrderBy(y => y.CatVirusType.orden)
                                                .ThenBy(d => d.SampleNumber)
                                                .ThenBy(u => u.TestDate).FirstOrDefault(),
                                     HEALTH_INST = flucase.Hospital.Name ?? "",
                                     FLOW_FLUCASE = flucase.flow,
                                     TEST_LAST = flucase.CaseLabTests.Where(e => e.Processed != null).OrderByDescending(d => d.flow_test).FirstOrDefault(),
                                     FLOW_VIRUS = db.InstitutionConfEndFlowByVirus.Where(i => i.ID == flucase.CaseLabTests.Where(e => e.Processed != null).OrderByDescending(d => d.flow_test).ThenByDescending(c => c.inst_conf_end_flow_by_virus).FirstOrDefault().inst_conf_end_flow_by_virus).FirstOrDefault(),
                                     //FLOW_VIRUS = db.InstitutionConfEndFlowByVirus.Where(i => i.ID == flucase.CaseLabTests.Where(e => e.inst_conf_end_flow_by_virus != null).OrderByDescending(d => d.flow_test).FirstOrDefault().inst_conf_end_flow_by_virus).FirstOrDefault(),
                                     ICON_COMMEN = flucase.Comments == "" || flucase.Comments == null ? "" : commentsHtml,
                                     ICON_COMMEN_CLOSE = flucase.ObservationCase == "" || flucase.ObservationCase == null ? "" : commentsHtml,
                                     VI_OK = (from a in db.InstitutionConfEndFlowByVirus
                                              join p in db.InstitutionsConfiguration on a.id_InstCnf equals p.ID
                                              //join dt in db.Institutions on p.InstitutionFromID equals dt.ID
                                              where  p.InstitutionParentID == flucase.HospitalID
                                              //dt.CountryID == flucase.CountryID &&  // AM desactivar este parametro del país porque no pertenece al parametro del país de la institución, sino a la residencia del paciente.
                                              select new
                                              {
                                                  ID = a.ID
                                              }).Any()
                                 }).AsEnumerable()
                                   .Select(x => new
                                   {
                                       id = x.id_D.ToString(),
                                       cell = new string[]
                                     {
                                         //#### CAFQ: 180604 - Jamaica Universal
                                         "<img src='/Content/themes/base/images/" + ((UsrCtry==17 && (bool)x.surv_IDInusual==true) ? Convert.ToInt32((bool)x.surv_IDInusual).ToString() + "_" + "UNI" + ".png' alt='" + "UNIVERSAL" : x.surv_ID.ToString() + "_" + language + ".png' alt='" + (x.surv_ID == 1 ? "SARI":"ILI")) + "'/>",
                                         //x.id_D.ToString(),
                                         x.id_D.ToString() + ((user.Institution.CountryID==15) ? x.ICON_COMMEN : x.ICON_COMMEN_CLOSE),
                                         x.H_D.ToString((user.Institution.CountryID==17) ? "yyyy/MM/dd": "dd/MM/yyyy" ),
                                         x.LN_D,
                                         x.FN_D,
                                         x.NE_D ?? "",
                                         "<img src='/Content/themes/base/images/PDF.png' alt='print'/>",
                                         x.VR_IF_D == null ? "" :  x.VR_IF_D.TestResultID == null ? "": x.VR_IF_D.TestResultID.ToString() == "P" ? x.VR_IF_D.CatVirusType == null ? "" : (user.Institution.Country.Language == "SPA" ? x.VR_IF_D.CatVirusType.SPA : x.VR_IF_D.CatVirusType.ENG) :  x.VR_IF_D.TestResultID == null  ? ""  : user.Institution.Country.Language == "SPA" ? db.CatTestResult.Where(j=> j.value == x.VR_IF_D.TestResultID.ToString()).FirstOrDefault().description : db.CatTestResult.Where(j=> j.value == x.VR_IF_D.TestResultID.ToString()).FirstOrDefault().ENG ,
                                         x.VR_PCR_D == null ? "" : x.VR_PCR_D.TestResultID == null ? 
                                                        "": x.VR_PCR_D.TestResultID.ToString() == "P" ?  
                                                                x.VR_PCR_D.CatVirusType == null ? 
                                                                    "" : 
                                                                    x.VR_PCR_D.CatVirusType.SPA.Contains("Influenza A") == true ? 
                                                                        x.VR_PCR_D.CatVirusSubType == null ? 
                                                                            "" : 
                                                                            x.VR_PCR_D.TestResultID_VirusSubType_2 == "P" ? 
                                                                                (user.Institution.Country.Language == "SPA" ?  
                                                                                    x.VR_PCR_D.CatVirusSubType_2.SPA : 
                                                                                    x.VR_PCR_D.CatVirusSubType_2.ENG ) :
                                                                                    (user.Institution.Country.Language == "SPA" ?
                                                                                    x.VR_PCR_D.CatVirusSubType.SPA :
                                                                                    x.VR_PCR_D.CatVirusSubType.ENG ) : 
                                                                        (user.Institution.Country.Language == "SPA" ? x.VR_PCR_D.CatVirusType.SPA : x.VR_PCR_D.CatVirusType.ENG) :
                                                                     x.VR_PCR_D.TestResultID == null ? "" : 
                                                                  user.Institution.Country.Language == "SPA" ? 
                                                                  db.CatTestResult.Where(j=> j.value == x.VR_PCR_D.TestResultID.ToString()).FirstOrDefault().description : 
                                                                  db.CatTestResult.Where(j=> j.value == x.VR_PCR_D.TestResultID.ToString()).FirstOrDefault().ENG,
                                         x.IS_D == false ? getMsg("msgFlucasesMessageNoSample") : x.FR_ID == "P" ? x.FR_D_C == null ? "" : (user.Institution.Country.Language == "SPA" ? x.FR_D_C.SPA : x.FR_D_C.ENG) : (x.P_D == false) ? getMsg("msgFlucasesMessageNotProcessed") : x.FR_ID == "N" ? getMsg("msgFlucasesMessageNegative") : x.FR_ID == "I" ? getMsg("msgFlucasesMessageIndeterminated") : (x.P_D_N == false) ? getMsg("msgFlucasesMessageNotProcessed") : (x.P_D_NPHL == false) ? getMsg("msgFlucasesMessageNotProcessed") : ""  ,
                                         x.HEALTH_INST ?? "",
                                         //x.FLOW_VIRUS != null ? x.FLOW_VIRUS.value_Cat_TestResult : "nulo"  ,
                                         /*
                                         x.ready_close == 1 && x.FLOW_FLUCASE != 99 ?   "<img src='/Content/themes/base/images/ReadyClose.png' alt='"+getMsg("msgFlucasesMessageReadytoClose")+"'/> "  : "" + (x.CS_D_Cat == null ? "<img src='/Content/themes/base/images/open.png' alt='"+getMsg("msgFlucasesMessageNoStatus")+"'/>" +  getMsg("msgFlucasesMessageNoStatus") :
                                                                (user.Institution.Country.Language == "SPA" ? "<img src='/Content/themes/base/images/"+(x.CS_D == 3 || x.CS_D == 2 ? "close":"open" )+".png' alt='"+x.CS_D_Cat.SPA+"'/> " + x.CS_D_Cat.SPA : "<img src='/Content/themes/base/images/"+(x.CS_D == 3 || x.CS_D == 2 ? "close":"open" )+".png' alt='"+x.CS_D_Cat.ENG+"'/> " + x.CS_D_Cat.ENG ))
                                        */
                                        x.VI_OK == true ?
                                           // Los que tienen configuración de Virus para el cierre de caso
                                            (x.FLOW_VIRUS != null ?
                                               // si es negativo
                                               ((x.FR_ID == "N") ? 
                                                    (x.FLOW_VIRUS.value_Cat_TestResult==x.FR_ID && x.FLOW_FLUCASE != 99 ?
                                                        readyCloseHtml + ( ((x.ready_close_missing_Discharge == 1 || x.ready_close_missing_DateEx == 1 ) && user.Institution.CountryID == 17) ? MissingDischargeHtml : "")
                                                        : (x.ready_close2 == 1 && x.FLOW_FLUCASE != 99 ?
                                                                readyCloseHtml  + ( ((x.ready_close_missing_Discharge == 1 || x.ready_close_missing_DateEx == 1 ) && user.Institution.CountryID == 17) ? MissingDischargeHtml : "") 
                                                                : (x.CS_D_Cat == null ?
                                                                            openHtml 
                                                                            : (user.Institution.Country.Language == "SPA" ? "<img src='/Content/themes/base/images/"+(x.CS_D == 3 || x.CS_D == 2 ? "close":"open" )+".png' alt='"+x.CS_D_Cat.SPA+"'/> " + x.CS_D_Cat.SPA : "<img src='/Content/themes/base/images/"+(x.CS_D == 3 || x.CS_D == 2 ? "close":"open" )+".png' alt='"+x.CS_D_Cat.ENG+"'/> " + x.CS_D_Cat.ENG )
                                                                    )
                                                            )
                                                    ) // Cuando el resultado final
                                                :
                                                (x.FLOW_VIRUS.value_Cat_TestResult==x.FR_ID && x.FLOW_VIRUS.id_Cat_VirusType==x.FRVT_ID && x.FLOW_FLUCASE != 99 ?
                                                     readyCloseHtml + ( ((x.ready_close_missing_Discharge == 1 || x.ready_close_missing_DateEx == 1 ) && user.Institution.CountryID == 17) ? MissingDischargeHtml : "")  :
                                                    (x.ready_close2 == 1 && x.FLOW_FLUCASE != 99 ?
                                                       readyCloseHtml + ( ((x.ready_close_missing_Discharge == 1 || x.ready_close_missing_DateEx == 1 ) && user.Institution.CountryID == 17) ? MissingDischargeHtml : "") :
                                                        (x.CS_D_Cat == null ?
                                                          openHtml :
                                                            (user.Institution.Country.Language == "SPA" ? "<img src='/Content/themes/base/images/"+(x.CS_D == 3 || x.CS_D == 2 ? "close":"open" )+".png' alt='"+x.CS_D_Cat.SPA+"'/> " + x.CS_D_Cat.SPA : "<img src='/Content/themes/base/images/"+(x.CS_D == 3 || x.CS_D == 2 ? "close":"open" )+".png' alt='"+x.CS_D_Cat.ENG+"'/> " + x.CS_D_Cat.ENG )
                                                        )
                                                    )
                                                ))
                                                :
                                                // Aquí entra cuando no tiene flujo por tipo de virus y ningún laboratorio del flujo tiene marcada la opción de conclusión, únicamente por laboratorios
                                                (x.ready_close2 == 1 && x.FLOW_FLUCASE != 99 ?
                                                    readyCloseHtml + ( ((x.ready_close_missing_Discharge == 1 || x.ready_close_missing_DateEx == 1 ) && user.Institution.CountryID == 17) ? MissingDischargeHtml : "")  :
                                                    (x.CS_D_Cat == null ?
                                                       openHtml :
                                                        (user.Institution.Country.Language == "SPA" ? "<img src='/Content/themes/base/images/"+(x.CS_D == 3 || x.CS_D == 2 ? "close":"open" )+".png' alt='"+x.CS_D_Cat.SPA+"'/> " + x.CS_D_Cat.SPA : "<img src='/Content/themes/base/images/"+(x.CS_D == 3 || x.CS_D == 2 ? "close":"open" )+".png' alt='"+x.CS_D_Cat.ENG+"'/> " + x.CS_D_Cat.ENG ) 
                                                    )
                                                )
                                            )
                                            :
                                            
                                            (x.ready_close == 1 && x.FLOW_FLUCASE != 99 ?
                                                  readyCloseHtml +  ( ((x.ready_close_missing_Discharge == 1 || x.ready_close_missing_DateEx == 1 ) && user.Institution.CountryID == 17) ? MissingDischargeHtml : "")  :
                                                    (x.CS_D_Cat == null ?
                                                       openHtml  :
                                                        (user.Institution.Country.Language == "SPA" ? "<img src='/Content/themes/base/images/"+(x.CS_D == 3 || x.CS_D == 2 ? "close":"open" )+".png' alt='"+x.CS_D_Cat.SPA+"'/> " + x.CS_D_Cat.SPA : "<img src='/Content/themes/base/images/"+(x.CS_D == 3 || x.CS_D == 2 ? "close":"open" )+".png' alt='"+x.CS_D_Cat.ENG+"'/> " + x.CS_D_Cat.ENG )
                                                    )
                                            )
                                     }
                                   }).ToArray();

                jsondata.Add(new
                {
                    total = totalPages,
                    page = page,
                    records = totalRecords,
                    rows = Arrayrows
                });

            return Json(jsondata[0], JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCIE10( string term, int max, string code)
        {
            var search = term;

            if (search == "" || search == null) search = "-A";

            IQueryable<CatDiag> diags = null;
            var user = UserManager.FindById(User.Identity.GetUserId());

            if (user.Institution.Country.Language == "SPA")
            {
                diags = (db.CIE10.Where(f => f.SPA.Contains(search)) as IQueryable<CatDiag>);
            }
            else if (user.Institution.Country.Language == "ENG") {
                diags = (db.CIE10.Where(f => f.ENG.Contains(search)) as IQueryable<CatDiag>);
            }
            

            if (code != "" && code!= null)
            {
                //diags = diags.Where(h => h.Diag.Contains(code));
                    //diags = (db.CIE10.Where(f => f.code.Contains(search)) as IQueryable<CatDiag>);

                if (user.Institution.Country.Language == "SPA")
                {
                    diags = (db.CIE10.Where(f => f.SPA.Contains(search)) as IQueryable<CatDiag>);
                }
                else if (user.Institution.Country.Language == "ENG")
                {
                    diags = (db.CIE10.Where(f => f.ENG.Contains(search)) as IQueryable<CatDiag>);
                }
            }

            diags = diags.Take(10);

            var jsondata = (
                    from diags_list in diags
                    select new
                    {
                        value = diags_list.ID,
                        label = (user.Institution.Country.Language == "SPA") ? diags_list.SPA : diags_list.ENG
                    }).ToArray();


            return Json(jsondata, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetSalon(string term, int max)
        {
            var search = term;

            if (search == "" || search == null) search = "A";

            var salones = (db.Salones.Where(f => f.description_service.Contains(search)) as IQueryable<CatServicios>);

            salones = salones.Take(10);

            var jsondata = (
                    from salons_list in salones
                    select new
                    {
                        value = salons_list.ID,
                        label = salons_list.description_service
                    }).ToArray();

            return Json(jsondata, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetSalonID(int ID)
        {
            CatServicios salones = db.Salones.Find(ID);
            if (salones == null)
            {
                return HttpNotFound();
            }

            var result =
                 new
                 {
                     value = salones.ID.ToString(),
                     label = salones.description_service
                 };

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetCIE10ID( int ID)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            CatDiag diag = db.CIE10.Find(ID);
            if (diag == null)
            {
                return HttpNotFound();
            }

            var result =
                 new
                  {
                      value = diag.ID.ToString(),
                      label = (user.Institution.Country.Language == "SPA") ? diag.SPA : diag.ENG
                 };

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        // GET: Cases/Delete/5
        public ActionResult GetContact(int Id)
        {
            FluCase flucase = db.FluCases.Find(Id);
            if (flucase == null)
            {
                return HttpNotFound();
            }

            IQueryable<Institution> institutions = null;
            IQueryable<Institution> CLOrd = null;
            var user = UserManager.FindById(User.Identity.GetUserId());

            institutions = db.Institutions.OfType<Hospital>().Where(i => i.ID == flucase.HospitalID);
            var institutionsName = institutions.Select(x => (string)x.FullName).ToList();
            
            var region_institutional = db.Regions.Where(d => d.CountryID == institutions.ToList().FirstOrDefault().CountryID  && d.orig_country == institutions.ToList().FirstOrDefault().cod_region_institucional && d.tipo_region == 1).Select(j => (string)j.Name).ToList(); //.Select(x => (string)x.Name)
            var region_salud = db.Regions.Where(d => d.CountryID == institutions.ToList().FirstOrDefault().CountryID && d.orig_country == institutions.ToList().FirstOrDefault().cod_region_salud && d.tipo_region == 2).Select(j => (string)j.Name).ToList();
            var region_pais = db.Regions.Where(d => d.CountryID == institutions.ToList().FirstOrDefault().CountryID && d.orig_country == institutions.ToList().FirstOrDefault().cod_region_pais && d.tipo_region == 3).Select(j => (string)j.Name).ToList();
            var institutionsConfiguration = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(i => i.InstitutionToID == user.Institution.ID && i.InstitutionParentID == flucase.HospitalID);
            var institutionsConfigurationMax = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(i => i.InstitutionParentID == flucase.HospitalID).OrderByDescending(j => j.Priority);
            // Catalogo de las instituciones tipo laboratorio del flujo
            var InstConfFlowLab = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>()
                    .Where(i => i.InstitutionParentID == flucase.HospitalID).Select(j => j.InstitutionToID);
            CLOrd = db.Institutions.OrderBy(i => i.OrdenPrioritybyLab).Where( z => InstConfFlowLab.Contains(z.ID));
            var CLOrdDisplay =  CLOrd.Select(i => new LookupView<Institution>()
            {
                Id = i.ID.ToString(),
                Name = i.Name,
                orden = i.OrdenPrioritybyLab.ToString()
            }).ToList();

            var flow_local_lab = 0;
            var flow_max = 1;
            var flow_open_always = false;
            if (institutionsConfiguration.Any())
            {
                flow_local_lab = institutionsConfiguration.First().Priority;
                flow_open_always = institutionsConfiguration.First().OpenAlways;
            }

            if (institutionsConfigurationMax.Any())
            {
                flow_max = institutionsConfigurationMax.First().Priority;
            }

            var result =
                 new
                 {
                     Id = flucase.ID.ToString(),
                     Surv = flucase.Surv.ToString(),
                     SurvInusual = flucase.SurvInusual,
                     Brote = flucase.Brote,
                     LName1 = flucase.LName1,
                     LName2 = flucase.LName2,
                     FName1 = flucase.FName1,
                     FName2 = flucase.FName2,
                     DocumentType = flucase.DocumentType,
                     NoExpediente = flucase.NoExpediente,
                     NationalId = flucase.NationalId,
                     DOB = flucase.DOB,
                     Age = flucase.Age,
                     AMeasure = flucase.AMeasure.ToString(),
                     Gender = flucase.Gender.ToString(),
                     ResponsibleMinor = flucase.ResponsibleMinor,        //####CAFQ
                     HospitalDate = flucase.HospitalDate.ToUniversalTime(),
                     RegDate = flucase.RegDate,
                     nationality = flucase.nationality,
                     nativepeople = flucase.nativepeople,
                     hospitalIDRecord = flucase.HospitalID,
                     hospitalIDCaseGenerating = flucase.HospitalID_CaseGenerating,
                     hospitalName = institutionsName[0],
                     DataStatement = flucase.statement,
                     flow_record = flucase.flow,
                     flow_institution = flow_local_lab,
                     flow_max = flow_max,
                     flow_open_always = flow_open_always,
                     region_institucional = region_institutional.FirstOrDefault(),
                     region_salud = region_salud.FirstOrDefault(),
                     region_pais = region_pais.FirstOrDefault(),
                     selectedServiceId = (db.Institutions.Where(j => j.ID == flucase.HospitalID).FirstOrDefault().AccessLevel == (AccessLevel)6) ? flucase.HospitalID : 0,
                     IntsFlow = CLOrdDisplay,
                 };
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public JsonResult DeleteCase(int id)
        {
            try {
                var flucase = db.FluCases.Find(id);
                var flow = flucase.flow;
                var statement = flucase.statement;
                db.Entry(flucase).State = EntityState.Deleted;
                db.FluCases.Remove(flucase);
                db.SaveChanges();
                HistoryRecord(id, 3, flow, statement);
                return Json("Registo borrado!", JsonRequestBehavior.AllowGet);
            } catch (Exception) {
                return Json("Fallo borrar el registro !", JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Cases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult SaveContact(
            int? id,
            int? Surv,
            bool? SInusual,
            bool? Brote,
            string LName1,
            string LName2,
            string FName1,
            string FName2,
            int? DocumentType,
            string NoExpediente,
            string NationalId,
            DateTime? DOB,
            int? Age,
            AMeasure? AMeasure,
            Gender Gender,
            string ResponsibleMinor,        //####CAFQ 
            DateTime HospitalDate,
            DateTime RegDate,
            int HospitalId,
            int? HospitalID_CaseGenerating,
            int? nativepeople,
            int? nationality,
            DateTime DateFeverDummy,
            bool? IsSample // Esto se agrego únicamente para la bitacora
            )
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var UsrCtry = user.Institution.CountryID;
            FluCase flucase;
            /*var DateDummyRange1 = DateFeverDummy.Date.AddDays(1);         //#### CAFQ: 180604 - Jamaica Universal
            var DateDummyRange2 = DateFeverDummy.Date.AddDays(-10);*/

            if (!id.HasValue)
            {
 

                //#### CAFQ: 180604 - Jamaica Universal
                if (UsrCtry == 17 && SInusual == true)       // Jamaica Universal
                {
                    flucase = new FluCase();
                    flucase.HospitalID = HospitalId;
                    flucase.HospitalID_CaseGenerating = HospitalID_CaseGenerating;
                    flucase.flow = 0;
                    db.Entry(flucase).State = EntityState.Added;
                }
                else
                {
                    var DateDummyRange1 = DateFeverDummy.Date.AddDays(1);
                    var DateDummyRange2 = DateFeverDummy.Date.AddDays(-10);
                    var flucases = db.FluCases.Where(f => f.Surv == Surv && f.NoExpediente == NoExpediente.ToUpper() && (f.FeverDate < DateDummyRange1 && f.FeverDate >= DateDummyRange2)).ToList();

                    if (flucases.Any())
                    {
                        flucase = db.FluCases.Find(id);
                        //getMsg("msgRepeatedRegistration")
                        return Json(getMsg("msgRepeatedRegistration") + flucases.First().ID.ToString());
                    }
                    else
                    {
                        flucase = new FluCase();
                        flucase.HospitalID = HospitalId;
                        flucase.HospitalID_CaseGenerating = HospitalID_CaseGenerating;
                        flucase.flow = 0;
                        db.Entry(flucase).State = EntityState.Added;
                    }
                }
                //####
            }
            else
            {
                flucase = db.FluCases.Find(id);
                flucase.HospitalID = HospitalId;
                flucase.HospitalID_CaseGenerating = HospitalID_CaseGenerating;
                db.Entry(flucase).State = EntityState.Modified;
            }
            flucase.Surv = Surv;
            flucase.SurvInusual = SInusual;
            flucase.Brote = Brote;
            flucase.LName1 = LName1 ?? "";
            flucase.LName2 = LName2 ?? "";
            flucase.FName1 = FName1 ?? "";
            flucase.FName2 = FName2 ?? "";
            flucase.FullName = (FName1 ?? "" )+ ( FName2 == "" ? "" : " " )+ (FName2 ?? "");
            flucase.DocumentType = DocumentType;
            flucase.NoExpediente = NoExpediente ?? "";
            flucase.NationalId = NationalId ?? "";
            flucase.DOB = DOB;
            flucase.Age = Age;
            flucase.AMeasure = AMeasure;
            flucase.Gender = Gender;
            flucase.ResponsibleMinor = ResponsibleMinor;    //####CAFQ         
            flucase.HospitalDate = HospitalDate;
            flucase.RegDate = RegDate;
            flucase.nativepeople = nativepeople;
            flucase.nationality = nationality;
            /*flucase.Ocupacion = Ocupacion;                                      //#### CAFQ 
            flucase.TrabajoDirecc = TrabajoDirecc;                              //#### CAFQ 
            flucase.TrabajoEstablec = TrabajoEstablec;                          //#### CAFQ
            flucase.ContactoAnimVivos = ContactoAnimVivos;                      //#### CAFQ
            flucase.OcupacMercAnimVivos = OcupacMercAnimVivos;                  //#### CAFQ*/
            flucase.InsertDate = DateTime.Now;
            //flucase.UserID = User.Identity.Name;
            try
            {
                db.SaveChanges();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            // Grabar en la bitacora

            if (!id.HasValue)
            {
                HistoryRecord(flucase.ID, 1, flucase.flow, 1);
                if (IsSample == false)
                {
                    HistoryRecord(flucase.ID, 1, flucase.flow, 12);
                }
                else
                {
                    HistoryRecord(flucase.ID, 1, flucase.flow, 3);
                }
                
            }
            else
            {
                HistoryRecord(id, 2, flucase.flow, 5);
            }

            return Json(flucase.ID);
        }

        private JsonResult SaveChanges()
        {
            db.SaveChanges();
            return Json("Success");
        }

        // GET: Cases/GetRisk/5
        public ActionResult GetRisk(int Id)
        {
            FluCase flucase = db.FluCases.Find(Id);
            if (flucase == null)
            {
                return HttpNotFound();
            }

            if (flucase != null)
            {
                return Json(new
                {
                    id = flucase.ID.ToString(),
                    Vaccin = flucase.Vaccin,
                    HDisease = flucase.HDisease,
                    SickleCellDisease = flucase.SickleCellDisease,
                    Diabetes = flucase.Diabetes,
                    Neuro = flucase.Neuro,
                    Asthma = flucase.Asthma,
                    Pulmonary = flucase.Pulmonary,
                    Liver = flucase.Liver,
                    Renal = flucase.Renal,
                    Immunsupp = flucase.Immunsupp,
                    ParaCerebral= flucase.ParaCerebral,
                    Indigena = flucase.Indigena,
                    TrabSalud = flucase.TrabSalud,
                    Desnutricion = flucase.Desnutricion,
                    Prematuridad = flucase.Prematuridad,
                    BajoPesoNacer = flucase.BajoPesoNacer,
                    AusLacMat = flucase.AusLacMat,
                    Pregnant = flucase.Pregnant,
                    Pperium = flucase.Pperium,
                    Trimester = flucase.Trimester,
                    PregnantWeek = flucase.PregnantWeek,
                    Smoking = flucase.Smoking,
                    Alcohol = flucase.Alcohol,
                    DownSyn = flucase.DownSyn,
                    Obesity = flucase.Obesity,
                    OtherRisk = flucase.OtherRisk,
                    OtherComment = flucase.OtherComment,
                    RiskFactors = flucase.RiskFactors,
                    Comorbidities = flucase.Comorbidities,
                    VacInfluenza = flucase.VacInfluenza,
                    VacInfluenzaDateFirst = flucase.VacInfluenzaDateFirst,
                    VacInfluenzaDateSecond = flucase.VacInfluenzaDateSecond,
                    VacBcg = flucase.VacBcg,
                    VacBcgDate = flucase.VacBcgDate,
                    VacBcgDosis = flucase.VacBcgDosis,
                    VacNeumococo = flucase.VacNeumococo,
                    VacNeumococoDate = flucase.VacNeumococoDate,
                    VacNeumococoDosis = flucase.VacNeumococoDosis,
                    VacTosFerina = flucase.VacTosFerina,
                    VacTosFerinaDate = flucase.VacTosFerinaDate,
                    VacTosFerinaDosis = flucase.VacTosFerinaDosis,
                    VacHaemophilus = flucase.VacHaemophilus,
                    VacHaemophilusDate = flucase.VacHaemophilusDate,
                    VaccinFuente = flucase.VaccinFuente,
                    AntiViral = flucase.AntiViral,
                    AntiViralDate = flucase.AntiViralDate,
                    AntiViralDateEnd = flucase.AntiViralDateEnd,
                    AntiViralType = flucase.AntiViralType,
                    AntiViralTypeOther = flucase.AntiViralTypeOther,
                    OseltaDose = flucase.OseltaDose,
                    AntiViralDose = flucase.AntiViralDose,
                    AntiViralDays = flucase.AntiViralDays,

                    Antibiotic = flucase.Antibiotic,
                    AntibioticName = flucase.AntibioticName,

                    Ocupacion = flucase.Ocupacion,                             //#### CAFQ
                    TrabajoDirecc = flucase.TrabajoDirecc,                     //#### CAFQ
                    TrabajoEstablec = flucase.TrabajoEstablec,                 //#### CAFQ
                    ContactoAnimVivos = flucase.ContactoAnimVivos,             //#### CAFQ
                    OcupacMercAnimVivos = flucase.OcupacMercAnimVivos,          //#### CAFQ

                    InfeccHospit = flucase.InfeccHospit,                                //#### CAFQ
                    InfeccHospitFecha = flucase.InfeccHospitFecha,                      //#### CAFQ

                    ViajePrevSintoma = flucase.ViajePrevSintoma,                //#### CAFQ
                    DestPrevSintoma1 = flucase.DestPrevSintoma1,                //#### CAFQ
                    DestFechaLlegada1 = flucase.DestFechaLlegada1,                //#### CAFQ
                    DestFechaSalida1 = flucase.DestFechaSalida1,                //#### CAFQ
                    DestPrevSintoma2 = flucase.DestPrevSintoma2,                //#### CAFQ
                    DestFechaLlegada2 = flucase.DestFechaLlegada2,                //#### CAFQ
                    DestFechaSalida2 = flucase.DestFechaSalida2,                //#### CAFQ
                    DestPrevSintoma3 = flucase.DestPrevSintoma3,                //#### CAFQ
                    DestFechaLlegada3 = flucase.DestFechaLlegada3,                //#### CAFQ
                    DestFechaSalida3 = flucase.DestFechaSalida3,                //#### CAFQ
                    ContacDirectoAnim = flucase.ContacDirectoAnim,                //#### CAFQ
                    AnimalNaturaContac = flucase.AnimalNaturaContac,                //#### CAFQ
                    ExpuextoSimilSintoma = flucase.ExpuextoSimilSintoma,                //#### CAFQ
                    NumeIdentContacto = flucase.NumeIdentContacto,                //#### CAFQ
                    InfluConfirContacto = flucase.InfluConfirContacto,                //#### CAFQ
                    TipoRelaContacto = flucase.TipoRelaContacto,                //#### CAFQ
                    FamiDirecContacto = flucase.FamiDirecContacto,                //#### CAFQ
                    TrabSaludRama = flucase.TrabSaludRama,                //#### CAFQ
                    TrabLaboratorio = flucase.TrabLaboratorio,                //#### CAFQ
                    TrabLaboratorioRama = flucase.TrabLaboratorioRama                //#### CAFQ

                }, JsonRequestBehavior.AllowGet);
            };
            return Json(new
                 {
                     id = "",
                     Vaccin = 9,
                     HDisease = false,
                     SickleCellDisease = false,
                     Diabetes = false,
                     Neuro = false,
                     Asthma = false,
                     Pulmonary = false,
                     Liver = false,
                     Renal = false,
                     Immunsupp = false,
                     Pregnant = 0,
                     Pperium = false,
                     Trimester = 0,
                     Smoking = false,
                     Alcohol = false,
                     DownSyn = false,
                     Obesity = 0,
                     OtherRisk = "",
                     AntiViralDose = ""
            },
                 JsonRequestBehavior.AllowGet);
        }

        // POST: Cases/SaveRisk/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult SaveRisk(
                int Id,
                Vaccin? Vaccin,
                bool? HDisease,
                bool? SickleCellDisease,
                bool? Diabetes,
                bool? Neuro,
                bool? Asthma,
                bool? Pulmonary,
                bool? Liver,
                bool? Renal,
                bool? Immunsupp,
                bool? ParaCerebral,
                bool? Indigena,
                bool? TrabSalud,
                bool? Desnutricion,
                bool? Prematuridad,
                bool? BajoPesoNacer,
                bool? AusLacMat,
                Pregnant? Pregnant,
                bool? Pperium,
                Trimester? Trimester,
                int? PregnantWeek,
                bool? Smoking,
                bool? Alcohol,
                bool? DownSyn,
                Obesity? Obesity,
                string OtherRisk,
                string OtherComment,
                int? VacInfluenza,
                DateTime? VacInfluenzaDateFirst,
                DateTime? VacInfluenzaDateSecond,
                int? VacBcg,
                DateTime? VacBcgDate,
                int? VacBcgDosis,
                int? VacNeumococo,
                DateTime? VacNeumococoDate,
                int? VacNeumococoDosis,
                int? VacTosFerina,
                DateTime? VacTosFerinaDate,
                int? VacTosFerinaDosis,
                int? VacHaemophilus,
                DateTime? VacHaemophilusDate,
                int? VaccinFuente,

                int? AntiViral,
                DateTime? AntiViralDate,
                DateTime? AntiViralDateEnd,
                int? AntiViralType,
                string AntiViralTypeOther,
                int? OseltaDose,
                string AntiViralDose,
                int? AntiViralDays,

                // Antibiotic
                int? Antibiotic,
                string AntibioticName,                

                int? RiskFactors,
                int? Comorbidities,

                int? Ocupacion,                         //#### CAFQ
                string TrabajoDirecc,                   //#### CAFQ
                string TrabajoEstablec,                 //#### CAFQ
                int? ContactoAnimVivos,                 //#### CAFQ
                int? OcupacMercAnimVivos,                //#### CAFQ

                int? InfeccHospit,                              //#### CAFQ
                DateTime? InfeccHospitFecha,                    //#### CAFQ

                int? ViajePrevSintoma,              //#### CAFQ
                string DestPrevSintoma1,             //#### CAFQ
                DateTime? DestFechaLlegada1,          //#### CAFQ
                DateTime? DestFechaSalida1,          //#### CAFQ
                string DestPrevSintoma2,             //#### CAFQ
                DateTime? DestFechaLlegada2,          //#### CAFQ
                DateTime? DestFechaSalida2,          //#### CAFQ
                string DestPrevSintoma3,             //#### CAFQ
                DateTime? DestFechaLlegada3,          //#### CAFQ
                DateTime? DestFechaSalida3,          //#### CAFQ
                int? ContacDirectoAnim,              //#### CAFQ
                string AnimalNaturaContac,             //#### CAFQ
                int? ExpuextoSimilSintoma,              //#### CAFQ
                string NumeIdentContacto,             //#### CAFQ
                int? InfluConfirContacto,              //#### CAFQ
                string TipoRelaContacto,             //#### CAFQ
                int? FamiDirecContacto,              //#### CAFQ
                int? TrabSaludRama,                         //#### CAFQ
                bool? TrabLaboratorio,                      //#### CAFQ
                int? TrabLaboratorioRama                    //#### CAFQ
            )
        {
            FluCase flucase;

            flucase = db.FluCases.Find(Id);
            if (flucase == null)
            {
                flucase = new FluCase();
                db.Entry(flucase).State = EntityState.Added;
            }
            else
            {
                db.Entry(flucase).State = EntityState.Modified;
            }
            flucase.Vaccin = Vaccin;
            flucase.HDisease = HDisease;
            flucase.SickleCellDisease = SickleCellDisease;
            flucase.Diabetes = Diabetes;
            flucase.Neuro = Neuro;
            flucase.Asthma = Asthma;
            flucase.Pulmonary = Pulmonary;
            flucase.Liver = Liver;
            flucase.Renal = Renal;
            flucase.Immunsupp = Immunsupp;

            flucase.ParaCerebral = ParaCerebral;
            flucase.Indigena = Indigena;
            flucase.TrabSalud = TrabSalud;
            flucase.Desnutricion = Desnutricion;
            flucase.Prematuridad = Prematuridad;
            flucase.BajoPesoNacer = BajoPesoNacer;
            flucase.AusLacMat = AusLacMat;

            flucase.Pregnant = Pregnant;
            flucase.Pperium = Pperium;
            flucase.Trimester = Trimester;
            flucase.PregnantWeek = PregnantWeek;
            flucase.Smoking = Smoking;
            flucase.Alcohol = Alcohol;
            flucase.DownSyn = DownSyn;
            flucase.Obesity = Obesity;
            flucase.OtherRisk = OtherRisk;
            flucase.OtherComment = OtherComment;
            flucase.InsertDate = DateTime.Now;
            //flucase.UserID = User.Identity.Name;

            //flucase.VacInfluenza = (VacInfluenza.ToString() == null) ? VacInfluenza : (VaccineOptions)Enum.Parse(typeof(VaccineOptions), VacInfluenza.ToString());
            flucase.VacInfluenza = VacInfluenza;
            flucase.VacInfluenzaDateFirst = VacInfluenzaDateFirst;
            flucase.VacInfluenzaDateSecond = VacInfluenzaDateSecond;
            if (VacBcg != null) flucase.VacBcg = (VaccineOptions)Enum.Parse(typeof(VaccineOptions), VacBcg.ToString());
            flucase.VacBcgDate = VacBcgDate;
            flucase.VacBcgDosis = VacBcgDosis;
            if (VacNeumococo != null) flucase.VacNeumococo = (VaccineOptions)Enum.Parse(typeof(VaccineOptions), VacNeumococo.ToString());
            flucase.VacNeumococoDate = VacNeumococoDate;
            flucase.VacNeumococoDosis = VacNeumococoDosis;
            if (VacTosFerina != null) flucase.VacTosFerina = (VaccineOptions)Enum.Parse(typeof(VaccineOptions), VacTosFerina.ToString());
            flucase.VacTosFerinaDate = VacTosFerinaDate;
            flucase.VacTosFerinaDosis = VacTosFerinaDosis;
            if (VacHaemophilus != null) flucase.VacHaemophilus = (VaccineOptions)Enum.Parse(typeof(VaccineOptions), VacHaemophilus.ToString());
            flucase.VacHaemophilusDate = VacHaemophilusDate;
            flucase.VaccinFuente = VaccinFuente;
            //if (AntiViral != null)  flucase.AntiViral = (VaccineOptions)Enum.Parse(typeof(VaccineOptions), AntiViral.ToString());
            if (AntiViral != null) flucase.AntiViral = AntiViral;
            flucase.AntiViralDate = AntiViralDate;
            flucase.AntiViralDateEnd = AntiViralDateEnd;
            flucase.AntiViralType = AntiViralType;
            flucase.AntiViralTypeOther = AntiViralTypeOther;
            flucase.OseltaDose = OseltaDose;
            flucase.AntiViralDose = AntiViralDose;
            flucase.AntiViralDays = AntiViralDays;

            flucase.Antibiotic = Antibiotic;
            flucase.AntibioticName = AntibioticName;

            if (AntiViral != null)  flucase.RiskFactors = (RiskFactor)Enum.Parse(typeof(RiskFactor), RiskFactors.ToString());
            flucase.Comorbidities = Comorbidities;

            flucase.Ocupacion = Ocupacion;                                      //#### CAFQ 
            flucase.TrabajoDirecc = TrabajoDirecc;                              //#### CAFQ 
            flucase.TrabajoEstablec = TrabajoEstablec;                          //#### CAFQ
            flucase.ContactoAnimVivos = ContactoAnimVivos;                      //#### CAFQ
            flucase.OcupacMercAnimVivos = OcupacMercAnimVivos;                  //#### CAFQ

            flucase.InfeccHospit = InfeccHospit;                    //#### CAFQ
            flucase.InfeccHospitFecha = InfeccHospitFecha;          //#### CAFQ

            flucase.ViajePrevSintoma = ViajePrevSintoma;        //#### CAFQ
            flucase.DestPrevSintoma1 = DestPrevSintoma1;        //#### CAFQ
            flucase.DestFechaLlegada1 = DestFechaLlegada1;        //#### CAFQ
            flucase.DestFechaSalida1 = DestFechaSalida1;        //#### CAFQ
            flucase.DestPrevSintoma2 = DestPrevSintoma2;        //#### CAFQ
            flucase.DestFechaLlegada2 = DestFechaLlegada2;        //#### CAFQ
            flucase.DestFechaSalida2 = DestFechaSalida2;        //#### CAFQ
            flucase.DestPrevSintoma3 = DestPrevSintoma3;        //#### CAFQ
            flucase.DestFechaLlegada3 = DestFechaLlegada3;        //#### CAFQ
            flucase.DestFechaSalida3 = DestFechaSalida3;        //#### CAFQ

            flucase.ContacDirectoAnim = ContacDirectoAnim;        //#### CAFQ
            flucase.AnimalNaturaContac = AnimalNaturaContac;        //#### CAFQ
            flucase.ExpuextoSimilSintoma = ExpuextoSimilSintoma;        //#### CAFQ
            flucase.NumeIdentContacto = NumeIdentContacto;        //#### CAFQ
            flucase.InfluConfirContacto = InfluConfirContacto;        //#### CAFQ
            flucase.TipoRelaContacto = TipoRelaContacto;        //#### CAFQ
            flucase.FamiDirecContacto = FamiDirecContacto;        //#### CAFQ
            flucase.TrabSaludRama = TrabSaludRama;                      //#### CAFQ
            flucase.TrabLaboratorio = TrabLaboratorio;                  //#### CAFQ
            flucase.TrabLaboratorioRama = TrabLaboratorioRama;          //#### CAFQ

            db.SaveChanges();

            return Json("Success");
        }

        public ActionResult GetLabsHospital(long? institutionId)
        {
            if (institutionId == null) return Json(new { }, JsonRequestBehavior.AllowGet);
            return Json(new
            {
                LabsHospital = GetLabsByInstitution(Convert.ToInt32(institutionId)).Select(x => new { Id = x.ID, x.Name , orden = x.OrdenPrioritybyLab }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        private IQueryable<Institution> GetLabsByInstitution(long institutionId) {
            if (institutionId == 0)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                var service_inst = db.Institutions.Where(j => j.Father_ID == user.InstitutionID && j.AccessLevel == AccessLevel.Service).Any();

                if (user.Institution.AccessLevel == AccessLevel.Service || (user.Institution.AccessLevel == AccessLevel.SelfOnly && user.Institution is Hospital && service_inst == true))
                {
                    institutionId = (long)user.InstitutionID;
                } 

            }
            //var user = UserManager.FindById(User.Identity.GetUserId());
            //if (institutionId == 0) { institutionId = Convert.ToInt32(user.InstitutionID); }
            var institutionsConfiguration = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>()
                    .Where(i => i.InstitutionFromID == institutionId && i.Priority == 1);

            var institutionLabs = institutionsConfiguration.Select(i => i.InstitutionTo);
            if (!institutionLabs.Any()) {
                var user = UserManager.FindById(User.Identity.GetUserId());
                institutionLabs = db.Institutions.OfType<Lab>().Where(i => i.CountryID == user.Institution.CountryID);
            }

            return institutionLabs;
        }

        // GET: Cases/GetHospital/5
        public ActionResult GetHospital(int Id, long institutionId)
        {
            FluCase flucase = db.FluCases.Find(Id);
            if (flucase == null)
            {
                return HttpNotFound();
            }

            //if ( institutionId <= 0 ) { institutionId = flucase.HospitalID; }

            var institutionLabs = GetLabsByInstitution(institutionId);

            if (flucase != null)
            {
                return Json(new
                {
                    id = flucase.ID.ToString(),
                    CHNum = flucase.CHNum,
                    FeverDate = flucase.FeverDate,
                    DiagDate = flucase.DiagDate,
                    SalonVal = flucase.Salon,
                    DiagPrinAdmVal = flucase.DiagAdm,
                    DiagOtroAdm = flucase.DiagOtroAdm,
                    //Antiviral = flucase.Antiviral,
                    AStartDate = flucase.AStartDate,
                    HospAmDate = flucase.HospAmDate,
                    HospExDate = flucase.HospExDate,
                    ICU = flucase.ICU,
                    ICUAmDate = flucase.ICUAmDate,
                    ICUExDate = flucase.ICUExDate,
                    HospitalizedIn = flucase.HospitalizedIn,
                    HospitalizedInNumber = flucase.HospitalizedInNumber,
                    FalleDate = flucase.FalleDate,
                    InstReferName = flucase.InstReferName,
                    Destin = flucase.Destin,
                    DestinICU = flucase.DestinICU,
                    HallRadio = flucase.HallRadio,
                    HallRadioFindings = flucase.HallRadioFindings,      //#### CAFQ
                    UCInt = flucase.UCInt,                              // Unidad de cuidados intermedios
                    UCri = flucase.UCri,                                // Unidad critica
                    MecVent = flucase.MecVent,                          // Ventilacion mecanica
                    MecVentNoInv = flucase.MecVentNoInv,                // Ventilacion mecanica no invasiva
                    ECMO = flucase.ECMO,                                // Oxigenación por membrana extracorpórea
                    VAFO = flucase.VAFO,                                // Ventilación de alta frecuencia oscilatoria
                    Comments = flucase.Comments,                        //#### CAFQ: 190107 Comentario
                    DiagEgVal = flucase.DiagEg,
                    DiagEgOtro = flucase.DiagEgOtro, 
                    IsSample = flucase.IsSample,
                    ReasonNotSamplingID = flucase.ReasonNotSamplingID,
                    ReasonNotSamplingOther = flucase.ReasonNotSamplingOther,
                    SampleDate = flucase.SampleDate,                    // Muestra 1
                    SampleType = flucase.SampleType,
                    ShipDate = flucase.ShipDate,
                    LabId = flucase.LabID,
                    SampleDate2 = flucase.SampleDate2,                  // Muestra 2
                    SampleType2 = flucase.SampleType2,
                    ShipDate2 = flucase.ShipDate2,
                    LabId2 = flucase.LabID2,
                    SampleDate3 = flucase.SampleDate3,                  // Muestra 3
                    SampleType3 = flucase.SampleType3,
                    ShipDate3 = flucase.ShipDate3,
                    LabId3 = flucase.LabID3,
                    Adenopatia = flucase.Adenopatia,
                    Wheezing = flucase.Wheezing,                                        //#### CAFQ: 180619
                    AntecedentesFiebre = flucase.AntecedentesFiebre,
                    Rinorrea = flucase.Rinorrea,
                    //Malestar = flucase.Malestar,
                    //Nauseas = flucase.Nauseas,
                    //DolorMuscular = flucase.DolorMuscular,
                    //Disnea = flucase.Disnea,
                    //DolorCabeza = flucase.DolorCabeza,
                    Estridor = flucase.Estridor,
                    Tos = flucase.Tos,

                    Temperatura = flucase.Temperatura,                                  //#### CAFQ
                    DolorCabeza = flucase.DolorCabeza,
                    Mialgia = flucase.Mialgia,                                          //#### CAFQ
                    Erupcion = flucase.Erupcion,                                        //#### CAFQ
                    ErupcionLocaliz = flucase.ErupcionLocaliz,                          //#### CAFQ
                    DolorMuscular = flucase.DolorMuscular,
                    DolorMuscularLocaliz = flucase.DolorMuscularLocaliz,                //#### CAFQ
                    Disnea = flucase.Disnea,
                    SintomHemorrag = flucase.SintomHemorrag,                            //#### CAFQ
                    SintomHemorragDesc = flucase.SintomHemorragDesc,                    //#### CAFQ
                    AlteracEstadoMental = flucase.AlteracEstadoMental,                  //#### CAFQ
                    Altralgia = flucase.Altralgia,                                      //#### CAFQ
                    Escalofrios = flucase.Escalofrios,                                  //#### CAFQ
                    Conjuntivitis = flucase.Conjuntivitis,                              //#### CAFQ
                    Rinitis = flucase.Rinitis,                                          //#### CAFQ
                    DiarreaAguda = flucase.DiarreaAguda,                                //#### CAFQ
                    DiarreaCronica = flucase.DiarreaCronica,                            //#### CAFQ
                    Mareo = flucase.Mareo,                                              //#### CAFQ
                    FalloDesarrollo = flucase.FalloDesarrollo,                          //#### CAFQ
                    Hepatomegalea = flucase.Hepatomegalea,                              //#### CAFQ
                    Ictericia = flucase.Ictericia,                                      //#### CAFQ
                    Linfadenopatia = flucase.Linfadenopatia,                            //#### CAFQ
                    Malestar = flucase.Malestar,
                    Nauseas = flucase.Nauseas,
                    RigidezNuca = flucase.RigidezNuca,                                  //#### CAFQ
                    Paralisis = flucase.Paralisis,                                      //#### CAFQ
                    RespiratSuperior = flucase.RespiratSuperior,                        //#### CAFQ
                    RespiratInferior = flucase.RespiratInferior,                        //#### CAFQ
                    DolorRetrorobitario = flucase.DolorRetrorobitario,                  //#### CAFQ
                    PerdidaPeso = flucase.PerdidaPeso,                                  //#### CAFQ
                    Otro = flucase.Otro,                                                //#### CAFQ
                    OtroDesc = flucase.OtroDesc,                                        //#### CAFQ
                    /*InfeccHospit = flucase.InfeccHospit,                                //#### CAFQ
                    InfeccHospitFecha = flucase.InfeccHospitFecha,                      //#### CAFQ*/

                    DifResp = flucase.DifResp,
                    MedSatOxig = flucase.MedSatOxig,
                    SatOxigPor = flucase.SatOxigPor,
                    Tiraje = flucase.Tiraje,
                    Odinofagia = flucase.Odinofagia,

                    CaseStatus = flucase.CaseStatus,
                    CloseDate = flucase.CloseDate,
                    ObservationCase = flucase.ObservationCase,
                    LabsHospital = institutionLabs.Select(x => new { Id = x.ID, x.Name }).ToList(),
                }, JsonRequestBehavior.AllowGet);
            };
            return Json(new
            {
                id = "",
                CHNum = "",
                FeverDate = new DateTime?(),
                Antiviral = "N",
                AStartDate = new DateTime?(),
                HospAmDate = new DateTime?(),
                HospExDate = new DateTime?(),
                ICUAmDate = new DateTime?(),
                ICUExDate = new DateTime?(),
                Destin = "",
                IsSample = false,
                SampleDate = new DateTime?(),
                SampleType = "",
                ShipDate = new DateTime?(),
                LabId = "",
                LabsHospital = institutionLabs.Select(x => new { x.ID, x.Name }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        // POST: Cases/SaveHospital/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult SaveHospital(
                int id,
                string CHNum,
                DateTime? FeverDate,
                int? FeverEW,
                int? FeverEY,
                DateTime? DiagDate,
                int? DiagEW,
                string Antiviral,
                DateTime? AStartDate,
                DateTime? HospAmDate,
                int? HospEW,
                DateTime? HospExDate,
                int? ICU,
                DateTime? ICUAmDate,
                int? ICUEW,
                DateTime? ICUExDate,
                int? HospitalizedIn,
                string HospitalizedInNumber,
                string Destin,
                bool? IsSample,
                int? ReasonNotSamplingID,
                string ReasonNotSamplingOther,
                DateTime? SampleDate,                   // Sample 1
                string SampleType,
                DateTime? ShipDate,
                long? LabId,
                DateTime? SampleDate2,                  // Sample 2
                string SampleType2,
                DateTime? ShipDate2,
                long? LabId2,
                DateTime? SampleDate3,
                string SampleType3,                     // Sample 3
                DateTime? ShipDate3,
                long? LabId3,
                bool? Adenopatia,
                bool? Wheezing,       
                bool? AntecedentesFiebre,
                bool? Rinorrea,
                //bool? Malestar,
                //bool? Nauseas,
                //bool? DolorMuscular,
                //bool? Disnea,
                //bool? DolorCabeza,
                bool? Estridor,
                bool? Tos,
                Decimal? Temperatura,                           //#### CAFQ
                bool? DolorCabeza,                              //#### CAFQ
                bool? Mialgia,                                  //#### CAFQ
                bool? Erupcion,                                 //#### CAFQ
                string ErupcionLocaliz,                         //#### CAFQ
                bool? DolorMuscular,                            //#### CAFQ
                string DolorMuscularLocaliz,                    //#### CAFQ
                bool? Disnea,                                   //#### CAFQ
                bool? SintomHemorrag,                           //#### CAFQ
                string SintomHemorragDesc,                      //#### CAFQ
                bool? AlteracEstadoMental,                      //#### CAFQ
                bool? Altralgia,                                //#### CAFQ
                bool? Escalofrios,                              //#### CAFQ
                bool? Conjuntivitis,                            //#### CAFQ
                bool? Rinitis,                                  //#### CAFQ
                bool? DiarreaAguda,                             //#### CAFQ
                bool? DiarreaCronica,                           //#### CAFQ
                bool? Mareo,                                    //#### CAFQ
                bool? FalloDesarrollo,                          //#### CAFQ
                bool? Hepatomegalea,                            //#### CAFQ
                bool? Ictericia,                                //#### CAFQ
                bool? Linfadenopatia,                           //#### CAFQ
                bool? Malestar,                                 //#### CAFQ
                bool? Nauseas,                                  //#### CAFQ
                bool? RigidezNuca,                              //#### CAFQ
                bool? Paralisis,                                //#### CAFQ
                bool? RespiratSuperior,                         //#### CAFQ
                bool? RespiratInferior,                         //#### CAFQ
                bool? DolorRetrorobitario,                      //#### CAFQ
                bool? PerdidaPeso,                              //#### CAFQ
                bool? Otro,                                     //#### CAFQ
                string OtroDesc,                                //#### CAFQ
                /*int? InfeccHospit,                              //#### CAFQ
                DateTime? InfeccHospitFecha,                    //#### CAFQ*/
                bool? DifResp,
                bool? MedSatOxig,
                int? SatOxigPor,
                int? SalonVal,
                int? DiagPrinAdmVal,
                string DiagOtroAdm,
                string DestinICU,
                DateTime? FalleDate,
                string InstReferName,
                bool? HallRadio,
                string HallRadioFindings,                       //#### CAFQ
                bool? UCInt,
                bool? UCri,
                bool? MecVent,
                bool? MecVentNoInv,
                bool? ECMO,
                bool? VAFO,
                string Comments,                                //#### CAFQ: 190107
                int? DiagEgVal,
                string DiagEgOtro,                              //#### CAFQ
                bool? Tiraje,
                bool? Odinofagia,
                int? CaseStatus,
                DateTime? CloseDate,
                string ObservationCase,
                int? DataStatement
            )
        {
            FluCase flucase;

            int CaseStatusD = CaseStatus ?? 0;

            flucase = db.FluCases.Find(id);
            if (flucase == null)
            {
                flucase = new FluCase();
                db.Entry(flucase).State = EntityState.Added;
            }
            else
            {
                db.Entry(flucase).State = EntityState.Modified;
            }

            //inicializar llaves
            if (Destin == "") Destin = null;

            bool ifclosecase = flucase.flow == 99;

            flucase.CHNum = CHNum;
            flucase.FeverDate = FeverDate;
            flucase.FeverEW = FeverEW;
            flucase.FeverEY = FeverEY;
            flucase.DiagDate = DiagDate;
            if (flucase.DOB != null)
            {
               var months_age =  flucase.DOB.Value.Subtract(flucase.FeverDate.Value).Days / (365.25 / 12);
            } else
            {

            }
            flucase.DiagEW = DiagEW;
            //flucase.Antiviral = Antiviral;
            flucase.AStartDate = AStartDate;
            flucase.HospAmDate = HospAmDate;
            flucase.HospEW = HospEW;
            flucase.HospExDate = HospExDate;
            flucase.ICU = ICU;
            flucase.ICUAmDate = ICUAmDate;
            flucase.ICUEW = ICUEW;
            flucase.ICUExDate = ICUExDate;
            flucase.HospitalizedIn = HospitalizedIn;
            flucase.HospitalizedInNumber = HospitalizedInNumber;
            flucase.Destin = Destin;
            flucase.FalleDate = FalleDate;
            flucase.InstReferName = InstReferName;
            flucase.IsSample = IsSample;
            flucase.ReasonNotSamplingID = ReasonNotSamplingID;
            flucase.ReasonNotSamplingOther = ReasonNotSamplingOther;
            flucase.SampleDate = SampleDate;
            flucase.SampleType = SampleType;
            flucase.ShipDate = ShipDate;
            flucase.LabID = LabId;
            flucase.SampleDate2 = SampleDate2;
            flucase.SampleType2 = SampleType2;
            flucase.ShipDate2 = ShipDate2;
            flucase.LabID2 = LabId2;
            flucase.SampleDate3 = SampleDate3;
            flucase.SampleType3 = SampleType3;
            flucase.ShipDate3 = ShipDate3;
            flucase.LabID3 = LabId3;
            flucase.InsertDate = DateTime.Now;
            //flucase.UserID = User.Identity.Name;
            flucase.Adenopatia = Adenopatia;
            flucase.Wheezing = Wheezing;                            //#### CAFQ: 180619
            flucase.AntecedentesFiebre = AntecedentesFiebre;
            flucase.Rinorrea = Rinorrea;
            //flucase.Malestar = Malestar;
            //flucase.Nauseas = Nauseas;
            //flucase.DolorMuscular = DolorMuscular;
            //flucase.Disnea = Disnea;
            //flucase.DolorCabeza = DolorCabeza;
            flucase.Estridor = Estridor;
            flucase.Tos = Tos;
            flucase.Temperatura = Temperatura;                      //#### CAFQ
            flucase.DolorCabeza = DolorCabeza;
            flucase.Mialgia = Mialgia;                              //#### CAFQ
            flucase.Erupcion = Erupcion;                            //#### CAFQ
            flucase.ErupcionLocaliz = ErupcionLocaliz;              //#### CAFQ
            flucase.DolorMuscular = DolorMuscular;                  //#### CAFQ
            flucase.DolorMuscularLocaliz = DolorMuscularLocaliz;    //#### CAFQ
            flucase.Disnea = Disnea;                                //#### CAFQ
            flucase.SintomHemorrag = SintomHemorrag;                //#### CAFQ
            flucase.SintomHemorragDesc = SintomHemorragDesc;        //#### CAFQ
            flucase.AlteracEstadoMental = AlteracEstadoMental;      //#### CAFQ
            flucase.Altralgia = Altralgia;                          //#### CAFQ
            flucase.Escalofrios = Escalofrios;                      //#### CAFQ
            flucase.Conjuntivitis = Conjuntivitis;                  //#### CAFQ
            flucase.Rinitis = Rinitis;                              //#### CAFQ
            flucase.DiarreaAguda = DiarreaAguda;                    //#### CAFQ
            flucase.DiarreaCronica = DiarreaCronica;                //#### CAFQ
            flucase.Mareo = Mareo;                                  //#### CAFQ
            flucase.FalloDesarrollo = FalloDesarrollo;              //#### CAFQ
            flucase.Hepatomegalea = Hepatomegalea;                  //#### CAFQ
            flucase.Ictericia = Ictericia;                          //#### CAFQ
            flucase.Linfadenopatia = Linfadenopatia;                //#### CAFQ
            flucase.Malestar = Malestar;                            //#### CAFQ
            flucase.Nauseas = Nauseas;                              //#### CAFQ
            flucase.RigidezNuca = RigidezNuca;                      //#### CAFQ
            flucase.Paralisis = Paralisis;                          //#### CAFQ
            flucase.RespiratSuperior = RespiratSuperior;            //#### CAFQ
            flucase.RespiratInferior = RespiratInferior;            //#### CAFQ
            flucase.DolorRetrorobitario = DolorRetrorobitario;      //#### CAFQ
            flucase.PerdidaPeso = PerdidaPeso;                      //#### CAFQ
            flucase.Otro = Otro;                                    //#### CAFQ
            flucase.OtroDesc = OtroDesc;                            //#### CAFQ
            /*flucase.InfeccHospit = InfeccHospit;                    //#### CAFQ
            flucase.InfeccHospitFecha = InfeccHospitFecha;          //#### CAFQ*/
            flucase.DifResp = DifResp;
            flucase.MedSatOxig = MedSatOxig;
            flucase.SatOxigPor = SatOxigPor;
            flucase.Salon = SalonVal;
            flucase.DiagAdm = DiagPrinAdmVal;
            flucase.DiagOtroAdm = DiagOtroAdm;
            flucase.DestinICU = DestinICU;
            flucase.HallRadio = HallRadio;
            flucase.HallRadioFindings = HallRadioFindings;          //#### CAFQ*/
            flucase.UCInt = UCInt;
            flucase.UCri = UCri;
            flucase.MecVent = MecVent;
            flucase.MecVentNoInv = MecVentNoInv;
            flucase.ECMO = ECMO;
            flucase.VAFO = VAFO;
            flucase.Comments = Comments;                            //#### CAFQ: 190107
            flucase.DiagEg = DiagEgVal;
            flucase.DiagEgOtro = DiagEgOtro;                        //#### CAFQ
            flucase.Tiraje = Tiraje;
            flucase.Odinofagia = Odinofagia;
            flucase.CaseStatus = CaseStatus;
            if (CaseStatusD > 0)
                flucase.flow = 99 ;
            flucase.statement = DataStatement;
            flucase.CloseDate = CloseDate;
            flucase.ObservationCase = ObservationCase;
            db.SaveChanges();

            if (flucase.flow == 99 && ifclosecase == false )
                HistoryRecord(flucase.ID, 1, flucase.flow, 9);

            return Json("Success");
        }

        //// GET: Cases/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Case @case = db.FluCases.Find(id);
        //    if (@case == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(@case);
        //}

        //// POST: Cases/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Case @case = db.FluCases.Find(id);
        //    db.FluCases.Remove(@case);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        // GET: Cases/GetGEO/5
        public ActionResult GetGEO(int Id)
        {
            FluCase flucase = db.FluCases.Find(Id);
            if (flucase == null)
            {
                return HttpNotFound();
            }

            if (flucase != null)
            {
                return Json(new
                {
                    id = flucase.ID.ToString(),
                    CountryID = flucase.CountryID,
                    AreaID = flucase.AreaID,
                    LocalID = flucase.LocalID,
                    StateID = flucase.StateID,
                    ParishPostOfficeJMID = flucase.ParishPostOfficeIDJMID,
                    StreetNo = flucase.StreetNo,
                    StreetName = flucase.StreetName,
                    ApartmentSuiteLot = flucase.ApartmentSuiteLot,
                    Address2 = flucase.Address2,
                    NeighborhoodID = flucase.NeighborhoodID,
                    HamletID = flucase.HamletID,                        //#### CAFQ: 181018
                    ColonyID = flucase.ColonyID,                        //#### CAFQ: 181018
                    UrbanRural = flucase.UrbanRural,
                    CountryID2weeks = flucase.CountryID2weeks,
                    AreaID2weeks = flucase.AreaID2weeks,
                    StateID2weeks = flucase.StateID2weeks,
                    NeighborhoodID2weeks = flucase.NeighborhoodID2weeks,
                    Address = flucase.Address,
                    PhoneNumber = flucase.PhoneNumber,
                    Latitude = flucase.Latitude,
                    Longitude = flucase.Longitude,
                    CountryOrigin = flucase.CountryOrigin,
                    RegionAddress = flucase.RegionAddress                   //#### CAFQ: 181008
                }, JsonRequestBehavior.AllowGet);
            };
            return Json(new
            {
                id = "",
                CountryID = "",
                AreaID = "",
                StateID = "",
                LocalID = "",
                NeighborhoodID = "",
                HamletID = "",
                ColonyID = "",                                              //#### CAFQ: 181008
                UrbanRural = UrbanRural.Unknow,
                Address = "",
                StreetNo = "",
                StreetName = "",
                ApartmentSuiteLot = "",
                Address2 = "",
                CountryOrigin = ""
            },
            JsonRequestBehavior.AllowGet);
        }

        // POST: Cases/SaveGEO/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult SaveGEO(
                int id,
                int? CountryId,
                int? AreaId,
                int? StateId,
                int? ParishPostOfficeJMId,
                string StreetNo,
                string StreetName,
                string ApartmentSuiteLot,
                string Address2,
                int? LocalId,
                int? NeighborhoodId,
                int? HamletID,                      //#### CAFQ: 181018
                int? ColonyID,                      //#### CAFQ: 181018
                UrbanRural UrbanRural,
                int? CountryId2weeks,
                int? AreaId2weeks,
                int? StateId2weeks,
                int? LocalId2weeks,
                int? NeighborhoodId2weeks,
                string Address,
                int? CountryOrigin,
                string PhoneNumber,
                string Latitude,
                string Longitude,
                int? RegionAddress
            )
        {
            FluCase flucase;

            flucase = db.FluCases.Find(id);
            if (flucase == null)
            {
                flucase = new FluCase();
                db.Entry(flucase).State = EntityState.Added;
            }
            else
            {
                db.Entry(flucase).State = EntityState.Modified;
            }
            flucase.CountryID = CountryId;
            flucase.AreaID = AreaId;
            flucase.StateID = StateId;
            flucase.ParishPostOfficeIDJMID = ParishPostOfficeJMId;
            flucase.StreetNo = StreetNo;
            flucase.StreetName = StreetName;
            flucase.ApartmentSuiteLot = ApartmentSuiteLot;
            flucase.Address2 = Address2;
            flucase.LocalID = LocalId;
            flucase.NeighborhoodID = NeighborhoodId;
            flucase.HamletID = HamletID;                        //#### CAFQ: 181018
            flucase.ColonyID = ColonyID;                        //#### CAFQ: 181018
            flucase.UrbanRural = UrbanRural;
            flucase.CountryID2weeks = CountryId2weeks;
            flucase.AreaID2weeks = AreaId2weeks;
            flucase.StateID2weeks = StateId2weeks;
            flucase.NeighborhoodID2weeks = NeighborhoodId2weeks;
            flucase.Address = Address;
            flucase.PhoneNumber = PhoneNumber;
            flucase.Latitude = Latitude;
            flucase.Longitude = Longitude;
            flucase.CountryOrigin = CountryOrigin;
            flucase.InsertDate = DateTime.Now;
            flucase.RegionAddress = RegionAddress;              //#### CAFQ: 181008
            //flucase.UserID = User.Identity.Name;

            db.SaveChanges();

            return Json("Success");
        }

        // GET: Cases/GetGEO/5
        public ActionResult GetLab(int Id)
        {
            FluCase flucase = db.FluCases.Find(Id);
            if (flucase == null)
            {
                return HttpNotFound();
            }

            IQueryable<Institution> institutions = null;


            var user = UserManager.FindById(User.Identity.GetUserId());
            institutions = db.Institutions.OfType<Lab>().Where(i => i.ID == user.Institution.ID);
            var institutionsIds = institutions.Select(x => (long?)x.ID).ToArray();
            var user_cty = user.Institution.CountryID;

            //var InstFlow_NPHL = false;
            //var ExistInstitutionFlow_NPHL = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where( i => i.InstitutionParentID == flucase.HospitalID && i.InstitutionTo.NPHL == true).Any();
            var canConclude = true;
            var canConclude_Sample_1 = false;
            var canConclude_Sample_2 = false;
            var canConclude_Sample_3 = false;

            var SaveAndAdd_1 = true;
            var SaveAndAdd_2 = true;
            var SaveAndAdd_3 = true;
            var CanPCRLab = db.Institutions.Where(i => i.ID == user.Institution.ID).First()?.PCR;
            var CanIFILab = db.Institutions.Where(i => i.ID == user.Institution.ID).First()?.IFI;

            var institutionsConfiguration = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(i => i.InstitutionToID == user.Institution.ID && i.InstitutionParentID == flucase.HospitalID);
            var institutionAnteriorFlow = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(i => i.Priority == flucase.flow - 1 && i.InstitutionParentID == flucase.HospitalID);
            var institutionActualFlow = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(i => i.Priority == flucase.flow && i.InstitutionParentID == flucase.HospitalID);
            var institucionActualAndPreviousFlow = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(i => i.Priority <= flucase.flow && i.InstitutionParentID == flucase.HospitalID).Select(d=> d.ID);
            var flow_local_lab = 0;
            var flow_statement = flucase.statement ?? 1;
            var flow_open_always = false;
            var flow_max_record = db.InstitutionsConfiguration.Where(i => i.InstitutionParentID == flucase.HospitalID).OrderByDescending(j => j.Priority).FirstOrDefault().Priority;

            var LabForeignCountry = db.InstitutionForeignConf.Where(z => z.InstitutionLocal.CountryID == user.Institution.CountryID).Any();
            var LabForeignInstitutionLocal = db.InstitutionForeignConf.Where(y => y.InstitutionLocalID == user.InstitutionID).Any();
            // Chequeo de muestas segun configuracion de virus por flujo actual
            // Revisar si existe alguna configuracion por virus en el flujo actual
            var list_by_virus_endflow_byActualFlow = db.InstitutionConfEndFlowByVirus.Where(y=> institucionActualAndPreviousFlow.Contains(y.id_InstCnf));


            if (user.Institution is Hospital  || user.Institution is AdminInstitution)
            {
                var list_institution_conf = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(i => i.InstitutionParentID == flucase.HospitalID && i.Conclusion == true).OrderBy(x=> x.Priority).Select(t => t.InstitutionToID).ToList();
                if (list_institution_conf.Any())
                {
                     canConclude = flucase.CaseLabTests.Where(y => list_institution_conf.Contains(y.LabID)).Any() && flucase.statement==2;
                }

                //// Chequeo de muestra de virus para terminar el flujo

                if (list_by_virus_endflow_byActualFlow.Any())
                {
                    var list_test_record = flucase.CaseLabTests.OfType<CaseLabTest>().OrderByDescending(d => d.CatTestType.orden)
                                                                                     .ThenBy(c => c.Institution.OrdenPrioritybyLab != null ? c.Institution.OrdenPrioritybyLab : 99)
                                                                                     .ThenBy(e => e.CatTestResult != null ? e.CatTestResult.orden : 99)
                                                                                     .ThenByDescending(f => f.CatVirusType != null ? f.CatVirusType.orden : 99)
                                                                                     .ThenByDescending(g => g.CatVirusSubType != null ? g.CatVirusSubType.orden : 99)
                                                                                     .ThenByDescending(h => h.CatVirusLinaje != null ? h.CatVirusLinaje.orden : 99);


                    if (list_test_record.Count() > 0 )
                    {
                        var Any_Test_EndFlow = false;

                        foreach (CaseLabTest test_Record in list_test_record)
                        {
                            Any_Test_EndFlow = test_Record.inst_conf_end_flow_by_virus > 0;
                            if (Any_Test_EndFlow == true) break;  // Esta linea lo agregue para cuando hay alguna 
                        }
                        canConclude = Any_Test_EndFlow && flucase.statement == 2;
                    }
                    else if (((flucase.NPHL_Processed == false || flucase.NPHL_Processed == null) && (flucase.NPHL_Processed_2 == false || flucase.NPHL_Processed_2 == null) && (flucase.NPHL_Processed_3 == false || flucase.NPHL_Processed_3 == null) && flucase.IsSample != true) || flucase.IsSample == false)
                    {
                        canConclude = true;
                    }
                    else if (((flucase.Processed == false || flucase.Processed == null) && (flucase.Processed2 == false || flucase.Processed2 == null) && (flucase.Processed3 == false || flucase.Processed3 == null) && flucase.IsSample != true) || flucase.IsSample == false )
                    {
                        if ( user_cty == 15)
                        {
                             if(  flucase.Processed_National == false)
                            {
                                canConclude = true;
                            }else
                            {
                                canConclude = false;
                            }

                        } else
                        {
                            canConclude = true;
                        }
                    }


                }

                if ((flucase.NPHL_Processed == false && (flucase.NPHL_Processed_2 == false || flucase.NPHL_Processed_2 == null) && (flucase.NPHL_Processed_3 == false || flucase.NPHL_Processed_3 == null)) && flucase.IsSample == true)
                {
                    canConclude = true;
                } else if ((flucase.Processed == false  && (flucase.Processed2 == false || flucase.Processed2 == null) && (flucase.Processed3 == false || flucase.Processed3 == null)) && flucase.IsSample == true)
                {
                    if (user_cty == 15)
                    {
                        if (flucase.Processed_National == false)
                        {
                            canConclude = true;
                        }
                        else
                        {
                            canConclude = false;
                        }

                    }
                    else
                    {
                        canConclude = true;
                    }
                }

               

                if (flucase.CaseLabTests.Count > 0)
                {
                    // Chequeo para activar la conclusión del caso si el flujo esta terminado o por lo menos activado
                    var list_test_record = flucase.CaseLabTests.OfType<CaseLabTest>().OrderBy(c => c.SampleNumber);
                    var Sample_1_test = list_test_record.Where(y => y.SampleNumber == 1).OrderBy(j => j.flow_test);
                    var flow_complete_Sample_1 = false;
                    var Sample_2_test = list_test_record.Where(y => y.SampleNumber == 2).OrderBy(j => j.flow_test);
                    var flow_complete_Sample_2 = (Sample_2_test.Count() > 0 ) ? false : (flucase.SampleDate2 != null && flucase.Processed2 != null) ? false : true;
                    var Sample_3_test = list_test_record.Where(y => y.SampleNumber == 3).OrderBy(j => j.flow_test);
                    var flow_complete_Sample_3 = (Sample_3_test.Count() > 0) ? false : (flucase.SampleDate3 != null && flucase.Processed3 != null) ? false : true;
                    
                    if (Sample_1_test.Count() == 0 && flucase.SampleDate != null) flow_complete_Sample_1 = true;
                    if (Sample_2_test.Count() == 0 && flucase.SampleDate2 != null) flow_complete_Sample_2 = true;
                    if (Sample_3_test.Count() == 0 && flucase.SampleDate3 != null) flow_complete_Sample_3 = true;

                    //foreach (EndFlowByVirus test_Record in response)

                    var actual_flow = 0;
                    var preview_flow = 0;
                    foreach (CaseLabTest Sample_test in Sample_1_test)
                    {
                        //if (Count_ == 1)
                        //{ actual_flow = preview_flow = (Int32)Sample_test.flow_test; }
                        //else
                        //{
                            preview_flow = actual_flow;
                            actual_flow = (Int32)Sample_test.flow_test;
                        //}

                        if (user_cty == 17 && preview_flow == 0) preview_flow = 1;
                        if (user_cty == 15 && preview_flow == 0 && flucase.Processed == false) preview_flow = 1;

                        if ((actual_flow - preview_flow) == 1 || (actual_flow - preview_flow) == 0)
                        {
                            flow_complete_Sample_1 = true;
                        }
                        else
                        {
                            flow_complete_Sample_1 = false;
                            break;
                        }
                    }
                    canConclude_Sample_1 = flow_complete_Sample_1;

                   actual_flow = 0;
                    preview_flow = 0;
                    foreach (CaseLabTest Sample_test in Sample_2_test)
                    {
                        //if (Count_ == 1)
                        //{ actual_flow = preview_flow = (Int32)Sample_test.flow_test; }
                        //else
                        //{
                        preview_flow = actual_flow;
                        actual_flow = (Int32)Sample_test.flow_test;
                        //}

                        if (user_cty == 17 && preview_flow == 0) preview_flow = 1;

                        if ((actual_flow - preview_flow) == 1 || (actual_flow - preview_flow) == 0)
                        {
                            flow_complete_Sample_2 = true;
                        }
                        else
                        {
                            flow_complete_Sample_2 = false;
                            break;
                        }
                    }
                    canConclude_Sample_2 = flow_complete_Sample_2;

                    actual_flow = 0;
                    preview_flow = 0;
                    foreach (CaseLabTest Sample_test in Sample_3_test)
                    {
                        //if (Count_ == 1)
                        //{ actual_flow = preview_flow = (Int32)Sample_test.flow_test; }
                        //else
                        //{
                        preview_flow = actual_flow;
                        actual_flow = (Int32)Sample_test.flow_test;
                        //}

                        if ((actual_flow - preview_flow) == 1 || (actual_flow - preview_flow) == 0)
                        {
                            flow_complete_Sample_3 = true;
                        }
                        else
                        {
                            flow_complete_Sample_3 = false;
                            break;
                        }
                    }
                    canConclude_Sample_3 = flow_complete_Sample_3;
                }

                if (flucase.IsSample == false)
                {
                    canConclude = true;
                    canConclude_Sample_1 = true;
                    canConclude_Sample_2 = true;
                    canConclude_Sample_3 = true;
                }
                if (flucase.IsSample == true)
                {
                    if (((flucase.SampleDate == null && flucase.Processed == null) || flucase.Processed == false) &&
                       ((flucase.SampleDate2 == null && flucase.Processed2 == null) || flucase.Processed2 == false) &&
                       ((flucase.SampleDate3 == null && flucase.Processed3 == null) || flucase.Processed3 == false))
                    {
                        canConclude_Sample_1 = true;
                        canConclude_Sample_2 = true;
                        canConclude_Sample_3 = true;
                    }
                    
                    if (user_cty == 15)
                    {
                        if (((flucase.SampleDate == null && flucase.Processed == null) || flucase.Processed == false) &&
                        ((flucase.SampleDate == null && flucase.Processed_National == null) || flucase.Processed_National == false) &&
                       ((flucase.SampleDate2 == null && flucase.Processed2 == null) || flucase.Processed2 == false) &&
                       ((flucase.SampleDate3 == null && flucase.Processed3 == null) || flucase.Processed3 == false))
                        {
                            canConclude_Sample_1 = true;
                            canConclude_Sample_2 = true;
                            canConclude_Sample_3 = true;
                        }

                        if ((flucase.Processed == false && flucase.Processed_National == true) || (flucase.Processed == true && flucase.Processed_National == false)  )
                        {
                            canConclude = true;
                        }
                    }
                    if (user_cty == 17)
                    {
                        if (flucase.NPHL_Processed == false)
                        {
                            canConclude_Sample_1 = true;
                            canConclude_Sample_2 = true;
                            canConclude_Sample_3 = true;
                        }

                        if (((flucase.SampleDate == null && flucase.Processed == null) || flucase.Processed == false) &&
                        ((flucase.SampleDate == null && flucase.NPHL_Processed == null) || flucase.NPHL_Processed == false) &&
                       ((flucase.SampleDate2 == null && flucase.Processed2 == null) || flucase.Processed2 == false) &&
                       ((flucase.SampleDate3 == null && flucase.Processed3 == null) || flucase.Processed3 == false))
                        {
                            canConclude_Sample_1 = true;
                            canConclude_Sample_2 = true;
                            canConclude_Sample_3 = true;
                        }
                    }

                }
                    

            }

            if (institutionsConfiguration.Any()) {

                if (list_by_virus_endflow_byActualFlow.Any())
                {
                    var list_test_record = flucase.CaseLabTests.OfType<CaseLabTest>().OrderBy(c => c.SampleNumber)
                                                                                     .ThenByDescending(d => d.CatTestType != null ? d.CatTestType.orden : 99)
                                                                                     .ThenBy(e => e.CatTestResult != null ? e.CatTestResult.orden : 99)
                                                                                     .ThenByDescending(f => f.CatVirusType != null ? f.CatVirusType.orden : 99);
                    //List<EndFlowByVirus> respuesta = ProcedureExecute<EndFlowByVirus>("EndFlowByVirus", "@RecordID", Id);
                    var Any_Test_EndFlow_1 = false;
                    var Any_Test_EndFlow_2 = false;
                    var Any_Test_EndFlow_3 = false;

                    foreach (CaseLabTest test_Record in list_test_record)
                    {
                        Any_Test_EndFlow_1 = test_Record.inst_conf_end_flow_by_virus > 0  && test_Record.SampleNumber == 1;
                        Any_Test_EndFlow_2 = test_Record.inst_conf_end_flow_by_virus > 0 && test_Record.SampleNumber == 2;
                        Any_Test_EndFlow_3 = test_Record.inst_conf_end_flow_by_virus > 0 && test_Record.SampleNumber == 3;
                    }

                    //SaveAndAdd_1 = !Any_Test_EndFlow_1 && flucase.statement == 2;
                    SaveAndAdd_1 = !Any_Test_EndFlow_1 && flucase.statement == 2;
                    SaveAndAdd_2 = !Any_Test_EndFlow_2 && flucase.statement == 2;
                    SaveAndAdd_3 = !Any_Test_EndFlow_3 && flucase.statement == 2;
                }


                canConclude = institutionsConfiguration.Count(x => x.Conclusion == true) > 0 && flucase.statement == 2;
                canConclude = canConclude && canConclude_Sample_1 && canConclude_Sample_2 && canConclude_Sample_3;  // Agregar 
                CanPCRLab = db.Institutions.OfType<Lab>().Where(i => i.ID == user.Institution.ID).First()?.PCR;
                CanIFILab = db.Institutions.OfType<Lab>().Where(i => i.ID == user.Institution.ID).First()?.IFI;
                flow_local_lab = institutionsConfiguration.First().Priority;
                flow_open_always = institutionsConfiguration.First().OpenAlways;
                
                //canConclude = institutionsConfiguration.Count(x => x.Conclusion == 1) > 0; // Original - modificado por AM
            }

            if (flucase != null)
            {
                return Json(new
                {
                    id = flucase.ID.ToString(),
                    RecDate = flucase.RecDate,
                    Identification_Test = flucase.Identification_Test,
                    Processed = flucase.Processed,
                    RecDate2 = flucase.RecDate2,
                    Identification_Test2 = flucase.Identification_Test2,
                    Processed2 = flucase.Processed2,
                    RecDate3 = flucase.RecDate3,
                    Identification_Test3 = flucase.Identification_Test3,
                    Processed3 = flucase.Processed3,
                    EndLabDate = flucase.EndLabDate,
                    FResult = flucase.FResult,
                    NoProRen = flucase.NoProRen,
                    NoProRenId = flucase.NoProRenId,
                    TempSample1 = flucase.TempSample1,

                    NoProRen2 = flucase.NoProRen2,
                    NoProRenId2 = flucase.NoProRenId2,
                    TempSample2 = flucase.TempSample2,

                    NoProRen3 = flucase.NoProRen3,
                    NoProRenId3 = flucase.NoProRenId3,
                    TempSample3 = flucase.TempSample3,

                    // Laboratorio Nacional solo para HN

                    RecDate_National = flucase.RecDate_National,
                    Processed_National = flucase.Processed_National,
                    Identification_Test_National = flucase.Identification_Test_National,
                    TempSample_National = flucase.TempSample_National,
                    NoProRenId_National = flucase.NoProRenId_National,
                    NoProRen_National = flucase.NoProRen_National,


                    // Laboratorio Intermedio
                    Rec_Date_NPHL = flucase.Rec_Date_NPHL,                    
                    Temp_NPHL = flucase.Temp_NPHL,
                    Identification_Test_NPHL = flucase.Identification_Test_NPHL,
                    Observation_NPHL = flucase.Observation_NPHL,                    
                    Ship_Date_NPHL = flucase.Ship_Date_NPHL,
                    NPHL_Processed = flucase.NPHL_Processed,
                    NPHL_NoProRenId = flucase.NPHL_NoProRenId,
                    NPHL_NoProRen = flucase.NPHL_NoProRen,

                    Rec_Date_NPHL_2 = flucase.Rec_Date_NPHL_2,
                    Temp_NPHL_2 = flucase.Temp_NPHL_2,
                    Identification_Test_NPHL_2 = flucase.Identification_Test_NPHL_2,
                    Observation_NPHL_2 = flucase.Observation_NPHL_2,
                    Ship_Date_NPHL_2 = flucase.Ship_Date_NPHL_2,
                    NPHL_Processed_2 = flucase.NPHL_Processed_2,
                    NPHL_NoProRenId_2 = flucase.NPHL_NoProRenId_2,
                    NPHL_NoProRen_2 = flucase.NPHL_NoProRen_2,

                    Rec_Date_NPHL_3 = flucase.Rec_Date_NPHL_3,
                    Temp_NPHL_3 = flucase.Temp_NPHL_3,
                    Identification_Test_NPHL_3 = flucase.Identification_Test_NPHL_3,
                    Observation_NPHL_3 = flucase.Observation_NPHL_3,
                    Ship_Date_NPHL_3 = flucase.Ship_Date_NPHL_3,
                    NPHL_Processed_3 = flucase.NPHL_Processed_3,
                    NPHL_NoProRenId_3 = flucase.NPHL_NoProRenId_3,
                    NPHL_NoProRen_3 = flucase.NPHL_NoProRen_3,

                    Comments = flucase.Comments,
                    FinalResult = flucase.FinalResult,
                    FinalResultVirusTypeID = flucase.FinalResultVirusTypeID,
                    FinalResultVirusSubTypeID = flucase.FinalResultVirusSubTypeID,
                    FinalResultVirusLineageID = flucase.FinalResultVirusLineageID,
                    FinalResult_2 = flucase.FinalResult_2,
                    FinalResultVirusTypeID_2 = flucase.FinalResultVirusTypeID_2,
                    FinalResultVirusSubTypeID_2 = flucase.FinalResultVirusSubTypeID_2,
                    FinalResultVirusLineageID_2 = flucase.FinalResultVirusLineageID_2,
                    FinalResult_3 = flucase.FinalResult_3,
                    FinalResultVirusTypeID_3 = flucase.FinalResultVirusTypeID_3,
                    FinalResultVirusSubTypeID_3 = flucase.FinalResultVirusSubTypeID_3,
                    FinalResultVirusLineageID_3 = flucase.FinalResultVirusLineageID_3,
                    GeneticGroup = flucase.GeneticGroup,            // Grupo genetico
                    GeneticGroup_2 = flucase.GeneticGroup_2,
                    GeneticGroup_3 = flucase.GeneticGroup_3,
                    flow_record = flucase.flow,
                    flow_institution = flow_local_lab,
                    flow_open_always = flow_open_always,
                    flow_max_record = flow_max_record,
                    DataStatement = flow_statement,
                    CanPCRLab = CanPCRLab,
                    CanIFILab = CanIFILab,
                    InstFlow_NPHL = user.Institution.NPHL != null ? (bool)user.Institution.NPHL : false,
                    ExistAnyInstitutionFlow_NPHL = user.Institution.CountryID != 15 ? db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(i => i.InstitutionParentID == flucase.HospitalID && i.InstitutionTo.NPHL == true).Any() : false,
                // Lab Foreign
                ForeignLabCountry = LabForeignCountry,
                    ForeignLabLocal = LabForeignInstitutionLocal,
                LabTests = (
                          from caselabtest in flucase.CaseLabTests
                          where caselabtest.SampleNumber == 1 || caselabtest.SampleNumber == null
                          select new
                          {
                              Id = caselabtest.ID,
                              CaseLabID = caselabtest.FluCaseID,
                              ProcLab = caselabtest.LabID.ToString(),
                              OrdenLabID = caselabtest.Institution.OrdenPrioritybyLab != null ? caselabtest.Institution.OrdenPrioritybyLab : 99,
                              //ProcLabName = GetLabName(caselabtest.LabID),
                              ProcLabName = caselabtest.Institution.Name,
                              ProcessLab = caselabtest.Processed,
                              SampleNumber = caselabtest.SampleNumber,
                              TestType = caselabtest.TestType,
                              TestDate = caselabtest.TestDate,
                              TestResultID = caselabtest.TestResultID,
                              VirusTypeID = caselabtest.VirusTypeID,
                              CTVirusType = caselabtest.CTVirusType,
                              CTRLVirusType = caselabtest.CTRLVirusType,
                              OtherVirusTypeID = caselabtest.OtherVirusTypeID,
                              CTOtherVirusType = caselabtest.CTOtherVirusType,
                              CTRLOtherVirusType = caselabtest.CTRLOtherVirusType,
                              OtherVirus = caselabtest.OtherVirus,
                              //InfA = caselabtest.InfA,
                              VirusSubTypeID = caselabtest.VirusSubTypeID,
                              CTSubType = caselabtest.CTSubType,
                              CTRLSubType = caselabtest.CTRLSubType,
                              TestResultID_VirusSubType = caselabtest.TestResultID_VirusSubType,
                              VirusSubTypeID_2 = caselabtest.VirusSubTypeID_2,
                              CTSubType_2 = caselabtest.CTSubType_2,
                              CTRLSubType_2 = caselabtest.CTRLSubType_2,
                              TestResultID_VirusSubType_2 = caselabtest.TestResultID_VirusSubType_2,

                              //InfB = caselabtest.InfB,
                              VirusLineageID = caselabtest.VirusLineageID,
                              CTLineage = caselabtest.CTLineage,
                              CTRLLineage = caselabtest.CTRLLineage,
                              //ParaInfI = caselabtest.ParaInfI,
                              //ParaInfII = caselabtest.ParaInfII,
                              //ParaInfIII = caselabtest.ParaInfIII,
                              //RSV = caselabtest.RSV,
                              //Adenovirus = caselabtest.Adenovirus,
                              //Metapneumovirus = caselabtest.Metapneumovirus,
                              RNP = caselabtest.RNP,
                              CTRLRNP = caselabtest.CTRLRNP,
                              CTRLNegative = caselabtest.CTRLNegative,
                              TestEndDate = caselabtest.TestEndDate,
                              //CanEdit = flucase.flow == 99 ? false : (((flucase.flow - 1) == flow_local_lab || flucase.flow == flow_local_lab) & flow_local_lab > 0  && flow_statement == 1) ? true : false, //institutionsIds.Contains(caselabtest.LabID),
                              CanEdit = User.IsInRole("Modify_Lab") ? true : institutionsIds.Contains(caselabtest.LabID),
                              //CanEdit = institutionsIds.Contains(caselabtest.LabID),
                              CanDeleteProcess = User.IsInRole("Modify_Lab") ? true : institutionsIds.Contains(caselabtest.LabID),
                              CanPCR = db.Institutions.OfType<Lab>().Where(i => i.ID == caselabtest.LabID).First()?.PCR,
                              CanIFI = db.Institutions.OfType<Lab>().Where(i => i.ID == caselabtest.LabID).First()?.IFI,
                              EndFlow = institutionActualFlow.Any() ? ((caselabtest.TestResultID == "N") ? db.InstitutionConfEndFlowByVirus.Where(j => j.id_InstCnf == institutionActualFlow.FirstOrDefault().ID && j.id_Cat_TestType == caselabtest.TestType && j.value_Cat_TestResult == caselabtest.TestResultID).Any().ToString().ToUpper() : db.InstitutionConfEndFlowByVirus.Where(j => j.id_InstCnf == institutionActualFlow.FirstOrDefault().ID && j.id_Cat_TestType == caselabtest.TestType && j.value_Cat_TestResult == caselabtest.TestResultID && j.id_Cat_VirusType == caselabtest.VirusTypeID).Any().ToString().ToUpper()) : "UNKNOWN",
                              // Orden de muestra
                              InstPriority = db.InstitutionsConfiguration.Where(i => i.InstitutionToID == caselabtest.LabID && i.InstitutionParentID == flucase.HospitalID).Any() ? db.InstitutionsConfiguration.Where(i=> i.InstitutionToID == caselabtest.LabID && i.InstitutionParentID == flucase.HospitalID).FirstOrDefault().ID : 0 ,
                              OrderTestType = caselabtest.CatTestType != null ?  caselabtest.CatTestType.orden : 99,
                              // Flujo de la muestra especifica
                              flow_test = caselabtest.flow_test
                          }
                      )
                      .OrderBy(x => x.InstPriority)
                      .ThenByDescending(d => d.OrderTestType)
                      .ThenBy(c=> c.TestDate)
                      .ToArray(),
                    LabTests_Sample2 = (
                          from caselabtest in flucase.CaseLabTests
                          where caselabtest.SampleNumber == 2
                          select new
                          {
                              Id = caselabtest.ID,
                              CaseLabID = caselabtest.FluCaseID,
                              ProcLab = caselabtest.LabID.ToString(),
                              ProcLabName = GetLabName(caselabtest.LabID),
                              ProcessLab = caselabtest.Processed,
                              SampleNumber = caselabtest.SampleNumber,
                              TestType = caselabtest.TestType,
                              TestDate = caselabtest.TestDate,
                              TestResultID = caselabtest.TestResultID,
                              VirusTypeID = caselabtest.VirusTypeID,
                              CTVirusType = caselabtest.CTVirusType,
                              CTRLVirusType = caselabtest.CTRLVirusType,
                              OtherVirusTypeID = caselabtest.OtherVirusTypeID,
                              CTOtherVirusType = caselabtest.CTOtherVirusType,
                              CTRLOtherVirusType = caselabtest.CTRLOtherVirusType,
                              OtherVirus = caselabtest.OtherVirus,
                              //InfA = caselabtest.InfA,
                              VirusSubTypeID = caselabtest.VirusSubTypeID,
                              CTSubType = caselabtest.CTSubType,
                              CTRLSubType = caselabtest.CTRLSubType,
                              //InfB = caselabtest.InfB,
                              VirusLineageID = caselabtest.VirusLineageID,
                              CTLineage = caselabtest.CTLineage,
                              CTRLLineage = caselabtest.CTRLLineage,
                              //ParaInfI = caselabtest.ParaInfI,
                              //ParaInfII = caselabtest.ParaInfII,
                              //ParaInfIII = caselabtest.ParaInfIII,
                              //RSV = caselabtest.RSV,
                              //Adenovirus = caselabtest.Adenovirus,
                              //Metapneumovirus = caselabtest.Metapneumovirus,
                              RNP = caselabtest.RNP,
                              CTRLRNP = caselabtest.CTRLRNP,
                              CTRLNegative = caselabtest.CTRLNegative,
                              TestEndDate = caselabtest.TestEndDate,
                              //CanEdit = institutionsIds.Contains(caselabtest.LabID),
                              CanEdit = User.IsInRole("Modify_Lab") ? true : institutionsIds.Contains(caselabtest.LabID),
                              CanModLab = User.IsInRole("Modify_Lab"),
                              CanPCR = db.Institutions.OfType<Lab>().Where(i => i.ID == caselabtest.LabID).First()?.PCR,
                              CanIFI = db.Institutions.OfType<Lab>().Where(i => i.ID == caselabtest.LabID).First()?.IFI,
                              EndFlow = institutionActualFlow.Any() ? ((caselabtest.TestResultID == "N") ? db.InstitutionConfEndFlowByVirus.Where(j => j.id_InstCnf == institutionActualFlow.FirstOrDefault().ID && j.id_Cat_TestType == caselabtest.TestType && j.value_Cat_TestResult == caselabtest.TestResultID).Any().ToString().ToUpper() : db.InstitutionConfEndFlowByVirus.Where(j => j.id_InstCnf == institutionActualFlow.FirstOrDefault().ID && j.id_Cat_TestType == caselabtest.TestType && j.value_Cat_TestResult == caselabtest.TestResultID && j.id_Cat_VirusType == caselabtest.VirusTypeID).Any().ToString().ToUpper()) : "UNKNOWN",
                              // Orden de muestra
                              InstPriority = db.InstitutionsConfiguration.Where(i => i.InstitutionToID == caselabtest.LabID && i.InstitutionParentID == flucase.HospitalID).Any() ? db.InstitutionsConfiguration.Where(i => i.InstitutionToID == caselabtest.LabID && i.InstitutionParentID == flucase.HospitalID).FirstOrDefault().ID : 0,
                              OrderTestType = caselabtest.CatTestType != null ? caselabtest.CatTestType.orden : 99,
                              // Flujo de la muestra especifica
                              flow_test = caselabtest.flow_test
                          }
                      )
                      .OrderBy(x => x.InstPriority)
                      .ThenByDescending(d => d.OrderTestType)
                      .ThenBy(c => c.TestDate)
                      .ToArray(),
                    LabTests_Sample3 = (
                          from caselabtest in flucase.CaseLabTests
                          where caselabtest.SampleNumber == 3
                          select new
                          {
                              Id = caselabtest.ID,
                              CaseLabID = caselabtest.FluCaseID,
                              ProcLab = caselabtest.LabID.ToString(),
                              ProcLabName = GetLabName(caselabtest.LabID),
                              ProcessLab = caselabtest.Processed,
                              SampleNumber = caselabtest.SampleNumber,
                              TestType = caselabtest.TestType,
                              TestDate = caselabtest.TestDate,
                              TestResultID = caselabtest.TestResultID,
                              VirusTypeID = caselabtest.VirusTypeID,
                              CTVirusType = caselabtest.CTVirusType,
                              CTRLVirusType = caselabtest.CTRLVirusType,
                              OtherVirusTypeID = caselabtest.OtherVirusTypeID,
                              CTOtherVirusType = caselabtest.CTOtherVirusType,
                              OtherVirus = caselabtest.OtherVirus,
                              //InfA = caselabtest.InfA,
                              VirusSubTypeID = caselabtest.VirusSubTypeID,
                              CTSubType = caselabtest.CTSubType,
                              CTRLSubType = caselabtest.CTRLSubType,
                              //InfB = caselabtest.InfB,
                              VirusLineageID = caselabtest.VirusLineageID,
                              CTLineage = caselabtest.CTLineage,
                              CTRLLineage = caselabtest.CTRLLineage,
                              //ParaInfI = caselabtest.ParaInfI,
                              //ParaInfII = caselabtest.ParaInfII,
                              //ParaInfIII = caselabtest.ParaInfIII,
                              //RSV = caselabtest.RSV,
                              //Adenovirus = caselabtest.Adenovirus,
                              //Metapneumovirus = caselabtest.Metapneumovirus,
                              RNP = caselabtest.RNP,
                              CTRLRNP = caselabtest.CTRLRNP,
                              CTRLNegative = caselabtest.CTRLNegative,
                              TestEndDate = caselabtest.TestEndDate,
                              //CanEdit = institutionsIds.Contains(caselabtest.LabID),
                              CanEdit = User.IsInRole("Modify_Lab") ? true : institutionsIds.Contains(caselabtest.LabID),
                              CanModLab = institutionsIds.Contains(caselabtest.LabID),
                              CanPCR = db.Institutions.OfType<Lab>().Where(i => i.ID == caselabtest.LabID).First()?.PCR,
                              CanIFI = db.Institutions.OfType<Lab>().Where(i => i.ID == caselabtest.LabID).First()?.IFI,
                              EndFlow = institutionActualFlow.Any() ? ((caselabtest.TestResultID == "N") ? db.InstitutionConfEndFlowByVirus.Where(j => j.id_InstCnf == institutionActualFlow.FirstOrDefault().ID && j.id_Cat_TestType == caselabtest.TestType && j.value_Cat_TestResult == caselabtest.TestResultID).Any().ToString().ToUpper() : db.InstitutionConfEndFlowByVirus.Where(j => j.id_InstCnf == institutionActualFlow.FirstOrDefault().ID && j.id_Cat_TestType == caselabtest.TestType && j.value_Cat_TestResult == caselabtest.TestResultID && j.id_Cat_VirusType == caselabtest.VirusTypeID).Any().ToString().ToUpper()) : "UNKNOWN",
                              // Orden de muestra
                              InstPriority = db.InstitutionsConfiguration.Where(i => i.InstitutionToID == caselabtest.LabID && i.InstitutionParentID == flucase.HospitalID).Any() ? db.InstitutionsConfiguration.Where(i => i.InstitutionToID == caselabtest.LabID && i.InstitutionParentID == flucase.HospitalID).FirstOrDefault().ID : 0,
                              OrderTestType = caselabtest.CatTestType != null ? caselabtest.CatTestType.orden : 99,
                              // Flujo de la muestra especifica
                              flow_test = caselabtest.flow_test
                          }
                      )
                      .OrderBy(x => x.InstPriority)
                      .ThenByDescending(d => d.OrderTestType)
                      .ThenBy(c => c.TestDate)
                      .ToArray(),
                    LabTests_Extern = (
                          from caselabtest in flucase.CaseLabTests
                          where caselabtest.SampleNumber == 999
                          select new
                          {
                              Id = caselabtest.ID,
                              CaseLabID = caselabtest.FluCaseID,
                              ProcLab = caselabtest.LabID.ToString(),
                              ProcLabName = GetLabName(caselabtest.LabID),
                              ProcessLab = caselabtest.Processed,
                              SampleNumber = caselabtest.SampleNumber,
                              TestType = caselabtest.TestType,
                              TestDate = caselabtest.TestDate,
                              TestResultID = caselabtest.TestResultID,
                              VirusTypeID = caselabtest.VirusTypeID,
                              CTVirusType = caselabtest.CTVirusType,
                              CTRLVirusType = caselabtest.CTRLVirusType,
                              OtherVirusTypeID = caselabtest.OtherVirusTypeID,
                              CTOtherVirusType = caselabtest.CTOtherVirusType,
                              OtherVirus = caselabtest.OtherVirus,
                              //InfA = caselabtest.InfA,
                              VirusSubTypeID = caselabtest.VirusSubTypeID,
                              CTSubType = caselabtest.CTSubType,
                              CTRLSubType = caselabtest.CTRLSubType,
                              //InfB = caselabtest.InfB,
                              VirusLineageID = caselabtest.VirusLineageID,
                              CTLineage = caselabtest.CTLineage,
                              CTRLLineage = caselabtest.CTRLLineage,
                              //ParaInfI = caselabtest.ParaInfI,
                              //ParaInfII = caselabtest.ParaInfII,
                              //ParaInfIII = caselabtest.ParaInfIII,
                              //RSV = caselabtest.RSV,
                              //Adenovirus = caselabtest.Adenovirus,
                              //Metapneumovirus = caselabtest.Metapneumovirus,
                              RNP = caselabtest.RNP,
                              CTRLRNP = caselabtest.CTRLRNP,
                              CTRLNegative = caselabtest.CTRLNegative,
                              TestEndDate = caselabtest.TestEndDate,
                              CanEdit = institutionsIds.Contains(caselabtest.LabID),
                              CanModLab = User.IsInRole("Modify_Lab"),
                              CanPCR = db.Institutions.OfType<Lab>().Where(i => i.ID == caselabtest.LabID).First()?.PCR,
                              CanIFI = db.Institutions.OfType<Lab>().Where(i => i.ID == caselabtest.LabID).First()?.IFI,
                              EndFlow = institutionActualFlow.Any() ? ((caselabtest.TestResultID == "N") ? db.InstitutionConfEndFlowByVirus.Where(j => j.id_InstCnf == institutionActualFlow.FirstOrDefault().ID && j.id_Cat_TestType == caselabtest.TestType && j.value_Cat_TestResult == caselabtest.TestResultID).Any().ToString().ToUpper() : db.InstitutionConfEndFlowByVirus.Where(j => j.id_InstCnf == institutionActualFlow.FirstOrDefault().ID && j.id_Cat_TestType == caselabtest.TestType && j.value_Cat_TestResult == caselabtest.TestResultID && j.id_Cat_VirusType == caselabtest.VirusTypeID).Any().ToString().ToUpper()) : "UNKNOWN",
                              // Orden de muestra
                              InstPriority = db.InstitutionsConfiguration.Where(i => i.InstitutionToID == caselabtest.LabID && i.InstitutionParentID == flucase.HospitalID).Any() ? db.InstitutionsConfiguration.Where(i => i.InstitutionToID == caselabtest.LabID && i.InstitutionParentID == flucase.HospitalID).FirstOrDefault().ID : 0,
                              OrderTestType = caselabtest.CatTestType != null ? caselabtest.CatTestType.orden : 99
                          }
                      )
                      .OrderBy(x => x.InstPriority)
                      .ThenByDescending(d => d.OrderTestType)
                      .ThenBy(c => c.TestDate)
                      .ToArray(),
                    LabsResult = institutions.Select(x => new { Id = x.ID.ToString(), x.Name }).ToList(),
                    SubTypeByLabRes = GetSubTypebyLab(user.InstitutionID),
                    CanConclude = canConclude & canConclude_Sample_1 & canConclude_Sample_2 & canConclude_Sample_3,
                    SaveAndAdd_1 = SaveAndAdd_1,
                    SaveAndAdd_2 = SaveAndAdd_2,
                    SaveAndAdd_3 = SaveAndAdd_3

                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    id = "",
                    RecDate = new DateTime?(),
                    Processed = "",
                    EndLabDate = new DateTime?(),
                    FResult = "",
                    NoProRen ="",
                    Comments = "",
                    LabTests = ""
                },
                     JsonRequestBehavior.AllowGet);
            }
        }

        public string GetLabName(long? id)
        {
            if (id == null) return "";
            var labName = db.Institutions.OfType<Lab>().Where(i => i.ID == id).First()?.Name;
            return labName;
        }

        public object GetSubTypebyLab(long? id)
        {
            IQueryable<CatVirusSubType> CVST_Test = null;
            var user = UserManager.FindById(User.Identity.GetUserId());

            var CatVirusSubtypeConfiguration = db.CatVirusSubTypeConfByLab.OfType<CatVirusSubTypeConfByLab>()
        .Where(i => i.id_Institution == id);

            if (!CatVirusSubtypeConfiguration.Any())
            {
                CVST_Test = db.CatVirusSubType.OrderBy(i => i.orden);
            }
            else
            {
                var ListVaccines = CatVirusSubtypeConfiguration.Select(i => i.id_CatSubType).ToList();
                CVST_Test = db.CatVirusSubType.Where(k => ListVaccines.Contains(k.ID)).OrderBy(i => i.orden);
            }

            return (user.Institution.Country.Language == "SPA" ?  CVST_Test.Select(x => new { Id = x.ID.ToString(), Name = x.SPA }).ToList() : CVST_Test.Select(x => new { Id = x.ID.ToString(), Name = x.ENG }).ToList());
        }

        // POST: Cases/SaveGEO/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult SaveLab(
                int id,
                DateTime? RecDate,
                string Identification_Test,
                bool? Processed,
                DateTime? RecDate2,
                string Identification_Test2,
                bool? Processed2,
                DateTime? RecDate3,
                string Identification_Test3,
                bool? Processed3,
                DateTime? EndLabDate,
                string FResult,
                int? NoProRenId,
                string NoProRen,
                Decimal? TempSample1,
                int? NoProRenId2,
                string NoProRen2,
                Decimal? TempSample2,
                int? NoProRenId3,
                string NoProRen3,
                Decimal? TempSample3,
                string Comments,
                string FinalResult,
                int? FinalResultVirusTypeID,
                int? FinalResultVirusSubTypeID,
                int? FinalResultVirusLineageID,
                string FinalResult_2,
                int? FinalResultVirusTypeID_2,
                int? FinalResultVirusSubTypeID_2,
                int? FinalResultVirusLineageID_2,
                string FinalResult_3,
                int? FinalResultVirusTypeID_3,
                int? FinalResultVirusSubTypeID_3,
                int? FinalResultVirusLineageID_3,
                string GeneticGroup,
                string GeneticGroup_2,
                string GeneticGroup_3,
                int? DataStatement,
                // Lab nacional solo para HN
                DateTime? RecDate_National,
                Decimal? TempSample_National,
                string Identification_Test_National,
                bool? Processed_National,
                int? NoProRenId_National,
                string NoProRen_National,
                //AM Laboratorio intermedio
                DateTime? Rec_Date_NPHL,
                Decimal? Temp_NPHL,
                string Identification_Test_NPHL,
                string Observation_NPHL,
                DateTime? Ship_Date_NPHL,
                bool? NPHL_Processed,
                int? NPHL_NoProRenId,
                string NPHL_NoProRen,
                DateTime? Rec_Date_NPHL_2,
                Decimal? Temp_NPHL_2,
                string Identification_Test_NPHL_2,
                string Observation_NPHL_2,
                DateTime? Ship_Date_NPHL_2,
                bool? NPHL_Processed_2,
                int? NPHL_NoProRenId_2,
                string NPHL_NoProRen_2,
                DateTime? Rec_Date_NPHL_3,
                Decimal? Temp_NPHL_3,
                string Identification_Test_NPHL_3,
                string Observation_NPHL_3,
                DateTime? Ship_Date_NPHL_3,
                bool? NPHL_Processed_3,
                int? NPHL_NoProRenId_3,
                string NPHL_NoProRen_3,
                List<LabTestViewModel> LabTests
            )
        {
            FluCase flucase;
            var IFI_RecordHistory = false;
            var PCR_RecordHistory = false;
            var IFI_Count = 0;
            var PCR_Count = 0;
            var user = UserManager.FindById(User.Identity.GetUserId());
            

            flucase = db.FluCases.Find(id);

            if (flucase == null) {
                flucase = new FluCase();
                db.Entry(flucase).State = EntityState.Added;
            } else {
                db.Entry(flucase).State = EntityState.Modified;
            }

            bool ifclosecase = flucase.flow == 99;
            IFI_Count = flucase.CaseLabTests.Where(e => e.FluCaseID == id && e.TestType == 1 && e.Processed != null).Count();
            PCR_Count = flucase.CaseLabTests.Where(e => e.FluCaseID == id && e.TestType == 2 && e.Processed != null).Count();

            flucase.RecDate = RecDate;
            flucase.Identification_Test = Identification_Test;
            flucase.Processed = Processed;
            flucase.RecDate2 = RecDate2;
            flucase.Identification_Test2 = Identification_Test2;
            flucase.Processed2 = Processed2;
            flucase.RecDate3 = RecDate3;
            flucase.Identification_Test3 = Identification_Test3;
            flucase.Processed3 = Processed3;
            flucase.EndLabDate = EndLabDate;
            flucase.FResult = FResult;
            flucase.NoProRen = NoProRen;

            flucase.NoProRenId = NoProRenId;
            flucase.TempSample1 = TempSample1;

            flucase.NoProRen2 = NoProRen2;
            flucase.NoProRenId2 = NoProRenId2;
            flucase.TempSample2 = TempSample2;

            flucase.NoProRen3 = NoProRen3;
            flucase.NoProRenId3 = NoProRenId3;
            flucase.TempSample3 = TempSample3;

            // Laboratorio Nacional, solo para HN
            flucase.RecDate_National = RecDate_National;
            flucase.Processed_National = Processed_National;
            flucase.TempSample_National = TempSample_National;
            flucase.Identification_Test_National = Identification_Test_National;
            flucase.NoProRen_National = NoProRen_National;
            flucase.NoProRenId_National = NoProRenId_National;


            // Laboratorio de intermedio
            flucase.Rec_Date_NPHL = Rec_Date_NPHL;
            flucase.Identification_Test_NPHL = Identification_Test_NPHL;
            flucase.Temp_NPHL = Temp_NPHL;
            flucase.Observation_NPHL = Observation_NPHL;
            flucase.Ship_Date_NPHL = Ship_Date_NPHL;
            flucase.NPHL_Processed = NPHL_Processed;
            flucase.NPHL_NoProRenId = NPHL_NoProRenId;
            flucase.NPHL_NoProRen = NPHL_NoProRen;

            flucase.Rec_Date_NPHL_2 = Rec_Date_NPHL_2;
            flucase.Temp_NPHL_2 = Temp_NPHL_2;
            flucase.Identification_Test_NPHL_2 = Identification_Test_NPHL_2;
            flucase.Observation_NPHL_2 = Observation_NPHL_2;
            flucase.Ship_Date_NPHL_2 = Ship_Date_NPHL_2;
            flucase.NPHL_Processed_2 = NPHL_Processed_2;
            flucase.NPHL_NoProRenId_2 = NPHL_NoProRenId_2;
            flucase.NPHL_NoProRen_2 = NPHL_NoProRen_2;

            flucase.Rec_Date_NPHL_3 = Rec_Date_NPHL_3;
            flucase.Temp_NPHL_3 = Temp_NPHL_3;
            flucase.Identification_Test_NPHL_3 = Identification_Test_NPHL_3;
            flucase.Observation_NPHL_3 = Observation_NPHL_3;
            flucase.Ship_Date_NPHL_3 = Ship_Date_NPHL_3;
            flucase.NPHL_Processed_3 = NPHL_Processed_3;
            flucase.NPHL_NoProRenId_3 = NPHL_NoProRenId_3;
            flucase.NPHL_NoProRen_3 = NPHL_NoProRen_3;

            flucase.Comments = Comments;
            flucase.FinalResult = FinalResult;
            flucase.FinalResultVirusTypeID = FinalResultVirusTypeID;
            flucase.FinalResultVirusSubTypeID = FinalResultVirusSubTypeID;
            flucase.FinalResultVirusLineageID = FinalResultVirusLineageID;
            flucase.FinalResult_2 = FinalResult_2;
            flucase.FinalResultVirusTypeID_2 = FinalResultVirusTypeID_2;
            flucase.FinalResultVirusSubTypeID_2 = FinalResultVirusSubTypeID_2;
            flucase.FinalResultVirusLineageID_2 = FinalResultVirusLineageID_2;
            flucase.FinalResult_3 = FinalResult_3;
            flucase.FinalResultVirusTypeID_3 = FinalResultVirusTypeID_3;
            flucase.FinalResultVirusSubTypeID_3 = FinalResultVirusSubTypeID_3;
            flucase.FinalResultVirusLineageID_3 = FinalResultVirusLineageID_3;

            flucase.GeneticGroup = GeneticGroup;
            flucase.GeneticGroup_2 = GeneticGroup_2;
            flucase.GeneticGroup_3 = GeneticGroup_3;

            //flucase.flow = 2;
            //if (db.CaseLabTests.Count() > 0) PCR_IFI_RecordHistory = true;

            db.CaseLabTests.RemoveRange(flucase.CaseLabTests);
            var existrecordlabtest = false;
            flucase.CaseLabTests = new List<CaseLabTest>();
            
            if (LabTests != null) {
                foreach (LabTestViewModel labTestViewModel in LabTests.OrderBy(x=>x.SampleNumber)
                    //.ThenBy(x => x.InstPriority)
                      .ThenBy(z => z.TestDate)
                      //.ThenBy(d => d.CatTestType != null ? d.CatTestType.orden : null)
                      .ThenBy(y=>y.LabID)) {

                    if (labTestViewModel.LabID == user.InstitutionID)
                        existrecordlabtest = true;
                    if (user.Institution.CountryID == 15 && flucase.Processed == false && user.Institution.LabNIC != true)
                        existrecordlabtest = true;
                    if (user.Institution.CountryID == 15 && flucase.Processed_National == false && user.Institution.LabNIC == true)
                        existrecordlabtest = true;
                    if (labTestViewModel.TestType == 1) IFI_RecordHistory = true;
                    if (labTestViewModel.TestType == 2) PCR_RecordHistory = true;
                    // Campos para controlar el flujo de la muestra
                    var Flow_Test = db.InstitutionsConfiguration.Where(i => i.InstitutionToID == labTestViewModel.LabID && i.InstitutionParentID == flucase.HospitalID).Any() ? db.InstitutionsConfiguration.Where(i => i.InstitutionToID == labTestViewModel.LabID && i.InstitutionParentID == flucase.HospitalID).FirstOrDefault().Priority : 0;
                    var Statement_Test = (flucase.flow != 99) ? DataStatement : flucase.statement;
                    var Flow_Flucase = flucase.flow;
                    var Statement_Flucase = (flucase.flow != 99) ?  DataStatement : flucase.statement;
                    var Institution_Conf_Original = db.InstitutionsConfiguration.Where(i => i.InstitutionToID == labTestViewModel.LabID && i.InstitutionParentID == flucase.HospitalID).Any() ? db.InstitutionsConfiguration.Where(i => i.InstitutionToID == labTestViewModel.LabID && i.InstitutionParentID == flucase.HospitalID).FirstOrDefault().ID : 0;
                    //var Inst_Conf_End_Flow_By_Virus = labTestViewModel.TestResultID == "P"?  db.InstitutionConfEndFlowByVirus.Where(j => j.id_Lab == labTestViewModel.LabID && j.id_Cat_TestType == labTestViewModel.TestType && j.value_Cat_TestResult == labTestViewModel.TestResultID && j.id_Cat_VirusType == labTestViewModel.VirusTypeID ).Any() ? db.InstitutionConfEndFlowByVirus.Where(j => j.id_Lab == labTestViewModel.LabID && j.id_Cat_TestType == labTestViewModel.TestType && j.value_Cat_TestResult == labTestViewModel.TestResultID && j.id_Cat_VirusType == labTestViewModel.VirusTypeID).FirstOrDefault().ID : 0 : db.InstitutionConfEndFlowByVirus.Where(j => j.id_Lab == labTestViewModel.LabID && j.id_Cat_TestType == labTestViewModel.TestType && j.value_Cat_TestResult == labTestViewModel.TestResultID ).Any() ? db.InstitutionConfEndFlowByVirus.Where(j => j.id_Lab == labTestViewModel.LabID && j.id_Cat_TestType == labTestViewModel.TestType && j.value_Cat_TestResult == labTestViewModel.TestResultID).FirstOrDefault().ID : 0;
                    var Inst_Conf_End_Flow_By_Virus = labTestViewModel.TestResultID == "P" ? 
                                                          db.InstitutionConfEndFlowByVirus.Where(j => j.id_Lab == labTestViewModel.LabID && j.id_Cat_TestType == labTestViewModel.TestType && j.value_Cat_TestResult == labTestViewModel.TestResultID && j.id_Cat_VirusType == labTestViewModel.VirusTypeID).Any()  //Positivo -- Flujo por virus
                                                        ? db.InstitutionConfEndFlowByVirus.Where(j => j.id_Lab == labTestViewModel.LabID && j.id_Cat_TestType == labTestViewModel.TestType && j.value_Cat_TestResult == labTestViewModel.TestResultID && j.id_Cat_VirusType == labTestViewModel.VirusTypeID).FirstOrDefault().ID 
                                                        : 0 : 
                                                        db.InstitutionConfEndFlowByVirus.Where(j => j.id_Lab == labTestViewModel.LabID && j.id_InstCnf == Institution_Conf_Original && j.id_Cat_TestType == labTestViewModel.TestType && j.value_Cat_TestResult == labTestViewModel.TestResultID).Any() ? // Negativo
                                                        db.InstitutionConfEndFlowByVirus.Where(j => j.id_Lab == labTestViewModel.LabID && j.id_InstCnf == Institution_Conf_Original && j.id_Cat_TestType == labTestViewModel.TestType && j.value_Cat_TestResult == labTestViewModel.TestResultID).FirstOrDefault().ID :
                                                        db.InstitutionConfEndFlowByVirus.Where(j => j.id_Lab == labTestViewModel.LabID && j.id_InstCnf == Institution_Conf_Original && j.id_Cat_TestType == labTestViewModel.TestType && j.value_Cat_TestResult == labTestViewModel.TestResultID).Any() ? 
                                                        db.InstitutionConfEndFlowByVirus.Where(j => j.id_Lab == labTestViewModel.LabID && j.id_InstCnf == Institution_Conf_Original && j.id_Cat_TestType == labTestViewModel.TestType && j.value_Cat_TestResult == labTestViewModel.TestResultID).FirstOrDefault().ID : 0;

                    flucase.CaseLabTests.Add(
                        new CaseLabTest {

                            LabID = labTestViewModel.LabID,
                            Processed = labTestViewModel.Processed,
                            SampleNumber = labTestViewModel.SampleNumber,
                            TestDate = labTestViewModel.TestDate,
                            TestResultID = labTestViewModel.TestResultID,
                            TestEndDate = labTestViewModel.TestEndDate,
                            TestType = labTestViewModel.TestType,
                            VirusTypeID = labTestViewModel.VirusTypeID,
                            CTVirusType = labTestViewModel.CTVirusType,
                            CTRLVirusType = labTestViewModel.CTRLVirusType,
                            OtherVirusTypeID = labTestViewModel.OtherVirusTypeID,
                            CTOtherVirusType = labTestViewModel.CTOtherVirusType,
                            CTRLOtherVirusType = labTestViewModel.CTRLOtherVirusType,
                            OtherVirus = labTestViewModel.OtherVirus,
                            //InfA = labTestViewModel.InfA,
                            VirusSubTypeID = labTestViewModel.VirusSubTypeID,
                            CTSubType = labTestViewModel.CTSubType,
                            CTRLSubType = labTestViewModel.CTRLSubType,
                            TestResultID_VirusSubType = labTestViewModel.TestResultID_VirusSubType,
                            VirusSubTypeID_2 = labTestViewModel.VirusSubTypeID_2,
                            CTSubType_2 = labTestViewModel.CTSubType_2,
                            CTRLSubType_2 = labTestViewModel.CTRLSubType_2,
                            TestResultID_VirusSubType_2 = labTestViewModel.TestResultID_VirusSubType_2,
                            VirusLineageID = labTestViewModel.VirusLineageID,
                            CTLineage = labTestViewModel.CTLineage,
                            CTRLLineage = labTestViewModel.CTRLLineage,
                            RNP = labTestViewModel.RNP,
                            CTRLRNP = labTestViewModel.CTRLRNP,
                            CTRLNegative = labTestViewModel.CTRLNegative,
                            // Parte del flujo

                            flow_test = Flow_Test,
                            statement_test = Statement_Test,
                            flow_flucase = Flow_Flucase,
                            statement_flucase = Statement_Flucase,
                            inst_cnf_orig = Institution_Conf_Original,
                            inst_conf_end_flow_by_virus = Inst_Conf_End_Flow_By_Virus,
 
                            FluCaseID = flucase.ID
                        }
                    );
                }
            } else if (user.Institution.CountryID != 15)
            {
                if (((flucase.SampleDate == null && flucase.Processed == null) || flucase.Processed == false) &&
                       ((flucase.SampleDate2 == null && flucase.Processed2 == null) || flucase.Processed2 == false) &&
                       ((flucase.SampleDate3 == null && flucase.Processed3 == null) || flucase.Processed3 == false))
                {
                    existrecordlabtest = true;
                }
            }
              else if (user.Institution.CountryID == 15)
            {
                if (((flucase.SampleDate == null && flucase.Processed == null && flucase.Processed_National == null) || (flucase.Processed == false && (flucase.Processed_National == false || flucase.Processed_National == null))) &&
                       ((flucase.SampleDate2 == null && flucase.Processed2 == null) || flucase.Processed2 == false) &&
                       ((flucase.SampleDate3 == null && flucase.Processed3 == null) || flucase.Processed3 == false))
                {
                    existrecordlabtest = true;
                }
            }

            //var Sample_1_process = LabTests.OrderBy(z => z.TestDate).ThenBy(y => y.LabID).Where(x => x.SampleNumber == 1);
            var flow_max_record = db.InstitutionsConfiguration.Where(z => z.InstitutionParentID == flucase.HospitalID).OrderByDescending(x => x.Priority).FirstOrDefault().Priority;
            var flow_min_record = db.InstitutionsConfiguration.Where(z => z.InstitutionParentID == flucase.HospitalID).OrderBy(x => x.Priority).FirstOrDefault().Priority;

            var Sample_1_process = flucase.CaseLabTests.Where(x => x.SampleNumber == 1).OrderBy(y => y.flow_test).ThenBy(z => z.TestDate);
            var flow_complete_Sample_1 = (Sample_1_process.Count() > 0) ? false : (flucase.SampleDate != null && flucase.Processed != null) ? (flucase.Processed == false) ? true : false : true;

            var Sample_2_process = flucase.CaseLabTests.Where(x => x.SampleNumber == 2).OrderBy(y => y.flow_test);
            var flow_complete_Sample_2 = (Sample_2_process.Count() > 0) ? false : (flucase.SampleDate2 != null && flucase.Processed2 != null) ? (flucase.Processed2 == false) ? true : false  : true;

            var Sample_3_process = flucase.CaseLabTests.Where(x => x.SampleNumber == 3).OrderBy(y => y.flow_test);
            var flow_complete_Sample_3 = (Sample_3_process.Count() > 0) ? false : (flucase.SampleDate3 != null && flucase.Processed3 != null) ? (flucase.Processed3 == false) ? true : false : true;

            var any_lab_nphl_in_flow = db.InstitutionsConfiguration.Where(z => z.InstitutionParentID == flucase.HospitalID && z.InstitutionTo.NPHL == true && z.InstitutionParent.CountryID != 25).Any();

            var actual_flow_Sample_1 =  0;
            var preview_flow_sample_1 = 0;
            var count = 0;
            foreach (CaseLabTest Sample_test in Sample_1_process)
            {
                preview_flow_sample_1 = (any_lab_nphl_in_flow == true && count < 1) ? actual_flow_Sample_1 + 1 :  (user.Institution.CountryID == 15 && flucase.Processed == false) ? actual_flow_Sample_1 + 1 :  actual_flow_Sample_1 ;
                actual_flow_Sample_1 = (Int32)Sample_test.flow_test;

                if ((actual_flow_Sample_1 - preview_flow_sample_1) == 1 || (actual_flow_Sample_1 - preview_flow_sample_1) == 0)
                {
                    flow_complete_Sample_1 = true;
                }
                else
                {
                    flow_complete_Sample_1 = false;
                    break;
                }
                count = +1;

            }

            var actual_flow_Sample_2 = 0;
            var preview_flow_sample_2 = 0;
             count = 0;
            foreach (CaseLabTest Sample_test in Sample_2_process)
            {
                preview_flow_sample_2 = (any_lab_nphl_in_flow == true && count < 1) ? actual_flow_Sample_2 + 1 : actual_flow_Sample_2;
                actual_flow_Sample_2 = (Int32)Sample_test.flow_test;

                if ((actual_flow_Sample_2 - preview_flow_sample_2) == 1 || (actual_flow_Sample_2 - preview_flow_sample_2) == 0)
                {
                    flow_complete_Sample_2 = true;
                }
                else
                {
                    flow_complete_Sample_2 = false;
                    break;
                }
                count = +1;
            }

            var actual_flow_Sample_3 = 0;
            var preview_flow_sample_3 = 0;
            count = 0;
            foreach (CaseLabTest Sample_test in Sample_3_process)
            {
                preview_flow_sample_3 = (any_lab_nphl_in_flow == true && count < 1) ? actual_flow_Sample_3 + 1 : actual_flow_Sample_3;
                actual_flow_Sample_3 = (Int32)Sample_test.flow_test;

                if ((actual_flow_Sample_3 - preview_flow_sample_3) == 1 || (actual_flow_Sample_3 - preview_flow_sample_3) == 0)
                {
                    flow_complete_Sample_3 = true;
                }
                else
                {
                    flow_complete_Sample_3 = false;
                    break;
                }
                count = +1;
            }

            // Revisión si los datos están completos

            var data_sample = false;
            var data_sample_1 = false;
            var data_sample_2 = false;
            var data_sample_3 = false;

            if (flucase.SampleDate == null && flucase.SampleDate2 == null && flucase.SampleDate3 == null)
                data_sample = true;
            else
                data_sample = false;

            if (flucase.SampleDate != null)
            {
                if  (user.Institution.NPHL == true  && user.Institution.CountryID != 25) {
                    if (flucase.NPHL_Processed != null)
                    {
                        data_sample_1 = true;
                    }
                    else
                    {
                        data_sample_1 = false;
                    }
                }
                else
                {
                    if (flucase.Processed != null)
                    {
                        data_sample_1 = true;
                    }
                    else
                    {
                        data_sample_1 = false;
                    }
                }
            }
            else
            {
                data_sample_1 = true;
            }

            if (flucase.SampleDate2 != null)
            {
                if (user.Institution.NPHL == true && user.Institution.CountryID != 25)
                {
                    if (flucase.NPHL_Processed_2 != null)
                    {
                        data_sample_2 = true;
                    }
                    else
                    {
                        data_sample_2 = false;
                    }
                }
                else
                {
                    if (flucase.Processed2 != null)
                    {
                        data_sample_2 = true;
                    }
                    else
                    {
                        data_sample_2 = false;
                    }
                }
            }
            else
            {
                data_sample_2 = true;
            }

            if (flucase.SampleDate3 != null)
            {
                if (user.Institution.NPHL == true && user.Institution.CountryID != 25)
                {
                    if (flucase.NPHL_Processed_3 != null)
                    {
                        data_sample_3 = true;
                    }
                    else
                    {
                        data_sample_3 = false;
                    }
                }
                else
                {
                    if (flucase.Processed3 != null)
                    {
                        data_sample_3 = true;
                    }
                    else
                    {
                        data_sample_3 = false;
                    }
                }
            }
            else
            {
                data_sample_3 = true;
            }

            data_sample = data_sample_1 & data_sample_2 & data_sample_3;


            if ((user.Institution is Lab && existrecordlabtest == true) || (user.Institution.NPHL == true && user.Institution.CountryID != 25))
            {
                var institutionsConfiguration = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(i => i.InstitutionToID == user.Institution.ID && i.InstitutionParentID == flucase.HospitalID);
                var flow_temp = institutionsConfiguration.First().Priority;
                var flow_original_flucase = flucase.flow;

                if (institutionsConfiguration.Any())
                {

                    //if ((flow_temp > flucase.flow) || flucase.flow == null)
                    //{
                    //    if (institutionsConfiguration.First().OpenAlways == true && (flow_temp - 1) > flucase.flow)
                    //    {
                    //        flucase.flow = flow_original_flucase;
                    //    }
                    //    else
                    //    {
                    //        flucase.flow = flow_temp;
                    //    }
                    //}  
                    if (ifclosecase == true)
                    {
                        flucase.flow = flow_original_flucase;
                    }
                    else if (flow_complete_Sample_1 == true && flow_complete_Sample_2 == true && flow_complete_Sample_3 == true)
                    {
                        if (user.Institution.CountryID == 15)
                        {
                            if (Processed == false && Processed_National == null && LabTests == null)
                            {
                                flucase.flow = flow_temp ;
                            } else
                            {
                                flucase.flow = flow_temp + ((Processed_National == false  && flow_temp < 2) ? 1 : (flow_temp < flow_max_record && Processed_National == true) ? flow_max_record - flow_temp : 0);
                            }

                            
                        }else
                        {
                            flucase.flow = flow_temp;
                        }
                        
                    }
                    else if (flow_complete_Sample_1 == true && flow_complete_Sample_2 == false && flow_complete_Sample_3 == true)
                    {
                        flucase.flow = actual_flow_Sample_2;
                    }
                    else if (flow_complete_Sample_1 == false && flow_complete_Sample_2 == false && flow_complete_Sample_3 == true)
                    {
                        if (preview_flow_sample_1 > preview_flow_sample_2)
                        { flucase.flow = preview_flow_sample_2; }
                        else { flucase.flow = preview_flow_sample_1; }
                    }
                    else if (flow_complete_Sample_1 == false && flow_complete_Sample_2 == true && flow_complete_Sample_3 == true)
                    {
                        flucase.flow = preview_flow_sample_1;
                    }
                    else
                    {
                        flucase.flow = flow_original_flucase;
                    }

                 
                }
                else
                {
                    flucase.flow = 1;
                }
                if (flucase.flow != 99)
                    if (flucase.flow == 0)
                    {
                        flucase.statement = 2;
                    }
                    else if (flow_complete_Sample_1 == true && flow_complete_Sample_2 == true && flow_complete_Sample_3 == true && data_sample == true)
                    {
                        flucase.statement = DataStatement;
                    }
                    else if (institutionsConfiguration.First().OpenAlways == true && (flow_temp - 1) == flow_original_flucase)
                    {
                        flucase.statement = DataStatement;
                    }
                    else if (data_sample == false)
                    {
                        flucase.statement = 1;
                    }
                    else if ( flow_complete_Sample_1 == true || flow_complete_Sample_2 == true || flow_complete_Sample_3 == true)
                    {
                        flucase.statement = flucase.statement;
                    }

                    
            }

            flucase.InsertDate = DateTime.Now;
            //flucase.UserID = User.Identity.Name;

            var result = SaveChanges();

            var tests = flucase.CaseLabTests.ToList();
            if (tests.Count <= 0)
            {
                if (Processed == false || Processed_National == false)
                {
                    HistoryRecord(flucase.ID, 4, flucase.flow, 6);
                    HistoryRecord(flucase.ID, 4, flucase.flow, 11);
                    HistoryRecord(flucase.ID, 4, flucase.flow, 13);
                }
                return result;
            }

            if (tests.Count >= 1) flucase.MuestraID1 = tests[0].ID;
            if (tests.Count >= 2) flucase.MuestraID2 = tests[1].ID;
            if (tests.Count >= 3) flucase.MuestraID3 = tests[2].ID;
            if (tests.Count >= 4) flucase.MuestraID4 = tests[3].ID;
            if (tests.Count >= 5) flucase.MuestraID5 = tests[4].ID;
            if (tests.Count >= 6) flucase.MuestraID6 = tests[5].ID;

            if (tests.Count >= 7) flucase.MuestraID7 = tests[6].ID;
            if (tests.Count >= 8) flucase.MuestraID8 = tests[7].ID;
            if (tests.Count >= 9) flucase.MuestraID9 = tests[8].ID;
            if (tests.Count >= 10) flucase.MuestraID10 = tests[9].ID;
            if (tests.Count >= 11) flucase.MuestraID11 = tests[10].ID;
            if (tests.Count >= 12) flucase.MuestraID12 = tests[11].ID;
            if (tests.Count >= 13) flucase.MuestraID13 = tests[12].ID;
            if (tests.Count >= 14) flucase.MuestraID14 = tests[13].ID;
            if (tests.Count >= 15) flucase.MuestraID15 = tests[14].ID;

            result = SaveChanges();

            //Bitacora
            if (IFI_Count == 0 && PCR_Count == 0)
                HistoryRecord(flucase.ID, 4, flucase.flow, 1);

            if ((IFI_Count > 0 || PCR_Count > 0) && ifclosecase == false)
                HistoryRecord(flucase.ID, 4, flucase.flow, 5);

            if ((IFI_Count > 0 || PCR_Count > 0) && ifclosecase == true)
                HistoryRecord(flucase.ID, 4, flucase.flow, 11);

            if (IFI_RecordHistory && IFI_Count == 0)
               HistoryRecord(flucase.ID, 4, flucase.flow, 7);

            if (PCR_RecordHistory && PCR_Count == 0)
                HistoryRecord(flucase.ID, 4, flucase.flow, 8);

            return result;
        }
        public JsonResult GetRecordHistory(int caseId)
        {
            List<Dictionary<string, string>> logsPerUser = new List<Dictionary<string, string>>();

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var user = UserManager.FindById(User.Identity.GetUserId());
            var user_lang = user.Institution.Country.Language.ToString() ?? "SPA";

            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("B1", con) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Record_ID", SqlDbType.Int).Value = caseId;
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = user.Institution.CountryID;
                    command.Parameters.Add("@Languaje", SqlDbType.NVarChar).Value = user_lang;
                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int caseNumber = 0;
                            int operationNumber = 0;
                            string actionDesc = "";
                            string userOperator = "";
                            string healthUnit = "";
                            string operationDate = "";
                            //x = reader.GetValue(1);
                            Dictionary<string, string> userRecord = new Dictionary<string, string>();
                            userRecord.Add("caseNumber", reader["caseNumber"].ToString());
                            userRecord.Add("operationNumber", reader["operationNumber"].ToString());
                            userRecord.Add("actionDesc", reader["actionDesc"].ToString());
                            userRecord.Add("userOperator", reader["userOperator"].ToString());
                            userRecord.Add("healthUnit", reader["healthUnit"].ToString());
                            //userRecord.Add("stateRecord", (user_lang == "ENG") ? reader["stateRecord_ENG"].ToString() : reader["stateRecord"].ToString());
                            userRecord.Add("operationDate", reader["operationDate"].ToString());
                            logsPerUser.Add(userRecord);

                        }
                    }
                }
                return Json(logsPerUser, JsonRequestBehavior.AllowGet);
            }
        }

        //public List<T> ProcedureExecute<T>(string NameProcedure, string Parameter1, int ValuePar1) where T : new()
        //{
        //    List<T> res = new List<T>();

        //    var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //    using (var con = new SqlConnection(consString))
        //    {
        //        SqlCommand command = new SqlCommand(NameProcedure, con);
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.Parameters.Clear();
        //        command.Parameters.Add(Parameter1, SqlDbType.Int).Value = ValuePar1;
        //        con.Open();
        //        using (var reader = command.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                T t = new T();

        //                for (int inc = 0; inc < reader.FieldCount; inc++)
        //                {
        //                    Type type = t.GetType();
        //                    System.Reflection.PropertyInfo prop = type.GetProperty(reader.GetName(inc));
        //                    prop.SetValue(t, reader.GetValue(inc) == DBNull.Value ? null : reader.GetValue(inc), null);
        //                }

        //                res.Add(t);
        //            }
        //        }
        //        con.Close();
        //    }

        //    return res;

        //}

        [Authorize]
        public JsonResult GetPatientInformation(int DTP, string DNP)
        {
            var dataforpadron = false;
            var user = UserManager.FindById(User.Identity.GetUserId());
            List<Dictionary<string, string>> PatientInformation_ = new List<Dictionary<string, string>>();
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                if (user.Institution.CountryID == 9) {
                    var number_ = (int)(Int64) Convert.ToDouble(DNP);
                    using (var command = new SqlCommand("PatientInformationCR", con) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@DocumentNumberP", SqlDbType.BigInt).Value = number_;
                        command.Parameters.Add("@DocumentTypeP", SqlDbType.Int).Value = DTP;
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //x = reader.GetValue(1);
                                Dictionary<string, string> PatientInformation = new Dictionary<string, string>();
                                var nombre = reader["NOM_PERSONA"].ToString().Split(' ');
                                PatientInformation.Add("nombre1", nombre[0].ToString());
                                PatientInformation.Add("nombre2", nombre.Length > 1 ? nombre[1].ToString():"");
                                PatientInformation.Add("apellido1", reader["NOM_APELLIDO1"].ToString());
                                PatientInformation.Add("apellido2", reader["NOM_APELLIDO2"].ToString());
                                PatientInformation.Add("sexo", reader["IND_SEXO"].ToString() == "M" ? "Male" : reader["IND_SEXO"].ToString() == "F" ? "Female" : "Unknown");
                                PatientInformation.Add("DOB", Convert.ToDateTime(reader["FEC_NACIMIENTO"]).ToString("yyyy-MM-dd"));
                                //PatientInformation.Add("value", reader["Val_Padron"].ToString( ));
                                PatientInformation_.Add(PatientInformation);
                                dataforpadron = true;

                            }
                        }
                        con.Close();
                    }
                }

                if (dataforpadron == false)
                {
                    using (var command = new SqlCommand("PatientInformation", con) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@DocumentNumberP", SqlDbType.NVarChar).Value = DNP;
                        command.Parameters.Add("@DocumentTypeP", SqlDbType.Int).Value = DTP;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = user.InstitutionID;
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //x = reader.GetValue(1);
                                Dictionary<string, string> PatientInformation = new Dictionary<string, string>();
                                PatientInformation.Add("nombre1", reader["FName1"].ToString());
                                PatientInformation.Add("nombre2", reader["FName2"].ToString());
                                PatientInformation.Add("apellido1", reader["LName1"].ToString());
                                PatientInformation.Add("apellido2", reader["LName2"].ToString());
                                PatientInformation.Add("sexo", reader["Gender"].ToString() == "1" ? "Male" : reader["Gender"].ToString() == "2" ? "Female" : "Unknown");
                                //PatientInformation.Add("DOB", JsonConvert.SerializeObject(reader["DOB"], new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat }));
                                PatientInformation.Add("DOB", Convert.ToDateTime(reader["DOB"]).ToString("yyyy-MM-dd"));
                                PatientInformation_.Add(PatientInformation);
                                dataforpadron = true;

                            }
                        }
                        con.Close();
                    }
                }

                return Json(PatientInformation_, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult GetPatientInformation_JAM(DateTime? DOB, DateTime? DateOnSetFever, DateTime? SampleDate)
        {

            var user = UserManager.FindById(User.Identity.GetUserId());
            var userRegion = user.Institution.cod_region_institucional;
            var DateToDay = DateTime.Now;
            var Date14DaysDiff = DateToDay.AddDays(-20);
            List<Dictionary<string, string>> PatientInformation_ = new List<Dictionary<string, string>>();


            IQueryable<FluCase> flucases = null;

            flucases = flucases.Where(x => x.DOB == DOB && x.FeverDate == DateOnSetFever && x.SampleDate == SampleDate && x.RegDate >= Date14DaysDiff && x.Hospital.cod_region_institucional == userRegion);


            var jsondata = new
            {
                rows = (
                    from flucase in flucases
                    select new
                    {
                        id = flucase.ID.ToString(),
                        cell = new string[]
                      { 
                         flucase.ID.ToString(),
                         flucase.HospitalDate.ToString((user.Institution.CountryID==17) ? "yyyy/MM/dd": "dd/MM/yyyy" ),
                         flucase.LName1 ??  "",
                         flucase.LName2  ?? "",
                         flucase.FName1 ?? "",
                         flucase.FName2 ?? "",
                         flucase.RegDate.ToString((user.Institution.CountryID==17) ? "yyyy/MM/dd": "dd/MM/yyyy" ),
                         //flucase.FeverDate.ToString((user.Institution.CountryID==17) ? "yyyy/MM/dd": "dd/MM/yyyy" ),
                      }
                    }).ToArray()
            };


            return Json(jsondata, JsonRequestBehavior.AllowGet);
           
        }

        [Authorize]
        public ActionResult GetSearchUbicaResid(string term, int max, string code)
        {
            //System.Diagnostics.Debug.WriteLine("ActionResult GetCIE10X->START");
            var user = UserManager.FindById(User.Identity.GetUserId());
            int countryId = (int)user.Institution.CountryID;
            string language = user.Institution.Area.Country.Language;
            int? numberAdmiDiv = user.Institution.Area.Country.NumberAdminisDivision;
            numberAdmiDiv = numberAdmiDiv ?? 0;
            var search = term;

            string areaAcron = SgetMsg("msgCaselistVigTabGeoAreaAcronym", countryId, language);
            string statAcron = SgetMsg("msgCaselistVigTabGeoStateAcronym", countryId, language);
            string neigAcron = SgetMsg("msgCaselistVigTabGeoNeighborhoodsAcronym", countryId, language);
            string hamlAcron = SgetMsg("msgCaselistVigTabGeoHamletAcronym", countryId, language);

            if (numberAdmiDiv == 2)
            {
                //*/**** AREA
                var diagsArea = (from area in db.Areas //as IQueryable<Area>
                                 where area.CountryID == countryId && area.Name.Contains(search)
                                 select new
                                 {
                                     value = area.ID,
                                     label = area.Name,
                                     typeubic = "AR",
                                     areaID = area.ID,
                                     areaName = area.Name,
                                     stateID = 0,
                                     stateName = "",
                                     neighborhoodID = 0,
                                     neighborhoodName = "",
                                     hamletID = 0,
                                     hamletName = "",
                                     colonyID = 0,
                                     colonyName = ""
                                 }).AsEnumerable();

                var jsonDataArea = diagsArea.ToArray();

                //*/**** STATE
                var diagsState = (from area in db.Areas
                                  join state in db.States on area.ID equals state.AreaID
                                  where state.Name.Contains(search) && area.CountryID == countryId
                                  select new
                                  {
                                      value = state.ID,
                                      label = state.Name + " (" + areaAcron + ": " + area.Name + ")",
                                      typeubic = "ST",
                                      areaID = area.ID,
                                      areaName = area.Name,
                                      stateID = state.ID,
                                      stateName = state.Name,
                                      neighborhoodID = 0,
                                      neighborhoodName = "",
                                      hamletID = 0,
                                      hamletName = "",
                                      colonyID = 0,
                                      colonyName = ""
                                  }).AsEnumerable();

                var jsonDataState = diagsState.ToArray();
                //****
                var jsonDataUnido = jsonDataArea.Concat(jsonDataState);

                var sortedValues = from x in jsonDataUnido
                                   orderby x.label
                                   select x;

                return Json(sortedValues, JsonRequestBehavior.AllowGet);
            }
            else if (numberAdmiDiv == 3)
            {
                //*/**** STATE
                var diagsState = (from area in db.Areas
                                  join state in db.States on area.ID equals state.AreaID
                                  where state.Name.Contains(search) && area.CountryID == countryId
                                  select new
                                  {
                                      value = state.ID,
                                      label = state.Name + " (" + areaAcron + ": " + area.Name + ")",
                                      typeubic = "ST",
                                      areaID = area.ID,
                                      areaName = area.Name,
                                      stateID = state.ID,
                                      stateName = state.Name,
                                      neighborhoodID = 0,
                                      neighborhoodName = "",
                                      hamletID = 0,
                                      hamletName = "",
                                      colonyID = 0,
                                      colonyName = ""
                                  }).AsEnumerable();

                var jsonDataState = diagsState.ToArray();

                //*/**** Neighborhood
                var diagsNeighborhood = (from area in db.Areas
                                         join state in db.States on area.ID equals state.AreaID
                                         join neighborhood in db.Neighborhoods on state.ID equals neighborhood.StateID
                                         where neighborhood.Name.Contains(search) && area.CountryID == countryId
                                         select new
                                         {
                                             value = neighborhood.ID,
                                             label = neighborhood.Name + " (" + statAcron + ": " + state.Name + " / " + areaAcron + ": " + area.Name + ")",
                                             typeubic = "NE",
                                             areaID = area.ID,
                                             areaName = area.Name,
                                             stateID = state.ID,
                                             stateName = state.Name,
                                             neighborhoodID = neighborhood.ID,
                                             neighborhoodName = neighborhood.Name,
                                             hamletID = 0,
                                             hamletName = "",
                                             colonyID = 0,
                                             colonyName = ""
                                         }).AsEnumerable();

                var jsonDataNeighborhood = diagsNeighborhood.ToArray();
                //****
                var jsonDataUnido = jsonDataState.Concat(jsonDataNeighborhood);

                var sortedValues = from x in jsonDataUnido
                                   orderby x.label
                                   select x;

                return Json(sortedValues, JsonRequestBehavior.AllowGet);
            }
            else if (numberAdmiDiv == 5)
            {
                //**** Hamlet
                var diagsHamlet = (from area in db.Areas
                                   join state in db.States on area.ID equals state.AreaID
                                   join neighborhood in db.Neighborhoods on state.ID equals neighborhood.StateID
                                   join hamlet in db.Hamlets on neighborhood.ID equals hamlet.NeighborhoodID
                                   where hamlet.Name.Contains(search) && area.CountryID == countryId
                                   select new
                                   {
                                       value = hamlet.ID,
                                       //label = hamlet.Name + " (" + "AL" + ": " + getMsg("msgCaselistVigTabGeoNeighborhoodsAcronym") +  neighborhood.Name + " / MU: " + state.Name + " / DE: " + area.Name + ")",
                                       label = hamlet.Name + " (" + neigAcron + ": " + neighborhood.Name + " / " + statAcron + ": " + state.Name + " / " + areaAcron + ": " + area.Name + ")",
                                       typeubic = "HA",
                                       areaID = area.ID,
                                       areaName = area.Name,
                                       stateID = state.ID,
                                       stateName = state.Name,
                                       neighborhoodID = neighborhood.ID,
                                       neighborhoodName = neighborhood.Name,
                                       hamletID = hamlet.ID,
                                       hamletName = hamlet.Name,
                                       colonyID = 0,
                                       colonyName = ""
                                   }).AsEnumerable();

                var jsonDataHamlet = diagsHamlet.ToArray();

                //**** Colony
                var diagsColony = (from area in db.Areas
                                   join state in db.States on area.ID equals state.AreaID
                                   join neighborhood in db.Neighborhoods on state.ID equals neighborhood.StateID
                                   join hamlet in db.Hamlets on neighborhood.ID equals hamlet.NeighborhoodID
                                   join colony in db.Colonies on hamlet.ID equals colony.HamletID
                                   where colony.Name.Contains(search) && area.CountryID == countryId
                                   select new
                                   {
                                       value = colony.ID,
                                       label = colony.Name + " (" + hamlAcron + ": " + hamlet.Name + " / " + neigAcron + ": " + neighborhood.Name + " / " + statAcron + ": " + state.Name + " / " + areaAcron + ": " + area.Name + ")",
                                       typeubic = "CO",
                                       areaID = area.ID,
                                       areaName = area.Name,
                                       stateID = state.ID,
                                       stateName = state.Name,
                                       neighborhoodID = neighborhood.ID,
                                       neighborhoodName = neighborhood.Name,
                                       hamletID = hamlet.ID,
                                       hamletName = hamlet.Name,
                                       colonyID = colony.ID,
                                       colonyName = colony.Name
                                   }).AsEnumerable();

                var jsonDataColony = diagsColony.ToArray();
                //****
                var jsonDataUnido = jsonDataHamlet.Concat(jsonDataColony);
                
                var sortedValues = from x in jsonDataUnido
                                   orderby x.label
                                   select x;

                return Json(sortedValues, JsonRequestBehavior.AllowGet);
            }
            //****
            return Json(null, JsonRequestBehavior.AllowGet);
            //****
        }

        private static string SgetMsg(string msgView, int? countryDisp, string langDisp)
        {
            string searchedMsg = ResourcesM.SgetMessage(msgView, countryDisp, langDisp);
            //searchedMsg = myR.getMessage(searchedMsg, 0, "ENG");
            return searchedMsg;
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

        private void IQueryableToXML(int? RecordId)
        {
            try
            {
                IQueryable<FluCase> flucases = null;

                var xEle = new XElement("Record",
                            from flucase in flucases
                                 select new XElement("flucase",
                                         new XAttribute("flucaseID", flucase.ID),
                                           new XElement("HospitalID", flucase.HospitalID),
                                           new XElement("HospitalID_CaseGenerating", flucase.HospitalID_CaseGenerating),
                                           new XElement("HospitalDate", flucase.HospitalDate),
                                           new XElement("RegDate", flucase.RegDate)
                                       ));
                
                xEle.Save("D:\\flucase.xml");
                //return xEle;
                Console.WriteLine("Converted to XML");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }
    }
}
