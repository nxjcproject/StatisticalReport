using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;

namespace StatisticalReport.Service.LightLoadOperationReport
{
    public class LightLoadOperationDaily
    {
        /// <summary>
        /// 获得配置信息
        /// </summary>
        /// <param name="myOrganizationId">分厂组织机构ID</param>
        /// <returns>配置信息表</returns>
        public static DataTable GetVariableInfo(string myOrganizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string m_Sql = @"SELECT A.ID as id
                                ,A.VariableDescription as text
                                FROM equipment_LowLoadOperationConfig A
                                where A.OrganizationID = '{0}'
                                and A.Record = 1
                                order by A.RunTag";
            try
            {
                return dataFactory.Query(string.Format(m_Sql, myOrganizationId));
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获得配置信息
        /// </summary>
        /// <param name="myOrganizationId">分厂组织机构ID</param>
        /// <returns>配置信息表</returns>
        public static DataTable GetVariableInfoById(string myOrganizationId, string myVariableId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string m_Sql = @"SELECT A.ID as id
                                ,A.OrganizationID
                                ,A.VariableName
                                ,A.VariableDescription as text
                                ,B.Name
                                ,ltrim(rtrim(A.RunTag)) as RunTag
                                ,ltrim(rtrim(A.RunTagTableName)) as RunTagTableName
                                ,ltrim(rtrim(A.RunTagDataBaseName)) as RunTagDataBaseName
                                ,A.Record
                                ,A.ValidValues
                                ,A.DelayTime
                                ,A.LoadTag
                                ,ltrim(rtrim(D.MeterDatabase)) as LoadDataBaseName
                                ,ltrim(rtrim(A.DCSLoadTag)) as DCSLoadTag
                                ,ltrim(rtrim(A.DCSLoadTableName)) as DCSLoadTableName
                                ,ltrim(rtrim(A.DCSLoadDataBaseName)) as DCSLoadDataBaseName
                                ,(case when A.LoadTagType = 'current' then '电流' when A.LoadTagType = 'power' then '功率' end) as  LoadTagTypeName
                                ,A.LoadTagType
                                ,A.LLoadLimit
                                ,A.Editor
                                ,A.EditTime
                                ,A.Remark
                                FROM equipment_LowLoadOperationConfig A
                                left join system_Organization B on A.OrganizationID = B.OrganizationID
                                ,system_Organization C, system_Database D
                                where A.OrganizationID = '{0}'
                                and A.Record = 1
                                and A.OrganizationID = C.OrganizationID
                                and C.DatabaseID = D.DatabaseID
                                {1}
                                order by A.RunTag";
            string m_VariableCondition = "";
            if (myVariableId != "All")
            {
                m_VariableCondition = string.Format(" and A.ID = '{0}' ", myVariableId);
            }
            try
            {
                return dataFactory.Query(string.Format(m_Sql, myOrganizationId, m_VariableCondition));
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 根据显示内容构造表
        /// </summary>
        /// <returns>低负荷报警数据表</returns>
        public static DataTable GetLightLoadData()
        {
            DataTable m_LightLoadDataTable = new DataTable();
            m_LightLoadDataTable.Columns.Add("ID", typeof(string));
            m_LightLoadDataTable.Columns.Add("OrganizationID", typeof(string));
            m_LightLoadDataTable.Columns.Add("LevelCode", typeof(string));
            m_LightLoadDataTable.Columns.Add("VariableDescription", typeof(string));
            m_LightLoadDataTable.Columns.Add("Name", typeof(string));
            m_LightLoadDataTable.Columns.Add("StartTimeRun", typeof(DateTime));
            m_LightLoadDataTable.Columns.Add("EndTimeStop", typeof(DateTime));
            m_LightLoadDataTable.Columns.Add("StartTimeAlarm", typeof(DateTime));
            m_LightLoadDataTable.Columns.Add("EndTimeAlarm", typeof(DateTime));
            m_LightLoadDataTable.Columns.Add("AlarmTimeLong", typeof(string));
            m_LightLoadDataTable.Columns.Add("RunTimeLong", typeof(string));
            m_LightLoadDataTable.Columns.Add("LoadValueAvg", typeof(decimal));
            m_LightLoadDataTable.Columns.Add("LoadTagType", typeof(string));
            m_LightLoadDataTable.Columns.Add("DelayTime", typeof(int));
            m_LightLoadDataTable.Columns.Add("LLoadLimit", typeof(decimal));
            m_LightLoadDataTable.Columns.Add("Remark", typeof(string));
            return m_LightLoadDataTable;
        }
        /// <summary>
        /// 获取设备有效运行时间段和报警时间段
        /// </summary>
        /// <param name="myVariableInfoRow">设备配置信息</param>
        /// <param name="myStartTime">开始时间</param>
        /// <param name="myEndTime">结束时间</param>
        /// <returns>设备运行时间段</returns>
        public static DataTable GetValidMachineRunAlarmTime(DataRow myVariableInfoRow, string myStartTime, string myEndTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            DataTable m_RunAlarmTimeTable = null;
          
            int m_DelayTime = (int)myVariableInfoRow["DelayTime"];
            string m_ID = myVariableInfoRow["ID"].ToString();

            string m_Sql = @"select * from [dbo].[shift_LightLoadAlarmlog]
                                   where [LightLoadID]='{0}'
                                     and [EditTime]>='{1}'
                                     and [EditTime]<'{2}'
                                     order by [StartTime]";
            try
            {
                m_Sql = string.Format(m_Sql, m_ID, myStartTime, myEndTime);
                m_RunAlarmTimeTable = dataFactory.Query(m_Sql);
                DataRow[] drs = m_RunAlarmTimeTable.Select("Type='run'", "StartTime asc");
                for (int i = 0; i < drs.Length; i++)
                {
                    if (drs[i]["EndTime"].ToString() == "")
                    {
                        if (i == drs.Length - 1)
                        {
                            drs[i]["EndTime"] = DateTime.Parse(myEndTime);
                        }
                        else
                        {
                            drs[i]["EndTime"] = (DateTime)drs[i + 1]["StartTime"];
                        }
                    }                            
                }
                foreach (DataRow row in drs)//除去开机停机延时时间
                {
                    row["StartTime"] = ((DateTime)row["StartTime"]).AddSeconds(m_DelayTime);
                    row["EndTime"] = ((DateTime)row["EndTime"]).AddSeconds(-m_DelayTime);
                }
                DataRow[] deleteNotAlarmRow = m_RunAlarmTimeTable.Select("StartTime>=EndTime");//若开始时间大于结束时间则认为不是报警
                foreach (DataRow rows in deleteNotAlarmRow)
                {
                    m_RunAlarmTimeTable.Rows.Remove(rows);
                }
                return m_RunAlarmTimeTable;
            }         
            catch
            {
                return m_RunAlarmTimeTable;
            }
        }
        /// <summary>
        /// 计算低负荷运转开始时间结束时间和平均值(电表)
        /// </summary>
        /// <param name="myValidMachineRunTimesList">设备有效运行时间段</param>
        /// <param name="myVariableInfoRow">设备配置信息</param>
        /// <param name="myStartTime">查询开始时间</param>
        /// <param name="myEndTime">查询结束时间</param>
        /// <returns></returns>
        public static decimal GetLightLoadAvgValueTemp_Ammeter(DataRow myVariableInfoRow, string m_StartTimeAlarm, string m_EndTimeAlarm)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string m_LoadTagName = myVariableInfoRow["LoadTag"].ToString();
            if (myVariableInfoRow["LoadTagType"].ToString() == "power")
            {
                m_LoadTagName = m_LoadTagName.Replace(" ", "").Replace("+", "Power+").Replace("-", "Power-").Replace(")", "Power)");
                if (m_LoadTagName.Substring(m_LoadTagName.Length - 1, 1) != ")")   //如果最后一个字符不是括号，那么一定是标签
                {
                    m_LoadTagName = m_LoadTagName + "Power";
                }
            }
            string m_LoadDataTableName = myVariableInfoRow["LoadTagType"].ToString() == "current" ? "History_Current" : "HistoryAmmeter";
            string m_LoadDataBaseName = myVariableInfoRow["LoadDataBaseName"].ToString();

            string m_Sql = @"select avg({0}) as LightLoadAvgValue 
                                               from [{2}].[dbo].[{1}] 
                                              where vDate>='{3}' 
                                                and vDate<='{4}'";
            try
            {
                m_Sql = string.Format(m_Sql, m_LoadTagName, m_LoadDataTableName, m_LoadDataBaseName, m_StartTimeAlarm, m_EndTimeAlarm);
                DataTable m_LigthLoadDataTable = dataFactory.Query(m_Sql);
                if (m_LigthLoadDataTable != null)
                {
                    return (decimal)m_LigthLoadDataTable.Rows[0]["LightLoadAvgValue"];
                }
                else
                {
                    return 0.0m;
                }
            }
            catch
            {
                return 0.0m;
            }
        }
        /// <summary>
        /// 计算设备有效运行时间内总平均负荷(电表)
        /// </summary>
        /// <param name="myValidMachineRunTimesList">有效运行时间段</param>
        /// <param name="myVariableInfoRow">设备配置信息</param>
        /// <returns></returns>
        public static decimal GetLigthLoadDataAvg_Ammeter(DataRow myVariableInfoRow, DataRow[] myRunTimeRow)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string m_LoadTagName = myVariableInfoRow["LoadTag"].ToString();
            if (myVariableInfoRow["LoadTagType"].ToString() == "power")
            {
                m_LoadTagName = m_LoadTagName.Replace(" ", "").Replace("+", "Power+").Replace("-", "Power-").Replace(")", "Power)");
                if (m_LoadTagName.Substring(m_LoadTagName.Length - 1, 1) != ")")   //如果最后一个字符不是括号，那么一定是标签
                {
                    m_LoadTagName = m_LoadTagName + "Power";
                }
            }
            string m_LoadDataTableName = myVariableInfoRow["LoadTagType"].ToString() == "current" ? "History_Current" : "HistoryAmmeter";
            string m_LoadDataBaseName = myVariableInfoRow["LoadDataBaseName"].ToString();
            
            string m_Sql = @"Select avg({0}) as AvgValue from [{2}].[dbo].[{1}]
                                where ({3})";
            try
            {
                string m_DateTimeCondition = "";
                for (int i = 0; i < myRunTimeRow.Length; i++)
                {
                    if (i == 0)
                    {
                        m_DateTimeCondition = string.Format("(vDate >= '{0}' and vDate <= '{1}')", myRunTimeRow[i]["StartTime"], myRunTimeRow[i]["EndTime"]);
                    }
                    else
                    {
                        m_DateTimeCondition = m_DateTimeCondition + " or " + string.Format("(vDate >= '{0}' and vDate <= '{1}')", myRunTimeRow[i]["StartTime"], myRunTimeRow[i]["EndTime"]);
                    }
                }
                if (m_DateTimeCondition != "")
                {
                    m_Sql = string.Format(m_Sql, m_LoadTagName, m_LoadDataTableName, m_LoadDataBaseName, m_DateTimeCondition);
                    DataTable m_LigthLoadDataTable = dataFactory.Query(m_Sql);
                    if (m_LigthLoadDataTable != null)
                    {
                        return (decimal)m_LigthLoadDataTable.Rows[0]["AvgValue"];
                    }
                    else
                    {
                        return 0.0m;
                    }
                }
                else
                {
                    return 0.0m;
                }
            }
            catch
            {
                return 0.0m;
            }
        }
        public static decimal GetLigthLoadDataAvgByRun_Ammeter(DataRow myVariableInfoRow, string m_StartTimeRun, string m_EndTimeRun)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string m_LoadTagName = myVariableInfoRow["LoadTag"].ToString();
            if (myVariableInfoRow["LoadTagType"].ToString() == "power")
            {
                m_LoadTagName = m_LoadTagName.Replace(" ", "").Replace("+", "Power+").Replace("-", "Power-").Replace(")", "Power)");
                if (m_LoadTagName.Substring(m_LoadTagName.Length - 1, 1) != ")")   //如果最后一个字符不是括号，那么一定是标签
                {
                    m_LoadTagName = m_LoadTagName + "Power";
                }
            }
            string m_LoadDataTableName = myVariableInfoRow["LoadTagType"].ToString() == "current" ? "History_Current" : "HistoryAmmeter";
            string m_LoadDataBaseName = myVariableInfoRow["LoadDataBaseName"].ToString();

            string m_Sql = @"select avg({0]) as RunAvgValue 
                                               from [{2}].[dbo].[{1}] 
                                              where vDate>='{3}' 
                                                and vDate<='{4}'";
            try
            {
                m_Sql = string.Format(m_Sql, m_LoadTagName, m_LoadDataTableName, m_LoadDataBaseName, m_StartTimeRun, m_EndTimeRun);
                DataTable m_LigthLoadDataTable = dataFactory.Query(m_Sql);
                if (m_LigthLoadDataTable != null)
                {
                    return (decimal)m_LigthLoadDataTable.Rows[0]["RunAvgValue"];
                }
                else
                {
                    return 0.0m;
                }
            }
            catch
            {
                return 0.0m;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="myValidMachineRunTimesList"></param>
        /// <param name="myVariableInfoRow"></param>
        /// <param name="myStartTime"></param>
        /// <param name="myEndTime"></param>
        /// <returns></returns>
        public static decimal GetLightLoadAvgValueTemp_DCS(DataRow myVariableInfoRow, string m_StartTimeAlarm, string m_EndTimeAlarm)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string m_LoadTagName = myVariableInfoRow["DCSLoadTag"].ToString();
            string m_LoadDataTableName = myVariableInfoRow["DCSLoadTableName"].ToString();
            string m_LoadDataBaseName = myVariableInfoRow["DCSLoadDataBaseName"].ToString();

            string m_Sql = @"select avg({0}) as LightLoadAvgValue 
                                               from [{2}].[dbo].[History_{1}] 
                                              where vDate>='{3}' 
                                                and vDate<='{4}'";
            try
            {
                m_Sql = string.Format(m_Sql, m_LoadTagName, m_LoadDataTableName, m_LoadDataBaseName, m_StartTimeAlarm, m_EndTimeAlarm);
                DataTable m_LigthLoadDataTable = dataFactory.Query(m_Sql);
                if (m_LigthLoadDataTable != null)
                {
                    return (decimal)m_LigthLoadDataTable.Rows[0]["LightLoadAvgValue"];
                }
                else
                {
                    return 0.0m;
                }
            }
            catch
            {
                return 0.0m;
            }
        }
        public static decimal GetLigthLoadDataAvg_DCS(DataRow myVariableInfoRow, DataRow[] myRunTimeRow)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string m_LoadTagName = myVariableInfoRow["DCSLoadTag"].ToString();
            string m_LoadDataTableName = myVariableInfoRow["DCSLoadTableName"].ToString();
            string m_LoadDataBaseName = myVariableInfoRow["DCSLoadDataBaseName"].ToString();          

            string m_Sql = @"Select avg({0}) as AvgValue 
                               from [{2}].[dbo].[History_{1}]
                              where ({3})";
            try
            {
                string m_DateTimeCondition = "";
                for (int i = 0; i < myRunTimeRow.Length; i++)
                {
                    if (i == 0)
                    {
                        m_DateTimeCondition = string.Format("(vDate >= '{0}' and vDate <= '{1}')", myRunTimeRow[i]["StartTime"], myRunTimeRow[i]["EndTime"]);
                    }
                    else
                    {
                        m_DateTimeCondition = m_DateTimeCondition + " or " + string.Format("(vDate >= '{0}' and vDate <= '{1}')", myRunTimeRow[i]["StartTime"], myRunTimeRow[i]["EndTime"]);
                    }
                }
                if (m_DateTimeCondition != "")
                {
                    m_Sql = string.Format(m_Sql, m_LoadTagName, m_LoadDataTableName, m_LoadDataBaseName, m_DateTimeCondition);
                    DataTable m_LigthLoadDataTable = dataFactory.Query(m_Sql);
                    if (m_LigthLoadDataTable != null)
                    {
                        return (decimal)m_LigthLoadDataTable.Rows[0]["AvgValue"];
                    }
                    else
                    {
                        return 0.0m;
                    }
                }
                else
                {
                    return 0.0m;
                }

            }
            catch
            {
                return 0.0m;
            }
        }
        public static decimal GetLigthLoadDataAvgByRun_DCS(DataRow myVariableInfoRow, string m_StartTimeRun, string m_EndTimeRun)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string m_LoadTagName = myVariableInfoRow["DCSLoadTag"].ToString();
            string m_LoadDataTableName = myVariableInfoRow["DCSLoadTableName"].ToString();
            string m_LoadDataBaseName = myVariableInfoRow["DCSLoadDataBaseName"].ToString();

            string m_Sql = @"select avg({0}) as RunAvgValue 
                                               from [{2}].[dbo].[History_{1}] 
                                              where vDate>='{3}' 
                                                and vDate<='{4}'";
            try
            {
                m_Sql = string.Format(m_Sql, m_LoadTagName, m_LoadDataTableName, m_LoadDataBaseName, m_StartTimeRun, m_EndTimeRun);
                DataTable m_LigthLoadDataTable = dataFactory.Query(m_Sql);
                if (m_LigthLoadDataTable != null)
                {
                    return (decimal)m_LigthLoadDataTable.Rows[0]["RunAvgValue"];
                }
                else
                {
                    return 0.0m;
                }
            }
            catch
            {
                return 0.0m;
            }
        }
        public static DataTable GetLightLoadData(DataTable myVariableInfoTable, string myStartTime, string myEndTime)
        {
            DataTable m_LightLoadDataTable = GetLightLoadData();
            if (myVariableInfoTable != null)
            {
                for (int i = 0; i < myVariableInfoTable.Rows.Count; i++)//ID
                {
                    string m_LoadDCSTagTemp = myVariableInfoTable.Rows[i]["DCSLoadTag"] != DBNull.Value ? myVariableInfoTable.Rows[i]["DCSLoadTag"].ToString() : "";
                    string m_LoadTagTemp = myVariableInfoTable.Rows[i]["LoadTag"] != DBNull.Value ? myVariableInfoTable.Rows[i]["LoadTag"].ToString() : "";
                    DataTable m_RunAlarmTimeTable = GetValidMachineRunAlarmTime(myVariableInfoTable.Rows[i], myStartTime, myEndTime);//第一需要取得
                    TimeSpan m_RunTimeAccumulationPreAll = new TimeSpan();//总运行时间
                    TimeSpan m_AlarmTimeAccumulationAll = new TimeSpan();//总报警时间
                    decimal m_AvgValueTemp = 0.0m;//总平均负荷
                    DataRow[] m_RunTimeRow = m_RunAlarmTimeTable.Select("Type='run'");
                    if (m_LoadDCSTagTemp != "")
                    {
                        m_AvgValueTemp = GetLigthLoadDataAvg_DCS(myVariableInfoTable.Rows[i], m_RunTimeRow);
                    }
                    else if (m_LoadTagTemp != "")    //DCS标签没有设置,表示走电表数据
                    {
                        m_AvgValueTemp = GetLigthLoadDataAvg_Ammeter(myVariableInfoTable.Rows[i], m_RunTimeRow);
                    }
                    
                    int m_RunDisplayIndex = 1;
                    int m_RunAlarmCount = 0;
                    for (int j = 0; j < m_RunTimeRow.Length; j++)
                    {
                        TimeSpan m_RunTimeAccumulationPreRun = new TimeSpan();//设备运行时间
                        TimeSpan m_AlarmTimeAccumulationPreRun = new TimeSpan();//设备报警时间
                                            
                        DateTime m_StartTimeRun = (DateTime)m_RunTimeRow[j]["StartTime"];
                        DateTime m_EndTimeRun = (DateTime)m_RunTimeRow[j]["EndTime"];
                        
                        m_RunTimeAccumulationPreRun = m_EndTimeRun - m_StartTimeRun;//设备运行时间 
                        m_RunTimeAccumulationPreAll = m_RunTimeAccumulationPreAll + m_RunTimeAccumulationPreRun;//总运行时间

                        DataRow[] m_AlarmTimeRow = m_RunAlarmTimeTable.Select(string.Format("StartTime>='{0}' and EndTime<='{1}' and Type='alarm'", m_StartTimeRun, m_EndTimeRun));//一个run下的alarm报警
                        m_RunAlarmCount = m_RunAlarmCount + m_AlarmTimeRow.Length;//记录报警总数，只有总数大于0，才会显示此ID的报警信息
                        for (int z = 0; z < m_AlarmTimeRow.Length; z++)
                        {
                            DateTime m_StartTimeAlarm = (DateTime)m_AlarmTimeRow[z]["StartTime"];//报警开始时间
                            DateTime m_EndTimeAlarm = (DateTime)m_AlarmTimeRow[z]["EndTime"];//报警结束时间
                            TimeSpan m_AlarmTimeAccumulationPreAlarm = m_EndTimeAlarm - m_StartTimeAlarm;//报警持续时间
                            m_AlarmTimeAccumulationPreRun = m_AlarmTimeAccumulationPreRun + m_AlarmTimeAccumulationPreAlarm;//设备报警时间
                            decimal m_LigthLoadAvgValueTemp = 0.0m;
                            if (m_LoadDCSTagTemp != "")
                            {
                                m_LigthLoadAvgValueTemp = GetLightLoadAvgValueTemp_DCS(myVariableInfoTable.Rows[i], m_StartTimeAlarm.ToString(), m_EndTimeAlarm.ToString());                             
                            }
                            else if (m_LoadTagTemp != "")    //DCS标签没有设置,表示走电表数据
                            {
                                m_LigthLoadAvgValueTemp = GetLightLoadAvgValueTemp_Ammeter(myVariableInfoTable.Rows[i], m_StartTimeAlarm.ToString(), m_EndTimeAlarm.ToString());                               
                            }
                            ///////////////////添加叶子节点/////////////////////
                            DataRow m_NewLeafRowTemp = m_LightLoadDataTable.NewRow();
                            m_NewLeafRowTemp["ID"] = myVariableInfoTable.Rows[i]["id"].ToString() + "_" + j.ToString("00") + "_" + z.ToString("00");
                            m_NewLeafRowTemp["OrganizationID"] = myVariableInfoTable.Rows[i]["OrganizationID"].ToString();
                            m_NewLeafRowTemp["LevelCode"] = "D" + (i + 1).ToString("00") + m_RunDisplayIndex.ToString("00") + (z + 1).ToString("00");    
                            m_NewLeafRowTemp["VariableDescription"] = "报警" + (z + 1).ToString();   
                            m_NewLeafRowTemp["Name"] = "";                 
                            m_NewLeafRowTemp["StartTimeRun"] = DBNull.Value;
                            m_NewLeafRowTemp["EndTimeStop"] = DBNull.Value;
                            m_NewLeafRowTemp["StartTimeAlarm"] = m_StartTimeAlarm;
                            m_NewLeafRowTemp["EndTimeAlarm"] = m_EndTimeAlarm;
                            m_NewLeafRowTemp["AlarmTimeLong"] = m_AlarmTimeAccumulationPreAlarm.Days.ToString() + "天" + m_AlarmTimeAccumulationPreAlarm.Hours.ToString() + "时" + m_AlarmTimeAccumulationPreAlarm.Minutes.ToString() + "分" + m_AlarmTimeAccumulationPreAlarm.Seconds.ToString() + "秒";
                            m_NewLeafRowTemp["RunTimeLong"] = "";
                            m_NewLeafRowTemp["LoadValueAvg"] = m_LigthLoadAvgValueTemp;
                            m_NewLeafRowTemp["LoadTagType"] = myVariableInfoTable.Rows[i]["LoadTagTypeName"].ToString();
                            m_NewLeafRowTemp["DelayTime"] = myVariableInfoTable.Rows[i]["DelayTime"].ToString();
                            m_NewLeafRowTemp["LLoadLimit"] = myVariableInfoTable.Rows[i]["LLoadLimit"].ToString();
                            m_NewLeafRowTemp["Remark"] = myVariableInfoTable.Rows[i]["Remark"].ToString();
                            m_LightLoadDataTable.Rows.Add(m_NewLeafRowTemp);  //添加子点
                        }
                        m_AlarmTimeAccumulationAll = m_AlarmTimeAccumulationAll + m_AlarmTimeAccumulationPreRun;//总报警时间
                        ///////////////////添加叶子节点/////////////////////
                        if (m_AlarmTimeRow.Length > 0)
                        {
                            DataRow m_NewSubRowTemp = m_LightLoadDataTable.NewRow();
                            m_NewSubRowTemp["ID"] = myVariableInfoTable.Rows[i]["id"].ToString() + "_" + j.ToString("00");
                            m_NewSubRowTemp["OrganizationID"] = myVariableInfoTable.Rows[i]["OrganizationID"].ToString();
                            m_NewSubRowTemp["LevelCode"] = "D" + (i + 1).ToString("00") + m_RunDisplayIndex.ToString("00");
                            m_NewSubRowTemp["VariableDescription"] = "设备运行" + m_RunDisplayIndex.ToString();
                            m_NewSubRowTemp["Name"] = "";
                            m_NewSubRowTemp["StartTimeRun"] = m_StartTimeRun;                                                    
                            m_NewSubRowTemp["EndTimeStop"] = m_EndTimeRun;
                            m_NewSubRowTemp["StartTimeAlarm"] = DBNull.Value;
                            m_NewSubRowTemp["EndTimeAlarm"] = DBNull.Value;
                            m_NewSubRowTemp["AlarmTimeLong"] = m_AlarmTimeAccumulationPreRun.Days.ToString() + "天" + m_AlarmTimeAccumulationPreRun.Hours.ToString() + "时" + m_AlarmTimeAccumulationPreRun.Minutes.ToString() + "分" + m_AlarmTimeAccumulationPreRun.Seconds.ToString() + "秒";
                            m_NewSubRowTemp["RunTimeLong"] = m_RunTimeAccumulationPreRun.Days.ToString() + "天" + m_RunTimeAccumulationPreRun.Hours.ToString() + "时" + m_RunTimeAccumulationPreRun.Minutes.ToString() + "分" + m_RunTimeAccumulationPreRun.Seconds.ToString() + "秒";
                            if (m_LoadDCSTagTemp != "")
                            {
                                m_NewSubRowTemp["LoadValueAvg"] = GetLigthLoadDataAvgByRun_DCS(myVariableInfoTable.Rows[i], m_StartTimeRun.ToString(), m_EndTimeRun.ToString());
                            }
                            else if (m_LoadTagTemp != "")    //DCS标签没有设置,表示走电表数据
                            {
                                m_NewSubRowTemp["LoadValueAvg"] = GetLigthLoadDataAvgByRun_Ammeter(myVariableInfoTable.Rows[i], m_StartTimeRun.ToString(), m_EndTimeRun.ToString());
                            }

                            m_NewSubRowTemp["LoadTagType"] = myVariableInfoTable.Rows[i]["LoadTagTypeName"].ToString();
                            m_NewSubRowTemp["DelayTime"] = myVariableInfoTable.Rows[i]["DelayTime"].ToString();
                            m_NewSubRowTemp["LLoadLimit"] = myVariableInfoTable.Rows[i]["LLoadLimit"].ToString();
                            m_NewSubRowTemp["Remark"] = myVariableInfoTable.Rows[i]["Remark"].ToString();
                            m_LightLoadDataTable.Rows.Add(m_NewSubRowTemp);  //添加子点

                            m_RunDisplayIndex = m_RunDisplayIndex + 1;
                        }                       
                    }
                    if (m_RunAlarmCount > 0)
                    {
                        DataRow m_NewRowTemp = m_LightLoadDataTable.NewRow();
                        m_NewRowTemp["ID"] = myVariableInfoTable.Rows[i]["id"].ToString();
                        m_NewRowTemp["OrganizationID"] = myVariableInfoTable.Rows[i]["OrganizationID"].ToString();
                        m_NewRowTemp["LevelCode"] = "D" + (i + 1).ToString("00");
                        m_NewRowTemp["VariableDescription"] = myVariableInfoTable.Rows[i]["text"].ToString();
                        m_NewRowTemp["Name"] = myVariableInfoTable.Rows[i]["Name"].ToString();
                        m_NewRowTemp["StartTimeRun"] = DBNull.Value;
                        m_NewRowTemp["EndTimeStop"] = DBNull.Value;
                        m_NewRowTemp["StartTimeAlarm"] = DBNull.Value;
                        m_NewRowTemp["EndTimeAlarm"] = DBNull.Value;
                        m_NewRowTemp["AlarmTimeLong"] = m_AlarmTimeAccumulationAll.Days.ToString() + "天" + m_AlarmTimeAccumulationAll.Hours.ToString() + "时" + m_AlarmTimeAccumulationAll.Minutes.ToString() + "分" + m_AlarmTimeAccumulationAll.Seconds.ToString() + "秒";
                        m_NewRowTemp["RunTimeLong"] = m_RunTimeAccumulationPreAll.Days.ToString() + "天" + m_RunTimeAccumulationPreAll.Hours.ToString() + "时" + m_RunTimeAccumulationPreAll.Minutes.ToString() + "分" + m_RunTimeAccumulationPreAll.Seconds.ToString() + "秒";
                        m_NewRowTemp["LoadValueAvg"] = m_AvgValueTemp;
                        m_NewRowTemp["LoadTagType"] = myVariableInfoTable.Rows[i]["LoadTagTypeName"].ToString();
                        m_NewRowTemp["DelayTime"] = myVariableInfoTable.Rows[i]["DelayTime"].ToString();
                        m_NewRowTemp["LLoadLimit"] = myVariableInfoTable.Rows[i]["LLoadLimit"].ToString();
                        m_NewRowTemp["Remark"] = myVariableInfoTable.Rows[i]["Remark"].ToString();
                        m_LightLoadDataTable.Rows.Add(m_NewRowTemp);  //添加根节点
                    }                   
                }
            }
            return m_LightLoadDataTable;
        }
    }
}
