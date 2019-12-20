using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Paho.Controllers
{
    [Authorize]
    public class LineaBasalController : Controller
    {
        // GET: LineaBasal
        public ActionResult Index()
        {
            return View();
        }
    }
}