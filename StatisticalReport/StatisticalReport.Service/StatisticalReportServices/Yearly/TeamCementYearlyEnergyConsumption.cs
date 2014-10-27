using SqlServerDataAdapter;
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
    public class TeamCementYearlyEnergyConsumption                          //zcs
    {
        private static TZHelper _tzHelper;

        static TeamCementYearlyEnergyConsumption()
        {
            string connString = ConnectionStringFactory.NXJCConnectionString;
            _tzHelper = new TZHelper(connString);
        }

        public static DataTable TableQuery(string organizationID, string date)
        {
            DataTable result = _tzHelper.CreateTableStructure("report_TeamCementYearlyEnergyConsumption");

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

                DataTable temp = TeamCementMonthlyEnergyConsumption.TableQuery(organizationID, dateTime);
                result.ImportRow(temp.Rows[temp.Rows.Count - 1]);
            }
            for (int i = 0; i < result.Rows.Count; i++)
            {
                result.Rows[i]["vDate"] = i + 1;
            }

            string totalField = "TeamA_Electricity_Cement,TeamA_Electricity_CementGrinding,TeamA_Electricity_AdmixturePreparation,TeamA_Electricity_BagsBulk," +
                "TeamA_Output_Cement,TeamA_Output_BagsBulk,TeamA_ElectricityConsumption_Cement,TeamA_ElectricityConsumption_CementGrinding,TeamA_ElectricityConsumption_BagsBulk," +
                "TeamA_ComprehensiveElectricityConsumption,TeamB_Electricity_Cement,TeamB_Electricity_CementGrinding,TeamB_Electricity_AdmixturePreparation," +
                "TeamB_Electricity_BagsBulk,TeamB_Output_Cement,TeamB_Output_BagsBulk,TeamB_ElectricityConsumption_Cement,TeamB_ElectricityConsumption_CementGrinding," +
                "TeamB_ElectricityConsumption_BagsBulk,TeamB_ComprehensiveElectricityConsumption,TeamC_Electricity_Cement,TeamC_Electricity_CementGrinding," +
                "TeamC_Electricity_AdmixturePreparation,TeamC_Electricity_BagsBulk,TeamC_Output_Cement,TeamC_Output_BagsBulk,TeamC_ElectricityConsumption_Cement," +
                "TeamC_ElectricityConsumption_CementGrinding,TeamC_ElectricityConsumption_BagsBulk,TeamC_ComprehensiveElectricityConsumption,TeamD_Electricity_Cement," +
                "TeamD_Electricity_CementGrinding,TeamD_Electricity_AdmixturePreparation,TeamD_Electricity_BagsBulk,TeamD_Output_Cement,TeamD_Output_BagsBulk," +
                "TeamD_ElectricityConsumption_Cement,TeamD_ElectricityConsumption_CementGrinding,TeamD_ElectricityConsumption_BagsBulk,TeamD_ComprehensiveElectricityConsumption," +
                "Amountto_Electricity_Cement,Amountto_Electricity_CementGrinding,Amountto_Electricity_AdmixturePreparation,Amountto_Electricity_BagsBulk,Amountto_Output_Cement," +
                "Amountto_Output_BagsBulk,Amountto_ElectricityConsumption_Cement,Amountto_ElectricityConsumption_CementGrinding,Amountto_ElectricityConsumption_BagsBulk," +
                "Amountto_ComprehensiveElectricityConsumption";
            ReportHelper.GetTotal(result, "vDate", totalField);

            return result;
        }
    }
}