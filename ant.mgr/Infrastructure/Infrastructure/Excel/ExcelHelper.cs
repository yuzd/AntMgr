using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Infrastructure.Logging;
using Npoi.Mapper;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
namespace Infrastructure.Excel
{
    public static class ExcelHelper
    {
        /// <summary>
        /// excel文件流转的某个sheet成对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static List<T> ReadExcelSheet<T>(this Stream data, string sheetName = "Sheet1") where T : class
        {
            try
            {
                var importer = new Mapper(data);
                var items = importer.Take<T>(sheetName).Select(r => r.Value).ToList();
                return items;
            }
            catch (Exception ex)
            {
                LogHelper.Warn("FromExcel", "导出失败", ex);
                return null;
            }
        }

        /// <summary>
        /// 由DataSet导出Excel
        /// </summary>
        /// <param name="sourceDs">要导出数据的DataSet</param>
        /// <param name="isxls">是否是xls还是xlsx</param>
        /// <returns></returns>
        public static byte[] ToExcel(this DataSet sourceDs,bool isxls = false)
        {

            IWorkbook workbook = CreateWorkbook(isxls);
            ICellStyle headerCellStyle = GetCellStyle(workbook, true,isxls);

            for (int i = 0; i < sourceDs.Tables.Count; i++)
            {
                DataTable table = sourceDs.Tables[i];
                string sheetName = string.IsNullOrEmpty(table.TableName) ? "result" + i.ToString() : table.TableName;
                ISheet sheet = workbook.CreateSheet(sheetName);
                IRow headerRow = sheet.CreateRow(0);
                Dictionary<int, ICellStyle> colStyles = new Dictionary<int, ICellStyle>();
                // handling header.
                foreach (DataColumn column in table.Columns)
                {
                    ICell headerCell = headerRow.CreateCell(column.Ordinal);
                    headerCell.SetCellValue(column.ColumnName);
                    headerCell.CellStyle = headerCellStyle;
                    sheet.AutoSizeColumn(headerCell.ColumnIndex);
                    colStyles[headerCell.ColumnIndex] = GetCellStyle(workbook);
                }

                // handling value.
                int rowIndex = 1;

                foreach (DataRow row in table.Rows)
                {
                    IRow dataRow = sheet.CreateRow(rowIndex);

                    foreach (DataColumn column in table.Columns)
                    {
                        ICell cell = dataRow.CreateCell(column.Ordinal);
                        SetCellValue(cell, (row[column] ?? "").ToString(), column.DataType, colStyles);
                        ReSizeColumnWidth(sheet, cell, column.DataType);
                    }

                    rowIndex++;
                }
                sheet.ForceFormulaRecalculation = true;
            }
            byte[] xlsInBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                xlsInBytes = ms.ToArray();
            }

            return xlsInBytes;
        }


        #region Private

        /// <summary>
        /// 创建工作薄
        /// </summary>
        /// <param name="isCompatible"></param>
        /// <returns></returns>
        private static IWorkbook CreateWorkbook(bool isCompatible)
        {
            if (isCompatible)
            {
                return new HSSFWorkbook(); // 03
            }
            else
            {
                return new XSSFWorkbook(); // 07 以上
            }
        }


        /// <summary>
        /// 创建单元格样式
        /// </summary>
        /// <param name="workbook">workbook</param>
        /// <param name="isHeaderRow">是否获取头部样式</param>
        /// <returns></returns>
        private static ICellStyle GetCellStyle(IWorkbook workbook, bool isHeaderRow = false,bool isXls = false)
        {
            if (!isXls)
            {
                return GetCellStyleXlsx(workbook, isHeaderRow);
            }
            ICellStyle style = workbook.CreateCellStyle();

            if (isHeaderRow)
            {
                style.FillPattern = FillPattern.SolidForeground;
                style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                IFont f = workbook.CreateFont();
                f.Boldweight = (short)FontBoldWeight.Bold;
                style.SetFont(f);
            }

            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            return style;
        }

        private static ICellStyle GetCellStyleXlsx(IWorkbook workbook, bool isHeaderRow = false)
        {
            XSSFCellStyle style = (XSSFCellStyle)workbook.CreateCellStyle();

            if (isHeaderRow)
            {
                style.FillPattern = FillPattern.SolidForeground;
                style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.Alignment = HorizontalAlignment.Center;
                XSSFFont defaultFont = (XSSFFont)workbook.CreateFont();
                defaultFont.FontHeightInPoints = (short)10;
                defaultFont.FontName = "Arial";
                defaultFont.Color = IndexedColors.Black.Index;
                defaultFont.IsBold = false;
                defaultFont.IsItalic = false;
                defaultFont.Boldweight = 700;
                style.SetFont(defaultFont);
            }

            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            return style;
        }


