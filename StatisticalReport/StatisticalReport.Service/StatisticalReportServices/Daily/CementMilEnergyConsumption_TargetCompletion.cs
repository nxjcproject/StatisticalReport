using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Daily             //cdy
{
    public class CementMilEnergyConsumption_TargetCompletion
    {
        private static string connectionString;
        private static TZHelper tzHelper;

        static CementMilEnergyConsumption_TargetCompletion()//静态构造函数
        {
            connectionString = ConnectionStringFactory.NXJCConnectionString;
            tzHelper = new TZHelper(connectionString);
        }
        public static DataTable TableQuery(string organizeID, string date)
        {
            string[] Date = date.Split('-', ',');
            string year = Date[0];
            string mounth = Date[1];
            string day = Date[2];
            string filedNameOfMounth;//月份字段名
            switch (mounth)
            {
                case "01":
                    filedNameOfMounth = "January";
                    break;
                case "02":
                    filedNameOfMounth = "February";
                    break;
                case "03":
                    filedNameOfMounth = "March";
                    break;
                case "04":
                    filedNameOfMounth = "April";
                    break;
                case "05":
                    filedNameOfMounth = "May";
                    break;
                case "06":
                    filedNameOfMounth = "June";
                    break;
                case "07":
                    filedNameOfMounth = "July";
                    break;
                case "08":
                    filedNameOfMounth = "August";
                    break;
                case "09":
                    filedNameOfMounth = "September";
                    break;
                case "10":
                    filedNameOfMounth = "October";
                    break;
                case "11":
                    filedNameOfMounth = "November";
                    break;
                case "12":
                    filedNameOfMounth = "December";
                    break;
                default:
                    filedNameOfMounth = "January";
                    break;

            }
            //第一步
            DataTable temp = tzHelper.CreateTableStructure("report_CementMilEnergyConsumption_TargetCompletion");//获得目标表结构
            //获得水泥生产线能耗计划报表(年)
            DataTable table = tzHelper.GetReportData("tz_Report", organizeID, year, "report_CementMillProductionLineProductionLineEnergyConsumptionSchedule");
            foreach (DataRow dr in table.Rows)
            {
                DataRow row = temp.NewRow();
                row["Name"] = dr["IndicatorName"];
                row["Monthly_Target"] = dr[filedNameOfMounth];
                row["Yearly_Target"] = dr["Annual"];
                temp.Rows.Add(row);
            }

            //第二步
            //获得水泥生产线产量及消耗统计报表月报（水泥生产线报表没有最后的合计行）
            DataTable table_MounthCL = tzHelper.GetReportData("tz_Report", organizeID, year + "-" + mounth, "table_CementMillMonthlyOutput");
            table_MounthCL = ReportHelper.MyTotalOn(table_MounthCL, "vDate", "CementProductionSum");
            DataTable temp_MounthCL = table_MounthCL.Copy();
            foreach (DataRow dr in temp_MounthCL.Rows)
            {
                dr["vDate"] = "合计";
            }
            temp_MounthCL = ReportHelper.MyTotalOn(temp_MounthCL, "vDate", "CementProductionSum");
            table_MounthCL.Merge(temp_MounthCL);
            DataRow[] row_DayCLs = table_MounthCL.Select("vDate='" + day + "'");
            DataRow[] row_sumMounthCLs = table_MounthCL.Select("vDate='合计'");


            //水泥产量（本日完成）
            if (row_DayCLs.Count() != 0)
            {
                temp.Rows[0]["Today_Completion"] = row_DayCLs[0]["CementProductionSum"];
            }
            //水泥产量（本月累计）
            if (row_DayCLs.Count() != 0)
            {
                temp.Rows[0]["Monthly_Accumulative"] = row_sumMounthCLs[0]["CementProductionSum"];
            }

            //第三步
            //获得水泥生产线产量及消耗统计报表年报
            DataTable table_YearCL = tzHelper.GetReportData("tz_Report", organizeID, year, "table_CementMillYearlyOutput");
            foreach (DataRow dr in table_YearCL.Rows)
            {
                dr["vDate"] = "合计";
            }
            table_YearCL = ReportHelper.MyTotalOn(table_YearCL, "vDate", "CementProductionSum");
            DataRow[] row_sumYearCLs = table_YearCL.Select("vDate='合计'");//取出年报表产量合计行
            //合计：水泥产量合计、发电量合计、合计用煤合计→本年累计
            //水泥产量（本年累计）
            if (row_sumYearCLs.Count() != 0)
            {
                temp.Rows[0]["Yearly_Accumulative"] = row_sumYearCLs[0]["CementProductionSum"];
            }

            //第四步
            //获得水泥生产线合计用电量统计月报表(并且将报表处理一下)
            DataTable table_MounthDL = tzHelper.GetReportData("tz_Report", organizeID, year + "-" + mounth, "table_CementMillMonthlyElectricity_sum");
            table_MounthDL = ReportHelper.MyTotalOn(table_MounthDL, "vDate", "AmounttoSum,CementGrindingSum");
            DataTable temp_MounthDL = table_MounthDL.Copy();
            foreach (DataRow dr in temp_MounthDL.Rows)
            {
                dr["vDate"] = "合计";
            }
            temp_MounthDL = ReportHelper.MyTotalOn(temp_MounthDL, "vDate", "AmounttoSum,CementGrindingSum");
            table_MounthDL.Merge(temp_MounthDL);
            DataRow[] row_sumMounthDLs = table_MounthDL.Select("vDate='合计'");//取出月报表电量合计行
            DataRow[] row_days = table_MounthDL.Select("vDate='" + day + "'");//取出本日用电量
            //吨水泥电耗（本日完成）
            if (row_days.Count() != 0 && row_DayCLs.Count() != 0 && 0 != MyToDecimal(row_DayCLs[0]["CementProductionSum"]))
            {
                temp.Rows[1]["Today_Completion"] = Convert.ToInt64((decimal)(row_days[0]["AmounttoSum"])
                    / (decimal)(row_DayCLs[0]["CementProductionSum"]));
            }
            //吨水泥电耗（本月累计）
            if (row_sumMounthCLs.Count() != 0 && MyToDecimal(row_sumMounthCLs[0]["CementProductionSum"]) != 0 && MyToDecimal(row_sumMounthDLs[0]["AmounttoSum"])!=0)
            {
                temp.Rows[1]["Monthly_Accumulative"] = Convert.ToInt64(MyToDecimal(row_sumMounthDLs[0]["AmounttoSum"]) / MyToDecimal(row_sumMounthCLs[0]["CementProductionSum"]));
            }
            //水泥磨电耗（本日完成）
            if (row_days.Count() != 0 && row_DayCLs.Count() != 0 && MyToDecimal(row_DayCLs[0]["CementProductionSum"]) != 0)
            {
                temp.Rows[2]["Today_Completion"] = Convert.ToInt64(MyToDecimal(row_days[0]["CementGrindingSum"]) / MyToDecimal(row_DayCLs[0]["CementProductionSum"]));
            }
            //水泥磨电耗（本月累计）
            if (row_sumMounthCLs.Count() != 0 && MyToDecimal(row_sumMounthCLs[0]["CementProductionSum"]) != 0 && MyToDecimal(row_sumMounthDLs[0]["CementGrindingSum"])!=0)
            {
                temp.Rows[2]["Monthly_Accumulative"] = Convert.ToInt64(MyToDecimal(row_sumMounthDLs[0]["CementGrindingSum"]) / MyToDecimal(row_sumMounthCLs[0]["CementProductionSum"]));
            }


            //获得水泥生产线合计用电量统计年报表
            DataTable table_YearDL = tzHelper.GetReportData("tz_Report", organizeID, year, "table_CementMillYearlyElectricity_sum");
            foreach (DataRow dr in table_YearDL.Rows)
            {
                dr["vDate"] = "合计";
            }
            DataRow[] row_sumYearDLs = table_YearDL.Select("vDate='合计'");

            //吨水泥电耗（本年累计）
            if (row_sumYearCLs.Count() != 0)
            {
                if (row_sumYearDLs.Count() != 0 && MyToDecimal(row_sumYearCLs[0]["CementProductionSum"]) != 0)
                {
                    temp.Rows[1]["Yearly_Accumulative"] = Convert.ToInt64(MyToDecimal(row_sumYearDLs[0]["AmounttoSum"]) / MyToDecimal(row_sumYearCLs[0]["CementProductionSum"]));
                }
            }
            //水泥磨电耗（本年累计）
            if (row_sumYearCLs.Count() != 0)
            {
                if (row_sumYearDLs.Count() != 0 && MyToDecimal(row_sumYearCLs[0]["CementProductionSum"]) != 0)
                {
                    temp.Rows[2]["Yearly_Accumulative"] = Convert.ToInt64(MyToDecimal(row_sumYearDLs[0]["CementGrindingSum"]) / MyToDecimal(row_sumYearCLs[0]["CementProductionSum"]));
                }
            }
            foreach (DataRow dr in temp.Rows)
            {
                if(dr["Monthly_Accumulative"] is DBNull)
                { dr["Monthly_Accumulative"] = 0; }
                if (dr["Yearly_Accumulative"] is DBNull)
                { dr["Yearly_Accumulative"] = 0; }  
                dr["Monthly_Gap"] = Convert.ToInt64(MyToDecimal(dr["Monthly_Target"]) - MyToDecimal(dr["Monthly_Accumulative"]));
                dr["Yearly_Gap"] = Convert.ToInt64(MyToDecimal(dr["Yearly_Target"]) - MyToDecimal(dr["Yearly_Accumulative"]));
            }


            return temp;
        }
        public static decimal MyToDecimal(object obj)
        {
            if (obj is DBNull)
            {
                obj = 0;
                return Convert.ToDecimal(obj);
            }
            else
            {
                return Convert.ToDecimal(obj);
            }
        }
    }
}
