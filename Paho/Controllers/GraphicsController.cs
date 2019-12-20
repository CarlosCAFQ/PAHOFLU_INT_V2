using System;
//using System.Collections.Generic;
using System.Configuration;
//using System.IO;
using System.Linq;
using System.Data;
using System.Text;
//using System.Web;
using System.Web.Mvc;
using Paho.Reports.Entities;
using ServiceStack.Text;
using System.Data.SqlClient;

namespace Paho.Controllers
{
    [Authorize(Roles = "Admin, Staff, Reader, Report, Tester")]
    public  class GraphicsController : ControllerBase
    {
        // GET: Graphics
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GenerateGraphic(string Report, int? CountryID, int? HospitalID, int? Year, int? Month, int? SE, DateTime? StartDate, DateTime? EndDate) {
            try {
                //var query = new Example();
                //var result = db.Database.SqlQuery<Example>(query.Query()).ToList();
                //var csv = result.ToCsv();
                var csv = CountryID.ToCsv();
                // Store Procedure
                var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (var con = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand(Report, con) { CommandType = CommandType.StoredProcedure })
                    {
                        DateTime AnioReport =  DateTime.Now;
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = CountryID;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = HospitalID;
                        command.Parameters.Add("@Languaje", SqlDbType.NVarChar).Value = "SPA";
                        command.Parameters.Add("@Year_case", SqlDbType.Int).Value = AnioReport.Year;
                        command.Parameters.Add("@Graph", SqlDbType.Int).Value = 1;


                        //command.Parameters.Add("@year", SqlDbType.Int).Value = year;
                        //command.Parameters.Add("@hospitald", SqlDbType.Int).Value = 53;
                        //command.Parameters.Add("@weekFrom", SqlDbType.Int).Value = weekFrom;
                        //command.Parameters.Add("@weekTo", SqlDbType.Int).Value = weekTo;
                        con.Open();
                        DataTable dt = new DataTable();
                        dt.Load(command.ExecuteReader());
                        var pruebacsv = dt.Select().ToList();
                        csv = ToCSV(dt, ",", true, false);
                        command.Parameters.Clear();
                        con.Close();

                    }
                }
                // End Store Procedure


                var filePath = ConfigurationManager.AppSettings["GraphicsPath"] + CountryID.ToString() + Report  + "Graph.csv";
                System.IO.File.WriteAllText(filePath, csv);

                return Json("Success", JsonRequestBehavior.AllowGet);
            } catch (Exception e) {
                Console.Error.WriteLine(string.Format("{0} | {1} | {2}", e.Message, e.StackTrace, e.InnerException));
                return Json("Coulnd't generate file", JsonRequestBehavior.AllowGet);
            }
        }

       

        private static string ToCSV(DataTable table, string delimiter, bool includeHeader, bool doublequote)
        {
            var result = new StringBuilder();

            if (includeHeader)
            {
                foreach (DataColumn column in table.Columns)
                {
                    result.Append(column.ColumnName);
                    result.Append(delimiter);
                }

                result.Remove(--result.Length, 0);
                result.Append(Environment.NewLine);
            }

            foreach (DataRow row in table.Rows)
            {
                foreach (object item in row.ItemArray)
                {
                    if (item is DBNull)
                        result.Append(delimiter);
                    else
                    {
                        string itemAsString = item.ToString();
                        // Double up all embedded double quotes
                        itemAsString = itemAsString.Replace("\"", "\"\"");

                        // To keep things simple, always delimit with double-quotes
                        // so we don't have to determine in which cases they're necessary
                        // and which cases they're not.
                        if (doublequote == true)
                            itemAsString = "\"" + itemAsString + "\"";

                        result.Append(itemAsString + delimiter);
                    }
                }

                result.Remove(--result.Length, 0);
                result.Append(Environment.NewLine);
            }

            return result.ToString();
        }


    }
}