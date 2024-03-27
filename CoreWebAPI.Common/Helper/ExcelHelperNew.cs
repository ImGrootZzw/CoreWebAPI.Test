using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections;
using NPOI.XSSF.Streaming;
using CoreWebAPI.Common.Helper;
using System.Reflection;
using System.Linq;

namespace ACH_DAM.Services.Helper
{
    public class ExcelHelperNew
    {
        private readonly string fileName = null;// 文件名
        private IWorkbook workbook = null/* TODO Change to default(_) if this is not a reference type */;
        private FileStream fs = null;
        private bool disposed;
        private readonly string ExcelDateFormat = Appsettings.App("ExcelFromat", "ExcelDateFormat").GetCString();
        private readonly string ExcelIntFormat = Appsettings.App("ExcelFromat", "ExcelIntFormat").GetCString();
        private readonly string ExcelDecFormat = Appsettings.App("ExcelFromat", "ExcelDecFormat").GetCString();
        private readonly string ExcelBoolTrueFormat = Appsettings.App("ExcelFromat", "ExcelBoolTrueFormat").GetCString();
        private readonly string ExcelBoolFalseFormat = Appsettings.App("ExcelFromat", "ExcelBoolFalseFormat").GetCString();

        public ExcelHelperNew(string fileName)
        {
            this.fileName = fileName;
            disposed = false;
        }

        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="sheetName">要导入的excel的sheet的名称，默认:sheet1</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入,默认true</param>
        /// <param name="dateFormat">日期格式，默认:yyyy/MM/dd</param>
        /// <param name="intFormat">整数值格式，默认:#,##0</param>
        /// <param name="decFormat">浮点数值格式，默认:#,##0.00</param>
        /// <param name="boolTrueFormat">布尔值“真”格式，默认:yes</param>
        /// <param name="boolFalseFormat">布尔值“假”格式，默认:no</param>
        /// <returns>导出数据行数(包含列名那一行)</returns>
        public int DataTableToExcel(DataTable data, string sheetName = "Sheet1", bool isColumnWritten = true, string dateFormat = "", string intFormat = "", string decFormat = "", string boolTrueFormat = "", string boolFalseFormat = "")
        {
            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (fileName.IndexOf(".xlsx") > 0)
                // 2007版本
                //workbook = new XSSFWorkbook();
                workbook = new SXSSFWorkbook(); //需要定时清理缓存文件 C:\Users\..\AppData\Local\Temp\poifiles
            else if (fileName.IndexOf(".xls") > 0)
                // 2003版本
                workbook = new HSSFWorkbook();

            if (dateFormat == "")
                dateFormat = ExcelDateFormat != "" ? ExcelDateFormat : "yyyy/MM/dd";
            if (intFormat == "")
                intFormat = ExcelIntFormat != "" ? ExcelIntFormat : "#,##0";
            if (decFormat == "")
                decFormat = ExcelDecFormat != "" ? ExcelDecFormat : "#,##0.00";
            if (boolTrueFormat == "")
                boolTrueFormat = ExcelBoolTrueFormat != "" ? ExcelBoolTrueFormat : "yes";
            if (boolFalseFormat == "")
                boolFalseFormat = ExcelBoolFalseFormat != "" ? ExcelBoolFalseFormat : "no";

            XSSFDataFormat sFormat = (XSSFDataFormat)workbook.CreateDataFormat();
            ICellStyle icsInt = workbook.CreateCellStyle();
            icsInt.DataFormat = sFormat.GetFormat(intFormat);
            ICellStyle icsDec = workbook.CreateCellStyle();
            icsDec.DataFormat = sFormat.GetFormat(decFormat);

            try
            {
                ISheet sheet;
                if (workbook != null)
                    sheet = workbook.CreateSheet(sheetName);
                else
                    return -1;

                int count = 0;
                if (isColumnWritten == true)
                {
                    // 写入DataTable的列名
                    IRow row = sheet.CreateRow(0);
                    for (int j = 0; j <= data.Columns.Count - 1; j++)
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    count ++;
                }

                for (int i = 0; i <= data.Rows.Count - 1; i++)
                {
                    IRow row = sheet.CreateRow(count++);
                    for (int j = 0; j <= data.Columns.Count - 1; j++)
                    {
                        switch (data.Columns[j].DataType.Name.ToLower())
                        {
                            case "int":
                                row.CreateCell(j).SetCellValue(data.Rows[i][j].GetCInt());
                                row.GetCell(j).CellStyle = icsInt;
                                break;
                            case "int16":
                                row.CreateCell(j).SetCellValue(data.Rows[i][j].GetCInt());
                                row.GetCell(j).CellStyle = icsInt;
                                break;
                            case "int32":
                                row.CreateCell(j).SetCellValue(data.Rows[i][j].GetCInt());
                                row.GetCell(j).CellStyle = icsInt;
                                break;
                            case "int64":
                                row.CreateCell(j).SetCellValue(data.Rows[i][j].GetCInt());
                                row.GetCell(j).CellStyle = icsInt;
                                break;
                            case "decimal":
                                row.CreateCell(j).SetCellValue(data.Rows[i][j].GetCDouble());
                                row.GetCell(j).CellStyle = icsDec;
                                break;
                            case "double":
                                row.CreateCell(j).SetCellValue(data.Rows[i][j].GetCDouble());
                                row.GetCell(j).CellStyle = icsDec;
                                break;
                            case "date":
                                if(data.Rows[i][j].ToString() != "")
                                    row.CreateCell(j).SetCellValue(data.Rows[i][j].GetCDate().ToString(dateFormat));
                                else
                                    row.CreateCell(j).SetCellValue("");
                                break;
                            case "datetime":
                                if(data.Rows[i][j].ToString() != "")
                                    row.CreateCell(j).SetCellValue(data.Rows[i][j].GetCDate().ToString(dateFormat));
                                else
                                    row.CreateCell(j).SetCellValue("");
                                break;
                            case "boolean":
                                row.CreateCell(j).SetCellValue(data.Rows[i][j].GetCBool() == true ? boolTrueFormat : boolFalseFormat);
                                break;
                            default:
                                row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                                break;
                        }
                    }
                }
                workbook.Write(fs);
                // 写入到excel
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// 输出excel的datatable结构体
        /// </summary>
        public struct DTlist
        {
            public DataTable Data;
            public string SheetName;
            public bool IsColumnWritten;
            public bool IsFirstRowColumn;
            public string str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtlistdata">数据集合</param>
        /// <param name="dateFormat">日期格式，默认:yyyy/MM/dd</param>
        /// <param name="intFormat">整数值格式，默认:#,##0</param>
        /// <param name="decFormat">浮点数值格式，默认:#,##0.00</param>
        /// <param name="boolTrueFormat">布尔值“真”格式，默认:yes</param>
        /// <param name="boolFalseFormat">布尔值“假”格式，默认:no</param>
        /// <returns></returns>
        public int DataTableListToExcel(List<DTlist> dtlistdata, string dateFormat = "", string intFormat = "", string decFormat = "", string boolTrueFormat = "", string boolFalseFormat = "")
        {
            int count = 0;
            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (fileName.IndexOf(".xlsx") > 0)
                // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls") > 0)
                // 2003版本
                workbook = new HSSFWorkbook();

            if (dateFormat == "")
                dateFormat = ExcelDateFormat != "" ? ExcelDateFormat : "yyyy/MM/dd";
            if (intFormat == "")
                intFormat = ExcelIntFormat != "" ? ExcelIntFormat : "#,##0";
            if (decFormat == "")
                decFormat = ExcelDecFormat != "" ? ExcelDecFormat : "#,##0.00";
            if (boolTrueFormat == "")
                boolTrueFormat = ExcelBoolTrueFormat != "" ? ExcelBoolTrueFormat : "yes";
            if (boolFalseFormat == "")
                boolFalseFormat = ExcelBoolFalseFormat != "" ? ExcelBoolFalseFormat : "no";

            XSSFDataFormat sFormat = (XSSFDataFormat)workbook.CreateDataFormat();
            ICellStyle icsInt = workbook.CreateCellStyle();
            icsInt.DataFormat = sFormat.GetFormat(intFormat);
            ICellStyle icsDec = workbook.CreateCellStyle();
            icsDec.DataFormat = sFormat.GetFormat(decFormat);

            try
            {
                for (var v_i = 0; v_i <= dtlistdata.Count - 1; v_i++)
                {
                    ISheet sheet;
                    if (workbook != null)
                    {
                        sheet = workbook.CreateSheet();
                        workbook.SetSheetName(v_i, dtlistdata[v_i].SheetName);
                    }
                    else
                        return -1;

                    if (dtlistdata[v_i].IsColumnWritten == true)
                    {
                        // 写入DataTable的列名
                        IRow row = sheet.CreateRow(0);
                        for (var j = 0; j <= dtlistdata[v_i].Data.Columns.Count - 1; j++)
                            row.CreateCell(j).SetCellValue(dtlistdata[v_i].Data.Columns[j].ColumnName);
                        count = 1;
                    }
                    else
                        count = 0;

                    for (var i = 0; i <= dtlistdata[v_i].Data.Rows.Count - 1; i++)
                    {
                        IRow row = sheet.CreateRow(count);
                        for (var j = 0; j <= dtlistdata[v_i].Data.Columns.Count - 1; j++)
                        {
                            switch (dtlistdata[v_i].Data.Columns[j].DataType.Name.ToLower())
                            {
                                case "int":
                                    row.CreateCell(j).SetCellValue(dtlistdata[v_i].Data.Rows[i][j].GetCInt());
                                    row.GetCell(j).CellStyle = icsInt;
                                    break;
                                case "int16":
                                    row.CreateCell(j).SetCellValue(dtlistdata[v_i].Data.Rows[i][j].GetCInt());
                                    row.GetCell(j).CellStyle = icsInt;
                                    break;
                                case "int32":
                                    row.CreateCell(j).SetCellValue(dtlistdata[v_i].Data.Rows[i][j].GetCInt());
                                    row.GetCell(j).CellStyle = icsInt;
                                    break;
                                case "int64":
                                    row.CreateCell(j).SetCellValue(dtlistdata[v_i].Data.Rows[i][j].GetCInt());
                                    row.GetCell(j).CellStyle = icsInt;
                                    break;
                                case "decimal":
                                    row.CreateCell(j).SetCellValue(dtlistdata[v_i].Data.Rows[i][j].GetCDouble());
                                    row.GetCell(j).CellStyle = icsDec;
                                    break;
                                case "double":
                                    row.CreateCell(j).SetCellValue(dtlistdata[v_i].Data.Rows[i][j].GetCDouble());
                                    row.GetCell(j).CellStyle = icsDec;
                                    break;
                                case "date":
                                    if (dtlistdata[v_i].Data.Rows[i][j].ToString() != "")
                                        row.CreateCell(j).SetCellValue(dtlistdata[v_i].Data.Rows[i][j].GetCDate().ToString(dateFormat));
                                    else
                                        row.CreateCell(j).SetCellValue("");
                                    break;
                                case "datetime":
                                    if (dtlistdata[v_i].Data.Rows[i][j].ToString() != "")
                                        row.CreateCell(j).SetCellValue(dtlistdata[v_i].Data.Rows[i][j].GetCDate().ToString(dateFormat));
                                    else
                                        row.CreateCell(j).SetCellValue("");
                                    break;
                                case "boolean":
                                    row.CreateCell(j).SetCellValue(dtlistdata[v_i].Data.Rows[i][j].GetCBool() == true ? boolTrueFormat : boolFalseFormat);
                                    break;
                                default:
                                    row.CreateCell(j).SetCellValue(dtlistdata[v_i].Data.Rows[i][j].ToString());
                                    break;
                            }
                        }
                        count += 1;
                    }
                }
                workbook.Write(fs);
                // 写入到excel
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }
        }

        ///// <summary>
        ///// 合并符合格式的单元格
        ///// </summary>
        ///// <param name="data"></param>
        ///// <param name="sheetName"></param>
        ///// <returns></returns>
        //public int DataTableToExcel(DataTable data, string sheetName)
        //{
        //    int i = 0;
        //    int j = 0;
        //    int count = 0;
        //    ISheet sheet = null/* TODO Change to default(_) if this is not a reference type */;

        //    fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        //    if (fileName.IndexOf(".xlsx") > 0)
        //        // 2007版本
        //        workbook = new XSSFWorkbook();
        //    else if (fileName.IndexOf(".xls") > 0)
        //        // 2003版本
        //        workbook = new HSSFWorkbook();
        //    ICellStyle style = workbook.CreateCellStyle;
        //    style.Alignment = HorizontalAlignment.Center; // 居中
        //    style.VerticalAlignment = VerticalAlignment.Center; // 垂直居中
        //    try
        //    {
        //        if (workbook != null)
        //            sheet = workbook.CreateSheet(sheetName);
        //        else
        //            return -1;



        //        for (i = 0; i <= data.Rows.Count - 1; i++)
        //        {
        //            IRow row = sheet.CreateRow(count);
        //            for (j = 0; j <= data.Columns.Count - 1; j++)
        //            {
        //                if (Information.IsNumeric(data.Rows[i][j].ToString()) & data.Rows[i][j].ToString().Length == getCDec(data.Rows[i][j].ToString()).ToString.Length)
        //                    row.CreateCell(j).SetCellValue(getCDec(data.Rows[i][j].ToString()));
        //                else
        //                    row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
        //            }
        //            count += 1;
        //        }
        //        // 合并标题

        //        // 记录下第1个起始行
        //        int intStartRow;
        //        for (i = 0; i <= data.Rows.Count - 1; i++)
        //        {
        //            if (data.Rows[i][0] != null)
        //            {
        //                if (data.Rows[i][0].ToString() != "")
        //                {
        //                    intStartRow = i;
        //                    break;
        //                }
        //            }
        //        }
        //        // 记录下第1个起始列
        //        int intStartColumns;
        //        for (i = 0; i <= data.Columns.Count - 1; i++)
        //        {
        //            if (data.Rows[0][i] != null)
        //            {
        //                if (data.Rows[0][i].ToString() != "")
        //                {
        //                    intStartColumns = i;
        //                    break;
        //                }
        //            }
        //        }
        //        // CellRangeAddress四个参数为：起始行，结束行，起始列，结束列
        //        string strcell = "";
        //        for (i = 0; i <= intStartColumns - 1; i++)
        //        {
        //            strcell = data.Rows[intStartRow][i].ToString();
        //            sheet.AddMergedRegion(new CellRangeAddress(0, intStartRow, i, i));
        //            sheet.GetRow(0).GetCell(i).CellStyle = style;
        //            sheet.GetRow(0).GetCell(i).SetCellValue(strcell);
        //        }

        //        int rangefrom = data.Columns.Count - 1;
        //        int rangeto = -1;
        //        for (j = 0; j <= intStartRow; j++)
        //        {
        //            rangefrom = data.Columns.Count - 1;
        //            rangeto = -1;
        //            for (i = data.Columns.Count - 1; i >= intStartColumns; i += -1)
        //            {
        //                if (data.Rows[j][i].ToString() != "")
        //                {
        //                    rangeto = i;
        //                    if (rangefrom >= intStartColumns & rangeto > -1)
        //                    {
        //                        sheet.AddMergedRegion(new CellRangeAddress(j, j, rangeto, rangefrom));
        //                        sheet.GetRow(j).GetCell(i).CellStyle = style;
        //                        rangefrom = i - 1;
        //                        rangeto = -1;
        //                    }
        //                }
        //            }
        //        }

        //        workbook.Write(fs);
        //        workbook.Close();
        //        // 写入到excel
        //        return count;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Exception: " + ex.Message);
        //        return -1;
        //    }
        //}

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(string sheetName, bool isFirstRowColumn)
        {
            DataTable data = new DataTable();
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx") > 0)
                    // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0)
                    // 2003版本
                    workbook = new HSSFWorkbook(fs);

                var evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();

                ISheet sheet;
                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null)
                        // 如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                        sheet = workbook.GetSheetAt(0);
                }
                else
                    sheet = workbook.GetSheetAt(0);
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum;
                    int startRow;
                    // 一行最后一个cell的编号 即总的列数
                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i <= cellCount - 1; i++)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                        startRow = sheet.FirstRowNum;

                    // 最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null)
                            continue;
                        // 没有数据的行默认是null　　　　　　　
                        List<object> itemArray = new List<object>();

                        for (int j = 0; j <= cellCount - 1; j++)
                        {
                            itemArray.Add(GetCellValue(row.GetCell(j)));
                        }
                        data.Rows.Add(itemArray.ToArray());
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 将excel中的数据导入到List中
        /// </summary>
        /// <typeparam name="T">List数据类型</typeparam>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns></returns>
        public List<T> ExcelToList<T>(string sheetName, bool isFirstRowColumn) {
            List<T> data = new();
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx") > 0)
                    // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0)
                    // 2003版本
                    workbook = new HSSFWorkbook(fs);

                var evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();

                ISheet sheet;
                var header=new List<string>();
                var props=new List<PropertyInfo>(typeof(T).GetProperties());
                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null)
                        // 如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                        sheet = workbook.GetSheetAt(0);
                }
                else
                    sheet = workbook.GetSheetAt(0);
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum;
                    int startRow;
                    // 一行最后一个cell的编号 即总的列数
                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i <= cellCount - 1; i++)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    header.Add(cellValue);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                        startRow = sheet.FirstRowNum;

