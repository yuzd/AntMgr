using Infrastructure.StaticExt;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Infrastructure.Excel
{
    public class ExcelHelper
    {


        #region Public

        /// <summary>
        /// 由DataSet导出Excel
        /// </summary>
        /// <param name="sourceDs">要导出数据的DataSet</param>
        /// <returns></returns>
        public static byte[] ToExcel(DataSet sourceDs)
        {


            // if (string.IsNullOrEmpty(filePath)) return null;

            bool isCompatible = true;//GetIsCompatible(filePath);

            IWorkbook workbook = CreateWorkbook(isCompatible);
            ICellStyle headerCellStyle = GetCellStyle(workbook, true);
            //ICellStyle cellStyle = Common.GetCellStyle(workbook);

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
                        //cell.SetCellValue((row[column] ?? "").ToString());
                        //cell.CellStyle = cellStyle;
                        SetCellValue(cell, (row[column] ?? "").ToString(), column.DataType, colStyles);
                        ReSizeColumnWidth(sheet, cell);
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

            workbook = null;
            return xlsInBytes;

        }

        /// <summary>
        /// 集合转变成Html的table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ToHtmlTable<T>(IEnumerable<T> list)
        {
            list = list.NotNull("list");
            Dictionary<PropertyInfo, ExcelInfoAttribute> _excelInfos = GetPropInfo<T>();
            StringBuilder strHTMLBuilder = new StringBuilder();
            strHTMLBuilder.Append("<table border='1px' cellpadding='1' cellspacing='1' style='border-collapse:collapse;'>");
            strHTMLBuilder.Append("<tr >");
            foreach (var item in _excelInfos)
            {
                strHTMLBuilder.Append("<th colspan='1' rowspan='1' style='background:#FFB90F;border:1px solid #bbb;'>");
                strHTMLBuilder.Append(item.Value.Name);
                strHTMLBuilder.Append("</th>");

            }
            strHTMLBuilder.Append("</tr>");
            foreach (T rowItem in list)
            {

                strHTMLBuilder.Append("<tr >");
                foreach (var item in _excelInfos)
                {
                    object _cellItemValue = item.Key.GetValue(rowItem, null);
                    strHTMLBuilder.Append("<td style='border:1px solid #bbb;' >");
                    strHTMLBuilder.Append(_cellItemValue);
                    strHTMLBuilder.Append("</td>");

                }
                strHTMLBuilder.Append("</tr>");
            }

            //Close tags. 
            strHTMLBuilder.Append("</table>");

            string Htmltext = strHTMLBuilder.ToString();

            return Htmltext;
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="list">导出模型数据</param>
        /// <param name="ExcelSheetName">初始页签名称</param>
        public static byte[] ExportExcel<T>(IEnumerable<T> list, string ExcelSheetName)
        {
            list = list.NotNull("list");
            //创建workbook
            IWorkbook workbook = CreateWorkbook(true);
            //创建worksheet
            ISheet sheet = workbook.CreateSheet(ExcelSheetName);
            //头样式
            ICellStyle headerCellStyle = GetCellStyle(workbook, true);
            //存储头部样式
            Dictionary<int, ICellStyle> colStyles = new Dictionary<int, ICellStyle>();
            //获取导出属性
            Dictionary<PropertyInfo, ExcelInfoAttribute> _excelInfos = GetPropInfo<T>();

            //设置Excel行
            IRow rowTitle = sheet.CreateRow(0);
            int _cellIndex = 0;
            foreach (var item in _excelInfos)
            {
                ICell celltitle = rowTitle.CreateCell(_cellIndex);
                celltitle.SetCellValue(item.Value.Name);
                celltitle.CellStyle = headerCellStyle;
                sheet.AutoSizeColumn(celltitle.ColumnIndex);//自动列宽
                colStyles[celltitle.ColumnIndex] = GetCellStyle(workbook);
                _cellIndex++;
            }

            //设置Excel内容
            int _rowNum = 1;
            foreach (T rowItem in list)
            {
                int _rowCell = 0;
                IRow _rowValue = sheet.CreateRow(_rowNum);
                foreach (var cellItem in _excelInfos)
                {
                    object _cellItemValue = cellItem.Key.GetValue(rowItem, null);
                    ICell _cell = _rowValue.CreateCell(_rowCell);
                    SetCellValue(_cell, _cellItemValue == null ? "" : _cellItemValue.ToString(), cellItem.Key.PropertyType, colStyles);
                    ReSizeColumnWidth(sheet, _cell);
                    _rowCell++;
                }
                _rowNum++;
            }
            sheet.ForceFormulaRecalculation = true;

            //导出
            byte[] xlsInBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                xlsInBytes = ms.ToArray();
            }
            return xlsInBytes;
        }


        #endregion

        #region Private


        /// <summary>
        /// 获取实体类的公共属性
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <returns>实体类相应集合</returns>
        private static Dictionary<PropertyInfo, ExcelInfoAttribute> GetPropInfo<T>()
        {
            Dictionary<PropertyInfo, ExcelInfoAttribute> _infos = new Dictionary<PropertyInfo, ExcelInfoAttribute>();
            Type _type = typeof(T);
            var classAttr = _type.GetCustomAttributes<ExcelClassAttribute>();

            //获取所有的Properties
            PropertyInfo[] _propInfos = _type.GetProperties();

            foreach (ExcelClassAttribute classInfo in classAttr)
            {
                ExcelInfoAttribute attr = new ExcelInfoAttribute(classInfo.Name);
                attr.OrderRule = classInfo.OrderRule;
                var p = _propInfos.FirstOrDefault(r => r.Name.Equals(classInfo.Column));
                if (p != null)
                {
                    _infos.Add(p, attr);
                }
            }

            foreach (var propInfo in _propInfos)
            {
                object[] objAttrs = propInfo.GetCustomAttributes(typeof(ExcelInfoAttribute), true);
                if (objAttrs.Length > 0)
                {
                    ExcelInfoAttribute attr = objAttrs[0] as ExcelInfoAttribute;
                    if (attr != null)
                    {
                        _infos.Add(propInfo, attr);
                    }
                }
            }
            _infos = _infos.OrderBy(r => r.Value.OrderRule).ToDictionary(r => r.Key, y => y.Value);
            return _infos;
        }
        /// <summary>
        /// 判断是否为兼容模式
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool GetIsCompatible(string filePath)
        {
            string ext = Path.GetExtension(filePath);
            return new[] { ".xls", ".xlt" }.Count(e => e.Equals(ext, StringComparison.OrdinalIgnoreCase)) > 0;
        }

        /// <summary>
        /// 创建工作薄
        /// </summary>
        /// <param name="isCompatible"></param>
        /// <returns></returns>
        private static IWorkbook CreateWorkbook(bool isCompatible)
        {
            if (isCompatible)
            {
                return new HSSFWorkbook();
            }
            else
            {
                return new XSSFWorkbook();
            }
        }

        /// <summary>
        /// 创建单元格样式
        /// </summary>
        /// <param name="workbook">workbook</param>
        /// <param name="isHeaderRow">是否获取头部样式</param>
        /// <returns></returns>
        private static ICellStyle GetCellStyle(IWorkbook workbook, bool isHeaderRow = false)
        {
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
        private static void ReSizeColumnWidth(ISheet sheet, ICell cell)
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
