using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.ComprehensiveReport
{
    public static class ElectricityConsumptionReportService
    {

        /// <summary>
        /// 查询所有产线的用电量
        /// </summary>
        /// <param name="organizationIds">组织机构ID数组（通常为授权的组织机构ID数组）</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static DataTable GetElectricityConsumptionDailyByOrganiztionIds(List<string> myOrganizationId, DateTime startDate, DateTime endDate)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string m_OrganizationIds = "";
            if (myOrganizationId != null)
            {
                for (int i = 0; i < myOrganizationId.Count; i++)
                {
                    if (i == 0)
                    {
                        m_OrganizationIds = "'" + myOrganizationId[i] + "'";
                    }
                    else
                    {
                        m_OrganizationIds = m_OrganizationIds + ",'" + myOrganizationId[i] + "'";
                    }
                }
            }
            //////找到当前授权分厂级组织机构ID

            string m_Sql_FactoryOrganizations = "Select distinct A.FactoryOrganizationID as FactoryOrganizationID from analyse_KPI_OrganizationContrast A where A.OrganizationID in ({0})";
            m_Sql_FactoryOrganizations = string.Format(m_Sql_FactoryOrganizations, m_OrganizationIds);
            DataTable m_FactoryOrganizationTable = dataFactory.Query(m_Sql_FactoryOrganizations);

            string m_ConditionFactoryOrganizations = "";           //找到的分厂级的组织机构
            if (m_FactoryOrganizationTable != null)
            {
                for (int i = 0; i < m_FactoryOrganizationTable.Rows.Count; i++)
                {
                    string[] m_FactoryOrganizationItem = m_FactoryOrganizationTable.Rows[i]["FactoryOrganizationID"].ToString().Split(',');
                    for (int j = 0; j < m_FactoryOrganizationItem.Length; j++)
                    {
                        if (i == 0 && j == 0)
                        {
                            m_ConditionFactoryOrganizations = "'" + m_FactoryOrganizationItem[j] + "'";
                        }
                        else
                        {
                            m_ConditionFactoryOrganizations = m_ConditionFactoryOrganizations + ",'" + m_FactoryOrganizationItem[j] + "'";
                        }
                    }
                }
            }
            string queryString = @"select M.Name,M.LevelCode,M.VariableName, M.FormulaLevelCode, N.FirstB, N.SecondB, N.ThirdB, N.TotalPeakValleyFlatB from(  
							        select 
								    E.Name as Name, 
								    E.OrganizationID as OrganizationID,
								    B.VariableId as VariableId,
								    (case when B.LevelType='ProductionLine' then A.Name else B.Name end) as VariableName,  
                                    (case when E.Type = '熟料' then E.LevelCode + SUBSTRING(B.LevelCode, 4, LEN(B.LevelCode) - 3)
								          when E.Type = '水泥磨' then E.LevelCode + SUBSTRING(B.LevelCode, 4, LEN(B.LevelCode) - 3)
									      when E.Type = '余热发电' then E.LevelCode + SUBSTRING(B.LevelCode, 4, LEN(B.LevelCode) - 3)
									      when E.Type = '分厂' then E.LevelCode + '99' + SUBSTRING(B.LevelCode, 2, LEN(B.LevelCode) - 1)  else E.LevelCode  end) as LevelCode,
								    E.LevelCode as FormulaLevelCode 
		                                    from tz_Formula A, formula_FormulaDetail B,system_Organization D, system_Organization E
			                                    where D.OrganizationID in ({0})
			                                    and E.LevelCode like D.LevelCode + '%' 
			                                    and A.OrganizationID in (E.OrganizationID)
			                                    and A.ENABLE = 1
			                                    and A.State = 0
			                                    and A.KeyID = B.KeyID
                                                and A.Type = 2
                                                and E.Type <> '余热发电'
                                                and B.LevelType <> 'MainMachine'
                                                and B.Visible = 1
                                        union all 
									    select distinct B.Name as Name, B.OrganizationID as OrganizationID, '' as VariableId, B.Name as VariableName, B.LevelCode as LevelCode, B.LevelCode as FormulaLevelCode from system_Organization A,system_Organization B
									      where A.OrganizationID in ({0})
									      and B.LevelType <> 'ProductionLine' 
		                                  and (B.LevelCode like A.LevelCode + '%' or CHARINDEX(B.LevelCode,A.LevelCode)>0)
		                            ) M
								    left join (
								    select B.OrganizationID as OrganizationID, 
								        B.VariableId as VariableId, 
									    SUM(B.FirstB) AS FirstB,
									    SUM(B.SecondB) AS SecondB,
									    SUM(B.ThirdB) AS ThirdB,
									    SUM(B.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
							        from tz_Balance A, balance_Energy B
								    where A.TimeStamp >= '{1}'
								    and A.TimeStamp <= '{2}'
								    and A.OrganizationID in ({0})
								    and A.BalanceId = B.KeyId
								    group by B.OrganizationID, B.VariableId) N 
                                    on M.VariableId + '_ElectricityConsumption' = N.VariableId and M.OrganizationID = N.OrganizationID 
                                    order by M.LevelCode";
            queryString = string.Format(queryString, m_ConditionFactoryOrganizations, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            DataTable resultTable = dataFactory.Query(queryString);

            DataColumn stateColumn = new DataColumn("state", typeof(string));
            resultTable.Columns.Add(stateColumn);

            foreach (DataRow dr in resultTable.Rows)
            {
                if (dr["LevelCode"].ToString().Trim().Length == 7)
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
    }
}
