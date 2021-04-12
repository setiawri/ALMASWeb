using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using LIBUtil;
using LIBWebMVC;
using LIBExcel;


namespace ALMASKITEWeb.Controllers
{
    public class HomeController : Controller
    {
        List<SelectListItem> ReportList = new List<SelectListItem> {
                new SelectListItem { Value = "DWSystem.ReportA_get", Text = "A. Pemasukan bahan baku"},
                new SelectListItem { Value = "DWSystem.ReportB_get", Text = "B. Pemakaian bahan baku"},
                new SelectListItem { Value = "DWSystem.ReportC_get", Text = "C. Pemakaian barang dalam proses dalam rangka kegiatan subkontrak"},
                new SelectListItem { Value = "DWSystem.ReportD_get", Text = "D. Pemasukan hasil produksi"},
                new SelectListItem { Value = "DWSystem.ReportE_get", Text = "E. Pengeluaran hasil produksi (PEB)"},
                new SelectListItem { Value = "DWSystem.ReportF_get", Text = "F. Mutasi bahan baku"},
                new SelectListItem { Value = "DWSystem.ReportG_get", Text = "G. Mutasi hasil produksi"},
                new SelectListItem { Value = "DWSystem.ReportH_get", Text = "H. Penyelesaian waste / scrap"}
            };

        /* INDEX **********************************************************************************************************************************************/

        public ActionResult Index()
        {
            ViewBag.ReportList = ReportList;
            return View();
        }

        // POST: Home
        [HttpPost]
        public ActionResult Index(string Reports, string NoPIB, bool chkPIBPeriod, DateTime? dtPIBPeriodStart, DateTime? dtPIBPeriodEnd, bool? Landscape)
        {
            if (string.IsNullOrEmpty(NoPIB) && !chkPIBPeriod)
            {
                UtilWebMVC.setBootboxMessage(this, "Silahkan isi No PIB atau Periode PIB");
                ViewBag.ReportList = ReportList;
            }
            else
            {
                string title = ReportList.Where(x => x.Value == Reports).FirstOrDefault().Text;
                string filename = string.Format("Laporan {0} {1:yyyy-MM-dd}.xlsx", title.Substring(0,1), DateTime.Now);
                string filter = getFilter(NoPIB, chkPIBPeriod, dtPIBPeriodStart, dtPIBPeriodEnd);

                DataTable datatable = get(Reports, NoPIB, chkPIBPeriod ? dtPIBPeriodStart : null, chkPIBPeriod ? dtPIBPeriodEnd : null);

                ViewBag.Title = title;
                ViewBag.Filter = filter;

                return Excel.GenerateExcelReport(filename, CompileExcelPackage(datatable, title, filter, Landscape));
            }

            return View();
        }

        /* PRINT **********************************************************************************************************************************************/

        public ActionResult Print(string Reports, string NoPIB, bool chkPIBPeriod, DateTime? dtPIBPeriodStart, DateTime? dtPIBPeriodEnd, bool? Landscape)
        {
            if (string.IsNullOrEmpty(Reports))
                return RedirectToAction(nameof(Index));

            string title = getTitle(Reports);
            string filename = getFilename(title, "xlsx");
            string filter = getFilter(NoPIB, chkPIBPeriod, dtPIBPeriodStart, dtPIBPeriodEnd);

            DataTable datatable = get(Reports, NoPIB, chkPIBPeriod ? dtPIBPeriodStart : null, chkPIBPeriod ? dtPIBPeriodEnd : null);

            ViewBag.Title = title;
            ViewBag.Filter = filter;
            return View(datatable);
        }

        public ActionResult PrintToPdf(string Reports, string NoPIB, bool chkPIBPeriod, DateTime? dtPIBPeriodStart, bool chkPIBPeriodEnd, DateTime? dtPIBPeriodEnd, bool? Landscape)
        {
            string title = getTitle(Reports);
            string filename = getFilename(title, "pdf");

            Rotativa.Options.Orientation orientation;
            if (Landscape != null && (bool)Landscape)
                orientation = Rotativa.Options.Orientation.Landscape;
            else
                orientation = Rotativa.Options.Orientation.Portrait;

            return new Rotativa.ActionAsPdf(nameof(Print), new { Reports = Reports, NoPIB = NoPIB, chkPIBPeriod = chkPIBPeriod, dtPIBPeriodStart = dtPIBPeriodStart, chkPIBPeriodEnd = chkPIBPeriodEnd, dtPIBPeriodEnd = dtPIBPeriodEnd, Landscape = Landscape })
            { FileName = filename, PageOrientation = orientation, PageSize = Rotativa.Options.Size.A4 };
        }

