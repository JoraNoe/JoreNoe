using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace JoreNoe.JoreExcel
{
    public class ExcelCommonApi
    {
        /// <summary>
        /// 将LIst导出Excel 返回 文件流
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List">数据</param>
        /// <param name="CloumnNames">列名</param>
        /// <param name="SheetName">表名 </param>
        /// <param name="IngorePropName">省略名</param>
        /// <returns></returns>
        public static MemoryStream ListToExcel<T>(IList<T> List, string[] CloumnNames, string SheetName = "sheet1", string[] IngorePropName = null)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            int headRowIndex = 0;
            string sheetName = "Sheet1";

            ISheet sheet = workbook.CreateSheet(sheetName);
            int rowIndex = 0;
            #region 列样式

            ICellStyle headStyle = workbook.CreateCellStyle();
            headStyle.Alignment = HorizontalAlignment.Center;
            headStyle.BorderBottom = BorderStyle.Thin;
            headStyle.BorderRight = BorderStyle.Thin;
            headStyle.BorderTop = BorderStyle.Thin;
            headStyle.BorderLeft = BorderStyle.Thin;
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 10;
            font.IsBold = true;
            headStyle.SetFont(font);

            ICellStyle Style = workbook.CreateCellStyle();
            Style.Alignment = HorizontalAlignment.Center;
            Style.BorderBottom = BorderStyle.Thin;
            Style.BorderRight = BorderStyle.Thin;
            Style.BorderTop = BorderStyle.Thin;
            Style.BorderLeft = BorderStyle.Thin;

            #endregion
            #region 列头
            {
                XSSFRow headerRow = (XSSFRow)sheet.CreateRow(headRowIndex);
                for (int i = 0; i < CloumnNames.Length; i++)
                {
                    headerRow.CreateCell(i).SetCellValue(CloumnNames[i]);
                    headerRow.GetCell(i).CellStyle = headStyle;
                    sheet.SetColumnWidth(i, 6000);
                }
            }
            #endregion

            #region 填充内容
            foreach (var item in List)
            {
                rowIndex++;
                XSSFRow Row = (XSSFRow)sheet.CreateRow(rowIndex);
                for (int i = 0; i < CloumnNames.Length; i++)
                {
                    //忽略字段
                    if (IngorePropName.Any(d => d == CloumnNames[i]))
                        continue;
                    string ColumnValue;
                    //动态获取数据
                    var GetProps = item.GetType().GetProperties();
                    //读取数据
                    ColumnValue = GetProps[i].GetValue(item, null) + "";

                    Row.CreateCell(i).SetCellValue(ColumnValue);
                    Row.GetCell(i).CellStyle = Style;
                }
            }
            #endregion

            MemoryStream Streams = new MemoryStream();

            workbook.Write(Streams);
            Streams.Flush();
            return Streams;
        }

        /// <summary>
        /// 将Excel转成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ExcelFilePath"></param>
        /// <returns></returns>
        public static IList<T> ExcelToList<T>(string ExcelFilePath)
            where T : new()
        {
            if (!File.Exists(ExcelFilePath))
                throw new Exception("文件不存在");

            IList<T> TempExcelData = new List<T>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var Stream = File.OpenRead(ExcelFilePath))

            using (ExcelPackage package = new ExcelPackage(Stream))
            {
                try
                {

                    foreach (var item in package.Workbook.Worksheets)
                    {
                        if (item.Dimension != null)
                        {
                            for (int i = 2; i <= item.Dimension.Rows; i++)
                            {
                                T SingleData = new T { };
                                var Props = SingleData.GetType().GetProperties();
                                for (int j = 1; j <= item.Dimension.Columns; j++)
                                {
                                    var Data = item.Cells[i, j].Value + "";
                                    Props[j - 1].SetValue(SingleData, Data);
                                }
                                TempExcelData.Add(SingleData);
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    //关闭读取
                    package.Workbook.Worksheets.Dispose();
                    package.Workbook.Dispose();
                    package.Dispose();
                    GC.Collect();

                }
            }


            GC.Collect();
            return TempExcelData;
        }

    }
}
