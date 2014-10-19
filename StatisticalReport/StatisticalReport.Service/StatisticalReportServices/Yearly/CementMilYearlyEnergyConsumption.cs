using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Yearly
{
    public class CementMilYearlyEnergyConsumption
    {
        private static string connectionString;
        private static TZHelper tzHelper;
        static CementMilYearlyEnergyConsumption()
        {
            connectionString = ConnectionStringFactory.NXJCConnectionString;
            tzHelper = new TZHelper(connectionString);
        }
        /// <summary>
        /// 水泥粉磨能耗年统计分析
        /// </summary>
        /// <param name="_organizeID"></param>
        /// <param name="_year"></param>
        /// <returns></returns>
        public static DataTable TableQuery(string _organizeID, string _year)
        {
            DataTable temp1;
            temp1 = tzHelper.CreateTableStructure("report_CementMilYearlyEnergyConsumption");//目标数据表
            int MonthEnd;
            if (DateTime.Now.Year.ToString() != _year)
            { MonthEnd = 12; }
            else
            { MonthEnd = Convert.ToInt32(DateTime.Now.Month); }

            //形成产量数据----------------start----------------------
            DataTable temp_cl;
            temp_cl = tzHelper.CreateTableStructure("report_CementMillMonthlyOutput");
            for (int i = 1; i <= MonthEnd; i++) //形成每月产量数据，包括累计
            {
                DataTable temp2;
                string date = _year + "-" + ReportHelper.MyToString(i, 2, 0);
                temp2 = tzHelper.GetReportData("tz_Report", _organizeID, date, "report_CementMillMonthlyOutput");
                foreach (DataRow dr in temp2.Rows)
                {
                    dr["vDate"] = "合计";
                }
                temp2 = ReportHelper.MyTotalOn(temp2, "vDate", "CementProductionSum");
                //if (temp2.Rows.Count == 0)
                //{ continue; }
                foreach (DataRow dr in temp2.Rows)
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Output_Cement_Monthly"] = dr["CementProductionSum"];//水泥制备
                    temp1.Rows.Add(row);
                }
                temp_cl.Merge(temp2);//产量并入累计暂存
                foreach (DataRow dr in temp_cl.Rows)//累计产量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Output_Cement_Accumulative"] = dr["CementProductionSum"];//水泥制备
                    temp1.Rows.Add(row);
                }
            }
            //-----------------------END------------------------------

            //形成电量数据----------------start-----------------------
            DataTable temp_dl;
            temp_dl = tzHelper.CreateTableStructure("report_CementMillMonthlyElectricity_sum");
            for (int i = 1; i <= MonthEnd; i++)//形成每月电量数据，包括累计
            {
                DataTable temp2;
                string date = _year + "-" + ReportHelper.MyToString(i, 2, 0);
                temp2 = tzHelper.GetReportData("tz_Report", _organizeID, date, "report_CementMillMonthlyElectricity_sum");
                foreach (DataRow dr in temp2.Rows)
                {
                    dr["vDate"] = "合计";
                }
                temp2 = ReportHelper.MyTotalOn(temp2, "vDate", "AmounttoCementPreparationSum,CementGrindingSum,AdmixturePreparationSum,AmounttoCementPackagingSum");
                //if (temp2.Rows.Count == 0)
                //{ continue; }
                foreach (DataRow dr in temp2.Rows)//本月电量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Electricity_Cement_Monthly"] = dr["AmounttoCementPreparationSum"];
                    row["Electricity_CementGrinding_Monthly"] = dr["CementGrindingSum"];
                    row["Electricity_AdmixturePreparation_Monthly"] = dr["AdmixturePreparationSum"];
                    row["Electricity_BagsBulk_Monthly"] = dr["AmounttoCementPackagingSum"];
                    temp1.Rows.Add(row);
                }
                temp_dl.Merge(temp2);    //电量并入累计暂存
                foreach (DataRow dr in temp_dl.Rows)//累计电量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Electricity_Cement_Accumulative"] = dr["AmounttoCementPreparationSum"];
                    row["Electricity_CementGrinding_Accumulative"] = dr["CementGrindingSum"];
                    row["Electricity_AdmixturePreparation_Accumulative"] = dr["AdmixturePreparationSum"];
                    row["Electricity_BagsBulk_Accumulative"] = dr["AmounttoCementPackagingSum"];
                    temp1.Rows.Add(row);
                }
            }
            //-----------------------END------------------------------
            string sumString = "Electricity_Cement_Monthly,Electricity_Cement_Accumulative,Electricity_CementGrinding_Monthly,"
                + "Electricity_CementGrinding_Accumulative,Electricity_AdmixturePreparation_Monthly,"
                + "Electricity_AdmixturePreparation_Accumulative,Electricity_BagsBulk_Monthly,Electricity_BagsBulk_Accumulative,"
                + "Output_Cement_Monthly,Output_Cement_Accumulative";
            DataTable temp = ReportHelper.MyTotalOn(temp1, "vDate", sumString);    // temp每月明细
            foreach (DataRow dr in temp.Rows)
            {
                //电耗—水泥制备-本月
                if (dr["Electricity_Cement_Monthly"].ToString() == "0")
                {
                    dr["Electricity_Cement_Monthly"] = 0;
                    dr["ElectricityConsumption_Cement_Monthly"] = 0;
                }
                else
                {
                    if (dr["Output_Cement_Monthly"].ToString() == "0")
                    {
                        dr["Output_Cement_Monthly"] = 0;
                        dr["ElectricityConsumption_Cement_Monthly"] = 0;
                    }
                    else
                    //dr["ElectricityConsumption_Cement_Monthly"] = Convert.ToDouble(dr["Electricity_Cement_Monthly"]) / Convert.ToDouble(dr["Output_Cement_Monthly"]);
                    { dr["ElectricityConsumption_Cement_Monthly"] = Convert.ToDecimal(dr["Electricity_Cement_Monthly"]) / Convert.ToDecimal(dr["Output_Cement_Monthly"]); }

                }
                //电耗-水泥制备-累计
                if (dr["Electricity_Cement_Accumulative"].ToString() == "0")
                {
                    dr["Electricity_Cement_Accumulative"] = 0;
                    dr["ElectricityConsumption_Cement_Accumulative"] = 0;
                }
                else
                {
                    if (dr["Output_Cement_Accumulative"].ToString() == "0")
                    { dr["Output_Cement_Accumulative"] = 0; }
                    dr["ElectricityConsumption_Cement_Accumulative"] = Convert.ToDouble(dr["Electricity_Cement_Accumulative"]) / Convert.ToDouble(dr["Output_Cement_Accumulative"]);
                }
                //电耗-水泥磨-本月  //用 水泥磨/水泥制备 不知道对不对 
                if (dr["Electricity_CementGrinding_Monthly"].ToString() == "0")
                {
                    dr["Electricity_CementGrinding_Monthly"] = 0;
                    dr["ElectricityConsumption_CementGrinding_Monthly"] = 0;
                }
                else
                {
                    if (dr["Output_Cement_Monthly"].ToString() == "0")
                    { dr["Output_Cement_Monthly"] = 0; }
                    dr["ElectricityConsumption_CementGrinding_Monthly"] = Convert.ToDouble(dr["Electricity_CementGrinding_Monthly"]) / Convert.ToDouble(dr["Output_Cement_Monthly"]);
                }
                //电耗-水泥磨-累计
                if (dr["Electricity_CementGrinding_Accumulative"].ToString() == "0")
                {
                    dr["Electricity_CementGrinding_Accumulative"] = 0;
                    dr["ElectricityConsumption_CementGrinding_Accumulative"] = 0;
                }
                else
                {
                    if (dr["Output_Cement_Accumulative"].ToString() == "0")
                    { { dr["Output_Cement_Accumulative"] = 0; } }
                    dr["ElectricityConsumption_CementGrinding_Accumulative"] = Convert.ToDouble(dr["Electricity_CementGrinding_Accumulative"]) / Convert.ToDouble(dr["Output_Cement_Accumulative"]);
                }
                //电耗-袋装与散装-本月
                //if (dr["Electricity_Clinker_Monthly"] is DBNull)
                //{
                //    dr["Electricity_Clinker_Monthly"] = 0;
                //    dr["ElectricityConsumption_Clinker_Monthly"] = 0; 
                //}
                //else
                //{
                //    if (dr["Output_Clinker_Monthly"] is DBNull)
                //    { { dr["Output_Clinker_Monthly"] = 0; } }
                //    dr["ElectricityConsumption_Clinker_Monthly"] = Convert.ToDouble(dr["Electricity_Clinker_Monthly"]) / Convert.ToDouble(dr["Output_Clinker_Monthly"]);
                //}
                ////电耗-袋装与散装-累计
                //if (dr["Electricity_Clinker_Accumulative"] is DBNull)
                //{
                //    dr["Electricity_Clinker_Accumulative"] = 0;
                //    dr["ElectricityConsumption_Clinker_Accumulative"] = 0;
                //}
                //else
                //{
                //    if (dr["Output_Clinker_Accumulative"] is DBNull)
                //    { { dr["Output_Clinker_Accumulative"] = 0; } }
                //    dr["ElectricityConsumption_Clinker_Accumulative"] = Convert.ToDouble(dr["Electricity_Clinker_Accumulative"]) / Convert.ToDouble(dr["Output_Clinker_Accumulative"]);
                //}

                //吨水泥综合电耗-本月
                if (0 == Convert.ToDouble(dr["Electricity_Cement_Monthly"])
                    && 0 == Convert.ToDouble(dr["Electricity_AdmixturePreparation_Monthly"])
                    && 0 == Convert.ToDouble(dr["Electricity_BagsBulk_Monthly"]))
                {
                    dr["ComprehensiveElectricityConsumption_Monthly"] = 0;
                }
                else
                {
                    dr["ComprehensiveElectricityConsumption_Monthly"]
                        = (Convert.ToDouble(dr["Electricity_Cement_Monthly"])
                        + Convert.ToDouble(dr["Electricity_AdmixturePreparation_Monthly"])
                        + Convert.ToDouble(dr["Electricity_BagsBulk_Monthly"]))
                        / Convert.ToDouble(dr["Output_Cement_Monthly"]);
                }
                //吨水泥综合电耗-累计
                if (0 == Convert.ToDouble(dr["Electricity_Cement_Accumulative"])
                    && 0 == Convert.ToDouble(dr["Electricity_AdmixturePreparation_Accumulative"])
                    && 0 == Convert.ToDouble(dr["Electricity_BagsBulk_Accumulative"]))
                {
                    dr["ComprehensiveElectricityConsumption_Accumulative"] = 0;
                }
                else
                {
                    dr["ComprehensiveElectricityConsumption_Accumulative"]
                        = (Convert.ToDouble(dr["Electricity_Cement_Accumulative"])
                        + Convert.ToDouble(dr["Electricity_AdmixturePreparation_Accumulative"])
                        + Convert.ToDouble(dr["Electricity_BagsBulk_Accumulative"]))
                        / Convert.ToDouble(dr["Output_Cement_Accumulative"]);
                }

            }
            return temp;
        }
    }
}
