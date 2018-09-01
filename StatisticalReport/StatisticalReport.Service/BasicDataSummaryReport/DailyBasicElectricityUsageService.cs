using EnergyConsumption;
using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.BasicDataSummaryReport
{
    public class DailyBasicElectricityUsageService
    {
        /// <summary>
        /// 查询所有产线的用电量
        /// </summary>
        /// <param name="organizationIds">组织机构ID数组（通常为授权的组织机构ID数组）</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static DataTable GetDailyBasicElectricityUsageByOrganiztionIds(string organizationId, string startDate, string endDate)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string queryString = @"Select C.Name AS Name, B.LevelCode as LevelCode
                                         ,(case when B.LevelType='ProductionLine' then C.Name else B.Name end) AS VariableName
                                         ,F.VariableId,B.LevelCode as FormulaLevelCode
                                         ,F.FirstB AS FirstB, F.SecondB AS SecondB, F.ThirdB AS ThirdB,F.PeakB AS PeakB,F.ValleyB AS ValleyB,F.FlatB AS FlatB
                                         ,'' AS A,'' AS B,'' AS C,'' AS D,F.TotalPeakValleyFlatB AS TotalPeakValleyFlatB
                                        from tz_Formula A, formula_FormulaDetail B
                                        left join system_Organization C on C.OrganizationID = '{2}',
                                            (Select E.OrganizationID, E.VariableId
                                                   ,SUM(E.FirstB) AS FirstB,SUM(E.SecondB) AS SecondB,SUM(E.ThirdB) AS ThirdB
                                                   ,SUM(E.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB,SUM(E.PeakB) AS PeakB,SUM(E.ValleyB) AS ValleyB,SUM(E.FlatB) AS FlatB
	                                            from tz_Balance D, balance_Energy E 
		                                        where D.TimeStamp >= '{0}'
		                                        and D.TimeStamp <= '{1}'
		                                        and D.StaticsCycle = 'day'
		                                        and D.BalanceId = E.KeyId
		                                        and E.ValueType = 'ElectricityQuantity'
		                                        and E.OrganizationID = '{2}'
		                                        group by E.OrganizationID, E.VariableId) F
                                        where A.OrganizationID = '{2}'
                                        and A.Enable = 1
                                        and A.State = 0
                                        and A.KeyID = (SELECT TOP 1 KeyID
                                                            FROM tz_Formula
                                                            WHERE OrganizationID= '{2}' AND CreatedDate <= '{1}' AND ENABLE = 1 AND State = 0 
                                                            ORDER BY CreatedDate DESC)--根据结束时间选择最近版本
                                        and A.KeyID = B.KeyID
                                        and B.Visible = 1
                                        and A.OrganizationID = F.OrganizationID
                                        and B.VariableId + '_ElectricityQuantity' = F.VariableId
                                        order by B.LevelCode";
            DataTable resultTable = dataFactory.Query(string.Format(queryString, startDate, endDate, organizationId));
            DataColumn stateColumn = new DataColumn("state", typeof(string));
            resultTable.Columns.Add(stateColumn);
            int i = 0;
            foreach (DataRow dr in resultTable.Rows)
            {
                i++;
                if (dr["VariableName"] is DBNull || dr["VariableName"].ToString().Trim() == "")
                {
                    dr["VariableName"] = dr["Name"].ToString().Trim();
                }
                if (dr["FormulaLevelCode"].ToString().Length > 3)
                {
                    dr["LevelCode"] = dr["LevelCode"] + dr["FormulaLevelCode"].ToString().Substring(3);
                }
                bool haveChidren = HaveChildren(dr["FormulaLevelCode"].ToString().Trim(), resultTable);
                if (dr["FormulaLevelCode"].ToString().Trim().Contains('P') && dr["FormulaLevelCode"].ToString().Trim().Length == 5
                    && haveChidren)
                {
                    dr["state"] = "closed";
                }
                else if (dr["FormulaLevelCode"].ToString().Trim().Contains('G') &&
                    dr["FormulaLevelCode"].ToString().Trim().Length == 3 && haveChidren)
                {
                    dr["state"] = "closed";
                }
                else
                {
                    dr["state"] = "open";
                }
            }
            return resultTable;
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
            DataRow[] rows = resultTable.Select("FormulaLevelCode Like '" + parent + "%' and Len(FormulaLevelCode)>" + myLength);
            if (rows.Count() > 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 获取排班记录
        /// </summary>
        /// <param name="organizationId">组织机构ID</param>
        /// <param name="date">时间，yyyy-MM</param>
        /// <returns></returns>
        public static DataTable GetShiftsSchedulingLogMonthly(string organizationId, string startDate, string endDate)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            string[] organizationIdNew = organizationId.Split('_');
            StringBuilder st = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                st.Append(organizationIdNew[i]);
                st.Append("_");
            }
            st.Append(organizationIdNew[3]);
            string organizationIdFinal = st.ToString();
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string sql = @" SELECT [BalanceId],[TimeStamp],[FirstWorkingTeam],[SecondWorkingTeam],[ThirdWorkingTeam]
                              FROM [tz_Balance]
                              WHERE TimeStamp>=@startDate AND TimeStamp<=@endDate
		                            and StaticsCycle = 'day' AND
		                            [OrganizationID] = @organizationId
                              ORDER BY [TimeStamp]";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationIdFinal),
                new SqlParameter("startDate", startDate),
                new SqlParameter("endDate",endDate)
            };

            return dataFactory.Query(sql, parameters); ;
        }
        private static DataTable GetProcessValueTableByOrganizationId(string organizationId, string start, string end)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string sql = @" SELECT
                                    B.[OrganizationID],
		                            B.[VariableId],
		                            SUM(CASE WHEN [A].[FirstWorkingTeam] = 'A班' THEN [B].[FirstB] WHEN [A].[SecondWorkingTeam] = 'A班' THEN [B].[SecondB] WHEN [A].[ThirdWorkingTeam] = 'A班' THEN [B].[ThirdB] ELSE 0 END) AS A班,
		                            SUM(CASE WHEN [A].[FirstWorkingTeam] = 'B班' THEN [B].[FirstB] WHEN [A].[SecondWorkingTeam] = 'B班' THEN [B].[SecondB] WHEN [A].[ThirdWorkingTeam] = 'B班' THEN [B].[ThirdB] ELSE 0 END) AS B班,
		                            SUM(CASE WHEN [A].[FirstWorkingTeam] = 'C班' THEN [B].[FirstB] WHEN [A].[SecondWorkingTeam] = 'C班' THEN [B].[SecondB] WHEN [A].[ThirdWorkingTeam] = 'C班' THEN [B].[ThirdB] ELSE 0 END) AS C班,
		                            SUM(CASE WHEN [A].[FirstWorkingTeam] = 'D班' THEN [B].[FirstB] WHEN [A].[SecondWorkingTeam] = 'D班' THEN [B].[SecondB] WHEN [A].[ThirdWorkingTeam] = 'D班' THEN [B].[ThirdB] ELSE 0 END) AS D班,
		                            SUM([B].[TotalPeakValleyFlatB]) AS 合计
                            FROM	tz_Balance AS A INNER JOIN
		                            balance_Energy AS B ON A.BalanceId = B.KeyId
                            WHERE
		                            ([B].[OrganizationID] LIKE @organizationId + '%') AND 
		                            ([A].[StaticsCycle] = 'day') AND 
		                            ([A].[TimeStamp] >= @startTime) AND
		                            ([A].[TimeStamp] <= @endTime)
                            GROUP BY [B].[OrganizationID], [B].[VariableId]";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationId),
                new SqlParameter("startTime", start),
                new SqlParameter("endTime", end)
            };
            DataTable table = dataFactory.Query(sql, parameters);
            return table;
        }

        public static DataTable GetTeamJobEvaluationMonthly(string organization, string startDate, string endDate)
        {
            DataTable table = GetProcessValueTableByOrganizationId(organization, startDate, endDate);
            DataTable templateTable = GetDailyBasicElectricityUsageByOrganiztionIds(organization, startDate, endDate);
            int i = 0;
            foreach (DataRow dr in table.Rows)
            {
                string variableidcontrost = dr["VariableId"].ToString();
                i++;
                foreach (DataRow drNew in templateTable.Rows)
                {
                    string variableid = drNew["VariableId"].ToString();
                    if (variableid == variableidcontrost)
                    {
                        drNew["A"] = decimal.Parse(dr["A班"].ToString()).ToString("0.00");
                        drNew["B"] = decimal.Parse(dr["B班"].ToString()).ToString("0.00");
                        drNew["C"] = decimal.Parse(dr["C班"].ToString()).ToString("0.00");
                        drNew["D"] = decimal.Parse(dr["D班"].ToString()).ToString("0.00");
                    }

                }
            }
            return templateTable;
        }
    }
}