                    // 最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; i++)
                    {
                        T s=Activator.CreateInstance<T>();
                        object v = null;
                        IRow row = sheet.GetRow(i);
                        if (row == null)
                            continue;
                        // 没有数据的行默认是null　　　　　　　
                        //List<object> itemArray = new List<object>();

                        for (int j = 0; j <= cellCount - 1; j++)
                        {
                            var infos = props.Where(w => w.Name == header[j]).ToList();
                            if (infos.Count != 0)
                            {
                                var info=infos.FirstOrDefault();
                                v = Convert.ChangeType(GetCellValue(row.GetCell(j)), info.PropertyType);
                                //itemArray.Add(GetCellValue(row.GetCell(j)));
                                info.SetValue(s, v, null);
                            }
                        }
                        //data.Rows.Add(itemArray.ToArray());
                        
                        data.Add(s);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ExcelToList Exception: " + ex.Message);
                return null;
            }
        }
        ///// <summary>
        ///// 将excel中的数据导入到List中
        ///// </summary>
        ///// <param name="sheetName">excel工作薄sheet的名称</param>
        ///// <param name="isFirstRowColumn">第一行是否是实体的属性名</param>
        ///// <param name="propNameMapping">实体属性名对应表格第一列名称</param>
        ///// <returns>返回List实体</returns>
        //public List<TEntity> ExcelToList<TEntity>(string sheetName, bool isFirstRowColumn, List<KeyValuePair<string, string>> propNameMapping)
        //{
        //    List<TEntity> data = new List<TEntity>();
        //    try
        //    {
        //        fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        //        if (fileName.IndexOf(".xlsx") > 0)
        //            // 2007版本
        //            workbook = new XSSFWorkbook(fs);
        //        else if (fileName.IndexOf(".xls") > 0)
        //            // 2003版本
        //            workbook = new HSSFWorkbook(fs);

        //        var evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();

        //        ISheet sheet;
        //        if (sheetName != null)
        //        {
        //            sheet = workbook.GetSheet(sheetName);
        //            if (sheet == null)
        //                // 如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
        //                sheet = workbook.GetSheetAt(0);
        //        }
        //        else
        //            sheet = workbook.GetSheetAt(0);
        //        if (sheet != null)
        //        {
        //            IRow firstRow = sheet.GetRow(0);
        //            int cellCount = firstRow.LastCellNum;
        //            int startRow;
        //            // 一行最后一个cell的编号 即总的列数
        //            if (isFirstRowColumn)
        //                startRow = sheet.FirstRowNum + 1;
        //            else
        //                startRow = sheet.FirstRowNum;

        //            // 最后一列的标号
        //            int rowCount = sheet.LastRowNum;
        //            for (int i = startRow; i <= rowCount; i++)
        //            {
        //                IRow row = sheet.GetRow(i);
        //                if (row == null)
        //                    continue;
        //                // 没有数据的行默认是null　　　　　　　
        //                List<object> itemArray = new List<object>();

        //                TEntity entity;
        //                foreach (PropertyInfo rp in TypeR.GetProperties())
        //                {
        //                    if (rp.Name != "Error" && rp.Name != "Item")
        //                        continue;

        //                    PropertyInfo piOld = TypeSOld.GetProperties().FirstOrDefault(p => rp.Name == p.Name && rp.PropertyType == p.PropertyType);
        //                    if (piOld != null)
        //                        rp.SetValue(r, piOld.GetValue(sOld, null), null);

        //                    PropertyInfo piNew = TypeSNew.GetProperties().FirstOrDefault(p => rp.Name == p.Name + "New" && rp.PropertyType == p.PropertyType);
        //                    if (piNew != null)
        //                        rp.SetValue(r, piNew.GetValue(sOld, null), null);
        //                }
        //                for (int j = 0; j <= cellCount - 1; j++)
        //                {
        //                    itemArray.Add(GetCellValue(row.GetCell(j)));
        //                }
        //                data.Add(itemArray.ToArray());
        //            }
        //        }

        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Exception: " + ex.Message);
        //        return null;
        //    }
        //}


        //public bool ExcelToDataTable(ref List<DTlist> dtlist)
        //{
        //    ISheet sheet = null/* TODO Change to default(_) if this is not a reference type */;

        //    int startRow = 0;
        //    try
        //    {
        //        fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        //        if (fileName.IndexOf(".xlsx") > 0)
        //            // 2007版本
        //            workbook = new XSSFWorkbook(fs);
        //        else if (fileName.IndexOf(".xls") > 0)
        //            // 2003版本
        //            workbook = new HSSFWorkbook(fs);
        //        for (var v_i = 0; v_i <= dtlist.Count - 1; v_i++)
        //        {
        //            if (dtlist[v_i].sheetName != null & dtlist[v_i].sheetName != "")
        //            {
        //                sheet = workbook.GetSheet(dtlist[v_i].sheetName);
        //                if (sheet == null)
        //                    // 如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
        //                    sheet = workbook.GetSheetAt(0);
        //            }
        //            else
        //                sheet = workbook.GetSheetAt(0);
        //            if (sheet != null)
        //            {
        //                IRow firstRow = sheet.GetRow(0);
        //                int cellCount = firstRow.LastCellNum;
        //                // 一行最后一个cell的编号 即总的列数
        //                if (dtlist[v_i].isFirstRowColumn)
        //                {
        //                    for (int i = firstRow.FirstCellNum; i <= cellCount - 1; i++)
        //                    {
        //                        ICell cell = firstRow.GetCell(i);
        //                        if (cell != null)
        //                        {
        //                            string cellValue = cell.StringCellValue;
        //                            if (cellValue != null)
        //                            {
        //                                DataColumn column = new DataColumn(cellValue);
        //                                dtlist[v_i].data.Columns.Add(column);
        //                            }
        //                        }
        //                    }
        //                    startRow = sheet.FirstRowNum + 1;
        //                }
        //                else
        //                    startRow = sheet.FirstRowNum;

        //                // 最后一列的标号
        //                int rowCount = sheet.LastRowNum;
        //                for (int i = startRow; i <= rowCount; i++)
        //                {
        //                    IRow row = sheet.GetRow(i);
        //                    if (row == null)
        //                        continue;
        //                    // 没有数据的行默认是null　　　　　　　
        //                    DataRow dataRow = dtlist[v_i].data.NewRow();
        //                    int startnum = 0;
        //                    if (row.FirstCellNum >= 0)
        //                    {
        //                        startnum = row.FirstCellNum;
        //                        for (int j = startnum; j <= cellCount - 1; j++)
        //                        {
        //                            if (row.GetCell(j) != null)
        //                                // 同理，没有数据的单元格都默认是null
        //                                dataRow[j] = row.GetCell(j).ToString();
        //                        }
        //                        dtlist[v_i].data.Rows.Add(dataRow);
        //                    }
        //                }
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Exception: " + ex.Message);
        //        return false;
        //    }
        //}


        ///// <summary>
        ///// 合并符合格式的单元格
        ///// </summary>
        ///// <param name="data"></param>
        ///// <param name="sheetName"></param>
        ///// <param name="intStartRow">表头的最后一行</param>
        ///// <returns></returns>
        //public int DataTableToExcel(DataTable data, string sheetName, int intStartRow)
        //{
        //    int i = 0;
        //    int j = 0;
        //    int count = 0;
        //    ISheet sheet = null/* TODO Change to default(_) if this is not a reference type */;

        //    fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        //    if (fileName.IndexOf(".xlsx") > 0)
        //        // 2007版本
        //        workbook = new XSSFWorkbook();
        //    else if (fileName.IndexOf(".xls") > 0)
        //        // 2003版本
        //        workbook = new HSSFWorkbook();
        //    ICellStyle style = workbook.CreateCellStyle;
        //    style.Alignment = HorizontalAlignment.Center; // 居中
        //    style.VerticalAlignment = VerticalAlignment.Center; // 垂直居中
        //    try
        //    {
        //        if (workbook != null)
        //            sheet = workbook.CreateSheet(sheetName);
        //        else
        //            return -1;



        //        for (i = 0; i <= data.Rows.Count - 1; i++)
        //        {
        //            IRow row = sheet.CreateRow(count);
        //            for (j = 0; j <= data.Columns.Count - 1; j++)
        //            {
        //                if (Information.IsNumeric(data.Rows[i][j].ToString()) & data.Rows[i][j].ToString().Length == getCDec(data.Rows[i][j].ToString()).ToString.Length)
        //                    row.CreateCell(j).SetCellValue(getCDec(data.Rows[i][j].ToString()));
        //                else
        //                    row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
        //            }
        //            count += 1;
        //        }
        //        // 合并标题




        //        // CellRangeAddress四个参数为：起始行，结束行，起始列，结束列


        //        string strcell = "";
        //        for (i = 0; i <= data.Columns.Count - 1; i++)
        //        {
        //            for (j = intStartRow; j >= 0; j += -1)
        //            {
        //                if (getCString(data.Rows[j][i]) != "")
        //                {
        //                    // strcell = data.Rows(j)(i)
        //                    if (j != intStartRow)
        //                        sheet.AddMergedRegion(new CellRangeAddress(j, intStartRow, i, i));

        //                    // sheet.GetRow(0).GetCell(i).CellStyle = style
        //                    // sheet.GetRow(0).GetCell(i).SetCellValue(strcell)
        //                    break;
        //                }
        //            }
        //        }

        //        int rangefrom = data.Columns.Count - 1;
        //        int rangeto = -1;
        //        for (j = 0; j <= intStartRow - 1; j++)
        //        {
        //            rangefrom = -1;
        //            rangeto = -1;
        //            for (i = data.Columns.Count - 1; i >= 0; i += -1)
        //            {
        //                if (data.Rows[j][i].ToString() == "" & rangefrom == -1)
        //                    rangefrom = i;
        //                if (data.Rows[j][i].ToString() != "" & rangefrom > -1)
        //                {
        //                    rangeto = i;
        //                    if (rangefrom > -1 & rangeto > -1)
        //                    {
        //                        sheet.AddMergedRegion(new CellRangeAddress(j, j, rangeto, rangefrom));
        //                        sheet.GetRow(j).GetCell(i).CellStyle = style;
        //                        rangefrom = -1;
        //                        rangeto = -1;
        //                    }
        //                }
        //            }
        //        }

        //        workbook.Write(fs);
        //        workbook.Close();
        //        // 写入到excel
        //        return count;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Exception: " + ex.Message);
        //        return -1;
        //    }
        //}

        /// <summary>
        /// 获取单元格类型
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <param name="isFormula">单元格内容是否为公式</param>
        /// <returns></returns>
        private object GetCellValue(ICell cell, bool isFormula = false)
        {
            if (cell == null)
                return null;
            CellType cellType = isFormula ? cell.CachedFormulaResultType : cell.CellType;
            switch (cellType)
            {
                case CellType.String: //STRING
                    return cell.StringCellValue;
                case CellType.Boolean: //BOOLEAN
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC
                    //NPOI中数字和日期都是NUMERIC类型的，这里对其进行判断是否是日期类型
                    if (HSSFDateUtil.IsCellDateFormatted(cell))//日期类型
                    {
                        return cell.DateCellValue;
                    }
                    else//其他数字类型
                    {
                        return cell.NumericCellValue;
                    }
                    //short format = cell.CellStyle.DataFormat;
                    //if (format == 14 || format == 31 || format == 57 || format == 58 || format == 20) { return Convert.ToDateTime(cell.DateCellValue).ToString("yyyy-MM-dd HH:mm:ss"); } else { return cell.NumericCellValue; }                
                case CellType.Formula: //FORMULA公式
                    // 递归处理公式结果
                    return GetCellValue(cell, true);
                case CellType.Error: //ERROR
                    return cell.ErrorCellValue;
                case CellType.Blank: //BLANK
                    return null;
                default:
                    return cell.CellFormula;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (fs != null)
                        fs.Close();
                }

                fs = null;
                disposed = true;
            }
        }
    }
}

