using StatisticalReport.Service.BasicDataSummaryReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalReport.Web.UI_BasicDataSummaryReport
{
    public partial class DailyBasicMaterialWeight : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                         //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "DailyBasicMaterialWeight.aspx";                                     //向web用户控件传递当前调用的页面名称
                //this.OrganisationTree_ProductionLine.OrganizationTypeItems.Add("水泥磨");
                //this.OrganisationTree_ProductionLine.OrganizationTypeItems.Add("熟料");
                this.OrganisationTree_ProductionLine.LeveDepth = 5;
            }
        }

        [WebMethod]
        public static string GetMaterialWeightDailyReport(string organizationId, DateTime datetime)
        {
            //List<string> oganizationIds = WebStyleBaseForEnergy.webStyleBase.GetDataValidIdGroup("ProductionOrganization");
            //IList<string> levelCodes = WebUserControls.Service.OrganizationSelector.OrganisationTree.GetOrganisationLevelCodeById(oganizationIds);
            DataTable dt = DailyBasicMaterialWeightService.GetMaterialWeightByOrganiztionIds(organizationId, datetime);
            return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(dt);
        }
    }
}