//测试结果导出excel
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LCD.dataBase;
using Microsoft.Office.Interop;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;

namespace LCD.Data
{
    class ExcelHelper
    {
        public static void Save2ExcelMultyWithTitle(string excelFile, List<dataBase.UserIdMode> list)
        {

            Dictionary<string, Data> datas = new Dictionary<string, Data>();
            //Data[] datas = null;
            bool Flag = false;

            var xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp==null)
            {
                throw new Exception("可能您的机子未安装excel，无法创建excel对象！");
            }
            string sFile = FolderHelper.GetTemplateFile();
            File.Copy(sFile, excelFile, true);
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(excelFile);


            for (int index = 0; index < list.Count; index++)//条码
            {

                for (int i = 0; i < list.Count; i++)
                {
                    for (int j = 0; j < list[i].ProjectModes.Count; j++)
                    {
                        try
                        {
                            datas.Add(list[i].ProjectModes[j].projectName,null);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }


                for (int i = 0; i < list[index].ProjectModes.Count; i++)//项目
                {
                    if (list[index].ProjectModes[i].projectName!=null)
                    {
                        Data data = null;
                        DataTable tempTable = new DataTable();



                        if (datas[list[index].ProjectModes[i].projectName] == null)
                        {
                            data = new Data();
                            if (list[index].ProjectModes[i].ModeType == 2)
                            {
                                
                                tempTable.Columns.Add("ID");
                                tempTable.Columns.Add("测试项");
                                tempTable.Columns.Add("Num");
                                tempTable.Columns.Add("Coor-X");
                                tempTable.Columns.Add("Coor-Y");
                                tempTable.Columns.Add("Coor-Z");
                                tempTable.Columns.Add("Coor-U");
                                tempTable.Columns.Add("Coor-V");
                                tempTable.Columns.Add("L");
                                tempTable.Columns.Add("X");
                                tempTable.Columns.Add("Y");
                                tempTable.Columns.Add("Z");
                                tempTable.Columns.Add("Cx");
                                tempTable.Columns.Add("Cy");
                                tempTable.Columns.Add("u");
                                tempTable.Columns.Add("v");
                                if(Project.cfg.ShowLab)
                                {
                                    tempTable.Columns.Add("L*");
                                    tempTable.Columns.Add("a*");
                                    tempTable.Columns.Add("b*");
                                }
                                tempTable.Columns.Add("CCT");
                                tempTable.Columns.Add("备注");
                                for (int t = 0; t < 401; t++)
                                {
                                    tempTable.Columns.Add($"{t + 380}");
                                }
                            }
                            else if (list[index].ProjectModes[i].ModeType == 8)
                            {
                                tempTable.Columns.Add("ID");
                                tempTable.Columns.Add("测试项");
                                tempTable.Columns.Add("Num");
                                tempTable.Columns.Add("电压");
                                tempTable.Columns.Add("电流");
                                tempTable.Columns.Add("功率");
                                tempTable.Columns.Add("备注");
                            }
                            else if (list[index].ProjectModes[i].ModeType == 1)
                            {
                                tempTable.Columns.Add("ID");
                                tempTable.Columns.Add("测试项");
                                tempTable.Columns.Add("Num");
                                tempTable.Columns.Add("Coor-X");
                                tempTable.Columns.Add("Coor-Y");
                                tempTable.Columns.Add("Coor-Z");
                                tempTable.Columns.Add("Coor-U");
                                tempTable.Columns.Add("Coor-V");
                                tempTable.Columns.Add("X");
                                tempTable.Columns.Add("Y");
                                tempTable.Columns.Add("Z");
                                tempTable.Columns.Add("Low");
                                tempTable.Columns.Add("High");
                                tempTable.Columns.Add("RiseTime");
                                tempTable.Columns.Add("FallTime");
                                tempTable.Columns.Add("完成时间");
                            }
                            else if(list[index].ProjectModes[i].ModeType == 4)
                            {
                                tempTable.Columns.Add("ID");
                                tempTable.Columns.Add("测试项");
                                tempTable.Columns.Add("Num");
                                tempTable.Columns.Add("La");
                                tempTable.Columns.Add("Lb");
                                tempTable.Columns.Add("CT");
                                tempTable.Columns.Add("完成时间");
                            }
                            else
                            {
                                tempTable.Columns.Add("ID");
                                tempTable.Columns.Add("测试项");
                                tempTable.Columns.Add("Num");
                                if (list[index].ProjectModes[i].ModeType == 0)
                                {
                                    tempTable.Columns.Add("Coor-X");
                                    tempTable.Columns.Add("Coor-Y");
                                    tempTable.Columns.Add("Coor-Z");
                                    tempTable.Columns.Add("Coor-U");
                                    tempTable.Columns.Add("Coor-V");
                                }
                                tempTable.Columns.Add("L");
                                tempTable.Columns.Add("Cx");
                                tempTable.Columns.Add("Cy");
                                tempTable.Columns.Add("X");
                                tempTable.Columns.Add("Y");
                                tempTable.Columns.Add("Z");                                
                                tempTable.Columns.Add("u");
                                tempTable.Columns.Add("v");

                                if (list[index].ProjectModes[i].ModeType == 0)
                                {
                                    if (Project.cfg.ShowLab)
                                    {
                                        tempTable.Columns.Add("L*");
                                        tempTable.Columns.Add("a*");
                                        tempTable.Columns.Add("b*");
                                    }
                                }
                                tempTable.Columns.Add("CCT");
                                tempTable.Columns.Add("备注");
                            }

                            data.dt = tempTable;
                        }
                        else
                        {
                            data = datas[list[index].ProjectModes[i].projectName];

                        }

                        data.ModeType = list[index].ProjectModes[i].ModeType;
                        string sheetName = list[index].ProjectModes[i].projectName;
                        //if (sheetName==null)
                        //{
                        //    return;
                        //}
                        Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = GetSheetByName(xlWorkBook, sheetName);
                        if (xlWorkSheet == null)
                        {
                            xlWorkSheet = xlWorkBook.Sheets.Add();
                            xlWorkSheet.Name = sheetName;
                            xlWorkSheet.Columns.WrapText = true;
                            xlApp.Columns.AutoFit();
                            //xlApp.Rows.AutoFit();
                            xlWorkSheet.Columns.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenterAcrossSelection;
                           
                        }

                        data.xlWorkSheet = xlWorkSheet;



                        DataRow dataRow = null;

                        int AAA = 0;
                        for (int j = 0; j < list[index].ProjectModes[i].TestDataModes.Count; j++)//数据
                        {


                            if (list[index].ProjectModes[i].ModeType == 2)
                            {

                                dataRow = data.dt.NewRow();

                                #region CH

                                dataRow["ID"] = "'" + list[index].BarCode.ToString();
                                dataRow["测试项"] = list[index].ProjectModes[i].projectName;
                                dataRow["Num"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].Num == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].Num);
                                dataRow["Coor-X"] = list[index].ProjectModes[i].TestDataModes[j].CoordX;
                                dataRow["Coor-Y"] = list[index].ProjectModes[i].TestDataModes[j].CoordY;
                                dataRow["Coor-Z"] = list[index].ProjectModes[i].TestDataModes[j].CoordZ;
                                dataRow["Coor-U"] = list[index].ProjectModes[i].TestDataModes[j].CoordU;
                                dataRow["Coor-V"] = list[index].ProjectModes[i].TestDataModes[j].CoordV;
                                dataRow["L"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].L == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].L);
                                dataRow["X"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].X == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].X);
                                dataRow["Y"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].Y == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].Y);
                                dataRow["Z"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].Z == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].Z);
                                dataRow["Cx"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].Cx == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].Cx);
                                dataRow["Cy"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].Cy == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].Cy);
                                dataRow["u"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].u == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].u);
                                dataRow["v"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].v == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].v);
                                if (Project.cfg.ShowLab)
                                {
                                    dataRow["L*"] = list[index].ProjectModes[i].TestDataModes[j].Lcolor;
                                    dataRow["a*"] = list[index].ProjectModes[i].TestDataModes[j].Acolor;
                                    dataRow["b*"] = list[index].ProjectModes[i].TestDataModes[j].Bcolor;
                                }
                                dataRow["CCT"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].CCT == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].CCT);
                                dataRow["备注"] = list[index].ProjectModes[i].TestDataModes[j].Remark.Trim() == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].Remark;
                                AAA = j * 400;

                                for (int k = AAA; k < AAA + 401; k++)
                                {

                                    List<SpectrumDataMode> sdList = Project.ListSpectrumData;
                                    if ($"{Project.ListSpectrumData[k].DataName}" == "779")
                                    {

                                    }
                                    String AA = Project.ListSpectrumData[k].DataName;
                                    String BB = Project.ListSpectrumData[k].dataValue;
                                    dataRow[$"{Project.ListSpectrumData[k].DataName}"] = Project.ListSpectrumData[k].dataValue;
                                }
                                data.dt.Rows.InsertAt(dataRow, data.dt.Rows.Count + 1);
                                #endregion
                                // data.dt.Columns.InsertAt(dataRow, data.dt.Rows.Count + 1);
                            }
                            else if (list[index].ProjectModes[i].ModeType == 8)
                            {
                                dataRow = data.dt.NewRow();
                                dataRow["ID"] = "'" + list[index].BarCode.ToString();
                                dataRow["测试项"] = list[index].ProjectModes[i].projectName;
                                dataRow["Num"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].Num == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].Num);
                                dataRow["电压"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].Voltage == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].Voltage);
                                dataRow["电流"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].ElectricCurrent.Trim() == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].ElectricCurrent);
                                dataRow["功率"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].Power.Trim() == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].Power);
                                dataRow["备注"] = list[index].ProjectModes[i].TestDataModes[j].Remark.Trim() == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].Remark;
                                data.dt.Rows.InsertAt(dataRow, data.dt.Rows.Count + 1);
                            }
                            else if (list[index].ProjectModes[i].ModeType == 1)
                            {
                                dataRow = data.dt.NewRow();
                                dataRow["ID"] = "'" + list[index].BarCode.ToString();
                                dataRow["测试项"] = list[index].ProjectModes[i].projectName;
                                dataRow["Num"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].Num == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].Num);
                                dataRow["Coor-X"] = list[index].ProjectModes[i].TestDataModes[j].CoordX;
                                dataRow["Coor-Y"] = list[index].ProjectModes[i].TestDataModes[j].CoordY;
                                dataRow["Coor-Z"] = list[index].ProjectModes[i].TestDataModes[j].CoordZ;
                                dataRow["Coor-U"] = list[index].ProjectModes[i].TestDataModes[j].CoordU;
                                dataRow["Coor-V"] = list[index].ProjectModes[i].TestDataModes[j].CoordV;
                                dataRow["X"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].X == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].X);
                                dataRow["Y"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].Y == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].Y);
                                dataRow["Z"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].Z == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].Z);
                                dataRow["Low"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].Low == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].Low);
                                dataRow["High"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].High == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].High);
                                dataRow["RiseTime"] = list[index].ProjectModes[i].TestDataModes[j].RiseTime;
                                dataRow["FallTime"] = list[index].ProjectModes[i].TestDataModes[j].FallTime;
                                dataRow["完成时间"] = list[index].ProjectModes[i].TestDataModes[j].Time;
                                data.dt.Rows.InsertAt(dataRow, data.dt.Rows.Count + 1);
                            }
                            else if (list[index].ProjectModes[i].ModeType == 4)
                            {
                                dataRow = data.dt.NewRow();
                                dataRow["ID"] = "'" + list[index].BarCode.ToString();
                                dataRow["测试项"] = list[index].ProjectModes[i].projectName;
                                dataRow["Num"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].Num == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].Num);
                                dataRow["La"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].La == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].La);
                                dataRow["Lb"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].Lb.Trim() == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].Lb);
                                dataRow["CT"] = double.Parse(list[index].ProjectModes[i].TestDataModes[j].CT.Trim() == "" ? "0" : list[index].ProjectModes[i].TestDataModes[j].CT);
                                dataRow["完成时间"] = list[index].ProjectModes[i].TestDataModes[j].Time;
                                data.dt.Rows.InsertAt(dataRow, data.dt.Rows.Count + 1);
                            }
                            else
                            {
                                dataRow = data.dt.NewRow();
                                dataRow["ID"] = "'" + list[index].BarCode.ToString();
                                dataRow["测试项"] = list[index].ProjectModes[i].projectName;
                                dataRow["Num"] = list[index].ProjectModes[i].TestDataModes[j].Num.ToString().Trim();
                                if (list[index].ProjectModes[i].ModeType == 0)
                                { 
                                    dataRow["Coor-X"] = list[index].ProjectModes[i].TestDataModes[j].CoordX;
                                    dataRow["Coor-Y"] = list[index].ProjectModes[i].TestDataModes[j].CoordY;
                                    dataRow["Coor-Z"] = list[index].ProjectModes[i].TestDataModes[j].CoordZ;
                                    dataRow["Coor-U"] = list[index].ProjectModes[i].TestDataModes[j].CoordU;
                                    dataRow["Coor-V"] = list[index].ProjectModes[i].TestDataModes[j].CoordV;
                                }
                                dataRow["L"] =   list[index].ProjectModes[i].TestDataModes[j].L.ToString().Trim();
                                dataRow["X"] =   list[index].ProjectModes[i].TestDataModes[j].X.ToString().Trim();
                                dataRow["Y"] =   list[index].ProjectModes[i].TestDataModes[j].Y.ToString().Trim();
                                dataRow["Z"] =  list[index].ProjectModes[i].TestDataModes[j].Z.ToString().Trim();
                                dataRow["Cx"] =  list[index].ProjectModes[i].TestDataModes[j].Cx.ToString().Trim();
                                dataRow["Cy"] = list[index].ProjectModes[i].TestDataModes[j].Cy.ToString().Trim();
                                dataRow["u"] =   list[index].ProjectModes[i].TestDataModes[j].u.ToString().Trim();
                                dataRow["v"] =   list[index].ProjectModes[i].TestDataModes[j].v.ToString().Trim();
                                if (list[index].ProjectModes[i].ModeType == 0)
                                {
                                    if (Project.cfg.ShowLab)
                                    {
                                        dataRow["L*"] = list[index].ProjectModes[i].TestDataModes[j].Lcolor;
                                        dataRow["a*"] = list[index].ProjectModes[i].TestDataModes[j].Acolor;
                                        dataRow["b*"] = list[index].ProjectModes[i].TestDataModes[j].Bcolor;
                                    }
                                }
                                dataRow["CCT"] = list[index].ProjectModes[i].TestDataModes[j].CCT.ToString().Trim();
                                dataRow["备注"] = list[index].ProjectModes[i].TestDataModes[j].Remark.Trim();
                                data.dt.Rows.InsertAt(dataRow, data.dt.Rows.Count + 1);
                            }
                            
                        }

                        //dataRow = data.dt.NewRow();
                        //dataRow["ID"] = "";
                        //dataRow["测试项"] = "";
                        //dataRow["Num"] = "";
                        //dataRow["L"] = "";
                        //dataRow["X"] = "";
                        //dataRow["Y"] = "";
                        //dataRow["Z"] = "";
                        //dataRow["Cx"] = "";
                        //dataRow["Cy"] = "";
                        //dataRow["u"] = "";
                        //dataRow["v"] = "";
                        //dataRow["CCT"] = "";
                        //data.dt.Rows.InsertAt(dataRow, data.dt.Rows.Count + 1);

                        datas[list[index].ProjectModes[i].projectName] = data;
                    }


                }
                #region 注销

                //string sheetName = list[index].BarCode;
                //Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = GetSheetByName(xlWorkBook, sheetName);
                //if (xlWorkSheet == null)
                //{
                //    xlWorkSheet = xlWorkBook.Sheets.Add();
                //    xlWorkSheet.Name = sheetName;
                //}

                //DataTable dt = list[index].table;
                //int row = dt.Rows.Count;
                //int col = dt.Columns.Count;
                //object[,] mdata = new object[row+1, col];

                //for (int i = 0; i < col; i++)
                //{
                //    mdata[0, i] = dt.Columns[i].ToString();
                //}

                //for (int j = 0; j < row; j++)
                //{
                //    for (int i = 0; i < col; i++)
                //    {
                //        string str = dt.Rows[j][i].ToString();
                //        double data;
                //        if (double.TryParse(str, out data)) { mdata[j + 1, i] = data; }
                //        else { mdata[j + 1, i] = str; }
                //    }
                //}
                //Microsoft.Office.Interop.Excel.Range DataRange = xlWorkSheet.Range[xlWorkSheet.Cells[1, 1], xlWorkSheet.Cells[row + 1, col]];
                //DataRange.Value2 = mdata;

                #endregion

            }

            

            for (int index = 0; index < datas.Count; index++)
            {
                string[] strs = datas.Keys.ToArray();
                if (datas[strs[index]] ==null)
                {
                    continue;
                }

                object[,] mdata = null;
                if (datas[strs[index]].ModeType==2)
                {
                    int row = datas[strs[index]].dt.Rows.Count;
                    int col = datas[strs[index]].dt.Columns.Count;
                    mdata = new Object[col, row + 1];
                    List<int> listints = new List<int>();

                    for (int k = 0; k < col; k++)
                    {
                        string Temp = datas[strs[index]].dt.Columns[k].ToString();
                        mdata[k, 0] = Temp;
                        if (Temp.IndexOf("Num") != -1 || Temp.IndexOf("CCT") != -1)
                        {
                            listints.Add(k);
                        }
                    }
                    for (int j = 0; j < col; j++)
                    {
                        for (int k = 0; k < row; k++)
                        {
                            object str = datas[strs[index]].dt.Rows[k][j];
                            //double data;
                            //if (double.TryParse(str, out data)) { mdata[j + 1, k] = data; }
                            //else { mdata[j + 1, k] = str; }

                            mdata[j, k+1] = str;
                        }
                    }
                    Microsoft.Office.Interop.Excel.Range DataRange = datas[strs[index]].xlWorkSheet.Range[datas[strs[index]].xlWorkSheet.Cells[1, 1], datas[strs[index]].xlWorkSheet.Cells[col, row + 1]];
                    DataRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlLineStyleNone;
                    DataRange.Value2 = mdata;
                    //DataRange.NumberFormat = "0.0000";
                    DataRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                    DataRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                    DataRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                }
                else
                {
                    int row = datas[strs[index]].dt.Rows.Count;
                    int col = datas[strs[index]].dt.Columns.Count; 
                    mdata = new Object[row + 1, col];

                    List<int> listints = new List<int>();

                    for (int k = 0; k < col; k++)
                    {
                        string Temp = datas[strs[index]].dt.Columns[k].ToString();
                        mdata[0, k] = Temp;
                        if (Temp.IndexOf("Num") != -1 || Temp.IndexOf("CCT") != -1)
                        {
                            listints.Add(k);
                        }
                    }
                    for (int j = 0; j < row; j++)
                    {
                        for (int k = 0; k < col; k++)
                        {
                            object str = datas[strs[index]].dt.Rows[j][k];
                            //double data;
                            //if (double.TryParse(str, out data)) { mdata[j + 1, k] = data; }
                            //else { mdata[j + 1, k] = str; }

                            mdata[j + 1, k] = str;
                        }
                    }
                    Microsoft.Office.Interop.Excel.Range DataRange = datas[strs[index]].xlWorkSheet.Range[datas[strs[index]].xlWorkSheet.Cells[1, 1], datas[strs[index]].xlWorkSheet.Cells[row + 1, col]];
                    DataRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlLineStyleNone;
                    DataRange.Value2 = mdata;
                    //DataRange.NumberFormat = "0.0000";
                    DataRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                    DataRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                    DataRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                }
                
                
                
                //for (int j = 0; j < listints.Count; j++)
                //{
                //    Microsoft.Office.Interop.Excel.Range DataRange1 = datas[index].xlWorkSheet.Range[datas[index].xlWorkSheet.Cells[1, listints[j]-1], datas[index].xlWorkSheet.Cells[row + 1, listints[j]]];
                //    DataRange1.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlLineStyleNone;
                //    DataRange1.Value2 = mdata;
                //    DataRange1.NumberFormat = "0";
                //    DataRange1.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                //    DataRange1.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                //    DataRange1.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                //}

            }

            xlWorkBook.Close(true);
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
        }


        private static Microsoft.Office.Interop.Excel.Worksheet GetSheetByName(Microsoft.Office.Interop.Excel.Workbook xlWorkBook, string sheetName)
        {
            for (int i = 0; i < xlWorkBook.Sheets.Count; i++)
            {
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = xlWorkBook.Sheets[i + 1];
                if (xlWorkSheet.Name == sheetName)
                    return xlWorkSheet;
            }
            return null;
        }
    }


    public class Data
    {
        public int ModeType { get; set; }
        public DataTable dt { get; set; }
        public Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet { get; set; }
    }
    //文本编辑
    class FolderHelper
    {
        static public string GetRecipeFolder()
        {

            string layoutFolder = GetExeParentFolder() + "Recipes\\";
            if (!Directory.Exists(layoutFolder))
                Directory.CreateDirectory(layoutFolder);
            return layoutFolder;
        }

        static public string GetImageFolder()
        {
            string acquiredImagesFolder = GetExeParentFolder() + "AcquiredImages\\";
            if (!Directory.Exists(acquiredImagesFolder))
                Directory.CreateDirectory(acquiredImagesFolder);
            return acquiredImagesFolder;
        }


        static public string GetExeFolder()
        {
            string s = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return s + "\\";
        }

        static public string GetExeParentFolder()
        {
            string s = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
            //int index = s.LastIndexOf("\\");
            //return s.Substring(0, index) + "\\";
            return s;
        }

        internal static string GetMiscFolder()
        {
            string miscFolder = GetExeParentFolder() + "Misc\\";
            if (!Directory.Exists(miscFolder))
                Directory.CreateDirectory(miscFolder);
            return miscFolder;
        }

        internal static string GetDefaultSaveFolder()
        {
            string saveFolder = GetExeParentFolder() + "Save\\";
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);
            return saveFolder;
        }

        internal static string GetTemplateFile()
        {
            string templateFolder = GetExeParentFolder() + "Template\\";
            if (!Directory.Exists(templateFolder))
                throw new Exception("No Template folder!");
            string file = templateFolder + "testResult.xlsx";
            if (!File.Exists(file))
                throw new Exception("No Template file!");
            return file;
        }
        internal static string GetSingleExcelTemplateFile()
        {
            string templateFolder = GetExeParentFolder() + "Template\\";
            if (!Directory.Exists(templateFolder))
                throw new Exception("No Template folder!");
            string file = templateFolder + "Book1.xlsx";
            if (!File.Exists(file))
                throw new Exception("No Template file!");
            return file;
        }
        internal static string GetDefaultTemplateFile()
        {
            string templateFolder = GetExeParentFolder() + "Template\\";
            if (!Directory.Exists(templateFolder))
                throw new Exception("No Template folder!");
            string file = templateFolder + "w.txt";
            if (!File.Exists(file))
                throw new Exception("No Template file!");
            return file;
        }

        internal static string GetConfigFolder()
        {
            string configFolder = GetExeParentFolder() + "config\\";
            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);
            return configFolder;
        }
    }


}
