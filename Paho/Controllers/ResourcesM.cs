using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Resources.Abstract;
using Resources.Concrete;
using System.Configuration;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace Paho.Controllers
{
    public class ResourcesM : ControllerBase
    {
        private static IResourceProvider resourceProvider = new DbResourceProvider();
        
        /// <summary>Estado caso</summary>
        /// 
        public string getMessage(string msg, int? countryID, string countryLang)
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
                if (dbMessage == "")
                {
                    dbMessage = " ";
                }
            }
            return dbMessage;
        }
        public static string msgCaselistVigTabCase
        {
            get
            {
                return resourceProvider.GetResource("msgCaselistVigTabCase", ConfigurationManager.AppSettings["lang"]) as string;
            }
        }
    }

}

