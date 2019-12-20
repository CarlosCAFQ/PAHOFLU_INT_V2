using Microsoft.AspNet.Identity;
using PagedList;
using Resources.Abstract;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Paho.Models;

namespace Paho.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RegionsController : ControllerBase
    {
        private static IResourceProvider resourceProvider = new DbResourceProvider();
        private int _pageSize = 10;

        // GET: Regions
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IDSortParm = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.OrigCounSortParm = sortOrder == "orig_country" ? "orig_country_desc" : "orig_country";
            var user = UserManager.FindById(User.Identity.GetUserId());

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var regions = from c in db.Regions select c;

            //var regions = db.Regions.Include(r => r.CatRegionType);

            if (user.Institution.AccessLevel == AccessLevel.Country ||  user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    regions = regions.Where(s => s.CountryID == user.Institution.CountryID);
                } else if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
                {
                    regions = regions.Where(s => s.CountryID == user.Institution.CountryID && ((s.tipo_region == 1 && s.orig_country == user.Institution.cod_region_institucional) || (s.tipo_region == 2 && s.orig_country == user.Institution.cod_region_salud) || (s.tipo_region == 3 && s.orig_country == user.Institution.cod_region_pais)));
                }

            }
            //var UsrLang = user.Institution.Country.Language;

            //if (UsrLang.ToUpper() == "ENG")
            //{
            //    regions = regions.Include(r => r.CatRegionType.ENG);
            //} else if (UsrLang.ToUpper() == "SPA")
            //{
            //    regions = regions.Include(x => x.CatRegionType.SPA);

            //} else 
            //{
            //    regions = regions.Include(r => r.CatRegionType.SPA);
            //}
            
            //regions = regions.Include(x => x.Cat_Country);

            if (!string.IsNullOrEmpty(searchString))
            {
                regions = regions.Where(s => s.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name":
                    regions = regions.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    regions = regions.OrderByDescending(s => s.Name);
                    break;
                case "id":
                    regions = regions.OrderBy(s => s.ID);
                    break;
                case "id_desc":
                    regions = regions.OrderByDescending(s => s.ID);
                    break;
                case "orig_country":
                    regions = regions.OrderBy(s => s.orig_country);
                    break;
                case "orig_country_desc":
                    regions = regions.OrderByDescending(s => s.orig_country);
                    break;
                default:
                    regions = regions.OrderBy(s => s.tipo_region).ThenBy(s=> s.Name);
                    break;
            }

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

            int pageSize = _pageSize;
            int pageNumber = (page ?? 1);

            //PagedList<Region> model = new PagedList<Region>(regions, pageNumber, pageSize);

            //return View(model);

            return View(regions.ToPagedList(pageNumber, pageSize));
        }

        // GET: Regions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Region region = db.Regions.Find(id);
            if (region == null)
            {
                return HttpNotFound();
            }
            return View(region);
        }

        // GET: Regions/Create
        public ActionResult Create()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var UsrLang = user.Institution.Country.Language;
            var cat_countries = from c in db.Countries select c;
            if (user.Institution.AccessLevel == AccessLevel.Country || user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    cat_countries = cat_countries.Where(s => s.ID == user.Institution.CountryID);
                }
                else if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
                {
                    cat_countries = cat_countries.Where(s => s.ID == user.Institution.CountryID);
                }
            }

            ViewBag.CountryID = new SelectList(cat_countries, "ID", "Name");
            ViewBag.tipo_region = new SelectList(db.CatRegionType, "ID", UsrLang);

            return View();
        }

        // POST: Regions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CountryID,Name,orig_country,tipo_region")] Region region)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var UsrLang = user.Institution.Country.Language;

            if (ModelState.IsValid)
            {
                db.Regions.Add(region);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(region);
        }

        // GET: Regions/Edit/5
        public ActionResult Edit(int? id)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var UsrLang = user.Institution.Country.Language;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Region region = db.Regions.Find(id);
            if (region == null)
            {
                return HttpNotFound();
            }

            var cat_countries = from c in db.Countries select c;
            if (user.Institution.AccessLevel == AccessLevel.Country || user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    cat_countries = cat_countries.Where(s => s.ID == user.Institution.CountryID);
                }
                else if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
                {
                    cat_countries = cat_countries.Where(s => s.ID == user.Institution.CountryID);
                }
            }

            ViewBag.CountryID = new SelectList(cat_countries, "ID", "Name");
            ViewBag.tipo_region = new SelectList(db.CatRegionType, "ID", UsrLang , region.tipo_region);
            return View(region);
        }

        // POST: Regions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CountryID,Name,orig_country,tipo_region")] Region region)
        {

            if (ModelState.IsValid)
            {
                db.Entry(region).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
       
            return View(region);
        }

        // GET: Regions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Region region = db.Regions.Find(id);
            if (region == null)
            {
                return HttpNotFound();
            }
            return View(region);
        }

        // POST: Regions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Region region = db.Regions.Find(id);
            db.Regions.Remove(region);
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

        public static string SgetMessage(string msg, int? countryID, string countryLang)
        {
            string dbMessage = msg;
            string dbCountry = countryID.ToString();
            string dbLang = countryLang;

            //dbMessage = "Pepito";
            dbMessage = resourceProvider.GetResource(dbMessage, dbCountry).ToString();
            if (dbMessage == "")
            {
                dbMessage = msg;
                dbMessage = resourceProvider.GetResource(dbMessage, dbLang).ToString();
            }
            return dbMessage;
        }
    }
}
