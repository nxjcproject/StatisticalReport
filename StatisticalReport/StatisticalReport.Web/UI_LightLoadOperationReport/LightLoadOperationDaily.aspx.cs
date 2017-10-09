using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Services;
using StatisticalReport.Service.StatisticalReportServices;
using System.Text;

namespace StatisticalReport.Web.UI_LightLoadOperationReport
{
    public partial class LightLoadOperationDaily : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
#if DEBUG
            ////////////////////调试用,自定义的数据授权
            List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_qtx", "zc_nxjc_byc_byf", "zc_nxjc_ychc", "zc_nxjc_tsc", "zc_nxjc_szsc", "zc_nxjc_klqc", "zc_nxjc_znc" };
            AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
            this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                            //向web用户控件传递数据授权参数
            this.OrganisationTree_ProductionLine.PageName = "LightLoadOperationDaily.aspx";                                                //向web用户控件传递当前调用的页面名称
            this.OrganisationTree_ProductionLine.LeveDepth = 5;

            ///以下是接收js脚本中post过来的参数
            string m_FunctionName = Request.Form["myFunctionName"] == null ? "" : Request.Form["myFunctionName"].ToString();             //方法名称,调用后台不同的方法
            string m_Parameter1 = Request.Form["myParameter1"] == null ? "" : Request.Form["myParameter1"].ToString();                   //方法的参数名称1
            string m_Parameter2 = Request.Form["myParameter2"] == null ? "" : Request.Form["myParameter2"].ToString();                   //方法的参数名称2
            if (m_FunctionName == "ExcelStream")
            {
                //ExportFile("xls", "导出报表1.xls");
                string m_ExportTable = m_Parameter1.Replace("&lt;", "<");
                m_ExportTable = m_ExportTable.Replace("&gt;", ">");

                //UTF8Encoding utf8 = new UTF8Encoding();
                //Encoding GB2312 = Encoding.GetEncoding("GB2312");
                //ASCIIEncoding ASCII = new ASCIIEncoding();
                //Byte[] encodedBytes = utf8.GetBytes(m_ExportTable);
                //Byte[] converBytes = Encoding.Convert(utf8, ASCII, encodedBytes);
                //String decodedString = ASCII.GetString(encodedBytes);
                //StatisticalReportHelper.ExportExcelFile("xls", m_Parameter2 + "设备低负荷运转统计报表.xls", decodedString);
                StatisticalReportHelper.ExportExcelFile("xls", m_Parameter2 + "设备低负荷运转统计报表.xls", m_ExportTable);
            }
        }
        [WebMethod]
        public static string GetVariableInfo(string myOrganizationId)
        {
            DataTable m_VariableInfoTable = StatisticalReport.Service.LightLoadOperationReport.LightLoadOperationDaily.GetVariableInfo(myOrganizationId);
            if (m_VariableInfoTable != null)
            {
                string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_VariableInfoTable,new string[]{"id","text"});
                return json;
            }
            else
            {
                return "{\"rows\":[],\"total\":0}";
            }
        }
        [WebMethod]
        public static string GetLightLoadData(string myOrganizationId, string myVariableId, string myStartTime, string myEndTime)
        {
            DataTable m_VariableInfoTable = StatisticalReport.Service.LightLoadOperationReport.LightLoadOperationDaily.GetVariableInfoById(myOrganizationId, myVariableId);
            DataTable m_LightLoadDataTable = StatisticalReport.Service.LightLoadOperationReport.LightLoadOperationDaily.GetLightLoadData(m_VariableInfoTable, myStartTime, myEndTime);
            string json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(m_LightLoadDataTable, "LevelCode");
            return json;
        }
    }
}