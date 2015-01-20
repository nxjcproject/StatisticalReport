﻿using StatisticalReport.Service.ComprehensiveReport.DispatchDailyReport;
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
            int denominatorPlan = Int16.Parse(DateTime.Now.AddMonths(1).AddDays(-(DateTime.Now.Day)).ToString("dd"));
            int denominatorComplet = DateTime.Now.Day;
            DispatchDailyReportService.GetCompanyTargetCompletion();
            DispatchDailyReportService.GetPlanAndTargetCompletionByCompanyName("青铜峡水泥");
            DispatchDailyReportService.GetDailyGapPlanAndTargetCompletion("青铜峡水泥");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            DataTable table = EnergyConsumption_TargetCompletion.TableQuery("zc_nxjc_qtx_efc", "2014-12-28");
            GridView1.DataSource = table;
            GridView1.DataBind();
        }
    }
}