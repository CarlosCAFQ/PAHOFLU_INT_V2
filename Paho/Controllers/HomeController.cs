using Paho.Models;
using System;
//using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Globalization;
using System.Collections.Generic;

namespace Paho.Controllers
{
    [Authorize]
    public class HomeController : ControllerBase
    {        
        public ActionResult Index()
        {
            var CaseViewModel = new CaseViewModel();
            IQueryable<Institution> institutions = null;
            IQueryable<Institution> InstitutionsCaseGenerating = null;
            IQueryable<Region> regions = null;
            IQueryable<Area> areas = null;

            IQueryable<CatReasonNotSampling> catreasonnotsampling = null;
            IQueryable<CatSampleNoProcessed> CSNP = null;
            IQueryable<CatTestType> CTT = null;
            IQueryable<CatTestResult> CTR = null;
            IQueryable<CatVirusType> CVT = null;
            IQueryable<CatOtherVirusType> COVT = null;
            IQueryable<CatVirusSubType> CVST = null;
            IQueryable<CatVirusSubType> CVST_Test = null;
            IQueryable<CatVirusLinaje> CVL = null;

            IQueryable<CatNativePeople> CNP = null;
            IQueryable<CatVaccinSource> CVS = null;

            IQueryable<CatOccupation> occupations = null;                       //**** CAFQ
            IQueryable<CatTrabSaludRama> trabsaludrama = null;                  //**** CAFQ
            IQueryable<CatTrabLaboRama> trablaborama = null;                    //**** CAFQ
            // Cat Hospitalizado en: para HN
            IQueryable<CatHospitalizedIn> CHIn = null;

            var user = UserManager.FindById(User.Identity.GetUserId());
            //var DoS = ( user.Institution.CountryID == 17) ? DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("es-GT")) : DateTime.Now.ToString("yy/mm/dd") ;
            var DoS = DateTime.Now.ToString(getMsg("msgDatePickerConfig"));
            var date_format = getMsg("msgDateFormatDP");

            //CaseViewModel.UsrCtry = user.Institution.CountryID;
            CaseViewModel.DatePickerConfig = DoS;
            CaseViewModel.DateFormatDP = date_format;
            var institutionsConfiguration = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>()
                    .Where(i => i.InstitutionToID == user.Institution.ID);

            if (user.Institution.AccessLevel == AccessLevel.All)
            {   
                CaseViewModel.DisplayCountries = true;
                CaseViewModel.DisplayAreas = true;
                CaseViewModel.DisplayRegionals = true;
                CaseViewModel.DisplayHospitals = true;
            }
            else if (user.Institution is Hospital || user.Institution is AdminInstitution)
            {
                CaseViewModel.DisplayHospitals = true;

                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID);

                    if (user.type_region == null)
                    {     // Regiones
                        regions = db.Regions.Where(c => c.CountryID == user.Institution.CountryID && c.tipo_region == 1).OrderBy(i=> i.Name);
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
                    //CaseViewModel.DisplayAreas = true;

                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.AreaID == user.Institution.AreaID).OrderBy(n => n.FullName);
                }
                else if (user.Institution is Hospital && user.Institution.AccessLevel != AccessLevel.Service)//línea agregada de la versión en inglés. Según AM, sirve para los servicios
                {

                    institutions = db.Institutions.OfType<Hospital>()
                                .Where(i => i.Father_ID == user.InstitutionID || i.ID == user.InstitutionID).OrderBy(n => n.FullName);
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
                                   .Where(i => i.ID == user.Institution.ID).OrderBy(j => j.FullName);
                }
            }
            else
            {
                CaseViewModel.DisplayLabs = true;

                
                institutions = institutionsConfiguration.Select(i => i.InstitutionParent).OrderBy(j => j.FullName);

                if (!institutions.Any())
                {
                    if (user.Institution.AccessLevel == AccessLevel.Country)
                    {
                        institutions = db.Institutions.OfType<Lab>()
                                       .Where(i => i.CountryID == user.Institution.CountryID).OrderBy(j => j.FullName);
                    }
                    else if (user.Institution.AccessLevel == AccessLevel.Area)
                    {
                        institutions = db.Institutions.OfType<Lab>()
                                       .Where(i => i.AreaID == user.Institution.AreaID).OrderBy(j => j.FullName);
                    }
                    else
                    {
                        institutions = db.Institutions.OfType<Lab>()
                                       .Where(i => i.ID == user.Institution.ID).OrderBy(j => j.FullName);
                    }
                }
            }

