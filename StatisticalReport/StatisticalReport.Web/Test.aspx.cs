using StatisticalReport.Service.StatisticalReportServices.Daily;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalReport.Web
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            DataTable table = EnergyConsumption_TargetCompletion.TableQuery("zc_nxjc_qtx_efc", "2014-12-28");
            GridView1.DataSource = table;
            GridView1.DataBind();
        }
    }
}