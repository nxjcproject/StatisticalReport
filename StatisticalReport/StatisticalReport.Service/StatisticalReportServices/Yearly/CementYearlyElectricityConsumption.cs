using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Yearly
{
    public class CementYearlyElectricityConsumption                      //cdy
    {
        private static TZHelper _tzHelper;

        static CementYearlyElectricityConsumption()
        {
            string connString = ConnectionStringFactory.NXJCConnectionString;
            _tzHelper = new TZHelper(connString);
        }

        public static DataTable TableQuery(string organizeID, string year)
        {
            DataTable temp1;
            temp1 = _tzHelper.CreateTableStructure("report_CementYearlyElectricityConsumption");//目标数据表
            int MonthEnd;
            if (DateTime.Now.Year.ToString() != year)
            { MonthEnd = 12; }
            else
            { MonthEnd = Convert.ToInt32(DateTime.Now.Month); }

            //形成产量数据----------------start----------------------
            DataTable temp_cl;
            temp_cl = _tzHelper.CreateTableStructure("table_CementMillMonthlyOutput");
            for (int i = 1; i <= MonthEnd; i++) //形成每月产量数据，包括累计
            {
                DataTable temp2;
                string date = year + "-" + ReportHelper.MyToString(i, 2, 0);
                temp2 = _tzHelper.GetReportData("tz_Report", organizeID, date, "table_CementMillMonthlyOutput");
                if (temp2.Rows.Count == 0)
                { continue; }
                foreach (DataRow dr in temp2.Rows)
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["CementTypes"] = dr["CementTypes"];
                    row["Output_Monthly"] = ReportHelper.MyToInt64(dr["CementProductionSum"]);//要改
                    temp1.Rows.Add(row);
                }
                temp_cl.Merge(temp2);//产量并入累计暂存
                foreach (DataRow dr in temp_cl.Rows)//累计产量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["CementTypes"] = dr["CementTypes"];//要改
                    row["Output_Accumulative"] = ReportHelper.MyToInt64(dr["CementProductionSum"]);//要改
                    temp1.Rows.Add(row);
                }
            }
            //-----------------------END------------------------------

            //形成电量数据----------------start-----------------------
            DataTable temp_dl;
            temp_dl = _tzHelper.CreateTableStructure("table_CementMillMonthlyElectricity_sum");
            for (int i = 1; i <= MonthEnd; i++)//形成每月电量数据，包括累计
            {
                DataTable temp2;
                string date = year + "-" + ReportHelper.MyToString(i, 2, 0);
                temp2 = _tzHelper.GetReportData("tz_Report", organizeID, date, "table_CementMillMonthlyElectricity_sum");
                if (temp2.Rows.Count == 0)
                { continue; }
                foreach (DataRow dr in temp2.Rows)//本月电量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["CementTypes"] = dr["CementTypes"];//要改
                    row["Electricity_Monthly"] = ReportHelper.MyToInt64(dr["AmounttoSum"]);//要改
                    temp1.Rows.Add(row);
                }
                temp_dl.Merge(temp2);    //电量并入累计暂存
                foreach (DataRow dr in temp_dl.Rows)//累计电量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["CementTypes"] = dr["CementTypes"];//要改
                    row["Electricity_Accumulative"] = ReportHelper.MyToInt64(dr["AmounttoSum"]);//要改
                    temp1.Rows.Add(row);
                }
            }
            //-----------------------END------------------------------
            DataTable temp = ReportHelper.GroupByTotal(temp1, "vDate,CementTypes", "Output_Monthly,Output_Accumulative,Electricity_Monthly,Electricity_Accumulative");    // temp每月明细
            DataTable table4 = ReportHelper.MyTotalOn(temp, "vDate", "Output_Monthly,Output_Accumulative,Electricity_Monthly,Electricity_Accumulative");//表4每月合计
            foreach (DataRow dr in table4.Rows)//将表4水泥品种字段中的数据全部用合计替换
            {
                dr["CementTypes"] = "总总合计";
            }
            temp.Merge(table4);

            DataTable result = ReportHelper.SortTable(temp, new string[] { "vDate", "CementTypes" });//按照月份和水泥品种排序
            foreach (DataRow dr in result.Rows)
            {
                
                string PZ = dr["CementTypes"].ToString().Trim();

                if (dr["CementTypes"].ToString() != "总总合计")
                {
                    //string test = "asd";//测试
                    decimal ZHXS = _tzHelper.GetConvertCoefficient(PZ);
                    dr["ConvertCoefficient"] = ZHXS;
                }
                else
                {
                    dr["CementTypes"] = "合计";
                }
                if (ReportHelper.MyToDecimal(dr["Output_Monthly"]) != 0)
                {
                    dr["ElectricityConsumption_Monthly"] = ReportHelper.MyToDecimal(dr["Electricity_Monthly"]) / ReportHelper.MyToDecimal(dr["Output_Monthly"]);
                }
                if (dr["CementTypes"].ToString() != "合计" && ReportHelper.MyToDecimal(dr["ConvertCoefficient"]) != 0)
                {
                    if (dr["ElectricityConsumption_Monthly"] is DBNull)
                    {
                        dr["ElectricityConsumption_Monthly"] = 0;
                    }
                    dr["Convert_ElectricityConsumption_Monthly"] = ReportHelper.MyToDecimal(dr["ElectricityConsumption_Monthly"]) / ReportHelper.MyToDecimal(dr["ConvertCoefficient"]);
                }


                if (ReportHelper.MyToDecimal(dr["Output_Accumulative"]) != 0)
                {
                    dr["ElectricityConsumption_Accumulative"] = ReportHelper.MyToDecimal(dr["Electricity_Accumulative"]) / ReportHelper.MyToDecimal(dr["Output_Accumulative"]);
                }
                if (dr["CementTypes"].ToString() != "合计")
                {
                    if (dr["ElectricityConsumption_Accumulative"] is DBNull)
                    {
                        dr["ElectricityConsumption_Accumulative"] = 0;
                    }
                    dr["Convert_ElectricityConsumption_Accumulative"] = ReportHelper.MyToDecimal(dr["ElectricityConsumption_Accumulative"]) / ReportHelper.MyToDecimal(dr["ConvertCoefficient"]);                  
                }

            }
            return result; 
        }

        //public static decimal ReportHelper.MyToDecimal(object obj)
        //{
        //    if (obj is DBNull)
        //    {
        //        obj = 0;
        //        return Convert.ToDecimal(obj);
        //    }
        //    else
        //    {
        //        return Convert.ToDecimal(obj);
        //   }
        //}
    }
}
