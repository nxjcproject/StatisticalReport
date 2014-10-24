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
            DataTable temp = tzHelper.CreateTableStructure("report_ClinkerEnergyConsumption_TargetCompletion");//获得目标表结构
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
            DataTable table_MounthCL = tzHelper.GetReportData("tz_Report", organizeID, year + "-" + mounth, "table_ClinkerMonthlyOutput");
            DataRow[] row_DayCLs = table_MounthCL.Select("vDate='" + day + "'");//取出指定日期所对应的行
            DataRow[] row_sumMounthCLs = table_MounthCL.Select("vDate='合计'");//取出月报表产量合计行           

            //熟料产量（本日完成）
            if (row_DayCLs.Count() != 0)
            {
                temp.Rows[0]["Today_Completion"] = row_DayCLs[0]["ClinkerProductionSum"];
            }
            //熟料产量（本月累计）
            if (row_sumMounthCLs.Count() != 0)
            {
                temp.Rows[0]["Monthly_Accumulative"] = row_sumMounthCLs[0]["ClinkerProductionSum"];
            }
            //发电量（本日完成）
            if (row_DayCLs.Count() != 0)
            {
                temp.Rows[1]["Today_Completion"] = row_DayCLs[0]["PowerGenerationSum"];
            }
            //发电量（本月累计）
            if (row_sumMounthCLs.Count() != 0)
            {
                temp.Rows[1]["Monthly_Accumulative"] = row_sumMounthCLs[0]["PowerGenerationSum"];
            }
            //吨熟料发电量（本日完成）
            if (row_DayCLs.Count() != 0)
            {
                decimal AmounttoCoal_day = MyToDecimal(row_DayCLs[0]["PowerGenerationSum"]);//取出本日行合计发电量的合计存到变量AmounttoCoal_day中
                temp.Rows[4]["Today_Completion"] = AmounttoCoal_day / MyToDecimal(row_DayCLs[0]["ClinkerProductionSum"]);
            }
            //吨熟料发电量（本月累计）    
            if (row_sumMounthCLs.Count() != 0)
            {
                decimal AmounttoCoal_MounthLJ = MyToDecimal(row_sumMounthCLs[0]["PowerGenerationSum"]);//取出合计行合计发电量的合计存到变量AmounttoCoal_MounthLJ中
                temp.Rows[4]["Monthly_Accumulative"] = AmounttoCoal_MounthLJ / MyToDecimal(row_sumMounthCLs[0]["ClinkerProductionSum"]);
            }

            //第三步
            //获得熟料生产线产量及消耗统计报表年报
            DataTable table_YearCL = tzHelper.GetReportData("tz_Report", organizeID, year, "table_ClinkerYearlyOutput");
            DataRow[] row_sumYearCLs = table_YearCL.Select("vDate='合计'");//取出年报表产量合计行
            //合计：熟料产量合计、发电量合计、合计用煤合计→本年累计

            //熟料产量（本年累计）
            if (row_sumYearCLs.Count() != 0)
            {
                temp.Rows[0]["Yearly_Accumulative"] = row_sumYearCLs[0]["ClinkerProductionSum"];
                //发电量（本年累计）
                temp.Rows[1]["Yearly_Accumulative"] = row_sumYearCLs[0]["PowerGenerationSum"];
                //吨熟料发电量（本年累计）
                decimal AmounttoCoal_YearLJ = MyToDecimal(row_sumYearCLs[0]["PowerGenerationSum"]);//取出合计行合计发电量的合计存到变量AmounttoCoal_YearLJ中
                temp.Rows[4]["Yearly_Accumulative"] = AmounttoCoal_YearLJ / MyToDecimal(row_sumYearCLs[0]["ClinkerProductionSum"]);
            }
            //第四步
            //获得熟料生产线合计用电量统计月报表
            DataTable table_MounthDL = tzHelper.GetReportData("tz_Report", organizeID, year + "-" + mounth, "table_ClinkerMonthlyElectricity_sum");
            DataRow[] row_sumMounthDLs = table_MounthDL.Select("vDate='合计'");//取出月报表电量合计行
            DataRow[] row_days = table_MounthDL.Select("vDate='" + day + "'");//取出本日用电量行

            //吨熟料电耗（本日完成）
            if (row_days.Count() != 0 && row_DayCLs.Count() != 0 && MyToDecimal(row_DayCLs[0]["ClinkerProductionSum"]) != 0)
            {
                temp.Rows[3]["Today_Completion"] = MyToDecimal(row_days[0]["AmounttoSum"]) / MyToDecimal(row_DayCLs[0]["ClinkerProductionSum"]);
            }
            //吨熟料电耗（本月累计）
            if (row_sumMounthDLs.Count() != 0 && row_sumMounthCLs.Count() != 0 && MyToDecimal(row_sumMounthCLs[0]["ClinkerProductionSum"]) != 0)
            {
                temp.Rows[3]["Monthly_Accumulative"] = MyToDecimal(row_sumMounthDLs[0]["AmounttoSum"]) / MyToDecimal(row_sumMounthCLs[0]["ClinkerProductionSum"]);
            }
            //生料磨电耗（本日完成）
            if (row_days.Count() != 0 && row_DayCLs.Count() != 0 && MyToDecimal(row_DayCLs[0]["ClinkerProductionSum"]) != 0)
            {
                temp.Rows[5]["Today_Completion"] = MyToDecimal(row_days[0]["RawBatchGrindingSum"]) / MyToDecimal(row_DayCLs[0]["ClinkerProductionSum"]);
            }
            //生料磨电耗（本月累计）
            if (row_sumMounthDLs.Count() != 0 && row_sumMounthCLs.Count() != 0 && MyToDecimal(row_sumMounthCLs[0]["ClinkerProductionSum"]) != 0)
            {
                temp.Rows[5]["Monthly_Accumulative"] = MyToDecimal(row_sumMounthDLs[0]["RawBatchGrindingSum"]) / MyToDecimal(row_sumMounthCLs[0]["ClinkerProductionSum"]);
            }
            //煤磨电耗（本日完成）
            if (row_days.Count() != 0 && row_DayCLs.Count() != 0 && MyToDecimal(row_DayCLs[0]["ClinkerProductionSum"]) != 0)
            {
                temp.Rows[6]["Today_Completion"] = MyToDecimal(row_days[0]["CoalMillSystemSum"]) / MyToDecimal(row_DayCLs[0]["ClinkerProductionSum"]);
            }
            //煤磨电耗（本月累计）
            if (row_sumMounthDLs.Count() != 0 && row_sumMounthCLs.Count() != 0 && MyToDecimal(row_sumMounthCLs[0]["ClinkerProductionSum"]) != 0)
            {
                temp.Rows[6]["Monthly_Accumulative"] = MyToDecimal(row_sumMounthDLs[0]["CoalMillSystemSum"]) / MyToDecimal(row_sumMounthCLs[0]["ClinkerProductionSum"]);
            }


            //获得熟料生产线合计用电量统计年报表
            DataTable table_YearDL = tzHelper.GetReportData("tz_Report", organizeID, year, "table_ClinkerYearlyElectricity_sum");
            DataRow[] row_sumYearDLs = table_MounthDL.Select("vDate='合计'");//取出年报表合计用电量行
            //吨熟料电耗（本年累计）
            if (row_sumYearDLs.Count() != 0 && row_sumYearCLs.Count() != 0 && MyToDecimal(row_sumYearCLs[0]["ClinkerProductionSum"]) != 0)
            {
                temp.Rows[3]["Yearly_Accumulative"] = MyToDecimal(row_sumYearDLs[0]["AmounttoSum"]) / MyToDecimal(row_sumYearCLs[0]["ClinkerProductionSum"]);
            }
            //生料磨电耗（本年累计）
            if(row_sumYearDLs.Count()!=0&&row_sumYearCLs.Count()!=0&&MyToDecimal(row_sumYearCLs[0]["ClinkerProductionSum"])!=0)
                temp.Rows[5]["Yearly_Accumulative"] = MyToDecimal(row_sumYearDLs[0]["RawBatchGrindingSum"]) / MyToDecimal(row_sumYearCLs[0]["ClinkerProductionSum"]);
            //煤磨电耗（本年累计）
            if(row_sumYearDLs.Count()!=0&&row_sumYearCLs.Count()!=0&& MyToDecimal(row_sumYearCLs[0]["ClinkerProductionSum"])!=0)
                temp.Rows[6]["Yearly_Accumulative"] = MyToDecimal(row_sumYearDLs[0]["CoalMillSystemSum"]) / MyToDecimal(row_sumYearCLs[0]["ClinkerProductionSum"]);

            foreach (DataRow dr in temp.Rows)
            {
                if (dr["Monthly_Target"] is DBNull) { dr["Monthly_Target"] = 0; }
                if (dr["Monthly_Accumulative"] is DBNull) { dr["Monthly_Accumulative"] = 0; }
                if (dr["Yearly_Target"] is DBNull) { dr["Yearly_Target"] = 0; }
                if (dr["Yearly_Accumulative"] is DBNull) { dr["Yearly_Accumulative"] = 0; }
                dr["Monthly_Gap"] = MyToDecimal(dr["Monthly_Target"]) - MyToDecimal(dr["Monthly_Accumulative"]);
                dr["Yearly_Gap"] = MyToDecimal(dr["Yearly_Target"]) - MyToDecimal(dr["Yearly_Accumulative"]);
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