        /* METHODS ********************************************************************************************************************************************/

        private string getTitle(string storedProcedureName)
        {
            return ReportList.Where(x => x.Value == storedProcedureName).FirstOrDefault().Text;
        }

        private string getFilename(string title, string extension)
        {
            return string.Format("Laporan {0} {1:yyyy-MM-dd}.{2}", title.Substring(0, 1), DateTime.Now, extension);
        }

        private string getFilter(string NoPIB, bool chkPIBPeriod, DateTime? dtPIBPeriodStart, DateTime? dtPIBPeriodEnd)
        {
            string filter = "";

            if (!string.IsNullOrEmpty(NoPIB))
                filter = Util.append(filter, "No PIB: " + NoPIB, ",");
            if (chkPIBPeriod)
            {
                filter = Util.append(filter, string.Format("Awal Periode PIB: {0:dd/MM/yy}", dtPIBPeriodStart), ",");
                filter = Util.append(filter, string.Format("Akhir Periode PIB: {0:dd/MM/yy}", dtPIBPeriodEnd), ",");
            }

            return filter;
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public DataTable get(string ReportStoredProcedureName, string PIBNo, DateTime? dtPIBPeriodStart, DateTime? dtPIBPeriodEnd)
        {
            //string sql = @"
            //        SELECT 
            //            0 AS [col A_group 1],
            //            0 AS [col B_group 1],
            //            0 AS [col C],
            //            0 AS colD_group2,
            //            0 AS colE_group2,
            //            0 AS colF_group2
            //    ";
            //return DBConnection.getDataTable("DBContext", sql, false);

            //CREATE PROCEDURE[DWSystem].[ReportA_get]

            //    @PIBNo varchar(MAX) = NULL,
            //    @PIBPeriodStart datetime = NULL,
            //    @PIBPeriodEnd datetime = NULL

            //AS
            //BEGIN

            //    SELECT 
            //            0 AS [col A_group 1],
            //            0 AS [col B_group 1],
            //            0 AS [col C],
            //            0 AS colD_group2,
            //            0 AS colE_group2,
            //            0 AS colF_group2
            //    --FROM Table

            //    --WHERE 1 = 1
            //    --  AND(@PIBNo IS NULL OR Table.column LIKE '%' + @PIBNo + '%')
            //    --  AND(@PIBPeriodStart IS NULL OR Table.column >= @PIBPeriodStart)
            //    --  AND(@PIBPeriodEnd IS NULL OR Table.column < @PIBPeriodEnd)

            //END
            //GO

            return DBConnection.getDataTable("DBContext", ReportStoredProcedureName, true,
                    DBConnection.getSqlParameter("PIBNo", PIBNo), 
                    DBConnection.getSqlParameter("PIBPeriodStart", dtPIBPeriodStart),
                    DBConnection.getSqlParameter("PIBPeriodEnd", dtPIBPeriodEnd)
                );
        }

        /* EXCEL METHODS **************************************************************************************************************************************/

        public ExcelPackage CompileExcelPackage(DataTable datatable, string title, string filter, bool? Landscape)
        {
            ExcelPackage excelPackage = new ExcelPackage();

            Excel.SetWorkbookProperties(excelPackage);
            var workbook = excelPackage.Workbook;
            var ws = workbook.Worksheets.Add("Sheet1");
            if (Landscape != null && (bool)Landscape)
                ws.PrinterSettings.Orientation = eOrientation.Landscape;

            /***********************************************************************************************************************************************
             * BUILD HEADERS
             **********************************************************************************************************************************************/

            int headerGroupRowIndex = 4;
            int headerCellRowIndex = headerGroupRowIndex + 1;
            int defaultColumnWidth = 1;

            List<string> columnNames = datatable.Columns
                .Cast<DataColumn>()
                .Select(column => column.ColumnName)
                .ToList();

            List<string> headerGroups = new List<string>();
            List<List<ExcelCellFormat>> headerCells = new List<List<ExcelCellFormat>>();

            string[] columnParts;
            string headerCellName, headerGroupName;

            //build headers
            int headerCellColumnIndex = 0;
            foreach (string columnName in columnNames)
            {
                columnParts = columnName.Split('_');
                headerCellName = columnParts[0];
                headerGroupName = columnParts.Length == 2 ? columnParts[1] : "";
                if (!string.IsNullOrEmpty(headerGroupName))
                {
                    if (!headerGroups.Contains(headerGroupName))
                    {
                        headerGroups.Add(headerGroupName);
                        headerCells.Add(new List<ExcelCellFormat>());
                    }

                    headerCells[headerGroups.LastIndexOf(headerGroupName)].Add(new ExcelCellFormat(defaultColumnWidth, ++headerCellColumnIndex, headerCellName));
                }
                else
                {
                    headerGroups.Add(null);
                    headerCells.Add(new List<ExcelCellFormat>());
                    headerCells[headerCells.Count - 1].Add(new ExcelCellFormat(defaultColumnWidth, ++headerCellColumnIndex, headerCellName));
                }
            }

            //write headers
            for (int i = 0; i <= headerGroups.Count - 1; i++)
                Excel.editCellGroup(ws, headerGroupRowIndex, headerCells[i][0].ColumnIndex, headerCells[i][0].Width, headerGroups[i], null, headerCells[i].ToArray());

            //header styling
            ws.Cells[headerGroupRowIndex, 1, headerGroupRowIndex, headerCellColumnIndex].Style.Font.Bold = true;
            ws.Cells[headerCellRowIndex, 1, headerCellRowIndex, headerCellColumnIndex].Style.Font.Bold = true;
            ws.Cells[headerGroupRowIndex, 1, headerGroupRowIndex, headerCellColumnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[headerCellRowIndex, 1, headerCellRowIndex, headerCellColumnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var headerBorder = ws.Cells[headerGroupRowIndex, 1, headerCellRowIndex, headerCellColumnIndex].Style.Border;
            headerBorder.Left.Style = headerBorder.Top.Style = headerBorder.Right.Style = headerBorder.Bottom.Style = ExcelBorderStyle.Thin;
            var headerFill = ws.Cells[headerGroupRowIndex, 1, headerCellRowIndex, headerCellColumnIndex].Style.Fill;
            headerFill.PatternType = ExcelFillStyle.Solid;
            headerFill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            /***********************************************************************************************************************************************
             * POPULATE DATA
             **********************************************************************************************************************************************/

            ws.Cells[headerCellRowIndex + 1, 1].LoadFromDataTable(datatable, false);
            Excel.setCellBorders(ws, headerCellRowIndex + 1, 1, headerCellRowIndex + 1 + datatable.Rows.Count, headerCellColumnIndex, ExcelBorderStyle.Thin);

            //autofit columns
            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            /***********************************************************************************************************************************************
             * BUILD TITLE : done last so autofit columns doesn't resize title row
             **********************************************************************************************************************************************/

            int titleRowIndex = 1;
            int filterRowIndex = titleRowIndex + 1;
            int columnWidth = (int)ws.Column(1).Width;

            Excel.editCell(ws, titleRowIndex, 1, columnWidth, title, null, 0, 0, null);
            ws.Cells[titleRowIndex, 1, titleRowIndex, 1].Style.Font.Bold = true;

            Excel.editCell(ws, filterRowIndex, 1, columnWidth, filter, null, 0, 0, null);

            /***********************************************************************************************************************************************
             * FINISHING
             **********************************************************************************************************************************************/

            string password = Util.getConfigVariable(Helper.APPCONFIG_REPORTEXCELPASSWORD);
            if (!string.IsNullOrEmpty(password))
            {
                if (password.ToLower() == "random")
                    password = Util.RandomString(10);
                ws.Protection.AllowFormatColumns = true;
                ws.Protection.AllowFormatRows = true;
                ws.Protection.SetPassword(password);
                //excelPackage.Save(password);
            }

            return excelPackage;
        }

        /******************************************************************************************************************************************************/

    }
}