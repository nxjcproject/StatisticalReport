using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SqlServerDataAdapter;
using System.Data.SqlClient;
using StatisticalReport.Infrastructure.Configuration;

namespace StatisticalReport.Service.BasicDataSummaryReport
{
    public class AirPollutantEmissionMonitoring
    {   //----------------------20180706闫潇华-----------------------//
        private static string _connectionString = ConnectionStringFactory.NXJCConnectionString;
        private static ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(_connectionString);

        public static DataTable GetAirPollutantEmissionDataInfo(string mStartTime, string mEndTime)
        {
            DataTable mInitialTable = GetOrgsAndTagsInfo();
            mInitialTable = GetTagsProcessDatabase(mInitialTable);
            DataTable mResultTable = GetTagsData(mInitialTable, mStartTime, mEndTime);
            return mResultTable;
        }

        /// <summary>
        /// 获取组织机构和标签信息
        /// </summary>
        /// <returns></returns>
        private static DataTable GetOrgsAndTagsInfo()
        {
            string mSql = @"SELECT A.[OrganizationID]
                                  ,A.[LevelCode]
                                  ,A.[DatabaseID]
	                              ,'' AS [ProcessDatabase]
	                              ,C.[DCSProcessDatabase]
                                  ,A.[Name]
                                  ,A.[Type]
                                  ,A.[LevelType]
	                              ,B.[ItemName]
                                  ,B.[Unit]
                                  ,B.[PageId]
                                  ,B.[Tags]
                                  ,'' AS TagsAvgData
                                  ,B.[DisplayIndex]
                              FROM system_Organization A
                                   LEFT JOIN
	                               realtime_KeyIndicatorsMonitorContrast B
	                               ON A.[OrganizationID] = B.[OrganizationID] 
	                                  AND B.[PageId] = 'EnvironmentalMonitor' 
		                              AND B.[Enabled] = 1
	                               LEFT JOIN
		                            system_Database C
	                               ON A.[DatabaseID] = C.[DatabaseID]
                             WHERE 
                                   A.[Type] <> '水泥磨' AND A.[Type] <> '余热发电'
                               AND A.[ENABLED] = 1
                              order by A.[LevelCode],B.[OrganizationID],B.[DisplayIndex]";
            DataTable table = _dataFactory.Query(mSql);
            return table;
        }

        /// <summary>
        /// 获取每个标签在哪个History_ProcessVariable**表中
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static DataTable GetTagsProcessDatabase(DataTable table)
        {
            DataTable mVariableAllInfoTable = table;
            int tableCount = mVariableAllInfoTable.Rows.Count;
            int drCount = 0; 
            for (int i = 0; i < tableCount; i++)
            {
                StringBuilder mStrBulider = new StringBuilder();
                string mVariableArray = null;
                string aa = mVariableAllInfoTable.Rows[i]["Tags"].ToString().Trim();
                if (mVariableAllInfoTable.Rows[i]["Tags"].ToString().Trim() != "")
                {
                    string mDCSProcessDatabase = mVariableAllInfoTable.Rows[i]["DCSProcessDatabase"].ToString().Trim();
                    DataRow[] drs = mVariableAllInfoTable.Select("DCSProcessDatabase = '" + mDCSProcessDatabase + "'");
                    drCount = drs.Length;
                    for (int j = 0; j < drCount; j++)//此循环为了拼接同一数据库下的变量VariableName
                    {
                        if (j == 0)
                        {
                            mVariableArray = "'" + drs[j]["Tags"] + "'";
                        }
                        else
                        {
                            mVariableArray = mVariableArray + ",'" + drs[j]["Tags"] + "'";
                        }
                    }
                    DataTable mProcessDatabase = SelectTagsProcessDatabase(mDCSProcessDatabase, mVariableArray);//带有变量和对应表的Table
                    for (int k = 0; k < mProcessDatabase.Rows.Count; k++)
                    {
                        string mTestOrg = mProcessDatabase.Rows[k]["OrganizationID"].ToString().Trim();
                        string mTestVariableName = mProcessDatabase.Rows[k]["VariableName"].ToString().Trim();
                        for (int y = 0; y < tableCount; y++)
                        {
                            if (mVariableAllInfoTable.Rows[y]["OrganizationID"].ToString().Trim() == mTestOrg && mVariableAllInfoTable.Rows[y]["Tags"].ToString().Trim() == mTestVariableName)
                            {
                                mVariableAllInfoTable.Rows[y]["ProcessDatabase"] = mProcessDatabase.Rows[k]["TableName"];
                            }
                        }
                    }
                    i = i + drCount - 1;
                }          
            }
            return mVariableAllInfoTable;
        }
        /// <summary>
        /// 查询每个字段在哪个表
        /// </summary>
        /// <param name="DCSProcessDatabase">数据库名，如zc_nxjc_byc_byf_dcs01</param>
        /// <param name="mVariableName">变量拼接</param>
        /// <returns></returns>
        private static DataTable SelectTagsProcessDatabase(string DCSProcessDatabase,string mVariableName)
        {
            string mSql = @"SELECT [OrganizationID]
                                  ,[VariableName]
                                  ,[Item]
                                  ,[TableName]
                                  ,[FieldName]
                                  ,[DataType]
                              FROM {0}.[dbo].[DCSContrast]
                              where [VariableName] in ({1})";
            mSql = string.Format(mSql, DCSProcessDatabase, mVariableName);
            DataTable result = _dataFactory.Query(mSql);
            return result;
        }

        /// <summary>
        /// 获取最终数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="mStartTime"></param>
        /// <param name="mEndTime"></param>
        /// <returns></returns>
        private static DataTable GetTagsData(DataTable table, string mStartTime, string mEndTime)
        {
            DataTable mRequireTable = table;            
            StringBuilder mFinalSelectSql = new StringBuilder();//最终拼接的sql语句
            string mCommonSql = "select ({0}) AS Id, AVG({1}) AS TagsAvgData from {2} where {3}";
            int tCount = mRequireTable.Rows.Count;
            string mTimeCondition = "vDate>='" + mStartTime + "' and vDate<='" + mEndTime + "'";//时间条件{3}
            for (int i = 0; i < tCount; i++)
            {
                if (mRequireTable.Rows[i]["Tags"].ToString().Trim() != "" && mRequireTable.Rows[i]["Tags"].ToString().Trim() != "0")
                {
                    string mId = "'" + mRequireTable.Rows[i]["DCSProcessDatabase"].ToString().Trim() + "_" + mRequireTable.Rows[i]["ProcessDatabase"].ToString().Trim() + "_" + mRequireTable.Rows[i]["Tags"].ToString().Trim() + "'";
                    string mTag = mRequireTable.Rows[i]["Tags"].ToString().Trim();
                    string mdbo = mRequireTable.Rows[i]["DCSProcessDatabase"].ToString().Trim() + ".dbo.History_" + mRequireTable.Rows[i]["ProcessDatabase"].ToString().Trim();
                    mFinalSelectSql.AppendFormat(mCommonSql, mId, mTag, mdbo, mTimeCondition);
                    mFinalSelectSql.Append(" UNION ");
                }
            }
            mFinalSelectSql.Remove(mFinalSelectSql.Length - 7, 7);
            DataTable mDataTable = _dataFactory.Query(mFinalSelectSql.ToString());
            return mRequireTable;
        }
    }
}

