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
            //            string queryString = @"SELECT  SO.Name,SO.LevelCode,FIN.VariableName,FIN.FormulaLevelCode,SUM(FIN.FirstB) AS FirstB,SUM(FIN.SecondB) AS SecondB,SUM(FIN.ThirdB) AS ThirdB,SUM(FIN.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
            //                                    FROM system_Organization AS SO LEFT JOIN 
            //                                        (SELECT TB.TimeStamp,TB.StaticsCycle,BE.*,RST.VariableId AS FirstVariable,RST.FormulaLevelCode  FROM 
            //	                                                 (SELECT TF.OrganizationID AS TFOrgID ,FFD.VariableId,FFD.LevelCode AS FormulaLevelCode
            //	                                                 FROM tz_Formula AS TF,formula_FormulaDetail AS FFD 
            //	                                                 WHERE TF.KeyID=FFD.KeyID AND 
            //	                                                 -- FFD.LevelType='ProductionLine' AND
            //	                                                  TF.ENABLE='true') AS RST,
            //	                                    tz_Balance AS TB,balance_Energy AS BE
            //	                                    WHERE TB.BalanceId=BE.KeyId AND RST.TFOrgID=BE.OrganizationID AND
            //                                        RST.VariableId+'_ElectricityQuantity'=BE.VariableId AND
            //	                                    TB.TimeStamp>='{0}' AND TB.TimeStamp<='{1}' AND
            //										TB.StaticsCycle='day'
            //                                         ) AS FIN
            //                                    ON SO.OrganizationID=FIN.OrganizationID 
            //                                    INNER JOIN 
            //										 (
            //										 SELECT A.OrganizationID 
            //                                            FROM system_Organization AS A 
            //                                            WHERE A.LevelCode = (
            //                                                                    SELECT T.LevelCode FROM system_Organization AS T
            //						                                            WHERE T.OrganizationID='{2}'
            //						                                            )
            //										 ) AS O
            //									ON
            //									O.OrganizationID=SO.OrganizationID
            //                                    WHERE ISNULL(SO.Type,'')<>'余热发电' 
            //									GROUP BY VariableName,
            //									SO.Name,SO.LevelCode,
            //									FIN.VariableName,
            //                                    FIN.FormulaLevelCode
            //                          SO.Name,SO.LevelCode,FIN.VariableName,FIN.FormulaLevelCode,            ";
            string queryString = @"Select C.Name AS Name, B.LevelCode as LevelCode,(case when B.LevelType='ProductionLine' then C.Name else B.Name end) AS VariableName,F.VariableId,B.LevelCode as FormulaLevelCode,F.FirstB AS FirstB, F.SecondB AS SecondB, F.ThirdB AS ThirdB,F.PeakB AS PeakB,F.ValleyB AS ValleyB,F.FlatB AS FlatB,'' AS A,'' AS B,'' AS C,'' AS D,F.TotalPeakValleyFlatB AS TotalPeakValleyFlatB
                                        from tz_Formula A, formula_FormulaDetail B
                                        left join system_Organization C on C.OrganizationID = '{2}',
                                            (Select E.OrganizationID, E.VariableId,SUM(E.FirstB) AS FirstB,SUM(E.SecondB) AS SecondB,SUM(E.ThirdB) AS ThirdB,SUM(E.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB,SUM(E.PeakB) AS PeakB,SUM(E.ValleyB) AS ValleyB,SUM(E.FlatB) AS FlatB
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
                                        and A.KeyID = B.KeyID
                                        and B.Visible = 1
                                        and A.OrganizationID = F.OrganizationID
                                        and B.VariableId + '_ElectricityQuantity' = F.VariableId
                                        order by B.LevelCode";

            //#if DEBUG
            //            DataTable resultTable = dataFactory.Query(string.Format(queryString, organizationId, "2015-02-09")); 
            //#else
            DataTable resultTable = dataFactory.Query(string.Format(queryString, startDate, endDate, organizationId));
            //#endif
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
		                            --LEFT([A].[TimeStamp],7) AS [TimeStamp],
                                    [B].[OrganizationID],
		                            [B].[VariableId],
		                            SUM(CASE WHEN [A].[FirstWorkingTeam] = 'A班' THEN [B].[FirstB] WHEN [A].[SecondWorkingTeam] = 'A班' THEN [B].[SecondB] WHEN [A].[ThirdWorkingTeam] = 'A班' THEN [B].[ThirdB] ELSE 0 END) AS A班,
		                            SUM(CASE WHEN [A].[FirstWorkingTeam] = 'B班' THEN [B].[FirstB] WHEN [A].[SecondWorkingTeam] = 'B班' THEN [B].[SecondB] WHEN [A].[ThirdWorkingTeam] = 'B班' THEN [B].[ThirdB] ELSE 0 END) AS B班,
		                            SUM(CASE WHEN [A].[FirstWorkingTeam] = 'C班' THEN [B].[FirstB] WHEN [A].[SecondWorkingTeam] = 'C班' THEN [B].[SecondB] WHEN [A].[ThirdWorkingTeam] = 'C班' THEN [B].[ThirdB] ELSE 0 END) AS C班,
		                            SUM(CASE WHEN [A].[FirstWorkingTeam] = 'D班' THEN [B].[FirstB] WHEN [A].[SecondWorkingTeam] = 'D班' THEN [B].[SecondB] WHEN [A].[ThirdWorkingTeam] = 'D班' THEN [B].[ThirdB] ELSE 0 END) AS D班,
		                            SUM([B].[TotalPeakValleyFlatB]) AS [合计]
                            FROM	[tz_Balance] AS [A] INNER JOIN
		                            [balance_Energy] AS [B] ON [A].[BalanceId] = [B].[KeyId]
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
            return dataFactory.Query(sql, parameters);
        }

        private static DataTable GetProcessValueTableMonthly(string organizationId, string startDate, string endDate)
        {
            //string startDate = (month.ToString("yyyy-MM") + "-01");
            //string endDate = (month.ToString("yyyy-MM") + "-" + month.AddMonths(1).AddDays(-(month.Day)).ToString("dd"));
            return GetProcessValueTableByOrganizationId(organizationId, startDate, endDate);
        }

        public static DataTable GetTeamJobEvaluationMonthly(string organization, string startDate, string endDate)
        {
            DataTable table = GetProcessValueTableMonthly(organization, startDate, endDate);
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
