using Microsoft.AspNet.Identity;
using PagedList;
using Paho.Models;
using Resources.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Paho.Controllers
{
    public class CatSampleReasonsNoProcessedController : ControllerBase
    {
        private static IResourceProvider resourceProvider = new DbResourceProvider();
        private int _pageSize = 10;

        // GET: CatSampleReasonsNoProcessed
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IDSortParm = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.SpaSortParm = string.IsNullOrEmpty(sortOrder) ? "spa_desc" : "";
            ViewBag.EngSortParm = sortOrder == "eng" ? "eng_desc" : "eng";
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

            var catalogo = from c in db.CatSampleNoProcessed select c;
            if (!string.IsNullOrEmpty(searchString))
            {
                catalogo = catalogo.Where(s => s.SPA.Contains(searchString) || s.ENG.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "spa_desc":
                    catalogo = catalogo.OrderByDescending(s => s.SPA);
                    break;
                case "eng":
                    catalogo = catalogo.OrderBy(s => s.ENG);
                    break;
                case "eng_desc":
                    catalogo = catalogo.OrderByDescending(s => s.ENG);
                    break;
                case "orden":
                    catalogo = catalogo.OrderBy(s => s.orden);
                    break;
                case "orden_desc":
                    catalogo = catalogo.OrderByDescending(s => s.orden);
                    break;
                default:
                    catalogo = catalogo.OrderBy(s => s.SPA);
                    break;
            }

            //**** Link Dashboard
            var user = UserManager.FindById(User.Identity.GetUserId());

            string dashbUrl = "", dashbTitle = "";
            List<Models.CatDashboardLink> lista = (from tg in db.CatDashboarLinks
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

            //****
            return View(catalogo.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: CatSampleNoProcessed/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SPA,ENG,orden,active")] CatSampleNoProcessed catSampleNoProcessed)
        {
            if (ModelState.IsValid)
            {
                db.CatSampleNoProcessed.Add(catSampleNoProcessed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(catSampleNoProcessed);
        }

        // GET: CatSampleNoProcessed/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CatSampleNoProcessed catSampleNoProcessed = db.CatSampleNoProcessed.Find(id);
            if (catSampleNoProcessed == null)
            {
                return HttpNotFound();
            }

            //****
            return View(catSampleNoProcessed);
        }

        // POST: CatSampleNoProcessed/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SPA,ENG,orden")] CatSampleNoProcessed catSampleNoProcessed)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catSampleNoProcessed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(catSampleNoProcessed);
        }

        // GET: CatSampleNoProcessed/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CatSampleNoProcessed catSampleNoProcessed = db.CatSampleNoProcessed.Find(id);
            if (catSampleNoProcessed == null)
            {
                return HttpNotFound();
            }
            //****
            return View(catSampleNoProcessed);
        }

        // POST: CatSampleNoProcessed/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CatSampleNoProcessed catSampleNoProcessed = db.CatSampleNoProcessed.Find(id);
            db.CatSampleNoProcessed.Remove(catSampleNoProcessed);
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