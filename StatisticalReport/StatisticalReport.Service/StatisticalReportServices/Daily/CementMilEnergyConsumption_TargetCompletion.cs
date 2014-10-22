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
            DataTable table_MounthCL = tzHelper.GetReportData("tz_Report", organizeID, year + "-" + mounth, "report_CementMillMonthlyOutput");
            table_MounthCL = ReportHelper.MyTotalOn(table_MounthCL, "vDate", "CementProductionSum");
            DataTable temp_MounthCL = table_MounthCL.Copy();
            foreach (DataRow dr in temp_MounthCL.Rows)
            {
                dr["vDate"] = "合计";
            }
            temp_MounthCL = ReportHelper.MyTotalOn(temp_MounthCL, "vDate", "CementProductionSum");
            table_MounthCL.Merge(temp_MounthCL);
            DataRow row_DayCL = table_MounthCL.Select("vDate='" + day + "'")[0];//取出指定日期所对应的行
            DataRow row_sumMounthCL = table_MounthCL.Select("vDate='合计'")[0];//取出月报表产量合计行           

            if (row_DayCL["CementProductionSum"] is DBNull) { row_DayCL["CementProductionSum"] = 0; }
            if (row_sumMounthCL["CementProductionSum"] is DBNull) { row_sumMounthCL["CementProductionSum"] = 0; }
            //水泥产量（本日完成）
            temp.Rows[0]["Today_Completion"] = row_DayCL["CementProductionSum"];
            //水泥产量（本月累计）
            temp.Rows[0]["Monthly_Accumulative"] = row_sumMounthCL["CementProductionSum"];

            //第三步
            //获得水泥生产线产量及消耗统计报表年报
            DataTable table_YearCL = tzHelper.GetReportData("tz_Report", organizeID, year, "report_CementMillYearlyOutput");
            foreach (DataRow dr in table_YearCL.Rows)
            {
                dr["vDate"] = "合计";
            }
            table_YearCL = ReportHelper.MyTotalOn(table_YearCL, "vDate", "CementProductionSum");
            DataRow row_sumYearCL = table_YearCL.Select("vDate='合计'")[0];//取出年报表产量合计行
            //合计：水泥产量合计、发电量合计、合计用煤合计→本年累计
            if (row_sumYearCL["CementProductionSum"] is DBNull) { row_sumYearCL["CementProductionSum"] = 0; }
            //水泥产量（本年累计）
            temp.Rows[0]["Yearly_Accumulative"] = row_sumYearCL["CementProductionSum"];

            //第四步
            //获得水泥生产线合计用电量统计月报表(并且将报表处理一下)
            DataTable table_MounthDL = tzHelper.GetReportData("tz_Report", organizeID, year + "-" + mounth, "report_CementMillMonthlyElectricity_sum");
            table_MounthDL = ReportHelper.MyTotalOn(table_MounthDL, "vDate", "AmounttoSum,CementGrindingSum");
            DataTable temp_MounthDL = table_MounthDL.Copy();
            foreach (DataRow dr in temp_MounthDL.Rows)
            {
                dr["vDate"] = "合计";
            }
            temp_MounthDL = ReportHelper.MyTotalOn(temp_MounthDL, "vDate", "AmounttoSum,CementGrindingSum");
            table_MounthDL.Merge(temp_MounthDL);
            DataRow row_sumMounthDL = table_MounthDL.Select("vDate='合计'")[0];//取出月报表电量合计行
            DataRow row_day = table_MounthDL.Select("vDate='" + day + "'")[0];//取出本日用电量行
            if (row_day["AmounttoSum"] is DBNull) { row_day["AmounttoSum"] = 0; }
            if (row_sumMounthDL["AmounttoSum"] is DBNull) { row_sumMounthDL["AmounttoSum"] = 0; }
            if (row_day["CementGrindingSum"] is DBNull) { row_day["CementGrindingSum"] = 0; }
            if (row_sumMounthDL["CementGrindingSum"] is DBNull) { row_sumMounthDL["CementGrindingSum"] = 0; }
            //吨水泥电耗（本日完成）
            temp.Rows[1]["Today_Completion"] = Convert.ToDouble(row_day["AmounttoSum"]) / Convert.ToDouble(row_DayCL["CementProductionSum"]);
            //吨水泥电耗（本月累计）
            temp.Rows[1]["Monthly_Accumulative"] = Convert.ToDouble(row_sumMounthDL["AmounttoSum"]) / Convert.ToDouble(row_sumMounthCL["CementProductionSum"]);
            //水泥磨电耗（本日完成）
            temp.Rows[2]["Today_Completion"] = Convert.ToDouble(row_day["CementGrindingSum"]) / Convert.ToDouble(row_DayCL["CementProductionSum"]);
            //水泥磨电耗（本月累计）
            temp.Rows[2]["Monthly_Accumulative"] = Convert.ToDouble(row_sumMounthDL["CementGrindingSum"]) / Convert.ToDouble(row_sumMounthCL["CementProductionSum"]);


            //获得水泥生产线合计用电量统计年报表
            DataTable table_YearDL = tzHelper.GetReportData("tz_Report", organizeID, year, "report_CementMillYearlyElectricity_sum");
            foreach (DataRow dr in table_YearDL.Rows)
            {
                dr["vDate"] = "合计";
            }
            DataRow row_sumYearDL = table_YearDL.Select("vDate='合计'")[0];//取出年报表合计用电量行
            //吨水泥电耗（本年累计）
            temp.Rows[1]["Yearly_Accumulative"] = Convert.ToDouble(row_sumYearDL["AmounttoSum"]) / Convert.ToDouble(row_sumYearCL["CementProductionSum"]);
            //水泥磨电耗（本年累计）
            temp.Rows[2]["Yearly_Accumulative"] = Convert.ToDouble(row_sumYearDL["CementGrindingSum"]) / Convert.ToDouble(row_sumYearCL["CementProductionSum"]);

            foreach (DataRow dr in temp.Rows)
            {
                if (dr["Monthly_Target"] is DBNull) { dr["Monthly_Target"] = 0; }
                if (dr["Monthly_Accumulative"] is DBNull) { dr["Monthly_Accumulative"] = 0; }
                if (dr["Yearly_Target"] is DBNull) { dr["Yearly_Target"] = 0; }
                if (dr["Yearly_Accumulative"] is DBNull) { dr["Yearly_Accumulative"] = 0; }
                dr["Monthly_Gap"] = Convert.ToDouble(dr["Monthly_Target"]) - Convert.ToDouble(dr["Monthly_Accumulative"]);
                dr["Yearly_Gap"] = Convert.ToDouble(dr["Yearly_Target"]) - Convert.ToDouble(dr["Yearly_Accumulative"]);
            }


            return temp;
        }
    }
}
