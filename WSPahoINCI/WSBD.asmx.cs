using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WSPahoINCI
{
    /// <summary>
    /// Descripción breve de WSBD
    /// </summary>
    //[WebService(Namespace = "http://tempuri.org/")]
    [WebService(Namespace = "http://wscr.pahoflu.com/webservices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class WSBD : System.Web.Services.WebService
    {
        [WebMethod]
        public string GetDaPI(int serverSQL, string cKey)
        {
            string cKeyValidate = "W~(4n-Xp@fcJRVV3gepmYeU3=8Rrg3C,{RUvmXH6shWT48;P";

            if (cKey != cKeyValidate)
            {
                return "";
            }

            string connectionString;
            if (serverSQL == 1)
                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            else if (serverSQL == 2)
                //connectionString = ConfigurationManager.ConnectionStrings["INCIENSAConnection"].ConnectionString;
                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            else
                //connectionString = ConfigurationManager.ConnectionStrings["CAFQConnection"].ConnectionString;
                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            DataTable dt = new DataTable();

            using (var con = new SqlConnection(connectionString))
            {
                //using (var command = new SqlCommand("SELECT TOP 5 * FROM SINTER_VIRresp", con) { CommandTimeout = 1200 })
                using (var command = new SqlCommand("SELECT * FROM SINTER_VIRresp", con) { CommandTimeout = 1200 })
                {
                    command.Parameters.Clear();
                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        DataColumn col1 = new DataColumn("muestra", typeof(int));
                        DataColumn col2 = new DataColumn("expediente", typeof(string));
                        DataColumn col3 = new DataColumn("cedula", typeof(string));
                        DataColumn col4 = new DataColumn("nombre", typeof(string));
                        DataColumn col5 = new DataColumn("apellido1", typeof(string));
                        DataColumn col6 = new DataColumn("apellido2", typeof(string));
                        DataColumn col7 = new DataColumn("sexo", typeof(string));
                        DataColumn col8 = new DataColumn("fecha_nacimiento", typeof(DateTime));
                        DataColumn col9 = new DataColumn("edad", typeof(int));
                        DataColumn col10 = new DataColumn("dx_presuntivo", typeof(string));

                        DataColumn col11 = new DataColumn("fecha_inicio_sintomas", typeof(DateTime));
                        DataColumn col12 = new DataColumn("fecha_toma_muestra", typeof(DateTime));
                        DataColumn col13 = new DataColumn("fecha_ingreso_inciensa", typeof(DateTime));
                        DataColumn col14 = new DataColumn("tipo_muestra", typeof(string));
                        DataColumn col15 = new DataColumn("dias_evolucion", typeof(int));
                        DataColumn col16 = new DataColumn("analisis", typeof(string));
                        DataColumn col17 = new DataColumn("metodo_tecnica", typeof(string));
                        DataColumn col18 = new DataColumn("resultado_1", typeof(string));
                        DataColumn col19 = new DataColumn("resultado_2", typeof(string));
                        DataColumn col20 = new DataColumn("resultado_3", typeof(string));

                        DataColumn col21 = new DataColumn("fecha_resultado", typeof(DateTime));
                        DataColumn col22 = new DataColumn("region", typeof(string));
                        DataColumn col23 = new DataColumn("provincia", typeof(string));
                        DataColumn col24 = new DataColumn("canton", typeof(string));
                        DataColumn col25 = new DataColumn("distrito", typeof(string));
                        DataColumn col26 = new DataColumn("nombre_establecimiento", typeof(string));
                        DataColumn col27 = new DataColumn("barrio", typeof(string));
                        DataColumn col28 = new DataColumn("otras_senas", typeof(string));
                        DataColumn col29 = new DataColumn("telefono", typeof(string));
                        DataColumn col30 = new DataColumn("numero_boleta", typeof(int));

                        DataColumn col31 = new DataColumn("codigo_referencia", typeof(string));
                        DataColumn col32 = new DataColumn("observacion", typeof(string));
                        DataColumn col33 = new DataColumn("codigo_establecimiento", typeof(string));//*/

                        dt.Columns.Add(col1);
                        dt.Columns.Add(col2);
                        dt.Columns.Add(col3);
                        dt.Columns.Add(col4);
                        dt.Columns.Add(col5);
                        dt.Columns.Add(col6);
                        dt.Columns.Add(col7);
                        dt.Columns.Add(col8);
                        dt.Columns.Add(col9);
                        dt.Columns.Add(col10);

                        dt.Columns.Add(col11);
                        dt.Columns.Add(col12);
                        dt.Columns.Add(col13);
                        dt.Columns.Add(col14);
                        dt.Columns.Add(col15);
                        dt.Columns.Add(col16);
                        dt.Columns.Add(col17);
                        dt.Columns.Add(col18);
                        dt.Columns.Add(col19);
                        dt.Columns.Add(col20);

                        dt.Columns.Add(col21);
                        dt.Columns.Add(col22);
                        dt.Columns.Add(col23);
                        dt.Columns.Add(col24);
                        dt.Columns.Add(col25);
                        dt.Columns.Add(col26);
                        dt.Columns.Add(col27);
                        dt.Columns.Add(col28);
                        dt.Columns.Add(col29);
                        dt.Columns.Add(col30);

                        dt.Columns.Add(col31);
                        dt.Columns.Add(col32);
                        dt.Columns.Add(col33);

                        while (reader.Read())
                        {
                            DataRow newrow = dt.NewRow();

                            //*******************************************//
                            if (!reader.IsDBNull(0))                                    // muestra
                                newrow[0] = Convert.ToInt32(reader.GetValue(0));
                            else
                                newrow[0] = DBNull.Value;
                            if (!reader.IsDBNull(1))                                    // expediente
                                newrow[1] = reader.GetValue(1).ToString();
                            else
                                newrow[1] = DBNull.Value;
                            if (!reader.IsDBNull(2))                                    // cedula
                                newrow[2] = reader.GetValue(2).ToString();
                            else
                                newrow[2] = DBNull.Value;
                            if (!reader.IsDBNull(3))                                    // nombre
                                newrow[3] = reader.GetValue(3).ToString();
                            else
                                newrow[3] = DBNull.Value;
                            if (!reader.IsDBNull(4))                                    // apellido1
                                newrow[4] = reader.GetValue(4).ToString();
                            else
                                newrow[4] = DBNull.Value;
                            if (!reader.IsDBNull(5))                                    // apellido2
                                newrow[5] = reader.GetValue(5).ToString();
                            else
                                newrow[5] = DBNull.Value;
                            if (!reader.IsDBNull(6))                                    // sexo
                                newrow[6] = reader.GetValue(6).ToString();
                            else
                                newrow[6] = DBNull.Value;
                            if (!reader.IsDBNull(7))                                    // fecha_nacimiento
                                newrow[7] = String.Format("{0:yyyy-MM-dd}", reader.GetValue(7));
                            else
                                newrow[7] = DBNull.Value;
                            if (!reader.IsDBNull(8))                                    // Edad
                                newrow[8] = Convert.ToInt32(reader.GetValue(8));
                            else
                                newrow[8] = DBNull.Value;
                            if (!reader.IsDBNull(9))                                    // dx_presuntivo
                                newrow[9] = reader.GetValue(9).ToString();
                            else
                                newrow[9] = DBNull.Value;

                            //*******************************************//
                            if (!reader.IsDBNull(10))                                   // fecha_inicio_sintomas
                                newrow[10] = String.Format("{0:yyyy-MM-dd}", reader.GetValue(10));
                            else
                                newrow[10] = DBNull.Value;
                            if (!reader.IsDBNull(11))                                   // fecha_toma_muestra
                                newrow[11] = String.Format("{0:yyyy-MM-dd}", reader.GetValue(11));
                            else
                                newrow[11] = DBNull.Value;
                            if (!reader.IsDBNull(12))                                   // fecha_ingreso_inciensa
                                newrow[12] = String.Format("{0:yyyy-MM-dd}", reader.GetValue(12));
                            else
                                newrow[12] = DBNull.Value;
                            if (!reader.IsDBNull(13))                                    // tipo_muestra
                                newrow[13] = reader.GetValue(13).ToString();
                            else
                                newrow[13] = DBNull.Value;
                            if (!reader.IsDBNull(14))                                   // dias_evolucion
                                newrow[14] = Convert.ToInt32(reader.GetValue(14));
                            else
                                newrow[14] = DBNull.Value;
                            if (!reader.IsDBNull(15))                                   // analisis
                                newrow[15] = reader.GetValue(15).ToString();
                            else
                                newrow[15] = DBNull.Value;
                            if (!reader.IsDBNull(16))                                   // metodo_tecnica
                                newrow[16] = reader.GetValue(16).ToString();
                            else
                                newrow[16] = DBNull.Value;
                            if (!reader.IsDBNull(17))                                   // resultado_1
                                newrow[17] = reader.GetValue(17).ToString();
                            else
                                newrow[17] = DBNull.Value;
                            if (!reader.IsDBNull(18))                                   // resultado_2
                                newrow[18] = reader.GetValue(18).ToString();
                            else
                                newrow[18] = DBNull.Value;
                            if (!reader.IsDBNull(19))                                   // resultado_3
                                newrow[19] = reader.GetValue(19).ToString();
                            else
                                newrow[19] = DBNull.Value;

                            //*******************************************//
                            if (!reader.IsDBNull(20))                                   // fecha_resultado
                                newrow[20] = String.Format("{0:yyyy-MM-dd}", reader.GetValue(20));
                            else
                                newrow[20] = DBNull.Value;
                            if (!reader.IsDBNull(21))                                   // region
                                newrow[21] = reader.GetValue(21).ToString();
                            else
                                newrow[21] = DBNull.Value;
                            if (!reader.IsDBNull(22))                                   // provincia
                                newrow[22] = reader.GetValue(22).ToString();
                            else
                                newrow[22] = DBNull.Value;
                            if (!reader.IsDBNull(23))                                   // canton
                                newrow[23] = reader.GetValue(23).ToString();
                            else
                                newrow[23] = DBNull.Value;
                            if (!reader.IsDBNull(24))                                   // distrito
                                newrow[24] = reader.GetValue(24).ToString();
                            else
                                newrow[24] = DBNull.Value;
                            if (!reader.IsDBNull(25))                                   // nombre_establecimiento
                                newrow[25] = reader.GetValue(25).ToString();
                            else
                                newrow[25] = DBNull.Value;
                            if (!reader.IsDBNull(26))                                   // barrio
                                newrow[26] = reader.GetValue(26).ToString();
                            else
                                newrow[26] = DBNull.Value;
                            if (!reader.IsDBNull(27))                                   // otras_senas
                                newrow[27] = reader.GetValue(27).ToString();
                            else
                                newrow[27] = DBNull.Value;
                            if (!reader.IsDBNull(28))                                   // telefono
                                newrow[28] = reader.GetValue(28).ToString();
                            else
                                newrow[28] = DBNull.Value;
                            if (!reader.IsDBNull(29))                                   // numero_boleta
                                newrow[29] = Convert.ToInt32(reader.GetValue(29));
                            else
                                newrow[29] = DBNull.Value;

                            //*******************************************//
                            if (!reader.IsDBNull(30))                                   // codigo_referencia
                                newrow[30] = reader.GetValue(30).ToString();
                            else
                                newrow[30] = DBNull.Value;
                            if (!reader.IsDBNull(31))                                   // observacion
                                newrow[31] = reader.GetValue(31).ToString();
                            else
                                newrow[31] = DBNull.Value;
                            if (!reader.IsDBNull(32))                                   // codigo_establecimiento
                                newrow[32] = reader.GetValue(32).ToString();
                            else
                                newrow[32] = DBNull.Value;

                            //*******************************************//
                            dt.Rows.Add(newrow);
                        }
                    }
                }
            }

            string resultat_Json = JsonConvert.SerializeObject(dt, Formatting.Indented);

            return resultat_Json;
        }
    }
}
