using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using StatisticalReport.Service.StatisticalReportServices.Monthly;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Yearly
{
    public class TeamClinkerYearlyProcessEnergyConsumption            //zcs
    {
        private static TZHelper _tzHelper;

        static TeamClinkerYearlyProcessEnergyConsumption()
        {
            string connString = ConnectionStringFactory.NXJCConnectionString;
            _tzHelper = new TZHelper(connString);
        }

        public static DataTable TableQuery(string organizationID, string date)
        {
            DataTable result = _tzHelper.CreateTableStructure("report_TeamClinkerYearlyProcessEnergyConsumption");

            int year = 0;
            int month = 0;
            int.TryParse(date, out year);
            if (year == System.DateTime.Now.Year)
            {
                month = System.DateTime.Now.Month - 1;
            }
            else
            {
                month = 12;
            }
            for (int i = 1; i <= month; i++)
            {
                string dateTime;
                if (i < 10)
                {
                    dateTime = date + "-" + "0" + i.ToString();
                }
                else
                {
                    dateTime = date + "-" + i.ToString();
                }

                DataTable temp = TeamClinkerMonthlyProcessEnergyConsumption.TableQuery(organizationID, dateTime);
                temp.Rows[temp.Rows.Count - 1]["vDate"] = i;
                result.ImportRow(temp.Rows[temp.Rows.Count - 1]);
            }
            //for (int i = 0; i < result.Rows.Count; i++)
            //{
            //    result.Rows[i]["vDate"] = i + 1;
            //}
            string totalField = "TeamA_Electricity_RawBatch,TeamA_Electricity_RawBatchGrinding,TeamA_Electricity_Clinker,TeamA_Electricity_CoalDust," +
                "TeamA_Consumption_CoalDust,TeamA_Output_RawBatch,TeamA_Output_Clinker,TeamA_Output_CoalDust,TeamA_Output_Cogeneration,TeamA_ElectricityConsumption_RawBatch," +
                "TeamA_ElectricityConsumption_RawBatchGrinding,TeamA_ElectricityConsumption_Clinker,TeamA_ElectricityConsumption_CoalDust,TeamA_ComprehensiveElectricityConsumption," +
                "TeamA_ComprehensiveCoalConsumption,TeamB_Electricity_RawBatch,TeamB_Electricity_RawBatchGrinding,TeamB_Electricity_Clinker,TeamB_Electricity_CoalDust," +
                "TeamB_Consumption_CoalDust,TeamB_Output_RawBatch,TeamB_Output_Clinker,TeamB_Output_CoalDust,TeamB_Output_Cogeneration,TeamB_ElectricityConsumption_RawBatch," +
                "TeamB_ElectricityConsumption_RawBatchGrinding,TeamB_ElectricityConsumption_Clinker,TeamB_ElectricityConsumption_CoalDust,TeamB_ComprehensiveElectricityConsumption," +
                "TeamB_ComprehensiveCoalConsumption,TeamC_Electricity_RawBatch,TeamC_Electricity_RawBatchGrinding,TeamC_Electricity_Clinker,TeamC_Electricity_CoalDust," +
                "TeamC_Consumption_CoalDust,TeamC_Output_RawBatch,TeamC_Output_Clinker,TeamC_Output_CoalDust,TeamC_Output_Cogeneration,TeamC_ElectricityConsumption_RawBatch," +
                "TeamC_ElectricityConsumption_RawBatchGrinding,TeamC_ElectricityConsumption_Clinker,TeamC_ElectricityConsumption_CoalDust,TeamC_ComprehensiveElectricityConsumption," +
                "TeamC_ComprehensiveCoalConsumption,TeamD_Electricity_RawBatch,TeamD_Electricity_RawBatchGrinding,TeamD_Electricity_Clinker,TeamD_Electricity_CoalDust," +
                "TeamD_Consumption_CoalDust,TeamD_Output_RawBatch,TeamD_Output_Clinker,TeamD_Output_CoalDust,TeamD_Output_Cogeneration,TeamD_ElectricityConsumption_RawBatch," +
                "TeamD_ElectricityConsumption_RawBatchGrinding,TeamD_ElectricityConsumption_Clinker,TeamD_ElectricityConsumption_CoalDust,TeamD_ComprehensiveElectricityConsumption," +
                "TeamD_ComprehensiveCoalConsumption,Amountto_Electricity_RawBatch,Amountto_Electricity_RawBatchGrinding,Amountto_Electricity_Clinker,Amountto_Electricity_CoalDust," +
                "Amountto_Consumption_CoalDust,Amountto_Output_RawBatch,Amountto_Output_Clinker,Amountto_Output_CoalDust,Amountto_Output_Cogeneration," +
                "Amountto_ElectricityConsumption_RawBatch,Amountto_ElectricityConsumption_RawBatchGrinding,Amountto_ElectricityConsumption_Clinker," +
                "Amountto_ElectricityConsumption_CoalDust,Amountto_ComprehensiveElectricityConsumption,Amountto_ComprehensiveCoalConsumption";
            ReportHelper.GetTotal(result, "vDate", totalField);

            return result;
        }
    }
}