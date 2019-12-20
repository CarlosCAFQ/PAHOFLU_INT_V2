using Microsoft.AspNet.Identity;
using PagedList;
using Paho.Models;
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
    public class CatAgeGroupController : ControllerBase
    {

        private int _pageSize = 10;
        
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var countryId = user.Institution.CountryID ?? 0;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IDSortParm = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.DescripcionSortParm = string.IsNullOrEmpty(sortOrder) ? "desc_desc" : "";
            ViewBag.MesIniSortParm = sortOrder == "mes_ini" ? "mes_ini_desc" : "mes_ini";
            ViewBag.MesFinSortParm = sortOrder == "mes_fin" ? "mes_fin_desc" : "mes_fin";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var catalogo = from c in db.CatAgeGroup where c.id_country == countryId select c;
            if (!string.IsNullOrEmpty(searchString))
            {
                catalogo = catalogo.Where(s => s.AgeGroup.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "desc_desc":
                    catalogo = catalogo.OrderByDescending(s => s.AgeGroup);
                    break;
                case "id":
                    catalogo = catalogo.OrderBy(s => s.id);
                    break;
                case "id_desc":
                    catalogo = catalogo.OrderByDescending(s => s.id);
                    break;
                case "mes_ini":
                    catalogo = catalogo.OrderBy(s => s.month_begin);
                    break;
                case "mes_ini_desc":
                    catalogo = catalogo.OrderByDescending(s => s.month_begin);
                    break;
                case "mes_fin":
                    catalogo = catalogo.OrderBy(s => s.month_end);
                    break;
                case "mes_fin_desc":
                    catalogo = catalogo.OrderByDescending(s => s.month_end);
                    break;
                default:
                    catalogo = catalogo.OrderBy(s => s.month_begin);
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

        // GET: CatAgeGroup/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CatAgeGroup/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CatAgeGroup/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_conf_country, AgeGroup, month_begin, month_end, label_fluid")]CatAgeGroup catalog)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = UserManager.FindById(User.Identity.GetUserId());
                    var countryId = user.Institution.CountryID ?? 0;

                    catalog.id_country = countryId;
                    db.CatAgeGroup.Add(catalog);
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

        // GET: CatAgeGroup/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatAgeGroup catalogo = db.CatAgeGroup.Find(id);
            if (catalogo == null)
            {
                return HttpNotFound();
            }
            return View(catalogo);
        }

        // POST: CatAgeGroup/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var studentToUpdate = db.CatAgeGroup.Find(id);
            if (TryUpdateModel(studentToUpdate, "",
               new string[] { "id_conf_country", "AgeGroup", "month_begin", "month_end", "label_fluid" }))
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

        // GET: CatAgeGroup/Delete/5
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
            CatAgeGroup catalogo = db.CatAgeGroup.Find(id);
            if (catalogo == null)
            {
                return HttpNotFound();
            }
            return View(catalogo);
        }

        // POST: CatAgeGroup/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                CatAgeGroup student = db.CatAgeGroup.Find(id);
                db.CatAgeGroup.Remove(student);
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
    }
}
