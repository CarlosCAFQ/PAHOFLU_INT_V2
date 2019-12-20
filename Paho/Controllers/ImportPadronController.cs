using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
//-------------------
using Microsoft.AspNet.Identity;
/*/------------------
using Paho.Models;

using Paho.Reports.Entities;
using System.Drawing;
using System.Net;
using System.Data.Entity;
*/

namespace Paho.Controllers {
    [Authorize(Roles = "Admin, Staff")]
    public class ImportPadronController : ControllerBase {
        // GET: ImportPadron
        public ActionResult Index() {
            return View();
        }

        public void LoadDirectDB()
        {

            var consStringDB_INCIENSA = ConfigurationManager.ConnectionStrings["INCIENSA"].ConnectionString;
            using (SqlConnection conServerRemote = new SqlConnection(consStringDB_INCIENSA))
            {
                conServerRemote.Open();

                string selstr = "select * from dbo.SINTER_VIRresp";
                SqlCommand cmd = new SqlCommand(selstr, conServerRemote);
                //SqlParameter name = cmd.Parameters.Add("@name", SqlDbType.NVarChar, 15);
                //name.Value = "Tang";
                SqlDataReader rdr = cmd.ExecuteReader();

                var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (var con = new SqlConnection(consString))
                {
                    using (var sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        sqlBulkCopy.DestinationTableName = "dbo.ImportLab";
                        con.Open();
                        sqlBulkCopy.WriteToServer(rdr);
                        con.Close();
                    }
                }

                //Archivo de exportacion de resultados y ejecucion de StoreProcedure

                var ms = new MemoryStream();
                using (var fs = System.IO.File.OpenRead(ConfigurationManager.AppSettings["ImportTemplate"]
                .Replace("{countryId}", "9")
                ))

                using (var excelPackage = new ExcelPackage(fs))
                {
                    var excelWorkBook = excelPackage.Workbook;
                    int startColumn = 1;
                    int startRow = 3;
                    bool insertRow = true;
                    string Report = "ImportLab_CR";

                    AppendDataToExcel(9, excelWorkBook, Report, startRow, startColumn, 1, insertRow);

                    excelPackage.SaveAs(ms);

                    //implementación para el guardado del archivo
                    FileInfo notImportedFile = new FileInfo(ConfigurationManager.AppSettings["ImportFailedFolder"] + User.Identity.Name + "_" + DateTime.Now.ToString("yyyyMMddhhmmsst") + "_" + Request.Files["file"]?.FileName + ".XLSX");
                    FileStream aFile = new FileStream(notImportedFile.FullName, FileMode.Create);
                    aFile.Seek(0, SeekOrigin.Begin);

                    ExcelPackage excelNotImported = new ExcelPackage();

                    excelNotImported.Load(ms);

                    //excelNotImported.SaveAs(notImportedFile);
                    excelNotImported.SaveAs(aFile);
                    aFile.Close();


                    //hacer inserción en la base de datos, bitácora de subidas
                    var consStringLogImport = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    using (var conImportLog = new SqlConnection(consStringLogImport))
                    {
                        using (var commandImportLog = new SqlCommand("importLogSP", conImportLog) { CommandType = CommandType.StoredProcedure })
                        {
                            var user = UserManager.FindById(User.Identity.GetUserId());
                            commandImportLog.Parameters.Clear();
                            commandImportLog.Parameters.Add("@Fecha_Import", SqlDbType.DateTime).Value = DateTime.Now.ToString();
                            commandImportLog.Parameters.Add("@User_Import", SqlDbType.NVarChar).Value = User.Identity.Name;
                            commandImportLog.Parameters.Add("@Country_ID", SqlDbType.Int).Value = user.Institution.CountryID;
                            //commandImportLog.Parameters.Add("@Filename", SqlDbType.NVarChar).Value = ConfigurationManager.AppSettings["ImportFailedFolder"] + User.Identity.Name + "_" + DateTime.UtcNow.ToString("yyyyMMddhhmmsst") + "_" + Request.Files["file"]?.FileName;
                            commandImportLog.Parameters.Add("@Filename", SqlDbType.NVarChar).Value = notImportedFile.FullName;
                            var returnParameter = commandImportLog.Parameters.Add("@ReturnVal", SqlDbType.Int);
                            returnParameter.Direction = ParameterDirection.ReturnValue;


                            conImportLog.Open();

                            using (var reader2 = commandImportLog.ExecuteReader())
                            {
                                if (returnParameter.Value.ToString() == "1")
                                {
                                    ViewBag.Message = $"Archivo registrado en al bitácora.";
                                }
                            }
                        }
                    }
                    //fin de inserción
                }
                ms.Position = 0;
                //ViewBag.Message = $"Archivo trabajado correctamente!";
                //ViewBag.Message = "Archivo procesado. En la lista inferior podrá ver la lista de los registros que no fueron importados. Lo podrá localizar por la hora de subida y por su usuario. Puede hacer clic en la flecha azul para descargarlo.";
                //return View();
                //return new FileStreamResult(ms, "application/xlsx")
                //{
                //    FileDownloadName = "NoInsertados.xlsx"
                //};

            }
        }


        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file) {
            try {
                //var fileLocation = Server.MapPath("~/Content/") + DateTime.UtcNow.ToString("yyyyMMddhhmmsst") + Request.Files["file"]?.FileName;
                //var fileLocation = "c:\\ImportTemp\\" + DateTime.UtcNow.ToString("yyyyMMddhhmmsst") + Request.Files["file"]?.FileName;
                var fileLocation = ConfigurationManager.AppSettings["ImportTempFolder"] + DateTime.UtcNow.ToString("yyyyMMddhhmmsst") + Request.Files["file"]?.FileName;             
                if (System.IO.File.Exists(fileLocation)) {
                    System.IO.File.Delete(fileLocation);
                }
                Request.Files["file"]?.SaveAs(fileLocation);

                var conString = string.Empty;
                var extension = Path.GetExtension(Request.Files["file"]?.FileName);

                extension = extension.ToUpper();

                switch (extension) {
                    case ".XLS":
                        conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                        break;
                    case ".XLSX":
                        conString = ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString;
                        break;
                }

                //var storeProcedureMessage = "";
                conString = string.Format(conString, fileLocation);
                using (var excelCon = new OleDbConnection(conString)) {
                    excelCon.Open();
                    var sheet1 = excelCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null)?.Rows[0]["TABLE_NAME"].ToString();
                    if ((bool) !sheet1?.Contains("$")) sheet1 = sheet1 + "$";
                    var dtExcelData = new DataTable();

                    using (var oda = new OleDbDataAdapter($"SELECT * FROM [{sheet1}]", excelCon)) {
                        oda.Fill(dtExcelData);
                    }
                    excelCon.Close();

                    // Insert en el la DB lo enviado en el archivo
                    var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    using (var con = new SqlConnection(consString)) {
                        using (var sqlBulkCopy = new SqlBulkCopy(con)) {
                            sqlBulkCopy.DestinationTableName = "dbo.CatPadronCR_Temp";
                            con.Open();
                            sqlBulkCopy.WriteToServer(dtExcelData);
                            con.Close();
                        }



                        // Ejecutar el proceso almacenado en la DB

                        using (var command = new SqlCommand("CatPadronImport", con) { CommandType = CommandType.StoredProcedure })
                        {
                            //var returnParameter = command.Parameters.Add("@result", SqlDbType.VarChar);
                            //returnParameter.Direction = ParameterDirection.ReturnValue;

                            con.Open();
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var i = 0;
                                    i = reader.FieldCount;
                                }
                            }

                            //command.ExecuteNonQuery();
                            //storeProcedureMessage = (string) returnParameter.Value;
                            con.Close();
                        }
                    }
                }

                //Archivo de exportacion de resultados y ejecucion de StoreProcedure

                //var ms = new MemoryStream();
                //using (var fs = System.IO.File.OpenRead(ConfigurationManager.AppSettings["ImportTemplate"]
                //.Replace("{countryId}", "9")
                //))

                //using (var excelPackage = new ExcelPackage(fs))
                //{
                //    var excelWorkBook = excelPackage.Workbook;
                //    int startColumn = 1;
                //    int startRow = 3;
                //    bool insertRow = true;
                //    string Report = "ImportLab_CR";

                //    AppendDataToExcel(9, excelWorkBook, Report, startRow, startColumn, 1, insertRow);

                //    excelPackage.SaveAs(ms);

                //    //implementación para el guardado del archivo
                //    FileInfo notImportedFile = new FileInfo(ConfigurationManager.AppSettings["ImportFailedFolder"] + User.Identity.Name + "_" + DateTime.Now.ToString("yyyyMMddhhmmsst") + "_" + Request.Files["file"]?.FileName + ".XLSX");
                //    FileStream aFile = new FileStream(notImportedFile.FullName, FileMode.Create);
                //    aFile.Seek(0, SeekOrigin.Begin);

                //    ExcelPackage excelNotImported = new ExcelPackage();

                //    excelNotImported.Load(ms);

                //    //excelNotImported.SaveAs(notImportedFile);
                //    excelNotImported.SaveAs(aFile);
                //    aFile.Close();


                //hacer inserción en la base de datos, bitácora de subidas
                var consStringLogImport = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (var conImportLog = new SqlConnection(consStringLogImport))
                {
                    using (var commandImportLog = new SqlCommand("CatPadronImportLogSP", conImportLog) { CommandType = CommandType.StoredProcedure })
                    {
                        var user = UserManager.FindById(User.Identity.GetUserId());
                        commandImportLog.Parameters.Clear();
                        commandImportLog.Parameters.Add("@Fecha_Import", SqlDbType.DateTime).Value = DateTime.Now.ToString();
                        commandImportLog.Parameters.Add("@User_Import", SqlDbType.NVarChar).Value = User.Identity.Name;
                        commandImportLog.Parameters.Add("@Country_ID", SqlDbType.Int).Value = user.Institution.CountryID;
                        //commandImportLog.Parameters.Add("@Filename", SqlDbType.NVarChar).Value = ConfigurationManager.AppSettings["ImportFailedFolder"] + User.Identity.Name + "_" + DateTime.UtcNow.ToString("yyyyMMddhhmmsst") + "_" + Request.Files["file"]?.FileName;
                        commandImportLog.Parameters.Add("@Filename", SqlDbType.NVarChar).Value = fileLocation;
                        var returnParameter = commandImportLog.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;


                        conImportLog.Open();

                        using (var reader2 = commandImportLog.ExecuteReader())
                        {
                            if (returnParameter.Value.ToString() == "1")
                            {
                                ViewBag.Message = $"Archivo registrado en al bitácora.";
                            }
                        }
                    }
                }
                //fin de inserción
                //}
                //ms.Position = 0;                
                ViewBag.Message = $"Archivo trabajado correctamente!";
                ViewBag.Message = "Archivo procesado correctamente";
                return View();
                ////return new FileStreamResult(ms, "application/xlsx")
                //{
                //    FileDownloadName = "NoInsertados.xlsx"
                //};
                
                //{ storeProcedureMessage}
            } catch (Exception e) {
                ViewBag.Message = "El archivo no se pudo cargar, por favor, compruebe el archivo -  " + e.Message  ;
            }

            return View();
        }
        private static void AppendDataToExcel(int countryId, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row)
        {

            var excelWorksheet = excelWorkBook.Worksheets[1];
            var row = startRow;
            var column = startColumn;

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand(storedProcedure, con) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Clear();
                    command.CommandTimeout = 180;
                    //command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    //command.Parameters.Add("@Languaje", SqlDbType.Text).Value = "SPA";
                    //command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                    //command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    //command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                    //command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                    //command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                    //command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var col = column;
                            if (row > startRow && insert_row == true) excelWorksheet.InsertRow(row, 1);

                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var cell = excelWorksheet.Cells[startRow, col];
                                if (reader.GetValue(i) != null)
                                {
                                    int number = 0;
                                    //bool isNumber = int.TryParse(reader.GetValue(i).ToString(), out number);
                                    bool isNumber = false;

                                    DateTime dt;
                                    bool isDate = DateTime.TryParse(reader.GetValue(i).ToString(), out dt);

                                    if (isNumber)
                                    {
                                        excelWorksheet.Cells[row, col].Value = number;
                                    }
                                    else
                                    {
                                        if (isDate)
                                        {
                                            excelWorksheet.Cells[row, col].Value = dt;
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[row, col].Value = reader.GetValue(i).ToString();
                                        }
                                    }
                                    excelWorksheet.Cells[row, col].StyleID = cell.StyleID;
                                }
                                col++;
                            }

                            row++;
                        }
                    }
                    command.Parameters.Clear();
                    con.Close();

                }
            }

            // Apply only if it has a Total row at the end and hast SUM in range, i.e. SUM(A1:A4)
            //excelWorksheet.DeleteRow(row, 2);
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