using Paho.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
//using System.Data.Entity;
using System.Net.Mail;
using System.Text;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net.Mime;
using System.IO;

namespace Paho.Controllers
{
    [Authorize]
    public class TicketController : ControllerBase
    {
        // GET: Ticket
        public ActionResult Index()
        {
            var SummaryViewModel = new SummaryViewModel();
            IQueryable<Institution> institutions = null;
            var user = UserManager.FindById(User.Identity.GetUserId());
            var DoS = DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("es-GT"));
            SummaryViewModel.DatePickerConfig = DoS;
            //SummaryViewModel.UsrCtry = user.Institution.CountryID;

            if (user.Institution.AccessLevel == AccessLevel.All)
            {
                SummaryViewModel.DisplayCountries = true;
                SummaryViewModel.DisplayHospitals = true;
            }
            else if (user.Institution is Hospital || user.Institution is AdminInstitution)
            {
                SummaryViewModel.DisplayHospitals = true;

                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Area)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.AreaID == user.Institution.AreaID);
                }
                else
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.ID == user.Institution.ID);
                }
            }


            return View(SummaryViewModel);
        }

        public string CreateTicket(string UsrCountry, string ticketSubject, string ticketMsg, HttpPostedFileBase myfile)
        {
            string ticketResult="";

            string userTicket = User.Identity.GetUserId();
            //---------------------------Para setear los tickets en la base de datos---------------------------------
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                string jsonGraphData = "";
                
                using (var command = new SqlCommand("SetTicket", con) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@subject", SqlDbType.Text).Value = ticketSubject;
                    command.Parameters.Add("@description", SqlDbType.Text).Value = ticketMsg;
                    command.Parameters.Add("@status", SqlDbType.Int).Value = 0;
                    command.Parameters.Add("@ticketDate", SqlDbType.DateTime).Value = DateTime.Now;
                    command.Parameters.Add("@userTicket", SqlDbType.Text).Value = userTicket;
                    command.Parameters.Add("@priority", SqlDbType.Int).Value = 0;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ticketResult = reader["ResultInsert"].ToString().Trim();

                        }
                    }
                }
            }
            //---------------extraccion de direcciones de correo----------
            List<Dictionary<string, string>> emailList = new List<Dictionary<string, string>>();
            using (var con = new SqlConnection(consString))
            {
                string jsonGraphData = "";

                using (var command = new SqlCommand("GetNotifiableSupport", con) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Clear();
                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, string> emailItem = new Dictionary<string, string>();
                            emailItem.Add("name", reader["name"].ToString().Trim());
                            emailItem.Add("email", reader["email"].ToString().Trim());
                            emailItem.Add("notification", reader["notification"].ToString().Trim());
                            emailItem.Add("notificationType", reader["notificationType"].ToString().Trim());
                            emailList.Add(emailItem);
                        }
                    }
                }
            }
            //--------------envio de correo----------------------
            
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 180000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("pahoflu@gmail.com", "Chachas1");

            //MailMessage mm = new MailMessage("pahoflu@gmail.com", mailSupport, ticketSubject, ticketMsg);
            MailMessage mm = new MailMessage();
            MailAddress fromMail = new MailAddress("pahoflu@gmail.com");
            mm.From = fromMail;
            mm.Subject = ticketSubject;
            mm.Body = ticketMsg;
            //------------------
            foreach (var emailObject in emailList)
            {
                switch (emailObject["notificationType"])
                {
                    case "":
                        mm.To.Add(emailObject["email"]);
                        break;
                    case "CC":
                        mm.CC.Add(emailObject["email"]);
                        break;
                    case "BCC":
                        mm.Bcc.Add(emailObject["email"]);
                        break;
                }
            }
            //------------Attachment---------------
            /*var file_ = Request.Files[0];
           
                
                var fileName = Path.GetFileName(file_.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                file_.SaveAs(path);
                Attachment data = new Attachment(path, MediaTypeNames.Application.Octet);
                // your path may look like Server.MapPath("~/file.ABC")
                mm.Attachments.Add(data);*/


            //------------------
            //------------Attachment---------------
            string fileName;
            string path;
            Attachment data;
            for (int i=0; i< Request.Files.Count;i++)
            {
                var file_ = Request.Files[i];
                fileName = Path.GetFileName(file_.FileName);
                path = Path.Combine(ConfigurationManager.AppSettings["UploadDir"], fileName);
                int counterFile = 0;
                while (System.IO.File.Exists(path))
                {
                    path = Path.Combine(ConfigurationManager.AppSettings["UploadDir"], counterFile + fileName);
                    counterFile++;
                }
                file_.SaveAs(path);
                data = new Attachment(path, MediaTypeNames.Application.Octet);
                // your path may look like Server.MapPath("~/file.ABC")
                mm.Attachments.Add(data);
                /*HttpPostedFileBase hpf = Request.Files[file__] as HttpPostedFileBase;
                fileName = Path.GetFileName(hpf.FileName);
                path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                hpf.SaveAs(path);
                data = new Attachment(path, MediaTypeNames.Application.Octet);
                // your path may look like Server.MapPath("~/file.ABC")
                mm.Attachments.Add(data);*/
            }
            

            //------------------
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            client.Send(mm);
            //--------------fin envio correo---------------------

            return (ticketResult);
        }

        public string EditTicket(string ticketID, string UsrCountry, string ticketSubject, string ticketMsg, string ticketStatus, string ticketPriority)
        {
            string ticketResult = "";

            string userTicket = User.Identity.GetUserId();
			if (User.Identity.IsAuthenticated)
			{
            //---------------------------Para setear los tickets en la base de datos---------------------------------
			if (User.IsInRole("Support"))//entran los de soporte porque tienen permisos
            {
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                string jsonGraphData = "";

                using (var command = new SqlCommand("UpdateTicket", con) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@ID", SqlDbType.Int).Value = ticketID;                    
                    command.Parameters.Add("@status", SqlDbType.Int).Value = ticketStatus;
                    command.Parameters.Add("@priority", SqlDbType.Int).Value = ticketPriority;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ticketResult = reader["ResultInsert"].ToString().Trim();

                        }
                    }
                }
            }
			}
			}            

            return (ticketResult);
        }
        public JsonResult GetTicket()
        {
            IdentityDbContext context = new IdentityDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            string rol = "";
            if (!roleManager.RoleExists("Support"))
            {
                rol = "creado";
                // first we create Admin rool   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Support";
                roleManager.Create(role);
            }
            
            /*bool x = _roleManager.RoleExistsAsync("Admin");
            if (!x)
            {

                // first we create Admin rool    
                var role = new IdentityRole();
                role.Name = "Admin";
                _roleManager.CreateAsync(role);
            }*/

            string userTicket = User.Identity.GetUserId();
            //string usertTicketRole = "";
            var roles = ((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
            string GetTicketSP = "";
            string ticketRol = "";
            if (User.Identity.IsAuthenticated)
            {
                if(User.IsInRole("Support")){
                    GetTicketSP = "GetAllTicket";
                    ticketRol = "Support";
                }
                else if (User.IsInRole("Admin"))
                {
                    GetTicketSP = "GetAllTicket";
                    ticketRol = "Admin";
                }
                else
                {
                    GetTicketSP = "GetTicket";
                    ticketRol = "User";
                }
            }
            List<Dictionary<string, string>> ticketList = new List<Dictionary<string, string>>();
            Dictionary<string, string> ticketInfo = new Dictionary<string, string>();
            ticketInfo.Add("rol", ticketRol);            
            ticketList.Add(ticketInfo);
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand(GetTicketSP, con) { CommandType = CommandType.StoredProcedure })
                {
                    if (GetTicketSP == "GetTicket")
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@userTicket", SqlDbType.Text).Value = userTicket;
                    }                
                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, string> ticketItem = new Dictionary<string, string>();
                            ticketItem.Add("ID", reader["ID"].ToString().Trim());
                            ticketItem.Add("subject", reader["subject"].ToString().Trim());
                            ticketItem.Add("description", reader["description"].ToString().Trim());
                            ticketItem.Add("status", reader["status"].ToString().Trim());
                            ticketItem.Add("ticketDate", reader["ticketDate"].ToString().Trim());
                            ticketItem.Add("priority", reader["priority"].ToString().Trim());
                            ticketList.Add(ticketItem);
                        }
                    }
                }
            }
            return Json(ticketList, JsonRequestBehavior.AllowGet);            
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