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
    [Authorize]
    public class TestCasesController : ControllerBase
    {       
        // GET: VirusTypes
        public JsonResult GetVirusTypes()
      {
          var virustypes = db.VirusTypes as IEnumerable<VirusType>;
          var jsondata =
                  (from virustype in virustypes
                   select new
                   {
                       virustype.ID,
                       virustype.Name,
                   }).ToArray();
         
          return Json(jsondata, JsonRequestBehavior.AllowGet);      
        }

       // GET: TestCases
 
    }
}
