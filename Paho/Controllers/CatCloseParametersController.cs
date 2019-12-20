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
    public class CatCloseParametersController : ControllerBase
    {
        private int _pageSize = 10;

        // GET: CatReasonNotSampling
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IDSortParm = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.SurvSortParm = string.IsNullOrEmpty(sortOrder) ? "surv_desc" : "";
            ViewBag.IsSampleSortParm = sortOrder == "issample" ? "issample_desc" : "issample";
            //ViewBag.OrdenSortParm = sortOrder == "orden" ? "orden_desc" : "orden";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var catalogo = from c in db.CatCloseParameters select c;
            //if (!string.IsNullOrEmpty(searchString))
            //{
            //    catalogo = catalogo.Where(s => s.SPA.Contains(searchString) || s.ENG.Contains(searchString));
            //}

            switch (sortOrder)
            {
                case "surv":
                    catalogo = catalogo.OrderBy(s => s.Surv);
                    break;
                case "surv_desc":
                    catalogo = catalogo.OrderByDescending(s => s.Surv);
                    break;
                case "issample":
                    catalogo = catalogo.OrderBy(s => s.IsSample);
                    break;
                case "issample_desc":
                    catalogo = catalogo.OrderByDescending(s => s.IsSample);
                    break;
                default:
                    catalogo = catalogo.OrderBy(s => s.Id);
                    break;
            }

            //****
            string NAText = Enum.GetName(typeof(BooleanTypeNA), 9);

            foreach (var oObje in catalogo)
            {
                string text = (oObje.IsSample == null) ? NAText : Enum.GetName(typeof(BooleanTypeNA), oObje.IsSample);
                oObje.IsSampleText = text;

                text = (oObje.Processed == null) ? NAText : Enum.GetName(typeof(BooleanTypeNA), oObje.Processed);
                oObje.ProcessedText = text;

                text = (oObje.LabEndClosing == null) ? NAText : Enum.GetName(typeof(BooleanTypeNA), oObje.LabEndClosing);
                oObje.LabEndClosingText = text;

                text = (oObje.HospExDate == null) ? NAText : Enum.GetName(typeof(BooleanTypeNA), oObje.HospExDate);
                oObje.HospExDateText = text;

                text = (oObje.DiagEg == null) ? NAText : Enum.GetName(typeof(BooleanTypeNA), oObje.DiagEg);
                oObje.DiagEgText = text;
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

        //// GET: CatCloseParameters/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        // GET: CatCloseParameters/Create
        public ActionResult Create()
        {
            CatCloseParameters oCatCloseParameters = new CatCloseParameters();

            try
            {
                oCatCloseParameters.SurvRegCollection = db.CatSurv.ToList<CatSurv>();
                return View(oCatCloseParameters);
            }
            catch (Exception err)
            {
                ViewBag.Message = "La lista de tipos de Vigilancia no se pudo generar, por favor intente de nuevo: " + err.Message;
                return View();
            }
        }

        // POST: CatCloseParameters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Surv,IsSample,Processed,LabEndClosing,HospExDate,DiagEg")] CatCloseParameters oCatCloseParameter)
        {
            if (ModelState.IsValid)
            {
                db.CatCloseParameters.Add(oCatCloseParameter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(oCatCloseParameter);
        }

        // GET: CatCloseParameters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //****
            var catalogo = db.CatCloseParameters.Find(id);
            if (catalogo == null)
            {
                return HttpNotFound();
            }

            catalogo.SurvRegCollection = db.CatSurv.ToList<CatSurv>();

            return View(catalogo);
        }

        // POST: CatCloseParameters/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var catalog = db.CatCloseParameters.Find(id);
            if (TryUpdateModel(catalog, "", new string[] { "Surv", "IsSample", "Processed", "LabEndClosing", "HospExDate", "DiagEg" }))
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

            return View(catalog);
        }

        // GET: CatCloseParameters/Delete/5
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

            //****
            var catalogo = db.CatCloseParameters.Find(id);
            if (catalogo == null)
            {
                return HttpNotFound();
            }

            //****
            string NAText = Enum.GetName(typeof(BooleanTypeNA), 9);

            string text = (catalogo.IsSample == null) ? NAText : Enum.GetName(typeof(BooleanTypeNA), catalogo.IsSample);
            catalogo.IsSampleText = text;

            text = (catalogo.Processed == null) ? NAText : Enum.GetName(typeof(BooleanTypeNA), catalogo.Processed);
            catalogo.ProcessedText = text;

            text = (catalogo.LabEndClosing == null) ? NAText : Enum.GetName(typeof(BooleanTypeNA), catalogo.LabEndClosing);
            catalogo.LabEndClosingText = text;

            text = (catalogo.HospExDate == null) ? NAText : Enum.GetName(typeof(BooleanTypeNA), catalogo.HospExDate);
            catalogo.HospExDateText = text;

            text = (catalogo.DiagEg == null) ? NAText : Enum.GetName(typeof(BooleanTypeNA), catalogo.DiagEg);
            catalogo.DiagEgText = text;

            return View(catalogo);
        }

        // POST: CatCloseParameters/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add delete logic here
                var catalog = db.CatCloseParameters.Find(id);
                db.CatCloseParameters.Remove(catalog);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
        }
    }
}
