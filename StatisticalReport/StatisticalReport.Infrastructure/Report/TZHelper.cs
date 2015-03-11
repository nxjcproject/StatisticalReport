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
        public decimal GetConvertCoefficient(string CementTypes)
        {
            decimal result;
            Query query = new Query("system_CementTypesAndConvertCoefficient");
            DataTable table_zhxs = _dataFactory.Query(query);
            //DataColumn[] Key = { table_zhxs.Columns["CementTypes"] };
            //table_zhxs.PrimaryKey = Key;
            //DataRow row = table_zhxs.Rows.Find(CementTypes);
            DataRow[] rows = table_zhxs.Select("CementTypes='" + CementTypes + "'");
            if (rows.Count() == 0)
            {
                throw new Exception("未找到水泥品种"+CementTypes);
            }
            else
            {
                result = Convert.ToDecimal(rows[0]["ConvertCoefficient"]);
            }
            //decimal result = Convert.ToDecimal(row["ConvertCoefficient"]);
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
            string[] orgaString = organizationID.Split('_');
            if (orgaString.Count() >= 5)
            {
                organizationID = orgaString[0] + "_" + orgaString[1] + "_" + orgaString[2] + "_" + orgaString[3];
            }
            IDictionary<string, string> result = new Dictionary<string, string>();

            string sqlString = "SELECT Shifts, WorkingTeam FROM shift_WorkingTeamShiftLog WHERE OrganizationID='" + organizationID + "' AND YEAR(ShiftDate)=" + year + " AND MONTH(ShiftDate)=" + month +
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
        /// <param name="tzName">主表名称</param>
        /// <param name="organizationID">组织机构代码</param>
        /// <param name="date">日期</param>
        /// <param name="tableName">从表名称</param>
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

        /// <summary>
        /// 返回组织结构表，填加了熟料生产线一级和水泥磨生产线一级。
        /// </summary>
        /// <param name="organizationId">organizationId可以为总公司，分公司，分厂级别</param>
        /// <returns></returns>
        public DataTable GetCompanyLevelTable(string organizationId)
        {
            const int groupLength = 2;
            const int companyLength = 3;
            const int factoryLength = 4;
            string[] levelArray = organizationId.Split('_');
            int topLevel = levelArray.Count();
            // 生产线类别，有顺序
            string[] typeArray = new string[] { "熟料", "水泥磨" };
            string[] typeDescription = new string[] { "熟料生产线", "水泥磨" };

            // 获取原始数据表
            DataTable originalTable = _dataFactory.Query("SELECT [LevelCode], [OrganizationID], [Type], [Name] FROM [system_Organization] WHERE [OrganizationID] LIKE '" + organizationId + "%'");
            //获得传入的组织机构层次码长度(公司级的认为是3，分厂级的认为是5)
            //int topLevel = originalTable.Select("OrganizationID='" + organizationId+"'")[0]["LevelCode"].ToString().Trim().Length;
            // 结果表
            DataTable resultTable = originalTable.Clone();
            //当传入总公司级别的organizationId         

            //当传入公司级别的organizationId
            if (groupLength == topLevel || companyLength == topLevel)
            {
                // 添加合计行
                DataRow ammountRow = resultTable.NewRow();
                if (groupLength == topLevel)//集团级别
                {
                    ammountRow["LevelCode"] = "P01";
                    ammountRow["Name"] = "宁夏建材";
                    ammountRow["OrganizationID"] = "zc_nxjc";
                    resultTable.Rows.Add(ammountRow);
                }
                else
                {
                    ammountRow["LevelCode"] = "P";
                    ammountRow["Name"] = "";
                }
                // 遍历公司级（层次码长度为3的认为是公司级）
                DataRow[] companyRows = originalTable.Select("LEN(TRIM(LevelCode)) = 3");
                for (int i = 0; i < companyRows.Length; i++)
                {
                    DataRow companyRow = resultTable.NewRow();
                    companyRow.ItemArray = companyRows[i].ItemArray;                // 复制行
                    companyRow["LevelCode"] = ammountRow["LevelCode"] + (i + 1).ToString("#00");      // 编辑层次码

                    resultTable.Rows.Add(companyRow);

                    // 遍历当前公司下的分厂级（层次码以公司层次码开头，并且长度为5的认为是分厂级）
                    DataRow[] factoryRows = originalTable.Select("LEN(TRIM(LevelCode)) = 5 AND LevelCode LIKE '" + companyRows[i]["LevelCode"] + "%'");
                    for (int j = 0; j < factoryRows.Length; j++)
                    {
                        DataRow factoryRow = resultTable.NewRow();
                        factoryRow.ItemArray = factoryRows[j].ItemArray;
                        factoryRow["LevelCode"] = companyRow["LevelCode"] + (j + 1).ToString("#00");

                        resultTable.Rows.Add(factoryRow);

                        // 遍历产线类型（按数组顺序）
                        for (int k = 0; k < typeArray.Length; k++)
                        {
                            DataRow typeRow = resultTable.NewRow();
                            typeRow["LevelCode"] = factoryRow["LevelCode"] + (k + 1).ToString("#00");
                            typeRow["Name"] = typeDescription[k];

                            resultTable.Rows.Add(typeRow);

                            // 遍历当前分厂下的产线级（层次码以分厂层次码开头，并且有产线类型的认为是产线级）
                            DataRow[] productLineRows = originalTable.Select("Type = '" + typeArray[k] + "' AND LevelCode LIKE '" + factoryRows[j]["LevelCode"] + "%'");
                            for (int l = 0; l < productLineRows.Length; l++)
                            {
                                DataRow productLineRow = resultTable.NewRow();
                                productLineRow.ItemArray = productLineRows[l].ItemArray;
                                productLineRow["LevelCode"] = typeRow["LevelCode"] + (l + 1).ToString("#00");

                                resultTable.Rows.Add(productLineRow);
                            }
                        }
                    }
                }
            }
            //当传入分厂级别的organizationId，此时只有一个分长不用再添加合计行
            if (factoryLength == topLevel)
            {
                // 遍历当前公司下的分厂级（层次码以公司层次码开头，并且长度为5的认为是分厂级）
                DataRow[] factoryRows = originalTable.Select("LEN(TRIM(LevelCode)) = 5");
                for (int i = 0; i < factoryRows.Length; i++)
                {
                    DataRow factoryRow = resultTable.NewRow();
                    factoryRow.ItemArray = factoryRows[i].ItemArray;                // 复制行
                    factoryRow["LevelCode"] = "P" + (i + 1).ToString("#00");      // 编辑层次码

                    resultTable.Rows.Add(factoryRow);
                    // 遍历产线类型（按数组顺序）
                    for (int k = 0; k < typeArray.Length; k++)
                    {
                        DataRow typeRow = resultTable.NewRow();
                        typeRow["LevelCode"] = factoryRow["LevelCode"] + (k + 1).ToString("#00");
                        typeRow["Name"] = typeDescription[k];

                        resultTable.Rows.Add(typeRow);

                        // 遍历当前分厂下的产线级（层次码以分厂层次码开头，并且有产线类型的认为是产线级）
                        DataRow[] productLineRows = originalTable.Select("Type = '" + typeArray[k] + "' AND LevelCode LIKE '" + factoryRows[i]["LevelCode"] + "%'");
                        for (int l = 0; l < productLineRows.Length; l++)
                        {
                            DataRow productLineRow = resultTable.NewRow();
                            productLineRow.ItemArray = productLineRows[l].ItemArray;
                            productLineRow["LevelCode"] = typeRow["LevelCode"] + (l + 1).ToString("#00");

                            resultTable.Rows.Add(productLineRow);
                        }
                    }
                }
            }
            return resultTable;
        }

        /// <summary>
        /// 获得公式报表详细信息
        /// </summary>
        /// <param name="tzName"></param>
        /// <param name="organizationID"></param>
        /// <param name="date"></param>
        /// <param name="tableName"></param>
        /// <param name="formulaKeyID"></param>
        /// <returns></returns>
        public DataTable GetFormulaData(string tzName, string organizationID, string date, string tableName, string formulaKeyID)
        {
            string keyID = GetKeyIDFrom(tzName, organizationID, date, tableName, formulaKeyID);

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
        /// 获得公式KeyID
        /// </summary>
        /// <param name="tzName"></param>
        /// <param name="organizationID"></param>
        /// <param name="date"></param>
        /// <param name="tableName"></param>
        /// <param name="formulaKeyID"></param>
        /// <returns></returns>
        private string GetKeyIDFrom(string tzName, string organizationID, string date, string tableName, string formulaKeyID)
        {
            Query query = new Query(tzName);
            query.AddCriterion("OrganizationID", organizationID, CriteriaOperator.Equal);
            query.AddCriterion("Date", date, CriteriaOperator.Equal);
            query.AddCriterion("TableName", tableName, CriteriaOperator.Equal);
            query.AddCriterion("formulaKeyID", formulaKeyID, CriteriaOperator.Equal);

            DataTable dt = _dataFactory.Query(query);
            string result;
            if (dt.Rows.Count != 0)
            {
                result = dt.Rows[0]["KeyID"].ToString();
            }
            else
            {
                result = "";////////
            }

            return result;
        }
    }
}
