using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Monthly
{
    public class ClinkerMonthlyProcessEnergyConsumption                          //zcs
    {
        private static TZHelper _tzHelper;

        static ClinkerMonthlyProcessEnergyConsumption()
        {
            string connString = ConnectionStringFactory.NXJCConnectionString;
            _tzHelper = new TZHelper(connString);
        }

        public static DataTable TableQuery(string organizationID, string date)
        {
            DataTable temp1 = _tzHelper.CreateTableStructure("report_ClinkerMonthlyProcessEnergyConsumption");

            DataTable temp2 = _tzHelper.GetReportData("tz_Report", organizationID, date, "table_ClinkerMonthlyElectricity_sum");
            foreach (DataRow dr in temp2.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)(dr["vDate"]);

                newRow["First_Electricity_RawBatch"] = ReportHelper.MyToInt64(dr["AmounttoRawBatchPreparationFirstShift"]);
                newRow["First_Electricity_RawBatchGrinding"] = ReportHelper.MyToInt64(dr["RawBatchGrindingFirstShift"]);
                newRow["First_Electricity_Clinker"] = ReportHelper.MyToInt64(dr["AmounttoFiringSystemFirstShift"]);
                newRow["First_Electricity_CoalDust"] = ReportHelper.MyToInt64(dr["CoalMillSystemFirstShift"]);

                newRow["Second_Electricity_RawBatch"] = ReportHelper.MyToInt64(dr["AmounttoRawBatchPreparationSecondShift"]);
                newRow["Second_Electricity_RawBatchGrinding"] = ReportHelper.MyToInt64(dr["RawBatchGrindingSecondShift"]);
                newRow["Second_Electricity_Clinker"] = ReportHelper.MyToInt64(dr["AmounttoFiringSystemSecondShift"]);
                newRow["Second_Electricity_CoalDust"] = ReportHelper.MyToInt64(dr["CoalMillSystemSecondShift"]);

                newRow["Third_Electricity_RawBatch"] = ReportHelper.MyToInt64(dr["AmounttoRawBatchPreparationThirdShift"]);
                newRow["Third_Electricity_RawBatchGrinding"] = ReportHelper.MyToInt64(dr["RawBatchGrindingThirdShift"]);
                newRow["Third_Electricity_Clinker"] = ReportHelper.MyToInt64(dr["AmounttoFiringSystemThirdShift"]);
                newRow["Third_Electricity_CoalDust"] = ReportHelper.MyToInt64(dr["CoalMillSystemThirdShift"]);

                temp1.Rows.Add(newRow);
            }

            DataTable temp3 = _tzHelper.GetReportData("tz_Report", organizationID, date, "table_ClinkerMonthlyOutput");
            foreach (DataRow dr in temp3.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)dr["vDate"];

                newRow["First_Consumption_CoalDust"] = ReportHelper.MyToInt64(dr["AmounttoCoalDustConsumptionFirstShift"]);
                newRow["First_Output_RawBatch"] = ReportHelper.MyToInt64(dr["RawBatchProductionFirstShift"]);
                newRow["First_Output_Clinker"] = ReportHelper.MyToInt64(dr["ClinkerProductionFirstShift"]);
                newRow["First_Output_CoalDust"] = ReportHelper.MyToInt64(dr["CoalDustProductionFirstShift"]);
                newRow["First_Output_Cogeneration"] = ReportHelper.MyToInt64(dr["PowerGenerationFirstShift"]);

                newRow["Second_Consumption_CoalDust"] = ReportHelper.MyToInt64(dr["AmounttoCoalDustConsumptionSecondShift"]);
                newRow["Second_Output_RawBatch"] = ReportHelper.MyToInt64(dr["RawBatchProductionSecondShift"]);
                newRow["Second_Output_Clinker"] = ReportHelper.MyToInt64(dr["ClinkerProductionSecondShift"]);
                newRow["Second_Output_CoalDust"] = ReportHelper.MyToInt64(dr["CoalDustProductionSecondShift"]);
                newRow["Second_Output_Cogeneration"] = ReportHelper.MyToInt64(dr["PowerGenerationSecondShift"]);

                newRow["Third_Consumption_CoalDust"] = ReportHelper.MyToInt64(dr["AmounttoCoalDustConsumptionThirdShift"]);
                newRow["Third_Output_RawBatch"] = ReportHelper.MyToInt64(dr["RawBatchProductionThirdShift"]);
                newRow["Third_Output_Clinker"] = ReportHelper.MyToInt64(dr["ClinkerProductionThirdShift"]);
                newRow["Third_Output_CoalDust"] = ReportHelper.MyToInt64(dr["CoalDustProductionThirdShift"]);
                newRow["Third_Output_Cogeneration"] = ReportHelper.MyToInt64(dr["PowerGenerationThirdShift"]);

                temp1.Rows.Add(newRow);
            }

            string column = "First_Electricity_RawBatch,First_Electricity_RawBatchGrinding,First_Electricity_Clinker,First_Electricity_CoalDust," +
                "Second_Electricity_RawBatch,Second_Electricity_RawBatchGrinding,Second_Electricity_Clinker,Second_Electricity_CoalDust," +
                "Third_Electricity_RawBatch,Third_Electricity_RawBatchGrinding,Third_Electricity_Clinker,Third_Electricity_CoalDust," +
                "First_Consumption_CoalDust,First_Output_RawBatch,First_Output_Clinker,First_Output_CoalDust,First_Output_Cogeneration," +
                "Second_Consumption_CoalDust,Second_Output_RawBatch,Second_Output_Clinker,Second_Output_CoalDust,Second_Output_Cogeneration," +
                "Third_Consumption_CoalDust,Third_Output_RawBatch,Third_Output_Clinker,Third_Output_CoalDust,Third_Output_Cogeneration";
            temp1 = ReportHelper.MyTotalOn(temp1, "vDate", column);
            //ReportHelper.GetTotal(temp1, "vDate", column);

            //Dictionary<int, decimal> peakValleyFlatElectrovalence = _tzHelper.GetPeakValleyFlatElectrovalence(organizationID);


            foreach (DataRow dr in temp1.Rows)
            {
                /////////////////////////////////////////////////////生料制备和生料磨电耗//////////////////////////////////////////////////////////////////////////////
                if (ReportHelper.MyToDecimal(dr["First_Output_RawBatch"]) != 0)
                {
                    dr["First_ElectricityConsumption_RawBatch"] = ReportHelper.MyToDecimal(dr["First_Electricity_RawBatch"]) / ReportHelper.MyToDecimal(dr["First_Output_RawBatch"]);
                    dr["First_ElectricityConsumption_RawBatchGrinding"] = ReportHelper.MyToDecimal(dr["First_Electricity_RawBatchGrinding"]) / ReportHelper.MyToDecimal(dr["First_Output_RawBatch"]);
                }
                if (ReportHelper.MyToDecimal(dr["Second_Output_RawBatch"]) != 0)
                {
                    dr["Second_ElectricityConsumption_RawBatch"] = ReportHelper.MyToDecimal(dr["Second_Electricity_RawBatch"]) / ReportHelper.MyToDecimal(dr["Second_Output_RawBatch"]);
                    dr["Second_ElectricityConsumption_RawBatchGrinding"] = ReportHelper.MyToDecimal(dr["Second_Electricity_RawBatchGrinding"]) / ReportHelper.MyToDecimal(dr["Second_Output_RawBatch"]);
                }
                if (ReportHelper.MyToDecimal(dr["Third_Output_RawBatch"]) != 0)
                {
                    dr["Third_ElectricityConsumption_RawBatch"] = ReportHelper.MyToDecimal(dr["Third_Electricity_RawBatch"]) / ReportHelper.MyToDecimal(dr["Third_Output_RawBatch"]);
                    dr["Third_ElectricityConsumption_RawBatchGrinding"] = ReportHelper.MyToDecimal(dr["Third_Electricity_RawBatchGrinding"]) / ReportHelper.MyToDecimal(dr["Third_Output_RawBatch"]);
                }
                ////////////////////////////////////////////////////熟料烧成电耗///////////////////////////////////////////////////////////////////////////
                if (ReportHelper.MyToDecimal(dr["First_Output_Clinker"]) != 0)
                {
                    dr["First_ElectricityConsumption_Clinker"] = ReportHelper.MyToDecimal(dr["First_Electricity_Clinker"]) / ReportHelper.MyToDecimal(dr["First_Output_Clinker"]);
                }
                if (ReportHelper.MyToDecimal(dr["Second_Output_Clinker"]) != 0)
                {
                    dr["Second_ElectricityConsumption_Clinker"] = ReportHelper.MyToDecimal(dr["Second_Electricity_Clinker"]) / ReportHelper.MyToDecimal(dr["Second_Output_Clinker"]);
                }
                if (ReportHelper.MyToDecimal(dr["Third_Output_Clinker"]) != 0)
                {
                    dr["Third_ElectricityConsumption_Clinker"] = ReportHelper.MyToDecimal(dr["Third_Electricity_Clinker"]) / ReportHelper.MyToDecimal(dr["Third_Output_Clinker"]);
                }
                /////////////////////////////////////////////////////煤磨电耗//////////////////////////////////////////////////////////////////////////////////////
                if (ReportHelper.MyToDecimal(dr["First_Output_CoalDust"]) != 0)
                {
                    dr["First_ElectricityConsumption_CoalDust"] = ReportHelper.MyToDecimal(dr["First_Electricity_CoalDust"]) / ReportHelper.MyToDecimal(dr["First_Output_CoalDust"]);
                }
                if (ReportHelper.MyToDecimal(dr["Second_Output_CoalDust"]) != 0)
                {
                    dr["Second_ElectricityConsumption_CoalDust"] = ReportHelper.MyToDecimal(dr["Second_Electricity_CoalDust"]) / ReportHelper.MyToDecimal(dr["Second_Output_CoalDust"]);
                }
                if (ReportHelper.MyToDecimal(dr["Third_Output_CoalDust"]) != 0)
                {
                    dr["Third_ElectricityConsumption_CoalDust"] = ReportHelper.MyToDecimal(dr["Third_Electricity_CoalDust"]) / ReportHelper.MyToDecimal(dr["Third_Output_CoalDust"]);
                }
                ///////////////////////////////////////////////////吨熟料综合电耗//////////////////////////////////////////////////////////////////////
                if (ReportHelper.MyToDecimal(dr["First_Output_Clinker"]) != 0)
                {
                    dr["First_ComprehensiveElectricityConsumption"] = (ReportHelper.MyToDecimal(dr["First_Electricity_RawBatch"]) + ReportHelper.MyToDecimal(dr["First_Electricity_Clinker"])) / ReportHelper.MyToDecimal(dr["First_Output_Clinker"]);
                }
                if (ReportHelper.MyToDecimal(dr["Second_Output_Clinker"]) != 0)
                {
                    dr["Second_ComprehensiveElectricityConsumption"] = (ReportHelper.MyToDecimal(dr["Second_Electricity_RawBatch"]) + ReportHelper.MyToDecimal(dr["Second_Electricity_Clinker"])) / ReportHelper.MyToDecimal(dr["Second_Output_Clinker"]);
                }
                if (ReportHelper.MyToDecimal(dr["Third_Output_Clinker"]) != 0)
                {
                    dr["Third_ComprehensiveElectricityConsumption"] = (ReportHelper.MyToDecimal(dr["Third_Electricity_RawBatch"]) + ReportHelper.MyToDecimal(dr["Third_Electricity_Clinker"])) / ReportHelper.MyToDecimal(dr["Third_Output_Clinker"]);
                }
                //////////////////////////////////////////////////吨熟料实物煤耗////////////////////////////////////////////////////////////
                if (ReportHelper.MyToDecimal(dr["First_Output_Clinker"]) != 0)
                {
                    dr["First_ComprehensiveCoalConsumption"] = ReportHelper.MyToDecimal(dr["First_Consumption_CoalDust"])*1000 / ReportHelper.MyToDecimal(dr["First_Output_Clinker"]);
                }
                if (ReportHelper.MyToDecimal(dr["Second_Output_Clinker"]) != 0)
                {
                    dr["Second_ComprehensiveCoalConsumption"] = ReportHelper.MyToDecimal(dr["Second_Consumption_CoalDust"])*1000 / ReportHelper.MyToDecimal(dr["Second_Output_Clinker"]);
                }
                if (ReportHelper.MyToDecimal(dr["Third_Output_Clinker"]) != 0)
                {
                    dr["Third_ComprehensiveCoalConsumption"] = ReportHelper.MyToDecimal(dr["Third_Consumption_CoalDust"])*1000 / ReportHelper.MyToDecimal(dr["Third_Output_Clinker"]);
                }

                /////////////////////////////////////////////////日合计//////////////////////////////////////////////////////////////////////
                dr["Amountto_Electricity_RawBatch"] = ReportHelper.MyToDecimal(dr["First_Electricity_RawBatch"]) + ReportHelper.MyToDecimal(dr["Second_Electricity_RawBatch"]) + ReportHelper.MyToDecimal(dr["Third_Electricity_RawBatch"]);
                dr["Amountto_Electricity_RawBatchGrinding"] = ReportHelper.MyToDecimal(dr["First_Electricity_RawBatchGrinding"]) + ReportHelper.MyToDecimal(dr["Second_Electricity_RawBatchGrinding"]) + ReportHelper.MyToDecimal(dr["Third_Electricity_RawBatchGrinding"]);
                dr["Amountto_Electricity_Clinker"] = ReportHelper.MyToDecimal(dr["First_Electricity_Clinker"]) + ReportHelper.MyToDecimal(dr["Second_Electricity_Clinker"]) + ReportHelper.MyToDecimal(dr["Third_Electricity_Clinker"]);
                dr["Amountto_Electricity_CoalDust"] = ReportHelper.MyToDecimal(dr["First_Electricity_CoalDust"]) + ReportHelper.MyToDecimal(dr["Second_Electricity_CoalDust"]) + ReportHelper.MyToDecimal(dr["Third_Electricity_CoalDust"]);
                dr["Amountto_Consumption_CoalDust"] = ReportHelper.MyToDecimal(dr["First_Consumption_CoalDust"]) + ReportHelper.MyToDecimal(dr["Second_Consumption_CoalDust"]) + ReportHelper.MyToDecimal(dr["Third_Consumption_CoalDust"]);
                dr["Amountto_Output_RawBatch"] = ReportHelper.MyToDecimal(dr["First_Output_RawBatch"]) + ReportHelper.MyToDecimal(dr["Second_Output_RawBatch"]) + ReportHelper.MyToDecimal(dr["Third_Output_RawBatch"]);
                dr["Amountto_Output_Clinker"] = ReportHelper.MyToDecimal(dr["First_Output_Clinker"]) + ReportHelper.MyToDecimal(dr["Second_Output_Clinker"]) + ReportHelper.MyToDecimal(dr["Third_Output_Clinker"]);
                dr["Amountto_Output_CoalDust"] = ReportHelper.MyToDecimal(dr["First_Output_CoalDust"]) + ReportHelper.MyToDecimal(dr["Second_Output_CoalDust"]) + ReportHelper.MyToDecimal(dr["Third_Output_CoalDust"]);
                dr["Amountto_Output_Cogeneration"] = ReportHelper.MyToDecimal(dr["First_Output_Cogeneration"]) + ReportHelper.MyToDecimal(dr["Second_Output_Cogeneration"]) + ReportHelper.MyToDecimal(dr["Third_Output_Cogeneration"]);

                if (ReportHelper.MyToDecimal(dr["Amountto_Output_RawBatch"]) != 0)
                {
                    dr["Amountto_ElectricityConsumption_RawBatch"] = ReportHelper.MyToDecimal(dr["Amountto_Electricity_RawBatch"]) / ReportHelper.MyToDecimal(dr["Amountto_Output_RawBatch"]);
                }
                if (ReportHelper.MyToDecimal(dr["Amountto_Output_RawBatch"]) != 0)
                {
                    dr["Amountto_ElectricityConsumption_RawBatchGrinding"] = ReportHelper.MyToDecimal(dr["Amountto_Electricity_RawBatchGrinding"]) / ReportHelper.MyToDecimal(dr["Amountto_Output_RawBatch"]);
                }
                if (ReportHelper.MyToDecimal(dr["Amountto_Output_Clinker"]) != 0)
                {
                    dr["Amountto_ElectricityConsumption_Clinker"] = ReportHelper.MyToDecimal(dr["Amountto_Electricity_Clinker"]) / ReportHelper.MyToDecimal(dr["Amountto_Output_Clinker"]);
                }
                if (ReportHelper.MyToDecimal(dr["Amountto_Output_CoalDust"]) != 0)
                {
                    dr["Amountto_ElectricityConsumption_CoalDust"] = ReportHelper.MyToDecimal(dr["Amountto_Electricity_CoalDust"])*1000 / ReportHelper.MyToDecimal(dr["Amountto_Output_CoalDust"]);
                }
                if (ReportHelper.MyToDecimal(dr["Amountto_Output_Clinker"]) != 0)
                {
                    dr["Amountto_ComprehensiveElectricityConsumption"] = (ReportHelper.MyToDecimal(dr["Amountto_Electricity_RawBatch"]) + ReportHelper.MyToDecimal(dr["Amountto_Electricity_Clinker"])) / ReportHelper.MyToDecimal(dr["Amountto_Output_Clinker"]);
                }
                if (ReportHelper.MyToDecimal(dr["Amountto_Output_Clinker"]) != 0)
                {
                    dr["Amountto_ComprehensiveCoalConsumption"] = ReportHelper.MyToDecimal(dr["Amountto_Consumption_CoalDust"]) / ReportHelper.MyToDecimal(dr["Amountto_Output_Clinker"]);
                }
            }

            return temp1;
        }

        //private static decimal ReportHelper.MyToDecimal(object obj)
        //{
        //    if (obj is DBNull)
        //    {
        //        obj = 0;
        //        return Convert.ToDecimal(obj);
        //    }
        //    else
        //    {
        //        return Convert.ToDecimal(obj);
        //    }
        //}
    }
}
