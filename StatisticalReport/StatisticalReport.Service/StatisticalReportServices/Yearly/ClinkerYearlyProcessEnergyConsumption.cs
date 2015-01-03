using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Yearly
{
    public class ClinkerYearlyProcessEnergyConsumption                         //cdy
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
            temp_cl = tzHelper.CreateTableStructure("table_ClinkerMonthlyOutput");
            for (int i = 1; i <= MonthEnd; i++) //形成每月产量数据，包括累计
            {
                DataTable temp2;
                string date = _year + "-" + ReportHelper.MyToString(i, 2, 0);
                temp2 = tzHelper.GetReportData("tz_Report", _organizeID, date, "table_ClinkerMonthlyOutput","vDate='合计'");

                if (temp2.Rows.Count == 0)
                { continue; }
                foreach (DataRow dr in temp2.Rows)
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Consumption_CoalDust_Monthly"] =Convert.ToInt64(dr["AmounttoCoalDustConsumptionSum"]);//用煤量
                    row["Output_RawBatch_Monthly"] = Convert.ToInt64(dr["RawBatchProductionSum"]);
                    row["Output_Clinker_Monthly"] = Convert.ToInt64(dr["ClinkerProductionSum"]);//要改
                    row["Output_CoalDust_Monthly"] = Convert.ToInt64(dr["CoalDustProductionSum"]);
                    row["Output_Cogeneration_Monthly"] = Convert.ToInt64(dr["PowerGenerationSum"]);
                    temp1.Rows.Add(row);
                }
                temp_cl.Merge(temp2);//产量并入累计暂存
                foreach (DataRow dr in temp_cl.Rows)//累计产量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Consumption_CoalDust_Accumulative"] = Convert.ToInt64(dr["AmounttoCoalDustConsumptionSum"]);//用煤量
                    row["Output_RawBatch_Accumulative"] = Convert.ToInt64(dr["RawBatchProductionSum"]);//要改
                    row["Output_Clinker_Accumulative"] = Convert.ToInt64(dr["ClinkerProductionSum"]);//要改
                    row["Output_CoalDust_Accumulative"] = Convert.ToInt64(dr["CoalDustProductionSum"]);//要改
                    row["Output_Cogeneration_Accumulative"] = Convert.ToInt64(dr["PowerGenerationSum"]);//要改
                    temp1.Rows.Add(row);
                }
            }
            //-----------------------END------------------------------

            //形成电量数据----------------start-----------------------
            DataTable temp_dl;
            temp_dl = tzHelper.CreateTableStructure("table_ClinkerMonthlyElectricity_sum");
            for (int i = 1; i <= MonthEnd; i++)//形成每月电量数据，包括累计
            {
                DataTable temp2;
                string date = _year + "-" + ReportHelper.MyToString(i, 2, 0);
                temp2 = tzHelper.GetReportData("tz_Report", _organizeID, date, "table_ClinkerMonthlyElectricity_sum", "vDate='合计'");
                if (temp2.Rows.Count == 0)
                { continue; }
                foreach (DataRow dr in temp2.Rows)//本月电量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Electricity_RawBatch_Monthly"] = Convert.ToInt64(dr["AmounttoRawBatchPreparationSum"]);//要改
                    row["Electricity_RawBatchMil_Monthly"] = Convert.ToInt64(dr["RawBatchGrindingSum"]);//要改
                    row["Electricity_Clinker_Monthly"] = Convert.ToInt64(dr["AmounttoFiringSystemSum"]);//要改
                    row["Electricity_CoalDust_Monthly"] = Convert.ToInt64(dr["CoalMillSystemSum"]);//要改
                    temp1.Rows.Add(row);
                }
                temp_dl.Merge(temp2);    //电量并入累计暂存
                foreach (DataRow dr in temp_dl.Rows)//累计电量并入目标表
                {
                    DataRow row = temp1.NewRow();
                    row["vDate"] = ReportHelper.MyToString(i, 2, 0);
                    row["Electricity_RawBatch_Accumulative"] = Convert.ToInt64(dr["AmounttoRawBatchPreparationSum"]);//要改
                    row["Electricity_RawBatchMil_Accumulative"] = Convert.ToInt64(dr["RawBatchGrindingSum"]);//要改
                    row["Electricity_Clinker_Accumulative"] = Convert.ToInt64(dr["AmounttoFiringSystemSum"]);//要改
                    row["Electricity_CoalDust_Accumulative"] = Convert.ToInt64(dr["CoalMillSystemSum"]);//要改
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
                if(MyToDecimal(dr["Output_RawBatch_Monthly"])!=0)              
                {                    
                    dr["ElectricityConsumption_RawBatch_Monthly"] = MyToDecimal(dr["Electricity_RawBatch_Monthly"]) / MyToDecimal(dr["Output_RawBatch_Monthly"]);
                }
                //电耗-生料制备-累计
                if (MyToDecimal(dr["Output_RawBatch_Accumulative"])!=0)               
                {                 
                    dr["ElectricityConsumption_RawBatch_Accumulative"] = MyToDecimal(dr["Electricity_RawBatch_Accumulative"]) / MyToDecimal(dr["Output_RawBatch_Accumulative"]);
                }
                //电耗-生料磨-本月
                if (MyToDecimal(dr["Output_RawBatch_Monthly"])!=0)          
                {
                    dr["ElectricityConsumption_RawBatchMill_Monthly"] = MyToDecimal(dr["Electricity_RawBatchMil_Monthly"]) / MyToDecimal(dr["Output_RawBatch_Monthly"]);
                }
                //电耗-生料磨-累计
                if (MyToDecimal(dr["Output_RawBatch_Accumulative"])!=0)
                {
                    dr["ElectricityConsumption_RawBatchMill_Accumulative"] = MyToDecimal(dr["Electricity_RawBatchMil_Accumulative"]) / MyToDecimal(dr["Output_RawBatch_Accumulative"]);
                }
                //电耗-熟料烧成-本月
                if (MyToDecimal(dr["Output_Clinker_Monthly"])!=0)
                {
                    dr["ElectricityConsumption_Clinker_Monthly"] = MyToDecimal(dr["Electricity_Clinker_Monthly"]) / MyToDecimal(dr["Output_Clinker_Monthly"]);
                }
                //电耗-熟料烧成-累计
                if (MyToDecimal(dr["Output_Clinker_Accumulative"])!=0)
                {
                    dr["ElectricityConsumption_Clinker_Accumulative"] = MyToDecimal(dr["Electricity_Clinker_Accumulative"]) / MyToDecimal(dr["Output_Clinker_Accumulative"]);
                }
                //电耗-煤磨-本月
                if (MyToDecimal(dr["Output_CoalDust_Monthly"])!=0)
                {
                    dr["ElectricityConsumption_CoalDust_Monthly"] = MyToDecimal(dr["Electricity_CoalDust_Monthly"]) / MyToDecimal(dr["Output_CoalDust_Monthly"]);
                }
                //电耗-煤磨-累计
                if (MyToDecimal(dr["Output_CoalDust_Accumulative"])!=0)
                {
                    dr["ElectricityConsumption_CoalDust_Accumulative"] = MyToDecimal(dr["Electricity_CoalDust_Accumulative"]) / MyToDecimal(dr["Output_CoalDust_Accumulative"]);
                }
                //吨熟料综合电耗-本月
                if (MyToDecimal(dr["Output_Clinker_Monthly"])!=0)
                {
                    dr["ComprehensiveElectricityConsumption_Monthly"]=(MyToDecimal(dr["Electricity_RawBatch_Monthly"])+MyToDecimal(dr["Electricity_Clinker_Monthly"]))/MyToDecimal(dr["Output_Clinker_Monthly"]);
                }
                //吨熟料综合电耗-累计
                if (MyToDecimal(dr["Output_Clinker_Accumulative"])!=0)
                {
                    dr["ComprehensiveElectricityConsumption_Accumulative"] = (MyToDecimal(dr["Electricity_RawBatch_Accumulative"]) + MyToDecimal(dr["Electricity_Clinker_Accumulative"])) / MyToDecimal(dr["Output_Clinker_Accumulative"]);
                }
                //吨熟料实物煤耗-本月
                if (MyToDecimal(dr["Output_Clinker_Monthly"])!=0)
                {
                    dr["ComprehensiveCoalConsumption_Monthly"] = MyToDecimal(dr["Consumption_CoalDust_Monthly"]) / MyToDecimal(dr["Output_Clinker_Monthly"]);
                }
                //吨熟料实物煤耗-累计
                if (MyToDecimal(dr["Output_Clinker_Accumulative"])!=0)
                {
                    dr["ComprehensiveCoalConsumption_Accumulative"] = MyToDecimal(dr["Consumption_CoalDust_Accumulative"])*1000 / MyToDecimal(dr["Output_Clinker_Accumulative"]);
                }
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
