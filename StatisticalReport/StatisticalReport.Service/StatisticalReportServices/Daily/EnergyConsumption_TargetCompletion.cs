using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Daily
{
    public class EnergyConsumption_TargetCompletion
    {
        private static string connectionString;
        private static TZHelper tzHelper;

        static EnergyConsumption_TargetCompletion()//静态构造函数
        {
            connectionString = ConnectionStringFactory.NXJCConnectionString;
            tzHelper = new TZHelper(connectionString);
        }
        public static DataTable TableQuery(string organizationID, string date)
        {
            DataTable resultTable = tzHelper.CreateTableStructure("report_ClinkerEnergyConsumption_TargetCompletion");
            DataTable organizationTable = GetOrganizationDate(organizationID);
            DataRow[] clinkerRows = organizationTable.Select("Type='熟料'");
            DataRow[] cementmillRows = organizationTable.Select("Type='水泥磨'");
            foreach (DataRow row in clinkerRows)//
            {
                string orgID=row["OrganizationID"].ToString();
                DataTable temp = ClinkerEnergyConsumption_TargetCompletion.TableQuery(orgID, date);
                
                resultTable.Merge(temp);
            }
            foreach (DataRow row in cementmillRows)
            {
                string orgID = row["OrganizationID"].ToString();
                DataTable temp = CementMilEnergyConsumption_TargetCompletion.TableQuery(orgID, date);
                resultTable.Merge(temp);
            }
            resultTable = ReportHelper.MyTotalOn(resultTable, "Name", "Monthly_Target,Today_Completion,Monthly_Accumulative,Monthly_Gap,Yearly_Target,Yearly_Accumulative,Yearly_Gap");
            return resultTable;
        }

        /// <summary>
        /// 根据OrganizationID获取产线所有熟料和水泥磨产线
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        private static DataTable GetOrganizationDate(string organizationId)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = "SELECT A.OrganizationID,A.LevelCode,A.Type FROM [dbo].[system_Organization] AS A WHERE ([Type]='熟料' OR [Type]='水泥磨') AND [LevelCode] LIKE (SELECT [LevelCode] FROM [system_Organization] WHERE [OrganizationID]=@organizationID)+'%'";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("organizationID", organizationId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds, "table");
            }
            return ds.Tables["table"];
        }
    }
}
