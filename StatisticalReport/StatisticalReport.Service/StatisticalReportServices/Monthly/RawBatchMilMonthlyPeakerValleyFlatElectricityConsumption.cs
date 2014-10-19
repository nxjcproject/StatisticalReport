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
    public class RawBatchMilMonthlyPeakerValleyFlatElectricityConsumption
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

            DataTable temp2 = _tzHelper.GetReportData("tz_Report", organizationID, date, "report_ClinkerMonthlyOutput");
            foreach (DataRow dr in temp2.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)(dr["vDate"]);
                newRow["First_Output"] = (decimal)dr["RawBatchProductionFirstShift"];
                newRow["Second_Output"] = (decimal)dr["RawBatchProductionSecondShift"];
                newRow["Third_Output"] = (decimal)dr["RawBatchProductionThirdShift"];
                temp1.Rows.Add(newRow);
            }

            DataTable temp3 = _tzHelper.GetReportData("tz_Report", organizationID, date, "report_ClinkerMonthlyElectricity_peak");
            foreach (DataRow dr in temp3.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)dr["vDate"];
                newRow["First_Peak_Electricity"] = (decimal)dr["RawBatchGrindingFirstShift"];
                newRow["Second_Peak_Electricity"] = (decimal)dr["RawBatchGrindingSecondShift"];
                newRow["Third_Peak_Electricity"] = (decimal)dr["RawBatchGrindingThirdShift"];
                temp1.Rows.Add(newRow);
            }

            DataTable temp4 = _tzHelper.GetReportData("tz_Report", organizationID, date, "report_ClinkerMonthlyElectricity_valley");
            foreach (DataRow dr in temp4.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)dr["vDate"];
                newRow["First_Valley_Electricity"] = (decimal)dr["RawBatchGrindingFirstShift"];
                newRow["Second_Valley_Electricity"] = (decimal)dr["RawBatchGrindingSecondShift"];
                newRow["Third_Valley_Electricity"] = (decimal)dr["RawBatchGrindingThirdShift"];
                temp1.Rows.Add(newRow);
            }

            DataTable temp5 = _tzHelper.GetReportData("tz_Report", organizationID, date, "report_ClinkerMonthlyElectricity_flat");
            foreach (DataRow dr in temp5.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)dr["vDate"];
                newRow["First_Flat_Electricity"] = (decimal)dr["RawBatchGrindingFirstShift"];
                newRow["Second_Flat_Electricity"] = (decimal)dr["RawBatchGrindingSecondShift"];
                newRow["Third_Flat_Electricity"] = (decimal)dr["RawBatchGrindingThirdShift"];
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
                dr["First_Sum_Electricity"] = (decimal)dr["First_Peak_Electricity"] + (decimal)dr["First_Valley_Electricity"] + (decimal)dr["First_Flat_Electricity"];
                dr["Second_Sum_Electricity"] = (decimal)dr["Second_Peak_Electricity"] + (decimal)dr["Second_Valley_Electricity"] + (decimal)dr["Second_Flat_Electricity"];
                dr["Third_Sum_Electricity"] = (decimal)dr["Third_Peak_Electricity"] + (decimal)dr["Third_Valley_Electricity"] + (decimal)dr["Third_Flat_Electricity"];

                if ((decimal)dr["First_Output"] != 0)
                {
                    dr["First_ElectricityConsumption"] = (decimal)dr["First_Sum_Electricity"] / (decimal)dr["First_Output"];
                }
                if ((decimal)dr["Second_Output"] != 0)
                {
                    dr["Second_ElectricityConsumption"] = (decimal)dr["Second_Sum_Electricity"] / (decimal)dr["Second_Output"];
                }
                if ((decimal)dr["Third_Output"] != 0)
                {
                    dr["Third_ElectricityConsumption"] = (decimal)dr["Third_Sum_Electricity"] / (decimal)dr["Third_Output"];
                }

                dr["First_Cost"] = (decimal)dr["First_Peak_Electricity"] * peakValleyFlatElectrovalence[1] + (decimal)dr["First_Valley_Electricity"] * peakValleyFlatElectrovalence[2] + (decimal)dr["First_Flat_Electricity"] * peakValleyFlatElectrovalence[3];
                dr["Second_Cost"] = (decimal)dr["Second_Peak_Electricity"] * peakValleyFlatElectrovalence[1] + (decimal)dr["Second_Valley_Electricity"] * peakValleyFlatElectrovalence[2] + (decimal)dr["Second_Flat_Electricity"] * peakValleyFlatElectrovalence[3];
                dr["Third_Cost"] = (decimal)dr["Third_Peak_Electricity"] * peakValleyFlatElectrovalence[1] + (decimal)dr["Third_Valley_Electricity"] * peakValleyFlatElectrovalence[2] + (decimal)dr["Third_Flat_Electricity"] * peakValleyFlatElectrovalence[3];

                dr["Amountto_Output"] = (decimal)dr["First_Output"] + (decimal)dr["Second_Output"] + (decimal)dr["Third_Output"];
                dr["Amountto_Peak_Electricity"] = (decimal)dr["First_Peak_Electricity"] + (decimal)dr["Second_Peak_Electricity"] + (decimal)dr["Third_Peak_Electricity"];
                dr["Amountto_Valley_Electricity"] = (decimal)dr["First_Valley_Electricity"] + (decimal)dr["Second_Valley_Electricity"] + (decimal)dr["Third_Valley_Electricity"];
                dr["Amountto_Flat_Electricity"] = (decimal)dr["First_Flat_Electricity"] + (decimal)dr["Second_Flat_Electricity"] + (decimal)dr["Third_Flat_Electricity"];
                dr["Amountto_Sum_Electricity"] = (decimal)dr["First_Sum_Electricity"] + (decimal)dr["Second_Sum_Electricity"] + (decimal)dr["Third_Sum_Electricity"];
                //dr["Amountto_ElectricityConsumption"] = (decimal)dr["First_ElectricityConsumption"] + (decimal)dr["Second_ElectricityConsumption"] + (decimal)dr["Third_ElectricityConsumption"];
                if ((decimal)dr["Amountto_Output"] != 0)
                {
                    dr["Amountto_ElectricityConsumption"] = (decimal)dr["Amountto_Sum_Electricity"] / (decimal)dr["Amountto_Output"];
                }
                dr["Amountto_Cost"] = (decimal)dr["First_Cost"] + (decimal)dr["Second_Cost"] + (decimal)dr["Third_Cost"];
            }

            return temp1;
        }
    }
}
