using System;

namespace Paho.Reports.Entities
{
    public class Example
    {
        public string ColumnOne { get; set; }
        public string ColumnTwo { get; set; }
        public int ColumnThree { get; set; }
        public DateTime ColumnFour { get; set; }

        public string Query() {
            return "SELECT TOP 10 [ID] as ColumnThree ,[HospitalDate] as ColumnFour ,[FName1] as ColumnOne ,[FName2] as ColumTwo FROM [PahoFlu_CR].[dbo].[FluCase]";
        }
    }
}