﻿using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Monthly
{
    public class CoalMilMonthlyPeakerValleyFlatElectricityConsumption                  //zcs
    {
        public static TZHelper _tzHelper;

        static CoalMilMonthlyPeakerValleyFlatElectricityConsumption()
        {
            string connString = ConnectionStringFactory.NXJCConnectionString;
            _tzHelper = new TZHelper(connString);
        }

        public static DataTable TableQuery(string organizationID, string date)
        {
            DataTable temp1 = _tzHelper.CreateTableStructure("report_RawBatchMilMonthlyPeakerValleyFlatElectricityConsumption");

            DataTable temp2 = _tzHelper.GetReportData("tz_Report", organizationID, date, "table_ClinkerMonthlyOutput");
            foreach (DataRow dr in temp2.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)(dr["vDate"]);
                newRow["First_Output"] = Convert.ToInt64(dr["CoalDustProductionFirstShift"]);
                newRow["Second_Output"] = Convert.ToInt64(dr["CoalDustProductionSecondShift"]);
                newRow["Third_Output"] = Convert.ToInt64(dr["CoalDustProductionThirdShift"]);
                temp1.Rows.Add(newRow);
            }

            DataTable temp3 = _tzHelper.GetReportData("tz_Report", organizationID, date, "table_ClinkerMonthlyElectricity_peak");
            foreach (DataRow dr in temp3.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)dr["vDate"];
                newRow["First_Peak_Electricity"] = Convert.ToInt64(dr["CoalMillSystemFirstShift"]);
                newRow["Second_Peak_Electricity"] = Convert.ToInt64(dr["CoalMillSystemSecondShift"]);
                newRow["Third_Peak_Electricity"] = Convert.ToInt64(dr["CoalMillSystemThirdShift"]);
                temp1.Rows.Add(newRow);
            }

            DataTable temp4 = _tzHelper.GetReportData("tz_Report", organizationID, date, "table_ClinkerMonthlyElectricity_valley");
            foreach (DataRow dr in temp4.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)dr["vDate"];
                newRow["First_Valley_Electricity"] = Convert.ToInt64(dr["CoalMillSystemFirstShift"]);
                newRow["Second_Valley_Electricity"] = Convert.ToInt64(dr["CoalMillSystemSecondShift"]);
                newRow["Third_Valley_Electricity"] = Convert.ToInt64(dr["CoalMillSystemThirdShift"]);
                temp1.Rows.Add(newRow);
            }

            DataTable temp5 = _tzHelper.GetReportData("tz_Report", organizationID, date, "table_ClinkerMonthlyElectricity_flat");
            foreach (DataRow dr in temp5.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)dr["vDate"];
                newRow["First_Flat_Electricity"] = Convert.ToInt64(dr["CoalMillSystemFirstShift"]);
                newRow["Second_Flat_Electricity"] = Convert.ToInt64(dr["CoalMillSystemSecondShift"]);
                newRow["Third_Flat_Electricity"] = Convert.ToInt64(dr["CoalMillSystemThirdShift"]);
                temp1.Rows.Add(newRow);
            }
            string column = "First_Output,Second_Output,Third_Output," +
            "First_Peak_Electricity,Second_Peak_Electricity,Third_Peak_Electricity,First_Valley_Electricity,Second_Valley_Electricity,Third_Valley_Electricity," +
            "First_Flat_Electricity,Second_Flat_Electricity,Third_Flat_Electricity";
            temp1 = ReportHelper.MyTotalOn(temp1, "vDate", column);
            //ReportHelper.GetTotal(temp1, "vDate", column);

            Dictionary<int, decimal> peakValleyFlatElectrovalence = _tzHelper.GetPeakValleyFlatElectrovalence(organizationID);

            foreach (DataRow dr in temp1.Rows)
            {
                dr["First_Sum_Electricity"] = Convert.ToInt64(dr["First_Peak_Electricity"]) + Convert.ToInt64(dr["First_Valley_Electricity"]) + Convert.ToInt64(dr["First_Flat_Electricity"]);
                dr["Second_Sum_Electricity"] = Convert.ToInt64(dr["Second_Peak_Electricity"]) + Convert.ToInt64(dr["Second_Valley_Electricity"]) + Convert.ToInt64(dr["Second_Flat_Electricity"]);
                dr["Third_Sum_Electricity"] = Convert.ToInt64(dr["Third_Peak_Electricity"]) + Convert.ToInt64(dr["Third_Valley_Electricity"]) + Convert.ToInt64(dr["Third_Flat_Electricity"]);

                if (Convert.ToInt64(dr["First_Output"]) != 0)
                {
                    dr["First_ElectricityConsumption"] = Convert.ToDecimal(dr["First_Sum_Electricity"]) / Convert.ToDecimal(dr["First_Output"]);
                }
                if (Convert.ToInt64(dr["Second_Output"]) != 0)
                {
                    dr["Second_ElectricityConsumption"] = Convert.ToDecimal(dr["Second_Sum_Electricity"]) / Convert.ToDecimal(dr["Second_Output"]);
                }
                if (Convert.ToInt64(dr["Third_Output"]) != 0)
                {
                    dr["Third_ElectricityConsumption"] = Convert.ToDecimal(dr["Third_Sum_Electricity"]) / Convert.ToDecimal(dr["Third_Output"]);
                }

                dr["First_Cost"] = Convert.ToDecimal(dr["First_Peak_Electricity"]) * peakValleyFlatElectrovalence[1] + Convert.ToDecimal(dr["First_Valley_Electricity"]) * peakValleyFlatElectrovalence[2] + Convert.ToDecimal(dr["First_Flat_Electricity"]) * peakValleyFlatElectrovalence[3];
                dr["Second_Cost"] = Convert.ToDecimal(dr["Second_Peak_Electricity"]) * peakValleyFlatElectrovalence[1] + Convert.ToDecimal(dr["Second_Valley_Electricity"]) * peakValleyFlatElectrovalence[2] + Convert.ToDecimal(dr["Second_Flat_Electricity"]) * peakValleyFlatElectrovalence[3];
                dr["Third_Cost"] = Convert.ToDecimal(dr["Third_Peak_Electricity"]) * peakValleyFlatElectrovalence[1] + Convert.ToDecimal(dr["Third_Valley_Electricity"]) * peakValleyFlatElectrovalence[2] + Convert.ToDecimal(dr["Third_Flat_Electricity"]) * peakValleyFlatElectrovalence[3];

                dr["Amountto_Output"] = Convert.ToInt64(dr["First_Output"]) + Convert.ToInt64(dr["Second_Output"]) + Convert.ToInt64(dr["Third_Output"]);
                dr["Amountto_Peak_Electricity"] = Convert.ToInt64(dr["First_Peak_Electricity"]) + Convert.ToInt64(dr["Second_Peak_Electricity"]) + Convert.ToInt64(dr["Third_Peak_Electricity"]);
                dr["Amountto_Valley_Electricity"] = Convert.ToInt64(dr["First_Valley_Electricity"]) + Convert.ToInt64(dr["Second_Valley_Electricity"]) + Convert.ToInt64(dr["Third_Valley_Electricity"]);
                dr["Amountto_Flat_Electricity"] = Convert.ToInt64(dr["First_Flat_Electricity"]) + Convert.ToInt64(dr["Second_Flat_Electricity"]) + Convert.ToInt64(dr["Third_Flat_Electricity"]);
                dr["Amountto_Sum_Electricity"] = Convert.ToInt64(dr["First_Sum_Electricity"]) + Convert.ToInt64(dr["Second_Sum_Electricity"]) + Convert.ToInt64(dr["Third_Sum_Electricity"]);
                //dr["Amountto_ElectricityConsumption"] = (decimal)dr["First_ElectricityConsumption"] + (decimal)dr["Second_ElectricityConsumption"] + (decimal)dr["Third_ElectricityConsumption"];
                if (Convert.ToInt64(dr["Amountto_Output"]) != 0)
                {
                    dr["Amountto_ElectricityConsumption"] = Convert.ToDecimal(dr["Amountto_Sum_Electricity"]) / Convert.ToDecimal(dr["Amountto_Output"]);
                }
                dr["Amountto_Cost"] = (decimal)dr["First_Cost"] + (decimal)dr["Second_Cost"] + (decimal)dr["Third_Cost"];
            }

            return temp1;
        }
    }
}
