using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Paho.Models;
using PagedList;

namespace Paho.Controllers
{
    [Authorize(Roles = "Admin, Staff")]
    //[Authorize(Roles = "Staff")]
    public class CatPopulationInstitutionsController : ControllerBase
    {
        //private PahoDbContext db = new PahoDbContext();
        private int _pageSize = 10;

        // GET: CatPopulationInstitutions
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            var user = UserManager.FindById(User.Identity.GetUserId());
            var countryId = user.Institution.CountryID ?? 0;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.FullNameCountry = string.IsNullOrEmpty(sortOrder) ? "fullnamecountry_desc" : "fullnamecountry";
            ViewBag.FullNameInstitution = sortOrder == "fullnameinst" ? "fullnameinst_desc" : "fullnameinst";
            ViewBag.PopulationYear = sortOrder == "popyear" ? "popyear_desc" : "popyear";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            IQueryable<CatPopulationInstitution> catalogo = null;

            if (user.Institution is AdminInstitution && user.InstitutionID == 1)
            {
                catalogo = db.CatPopulationInstitutions.Include(c => c.CountryPopulation).Include(c => c.InstitutionPopulation);
            }
            else if (user.Institution is AdminInstitution)
            {
                catalogo = db.CatPopulationInstitutions.Include(c => c.CountryPopulation).Include(c => c.InstitutionPopulation).Where( d => d.country_id == countryId);
            }
            else
            {
                catalogo = db.CatPopulationInstitutions.Include(c => c.CountryPopulation).Include(c => c.InstitutionPopulation).Where(d => d.country_id == countryId && d.id_institution == user.InstitutionID);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                catalogo = catalogo.Where(s =>
                s.InstitutionPopulation.FullName.Contains(searchString) ||
                s.CountryPopulation.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "fullnameinst":
                    catalogo = catalogo.OrderBy(s => s.InstitutionPopulation.Name);
                    break;
                case "fullnameinst_desc":
                    catalogo = catalogo.OrderByDescending(s => s.InstitutionPopulation.Name);
                    break;
                case "fullnamecountry":
                    catalogo = catalogo.OrderBy(s => s.CountryPopulation.Name);
                    break;
                case "fullnamecountry_desc":
                    catalogo = catalogo.OrderByDescending(s => s.CountryPopulation.Name);
                    break;
                case "popyear":
                    catalogo = catalogo.OrderBy(s => s.year);
                    break;
                case "popyear_desc":
                    catalogo = catalogo.OrderByDescending(s => s.year);
                    break;
                default:
                    catalogo = catalogo.OrderBy(s => s.CountryPopulation.Name).ThenBy(x=> x.InstitutionPopulation.Name).ThenBy(y => y.year);
                    break;
            }

            int pageSize = _pageSize;
            int pageNumber = (page ?? 1);

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

            return View(catalogo.ToPagedList(pageNumber, pageSize));
        }

        // GET: CatPopulationInstitutions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatPopulationInstitution catPopulationInstitution = db.CatPopulationInstitutions.Find(id);
            if (catPopulationInstitution == null)
            {
                return HttpNotFound();
            }
            return View(catPopulationInstitution);
        }

        // GET: CatPopulationInstitutions/Create
        public ActionResult Create()
        {


            PopulationDepartmentsDropDownList();
            return View();
        }