        /// <summary>
        /// 依据值类型为单元格设置值
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="value"></param>
        /// <param name="colType"></param>
        /// <param name="colStyles"></param>
        private static void SetCellValue(ICell cell, string value, Type colType, IDictionary<int, ICellStyle> colStyles)
        {
            string dataFormatStr = null;
            switch (colType.ToString())
            {
                case "System.String": //字符串类型
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(value);
                    break;
                case "System.Nullable`1[System.DateTime]":
                case "System.DateTime": //日期类型
                    DateTime dateV;
                    if (DateTime.TryParse(value, out dateV))
                    {
                        cell.SetCellValue(dateV);
                    }
                    dataFormatStr = "yyyy/mm/dd hh:mm:ss";
                    break;
                case "System.Nullable`1[System.Boolean]":
                case "System.Boolean": //布尔型
                    bool boolV = false;
                    if (bool.TryParse(value, out boolV))
                    {
                        cell.SetCellType(CellType.Boolean);
                        cell.SetCellValue(boolV);
                    }
                    break;
                case "System.Int16": //整型
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                case "System.Nullable`1[System.Int16]": //整型
                case "System.Nullable`1[System.Int32]":
                case "System.Nullable`1[System.Int64]":
                case "System.Nullable`1[System.Byte]":
                    long intV = 0;
                    if (long.TryParse(value, out intV))
                    {
                        cell.SetCellType(CellType.Numeric);
                        cell.SetCellValue(intV);
                    }
                    dataFormatStr = "0";
                    break;
                case "System.Decimal": //浮点型
                case "System.Double":
                case "System.Nullable`1[System.Decimal]": //浮点型
                case "System.Nullable`1[System.Double]":
                    double doubV = 0;
                    if (double.TryParse(value, out doubV))
                    {
                        cell.SetCellType(CellType.Numeric);
                        cell.SetCellValue(doubV);
                    }
                    dataFormatStr = "0.00";
                    break;
                case "System.DBNull": //空值处理
                    cell.SetCellType(CellType.Blank);
                    cell.SetCellValue("");
                    break;
                default:
                    cell.SetCellType(CellType.Unknown);
                    cell.SetCellValue(value);
                    break;
            }

            if (!string.IsNullOrEmpty(dataFormatStr) && colStyles[cell.ColumnIndex].DataFormat <= 0) //没有设置，则采用默认类型格式
            {
                colStyles[cell.ColumnIndex] = GetCellStyleWithDataFormat(cell.Sheet.Workbook, dataFormatStr);
            }
            cell.CellStyle = colStyles[cell.ColumnIndex];
        }

        /// <summary>
        /// 根据单元格内容重新设置列宽
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="cell"></param>
        /// <param name="cellType"></param>
        private static void ReSizeColumnWidth(ISheet sheet, ICell cell,Type cellType)
        {
            int cellLength = (Encoding.Default.GetBytes(cell.ToString()).Length + 2) * 256;
            const int maxLength = 60 * 256; //255 * 256;
            if (cellLength > maxLength) //当单元格内容超过30个中文字符（英语60个字符）宽度，则强制换行
            {
                cellLength = maxLength;
                cell.CellStyle.WrapText = true;
            }
            int colWidth = sheet.GetColumnWidth(cell.ColumnIndex);
            if (colWidth < cellLength)
            {
                sheet.SetColumnWidth(cell.ColumnIndex, cellLength);
            }
            else if (cellType == typeof(DateTime))
            {
                sheet.SetColumnWidth(cell.ColumnIndex, colWidth+10);
            }
        }

        /// <summary>
        /// 创建单元格样式并设置数据格式化规则
        /// </summary>
        /// <param name="workbook">workbook</param>
        /// <param name="format">格式化字符串</param>
        private static ICellStyle GetCellStyleWithDataFormat(IWorkbook workbook, string format)
        {
            var style = GetCellStyle(workbook);
            var dataFormat = workbook.CreateDataFormat();
            short formatId = -1;
            if (dataFormat is HSSFDataFormat)
            {
                formatId = HSSFDataFormat.GetBuiltinFormat(format);
            }
            if (formatId != -1)
            {
                style.DataFormat = formatId;
            }
            else
            {
                style.DataFormat = dataFormat.GetFormat(format);
            }
            return style;
        }

        #endregion

    }
}