            if (regions == null && user.Institution.CountryID == 11)        // Dominica
            {
                regions = db.Regions.Where(c => c.CountryID == user.Institution.CountryID && c.tipo_region == 1).OrderBy(i => i.Name);
            }

            //**** Paises
            CaseViewModel.Countries = db.Countries
                    .Select(c => new CountryView()
                    {
                        Id = c.ID.ToString(),
                        Name = (user.Institution.Country.Language == "SPA") ? c.Name : c.ENG,
                        Active = c.Active
                    })
                    .OrderBy(d => d.Name) 
                    .ToArray();

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
                    CaseViewModel.DisplayAreas = true;
                }

                CaseViewModel.Areas = areasDisplay;
                //CaseViewModel.Regions = regions;
            }

            //**** Regiones
            if ( regions != null )
            {
                var regionsDisplay = regions.Select(i => new LookupView<Region>()
                {
                    Id = i.orig_country.ToString(),
                    Name = i.Name
                })
                .OrderBy( a => a.Name).ToList();

                if (regionsDisplay.Count() > 1)
                {
                    //var all = new LookupView<Region> { Id = "0", Name = "-- Todo(a)s --" };
                    //var all = new LookupView<Region> { Id = "0", Name = getMsg("msgGeneralMessageAll") };
                    var all = new LookupView<Region> { Id = "0", Name = getMsg("msgSelectLabel") };         //#### CAFQ: 181008
                    regionsDisplay.Insert(0, all);
                    CaseViewModel.DisplayRegionals = true;
                }

                CaseViewModel.Regions = regionsDisplay;
                //CaseViewModel.Regions = regions;
            }

            //**** Hospitales
            if (institutions != null) {
                var institutionsDisplay = institutions.Select(i => new LookupView<Institution>() {
                    Id = i.ID.ToString(),
                    Name = i.Name
                })
                .OrderBy( a => a.Name).ToList();

                if (institutionsDisplay.Count() > 1) {
                    //var all = new LookupView<Institution> { Id = "0", Name = "-- Todo(a)s --" };
                    var all = new LookupView<Institution> { Id = "0", Name = getMsg("msgGeneralMessageAll") };
                    institutionsDisplay.Insert(0, all);
                    
                }

                CaseViewModel.Institutions = institutionsDisplay;
            }

            // Health institution Case Generating 
            InstitutionsCaseGenerating = db.Institutions.OfType<Hospital>().Where(i => i.CountryID == user.Institution.CountryID);
             var institutionsCaseGeneratingDisplay = InstitutionsCaseGenerating.Select(i => new LookupView<Institution>() {
                    Id = i.ID.ToString(),
                    Name = i.Name
                })
                .OrderBy ( a => a.Name).ToList();
            if (institutionsCaseGeneratingDisplay.Count() > 1)
            {
                var all = new LookupView<Institution> { Id = "", Name = getMsg("msgSelectLabel") };        
                institutionsCaseGeneratingDisplay.Insert(0, all);
            }

            CaseViewModel.InstitutionsCaseGenerarting = institutionsCaseGeneratingDisplay;

            //**** Informacion del usuario
            var region_institucional_usr = user.Institution.cod_region_institucional;
            var region_salud_usr = user.Institution.cod_region_salud;
            var region_pais_usr = user.Institution.cod_region_pais;

            if (region_institucional_usr > 0)
                CaseViewModel.reg_inst_usr = db.Regions.Where(j => j.CountryID == user.Institution.CountryID && j.tipo_region == 1 && j.orig_country == user.Institution.cod_region_institucional).FirstOrDefault().Name.ToString();

            if (region_salud_usr > 0)
                CaseViewModel.reg_salu_usr = db.Regions.Where(j => j.CountryID == user.Institution.CountryID && j.tipo_region == 2 && j.orig_country == user.Institution.cod_region_salud).FirstOrDefault().Name.ToString();

            if (region_pais_usr > 0)
                CaseViewModel.reg_pais_usr = db.Regions.Where(j => j.CountryID == user.Institution.CountryID && j.tipo_region == 3 && j.orig_country == user.Institution.cod_region_pais).FirstOrDefault().Name.ToString();

            //**** Usuario de laboratorio NIC
            CaseViewModel.lab_NIC_usr = user.Institution.LabNIC;

            //**** Motivos de no toma de muestra
            //CaseViewModel.ReasonNotSampling = catreasonnotsampling.Select(c => new LookupView<CatReasonNotSampling>()
            //                                                                    {
            //                                                                        Id = c.ID.ToString(),
            //                                                                        Name = (user.Institution.Country.Language == "SPA") ? c.SPA : c.ENG,
            //                                                                    })
            //                                                                    .OrderBy(d => d.Name)
            //                                                                    .ToArray();

            //catreasonnotsampling = db.Areas.Where(d => d.CountryID == user.Institution.CountryID).OrderBy(i => i.Name);
            catreasonnotsampling = db.CatReasonNotSampling;

            var ReasonNotSamplingDisplay = catreasonnotsampling.Select(i => new LookupView<CatReasonNotSampling>()
            {
                Id = i.ID.ToString(),
                Name = (user.Institution.Country.Language == "SPA") ? i.SPA : i.ENG,
                orden = ""
            }).ToList();

            CaseViewModel.ReasonNotSampling = ReasonNotSamplingDisplay;

            // Catalogos del laboratorio
            CSNP = db.CatSampleNoProcessed.OrderBy(i => i.orden).ThenBy(j => j.SPA);
            var CSNPDisplay = (user.Institution.Country.Language == "SPA" ? CSNP.Select(i => new LookupView<CatSampleNoProcessed>()
            {
                Id = i.ID.ToString(),
                Name = i.SPA,
                orden = i.orden.ToString()
            }).ToList() : CSNP.Select(i => new LookupView<CatSampleNoProcessed>()
            {
                Id = i.ID.ToString(),
                Name = i.ENG,
                orden = i.orden.ToString()
            }).ToList());

            CTT = db.CatTestType.OrderBy(i => i.orden);
            var CTTDisplay = CTT.Select(i => new LookupView<CatTestType>()
            {
                Id = i.ID.ToString(),
                Name = i.description,
                orden = i.orden.ToString()
            }).ToList();

            CTR = db.CatTestResult.OrderBy(i => i.orden);
            var CTRDisplay = (user.Institution.Country.Language == "SPA" ? CTR.Select(j => new LookupView<CatTestResult>()
            {
                Id = j.value.ToString().Trim(),
                Name = j.description.ToString().Trim(),
                orden = j.orden.ToString()
            }).ToList() : CTR.Select(j => new LookupView<CatTestResult>()
            {
                Id = j.value.ToString().Trim(),
                Name = j.ENG.ToString().Trim(),
                orden = j.orden.ToString()
            }).ToList());

            CVT = db.CatVirusType.OrderBy(i => i.orden);
            var CVTDisplay = ( user.Institution.Country.Language ==  "SPA" ? CVT.Select(i => new LookupView<CatVirusType>()
            {
                Id = i.ID.ToString(),
                Name = i.SPA,
                orden = i.orden.ToString()
            }).ToList() : CVT.Select(i => new LookupView<CatVirusType>()
            {
                Id = i.ID.ToString(),
                Name = i.ENG,
                orden = i.orden.ToString()
            }).ToList()) ;

            COVT = db.CatOtherVirusType.OrderBy(i => i.orden);
            var COVTDisplay = (user.Institution.Country.Language == "SPA" ? COVT.Select(i => new LookupView<CatOtherVirusType>()
            {
                Id = i.ID.ToString(),
                Name = i.SPA,
                orden = i.orden.ToString()
            }).ToList() : COVT.Select(i => new LookupView<CatOtherVirusType>()
            {
                Id = i.ID.ToString(),
                Name = i.ENG,
                orden = i.orden.ToString()
            }).ToList());

            CHIn = db.CatHospitalizedIn.OrderBy(i => i.orden);
            var CHInDisplay = (user.Institution.Country.Language == "SPA" ? CHIn.Select(i => new LookupView<CatHospitalizedIn>()
            {
                Id = i.ID.ToString(),
                Name = i.SPA,
                orden = i.orden.ToString()
            }).ToList() : CHIn.Select(i => new LookupView<CatHospitalizedIn>()
            {
                Id = i.ID.ToString(),
                Name = i.ENG,
                orden = i.orden.ToString()
            }).ToList());

            occupations = db.CatOccupations.OrderBy(i => i.Occupation_SPA);         //**** CAFQ
            var occupationsDisplay = (user.Institution.Country.Language == "SPA" ? occupations.Select(i => new LookupView<CatOccupation>()
            {
                Id = i.Id.ToString(),
                Name = i.Occupation_SPA,
                orden = i.CIUO_08.ToString()
            }).ToList() : occupations.Select(i => new LookupView<CatOccupation>()
            {
                Id = i.Id.ToString(),
                Name = i.Occupation_ENG,
                orden = i.CIUO_08.ToString()
            }).ToList());

            trabsaludrama = db.CatTrabSaludRamas.OrderBy(i => i.Rama_SPA);                 //#### CAFQ
            var trabsaludramaDisplay = (user.Institution.Country.Language == "SPA" ? trabsaludrama.Select(i => new LookupView<CatTrabSaludRama>()
            {
                Id = i.Id.ToString(),
                Name = i.Rama_SPA,
                orden = "1"
            }).ToList() : trabsaludrama.Select(i => new LookupView<CatTrabSaludRama>()
            {
                Id = i.Id.ToString(),
                Name = i.Rama_ENG,
                orden = "1"
            }).ToList());

            trablaborama = db.CatTrabLaboRamas.OrderBy(i => i.Rama_SPA);                 //#### CAFQ
            var trablaboramaDisplay = (user.Institution.Country.Language == "SPA" ? trablaborama.Select(i => new LookupView<CatTrabLaboRama>()
            {
                Id = i.Id.ToString(),
                Name = i.Rama_SPA,
                orden = "1"
            }).ToList() : trablaborama.Select(i => new LookupView<CatTrabLaboRama>()
            {
                Id = i.Id.ToString(),
                Name = i.Rama_ENG,
                orden = "1"
            }).ToList());

            CVST = db.CatVirusSubType.OrderBy(i => i.orden);
            var CVSTDisplay = (user.Institution.Country.Language == "SPA" ? CVST.Select(i => new LookupView<CatVirusSubType>()
            {
                Id = i.ID.ToString(),
                Name = i.SPA,
                orden = i.orden.ToString()
            }).ToList() : CVST.Select(i => new LookupView<CatVirusSubType>()
            {
                Id = i.ID.ToString(),
                Name = i.ENG,
                orden = i.orden.ToString()
            }).ToList());


            var CatVirusSubtypeConfiguration = db.CatVirusSubTypeConfByLab.OfType<CatVirusSubTypeConfByLab>().Where(i => i.id_Institution == user.InstitutionID);

            if (!CatVirusSubtypeConfiguration.Any())
            {
                CVST_Test = db.CatVirusSubType.OrderBy(i => i.orden);
            }
            else
            {
                var ListVaccines = CatVirusSubtypeConfiguration.Select(i => i.id_CatSubType).ToList();
                CVST_Test = db.CatVirusSubType.Where(k => ListVaccines.Contains(k.ID));
            }

            var CVST_TestDisplay = (user.Institution.Country.Language == "SPA" ?  CVST_Test.Select(i => new LookupView<CatVirusSubType>()
            {
                Id = i.ID.ToString(),
                Name = i.SPA,
                orden = i.orden.ToString()
            }).ToList() : CVST_Test.Select(i => new LookupView<CatVirusSubType>()
            {
                Id = i.ID.ToString(),
                Name = i.ENG,
                orden = i.orden.ToString()
            }).ToList());


            CVL = db.CatVirusLinaje.OrderBy(i => i.orden);
            var CVLDisplay = (user.Institution.Country.Language == "SPA" ?  CVL.Select(i => new LookupView<CatVirusLinaje>()
            {
                Id = i.ID.ToString(),
                Name = i.SPA,
                orden = i.orden.ToString()
            }).ToList() : CVL.Select(i => new LookupView<CatVirusLinaje>()
            {
                Id = i.ID.ToString(),
                Name = i.ENG,
                orden = i.orden.ToString()
            }).ToList());

            CNP = db.CatNativePeople.Where(i => i.id_country == user.Institution.CountryID).OrderBy(i => i.description);
            var CNPDisplay = CNP.Select(i => new LookupView<CatNativePeople>()
            {
                Id = i.ID.ToString(),
                Name = i.description,
            }).ToList();

            var VaccineSourceConfiguration = db.CatVaccinSourceConf.OfType<CatVaccinSourceConf>()
                    .Where(i => i.id_country == user.Institution.CountryID);


            if (!VaccineSourceConfiguration.Any())
            {
                CVS = db.CatVaccinSource.Where(j => j.id_country == null).OrderBy(i => i.description);
            } else
            {
                var ListVaccines = VaccineSourceConfiguration.Select(i => i.id_catvaccinsource).ToList();
                CVS = db.CatVaccinSource.Where(k => ListVaccines.Contains(k.ID));
            }

            var CVSDisplay = CVS.Select(i => new LookupView<CatVaccinSource>()
            {
                Id = i.ID.ToString(),
                Name = i.description,
            }).ToList();

            CaseViewModel.CSNP = CSNPDisplay;
            CaseViewModel.CTT = CTTDisplay;
            CaseViewModel.CTR = CTRDisplay;
            CaseViewModel.CVT = CVTDisplay;
            CaseViewModel.COVT = COVTDisplay;
            CaseViewModel.CVST = CVSTDisplay;
            CaseViewModel.CVST_Test = CVST_TestDisplay;
            CaseViewModel.CVL = CVLDisplay;
            CaseViewModel.CHIn = CHInDisplay;

            CaseViewModel.CNP = CNPDisplay;
            CaseViewModel.CVS = CVSDisplay;
            CaseViewModel.CatOccupations = occupationsDisplay;                  //**** CAFQ
            CaseViewModel.CatTrabSaludRamas = trabsaludramaDisplay;             //**** CAFQ
            CaseViewModel.CatTrabLaboRamas = trablaboramaDisplay;               //**** CAFQ
            int[] _interval_1 = {};
            int[] _interval_2 = {};

            // activar paginador para Jamaica
            if (user.Institution.CountryID == 17)
            {
                _interval_1 = new int[] { 10, 20, 30 };
            } else
            {
                _interval_1 = new int[] { 10, 20, 30 };
            }

            // activar paginador segunda bandeja para Jamaica
            if (user.Institution.CountryID == 17)
            {
                _interval_2 = new int[] { 10, 30, 50, 100 };
            } else
            {
                _interval_2 = new int[] { 10, 20, 30 };
            }


            CaseViewModel.caselist_interval_1 =  _interval_1;
            CaseViewModel.caselist_interval_2 = _interval_2;

            // Laboratorios de la ficha
            // *** DELETE ***
            CaseViewModel.Labs = (from institution in db.Institutions.OfType<Lab>().Where(i => i.CountryID == user.Institution.CountryID)
                                  select new LookupView<Lab>()
                                  {
                                      Id = institution.ID.ToString(),
                                      Name = institution.Name
                                  }).ToArray();

            // Combo de laboratorios externos
            var LabsForeign = db.InstitutionForeignConf.Where(z => z.InstitutionLocal.CountryID == user.Institution.CountryID).Select(x => x.InstitutionForeignID);

            if (LabsForeign.Any() == true)
            {

                CaseViewModel.LabsExternal = (from institution in db.Institutions.OfType<Lab>().Where(i => LabsForeign.Contains(i.ID) && i.ForeignLab == true)
                                              select new LookupView<Lab>()
                                              {
                                                  Id = institution.ID.ToString(),
                                                  Name = institution.Name
                                              })
                      .ToArray();

            }
            else
            {
                CaseViewModel.LabsExternal = LabsForeign.ToArray();
            }



            // Modificacion de Labs lo agrego AM 25 abril 2016

            var institutionsConfigurationLabs = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>()
                    .Where(i => i.InstitutionFromID == user.InstitutionID && i.Priority == 1).OrderBy(j => j.InstitutionParent.FullName);


            var institutionLabs = institutionsConfigurationLabs.Select(i => i.InstitutionTo);
            
            if (!institutionLabs.Any())
            {
                CaseViewModel.LabsHospital_cmb = (from institution in db.Institutions.OfType<Lab>().Where(i => i.CountryID == user.Institution.CountryID)
                                      select new LookupView<Lab>()
                                      {
                                          Id = institution.ID.ToString(),
                                          Name = institution.Name
                                      }).ToArray();
            } else
            {
                CaseViewModel.LabsHospital_cmb = institutionLabs.ToArray();
            }

            // *** DELETE ***

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

            return View(CaseViewModel);
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
