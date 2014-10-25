using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Net;
using System.IO;
using System.Data;
using StatisticalReport.Service.StatisticalReportServices;
using StatisticalReport.Service.StatisticalReportServices.Monthly;

namespace StatisticalReport.Web.UI_StatisticalReport.Yearly
{
    public partial class report_ClinkerYearlyProcessEnergyConsumption : WebStyleBaseForEnergy.webStyleBase
    {
        private static DataTable myDataTable;

        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();

            if (!IsPostBack)
            {

            }

            ///以下是接收js脚本中post过来的参数
            string m_FunctionName = Request.Form["myFunctionName"] == null ? "" : Request.Form["myFunctionName"].ToString();             //方法名称,调用后台不同的方法
            string m_Parameter1 = Request.Form["myParameter1"] == null ? "" : Request.Form["myParameter1"].ToString();                   //方法的参数名称1
            string m_Parameter2 = Request.Form["myParameter2"] == null ? "" : Request.Form["myParameter2"].ToString();                   //方法的参数名称2
            if (m_FunctionName == "ExcelStream")
            {
                //ExportFile("xls", "导出报表1.xls");
                string[] m_TagData = new string[] { "10月份", "报表类型:日报表", "汇总人:某某某", "审批人:某某某" };
                string m_HtmlData = StatisticalReportHelper.CreateExportHtmlTable(mFileRootPath +
                    "\\ReportHeaderTemplate\\report_ClinkerYearlyProcessEnergyConsumption.xml", myDataTable, m_TagData);
                StatisticalReportHelper.ExportExcelFile("xls", "导出报表1.xls", m_HtmlData);
            }
        }

        /// <summary>
        /// 获得报表数据并转换为json
        /// </summary>
        /// <returns>column的json字符串</returns>
        [WebMethod]
        public static string GetReportData(string organizationId, string datetime)
        {
            //myDataTable = ClinkerMonthlyPeakerValleyFlatElectricityConsumption.TableQuery("df863854-89ae-46e6-80e8-96f6db6471b4", "2014-10");
            myDataTable = ClinkerMonthlyPeakerValleyFlatElectricityConsumption.TableQuery(organizationId, datetime);
            string m_UserInfoJson = StatisticalReportHelper.ReadReportHeaderFile(mFileRootPath +
                "\\ReportHeaderTemplate\\report_ClinkerYearlyProcessEnergyConsumption.xml", myDataTable);
            return m_UserInfoJson;
        }

        /// <summary>
        /// 获得报表数据并转换为json
        /// </summary>
        /// <returns>column的json字符串</returns>
        [WebMethod]
        public static string PrintFile()
        {
            string[] m_TagData = new string[] { "10月份", "报表类型:日报表", "汇总人:某某某", "审批人:某某某" };
            string m_HtmlData = StatisticalReportHelper.CreatePrintHtmlTable(mFileRootPath +
                "\\ReportHeaderTemplate\\report_ClinkerYearlyProcessEnergyConsumption.xml", myDataTable, m_TagData);
            return m_HtmlData;
        }
    }
}