using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Paho.Models;

namespace Paho.Controllers
{
    [Authorize]
    public class PrincipalController : ControllerBase
    {
        // GET: Principal
        public ActionResult Index()
        {
            var PrincipalViewModel = new PrincipalViewModel();
            IQueryable<Institution> institutions = null;
            IQueryable<Region> regions = null;
            IQueryable<State> states = null;
            IQueryable<Area> areas = null;
            var user = UserManager.FindById(User.Identity.GetUserId());
            //regions = db.Regions.Where(i => i.CountryID == user.Institution.CountryID);
            areas = db.Areas.Where(i => i.CountryID == user.Institution.CountryID);

            if (user.type_region == null)
            {     // Regiones
                regions = db.Regions.Where(c => c.CountryID == user.Institution.CountryID && c.tipo_region == 1).OrderBy(i => i.Name);
            }
            else
            {
                // Regiones
                regions = db.Regions.Where(c => c.CountryID == user.Institution.CountryID && c.tipo_region == user.type_region).OrderBy(i => i.Name);
            }

            if (areas != null)
            {
                PrincipalViewModel.Areas = areas.Select(i => new LookupView<Area>()
                {
                    Id = i.ID.ToString(),
                    Name = i.Name
                }).ToArray();
            };
            List<Dictionary<string, string>> summaryState = new List<Dictionary<string, string>>();


            /*foreach (var myArea in areas)
            {
                states = db.States.Where(i => i.AreaID == myArea.ID);
                if (states != null)
                {
                    //PrincipalViewModel.Areas = areas.Select(i => new LookupView<Area>()

                    PrincipalViewModel.States = states.Select(i => new LookupView<State>()
                    {
                        Id = i.ID.ToString(),
                        Name = i.Name
                    }).ToArray();

                    foreach(var ar in PrincipalViewModel.Areas)
                    {
                        List<PrincipalViewModel.States> dictionary = new List<PrincipalViewModel.States>();
                        dictionary.Add("Id", ar.Id);
                        dictionary.Add("Name", ar.Name);
                        summaryState.Add(dictionary);
                    }



                }
            }
            PrincipalViewModel.States = (IQueryable < Area >) summaryState.ToArray();*/


            PrincipalViewModel.CountryID = user.Institution.CountryID ?? 0;
            if (user.Institution.AccessLevel == AccessLevel.All)
            {
                PrincipalViewModel.DisplayCountries = true;
                PrincipalViewModel.DisplayHospitals = true;
            }
            else if (user.Institution is Hospital || user.Institution is AdminInstitution)
            {
                PrincipalViewModel.DisplayHospitals = true;

                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Area)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.AreaID == user.Institution.AreaID);
                }
                else
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.ID == user.Institution.ID);
                }
            }
            else
            {
                var institutionsConfiguration = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>()
                    .Where(i => i.InstitutionFromID == user.Institution.ID && i.Priority == 1);

                institutions = institutionsConfiguration.Select(i => i.InstitutionTo);

                if (!institutions.Any())
                {

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
            }

            PrincipalViewModel.Countries = db.Countries
                    .Select(c => new CountryView()
                    {
                        Id = c.ID.ToString(),
                        Name = c.Name,
                        Active = c.Active
                    }).ToArray();

            if (institutions != null)
            {
                PrincipalViewModel.Institutions = institutions.Select(i => new LookupView<Institution>()
                {
                    Id = i.ID.ToString(),
                    Name = i.Name
                }).ToArray();
            };
            if (regions != null)
            {
                PrincipalViewModel.Regions = regions.Select(i => new LookupView<Region>()
                {
                    //Id = i.ID.ToString(),
                    Id = i.orig_country.ToString(),
                    Name = i.Name
                }).ToArray();
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
            ViewBag.DisclaimerMap = "The designations employed and the presentation of the material in these maps do not imply the expression of any opinion whatsoever on the part of the Secretariat of the Pan American Health Organization concerning the legal status of any country, territory, city or area or of its authorities, or concerning the delimitation of its frontiers or boundaries. Dotted and dashed lines on maps represent approximate border lines for which there may not yet be full agreement.";
            //****

            return View(PrincipalViewModel);
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
    }
}