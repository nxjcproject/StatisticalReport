using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Yearly
{
    public class CementYearlyElectricityConsumption
    {
        private static TZHelper _tzHelper;

        static CementYearlyElectricityConsumption()
        {
            string connString = ConnectionStringFactory.NXJCConnectionString;
            _tzHelper = new TZHelper(connString);
        }

        public static DataTable Query(string organizeID, string year, string tableName)
        {
            DataTable temp1;
            temp1 = _tzHelper.CreateTableStructure(tableName);//目标数据表
            int MonthEnd;
            if (DateTime.Now.Year.ToString() != year)
            { MonthEnd = 12; }
            else
            { MonthEnd = Convert.ToInt32(DateTime.Now.Month); }

            //形成产量数据----------------start----------------------
            DataTable temp_cl;
            temp_cl = _tzHelper.CreateTableStructure("report_CementMillMonthlyOutput");
            for (int i = 1; i <= MonthEnd; i++) //形成每月产量数据，包括累计
            {
                DataTable temp2;
                string date = year + "-" + ReportHelper.MyToString(i, 2, 0);
                temp2 = _tzHelper.GetReportData("tz_Report", organizeID, date, "report_CementMillMonthlyOutput");
                if (temp2.Rows.Count == 0)
                { continue; }
                foreach (DataRow dr in temp2.Rows)
                {
                    DataRow row = temp1.NewRow();
                    row["月份"] = ReportHelper.MyToString(i, 2, 0);
                    row["水泥品种"] = dr["CementTypes"];
                    row["本月产量"] = dr["CementProductionSum"];//要改
                    temp1.Rows.Add(row);
                }
                temp_cl.Merge(temp2);//产量并入累计暂存
                foreach (DataRow dr in temp_cl.Rows)//累计产量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["月份"] = ReportHelper.MyToString(i, 2, 0);
                    row["水泥品种"] = dr["CementTypes"];//要改
                    row["累计产量"] = dr["CementProductionSum"];//要改
                    temp1.Rows.Add(row);
                }
            }
            //-----------------------END------------------------------

            //形成电量数据----------------start-----------------------
            DataTable temp_dl;
            temp_dl = _tzHelper.CreateTableStructure("report_CementMillMonthlyElectricity_sum");
            for (int i = 1; i <= MonthEnd; i++)//形成每月电量数据，包括累计
            {
                DataTable temp2;
                string date = year + "-" + ReportHelper.MyToString(i, 2, 0);
                temp2 = _tzHelper.GetReportData("tz_Report", organizeID, date, "report_CementMillMonthlyElectricity_sum");
                if (temp2.Rows.Count == 0)
                { continue; }
                foreach (DataRow dr in temp2.Rows)//本月电量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["月份"] = ReportHelper.MyToString(i, 2, 0);
                    row["水泥品种"] = dr["CementTypes"];//要改
                    row["本月用电量"] = dr["AmounttoSum"];//要改
                    temp1.Rows.Add(row);
                }
                temp_dl.Merge(temp2);    //电量并入累计暂存
                foreach (DataRow dr in temp_dl.Rows)//累计电量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["月份"] = ReportHelper.MyToString(i, 2, 0);
                    row["水泥品种"] = dr["CementTypes"];//要改
                    row["累计用电量"] = dr["AmounttoSum"];//要改
                    temp1.Rows.Add(row);
                }
            }
            //-----------------------END------------------------------
            DataTable temp = ReportHelper.GroupByTotal(temp1, "月份,水泥品种", "本月产量,累计产量,本月用电量,累计用电量");    // temp每月明细
            DataTable table4 = ReportHelper.MyTotalOn(temp, "月份", "本月产量,累计产量,本月用电量,累计用电量");//表4每月合计
            foreach (DataRow dr in table4.Rows)//将表4水泥品种字段中的数据全部用合计替换
            {
                dr["水泥品种"] = "总总合计";
            }
            temp.Merge(table4);

            DataTable result = ReportHelper.SortTable(temp, new string[] { "月份", "水泥品种" });//按照月份和水泥品种排序
            foreach (DataRow dr in result.Rows)
            {
                string PZ = dr["水泥品种"].ToString().Trim();

                if (dr["水泥品种"].ToString() != "总总合计")
                {
                    double ZHXS = _tzHelper.GetConvertCoefficient(PZ);
                    dr["折合系数"] = ZHXS;
                }
                else
                {
                    dr["水泥品种"] = "合计";
                }
                if (dr["本月产量"] is DBNull)
                { dr["本月产量"] = 0; }
                if (dr["本月用电量"] is DBNull)
                {
                    dr["本月用电量"] = 0;
                    dr["本月电耗"] = 0;
                    dr["本月折算电耗"] = 0;
                }
                else
                {
                    dr["本月电耗"] = Convert.ToDouble(dr["本月用电量"]) / Convert.ToDouble(dr["本月产量"]);
                    if (dr["水泥品种"].ToString() != "合计")
                    { dr["本月折算电耗"] = Convert.ToDouble(dr["本月电耗"]) / Convert.ToDouble(dr["折合系数"]); }
                }

                if (dr["累计产量"] is DBNull)
                { dr["累计产量"] = 0; }
                if (dr["累计用电量"] is DBNull)
                {
                    dr["累计用电量"] = 0;
                    dr["累计电耗"] = 0;
                    dr["累计折算电耗"] = 0;
                }
                else
                {
                    dr["累计电耗"] = Convert.ToDouble(dr["累计用电量"]) / Convert.ToDouble(dr["累计产量"]);
                    if (dr["水泥品种"].ToString() != "合计")
                    { dr["累计折算电耗"] = Convert.ToDouble(dr["累计电耗"]) / Convert.ToDouble(dr["折合系数"]); }
                }
            }
            return result;
        }
    }
}
