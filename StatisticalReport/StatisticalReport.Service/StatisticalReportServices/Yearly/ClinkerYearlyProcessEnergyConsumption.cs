using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Yearly
{
    public class ClinkerYearlyProcessEnergyConsumption
    {
        private static string connectionString;
        private static TZHelper tzHelper;
        static ClinkerYearlyProcessEnergyConsumption()
        {
            connectionString = ConnectionStringFactory.NXJCConnectionString;
            tzHelper = new TZHelper(connectionString);
        }
        /// <summary>
        /// 孰料能耗年统计分析
        /// </summary>
        /// <param name="_organizeID"></param>
        /// <param name="_year"></param>
        /// <returns></returns>
        public static DataTable TableQuery(string _organizeID, string _year)
        {           
            DataTable temp1;
            temp1 = tzHelper.CreateTableStructure("report_ClinkerYearlyProcessEnergyConsumption");//目标数据表
            int MonthEnd;
            if (DateTime.Now.Year.ToString() != _year)
            { MonthEnd = 12; }
            else
            { MonthEnd = Convert.ToInt32(DateTime.Now.Month); }

            //形成产量数据----------------start----------------------
            DataTable temp_cl;
            temp_cl = tzHelper.CreateTableStructure("report_ClinkerMonthlyOutput");
            for (int i = 1; i <= MonthEnd; i++) //形成每月产量数据，包括累计
            {
                DataTable temp2;
                string date = _year + "-" + ReportHelper.MyToString(i, 2, 0);
                temp2 = tzHelper.GetReportData("tz_Report", _organizeID, date, "report_ClinkerMonthlyOutput","vDate='合计'");
                
                if (temp2.Rows.Count == 0)
                { continue; }
                foreach (DataRow dr in temp2.Rows)
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Consumption_CoalDust_Monthly"] = dr["AmounttoCoalDustConsumptionSum"];//用煤量
                    row["Output_RawBatch_Monthly"] = dr["RawBatchProductionSum"];
                    row["Output_Clinker_Monthly"] = dr["ClinkerProductionSum"];//要改
                    row["Output_CoalDust_Monthly"] = dr["CoalDustProductionSum"];
                    row["Output_Cogeneration_Monthly"] = dr["PowerGenerationSum"];
                    temp1.Rows.Add(row);
                }
                temp_cl.Merge(temp2);//产量并入累计暂存
                foreach (DataRow dr in temp_cl.Rows)//累计产量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Consumption_CoalDust_Accumulative"] = dr["AmounttoCoalDustConsumptionSum"];//用煤量
                    row["Output_RawBatch_Accumulative"] = dr["RawBatchProductionSum"];//要改
                    row["Output_Clinker_Accumulative"] = dr["ClinkerProductionSum"];//要改
                    row["Output_CoalDust_Accumulative"] = dr["CoalDustProductionSum"];//要改
                    row["Output_Cogeneration_Accumulative"] = dr["PowerGenerationSum"];//要改
                    temp1.Rows.Add(row);
                }
            }
            //-----------------------END------------------------------

            //形成电量数据----------------start-----------------------
            DataTable temp_dl;
            temp_dl = tzHelper.CreateTableStructure("report_ClinkerMonthlyElectricity_sum");
            for (int i = 1; i <= MonthEnd; i++)//形成每月电量数据，包括累计
            {
                DataTable temp2;
                string date = _year + "-" + ReportHelper.MyToString(i, 2, 0);
                temp2 = tzHelper.GetReportData("tz_Report", _organizeID, date, "report_ClinkerMonthlyElectricity_sum", "vDate='合计'");
                if (temp2.Rows.Count == 0)
                { continue; }
                foreach (DataRow dr in temp2.Rows)//本月电量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Electricity_RawBatch_Monthly"] = dr["AmounttoRawBatchPreparationSum"];//要改
                    row["Electricity_RawBatchMil_Monthly"] = dr["RawBatchGrindingSum"];//要改
                    row["Electricity_Clinker_Monthly"] = dr["AmounttoFiringSystemSum"];//要改
                    row["Electricity_CoalDust_Monthly"] = dr["CoalMillSystemSum"];//要改
                    temp1.Rows.Add(row);
                }
                temp_dl.Merge(temp2);    //电量并入累计暂存
                foreach (DataRow dr in temp_dl.Rows)//累计电量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Electricity_RawBatch_Accumulative"] = dr["AmounttoRawBatchPreparationSum"];//要改
                    row["Electricity_RawBatchMil_Accumulative"] = dr["RawBatchGrindingSum"];//要改
                    row["Electricity_Clinker_Accumulative"] = dr["AmounttoFiringSystemSum"];//要改
                    row["Electricity_CoalDust_Accumulative"] = dr["CoalMillSystemSum"];//要改
                    temp1.Rows.Add(row);
                }
            }
            //-----------------------END------------------------------
            string column = "Electricity_RawBatch_Monthly,Electricity_RawBatch_Accumulative,Electricity_RawBatchMil_Monthly,Electricity_RawBatchMil_Accumulative," 
                + "Electricity_Clinker_Monthly,Electricity_Clinker_Accumulative,Electricity_CoalDust_Monthly,Electricity_CoalDust_Accumulative,Consumption_CoalDust_Monthly," 
                + "Consumption_CoalDust_Accumulative,Output_RawBatch_Monthly,Output_RawBatch_Accumulative,Output_Clinker_Monthly,Output_Clinker_Accumulative,Output_CoalDust_Monthly," 
                + "Output_CoalDust_Accumulative,Output_Cogeneration_Monthly,Output_Cogeneration_Accumulative";
            DataTable temp = ReportHelper.MyTotalOn(temp1, "vDate", column);    // temp每月明细
            foreach (DataRow dr in temp.Rows)
            {
                //电耗—生料制备-本月
                if(dr["Electricity_RawBatch_Monthly"] is DBNull)
                {
                    dr["Electricity_RawBatch_Monthly"] = 0;
                    dr["ElectricityConsumption_RawBatch_Monthly"]=0;
                }
                else
                {
                    if (dr["Output_RawBatch_Monthly"] is DBNull)
                    { dr["Output_RawBatch_Monthly"] = 0; }
                    dr["ElectricityConsumption_RawBatch_Monthly"] = Convert.ToDouble(dr["Electricity_RawBatch_Monthly"]) / Convert.ToDouble(dr["Output_RawBatch_Monthly"]);
                }
                //电耗-生料制备-累计
                if (dr["Electricity_RawBatch_Accumulative"] is DBNull)
                {
                    dr["Electricity_RawBatch_Accumulative"] = 0;
                    dr["ElectricityConsumption_RawBatch_Accumulative"] = 0; 
                }
                else
                {
                    if (dr["Output_RawBatch_Accumulative"] is DBNull)
                    { dr["Output_RawBatch_Accumulative"] = 0; }
                    dr["ElectricityConsumption_RawBatch_Accumulative"] = Convert.ToDouble(dr["Electricity_RawBatch_Accumulative"]) / Convert.ToDouble(dr["Output_RawBatch_Accumulative"]);
                }
                //电耗-生料磨-本月
                if (dr["Electricity_RawBatchMil_Monthly"] is DBNull)
                {
                    dr["Electricity_RawBatchMil_Monthly"] = 0;
                    dr["ElectricityConsumption_RawBatchMill_Monthly"] = 0;
                }
                else
                {
                    if (dr["Output_RawBatch_Monthly"] is DBNull)
                    { dr["Output_RawBatch_Monthly"] = 0; }
                    dr["ElectricityConsumption_RawBatchMill_Monthly"] = Convert.ToDouble(dr["Electricity_RawBatchMil_Monthly"]) / Convert.ToDouble(dr["Output_RawBatch_Monthly"]);
                }
                //电耗-生料磨-累计
                if (dr["Electricity_RawBatchMil_Accumulative"] is DBNull)
                {
                    dr["Electricity_RawBatchMil_Accumulative"] = 0;
                    dr["ElectricityConsumption_RawBatchMill_Accumulative"] = 0; 
                }
                else
                {
                    if (dr["Output_RawBatch_Accumulative"] is DBNull)
                    { { dr["Output_RawBatch_Accumulative"] = 0; } }
                    dr["ElectricityConsumption_RawBatchMill_Accumulative"] = Convert.ToDouble(dr["Electricity_RawBatchMil_Accumulative"]) / Convert.ToDouble(dr["Output_RawBatch_Accumulative"]);
                }
                //电耗-熟料烧成-本月
                if (dr["Electricity_Clinker_Monthly"] is DBNull)
                {
                    dr["Electricity_Clinker_Monthly"] = 0;
                    dr["ElectricityConsumption_Clinker_Monthly"] = 0; 
                }
                else
                {
                    if (dr["Output_Clinker_Monthly"] is DBNull)
                    { { dr["Output_Clinker_Monthly"] = 0; } }
                    dr["ElectricityConsumption_Clinker_Monthly"] = Convert.ToDouble(dr["Electricity_Clinker_Monthly"]) / Convert.ToDouble(dr["Output_Clinker_Monthly"]);
                }
                //电耗-熟料烧成-累计
                if (dr["Electricity_Clinker_Accumulative"] is DBNull)
                {
                    dr["Electricity_Clinker_Accumulative"] = 0;
                    dr["ElectricityConsumption_Clinker_Accumulative"] = 0;
                }
                else
                {
                    if (dr["Output_Clinker_Accumulative"] is DBNull)
                    { { dr["Output_Clinker_Accumulative"] = 0; } }
                    dr["ElectricityConsumption_Clinker_Accumulative"] = Convert.ToDouble(dr["Electricity_Clinker_Accumulative"]) / Convert.ToDouble(dr["Output_Clinker_Accumulative"]);
                }
                //电耗-煤磨-本月
                if (dr["Electricity_CoalDust_Monthly"] is DBNull)
                {
                    dr["Electricity_CoalDust_Monthly"] = 0;
                    dr["ElectricityConsumption_CoalDust_Monthly"] = 0; 
                }
                else
                {
                    if (dr["Output_CoalDust_Monthly"] is DBNull)
                    { { dr["Output_CoalDust_Monthly"] = 0; } }
                    dr["ElectricityConsumption_CoalDust_Monthly"] = Convert.ToDouble(dr["Electricity_CoalDust_Monthly"]) / Convert.ToDouble(dr["Output_CoalDust_Monthly"]);
                }
                //电耗-煤磨-累计
                if (dr["Electricity_CoalDust_Accumulative"] is DBNull)
                {
                    dr["Electricity_CoalDust_Accumulative"] = 0;
                    dr["ElectricityConsumption_CoalDust_Accumulative"] = 0; 
                }
                else
                {
                    if (dr["Output_CoalDust_Accumulative"] is DBNull)
                    { { dr["Output_CoalDust_Accumulative"] = 0; } }
                    dr["ElectricityConsumption_CoalDust_Accumulative"] = Convert.ToDouble(dr["Electricity_CoalDust_Accumulative"]) / Convert.ToDouble(dr["Output_CoalDust_Accumulative"]);
                }
                //吨熟料综合电耗-本月
                if (0 == Convert.ToDouble(dr["Electricity_RawBatch_Monthly"]) && 0 == Convert.ToDouble(dr["Electricity_Clinker_Monthly"]))
                {
                    dr["ComprehensiveElectricityConsumption_Monthly"] = 0;
                }
                else
                {
                    dr["ComprehensiveElectricityConsumption_Monthly"]=(Convert.ToDouble(dr["Electricity_RawBatch_Monthly"])+Convert.ToDouble(dr["Electricity_Clinker_Monthly"]))/Convert.ToDouble(dr["Output_Clinker_Monthly"]);
                }
                //吨熟料综合电耗-累计
                if (0 == Convert.ToDouble(dr["Electricity_RawBatch_Accumulative"]) && 0 == Convert.ToDouble(dr["Electricity_Clinker_Accumulative"]))
                {
                    dr["ComprehensiveElectricityConsumption_Accumulative"] = 0;
                }
                else
                {
                    dr["ComprehensiveElectricityConsumption_Accumulative"] = (Convert.ToDouble(dr["Electricity_RawBatch_Accumulative"]) + Convert.ToDouble(dr["Electricity_Clinker_Accumulative"])) / Convert.ToDouble(dr["Output_Clinker_Accumulative"]);
                }
                //吨熟料实物煤耗-本月
                if (dr["Consumption_CoalDust_Monthly"] is DBNull)
                {
                    dr["Consumption_CoalDust_Monthly"] = 0;
                    dr["ComprehensiveCoalConsumption_Monthly"] = 0;
                }
                else
                {
                    dr["ComprehensiveCoalConsumption_Monthly"] = Convert.ToDouble(dr["Consumption_CoalDust_Monthly"]) / Convert.ToDouble(dr["Output_Clinker_Monthly"]);
                }
                //吨熟料实物煤耗-累计
                if ( dr["Consumption_CoalDust_Accumulative"] is DBNull)
                {
                    dr["Consumption_CoalDust_Accumulative"] = 0;
                    dr["ComprehensiveCoalConsumption_Accumulative"] = 0;
                }
                else
                {
                    dr["ComprehensiveCoalConsumption_Accumulative"] = Convert.ToDouble(dr["Consumption_CoalDust_Accumulative"]) / Convert.ToDouble(dr["Output_Clinker_Accumulative"]);
                }
            }
            return temp;
        }
    }
}
