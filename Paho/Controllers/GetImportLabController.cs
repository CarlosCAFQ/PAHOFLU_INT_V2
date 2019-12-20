using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Paho.Models;
using System.Data.Entity;
using System.Globalization;
using Microsoft.AspNet.Identity;
using System.Web.Services;
using System.IO;
using System.Configuration;
using OfficeOpenXml;



namespace Paho.Controllers
{
    public class GetImportLabController : ControllerBase
    {
        // GET: GetImportLab
        public ActionResult Index(int Country_ID)
        {
            var req = Request;
            //Request.Params
            return View();
        }

        public FileResult GetImportedFile(string importedFileName)
        {
            FileInfo notImportedFile = new FileInfo(ConfigurationManager.AppSettings["ImportFailedFolder"] + importedFileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(notImportedFile.FullName);
            string fileName = importedFileName;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            //return View();
        }

        public FileResult GetImportedFilePadron(string importedFileName)
        {
            FileInfo notImportedFile = new FileInfo(ConfigurationManager.AppSettings["ImportTempFolder"] + importedFileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(notImportedFile.FullName);
            string fileName = importedFileName;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            //return View();
        }

        public ActionResult GetImportedFile2(string importedFileName)
        {
            FileInfo notImportedFile = new FileInfo(ConfigurationManager.AppSettings["ImportFailedFolder"] + importedFileName);
            var fs = System.IO.File.OpenRead(notImportedFile.FullName);
            var ms = new MemoryStream();
            ExcelPackage excelNotImported = new ExcelPackage(fs);
            ms.Position = 0;
            excelNotImported.SaveAs(ms);
            return new FileStreamResult(ms, "text/plain")
            {
                FileDownloadName = importedFileName
            };

            return View();
        }
        public JsonResult GetImportFileList(int Country_ID)
        {            
            List<Dictionary<string, string>> ImportFileList = new List<Dictionary<string, string>>();
            
            IQueryable<ImportLog> ImportedFileList = null;
            //ImportedFileList = db.ImportedFileList.Where(i => i.CountryID == Country_ID);
            var lista = db.ImportedFileList.Where(i => i.Country_ID == Country_ID).OrderByDescending(j => j.Fecha_Import);

            var id = "0";
            var fecha = "1900-1-1";
            var usuario = "n/a";
            var archivo = "";

            if (lista == null)
            {

            }
            else
            {
                //var listaProcesada = lista.ToArray<>;
                //StartDateOfWeek = casesummary.StartDateOfWeek;
                foreach (ImportLog importItem in lista)
                {//casesummaryDetails
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    id = importItem.ID.ToString();
                    fecha = importItem.Fecha_Import.ToString();
                    usuario = importItem.User_Import;
                    archivo = importItem.ImportedFilename;
                    archivo = archivo.Substring(archivo.LastIndexOf("\\") + 1);
                    dictionary.Clear();
                    dictionary.Add("ID", id);
                    dictionary.Add("Fecha", fecha);
                    dictionary.Add("Usuario", usuario);
                    dictionary.Add("Archivo", archivo);
                    ImportFileList.Add(dictionary);
                }
            }

            return Json(ImportFileList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetImportFileList_Padron(int Country_ID)
        {
            List<Dictionary<string, string>> ImportFileList = new List<Dictionary<string, string>>();

            IQueryable<CatPadronCR_ImportLog> ImportedFileList = null;
            //ImportedFileList = db.ImportedFileList.Where(i => i.CountryID == Country_ID);
            var lista = db.ImportedFileListPadron.Where(i => i.Country_ID == Country_ID).OrderByDescending(j => j.Fecha_Import);

            var id = "0";
            var fecha = "1900-1-1";
            var usuario = "n/a";
            var archivo = "";

            if (lista == null)
            {

            }
            else
            {
                //var listaProcesada = lista.ToArray<>;
                //StartDateOfWeek = casesummary.StartDateOfWeek;
                foreach (CatPadronCR_ImportLog importItem in lista)
                {//casesummaryDetails
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    id = importItem.ID.ToString();
                    fecha = importItem.Fecha_Import.ToString();
                    usuario = importItem.User_Import;
                    archivo = importItem.ImportedFilename;
                    archivo = archivo.Substring(archivo.LastIndexOf("\\") + 1);
                    dictionary.Clear();
                    dictionary.Add("ID", id);
                    dictionary.Add("Fecha", fecha);
                    dictionary.Add("Usuario", usuario);
                    dictionary.Add("Archivo", archivo);
                    ImportFileList.Add(dictionary);
                }
            }

            return Json(ImportFileList, JsonRequestBehavior.AllowGet);
        }
    }
}