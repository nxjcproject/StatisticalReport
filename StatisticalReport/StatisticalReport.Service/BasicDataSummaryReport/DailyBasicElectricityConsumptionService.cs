using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.BasicDataSummaryReport
{
    public class DailyBasicElectricityConsumptionService
    {

        public static DataTable GetElectricityConsumptionData(string organizationId,string startTime,string endTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select A.OrganizationID,B.Name,B.LevelCode,B.VariableID,B.LevelType,C.ValueFormula
                                from (SELECT LevelCode FROM system_Organization WHERE OrganizationID=@organizationId) M inner join system_Organization N
	                                on N.LevelCode Like M.LevelCode+'%' inner join tz_Formula A 
	                                on A.OrganizationID=N.OrganizationID inner join formula_FormulaDetail B
	                                on A.KeyID=B.KeyID and A.Type=2 left join balance_Energy_Template C 
	                                on B.VariableId+'_ElectricityConsumption'=C.VariableId
                                order by OrganizationID,LevelCode";
            SqlParameter parameter = new SqlParameter("organizationId", organizationId);
            DataTable frameTable = dataFactory.Query(mySql,parameter);
            string preFormula = "";
            foreach(DataRow dr in frameTable.Rows)
            {
                if (dr["ValueFormula"] is DBNull)
                {
                    if(dr["VariableId"] is DBNull || dr["VariableId"].ToString().Trim()=="")
                    {
                        continue;
                    }
                    preFormula=DealWithFormula(preFormula,dr["VariableId"].ToString().Trim());
                    dr["ValueFormula"] = preFormula;
                }
                else
                {
                    preFormula = dr["ValueFormula"].ToString().Trim();
                }
            }
            string dataSql = @"select B.OrganizationID,B.VariableId,SUM(B.FirstB) as FirstB,SUM(B.SecondB) as SecondB,SUM(B.ThirdB) as ThirdB,SUM(B.TotalPeakValleyFlatB) as TotalPeakValleyFlatB
                                from tz_Balance A,balance_Energy B
                                where A.BalanceId=B.KeyId
                                and A.TimeStamp>=@startTime and A.TimeStamp<=@endTime
                                and B.OrganizationID=@organizationId
                                group by B.OrganizationID,B.VariableId";
            SqlParameter[] parameters = { new SqlParameter("organizationId", organizationId), new SqlParameter("startTime", startTime), new SqlParameter("endTime", endTime) };
            DataTable sourceData = dataFactory.Query(dataSql,parameters);
            string[] calColumns = new string[] { "FirstB", "SecondB", "ThirdB", "TotalPeakValleyFlatB" };
            DataTable result=EnergyConsumption.EnergyConsumptionCalculate.CalculateByOrganizationId(sourceData, frameTable, "ValueFormula", calColumns);
            DataColumn stateColumn=new DataColumn("state",typeof(string));
            result.Columns.Add(stateColumn);
            foreach (DataRow dr in result.Rows)
            {
                bool haveChidren = HaveChildren(dr["LevelCode"].ToString().Trim(), result);
                if (dr["LevelCode"].ToString().Trim().Length == 7 && haveChidren)
                {
                    dr["state"] = "closed";
                }
                else
                {
                    dr["state"] = "open";
                }
            }
            return result;
        }
        /// <summary>
        /// 处理公式
        /// </summary>
        /// <param name="preFormula"></param>
        /// <param name="variableId"></param>
        /// <returns></returns>
        private static string DealWithFormula(string preFormula,string variableId)
        {
            if (preFormula.Contains('_'))
            {
                int num = preFormula.IndexOf('_');
                string subStr = preFormula.Substring(1, num - 1);
                return preFormula.Replace(subStr, variableId);
            }
            else
                return preFormula;
        }
        /// <summary>
        /// 判断是否有孩子结点
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="resultTable"></param>
        /// <returns></returns>
        private static bool HaveChildren(string parent, DataTable resultTable)
        {
            int myLength = parent.Trim().Length;
            DataRow[] rows = resultTable.Select("LevelCode Like '" + parent + "%' and Len(LevelCode)>" + myLength);
            if (rows.Count() > 0)
                return true;
            else
                return false;
        }
    }
}
