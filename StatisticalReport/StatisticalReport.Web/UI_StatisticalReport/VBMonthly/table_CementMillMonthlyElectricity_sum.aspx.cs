using StatisticalReport.Service.StatisticalReportServices;
using StatisticalReport.Service.VBReport.Monthly;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalReport.Web.UI_StatisticalReport.VBMonthly
{
    public partial class table_CementMillMonthlyElectricity_sum : WebStyleBaseForEnergy.webStyleBase
    {
        private const string REPORT_TEMPLATE_PATH = "\\VBReportHeaderTemplate\\table_CementMillMonthlyElectricity_sum.xml";
        private static DataTable myDataTable;

        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();

            ////////////////////调试用,自定义的数据授权
#if DEBUG
            List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_qtx_efc" };
            AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
            this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                         //向web用户控件传递数据授权参数
            this.OrganisationTree_ProductionLine.PageName = "table_CementMillMonthlyElectricity_sum.aspx";                                     //向web用户控件传递当前调用的页面名称
            this.OrganisationTree_ProductionLine.OrganizationTypeItems.Add("水泥磨");  

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
                    REPORT_TEMPLATE_PATH, myDataTable, m_TagData);
                StatisticalReportHelper.ExportExcelFile("xls", "水泥（分品种）粉磨用电量统计月报表.xls", m_HtmlData);
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
            myDataTable = CementMillMonthlyElectricity_sum.TableQuery(organizationId, datetime);
            string m_UserInfoJson = StatisticalReportHelper.ReadReportHeaderFile(mFileRootPath +
                REPORT_TEMPLATE_PATH, myDataTable);
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
                REPORT_TEMPLATE_PATH, myDataTable, m_TagData);
            return m_HtmlData;
        }
    }
}