using Microsoft.AspNet.Identity;
using PagedList;
using Resources.Abstract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Paho.Models;

namespace Paho.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NeighborhoodController : ControllerBase
    {
        private int _pageSize = 10;

        // GET: Neighborhood
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var countryId = user.Institution.CountryID ?? 0;
            //var neighborhood = db.Neighborhoods.Include(p => p.State);

            //****
            ViewBag.CurrentSort = sortOrder;
            ViewBag.AreaParm = "areaName";
            ViewBag.StateParm = "stateName";
            ViewBag.NeighborhoodParm = "neighborhoodName";
            //ViewBag.OrigCountryParm = "origCountry";

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            var catalogo = from c in db.Neighborhoods where c.State.Area.CountryID == countryId select c;
            if (!string.IsNullOrEmpty(searchString))
            {
                catalogo = catalogo.Where(s => s.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "areaName":
                    catalogo = catalogo.OrderBy(s => s.State.Area.Name).ThenBy(s => s.State.Name).ThenBy(s => s.Name);
                    break;
                case "stateName":
                    catalogo = catalogo.OrderBy(s => s.State.Name).ThenBy(s => s.Name);
                    break;
                case "neighborhoodName":
                    catalogo = catalogo.OrderBy(s => s.Name);
                    break;
                case "origCountry":
                    catalogo = catalogo.OrderBy(s => s.orig_country);
                    break;
                default:
                    catalogo = catalogo.OrderBy(s => s.State.Area.Name).ThenBy(s => s.State.Name).ThenBy(s => s.Name);
                    break;
            }

            //****
            if (user.Institution.AccessLevel == AccessLevel.Country || user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    catalogo = catalogo.Where(s => s.State.Area.CountryID == user.Institution.CountryID);
                    /*.OrderBy(o => o.State.Area.Country.Name).ThenBy(o => o.State.Area.Name)
                    .ThenBy(o => o.State.Name).ThenBy(o => o.Name);*/
                }
                else if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
                {
                    catalogo = catalogo.Where(s => s.State.Area.CountryID == user.Institution.CountryID);
                    /*.OrderBy(o => o.State.Area.Country.Name).ThenBy(o => o.State.Area.Name)
                    .ThenBy(o => o.State.Name).ThenBy(o => o.Name);*/
                }
            }

            int pageSize = _pageSize;
            int pageNumber = (page ?? 1);

            //return View(neighborhood.ToList());
            return View(catalogo.ToPagedList(pageNumber, pageSize));
        }


        // GET: Neighborhood/Create
        public ActionResult Create()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var UsrLang = user.Institution.Country.Language;

            //****
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

            ViewBag.Countries = new SelectList(cat_countries, "ID", "Name");

            //****
            var areas = db.Areas.Include(a => a.Country);

            if (user.Institution.AccessLevel == AccessLevel.Country || user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    areas = areas.Where(s => s.CountryID == user.Institution.CountryID)
                                    .OrderBy(o => o.Country.Name).ThenBy(o => o.Name);
                }
                else if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
                {
                    areas = areas.Where(s => s.CountryID == user.Institution.CountryID)
                                    .OrderBy(o => o.Country.Name).ThenBy(o => o.Name);
                }
            }

            ViewBag.Areas = new SelectList(areas, "ID", "Name");

            //****
            return View();
        }

        // POST: Neighborhood/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID, StateID, Code, Name, RegionID, orig_country, latitude, longitude, x, y")] Neighborhood neighborhood)
        {
            if (ModelState.IsValid)
            {
                db.Neighborhoods.Add(neighborhood);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.CountryID = new SelectList(db.Countries, "ID", "Code", area.CountryID);           //???????
            return View(neighborhood);
        }

        // GET: Neighborhood/Edit/5
        public ActionResult Edit(int? id)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var UsrLang = user.Institution.Country.Language;
            //****
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

            ViewBag.Countries = new SelectList(cat_countries, "ID", "Name");

            //****
            var cat_areas = db.Areas.Include(a => a.Country);

            if (user.Institution.AccessLevel == AccessLevel.Country || user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    cat_areas = cat_areas.Where(s => s.CountryID == user.Institution.CountryID)
                                    .OrderBy(o => o.Country.Name).ThenBy(o => o.Name);
                }
                else if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
                {
                    cat_areas = cat_areas.Where(s => s.CountryID == user.Institution.CountryID)
                                    .OrderBy(o => o.Country.Name).ThenBy(o => o.Name);
                }
            }

            ViewBag.Areas = new SelectList(cat_areas, "ID", "Name");

            /*/****
            var cat_states = db.States.Include(a => a.Area);

            if (user.Institution.AccessLevel == AccessLevel.Country || user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    cat_states = cat_states.Where(s => s.Area.CountryID == user.Institution.CountryID)
                                    .OrderBy(o => o.Area.Country.Name).ThenBy(o => o.Area.Name).ThenBy(o => o.Name);
                }
                else if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
                {
                    cat_states = cat_states.Where(s => s.Area.CountryID == user.Institution.CountryID)
                                    .OrderBy(o => o.Area.Country.Name).ThenBy(o => o.Area.Name).ThenBy(o => o.Name);
                }
            }

            ViewBag.States = new SelectList(cat_states, "ID", "Name");
            */

            //****
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Neighborhood neighborhood = db.Neighborhoods.Find(id);
            if (neighborhood == null)
                return HttpNotFound();

            return View(neighborhood);
        }

        // POST: Neighborhood/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID, StateID, Code, Name, RegionID, orig_country, latitude, longitude, x, y")] Neighborhood neighborhood)
        {
            if (ModelState.IsValid)
            {
                db.Entry(neighborhood).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.CountryID = new SelectList(db.Countries, "ID", "Code", area.CountryID);           //??????
            return View(neighborhood);
        }

        // GET: Neighborhood/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Neighborhood neighborhood = db.Neighborhoods.Find(id);
            if (neighborhood == null)
                return HttpNotFound();

            return View(neighborhood);
        }

        // POST: Neighborhood/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Neighborhood neighborhood = db.Neighborhoods.Find(id);
            db.Neighborhoods.Remove(neighborhood);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: State/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Neighborhood neighborhood = db.Neighborhoods.Find(id);
            if (neighborhood == null)
            {
                return HttpNotFound();
            }

            return View(neighborhood);
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

            return searchedMsg;
        }
    }
}
