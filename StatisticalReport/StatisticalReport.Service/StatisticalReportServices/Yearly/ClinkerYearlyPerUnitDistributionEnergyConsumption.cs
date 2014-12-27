using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Yearly
{
    public class ClinkerYearlyPerUnitDistributionEnergyConsumption
    {
        private static string connectionString;
        private static TZHelper tzHelper;
        private static SqlServerDataFactory _dataFactory;

        static ClinkerYearlyPerUnitDistributionEnergyConsumption()
        {
            connectionString = ConnectionStringFactory.NXJCConnectionString;
            tzHelper = new TZHelper(connectionString);
            _dataFactory = new SqlServerDataFactory(connectionString);
        }

        public static DataTable TableQuery(string _organizeID, string _year)
        {
            string v_begin, v_end;
            v_begin = _year + "-01";
            v_end = _year + "-" + DateTime.Now.ToString("MM");    //从1月到当前月
            string organizeID = _organizeID;



            // 1. 从erp中导入v_begin=”2013-11”，v_end=”2014-03”范围内的煤粉低位发热量、点火用油和熟料强度——〉temp_erp
            string sqlQuery = "select * from [temp_erp] where [OrganizationID] = '" + _organizeID + "'";
            DataTable temp_erp = _dataFactory.Query(sqlQuery);


            // 2.	获取v_begin，v_end范围内生产线数据——〉temp_result
            DataTable temp_result;
            temp_result = tzHelper.CreateTableStructure("report_ClinkerYearlyPerUnitDistributionEnergyConsumption");
            DataTable temp1;
            DateTime v_date = DateTime.Parse(v_begin);
            string v_date_string;
            while (v_date <= DateTime.Parse(v_end))
            {
                v_date_string = v_date.ToString("yyyy-MM");
                DataRow row = temp_result.NewRow();
                row["vDate"] = v_date_string;
                row["Type"] = 1;  //	[Type] [int] NULL,    	-- 分月/累计：1--分月；2--累计	

                //////////////////////////////////////////////////
                //以下为简化计算中处理空值的麻烦，置初值
                row["CoalDust"] = 0;   //入窑煤粉量
                row["CogenerationProduction"] = 0;   //余热发电量
                row["CogenerationSelfUse"] = 0;   //余热发电自用电量
                row["ClinkerOutput"] = 0;   //熟料产量
                row["ElectricityConsumption"] = 0;   //熟料用电量
                row["Qnet"] = 0;  //煤粉低位发热量
                row["Diesel"] = 0;  //点火用油
                row["ClinkerIntensity"] = 0;  //熟料强度              

                temp1 = tzHelper.GetReportData("tz_Report", organizeID, v_date_string.Substring(0, 4), "table_ClinkerYearlyOutput", "vDate='" + v_date_string.Substring(5, 2) + "'");
                //从熟料生产线产量报表年报table_ClinkerYearlyOutput中，取对应月数据，最多一行
                foreach (DataRow dr in temp1.Rows)
                {
                    row["CoalDust"] = dr["AmounttoCoalDustConsumptionSum"];   //入窑煤粉量
                    row["CogenerationProduction"] = dr["PowerGenerationSum"];   //余热发电量
                    row["CogenerationSelfUse"] = dr["PowerSelfUseSum"];   //余热发电自用电量
                    row["ClinkerOutput"] = dr["ClinkerProductionSum"];   //熟料产量
                }
                temp1 = tzHelper.GetReportData("tz_Report", organizeID, v_date_string.Substring(0, 4), "table_ClinkerYearlyElectricity_sum", "vDate='" + v_date_string.Substring(5, 2) + "'");
                //从熟料生产线合计用电量统计月报表table_ClinkerYearlyElectricity_sum，取对应月数据，最多一行
                foreach (DataRow dr in temp1.Rows)
                {
                    row["ElectricityConsumption"] = dr["AmounttoSum"];   //熟料用电量
                }
                temp_result.Rows.Add(row);
                v_date = v_date.AddMonths(1);
            }

            // 3. temp_erp 并入temp_result
            int No1_Row;
            foreach (DataRow dr in temp_result.Rows)
            {
                No1_Row = ReportHelper.GetNoRow(temp_erp, "vDate", Convert.ToString(dr["vDate"]));
                if (No1_Row != -1)
                {
                    dr["Qnet"] = Convert.IsDBNull(temp_erp.Rows[No1_Row]["Qnet"]) ? 0 : Convert.ToDouble(temp_erp.Rows[No1_Row]["Qnet"]);  //煤粉低位发热量
                    dr["Diesel"] = Convert.IsDBNull(temp_erp.Rows[No1_Row]["Diesel"]) ? 0 : Convert.ToDouble(temp_erp.Rows[No1_Row]["Diesel"]);  //点火用油
                    dr["ClinkerIntensity"] = Convert.IsDBNull(temp_erp.Rows[No1_Row]["ClinkerIntensity"]) ? 0 : Convert.ToDouble(temp_erp.Rows[No1_Row]["ClinkerIntensity"]);  //熟料强度              
                }
            }

            return temp_result;
        }

        public static DataTable Calculate(string organizationId, string year, DataTable data)
        {
            DataTable temp_result = data;
            for (int i = temp_result.Rows.Count - 1; i >= 0; i--)
            {
                if ((int)temp_result.Rows[i]["Type"] == 2 || (int)temp_result.Rows[i]["Type"] == 3)
                    temp_result.Rows.RemoveAt(i);
            }

            //    读入编辑结果
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // 2. 求累计， 存入temp_Accumulative
            DataTable temp_Accumulative = new DataTable();
            temp_Accumulative = temp_result.Copy();
            //求累计,方法是：2行累计=2行值+1行累计，3行累计=3行值+2行累计，
            for (int i = 0; i < temp_Accumulative.Rows.Count; i++)
            {
                temp_Accumulative.Rows[i]["Type"] = 2;  //	[Type] [int] NULL,    	-- 分月/累计：1--分月；2--累计	
                if (i >= 1)    //跳过第1行
                {
                    ////////////////////////////////////////////////////加权计算部分必须先计算
                    //煤粉低位发热量累计,须加权平均
                    long v_CoalDust, v_heat;
                    v_CoalDust = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["CoalDust"]) + Convert.ToInt64(temp_Accumulative.Rows[i]["CoalDust"]);
                    v_heat = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["CoalDust"]) * Convert.ToInt64(temp_Accumulative.Rows[i - 1]["Qnet"])
                        + Convert.ToInt64(temp_Accumulative.Rows[i]["CoalDust"]) * Convert.ToInt64(temp_Accumulative.Rows[i]["Qnet"]);
                    temp_Accumulative.Rows[i]["Qnet"] = v_CoalDust > 0 ? v_heat / v_CoalDust : 0;
                    //熟料强度熟料强度,须加权平均
                    long v_ClinkerOutput;
                    double v_ClinkerIntensityTotal;
                    v_ClinkerOutput = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["ClinkerOutput"]) + Convert.ToInt64(temp_Accumulative.Rows[i]["ClinkerOutput"]);
                    v_ClinkerIntensityTotal = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["ClinkerOutput"]) * Convert.ToDouble(temp_Accumulative.Rows[i - 1]["ClinkerIntensity"])
                        + Convert.ToInt64(temp_Accumulative.Rows[i]["ClinkerOutput"]) * Convert.ToDouble(temp_Accumulative.Rows[i]["ClinkerIntensity"]);
                    temp_Accumulative.Rows[i]["ClinkerIntensity"] = v_ClinkerOutput > 0 ? v_ClinkerIntensityTotal / v_ClinkerOutput : 0;

                    ////////////////////////////////////////////////////非加权计算部分必须后计算
                    temp_Accumulative.Rows[i]["ElectricityConsumption"] = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["ElectricityConsumption"])
                        + Convert.ToInt64(temp_Accumulative.Rows[i]["ElectricityConsumption"]);  //熟料用电量
                    temp_Accumulative.Rows[i]["CoalDust"] = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["CoalDust"])
                        + Convert.ToInt64(temp_Accumulative.Rows[i]["CoalDust"]);  //入窑煤粉量
                    temp_Accumulative.Rows[i]["CogenerationProduction"] = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["CogenerationProduction"])
                        + Convert.ToInt64(temp_Accumulative.Rows[i]["CogenerationProduction"]);  //余热发电量
                    temp_Accumulative.Rows[i]["CogenerationSelfUse"] = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["CogenerationSelfUse"])
                        + Convert.ToInt64(temp_Accumulative.Rows[i]["CogenerationSelfUse"]);  //余热发电自用电量
                    temp_Accumulative.Rows[i]["ClinkerOutput"] = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["ClinkerOutput"])
                        + Convert.ToInt64(temp_Accumulative.Rows[i]["ClinkerOutput"]);  //熟料产量
                    temp_Accumulative.Rows[i]["Diesel"] = Convert.ToDecimal(temp_Accumulative.Rows[i - 1]["Diesel"])
                        + Convert.ToDecimal(temp_Accumulative.Rows[i]["Diesel"]); //点火用油
                }
            }


            // 添加累计行
            DataRow drLabelCount =  temp_result.NewRow();
            drLabelCount["vDate"] = "累计：";
            drLabelCount["Type"] = 3;
            temp_result.Rows.Add(drLabelCount);

            // 3.	合并结果 temp_result
            temp_result.Merge(temp_Accumulative);

            // 4.	从组织表中获得海拔修正v_CoefficientAltitude
            //[CoefficientAltitude] decimal(8, 4)  DEFAULT (0) NULL,     --  海拔修正系数
            double v_CoefficientAltitude;
            DataTable temp_Organization = _dataFactory.Query("select * from System_Organization where OrganizationID='" + organizationId + "'");
            if (temp_Organization.Rows.Count > 0) v_CoefficientAltitude = Convert.IsDBNull(temp_Organization.Rows[0]["CoefficientAltitude"]) ?
                0 : Convert.ToDouble(temp_Organization.Rows[0]["CoefficientAltitude"]);
            else v_CoefficientAltitude = 0;

            // 5.	求熟料强度修正系数、余热供电量
            foreach (DataRow dr in temp_result.Rows)
            {
                if ((int)dr["Type"] == 3)
                    continue;
                // 	求熟料强度修正系数
                if (Convert.ToSingle(dr["ClinkerIntensity"]) > 0.0)
                    dr["ClinkerIntensityCorrectionFactor"] = Math.Pow(52.5 / Convert.ToDouble(dr["ClinkerIntensity"]), Convert.ToDouble(1) / 4);
                else dr["ClinkerIntensityCorrectionFactor"] = 0;
                // 余热供电量
                dr["CogenerationSupply"] = Convert.ToInt64(dr["CogenerationProduction"]) - Convert.ToInt64(dr["CogenerationSelfUse"]);
            }

            // 6.	求下列项
            foreach (DataRow dr in temp_result.Rows)
            {
                if ((int)dr["Type"] == 3)
                    continue;
                //熟料综合电耗  =  熟料用电量/熟料产量
                dr["Clinker_ComprehensiveElectricityConsumption"] = Convert.ToInt64(dr["ClinkerOutput"]) > 0 ?
                    Convert.ToDouble(dr["ElectricityConsumption"]) / Convert.ToDouble(dr["ClinkerOutput"]) : 0;
                // 可比熟料综合电耗  =  熟料综合电耗*海拔修正系数*熟料强度修正系数
                dr["Clinker_ComparableComprehensiveElectricityConsumption"] =
                    Convert.ToDouble(dr["Clinker_ComprehensiveElectricityConsumption"]) * v_CoefficientAltitude *
                    Convert.ToDouble(dr["ClinkerIntensityCorrectionFactor"]);
                // 熟料煤耗  = 入窑煤粉量*1000*煤粉发热量/29307.6/熟料产量
                dr["Clinker_CoalDustConsumption"] = Convert.ToInt64(dr["ClinkerOutput"]) > 0 ?
                   (Convert.ToDouble(dr["CoalDust"]) * 1000 * Convert.ToDouble(dr["Qnet"]) / 29307.6) / Convert.ToDouble(dr["ClinkerOutput"]) : 0;
                // 熟料油耗  = 点火用油*1000*1.4571/熟料产量
                dr["Clinker_DieselConsumption"] = Convert.ToInt64(dr["ClinkerOutput"]) > 0 ?
                   (Convert.ToDouble(dr["Diesel"]) * 1000 * 1.4571) / Convert.ToDouble(dr["ClinkerOutput"]) : 0;
                // 上网电量折标  =  余热供电量*0.404/熟料产量
                dr["CogenerationSupplyCorrection"] = Convert.ToInt64(dr["ClinkerOutput"]) > 0 ?
                   (Convert.ToDouble(dr["CogenerationSupply"]) * 0.404) / Convert.ToDouble(dr["ClinkerOutput"]) : 0;
                // 熟料综合煤耗  =  煤耗+油耗-余热供电折标
                dr["Clinker_ComprehensiveCoalDustConsumption"] = Convert.ToDouble(dr["Clinker_CoalDustConsumption"])
                    + Convert.ToDouble(dr["Clinker_DieselConsumption"]) - Convert.ToDouble(dr["CogenerationSupplyCorrection"]);
                // 可比熟料综合煤耗  =  熟料综合煤耗*海拔修正系数*熟料强度修正系数
                dr["Clinker_ComparableComprehensiveCoalDustConsumption"] = Convert.ToDouble(dr["Clinker_ComprehensiveCoalDustConsumption"])
                    * v_CoefficientAltitude * Convert.ToDouble(dr["ClinkerIntensityCorrectionFactor"]);
                // 熟料综合能耗  =  熟料综合电耗*0.1229+熟料综合煤耗
                dr["Clinker_ComprehensiveEnergyConsumption"] = Convert.ToDouble(dr["Clinker_ComprehensiveElectricityConsumption"])
                    * 0.1229 + Convert.ToDouble(dr["Clinker_ComprehensiveCoalDustConsumption"]);
                // 可比熟料综合能耗  =  熟料综合能耗*海拔修正系数*熟料强度修正系数
                dr["Clinker_ComparableComprehensiveEnergyConsumption"] = Convert.ToDouble(dr["Clinker_ComprehensiveEnergyConsumption"])
                    * v_CoefficientAltitude * Convert.ToDouble(dr["ClinkerIntensityCorrectionFactor"]);
            }

            return temp_result;

        }

        public static string Save(string organizationId, string year, DataTable data)
        {
            string keyId = "";

            // 查询是否已存在此报表
            Query query = new Query("tz_Report");
            query.AddCriterion("OrganizationID", organizationId, SqlServerDataAdapter.Infrastruction.CriteriaOperator.Equal);
            query.AddCriterion("Date", year, SqlServerDataAdapter.Infrastruction.CriteriaOperator.Equal);
            query.AddCriterion("TableName", "report_ClinkerYearlyPerUnitDistributionEnergyConsumption", SqlServerDataAdapter.Infrastruction.CriteriaOperator.Equal);

            DataTable result = _dataFactory.Query(query);

            if (result.Rows.Count > 0)
            {
                keyId = result.Rows[0]["KeyID"].ToString();
            }
            else
            {
                keyId = Guid.NewGuid().ToString();

                string insertCommand = @"INSERT INTO [dbo].[tz_Report]([OrganizationID],[ReportID],[ReportName],[Date],[TableName],[KeyID]) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')";
                string command = string.Format(insertCommand, organizationId, Guid.NewGuid().ToString(), "熟料生产线可比单位产品能耗分析报表", year, "report_ClinkerYearlyPerUnitDistributionEnergyConsumption", keyId);
                _dataFactory.ExecuteSQL(command);

            }

            // 删除当前保存的结果
            Delete delete = new Delete("report_ClinkerYearlyPerUnitDistributionEnergyConsumption");
            delete.AddCriterions("KeyID", keyId, SqlServerDataAdapter.Infrastruction.CriteriaOperator.Equal);
            _dataFactory.Remove(delete);

            // 更新数据
            for (int i = data.Rows.Count - 1; i >= 0; i--)
            {
                //// 删除累计提示行
                //if ((int)data.Rows[i]["Type"] == 3)
                //{
                //    data.Rows.RemoveAt(i);
                //    continue;
                //}

                // 建立ID
                data.Rows[i]["ID"] = Guid.NewGuid();
                // 更新KeyID
                data.Rows[i]["KeyID"] = keyId;
            }

            _dataFactory.Save("report_ClinkerYearlyPerUnitDistributionEnergyConsumption", data);

            return "保存成功";
        }

        public static DataTable CreateTableStructure(string myDataTableName)
        {
            string queryString = @"SELECT TOP 1 * FROM " + myDataTableName;
            DataTable sourceTable = _dataFactory.Query(queryString);
            DataTable result = sourceTable.Clone();

            return result;
        }
    }
}