        // POST: CatPopulationInstitutions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,country_id,id_institution,population,year")] CatPopulationInstitution catPopulationInstitution, int[] Pop_PobFem , int[] Pop_PobMas, int[] Pop_PobTot, int[] Pop_id_conf_country)
        {
            if (ModelState.IsValid)
            {
                var CatPop = new CatPopulationInstitution()
                {
                    country_id = catPopulationInstitution.country_id,
                    id_institution = catPopulationInstitution.id_institution,
                    population = catPopulationInstitution.population,
                    year = catPopulationInstitution.year,
                    CatPopulationDetails = new List<CatPopulationInstitutionDetail>()
                };
                db.Entry(CatPop).State = EntityState.Added;
 
                for (int i = 0; i < Pop_id_conf_country.Length; i++)
                {
                    CatPop.CatPopulationDetails.Add(
                        new CatPopulationInstitutionDetail()
                        {
                            AgeGroup = Pop_id_conf_country[i],
                            PopulationFem = Pop_PobFem[i],
                            PopulationMaso = Pop_PobMas[i],
                            PopulationT = Pop_PobTot[i]
                        }
                    );
                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            PopulationDepartmentsDropDownList();

            return View(catPopulationInstitution);
        }

        // GET: CatPopulationInstitutions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatPopulationInstitution catPopulationInstitution = db.CatPopulationInstitutions.Find(id);
            if (catPopulationInstitution == null)
            {
                return HttpNotFound();
            }

            PopulationDepartmentsDropDownList(catPopulationInstitution.country_id, catPopulationInstitution.id_institution, id);

            return View(catPopulationInstitution);
        }

        // POST: CatPopulationInstitutions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,country_id,id_institution,population,year")] CatPopulationInstitution catPopulationInstitution, int[] Pop_PobFem, int[] Pop_PobMas, int[] Pop_PobTot, int[] Pop_id_conf_country)
        {
            if (ModelState.IsValid)
            {
                var CatPop = new CatPopulationInstitution()
                {
                    id = catPopulationInstitution.id,
                    country_id = catPopulationInstitution.country_id,
                    id_institution = catPopulationInstitution.id_institution,
                    population = catPopulationInstitution.population,
                    year = catPopulationInstitution.year
                    //CatPopulationDetails = new List<CatPopulationInstitutionDetail>()
                };

                db.Entry(CatPop).State = EntityState.Modified;


                //foreach (object Pop_Det_Age in Pop_id_conf_country)

                for (int i = 0; i < Pop_id_conf_country.Length; i++)
                {
                    var valor = Pop_id_conf_country[i];
                    var CatPopDetail = db.CatPopulationInstitutionsDetails.FirstOrDefault(j => j.CatPobInstId == catPopulationInstitution.id && j.AgeGroup == valor);

                    CatPopDetail.PopulationFem = Pop_PobFem[i];
                    CatPopDetail.PopulationMaso = Pop_PobMas[i];
                    CatPopDetail.PopulationT = Pop_PobTot[i];
                    db.Entry(CatPopDetail).State = EntityState.Modified;
                }

                

                //db.Entry(catPopulationInstitution).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulationDepartmentsDropDownList(catPopulationInstitution.country_id, catPopulationInstitution.id_institution);

            return View(catPopulationInstitution);
        }

        // GET: CatPopulationInstitutions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatPopulationInstitution catPopulationInstitution = db.CatPopulationInstitutions.Find(id);
            if (catPopulationInstitution == null)
            {
                return HttpNotFound();
            }
            return View(catPopulationInstitution);
        }

        // POST: CatPopulationInstitutions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CatPopulationInstitution catPopulationInstitution = db.CatPopulationInstitutions.Find(id);
            db.CatPopulationInstitutions.Remove(catPopulationInstitution);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private void PopulationDepartmentsDropDownList(
    object selectedCountry = null,
    object selectedInstitution = null,
    int? id_record = 0)
        {

            var user = UserManager.FindById(User.Identity.GetUserId());
            var countryId = user.Institution.CountryID ?? 0;

            // Combo país
            IQueryable<Country> countryQuery = null;

            if (user.Institution is AdminInstitution && user.InstitutionID == 1)
            { countryQuery = from d in db.Countries orderby d.Name select d; }
            else
            { countryQuery = from d in db.Countries where d.ID == countryId orderby d.Name select d; }

            if (user.Institution is AdminInstitution && user.InstitutionID == 1)
                countryQuery.ToList().Insert(0, new Country { ID = 0, Name = "-- Seleccione -- " });

            var CountryItems = countryQuery.ToList();

            if (user.Institution is AdminInstitution && user.InstitutionID == 1)
                CountryItems.Insert(0, new Country { ID = 0, Name = "-- Seleccione -- " });

            ViewBag.country_id = new SelectList(CountryItems, "ID", "Name", selectedCountry);

            // Combo institución

            IQueryable<Institution> instQuery = null;
            instQuery = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID && i.ILI == true).OrderBy(n => n.Name);

            if (user.Institution is AdminInstitution && user.InstitutionID != 1)
            {
                instQuery = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == countryId && i.ILI == true).OrderBy(x => x.Name);
            }
            else if (user.InstitutionID != 1)
            {
                instQuery = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.ID == user.InstitutionID && i.ILI == true).OrderBy(x => x.Name);
            }

            var institutionsItems = instQuery.ToList();

            if (institutionsItems.Count > 1)
            institutionsItems.Insert(0, new Hospital { ID = 0, Name = "-- Seleccione -- " });

            ViewBag.id_institution = new SelectList(institutionsItems, "ID", "Name", selectedInstitution);

            IQueryable<CatAgeGroup> AgeGroupQuery = null;

            AgeGroupQuery = db.CatAgeGroup
                                  .Where(i => i.id_country == user.Institution.CountryID).OrderBy(n => n.id_conf_country);

            //ViewBag.AgeGroupsbyCountry = new SelectList(AgeGroupQuery, "id_conf_country", "AgeGroup", selectedInstitution);

            var CasePopulationInstitutionDetails = new List<CatPopulationInstitutionDetail>();
            var CPID_data = db.CatPopulationInstitutionsDetails.Where(i => i.CatPobInstId == id_record).ToList();


            foreach (CatAgeGroup agegroup in AgeGroupQuery)
            {
                if (id_record == 0) { 
                    CasePopulationInstitutionDetails.Add(new CatPopulationInstitutionDetail { 
                        AgeGroup = agegroup.id_conf_country,
                        PopulationFem = 0,
                        PopulationMaso = 0,
                        PopulationT = 0,
                        AgeGroupbyCountry = AgeGroupQuery.Where(i => i.id_conf_country == agegroup.id_conf_country).FirstOrDefault()
                    });
                } else
                {
                    CasePopulationInstitutionDetails.Add(new CatPopulationInstitutionDetail
                    {
                        AgeGroup = agegroup.id_conf_country,
                        PopulationFem = CPID_data.Where(i=> i.AgeGroup == agegroup.id_conf_country).FirstOrDefault().PopulationFem,
                        PopulationMaso = CPID_data.Where(i => i.AgeGroup == agegroup.id_conf_country).FirstOrDefault().PopulationMaso,
                        PopulationT = CPID_data.Where(i => i.AgeGroup == agegroup.id_conf_country).FirstOrDefault().PopulationT,
                        AgeGroupbyCountry = AgeGroupQuery.Where(i => i.id_conf_country == agegroup.id_conf_country).FirstOrDefault()
                    });
                }


            }

            ViewBag.CPIDetails = CasePopulationInstitutionDetails;
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
