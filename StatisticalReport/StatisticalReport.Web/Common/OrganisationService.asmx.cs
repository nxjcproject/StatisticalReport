using EasyUIJsonParser;
using StatisticalReport.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace NXJC.UI.Web.Common
{
    /// <summary>
    /// OrganisationService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class OrganisationService : System.Web.Services.WebService
    {
        [WebMethod]
        public string GetTree()
        {
            DataTable dt = OrganizationService.GetOrganizationTree();
            return TreeJsonParser.DataTableToJsonByLevelCode(dt, "LevelCode", "Name", "OrganizationID");
        }
    }
}
