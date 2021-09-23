using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

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

        //private List<T> ReadExcel<T>(string SubPath, string Category = "")
        //{
        //    DirectoryInfo Files = null;  // 未完善帮助类
        //    IList<string> MDBFileFullNamePaths = null;
        //    if (string.IsNullOrEmpty(Category))
        //    {
        //        //查找约定EXCEl 
        //        Files = new DirectoryInfo(SubPath);
        //        MDBFileFullNamePaths = new List<string>();
        //        foreach (FileInfo FileItem in Files.GetFiles())//遍历文件夹下所有文件
        //        {
        //            if (Path.GetExtension(FileItem.FullName) == ".xlsx")
        //            {
        //                //获取MBD文件地址 
        //                MDBFileFullNamePaths.Add(string.Concat(FileItem.FullName));
        //                break;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MDBFileFullNamePaths = new List<string>();
        //        MDBFileFullNamePaths.Add(Category);
        //    }



        //    List<T> TempExcelData = new List<T>();
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //    using (var Stream = File.OpenRead(MDBFileFullNamePaths[0]))

        //    using (ExcelPackage package = new ExcelPackage(Stream))
        //    {
        //        try
        //        {
        //            foreach (var item in package.Workbook.Worksheets)
        //            {
        //                if (item.Dimension != null)
        //                {
        //                    for (int i = 2; i <= item.Dimension.Rows; i++)
        //                    {
        //                        //去除空数据 
        //                        if (!string.IsNullOrEmpty(item.Cells[i, 1].Value + ""))
        //                        {
        //                            TempExcelData.Add(new T
        //                            {
        //                                IdNumber = string.IsNullOrEmpty(item.Cells[i, 1].Value + "") ? "" : item.Cells[i, 1].Value.ToString(),
        //                                IDFront = string.IsNullOrEmpty(item.Cells[i, 2].Value + "") ? "" : item.Cells[i, 2].Value.ToString(),
        //                                IDBack = string.IsNullOrEmpty(item.Cells[i, 3].Value + "") ? "" : item.Cells[i, 3].Value.ToString(),
        //                                GraduationCertificate = string.IsNullOrEmpty(item.Cells[i, 4].Value + "") ? "" : item.Cells[i, 4].Value.ToString(),
        //                                IdentificationPhoto = string.IsNullOrEmpty(item.Cells[i, 5].Value + "") ? "" : item.Cells[i, 5].Value.ToString(),
        //                                IdNumberChange = string.IsNullOrEmpty(item.Cells[i, 6].Value + "") ? "" : item.Cells[i, 6].Value.ToString(),
        //                                PersonalPhoto = string.IsNullOrEmpty(item.Cells[i, 7].Value + "") ? "" : item.Cells[i, 7].Value.ToString(),
        //                                PreQualification = string.IsNullOrEmpty(item.Cells[i, 8].Value + "") ? "" : item.Cells[i, 8].Value.ToString(),
        //                            });
        //                        }
        //                    }
        //                }

        //            }
        //        }
        //        catch
        //        {

        //        }
        //        finally
        //        {
        //            //关闭读取
        //            package.Workbook.Worksheets.Dispose();
        //            package.Workbook.Dispose();
        //            package.Dispose();
        //            GC.Collect();

        //        }
        //    }


        //    GC.Collect();
        //    return TempExcelData;
        //}

    }
}
