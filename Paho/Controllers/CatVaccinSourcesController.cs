using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Paho.Models;
using Microsoft.AspNet.Identity;

namespace Paho.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CatVaccinSourcesController : ControllerBase
    {
        private int _pageSize = 10;

        // GET: CatVaccinSources
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DescSortParm = string.IsNullOrEmpty(sortOrder) ? "descrip_desc" : "";
            ViewBag.OrdenSortParm = sortOrder == "orden" ? "orden_desc" : "orden";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var catalogo = from c in db.CatVaccinSource select c;
            if (!string.IsNullOrEmpty(searchString))
            {
                catalogo = catalogo.Where(s => s.description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "descrip_desc":
                    catalogo = catalogo.OrderByDescending(s => s.description);
                    break;
                case "id":
                    catalogo = catalogo.OrderBy(s => s.ID);
                    break;
                case "id_desc":
                    catalogo = catalogo.OrderByDescending(s => s.ID);
                    break;
                case "orden":
                    catalogo = catalogo.OrderBy(s => s.orden);
                    break;
                case "orden_desc":
                    catalogo = catalogo.OrderByDescending(s => s.orden);
                    break;
                default:
                    catalogo = catalogo.OrderBy(s => s.description);
                    break;
            }

            //**** Link Dashboard
            var user = UserManager.FindById(User.Identity.GetUserId());

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

            return View(catalogo.ToPagedList(pageNumber, pageSize));
        }

        // GET: CatVaccinSources/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatVaccinSource catVaccinSource = db.CatVaccinSource.Find(id);
            if (catVaccinSource == null)
            {
                return HttpNotFound();
            }
            return View(catVaccinSource);
        }

        // GET: CatVaccinSources/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CatVaccinSources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,description,orden,id_country")] CatVaccinSource catVaccinSource)
        {
            if (ModelState.IsValid)
            {
                db.CatVaccinSource.Add(catVaccinSource);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(catVaccinSource);
        }

        // GET: CatVaccinSources/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatVaccinSource catVaccinSource = db.CatVaccinSource.Find(id);
            if (catVaccinSource == null)
            {
                return HttpNotFound();
            }
            return View(catVaccinSource);
        }

        // POST: CatVaccinSources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,description,orden,id_country")] CatVaccinSource catVaccinSource)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catVaccinSource).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(catVaccinSource);
        }

        // GET: CatVaccinSources/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatVaccinSource catVaccinSource = db.CatVaccinSource.Find(id);
            if (catVaccinSource == null)
            {
                return HttpNotFound();
            }
            return View(catVaccinSource);
        }

        // POST: CatVaccinSources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CatVaccinSource catVaccinSource = db.CatVaccinSource.Find(id);
            db.CatVaccinSource.Remove(catVaccinSource);
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
    }
}
