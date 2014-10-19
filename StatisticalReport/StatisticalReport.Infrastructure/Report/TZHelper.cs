using SqlServerDataAdapter;
using SqlServerDataAdapter.Infrastruction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Infrastructure.Report
{
    public class TZHelper
    {
        private ISqlServerDataFactory _dataFactory;

        public TZHelper(string connString)
        {
            _dataFactory = new SqlServerDataFactory(connString);
        }
        /// <summary>
        /// Clone表结构
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable CreateTableStructure(string tableName)
        {
            string queryString = @"SELECT TOP 1 * FROM " + tableName;
            DataTable sourceTable = _dataFactory.Query(queryString);
            DataTable result = sourceTable.Clone();

            return result;
        }
        /// <summary>
        /// 根据水泥品种获得折合系数
        /// </summary>
        /// <param name="CementTypes"></param>
        /// <returns></returns>
        public double GetConvertCoefficient(string CementTypes)
        {
            Query query = new Query("CementTypesAndConvertCoefficient");
            DataTable table_zhxs = _dataFactory.Query(query);
            DataColumn[] Key = { table_zhxs.Columns["CementTypes"] };
            table_zhxs.PrimaryKey = Key;
            DataRow row = table_zhxs.Rows.Find(CementTypes);
            double result = Convert.ToDouble(row["ConvertCoefficient"]);
            return result;
        }
        /// <summary>
        /// 获得交接班情况
        /// </summary>
        /// <param name="organizationID"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public IDictionary<string, string> GetTeamDictionary(string organizationID, int year, int month, int day)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();

            string sqlString = "SELECT Shifts, WorkingTeam FROM shift_WorkingTeamShiftLog WHERE OrganizationID=" + organizationID + " AND YEAR(ShiftDate)=" + year + " AND MONTH(ShiftDate)=" + month +
                    " AND DAY(ShiftDate)=" + day;

            DataTable dt = _dataFactory.Query(sqlString);
            foreach (DataRow item in dt.Rows)
            {
                if (!result.Keys.Contains(item["Shifts"].ToString().Trim()))
                {
                    result.Add(item["Shifts"].ToString().Trim(), item["WorkingTeam"].ToString().Trim());
                }
            }

            return result;
        }
        /// <summary>
        /// 获得报表详细信息
        /// </summary>
        /// <param name="tzName"></param>
        /// <param name="organizationID"></param>
        /// <param name="date"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable GetReportData(string tzName, string organizationID, string date, string tableName)
        {
            string keyID = GetKeyIDFrom(tzName, organizationID, date, tableName);

            DataTable result;
            if (keyID != "")
            {
                result = GetDetailData(tableName, keyID);
            }
            else
            {
                result = CreateTableStructure(tableName);
            }

            return result;
        }
        /// <summary>
        /// 获得报表详细信息
        /// </summary>
        /// <param name="tzName"></param>
        /// <param name="organizationID"></param>
        /// <param name="date"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable GetReportData(string tzName, string organizationID, string date, string tableName, string criterionString)
        {
            string keyID = GetKeyIDFrom(tzName, organizationID, date, tableName);

            DataTable result;
            if (keyID != "")
            {
                string sqlString = "SELECT * FROM " + tableName + " WHERE KeyID='" + keyID + "' AND " + criterionString;
                result = GetDetailData(sqlString);
            }
            else
            {
                result = CreateTableStructure(tableName);
            }

            return result;
        }
        /// <summary>
        /// 获得报表指定字段的详细信息
        /// </summary>
        /// <param name="tzName"></param>
        /// <param name="organizationID"></param>
        /// <param name="date"></param>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public DataTable GetReportData(string tzName, string organizationID, string date, string tableName, string[] columns)
        {
            string keyID = GetKeyIDFrom(tzName, organizationID, date, tableName);

            DataTable result;
            if (keyID != "")
            {
                result = GetDetailData(tableName, keyID, columns);
            }
            else
            {
                result = CreateTableStructure(tableName);
            }

            return result;
        }
        /// <summary>
        /// 获得峰谷平电价，在电价字典中1表示峰，2表示谷，3表示平
        /// </summary>
        /// <param name="organizationID"></param>
        /// <returns></returns>
        public Dictionary<int, decimal> GetPeakValleyFlatElectrovalence(string organizationID)
        {
            Dictionary<int, decimal> result = new Dictionary<int, decimal>();

            Query query = new Query("system_PeakValleyFlatElectrovalence");
            query.AddCriterion("OrganizationID", organizationID, CriteriaOperator.Equal);
            DataTable dt = _dataFactory.Query(query);
            if (dt.Rows.Count != 0)
            {
                decimal peak = (decimal)dt.Rows[0]["PeakElectrovalence"];
                decimal valley = (decimal)dt.Rows[0]["ValleyElectrovalence"];
                decimal flat = (decimal)dt.Rows[0]["FlatElectrovalence"];
                result.Add(1, peak);
                result.Add(2, valley);
                result.Add(3, flat);
            }

            return result;
        }
        /// <summary>
        /// 获得KeyID
        /// </summary>
        /// <param name="tzName"></param>
        /// <param name="organizationID"></param>
        /// <param name="date"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string GetKeyIDFrom(string tzName, string organizationID, string date, string tableName)
        {
            Query query = new Query(tzName);
            query.AddCriterion("OrganizationID", organizationID, CriteriaOperator.Equal);
            query.AddCriterion("Date", date, CriteriaOperator.Equal);
            query.AddCriterion("TableName", tableName, CriteriaOperator.Equal);

            DataTable dt = _dataFactory.Query(query);
            string result;
            if (dt.Rows.Count != 0)
            {
                result = dt.Rows[0]["KeyID"].ToString();
            }
            else
            {
                result = "";
            }

            return result;
        }
        /// <summary>
        /// 获得报表详细信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="keyID"></param>
        /// <returns></returns>
        private DataTable GetDetailData(string tableName, string keyID)
        {
            Query query = new Query(tableName);
            query.AddCriterion("KeyID", keyID, CriteriaOperator.Equal);

            DataTable result = _dataFactory.Query(query);

            return result;
        }
        /// <summary>
        /// 获得报表具体字段的详细信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="keyID"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private DataTable GetDetailData(string tableName, string keyID, string[] columns)
        {
            ComplexQuery cmquery = new ComplexQuery();
            foreach (string item in columns)
            {
                cmquery.AddNeedField(tableName, item);
            }
            cmquery.AddCriterion("KeyID", keyID, CriteriaOperator.Equal);

            DataTable result = _dataFactory.Query(cmquery);

            return result;
        }

        /// <summary>
        /// 根据条件语句获得报表详细信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="keyID"></param>
        /// <returns></returns>
        private DataTable GetDetailData(string sqlString)
        {
            DataTable result = _dataFactory.Query(sqlString);

            return result;
        }
    }
}
