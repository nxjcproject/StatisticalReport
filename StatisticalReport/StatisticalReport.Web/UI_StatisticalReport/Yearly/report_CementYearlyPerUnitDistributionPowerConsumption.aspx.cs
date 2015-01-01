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
    public partial class report_CementYearlyPerUnitDistributionPowerConsumption : WebStyleBaseForEnergy.webStyleBase
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
            this.OrganisationTree_ProductionLine.PageName = "EnergyConsumptionPlan.aspx";                                     //向web用户控件传递当前调用的页面名称
        }

        [WebMethod]
        public static string GetCementYearlyPerUnitDistributionPowerConsumptionInfo(string organizationId, string clinkerOrganizationId, string year)
        {
            string[] m_ColumnText = new string[] { "ID","KeyID","Type",
                "月份", 
                "用电量(kwh)", 
                "熟料综合电耗(kwh/t)", 
                "可比熟料综合煤耗(kgce/t)",
                "水泥产量(t)",
                "水泥用熟料量(t)",
                "熟料平均配比(%)", 
                "各品种水泥平均强度(Mpa)", 
                "水泥强度修正系数",
                "分步电耗(kwh/t)", 
                "水泥综合电耗(kwh/t)",
                "可比水泥综合电耗(kwh/t)", 
                "可比水泥综合煤耗(kgce/t)", 
                "可比水泥综合能耗(kgce/t)"
            };

            //                                1  2  3  4   5   6    7    8    9   10   11   12  13  14   15   16   17
            int[] m_ColumnWidth = new int[] { 0, 0, 0, 60, 90, 130, 155, 95, 110, 110, 150, 120, 110, 130, 155, 165, 165 };
            string[] m_FormatString = new string[] { "","","","",                             // "月份",                          vDate
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                  // "用电量(kwh)",                   ElectricityConsumption
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"1\"}",                  // "熟料综合电耗(kwh/t)",           Clinker_ComprehensiveElectricityConsumption
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"1\"}",                  // "可比熟料综合煤耗(kgce/t)",      Clinker_ComparableComprehensiveCoalDustConsumption
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                  // "水泥产量(t)",                   CementProductionSum
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                  // "水泥用熟料量(t)",               ClinkerConsumptionSum
                "",                                                                           // "熟料平均配比(%)",               ClinkerMatching
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"0\"}",                  // "各品种水泥平均强度(Mpa)",       CementIntensity
                "", //"\"type\":\"numberbox\", \"options\":{\"precision\":\"1\"}",                  // "水泥强度修正系数",              CementIntensityCorrectionFactor
                "\"type\":\"numberbox\", \"options\":{\"precision\":\"4\"}",                  // "分步电耗(kwh/t)",               DistributionPowerConsumption
                "",                                                                           // "水泥综合电耗(kwh/t)",           Cement_ComprehensiveElectricityConsumption
                "",                                                                           // "可比水泥综合电耗(kwh/t)",       Cement_ComparableComprehensiveElectricityConsumption
                "",                                                                           // "可比水泥综合煤耗(kgce/t)",      Cement_ComparableComprehensiveCoalDustConsumption
                ""                                                                            // "可比水泥综合能耗(kgce/t)"       Cement_ComparableComprehensiveEnergyConsumption
            };

            DataTable m_CementYearlyPerUnitDistributionPowerConsumptionInfo = CementYearlyPerUnitDistributionPowerConsumption.TableQuery(organizationId, clinkerOrganizationId, year);
            string m_Rows = EasyUIJsonParser.DataGridJsonParser.GetDataRowJson(m_CementYearlyPerUnitDistributionPowerConsumptionInfo);
            StringBuilder m_Columns = new StringBuilder();
            if (m_Rows == "")
            {
                m_Rows = "\"rows\":[],\"total\":0";
            }
            m_Columns.Append("\"columns\":[");
            for (int i = 0; i < m_CementYearlyPerUnitDistributionPowerConsumptionInfo.Columns.Count; i++)
            {
                m_Columns.Append("{");
                if(m_ColumnWidth[i] == 0)
                    m_Columns.Append("\"hidden\":true");
                else
                    m_Columns.Append("\"width\":\"" + m_ColumnWidth[i] + "\"");
                m_Columns.Append(", \"title\":\"" + m_ColumnText[i] + "\"");
                m_Columns.Append(", \"field\":\"" + m_CementYearlyPerUnitDistributionPowerConsumptionInfo.Columns[i].ColumnName.ToString() + "\"");
                if (m_FormatString[i] != "")
                {
                    m_Columns.Append(", \"editor\":{" + m_FormatString[i] + "}");
                }

                m_Columns.Append("}");
                if (i < m_CementYearlyPerUnitDistributionPowerConsumptionInfo.Columns.Count - 1)
                {
                    m_Columns.Append(",");
                }
            }
            m_Columns.Append("]");

            return "{" + m_Rows + "," + m_Columns + "}";
        }

        [WebMethod]
        public static string CalculateCementYearlyPerUnitDistributionPowerConsumption(string organizationId, string clinkerOrganizationId, string year, string gridData)
        {
            DataTable m_DataGridDataStruct = CementYearlyPerUnitDistributionPowerConsumption.CreateTableStructure("report_CementYearlyPerUnitDistributionPowerConsumption");
            string[] m_DataGridDataGroup = EasyUIJsonParser.Utility.JsonPickArray(gridData, "rows");
            DataTable m_DataGridData = EasyUIJsonParser.DataGridJsonParser.JsonToDataTable(m_DataGridDataGroup, m_DataGridDataStruct);

            DataTable m_CementYearlyPerUnitDistributionPowerConsumptionInfo = CementYearlyPerUnitDistributionPowerConsumption.Calculate(organizationId, clinkerOrganizationId, year, m_DataGridData);
            return "{" + EasyUIJsonParser.DataGridJsonParser.GetDataRowJson(m_CementYearlyPerUnitDistributionPowerConsumptionInfo) + "}";
        }

        [WebMethod]
        public static string SaveCementYearlyPerUnitDistributionPowerConsumption(string organizationId, string clinkerOrganizationId, string year, string gridData)
        {
            DataTable m_DataGridDataStruct = CementYearlyPerUnitDistributionPowerConsumption.CreateTableStructure("report_CementYearlyPerUnitDistributionPowerConsumption");
            string[] m_DataGridDataGroup = EasyUIJsonParser.Utility.JsonPickArray(gridData, "rows");
            DataTable m_DataGridData = EasyUIJsonParser.DataGridJsonParser.JsonToDataTable(m_DataGridDataGroup, m_DataGridDataStruct);

            string result = CementYearlyPerUnitDistributionPowerConsumption.Save(organizationId, clinkerOrganizationId, year, m_DataGridData);
            return "{\"result\":\"" + result + "\"}";
        }

        [WebMethod]
        public static string GetClinkerListWithCombotreeFormat(string organizationId)
        {
            DataTable dt = CementYearlyPerUnitDistributionPowerConsumption.GetClinkerTable(organizationId);

            return EasyUIJsonParser.TreeJsonParser.DataTableToJsonByLevelCodeWithIdColumn(dt, "LevelCode", "OrganizationID", "Name");
        }
    }
}