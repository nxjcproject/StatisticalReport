using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Monthly                   //zcs
{
    public class CementMilMonthlyEnergyConsumption
    {
        public static TZHelper _tzHelper;

        static CementMilMonthlyEnergyConsumption()
        {
            string connString = ConnectionStringFactory.NXJCConnectionString;
            _tzHelper = new TZHelper(connString);
        }

        public static DataTable TableQuery(string organizationID, string date)
        {
            DataTable temp1 = _tzHelper.CreateTableStructure("report_CementMilMonthlyEnergyConsumption");

            DataTable temp2 = _tzHelper.GetReportData("tz_Report", organizationID, date, "table_CementMillYearlyElectricity_sum");
            foreach (DataRow dr in temp2.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)(dr["vDate"]);
                newRow["First_Electricity_Cement"] = dr["AmounttoCementPreparationFirstShift"];
                newRow["First_Electricity_CementGrinding"] = Convert.ToInt64(dr["CementGrindingFirstShift"]);
                newRow["First_Electricity_AdmixturePreparation"] = Convert.ToInt64(dr["AdmixturePreparationFirstShift"]);
                newRow["First_Electricity_BagsBulk"] = Convert.ToInt64(dr["AmounttoCementPackagingFirstShift"]);

                newRow["Second_Electricity_Cement"] = Convert.ToInt64(dr["AmounttoCementPreparationSecondShift"]);
                newRow["Second_Electricity_CementGrinding"] = Convert.ToInt64(dr["CementGrindingSecondShift"]);
                newRow["Second_Electricity_AdmixturePreparation"] = Convert.ToInt64(dr["AdmixturePreparationSecondShift"]);
                newRow["Second_Electricity_BagsBulk"] = Convert.ToInt64(dr["AmounttoCementPackagingSecondShift"]);

                newRow["Third_Electricity_Cement"] = Convert.ToInt64(dr["AmounttoCementPreparationThirdShift"]);
                newRow["Third_Electricity_CementGrinding"] = Convert.ToInt64(dr["CementGrindingThirdShift"]);
                newRow["Third_Electricity_AdmixturePreparation"] = Convert.ToInt64(dr["AdmixturePreparationThirdShift"]);
                newRow["Third_Electricity_BagsBulk"] = Convert.ToInt64(dr["AmounttoCementPackagingThirdShift"]);

                temp1.Rows.Add(newRow);
            }

            DataTable temp3 = _tzHelper.GetReportData("tz_Report", organizationID, date, "table_CementMillMonthlyOutput");
            foreach (DataRow dr in temp3.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)dr["vDate"];
                newRow["First_Output_Cement"] = Convert.ToInt64(dr["CementProductionFirstShift"]);
                //newRow["First_Output_BagsBulk"] = (decimal)dr[""];

                newRow["Second_Output_Cement"] = Convert.ToInt64(dr["CementProductionSecondShift"]);
                //newRow["Second_Output_BagsBulk"] = (decimal)dr[""];

                newRow["Third_Output_Cement"] = Convert.ToInt64(dr["CementProductionThirdShift"]);
                //newRow["Third_Output_BagsBulk"] = (decimal)dr[""];

                temp1.Rows.Add(newRow);
            }

            string column = "First_Electricity_Cement,First_Electricity_CementGrinding,First_Electricity_AdmixturePreparation,First_Electricity_BagsBulk,"
                + "Second_Electricity_Cement,Second_Electricity_CementGrinding,Second_Electricity_AdmixturePreparation,Second_Electricity_BagsBulk,"
                + "Third_Electricity_Cement,Third_Electricity_CementGrinding,Third_Electricity_AdmixturePreparation,Third_Electricity_BagsBulk,"
                + "First_Output_Cement,Second_Output_Cement,Third_Output_Cement";
            temp1 = ReportHelper.MyTotalOn(temp1, "vDate", column);
            ReportHelper.GetTotal(temp1, "vDate", column);

            foreach (DataRow dr in temp1.Rows)
            {
                if (Convert.ToInt64(dr["First_Output_Cement"]) != 0)
                {
                    dr["First_ElectricityConsumption_Cement"] = Convert.ToDecimal(dr["First_Electricity_Cement"]) / Convert.ToDecimal(dr["First_Output_Cement"]);
                }
                if (Convert.ToInt64(dr["Second_Output_Cement"]) != 0)
                {
                    dr["Second_ElectricityConsumption_Cement"] = Convert.ToDecimal(dr["Second_Electricity_Cement"]) / Convert.ToDecimal(dr["Second_Output_Cement"]);
                }
                if (Convert.ToInt64(dr["Third_Output_Cement"]) != 0)
                {
                    dr["Third_ElectricityConsumption_Cement"] = Convert.ToDecimal(dr["Third_Electricity_Cement"]) / Convert.ToDecimal(dr["Third_Output_Cement"]);
                }

                dr["Amountto_Electricity_Cement"] = Convert.ToInt64(dr["First_Electricity_Cement"]) + Convert.ToInt64(dr["Second_Electricity_Cement"]) + Convert.ToInt64(dr["Third_Electricity_Cement"]);
                dr["Amountto_Electricity_CementGrinding"] = Convert.ToInt64(dr["First_Electricity_CementGrinding"]) + Convert.ToInt64(dr["Second_Electricity_CementGrinding"]) + Convert.ToInt64(dr["Third_Electricity_CementGrinding"]);
                dr["Amountto_Electricity_AdmixturePreparation"] = Convert.ToInt64(dr["First_Electricity_AdmixturePreparation"]) + Convert.ToInt64(dr["Second_Electricity_AdmixturePreparation"]) + Convert.ToInt64(dr["Third_Electricity_AdmixturePreparation"]);
                dr["Amountto_Electricity_BagsBulk"] = Convert.ToInt64(dr["First_Electricity_BagsBulk"]) + Convert.ToInt64(dr["Second_Electricity_BagsBulk"]) + Convert.ToInt64(dr["Third_Electricity_BagsBulk"]);
                dr["Amountto_Output_Cement"] = Convert.ToInt64(dr["First_Output_Cement"]) + Convert.ToInt64(dr["Second_Output_Cement"]) + Convert.ToInt64(dr["Third_Output_Cement"]);
                //dr["Amountto_Output_BagsBulk"] = (decimal)dr["First_ElectricityConsumption"] + (decimal)dr["Second_ElectricityConsumption"] + (decimal)dr["Third_ElectricityConsumption"];

                if (Convert.ToInt64(dr["Amountto_Output_Cement"]) != 0)
                {
                    dr["Amountto_ElectricityConsumption_Cement"] = Convert.ToDecimal(dr["Amountto_Electricity_Cement"]) / Convert.ToDecimal(dr["Amountto_Output_Cement"]);
                }
            }

            return temp1;
        }
    }
}
