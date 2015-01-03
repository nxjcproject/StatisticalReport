using StatisticalReport.Service.StatisticalReportServices;
using StatisticalReport.Service.StatisticalReportServices.Yearly;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalReport.Web.UI_StatisticalReport.Yearly
{
    public partial class report_ClinkerYearlyPerUnitDistributionEnergyConsumption : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ////////////////////调试用,自定义的数据授权
#if DEBUG
            List<string> m_DataValidIdItems = new List<string>() { "C41B1F47-A48A-495F-A890-0AABB2F3BFF7                            ", "zc_nxjc_qtx_efc" };
            AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
            this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
            this.OrganisationTree_ProductionLine.PageName = "report_ClinkerYearlyPerUnitDistributionEnergyConsumption.aspx";                                     //向web用户控件传递当前调用的页面名称
            this.OrganisationTree_ProductionLine.OrganizationTypeItems.Add("熟料");  
        }

        /// <summary>
        /// 查询已保存的
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [WebMethod]
        public static string ReadClinkerYearlyPerUnitDistributionEnergyConsumptionInfo(string organizationId, string year)
        {
            string[] m_ColumnText = new string[] { "ID","KeyID","Type",
                "月份", 
                "熟料用电量(kwh)", 
                "入窑煤粉量(t)", 
                "煤粉低位发热量(kJ/kg)",
                "点火用油",
                "余热发电量(kwh)",
                "余热自用电(kwh)", 
                "余热供电量(kwh)", 
                "熟料产量(t)",
                "熟料强度(Mpa)", 
                "熟料强度修正系数",
                "熟料综合电耗(kwh/t)", 
                "可比熟料综合电耗(kwh/t)", 
                "煤耗(kgce/t)", 
                "油耗 (kgce/t)", 
                "余热供电折标(kgce/t)",
                "熟料综合煤耗(kgce/t)", 
                "可比熟料综合煤耗(kgce/t)",
                "熟料综合能耗(kgce/t)", 
                "可比熟料综合能耗(kgce/t)" 
            };

            int[] m_ColumnWidth = new int[] { 0, 0, 0, 60, 100, 90, 135, 65, 110, 110, 110, 80, 90, 110, 120, 145, 90, 90, 140, 140, 150, 140, 150 };
            string[] m_FormatString = new string[] { "","","","",
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                //"熟料用电量(kwh)", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                //"入窑煤粉量(t)", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                //"煤粉低位发热量(kJ/kg)",
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"2\"}",                //"点火用油",
                "", //"\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                //"余热发电量(kwh)",
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                //"余热自用电(kwh)", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                //"余热供电量(kwh)", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                //"熟料产量(t)",
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"1\"}",                //"熟料强度(Mpa)", 
                "", //"\"type\":\"numberbox\", \"options\":{\"precision\":\"4\"}",                //"熟料强度修正系数",
                "",                                                                         //"熟料综合电耗(kwh/t)", 
                "",                                                                         //"可比熟料综合电耗(kwh/t)", 
                "",                                                                         //"煤耗(kgce/t)", 
                "",                                                                         //"油耗 (kgce/t)", 
                "",                                                                         //"余热供电折标(kgce/t)",
                "",                                                                         //"熟料综合煤耗(kgce/t)", 
                "",                                                                         //"可比熟料综合煤耗(kgce/t)",
                "",                                                                         //"熟料综合能耗(kgce/t)", 
                ""                                                                          //"可比熟料综合能耗(kgce/t)" 
            };

            DataTable m_ClinkerYearlyPerUnitDistributionEnergyConsumptionInfo = ClinkerYearlyPerUnitDistributionEnergyConsumption.Read(organizationId, year);
            string m_Rows = EasyUIJsonParser.DataGridJsonParser.GetDataRowJson(m_ClinkerYearlyPerUnitDistributionEnergyConsumptionInfo);
            StringBuilder m_Columns = new StringBuilder();
            if (m_Rows == "")
            {
                m_Rows = "\"rows\":[],\"total\":0";
            }
            m_Columns.Append("\"columns\":[");
            for (int i = 0; i < m_ClinkerYearlyPerUnitDistributionEnergyConsumptionInfo.Columns.Count; i++)
            {
                m_Columns.Append("{");
                if (m_ColumnWidth[i] == 0)
                    m_Columns.Append("\"hidden\":true");
                else
                    m_Columns.Append("\"width\":\"" + m_ColumnWidth[i] + "\"");
                m_Columns.Append(", \"title\":\"" + m_ColumnText[i] + "\"");
                m_Columns.Append(", \"field\":\"" + m_ClinkerYearlyPerUnitDistributionEnergyConsumptionInfo.Columns[i].ColumnName.ToString() + "\"");
                if (m_FormatString[i] != "")
                {
                    m_Columns.Append(", \"editor\":{" + m_FormatString[i] + "}");
                }

                m_Columns.Append("}");
                if (i < m_ClinkerYearlyPerUnitDistributionEnergyConsumptionInfo.Columns.Count - 1)
                {
                    m_Columns.Append(",");
                }
            }
            m_Columns.Append("]");

            return "{" + m_Rows + "," + m_Columns + "}";
        }

        /// <summary>
        /// 查询原始数据
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetClinkerYearlyPerUnitDistributionEnergyConsumptionInfo(string organizationId, string year)
        {
            string[] m_ColumnText = new string[] { "ID","KeyID","Type",
                "月份", 
                "熟料用电量(kwh)", 
                "入窑煤粉量(t)", 
                "煤粉低位发热量(kJ/kg)",
                "点火用油",
                "余热发电量(kwh)",
                "余热自用电(kwh)", 
                "余热供电量(kwh)", 
                "熟料产量(t)",
                "熟料强度(Mpa)", 
                "熟料强度修正系数",
                "熟料综合电耗(kwh/t)", 
                "可比熟料综合电耗(kwh/t)", 
                "煤耗(kgce/t)", 
                "油耗 (kgce/t)", 
                "余热供电折标(kgce/t)",
                "熟料综合煤耗(kgce/t)", 
                "可比熟料综合煤耗(kgce/t)",
                "熟料综合能耗(kgce/t)", 
                "可比熟料综合能耗(kgce/t)" 
            };

            int[] m_ColumnWidth = new int[] { 0, 0, 0, 60, 100, 90, 135, 65, 110, 110, 110, 80, 90, 110, 120, 145, 90, 90, 140, 140, 150, 140, 150 };
            string[] m_FormatString = new string[] { "","","","",
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                //"熟料用电量(kwh)", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                //"入窑煤粉量(t)", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                //"煤粉低位发热量(kJ/kg)",
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"2\"}",                //"点火用油",
                "", //"\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                //"余热发电量(kwh)",
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                //"余热自用电(kwh)", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                //"余热供电量(kwh)", 
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                //"熟料产量(t)",
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"1\"}",                //"熟料强度(Mpa)", 
                "", //"\"type\":\"numberbox\", \"options\":{\"precision\":\"4\"}",                //"熟料强度修正系数",
                "",                                                                         //"熟料综合电耗(kwh/t)", 
                "",                                                                         //"可比熟料综合电耗(kwh/t)", 
                "",                                                                         //"煤耗(kgce/t)", 
                "",                                                                         //"油耗 (kgce/t)", 
                "",                                                                         //"余热供电折标(kgce/t)",
                "",                                                                         //"熟料综合煤耗(kgce/t)", 
                "",                                                                         //"可比熟料综合煤耗(kgce/t)",
                "",                                                                         //"熟料综合能耗(kgce/t)", 
                ""                                                                          //"可比熟料综合能耗(kgce/t)" 
            };

            DataTable m_ClinkerYearlyPerUnitDistributionEnergyConsumptionInfo = ClinkerYearlyPerUnitDistributionEnergyConsumption.TableQuery(organizationId, year);
            string m_Rows = EasyUIJsonParser.DataGridJsonParser.GetDataRowJson(m_ClinkerYearlyPerUnitDistributionEnergyConsumptionInfo);
            StringBuilder m_Columns = new StringBuilder();
            if (m_Rows == "")
            {
                m_Rows = "\"rows\":[],\"total\":0";
            }
            m_Columns.Append("\"columns\":[");
            for (int i = 0; i < m_ClinkerYearlyPerUnitDistributionEnergyConsumptionInfo.Columns.Count; i++)
            {
                m_Columns.Append("{");
                if(m_ColumnWidth[i] == 0)
                    m_Columns.Append("\"hidden\":true");
                else
                    m_Columns.Append("\"width\":\"" + m_ColumnWidth[i] + "\"");
                m_Columns.Append(", \"title\":\"" + m_ColumnText[i] + "\"");
                m_Columns.Append(", \"field\":\"" + m_ClinkerYearlyPerUnitDistributionEnergyConsumptionInfo.Columns[i].ColumnName.ToString() + "\"");
                if (m_FormatString[i] != "")
                {
                    m_Columns.Append(", \"editor\":{" + m_FormatString[i] + "}");
                }

                m_Columns.Append("}");
                if (i < m_ClinkerYearlyPerUnitDistributionEnergyConsumptionInfo.Columns.Count - 1)
                {
                    m_Columns.Append(",");
                }
            }
            m_Columns.Append("]");

            return "{" + m_Rows + "," + m_Columns + "}";
        }

        [WebMethod]
        public static string CalculateClinkerYearlyPerUnitDistributionEnergyConsumption(string organizationId, string year, string gridData)
        {
            DataTable m_DataGridDataStruct = ClinkerYearlyPerUnitDistributionEnergyConsumption.CreateTableStructure("report_ClinkerYearlyPerUnitDistributionEnergyConsumption");
            string[] m_DataGridDataGroup = EasyUIJsonParser.Utility.JsonPickArray(gridData, "rows");
            DataTable m_DataGridData = EasyUIJsonParser.DataGridJsonParser.JsonToDataTable(m_DataGridDataGroup, m_DataGridDataStruct);

            DataTable m_ClinkerYearlyPerUnitDistributionEnergyConsumptionInfo = ClinkerYearlyPerUnitDistributionEnergyConsumption.Calculate(organizationId, year, m_DataGridData);
            return "{" + EasyUIJsonParser.DataGridJsonParser.GetDataRowJson(m_ClinkerYearlyPerUnitDistributionEnergyConsumptionInfo) + "}";
        }

        [WebMethod]
        public static string SaveClinkerYearlyPerUnitDistributionEnergyConsumption(string organizationId, string year, string gridData)
        {
            DataTable m_DataGridDataStruct = ClinkerYearlyPerUnitDistributionEnergyConsumption.CreateTableStructure("report_ClinkerYearlyPerUnitDistributionEnergyConsumption");
            string[] m_DataGridDataGroup = EasyUIJsonParser.Utility.JsonPickArray(gridData, "rows");
            DataTable m_DataGridData = EasyUIJsonParser.DataGridJsonParser.JsonToDataTable(m_DataGridDataGroup, m_DataGridDataStruct);

            string result = ClinkerYearlyPerUnitDistributionEnergyConsumption.Save(organizationId, year, m_DataGridData);
            return "{\"result\":\"" + result + "\"}";
        }
    }
}