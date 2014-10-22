using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Daily           //cdy
{
    public class ClinkerEnergyConsumption_TargetCompletion
    {
        private static string connectionString;
        private static TZHelper tzHelper;

        static ClinkerEnergyConsumption_TargetCompletion()//静态构造函数
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
            DataTable temp = tzHelper.CreateTableStructure("report_EnergyConsumption_TargetCompletion");//获得目标表结构
            //获得熟料生产线能耗计划报表(年)
            DataTable table = tzHelper.GetReportData("tz_Report", organizeID, year, "report_ClinkerProductionLineEnergyConsumptionSchedule");
            foreach (DataRow dr in table.Rows)
            {
                DataRow row = temp.NewRow();
                row["Name"] = dr["IndicatorName"];
                row["Monthly_Target"] = dr[filedNameOfMounth];
                row["Yearly_Target"] = dr["Annual"];
                temp.Rows.Add(row);
            }

            //第二步
            //获得熟料生产线产量及消耗统计报表月报
            DataTable table_MounthCL = tzHelper.GetReportData("tz_Report", organizeID, year + "-" + mounth, "report_ClinkerMonthlyOutput");
            DataRow row_DayCL = table_MounthCL.Select("vDate='" + day + "'")[0];//取出指定日期所对应的行
            DataRow row_sumMounthCL = table_MounthCL.Select("vDate='合计'")[0];//取出月报表产量合计行           

            if (row_DayCL["ClinkerProductionSum"] is DBNull) { row_DayCL["ClinkerProductionSum"] = 0; }
            if (row_DayCL["PowerGenerationSum"] is DBNull) { row_DayCL["PowerGenerationSum"] = 0; }
            if (row_sumMounthCL["ClinkerProductionSum"] is DBNull) { row_sumMounthCL["ClinkerProductionSum"] = 0; }
            if (row_sumMounthCL["PowerGenerationSum"] is DBNull) { row_sumMounthCL["PowerGenerationSum"] = 0; }
            //熟料产量（本日完成）
            temp.Rows[0]["Today_Completion"] = row_DayCL["ClinkerProductionSum"];
            //熟料产量（本月累计）
            temp.Rows[0]["Monthly_Accumulative"] = row_sumMounthCL["ClinkerProductionSum"];
            //发电量（本日完成）
            temp.Rows[1]["Today_Completion"] = row_DayCL["PowerGenerationSum"];
            //发电量（本月累计）
            temp.Rows[1]["Monthly_Accumulative"] = row_sumMounthCL["PowerGenerationSum"];
            //吨熟料发电量（本日完成）
            double AmounttoCoal_day = Convert.ToDouble(row_DayCL["PowerGenerationSum"]);//取出本日行合计发电量的合计存到变量AmounttoCoal_day中
            temp.Rows[4]["Today_Completion"] = AmounttoCoal_day / Convert.ToDouble(row_DayCL["ClinkerProductionSum"]);
            //吨熟料发电量（本月累计）        
            double AmounttoCoal_MounthLJ = Convert.ToDouble(row_sumMounthCL["PowerGenerationSum"]);//取出合计行合计发电量的合计存到变量AmounttoCoal_MounthLJ中
            temp.Rows[4]["Monthly_Accumulative"] = AmounttoCoal_MounthLJ / Convert.ToDouble(row_sumMounthCL["ClinkerProductionSum"]);

            //第三步
            //获得熟料生产线产量及消耗统计报表年报
            DataTable table_YearCL = tzHelper.GetReportData("tz_Report", organizeID, year, "report_ClinkerYearlyOutput");
            DataRow row_sumYearCL = table_YearCL.Select("vDate='合计'")[0];//取出年报表产量合计行
            //合计：熟料产量合计、发电量合计、合计用煤合计→本年累计
            if (row_sumYearCL["ClinkerProductionSum"] is DBNull) { row_sumYearCL["ClinkerProductionSum"] = 0; }
            if (row_sumYearCL["PowerGenerationSum"] is DBNull) { row_sumYearCL["PowerGenerationSum"] = 0; }
            //熟料产量（本年累计）
            temp.Rows[0]["Yearly_Accumulative"] = row_sumYearCL["ClinkerProductionSum"];
            //发电量（本年累计）
            temp.Rows[1]["Yearly_Accumulative"] = row_sumYearCL["PowerGenerationSum"];
            //吨熟料发电量（本年累计）
            double AmounttoCoal_YearLJ = Convert.ToDouble(row_sumYearCL["PowerGenerationSum"]);//取出合计行合计发电量的合计存到变量AmounttoCoal_YearLJ中
            temp.Rows[4]["Yearly_Accumulative"] = AmounttoCoal_YearLJ / Convert.ToDouble(row_sumYearCL["ClinkerProductionSum"]);

            //第四步
            //获得熟料生产线合计用电量统计月报表
            DataTable table_MounthDL = tzHelper.GetReportData("tz_Report", organizeID, year + "-" + mounth, "report_ClinkerMonthlyElectricity_sum");
            DataRow row_sumMounthDL = table_MounthDL.Select("vDate='合计'")[0];//取出月报表电量合计行
            DataRow row_day = table_MounthDL.Select("vDate='" + day + "'")[0];//取出本日用电量行

            //吨熟料电耗（本日完成）
            temp.Rows[3]["Today_Completion"] = Convert.ToDouble(row_day["AmounttoSum"]) / Convert.ToDouble(row_DayCL["ClinkerProductionSum"]);
            //吨熟料电耗（本月累计）
            temp.Rows[3]["Monthly_Accumulative"] = Convert.ToDouble(row_sumMounthDL["AmounttoSum"]) / Convert.ToDouble(row_sumMounthCL["ClinkerProductionSum"]);
            //生料磨电耗（本日完成）
            temp.Rows[5]["Today_Completion"] = Convert.ToDouble(row_day["RawBatchGrindingSum"]) / Convert.ToDouble(row_DayCL["ClinkerProductionSum"]);
            //生料磨电耗（本月累计）
            temp.Rows[5]["Monthly_Accumulative"] = Convert.ToDouble(row_sumMounthDL["RawBatchGrindingSum"]) / Convert.ToDouble(row_sumMounthCL["ClinkerProductionSum"]);
            //煤磨电耗（本日完成）
            temp.Rows[6]["Today_Completion"] = Convert.ToDouble(row_day["CoalMillSystemSum"]) / Convert.ToDouble(row_DayCL["ClinkerProductionSum"]);
            //煤磨电耗（本月累计）
            temp.Rows[6]["Monthly_Accumulative"] = Convert.ToDouble(row_sumMounthDL["CoalMillSystemSum"]) / Convert.ToDouble(row_sumMounthCL["ClinkerProductionSum"]);


            //获得熟料生产线合计用电量统计年报表
            DataTable table_YearDL = tzHelper.GetReportData("tz_Report", organizeID, year, "report_ClinkerYearlyElectricity_sum");
            DataRow row_sumYearDL = table_MounthDL.Select("vDate='合计'")[0];//取出年报表合计用电量行
            //吨熟料电耗（本年累计）
            temp.Rows[3]["Yearly_Accumulative"] = Convert.ToDouble(row_sumYearDL["AmounttoSum"]) / Convert.ToDouble(row_sumYearCL["ClinkerProductionSum"]);
            //生料磨电耗（本年累计）
            temp.Rows[5]["Yearly_Accumulative"] = Convert.ToDouble(row_sumYearDL["RawBatchGrindingSum"]) / Convert.ToDouble(row_sumYearCL["ClinkerProductionSum"]);
            //煤磨电耗（本年累计）
            temp.Rows[6]["Yearly_Accumulative"] = Convert.ToDouble(row_sumYearDL["CoalMillSystemSum"]) / Convert.ToDouble(row_sumYearCL["ClinkerProductionSum"]);

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
