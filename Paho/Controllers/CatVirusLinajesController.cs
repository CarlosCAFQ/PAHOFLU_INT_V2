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
    public class CatVirusLinajesController : ControllerBase
    {
        //private PahoDbContext db = new PahoDbContext();
        int _pageSize = 10;
        // GET: CatVirusLinajes
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


            var catalogo = from c in db.CatVirusLinaje select c;
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

            //return View(db.CatVirusLinaje.ToList());
        }

        // GET: CatVirusLinajes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatVirusLinaje catVirusLinaje = db.CatVirusLinaje.Find(id);
            if (catVirusLinaje == null)
            {
                return HttpNotFound();
            }
            return View(catVirusLinaje);
        }

        // GET: CatVirusLinajes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CatVirusLinajes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SPA,ENG,id_country,orden")] CatVirusLinaje catVirusLinaje)
        {
            if (ModelState.IsValid)
            {
                db.CatVirusLinaje.Add(catVirusLinaje);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(catVirusLinaje);
        }

        // GET: CatVirusLinajes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatVirusLinaje catVirusLinaje = db.CatVirusLinaje.Find(id);
            if (catVirusLinaje == null)
            {
                return HttpNotFound();
            }
            return View(catVirusLinaje);
        }

        // POST: CatVirusLinajes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SPA,ENG,id_country,orden")] CatVirusLinaje catVirusLinaje)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catVirusLinaje).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(catVirusLinaje);
        }

        // GET: CatVirusLinajes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatVirusLinaje catVirusLinaje = db.CatVirusLinaje.Find(id);
            if (catVirusLinaje == null)
            {
                return HttpNotFound();
            }
            return View(catVirusLinaje);
        }

        // POST: CatVirusLinajes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CatVirusLinaje catVirusLinaje = db.CatVirusLinaje.Find(id);
            db.CatVirusLinaje.Remove(catVirusLinaje);
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
