using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace ExcelExporter.Mvc {
	///<summary>An ActionResult that sends an Excel spreadsheet to the client.</summary>
	public class ExcelResult : FilePathResult {
		static readonly Dictionary<ExcelFormat, string> ContentTypes = new Dictionary<ExcelFormat, string> {
			{ ExcelFormat.Excel2003,		"application/vnd.ms-excel" },
			{ ExcelFormat.Excel2007,		"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
			{ ExcelFormat.Excel2007Binary,	"application/vnd.ms-excel.sheet.binary.macroEnabled.12" },
			{ ExcelFormat.Excel2007Macro,	"application/vnd.ms-excel.sheet.macroEnabled.12" },
		};
		static string GetFileName(string extension) {
			var path = Path.GetTempFileName();
			File.Delete(path);
			return Path.ChangeExtension(path, extension);
		}
		///<summary>Gets the Excel format that will be sent to the client.</summary>
		public ExcelFormat Format { get; private set; }

		///<summary>Creates an ExcelResult that sends an Excel spreadsheet with the specified filename and format.</summary>
		public ExcelResult(string name, ExcelFormat format)	//OleDb fails if the extension doesn't match...
			: base(GetFileName(ExcelExport.GetExtension(format)), ContentTypes[format]) {
			FileDownloadName = name;
			Format = format;
		}

		readonly ExcelExport exporter = new ExcelExport();

		///<summary>Adds a collection of strongly-typed objects to be exported.</summary>
		///<param sheetName="sheetName">The sheetName of the sheet to generate.</param>
		///<param sheetName="items">The rows to export to the sheet.</param>
		///<returns>This instance, to allow chaining.kds</returns>
		public ExcelExport AddSheet<TRow>(string sheetName, IEnumerable<TRow> items) {
			exporter.AddSheet(sheetName, items);
			return this;
		}

		///<summary>Adds the contents of a DataTable instance to be exported, using the table's name as the worksheet name.</summary>
		public ExcelExport AddSheet(DataTable table) {
			if (table == null) throw new ArgumentNullException("table");
			return AddSheet(table.TableName, table);
		}
		///<summary>Adds the contents of a DataTable instance to be exported.</summary>
		public ExcelExport AddSheet(string sheetName, DataTable table) {
			exporter.AddSheet(sheetName, table);
			return this;
		}

		///<summary>Creates the Excel file and sends it to the client.</summary>
		public override void ExecuteResult(ControllerContext context) {
			exporter.ExportTo(FileName, Format);
			base.ExecuteResult(context);
		}
	}
}