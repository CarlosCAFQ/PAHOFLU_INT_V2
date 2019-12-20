using Microsoft.AspNet.Identity;
using PagedList;
using Paho.Models;
using Resources.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Paho.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CatVirusTypeController : ControllerBase
    {
        private static IResourceProvider resourceProvider = new DbResourceProvider();
        private int _pageSize = 10;

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

            var catalogo = from c in db.CatVirusType select c;
            if (!string.IsNullOrEmpty(searchString))
            {
                catalogo = catalogo.Where(s => s.SPA.Contains(searchString) || s.ENG.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "spa_desc":
                    catalogo = catalogo.OrderByDescending(s => s.SPA);
                    break;
                case "id":
                    catalogo = catalogo.OrderBy(s => s.ID);
                    break;
                case "id_desc":
                    catalogo = catalogo.OrderByDescending(s => s.ID);
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
        
        public ActionResult Details(int id)
        {
            return View();
        }
        
        public ActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SPA, ENG, orden")]CatVirusType catalog)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.CatVirusType.Add(catalog);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "No es posible guardar los datos. Intente de nuevo, si el problema persiste contacte al administrador.");
            }
            return View(catalog);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var catalogo = db.CatVirusType.Find(id);
            if (catalogo == null)
            {
                return HttpNotFound();
            }
            return View(catalogo);
        }
        
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var studentToUpdate = db.CatVirusType.Find(id);
            if (TryUpdateModel(studentToUpdate, "",
               new string[] { "SPA", "ENG", "orden" }))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "No es posible guardar los datos. Intente de nuevo, si el problema persiste contacte al administrador.");
                }
            }
            return View(studentToUpdate);
        }
        
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var catalogo = db.CatVirusType.Find(id);
            if (catalogo == null)
            {
                return HttpNotFound();
            }
            return View(catalogo);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var student = db.CatVirusType.Find(id);
                db.CatVirusType.Remove(student);
                db.SaveChanges();
            }
            catch (RetryLimitExceededException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
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
