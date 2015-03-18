using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices
{
    public class StatisticalReportHelper
    {
        private static readonly FileIO.XmlSerializerIO mXmlSerializerIO = new FileIO.XmlSerializerIO();
        public static string ReadReportHeaderFile(string myFilePath, DataTable myDataTable)
        {
            string m_ColumnsJsonData = "";
            if (File.Exists(myFilePath))
            {
                FormTableConvert.FormTemplate m_FormTableTemplate = (FormTableConvert.FormTemplate)mXmlSerializerIO.XmlSerializerFromFile(myFilePath, typeof(FormTableConvert.FormTemplate));
                //DataTable m_DataTable = GetDataTable();
                string m_ColumnsJson = FormTableConvert.TemplateToEasyUIGridJson.ToEasyUIGridJson(m_FormTableTemplate);
                //string m_DataTableJson = DataTypeConvert.DataTableConvertJson.DataTableToJson(myDataTable, "rows", myDataTable.Rows.Count, true);
                string m_DataTableJson = EasyUIJsonParser.DataGridJsonParser.GetDataRowJson(myDataTable);   //DataTypeConvert.DataTableConvertJson.DataTableToJson(myDataTable, "rows", myDataTable.Rows.Count, true);
                m_ColumnsJsonData = "{" + m_DataTableJson + "," + m_ColumnsJson + "}";
            }
            return m_ColumnsJsonData;
        }
        public static string CreatePrintHtmlTable(string myFilePath, DataTable myDataTable, string[] m_TagData)
        {
            FormTableConvert.FormTemplate m_FormTemplate = (FormTableConvert.FormTemplate)mXmlSerializerIO.XmlSerializerFromFile(myFilePath, typeof(FormTableConvert.FormTemplate));
            //string[] m_TagData = new string[] { "10月份", "报表类型:日报表", "汇总人:某某某", "审批人:某某某" };
            //DataTable m_DataTable = GetDataTable();
            if (m_FormTemplate != null && myDataTable != null)
            {
                string m_HtmlTable = FormTableConvert.TemplateToHtmlTable.ToHtmlDataHtmlTable(m_FormTemplate, m_TagData, myDataTable, FormTableConvert.ConvertType.Print);
                //string m_HtmlTable = FormTableConvert.TemplateToHtmlTable.ToHtmlSourceHtmlTable(m_FormTemplate);
                return m_HtmlTable;
            }
            else
            {
                return "";
            }
        }
        public static string CreateExportHtmlTable(string myFilePath, DataTable myDataTable, string[] m_TagData)
        {
            FormTableConvert.FormTemplate m_FormTemplate = (FormTableConvert.FormTemplate)mXmlSerializerIO.XmlSerializerFromFile(myFilePath, typeof(FormTableConvert.FormTemplate));
            //string[] m_TagData = new string[] { "10月份", "报表类型:日报表", "汇总人:某某某", "审批人:某某某" };
            //DataTable m_DataTable = GetDataTable();
            if (m_FormTemplate != null && myDataTable != null)
            {
                string m_HtmlTable = FormTableConvert.TemplateToHtmlTable.ToHtmlDataHtmlTable(m_FormTemplate, m_TagData, myDataTable, FormTableConvert.ConvertType.Export);
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
    }
}
