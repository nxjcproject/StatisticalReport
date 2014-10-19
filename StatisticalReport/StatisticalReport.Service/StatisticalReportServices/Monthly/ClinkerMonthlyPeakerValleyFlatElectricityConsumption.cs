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
    public class ClinkerMonthlyPeakerValleyFlatElectricityConsumption
    {
        #region PAGE

        private static readonly FileIO.XmlSerializerIO mXmlSerializerIO = new FileIO.XmlSerializerIO();
        public static string ReadReportHeaderFile(string myFilePath)
        {
            string m_ColumnsJsonData = "";
            if (File.Exists(myFilePath))
            {
                FormTableConvert.FormTemplate m_FormTableTemplate = (FormTableConvert.FormTemplate)mXmlSerializerIO.XmlSerializerFromFile(myFilePath, typeof(FormTableConvert.FormTemplate));
                DataTable m_DataTable = TableQuery("df863854-89ae-46e6-80e8-96f6db6471b4", "2014-10");
                string m_ColumnsJson = FormTableConvert.TemplateToEasyUIGridJson.ToEasyUIGridJson(m_FormTableTemplate);
                string m_DataTableJson = DataTypeConvert.DataTableConvertJson.DataTableToJson(m_DataTable, "rows", m_DataTable.Rows.Count, true);
                m_ColumnsJsonData = "{" + m_DataTableJson + "," + m_ColumnsJson + "}";
            }
            return m_ColumnsJsonData;
        }

        public static string CreateHtmlTable(string myFilePath)
        {
            FormTableConvert.FormTemplate m_FormTemplate = (FormTableConvert.FormTemplate)mXmlSerializerIO.XmlSerializerFromFile(myFilePath, typeof(FormTableConvert.FormTemplate));
            string[] m_TagData = new string[] { "10月份", "报表类型:日报表", "汇总人:某某某", "审批人:某某某" };
            DataTable m_DataTable = GetDataTable();
            if (m_FormTemplate != null && m_DataTable != null)
            {
                string m_HtmlTable = FormTableConvert.TemplateToHtmlTable.ToHtmlDataHtmlTable(m_FormTemplate, m_TagData, m_DataTable, FormTableConvert.ConvertType.Print);
                //string m_HtmlTable = FormTableConvert.TemplateToHtmlTable.ToHtmlSourceHtmlTable(m_FormTemplate);
                return m_HtmlTable;
            }
            else
            {
                return "";
            }
        }

        public static void ExportExcelFile(string myFileType, string myFileName, string myData)
        {
            if (myFileType == "xls")
            {
                UpDownLoadFiles.DownloadFile.ExportExcelFile(myFileName, myData);
            }
        }

        private static DataTable GetDataTable()
        {
            DataTable m_DataTable = new DataTable("ReportTemplate");
            for (int i = 0; i < 17; i++)
            {
                m_DataTable.Columns.Add("Col" + i.ToString(), typeof(String));
                //m_DataTable.Columns.Add("cold" + i.ToString(), typeof(String));
            }
            for (int i = 0; i < 20; i++)
            {
                m_DataTable.Rows.Add(GetDataRow(i));
            }
            return m_DataTable;
        }
        private static string[] GetDataRow(int myIndex)
        {
            string[] m_DataRow = new string[17];
            for (int i = 0; i < m_DataRow.Length; i++)
            {
                m_DataRow[i] = myIndex.ToString() + "__" + i.ToString();
            }
            return m_DataRow;
        }

        #endregion
        private static TZHelper _tzHelper;

        static ClinkerMonthlyPeakerValleyFlatElectricityConsumption()
        {
            string connString = ConnectionStringFactory.NXJCConnectionString;
            _tzHelper = new TZHelper(connString);
        }

        public static DataTable TableQuery(string organizationID, string date)
        {
            DataTable temp1 = _tzHelper.CreateTableStructure("report_ClinkerMonthlyPeakerValleyFlatElectricityConsumption");

            DataTable temp2 = _tzHelper.GetReportData("tz_Report", organizationID, date, "report_ClinkerMonthlyOutput");
            foreach (DataRow dr in temp2.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)(dr["vDate"]);
                newRow["First_RawBatch_Output"] = (decimal)dr["RawBatchProductionFirstShift"];
                newRow["First_Clinker_Output"] = (decimal)dr["ClinkerProductionFirstShift"];
                newRow["First_CoalDust_Output"] = (decimal)dr["CoalDustProductionFirstShift"];
                newRow["Second_RawBatch_Output"] = (decimal)dr["RawBatchProductionSecondShift"];
                newRow["Second_Clinker_Output"] = (decimal)dr["ClinkerProductionSecondShift"];
                newRow["Second_CoalDust_Output"] = (decimal)dr["CoalDustProductionSecondShift"];
                newRow["Third_RawBatch_Output"] = (decimal)dr["RawBatchProductionThirdShift"];
                newRow["Third_Clinker_Output"] = (decimal)dr["ClinkerProductionThirdShift"];
                newRow["Third_CoalDust_Output"] = (decimal)dr["CoalDustProductionThirdShift"];
                temp1.Rows.Add(newRow);
            }

            DataTable temp3 = _tzHelper.GetReportData("tz_Report", organizationID, date, "report_ClinkerMonthlyElectricity_peak");
            foreach (DataRow dr in temp3.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)dr["vDate"];
                newRow["First_Peak_Electricity"] = (decimal)dr["AmounttoFirstShift"];
                newRow["Second_Peak_Electricity"] = (decimal)dr["AmounttoSecondShift"];
                newRow["Third_Peak_Electricity"] = (decimal)dr["AmounttoThirdShift"];
                temp1.Rows.Add(newRow);
            }

            DataTable temp4 = _tzHelper.GetReportData("tz_Report", organizationID, date, "report_ClinkerMonthlyElectricity_valley");
            foreach (DataRow dr in temp4.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)dr["vDate"];
                newRow["First_Valley_Electricity"] = (decimal)dr["AmounttoFirstShift"];
                newRow["Second_Valley_Electricity"] = (decimal)dr["AmounttoSecondShift"];
                newRow["Third_Valley_Electricity"] = (decimal)dr["AmounttoThirdShift"];
                temp1.Rows.Add(newRow);
            }

            DataTable temp5 = _tzHelper.GetReportData("tz_Report", organizationID, date, "report_ClinkerMonthlyElectricity_flat");
            foreach (DataRow dr in temp5.Rows)
            {
                DataRow newRow = temp1.NewRow();
                newRow["vDate"] = (string)dr["vDate"];
                newRow["First_Flat_Electricity"] = (decimal)dr["AmounttoFirstShift"];
                newRow["Second_Flat_Electricity"] = (decimal)dr["AmounttoSecondShift"];
                newRow["Third_Flat_Electricity"] = (decimal)dr["AmounttoThirdShift"];
                temp1.Rows.Add(newRow);
            }
            string column = "First_RawBatch_Output,First_Clinker_Output,First_CoalDust_Output,Second_RawBatch_Output,Second_Clinker_Output,Second_CoalDust_Output,Third_RawBatch_Output," +
            "Third_Clinker_Output,Third_CoalDust_Output,First_Peak_Electricity,Second_Peak_Electricity,Third_Peak_Electricity,First_Valley_Electricity,Second_Valley_Electricity,Third_Valley_Electricity," +
            "First_Flat_Electricity,Second_Flat_Electricity,Third_Flat_Electricity";
            temp1 = ReportHelper.MyTotalOn(temp1, "vDate", column);
            //ReportHelper.GetTotal(temp1, "vDate", column);

            Dictionary<int, decimal> peakValleyFlatElectrovalence = _tzHelper.GetPeakValleyFlatElectrovalence(organizationID);

            foreach (DataRow dr in temp1.Rows)
            {
                dr["First_Sum_Electricity"] = (decimal)dr["First_Peak_Electricity"] + (decimal)dr["First_Valley_Electricity"] + (decimal)dr["First_Flat_Electricity"];
                dr["Second_Sum_Electricity"] = (decimal)dr["Second_Peak_Electricity"] + (decimal)dr["Second_Valley_Electricity"] + (decimal)dr["Second_Flat_Electricity"];
                dr["Third_Sum_Electricity"] = (decimal)dr["Third_Peak_Electricity"] + (decimal)dr["Third_Valley_Electricity"] + (decimal)dr["Third_Flat_Electricity"];

                if ((decimal)dr["First_Clinker_Output"] != 0)
                {
                    dr["First_ElectricityConsumption"] = (decimal)dr["First_Sum_Electricity"] / (decimal)dr["First_Clinker_Output"];
                }
                if ((decimal)dr["Second_Clinker_Output"] != 0)
                {
                    dr["Second_ElectricityConsumption"] = (decimal)dr["Second_Sum_Electricity"] / (decimal)dr["Second_Clinker_Output"];
                }
                if ((decimal)dr["Third_Clinker_Output"] != 0)
                {
                    dr["Third_ElectricityConsumption"] = (decimal)dr["Third_Sum_Electricity"] / (decimal)dr["Third_Clinker_Output"];
                }

                dr["First_Cost"] = (decimal)dr["First_Peak_Electricity"] * peakValleyFlatElectrovalence[1] + (decimal)dr["First_Valley_Electricity"] * peakValleyFlatElectrovalence[2] + (decimal)dr["First_Flat_Electricity"] * peakValleyFlatElectrovalence[3];
                dr["Second_Cost"] = (decimal)dr["Second_Peak_Electricity"] * peakValleyFlatElectrovalence[1] + (decimal)dr["Second_Valley_Electricity"] * peakValleyFlatElectrovalence[2] + (decimal)dr["Second_Flat_Electricity"] * peakValleyFlatElectrovalence[3];
                dr["Third_Cost"] = (decimal)dr["Third_Peak_Electricity"] * peakValleyFlatElectrovalence[1] + (decimal)dr["Third_Valley_Electricity"] * peakValleyFlatElectrovalence[2] + (decimal)dr["Third_Flat_Electricity"] * peakValleyFlatElectrovalence[3];

                dr["Amountto_RawBatch_Output"] = (decimal)dr["First_RawBatch_Output"] + (decimal)dr["Second_RawBatch_Output"] + (decimal)dr["Third_RawBatch_Output"];
                dr["Amountto_Clinker_Output"] = (decimal)dr["First_Clinker_Output"] + (decimal)dr["Second_Clinker_Output"] + (decimal)dr["Third_Clinker_Output"];
                dr["Amountto_CoalDust_Output"] = (decimal)dr["First_CoalDust_Output"] + (decimal)dr["Second_CoalDust_Output"] + (decimal)dr["Third_CoalDust_Output"];
                dr["Amountto_Peak_Electricity"] = (decimal)dr["First_Peak_Electricity"] + (decimal)dr["Second_Peak_Electricity"] + (decimal)dr["Third_Peak_Electricity"];
                dr["Amountto_Valley_Electricity"] = (decimal)dr["First_Valley_Electricity"] + (decimal)dr["Second_Valley_Electricity"] + (decimal)dr["Third_Valley_Electricity"];
                dr["Amountto_Flat_Electricity"] = (decimal)dr["First_Flat_Electricity"] + (decimal)dr["Second_Flat_Electricity"] + (decimal)dr["Third_Flat_Electricity"];
                dr["Amountto_Sum_Electricity"] = (decimal)dr["First_Sum_Electricity"] + (decimal)dr["Second_Sum_Electricity"] + (decimal)dr["Third_Sum_Electricity"];
                //dr["Amountto_ElectricityConsumption"] = (decimal)dr["First_ElectricityConsumption"] + (decimal)dr["Second_ElectricityConsumption"] + (decimal)dr["Third_ElectricityConsumption"];
                if ((decimal)dr["Amountto_Clinker_Output"] != 0)
                {
                    dr["Amountto_ElectricityConsumption"] = (decimal)dr["Amountto_Sum_Electricity"] / (decimal)dr["Amountto_Clinker_Output"];
                }
                dr["Amountto_Cost"] = (decimal)dr["First_Cost"] + (decimal)dr["Second_Cost"] + (decimal)dr["Third_Cost"];
            }

            return temp1;
        }
    }
}
