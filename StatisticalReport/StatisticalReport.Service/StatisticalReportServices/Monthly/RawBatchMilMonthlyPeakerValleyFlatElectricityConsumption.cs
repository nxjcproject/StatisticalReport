using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Monthly
{
    public class RawBatchMilMonthlyPeakerValleyFlatElectricityConsumption                  //zcs
    {
        public static TZHelper _tzHelper;

        static RawBatchMilMonthlyPeakerValleyFlatElectricityConsumption()
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
                newRow["First_Output"] = ReportHelper.MyToInt64(dr["RawBatchProductionFirstShift"]);
                newRow["Second_Output"] = ReportHelper.MyToInt64(dr["RawBatchProductionSecondShift"]);
                newRow["Third_Output"] = ReportHelper.MyToInt64(dr["RawBatchProductionThirdShift"]);
                temp1.Rows.Add(newRow);
            }

            DataTable temp3 = _tzHelper.GetReportData("tz_Report", organizationID, date, "table_ClinkerMonthlyElectricity_peak");
            foreach (DataRow dr in temp3.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)dr["vDate"];
                newRow["First_Peak_Electricity"] = ReportHelper.MyToInt64(dr["RawBatchGrindingFirstShift"]);
                newRow["Second_Peak_Electricity"] = ReportHelper.MyToInt64(dr["RawBatchGrindingSecondShift"]);
                newRow["Third_Peak_Electricity"] = ReportHelper.MyToInt64(dr["RawBatchGrindingThirdShift"]);
                temp1.Rows.Add(newRow);
            }

            DataTable temp4 = _tzHelper.GetReportData("tz_Report", organizationID, date, "table_ClinkerMonthlyElectricity_valley");
            foreach (DataRow dr in temp4.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)dr["vDate"];
                newRow["First_Valley_Electricity"] = ReportHelper.MyToInt64(dr["RawBatchGrindingFirstShift"]);
                newRow["Second_Valley_Electricity"] = ReportHelper.MyToInt64(dr["RawBatchGrindingSecondShift"]);
                newRow["Third_Valley_Electricity"] = ReportHelper.MyToInt64(dr["RawBatchGrindingThirdShift"]);
                temp1.Rows.Add(newRow);
            }

            DataTable temp5 = _tzHelper.GetReportData("tz_Report", organizationID, date, "table_ClinkerMonthlyElectricity_flat");
            foreach (DataRow dr in temp5.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)dr["vDate"];
                newRow["First_Flat_Electricity"] = ReportHelper.MyToInt64(dr["RawBatchGrindingFirstShift"]);
                newRow["Second_Flat_Electricity"] = ReportHelper.MyToInt64(dr["RawBatchGrindingSecondShift"]);
                newRow["Third_Flat_Electricity"] = ReportHelper.MyToInt64(dr["RawBatchGrindingThirdShift"]);
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
                dr["First_Sum_Electricity"] = ReportHelper.MyToInt64(dr["First_Peak_Electricity"]) + ReportHelper.MyToInt64(dr["First_Valley_Electricity"]) + ReportHelper.MyToInt64(dr["First_Flat_Electricity"]);
                dr["Second_Sum_Electricity"] = ReportHelper.MyToInt64(dr["Second_Peak_Electricity"]) + ReportHelper.MyToInt64(dr["Second_Valley_Electricity"]) + ReportHelper.MyToInt64(dr["Second_Flat_Electricity"]);
                dr["Third_Sum_Electricity"] = ReportHelper.MyToInt64(dr["Third_Peak_Electricity"]) + ReportHelper.MyToInt64(dr["Third_Valley_Electricity"]) + ReportHelper.MyToInt64(dr["Third_Flat_Electricity"]);

                if (ReportHelper.MyToInt64(dr["First_Output"]) != 0)
                {
                    dr["First_ElectricityConsumption"] = ReportHelper.MyToDecimal(dr["First_Sum_Electricity"]) / ReportHelper.MyToDecimal(dr["First_Output"]);
                }
                if (ReportHelper.MyToInt64(dr["Second_Output"]) != 0)
                {
                    dr["Second_ElectricityConsumption"] = ReportHelper.MyToDecimal(dr["Second_Sum_Electricity"]) / ReportHelper.MyToDecimal(dr["Second_Output"]);
                }
                if (ReportHelper.MyToInt64(dr["Third_Output"]) != 0)
                {
                    dr["Third_ElectricityConsumption"] = ReportHelper.MyToDecimal(dr["Third_Sum_Electricity"]) / ReportHelper.MyToDecimal(dr["Third_Output"]);
                }

                dr["First_Cost"] = ReportHelper.MyToDecimal(dr["First_Peak_Electricity"]) * peakValleyFlatElectrovalence[1] + ReportHelper.MyToDecimal(dr["First_Valley_Electricity"]) * peakValleyFlatElectrovalence[2] + ReportHelper.MyToDecimal(dr["First_Flat_Electricity"]) * peakValleyFlatElectrovalence[3];
                dr["Second_Cost"] = ReportHelper.MyToDecimal(dr["Second_Peak_Electricity"]) * peakValleyFlatElectrovalence[1] + ReportHelper.MyToDecimal(dr["Second_Valley_Electricity"]) * peakValleyFlatElectrovalence[2] + ReportHelper.MyToDecimal(dr["Second_Flat_Electricity"]) * peakValleyFlatElectrovalence[3];
                dr["Third_Cost"] = ReportHelper.MyToDecimal(dr["Third_Peak_Electricity"]) * peakValleyFlatElectrovalence[1] + ReportHelper.MyToDecimal(dr["Third_Valley_Electricity"]) * peakValleyFlatElectrovalence[2] + ReportHelper.MyToDecimal(dr["Third_Flat_Electricity"]) * peakValleyFlatElectrovalence[3];

                dr["Amountto_Output"] = ReportHelper.MyToInt64(dr["First_Output"]) + ReportHelper.MyToInt64(dr["Second_Output"]) + ReportHelper.MyToInt64(dr["Third_Output"]);
                dr["Amountto_Peak_Electricity"] = ReportHelper.MyToInt64(dr["First_Peak_Electricity"]) + ReportHelper.MyToInt64(dr["Second_Peak_Electricity"]) + ReportHelper.MyToInt64(dr["Third_Peak_Electricity"]);
                dr["Amountto_Valley_Electricity"] = ReportHelper.MyToInt64(dr["First_Valley_Electricity"]) + ReportHelper.MyToInt64(dr["Second_Valley_Electricity"]) + ReportHelper.MyToInt64(dr["Third_Valley_Electricity"]);
                dr["Amountto_Flat_Electricity"] = ReportHelper.MyToInt64(dr["First_Flat_Electricity"]) + ReportHelper.MyToInt64(dr["Second_Flat_Electricity"]) + ReportHelper.MyToInt64(dr["Third_Flat_Electricity"]);
                dr["Amountto_Sum_Electricity"] = ReportHelper.MyToInt64(dr["First_Sum_Electricity"]) + ReportHelper.MyToInt64(dr["Second_Sum_Electricity"]) + ReportHelper.MyToInt64(dr["Third_Sum_Electricity"]);
                //dr["Amountto_ElectricityConsumption"] = (decimal)dr["First_ElectricityConsumption"] + (decimal)dr["Second_ElectricityConsumption"] + (decimal)dr["Third_ElectricityConsumption"];
                if (ReportHelper.MyToInt64(dr["Amountto_Output"]) != 0)
                {
                    dr["Amountto_ElectricityConsumption"] = ReportHelper.MyToDecimal(dr["Amountto_Sum_Electricity"]) / ReportHelper.MyToDecimal(dr["Amountto_Output"]);
                }
                dr["Amountto_Cost"] = ReportHelper.MyToDecimal(dr["First_Cost"]) + ReportHelper.MyToDecimal(dr["Second_Cost"]) + ReportHelper.MyToDecimal(dr["Third_Cost"]);
            }

            return temp1;
        }
    }
}
