using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Paho.Utility
{
    public class PAHOClassUtilities
    {
        public static DateTime fechaInicioPrimeraSemanaEpidemiologica(int nYear)
        {
            DateTime vFeIn, vTemp, vSaba;
            //****
            vTemp = new DateTime(nYear, 1, 1);                  // 1er día del anio
            int nWeDa = (int)vTemp.DayOfWeek + 1;               // Domingo 1er día   
            vSaba = vTemp.AddDays(7 - nWeDa);                   // 1er sábado

            int nDife = (int)vSaba.Subtract(vTemp).TotalDays;
            if (nDife >= 3)
                vFeIn = vSaba.AddDays(-(7 - 1));
            else
                vFeIn = vSaba.AddDays(1);
            //****
            return vFeIn;
        }

        public static int semanasAnioEpidemiologico(int nYear)
        {
            DateTime dtDiaUno, dtDiaUnoNext, dtTemp;
            int numSema = 0;

            //****
            dtDiaUno = fechaInicioPrimeraSemanaEpidemiologica(nYear);
            dtDiaUnoNext = fechaInicioPrimeraSemanaEpidemiologica(nYear + 1);
            dtTemp = dtDiaUno;

            while (dtTemp < dtDiaUnoNext)
            {
                ++numSema;
                dtTemp = dtTemp.AddDays(7);
            }
            //****
            return numSema;
        }

        public static int semanasActualEpidemiologico()
        {
            DateTime dtCurr, dtTemp1, dtTemp2;
            int numSema = 0;
            //****
            dtCurr = DateTime.Today;
            dtTemp1 = fechaInicioPrimeraSemanaEpidemiologica(dtCurr.Year);
            dtTemp2 = dtTemp1.AddDays(7);

            do
            {
                ++numSema;
                dtTemp1 = dtTemp1.AddDays(7);
                dtTemp2 = dtTemp2.AddDays(7);
            } while (!(dtCurr >= dtTemp1 && dtCurr <= dtTemp2));
            //****
            return numSema;
        }

        public static int NumeroActualSE()
        {
            int nSema = 1;
            DateTime dtHoy = DateTime.Today;
            DateTime dtInicio = fechaInicioPrimeraSemanaEpidemiologica(dtHoy.Year);
            DateTime dtFinal = dtInicio.AddDays(6);

            while (!(dtHoy >= dtInicio && dtHoy <= dtFinal))
            {
                ++nSema;
                dtInicio = dtInicio.AddDays(7);
                dtFinal = dtInicio.AddDays(6);
            }

            return nSema;
        }

        public static int getNumberAgeGroupCountry(int CountryID)
        {
            int nCaAG = 0;
            //****
            string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(connString))
            {
                using (var command = new SqlCommand("SELECT COUNT(*) FROM CatAgeGroup WHERE id_country = " + CountryID.ToString(), con))
                {
                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            nCaAG = (int)reader.GetValue(0);
                        }
                    }
                }
            }
            //****
            return nCaAG;
        }
    }
}