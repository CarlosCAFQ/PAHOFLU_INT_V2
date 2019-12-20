using Microsoft.AspNet.Identity;
using PagedList;
using Paho.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Paho.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CatInstitucionConfController : ControllerBase
    {
        private int _pageSize = 10;

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var countryId = user.Institution.CountryID ?? 0;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.FullNameParentSortParm = string.IsNullOrEmpty(sortOrder) ? "fullnameparent_desc" : "fullnameparent";
            ViewBag.FullNameFromSortParm = sortOrder == "fullname" ? "fullname_desc" : "fullname";
            ViewBag.FullNameToSortParm = sortOrder == "fullnameto" ? "fullnameto_desc" : "fullnameto";


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var catalogo = from c in db.InstitutionsConfiguration
                           where c.InstitutionFrom.CountryID == countryId && 
                           c.InstitutionTo.CountryID == countryId && 
                           c.InstitutionParent.CountryID == countryId
                           select c;
            if (!string.IsNullOrEmpty(searchString))
            {
                catalogo = catalogo.Where(s => 
                s.InstitutionFrom.FullName.Contains(searchString) || 
                s.InstitutionTo.FullName.Contains(searchString) ||
                s.InstitutionParent.FullName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "fullname_desc":
                    catalogo = catalogo.OrderByDescending(s => s.InstitutionFrom.FullName);
                    break;
                case "fullnameto":
                    catalogo = catalogo.OrderBy(s => s.InstitutionTo.FullName);
                    break;
                case "fullnameto_desc":
                    catalogo = catalogo.OrderByDescending(s => s.InstitutionTo.FullName);
                    break;
                case "fullnameparent":
                    catalogo = catalogo.OrderBy(s => s.InstitutionParent.FullName);
                    break;
                case "fullnameparent_desc":
                    catalogo = catalogo.OrderByDescending(s => s.InstitutionParent.FullName);
                    break;
                default:
                    catalogo = catalogo.OrderBy(s => s.InstitutionParent.FullName).ThenBy(w => w.Priority);
                    break;
            }

            if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                catalogo = catalogo.Where(j => j.InstitutionParentID == user.InstitutionID);
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

        // GET: CatInstitucionConf/Create
        public ActionResult Create()
        {
            //****
            InstitutionConfiguration oInCoModel = new InstitutionConfiguration();

            var oList = (from regis in db.CatViruFlows as IQueryable<CatVirusFlow>
                         orderby regis.CatTestTypes.description, regis.CatTestResults.description
                         select new
                         {
                             ID = regis.ID.ToString(),
                             Valor = regis.CatTestTypes.description + " - " + regis.CatTestResults.description +
                                     (regis.CatVirusTypes.ID <= 0 ? "" : " - " + regis.CatVirusTypes.SPA) + (regis.VirusSubtype_ID == null ? "" : " - " + regis.CatVirusSubTypes.SPA) +
                                     (regis.VirusLinaje_ID == null ? "" : " - " + regis.CatVirusLinaje.SPA)
                         });
            // catalogo = catalogo.OrderBy(s => s.CatTestTypes.description).ThenBy(s => s.CatTestResults.description);

            MultiSelectList oListMS;
            oListMS = new MultiSelectList(oList.ToList(), "ID", "Valor");
            oInCoModel.VirusFlowCollection = oListMS;

            //****
            PopulateDepartmentsDropDownList();

            //****
            //return View();
            return View(oInCoModel);
        }

        // POST: CatInstitucionConf/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "InstitutionFromID, InstitutionToID, InstitutionParentID, Priority, Conclusion, OpenAlways, VirusFlowSelected")] InstitutionConfiguration catalog)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.InstitutionsConfiguration.Add(catalog);
                    db.SaveChanges();

                    InstitutionConfFlowByVirusSaveUpdate(null, catalog.VirusFlowSelected, catalog, 1);

                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException dex)
            {
                ModelState.AddModelError("", "No es posible guardar los datos. Intente de nuevo, si el problema persiste contacte al administrador." + "\n" + dex.Message);
            }
            return View(catalog);
        }

        // GET: CatInstitucionConf/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var catalogo = db.InstitutionsConfiguration.Find(id);
            if (catalogo == null)
            {
                return HttpNotFound();
            }

            PopulateDepartmentsDropDownList(catalogo.InstitutionFromID, catalogo.InstitutionToID, catalogo.InstitutionParentID);
            //return View(catalogo);
            //***
            InstitutionConfiguration oInCoModel = new InstitutionConfiguration();

            var oList = (from regis in db.CatViruFlows as IQueryable<CatVirusFlow>
                         orderby regis.CatTestTypes.description, regis.CatTestResults.description
                         select new
                         {
                             ID = regis.ID.ToString(),
                             Valor = regis.CatTestTypes.description + " - " + regis.CatTestResults.description +
                                     (regis.CatVirusTypes.ID <= 0 ? "" : " - " + regis.CatVirusTypes.SPA) + (regis.VirusSubtype_ID == null ? "" : " - " + regis.CatVirusSubTypes.SPA) +
                                     (regis.VirusLinaje_ID == null ? "" : " - " + regis.CatVirusLinaje.SPA)
                         });

            MultiSelectList oListMS;
            var aSele = db.InstitutionConfFlowByVirus.Where(m => m.InstConf_ID == id).ToList();

            if (aSele != null)
            {
                int[] aSelected = new int[aSele.Count];

                for (int i = 0; i < aSele.Count; i++)
                {
                    aSelected[i] = (int)aSele[i].VirusFlow_ID;
                }

                oListMS = new MultiSelectList(oList.ToList(), "ID", "Valor", aSelected);
            }
            else
            {
                oListMS = new MultiSelectList(oList.ToList(), "ID", "Valor");
            }

            oInCoModel.VirusFlowCollection = oListMS;
            oInCoModel.Priority = catalogo.Priority;
            oInCoModel.Conclusion = catalogo.Conclusion;
            oInCoModel.OpenAlways = catalogo.OpenAlways;

            return View(oInCoModel);
        }

        // POST: CatInstitucionConf/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        //public ActionResult EditPost(int? id, long? InstitutionFromID, long? InstitutionToID, long? InstitutionParentID , int Priority, bool Conclusion, bool OpenAlways)
        public ActionResult EditPost(InstitutionConfiguration oInstConf)
        {
            int? id = (Int32)oInstConf.ID;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var count_records_exist = db.InstitutionsConfiguration.Where(z => z.InstitutionParentID == oInstConf.InstitutionParentID && z.InstitutionToID == oInstConf.InstitutionToID && z.InstitutionFromID == oInstConf.InstitutionFromID && z.ID != id).Count();

            var catalog = db.InstitutionsConfiguration.Find(id);
            var InstitutionFromID_original = catalog.InstitutionFromID;
            var InstitutionToID_original = catalog.InstitutionToID;
            var InstitutionParentID_original = catalog.InstitutionParentID;
            var Priority_original = catalog.Priority;
            var Conclusion_original = catalog.Conclusion;
            var OpenAlways_original = catalog.OpenAlways;

            if (count_records_exist >= 1)
            {
                ModelState.AddModelError("", "No es posible guardar los datos. El flujo se encuentra repetido.");
                return View(catalog);
            }

            if (TryUpdateModel(catalog, "",
               new string[] { "InstitutionFromID","InstitutionToID","InstitutionParentID","Priority","Conclusion", "OpenAlways" }))
            {
                try
                {
                    db.SaveChanges();
                    InstitutionConfFlowByVirusSaveUpdate(id, oInstConf.VirusFlowSelected, oInstConf, 2);

                    var consStringFlowCorrection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    using (var conFlowCorrection = new SqlConnection(consStringFlowCorrection))
                    {
                        using (var commandFlowCorrection = new SqlCommand("FlowCorrection", conFlowCorrection) { CommandType = CommandType.StoredProcedure })
                        {
                            var user = UserManager.FindById(User.Identity.GetUserId());
                            commandFlowCorrection.Parameters.Clear();
                            commandFlowCorrection.Parameters.Add("@InstitutionParentID", SqlDbType.Int).Value = catalog.InstitutionParentID;
                            commandFlowCorrection.Parameters.Add("@InstitutionToID", SqlDbType.Int).Value = catalog.InstitutionToID;
                            //commandImportLog.Parameters.Add("@Country_ID", SqlDbType.Int).Value = user.Institution.CountryID;
                            var returnParameter = commandFlowCorrection.Parameters.Add("@ReturnVal", SqlDbType.Int);
                            returnParameter.Direction = ParameterDirection.ReturnValue;

                            conFlowCorrection.Open();

                            using (var reader2 = commandFlowCorrection.ExecuteReader())
                            {
                                if (returnParameter.Value.ToString() == "1")
                                {
                                    ViewBag.Message = $"Correctamente ejecutado.";
                                }
                            }
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "No es posible guardar los datos. Intente de nuevo, si el problema persiste contacte al administrador.");
                }
            }
            return View(catalog);
        }

        // GET: CatInstitucionConf/Delete/5
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
            var catalogo = db.InstitutionsConfiguration.Find(id);
            if (catalogo == null)
            {
                return HttpNotFound();
            }

            PopulateDepartmentsDropDownList(catalogo.InstitutionFromID, catalogo.InstitutionToID, catalogo.InstitutionParentID);

            return View(catalogo);
        }

        // POST: CatAgeGroup/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Delete(int id)
        public ActionResult Delete(InstitutionConfiguration oInstConf)
        {
            long? id = 0;

            try
            {
                id = oInstConf.ID;
                InstitutionConfFlowByVirusSaveUpdate(id, null, oInstConf, 3);

                var catalog = db.InstitutionsConfiguration.Find(id);
                db.InstitutionsConfiguration.Remove(catalog);
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

        private void PopulateDepartmentsDropDownList(
            object selectedFrom = null,
            object selectedTo = null,
            object selectedParent = null)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var countryId = user.Institution.CountryID ?? 0;

            var instQuery = db.Institutions.OfType<Institution>().Where(x => x.CountryID == countryId).OrderBy(x => x.Name).ToList();
            ViewBag.InstitutionFromID = new SelectList(instQuery, "ID", "Name", selectedFrom);
            ViewBag.InstitutionToID = new SelectList(instQuery, "ID", "Name", selectedTo);
            ViewBag.InstitutionParentID = new SelectList(instQuery, "ID", "Name", selectedParent);

            if (selectedParent != null)
            {
                int IDParent = Convert.ToInt32(selectedParent);
                var instParent = db.Institutions.OfType<Institution>().Where(x => x.CountryID == countryId && x.ID == IDParent).FirstOrDefault();
                ViewBag.ParentActive = instParent.Active;
            } 
        }


        private void InstitutionConfFlowByVirusSaveUpdate(long? IdInstConf, int[] IdVirusFlow, InstitutionConfiguration catalog, int operacion)
        {
            try
            {
                if (operacion == 2 || operacion == 3)               // Edit, Delete
                {
                    //**** Eliminando de InstitutionConfEndFlowByVirus
                    var instConfEFV = db.InstitutionConfEndFlowByVirus.Where(j => j.id_InstCnf == IdInstConf).ToList();
                    if (instConfEFV != null)
                    {
                        foreach (var item in instConfEFV)
                        {
                            db.Entry(item).State = EntityState.Deleted;
                            db.InstitutionConfEndFlowByVirus.Remove(item);
                            db.SaveChanges();
                        }
                    }

                    // Eliminando de: InstitutionConfFlowByVirus
                    var instConfFV = db.InstitutionConfFlowByVirus.Where(j => j.InstConf_ID == IdInstConf).ToList();
                    if (instConfFV != null)
                    {
                        foreach (var item in instConfFV)
                        {
                            db.Entry(item).State = EntityState.Deleted;
                            db.InstitutionConfFlowByVirus.Remove(item);
                            db.SaveChanges();
                        }
                    }
                }

                if (operacion == 1 || operacion == 2)       // Create, Edit
                {
                    if (operacion == 1)                     // Create
                    {
                        //var instconf = db.InstitutionsConfiguration.OrderBy(x => x.ID).LastOrDefault();
                        var instconf = db.InstitutionsConfiguration.OrderByDescending(x => x.ID).FirstOrDefault();
                        if (instconf != null)
                        {
                            IdInstConf = instconf.ID;
                        }
                    }

                    foreach (var item in IdVirusFlow)
                    {
                        InstitutionConfFlowByVirus oElement = new InstitutionConfFlowByVirus();

                        oElement.InstConf_ID = IdInstConf;
                        oElement.VirusFlow_ID = item;
                        db.Entry(oElement).State = EntityState.Added;
                        db.SaveChanges();
                        oElement = null;
                    }

                    InstitutionConfFlowByENDVirusSaveUpdate(IdInstConf, catalog);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void InstitutionConfFlowByENDVirusSaveUpdate(long? IdInstConf, InstitutionConfiguration catalog)
        {
            int[] IdVirusFlow = catalog.VirusFlowSelected;

            foreach (var item in IdVirusFlow)
            {
                var regCVF = db.CatViruFlows.Where(x => x.ID == item).FirstOrDefault();
                if (regCVF != null)
                {
                    InstitutionConfEndFlowByVirus oElement = new InstitutionConfEndFlowByVirus();

                    oElement.id_InstCnf = (long)IdInstConf;
                    oElement.id_Lab = (long)catalog.InstitutionToID;
                    oElement.id_priority_flow = catalog.Priority;

                    oElement.id_Cat_TestType = regCVF.TestType_ID;
                    oElement.value_Cat_TestResult = regCVF.TestResult_ID;
                    oElement.id_Cat_VirusType = regCVF.VirusType_ID;
                    oElement.id_Cat_Subtype = regCVF.VirusSubtype_ID;
                    //oElement.id_InstCnfFlowByVirus = item;

                    db.Entry(oElement).State = EntityState.Added;
                    db.SaveChanges();

                    oElement = null;
                }
            } // END foreach
        }
    }
}
