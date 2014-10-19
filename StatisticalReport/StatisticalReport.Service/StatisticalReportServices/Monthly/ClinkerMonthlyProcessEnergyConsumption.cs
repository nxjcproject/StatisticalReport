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
    public class ClinkerMonthlyProcessEnergyConsumption
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

            DataTable temp2 = _tzHelper.GetReportData("tz_Report", organizationID, date, "report_ClinkerMonthlyElectricity_sum");
            foreach (DataRow dr in temp2.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)(dr["vDate"]);

                newRow["First_Electricity_RawBatch"] = (decimal)dr["AmounttoRawBatchPreparationFirstShift"];
                newRow["First_Electricity_RawBatchGrinding"] = (decimal)dr["RawBatchGrindingFirstShift"];
                newRow["First_Electricity_Clinker"] = (decimal)dr["AmounttoFiringSystemFirstShift"];
                newRow["First_Electricity_CoalDust"] = (decimal)dr["CoalMillSystemFirstShift"];

                newRow["Second_Electricity_RawBatch"] = (decimal)dr["AmounttoRawBatchPreparationSecondShift"];
                newRow["Second_Electricity_RawBatchGrinding"] = (decimal)dr["RawBatchGrindingSecondShift"];
                newRow["Second_Electricity_Clinker"] = (decimal)dr["AmounttoFiringSystemSecondShift"];
                newRow["Second_Electricity_CoalDust"] = (decimal)dr["CoalMillSystemSecondShift"];

                newRow["Third_Electricity_RawBatch"] = (decimal)dr["AmounttoRawBatchPreparationThirdShift"];
                newRow["Third_Electricity_RawBatchGrinding"] = (decimal)dr["RawBatchGrindingThirdShift"];
                newRow["Third_Electricity_Clinker"] = (decimal)dr["AmounttoFiringSystemThirdShift"];
                newRow["Third_Electricity_CoalDust"] = (decimal)dr["CoalMillSystemThirdShift"];

                temp1.Rows.Add(newRow);
            }

            DataTable temp3 = _tzHelper.GetReportData("tz_Report", organizationID, date, "report_ClinkerMonthlyOutput");
            foreach (DataRow dr in temp3.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)dr["vDate"];

                newRow["First_Consumption_CoalDust"] = (decimal)dr["AmounttoCoalDustConsumptionFirstShift"];
                newRow["First_Output_RawBatch"] = (decimal)dr["RawBatchProductionFirstShift"];
                newRow["First_Output_Clinker"] = (decimal)dr["ClinkerProductionFirstShift"];
                newRow["First_Output_CoalDust"] = (decimal)dr["CoalDustProductionFirstShift"];
                newRow["First_Output_Cogeneration"] = (decimal)dr["PowerGenerationFirstShift"];

                newRow["Second_Consumption_CoalDust"] = (decimal)dr["AmounttoCoalDustConsumptionSecondShift"];
                newRow["Second_Output_RawBatch"] = (decimal)dr["RawBatchProductionSecondShift"];
                newRow["Second_Output_Clinker"] = (decimal)dr["ClinkerProductionSecondShift"];
                newRow["Second_Output_CoalDust"] = (decimal)dr["CoalDustProductionSecondShift"];
                newRow["Second_Output_Cogeneration"] = (decimal)dr["PowerGenerationSecondShift"];

                newRow["Third_Consumption_CoalDust"] = (decimal)dr["AmounttoCoalDustConsumptionThirdShift"];
                newRow["Third_Output_RawBatch"] = (decimal)dr["RawBatchProductionThirdShift"];
                newRow["Third_Output_Clinker"] = (decimal)dr["ClinkerProductionThirdShift"];
                newRow["Third_Output_CoalDust"] = (decimal)dr["CoalDustProductionThirdShift"];
                newRow["Third_Output_Cogeneration"] = (decimal)dr["PowerGenerationThirdShift"];

                temp1.Rows.Add(newRow);
            }

            string column = "First_Electricity_RawBatch,First_Electricity_RawBatchGrinding,First_Electricity_Clinker,First_Electricity_CoalDust" +
                "Second_Electricity_RawBatch,Second_Electricity_RawBatchGrinding,Second_Electricity_Clinker,Second_Electricity_CoalDust" +
                "Third_Electricity_RawBatch,Third_Electricity_RawBatchGrinding,Third_Electricity_Clinker,Third_Electricity_CoalDust" +
                "First_Consumption_CoalDust,First_Output_RawBatch,First_Output_Clinker,First_Output_CoalDust,First_Output_Cogeneration" +
                "Second_Consumption_CoalDust,Second_Output_RawBatch,Second_Output_Clinker,Second_Output_CoalDust,Second_Output_Cogeneration" +
                "Third_Consumption_CoalDust,Third_Output_RawBatch,Third_Output_Clinker,Third_Output_CoalDust,Third_Output_Cogeneration";
            temp1 = ReportHelper.MyTotalOn(temp1, "vDate", column);
            //ReportHelper.GetTotal(temp1, "vDate", column);

            //Dictionary<int, decimal> peakValleyFlatElectrovalence = _tzHelper.GetPeakValleyFlatElectrovalence(organizationID);

            foreach (DataRow dr in temp1.Rows)
            {
                /////////////////////////////////////////////////////生料制备和生料磨电耗//////////////////////////////////////////////////////////////////////////////
                if ((decimal)dr["First_Output_RawBatch"] != 0)
                {
                    dr["First_ElectricityConsumption_RawBatch"] = (decimal)dr["First_Electricity_RawBatch"] / (decimal)dr["First_Output_RawBatch"];
                    dr["First_ElectricityConsumption_RawBatchGrinding"] = (decimal)dr["First_Electricity_RawBatchGrinding"] / (decimal)dr["First_Output_RawBatch"];
                }
                if ((decimal)dr["Second_Output_RawBatch"] != 0)
                {
                    dr["Second_ElectricityConsumption_RawBatch"] = (decimal)dr["Second_Electricity_RawBatch"] / (decimal)dr["Second_Output_RawBatch"];
                    dr["Second_ElectricityConsumption_RawBatchGrinding"] = (decimal)dr["Second_Electricity_RawBatchGrinding"] / (decimal)dr["Second_Output_RawBatch"];
                }
                if ((decimal)dr["Third_Output_RawBatch"] != 0)
                {
                    dr["Third_ElectricityConsumption_RawBatch"] = (decimal)dr["Third_Electricity_RawBatch"] / (decimal)dr["Third_Output_RawBatch"];
                    dr["Third_ElectricityConsumption_RawBatchGrinding"] = (decimal)dr["Third_Electricity_RawBatchGrinding"] / (decimal)dr["Third_Output_RawBatch"];
                }
                ////////////////////////////////////////////////////熟料烧成电耗///////////////////////////////////////////////////////////////////////////
                if ((decimal)dr["First_Output_Clinker"] != 0)
                {
                    dr["First_ElectricityConsumption_Clinker"] = (decimal)dr["First_Electricity_Clinker"] / (decimal)dr["First_Output_Clinker"];
                }
                if ((decimal)dr["Second_Output_Clinker"] != 0)
                {
                    dr["Second_ElectricityConsumption_Clinker"] = (decimal)dr["Second_Electricity_Clinker"] / (decimal)dr["Second_Output_Clinker"];
                }
                if ((decimal)dr["Third_Output_Clinker"] != 0)
                {
                    dr["Third_ElectricityConsumption_Clinker"] = (decimal)dr["Third_Electricity_Clinker"] / (decimal)dr["Third_Output_Clinker"];
                }
                /////////////////////////////////////////////////////煤磨电耗//////////////////////////////////////////////////////////////////////////////////////
                if ((decimal)dr["First_Output_CoalDust"] != 0)
                {
                    dr["First_ElectricityConsumption_CoalDust"] = (decimal)dr["First_Electricity_CoalDust"] / (decimal)dr["First_Output_CoalDust"];
                }
                if ((decimal)dr["Second_Output_CoalDust"] != 0)
                {
                    dr["Second_ElectricityConsumption_CoalDust"] = (decimal)dr["Second_Electricity_CoalDust"] / (decimal)dr["Second_Output_CoalDust"];
                }
                if ((decimal)dr["First_Output_CoalDust"] != 0)
                {
                    dr["Third_ElectricityConsumption_CoalDust"] = (decimal)dr["Third_Electricity_CoalDust"] / (decimal)dr["Third_Output_CoalDust"];
                }
                ///////////////////////////////////////////////////吨熟料综合电耗//////////////////////////////////////////////////////////////////////
                if ((decimal)dr["First_Output_Clinker"] != 0)
                {
                    dr["First_ComprehensiveElectricityConsumption"] = ((decimal)dr["First_Electricity_RawBatch"] + (decimal)dr["First_Electricity_Clinker"]) / (decimal)dr["First_Output_Clinker"];
                }
                if ((decimal)dr["Second_Output_Clinker"] != 0)
                {
                    dr["Second_ComprehensiveElectricityConsumption"] = ((decimal)dr["Second_Electricity_RawBatch"] + (decimal)dr["Second_Electricity_Clinker"]) / (decimal)dr["Second_Output_Clinker"];
                }
                if ((decimal)dr["Third_Output_Clinker"] != 0)
                {
                    dr["Third_ComprehensiveElectricityConsumption"] = ((decimal)dr["Third_Electricity_RawBatch"] + (decimal)dr["Third_Electricity_Clinker"]) / (decimal)dr["Third_Output_Clinker"];
                }
                //////////////////////////////////////////////////吨熟料实物煤耗////////////////////////////////////////////////////////////
                if ((decimal)dr["First_Output_Clinker"] != 0)
                {
                    dr["First_ComprehensiveCoalConsumption"] = (decimal)dr["First_Consumption_CoalDust"] / (decimal)dr["First_Output_Clinker"];
                }
                if ((decimal)dr["Second_Output_Clinker"] != 0)
                {
                    dr["Second_ComprehensiveCoalConsumption"] = (decimal)dr["Second_Consumption_CoalDust"] / (decimal)dr["Second_Output_Clinker"];
                }
                if ((decimal)dr["Third_Output_Clinker"] != 0)
                {
                    dr["Third_ComprehensiveCoalConsumption"] = (decimal)dr["Third_Consumption_CoalDust"] / (decimal)dr["Third_Output_Clinker"];
                }


                /////////////////////////////////////////////////日合计//////////////////////////////////////////////////////////////////////
                dr["Amountto_Electricity_RawBatch"] = (decimal)dr["First_Electricity_RawBatch"] + (decimal)dr["Second_Electricity_RawBatch"] + (decimal)dr["Third_Electricity_RawBatch"];
                dr["Amountto_Electricity_RawBatchGrinding"] = (decimal)dr["First_Electricity_RawBatchGrinding"] + (decimal)dr["Second_Electricity_RawBatchGrinding"] + (decimal)dr["Third_Electricity_RawBatchGrinding"];
                dr["Amountto_Electricity_Clinker"] = (decimal)dr["First_Electricity_Clinker"] + (decimal)dr["Second_Electricity_Clinker"] + (decimal)dr["Third_Electricity_Clinker"];
                dr["Amountto_Electricity_CoalDust"] = (decimal)dr["First_Electricity_CoalDust"] + (decimal)dr["Second_Electricity_CoalDust"] + (decimal)dr["Third_Electricity_CoalDust"];
                dr["Amountto_Consumption_CoalDust"] = (decimal)dr["First_Consumption_CoalDust"] + (decimal)dr["Second_Consumption_CoalDust"] + (decimal)dr["Third_Consumption_CoalDust"];
                dr["Amountto_Output_RawBatch"] = (decimal)dr["First_Output_RawBatch"] + (decimal)dr["Second_Output_RawBatch"] + (decimal)dr["Third_Output_RawBatch"];
                dr["Amountto_Output_Clinker"] = (decimal)dr["First_Output_Clinker"] + (decimal)dr["Second_Output_Clinker"] + (decimal)dr["Third_Output_Clinker"];
                dr["Amountto_Output_CoalDust"] = (decimal)dr["First_Output_CoalDust"] + (decimal)dr["Second_Output_CoalDust"] + (decimal)dr["Third_Output_CoalDust"];
                dr["Amountto_Output_Cogeneration"] = (decimal)dr["First_Output_Cogeneration"] + (decimal)dr["Second_Output_Cogeneration"] + (decimal)dr["Third_Output_Cogeneration"];

                if ((decimal)dr["Amountto_Output_RawBatch"] != 0)
                {
                    dr["Amountto_ElectricityConsumption_RawBatch"] = (decimal)dr["Amountto_Electricity_RawBatch"] / (decimal)dr["Amountto_Output_RawBatch"];
                }
                if ((decimal)dr["Amountto_Output_RawBatch"] != 0)
                {
                    dr["Amountto_ElectricityConsumption_RawBatchGrinding"] = (decimal)dr["Amountto_Electricity_RawBatchGrinding"] / (decimal)dr["Amountto_Output_RawBatch"];
                }
                if ((decimal)dr["Amountto_Output_Clinker"] != 0)
                {
                    dr["Amountto_ElectricityConsumption_Clinker"] = (decimal)dr["Amountto_Electricity_Clinker"] / (decimal)dr["Amountto_Output_Clinker"];
                }
                if ((decimal)dr["Amountto_Output_CoalDust"] != 0)
                {
                    dr["Amountto_ElectricityConsumption_CoalDust"] = (decimal)dr["Amountto_Electricity_CoalDust"] / (decimal)dr["Amountto_Output_CoalDust"];
                }
                if ((decimal)dr["Amountto_Output_Clinker"] != 0)
                {
                    dr["Amountto_ComprehensiveElectricityConsumption"] = ((decimal)dr["Amountto_Electricity_RawBatch"] + (decimal)dr["Amountto_Electricity_Clinker"]) / (decimal)dr["Amountto_Output_Clinker"];
                }
                if ((decimal)dr["Amountto_Output_Clinker"] != 0)
                {
                    dr["Amountto_ComprehensiveCoalConsumption"] = (decimal)dr["Amountto_Consumption_CoalDust"] / (decimal)dr["Amountto_Output_Clinker"];
                }
            }

            return temp1;
        }
    }
}
