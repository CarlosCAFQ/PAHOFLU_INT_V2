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
    public class CatVirusSubTypesController : ControllerBase
    {
        //private PahoDbContext db = new PahoDbContext();
        int _pageSize = 10;
        // GET: CatVirusSubTypes
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

            var catalogo = from c in db.CatVirusSubType select c;
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

        // GET: CatVirusSubTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatVirusSubType catVirusSubType = db.CatVirusSubType.Find(id);
            if (catVirusSubType == null)
            {
                return HttpNotFound();
            }
            return View(catVirusSubType);
        }

        // GET: CatVirusSubTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CatVirusSubTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SPA,ENG,id_country,orden")] CatVirusSubType catVirusSubType)
        {
            if (ModelState.IsValid)
            {
                db.CatVirusSubType.Add(catVirusSubType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(catVirusSubType);
        }

        // GET: CatVirusSubTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatVirusSubType catVirusSubType = db.CatVirusSubType.Find(id);
            if (catVirusSubType == null)
            {
                return HttpNotFound();
            }
            return View(catVirusSubType);
        }

        // POST: CatVirusSubTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SPA,ENG,id_country,orden")] CatVirusSubType catVirusSubType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catVirusSubType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(catVirusSubType);
        }

        // GET: CatVirusSubTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatVirusSubType catVirusSubType = db.CatVirusSubType.Find(id);
            if (catVirusSubType == null)
            {
                return HttpNotFound();
            }
            return View(catVirusSubType);
        }

        // POST: CatVirusSubTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CatVirusSubType catVirusSubType = db.CatVirusSubType.Find(id);
            db.CatVirusSubType.Remove(catVirusSubType);
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
