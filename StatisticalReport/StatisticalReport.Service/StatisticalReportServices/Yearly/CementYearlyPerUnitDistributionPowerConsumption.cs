using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalReport.Service.StatisticalReportServices.Yearly
{
    public class CementYearlyPerUnitDistributionPowerConsumption
    {
        private static string connectionString;
        private static TZHelper tzHelper;
        private static SqlServerDataFactory _dataFactory;

        static CementYearlyPerUnitDistributionPowerConsumption()
        {
            connectionString = ConnectionStringFactory.NXJCConnectionString;
            tzHelper = new TZHelper(connectionString);
            _dataFactory = new SqlServerDataFactory(connectionString);
        }

        public static DataTable TableQuery(string cementOrganizationId, string clinkerOrganizationId, string _year)
        {
            //////////////////////////////////////////////////////////////////
            //入口参数
            string v_begin = _year + "-01";                                 // 从一月份开始
            string v_end = "";
            if (int.Parse(_year) >= DateTime.Now.Year)
                v_end = _year + "-" + DateTime.Now.ToString("MM");          // 到当前月结束
            else
                v_end = _year + "-12";

            // 1．	创建temp_result
            DataTable temp_result = tzHelper.CreateTableStructure("report_CementYearlyPerUnitDistributionPowerConsumption");

            DateTime v_date = DateTime.Parse(v_begin);

            while (v_date <= DateTime.Parse(v_end))
            {
                DataRow row = temp_result.NewRow();
                row["vDate"] = v_date.ToString("yyyy-MM");
                row["Type"] = 1;  //	[Type] [int] NULL,    	-- 分月/累计：1--分月；2--累计	

                //////////////////////////////////////////////////
                // 以下为简化计算中处理空值的麻烦，置初值
                row["ElectricityConsumption"] = 0;                              // 用电量
                row["Clinker_ComprehensiveElectricityConsumption"] = 0;         // 熟料综合电耗
                row["Clinker_ComparableComprehensiveCoalDustConsumption"] = 0;  // 可比熟料综合煤耗
                row["CementProductionSum"] = 0;                                 // 水泥产量
                row["ClinkerConsumptionSum"] = 0;                               // 熟料消耗量
                row["CementIntensity"] = 0;                                     // 各品种水泥平均强度

                temp_result.Rows.Add(row);
                v_date = v_date.AddMonths(1);
            }

            //  2．读入信息	 
            int No1_Row;

            //从水泥（分品种）粉磨产量及消耗统计年报表,读入水泥产量、熟料消耗量
            //需增加月合计
            DataTable temp = tzHelper.GetReportData("tz_Report", cementOrganizationId, v_begin.Substring(0, 4), "table_CementMillYearlyOutput");
            DataTable temp1 = ReportHelper.MyTotalOn(temp, "vDate", "CementProductionSum,ClinkerConsumptionSum");   //月合计

            foreach (DataRow dr in temp1.Rows)
            {
                No1_Row = ReportHelper.GetNoRow(temp_result, "vDate", v_begin.Substring(0, 5) + Convert.ToString(dr["vDate"]));    //只有"月"的日期，变成“2014-01”
                if (No1_Row != -1)
                {
                    temp_result.Rows[No1_Row]["CementProductionSum"] =
                        Convert.IsDBNull(dr["CementProductionSum"]) ? 0 : Convert.ToDouble(dr["CementProductionSum"]);   // 水泥产量
                    temp_result.Rows[No1_Row]["ClinkerConsumptionSum"] =
                        Convert.IsDBNull(dr["ClinkerConsumptionSum"]) ? 0 : Convert.ToDouble(dr["ClinkerConsumptionSum"]);   //熟料消耗量
                }
            }
            foreach (DataRow dr in temp.Rows)     //求强度*产量，用字段CementProductionFirstShift(甲班产量)存储
            {
                // 从水泥品种及折合系数system_CementTypesAndConvertCoefficient中获得水泥强度
                double v_Intensity;
                DataTable temp_CementTypesAndConvertCoefficient = _dataFactory.Query("select * from System_CementTypesAndConvertCoefficient where CementTypes='" + Convert.ToString(dr["CementTypes"]) + "'");
                if (temp_CementTypesAndConvertCoefficient.Rows.Count > 0) v_Intensity = Convert.IsDBNull(temp_CementTypesAndConvertCoefficient.Rows[0]["Intensity"]) ?
                    0 : Convert.ToDouble(temp_CementTypesAndConvertCoefficient.Rows[0]["Intensity"]);
                else v_Intensity = 0;

                dr["CementProductionFirstShift"] = Convert.ToDouble(dr["CementProductionSum"]) * v_Intensity;  //求强度*产量，用字段CementProductionFirstShift(甲班产量)存储
            }
            temp1 = ReportHelper.MyTotalOn(temp, "vDate", "CementProductionSum,CementProductionFirstShift");   //月合计
            foreach (DataRow dr in temp1.Rows)
            {
                No1_Row = ReportHelper.GetNoRow(temp_result, "vDate", v_begin.Substring(0, 5) + Convert.ToString(dr["vDate"]));    //只有"月"的日期，变成“2014-01”
                if (No1_Row != -1)
                {
                    temp_result.Rows[No1_Row]["CementIntensity"] =
                        Convert.ToDouble(dr["CementProductionSum"]) > 0 ? Convert.ToDouble(dr["CementProductionFirstShift"]) / Convert.ToDouble(dr["CementProductionSum"]) : 0;    //各品种水泥平均强度
                }
            }


            //从水泥（分品种）粉磨用电量统计年报表,读入用电量
            //需增加月合计
            temp = tzHelper.GetReportData("tz_Report", cementOrganizationId, v_begin.Substring(0, 4), "table_CementMillYearlyElectricity_sum");
            temp1 = ReportHelper.MyTotalOn(temp, "vDate", "AmounttoSum");   //月合计
            foreach (DataRow dr in temp1.Rows)
            {
                No1_Row = ReportHelper.GetNoRow(temp_result, "vDate", v_begin.Substring(0, 5) + Convert.ToString(dr["vDate"]));    //只有"月"的日期，变成“2014-01”
                if (No1_Row != -1)
                {
                    temp_result.Rows[No1_Row]["ElectricityConsumption"] =
                        Convert.IsDBNull(dr["AmounttoSum"]) ? 0 : Convert.ToDouble(dr["AmounttoSum"]);   // 用电量
                }
            }

            //从熟料生产线单位产品能耗表,读入熟料综合电耗、可比熟料综合煤耗
            if (clinkerOrganizationId != "")
            {
                temp1 = tzHelper.GetReportData("tz_Report", clinkerOrganizationId, v_begin.Substring(0, 4), "report_ClinkerYearlyPerUnitDistributionEnergyConsumption");
                foreach (DataRow dr in temp1.Rows)
                {
                    No1_Row = ReportHelper.GetNoRow(temp_result, "vDate", Convert.ToString(dr["vDate"]));
                    if (No1_Row != -1)
                    {
                        temp_result.Rows[No1_Row]["Clinker_ComprehensiveElectricityConsumption"] =
                            Convert.IsDBNull(dr["Clinker_ComprehensiveElectricityConsumption"]) ? 0 : Convert.ToDouble(dr["Clinker_ComprehensiveElectricityConsumption"]);   //熟料综合电耗
                        temp_result.Rows[No1_Row]["Clinker_ComparableComprehensiveCoalDustConsumption"] =
                            Convert.IsDBNull(dr["Clinker_ComparableComprehensiveCoalDustConsumption"]) ? 0 : Convert.ToDouble(dr["Clinker_ComparableComprehensiveCoalDustConsumption"]);   //可比熟料综合煤耗
                    }
                }
            }

            ///////////////////////////////////////////////////////////////////
            //返回结果
            if (temp1.Rows.Count == 0 && clinkerOrganizationId != "")
            {
                //return ("无法从熟料生产线可比单位产品能耗表中获取熟料综合电耗、可比熟料综合煤耗等信息！");
                //return(temp_result) 只显示月份和读取的8列
            }
            else
            {
                //return(temp_result) 只显示月份和读取的8列
            }

            return temp_result;
        }


        public static DataTable Read(string cementOrganizationId, string clinkerOrganizationId, string _year)
        {
            // 如果已有保存数据，读取保存数据
            return tzHelper.GetReportData("tz_Report", cementOrganizationId, _year, "report_CementYearlyPerUnitDistributionPowerConsumption");
        }

        public static DataTable Calculate(string organiztionId, string clinkerOrganizationId, string year, DataTable data)
        {
            //////////////////////////////////////////////////////////////////
            //入口参数
            //熟料生产线id,如果用户未选相关“熟料生产线”，则 Clinker_organizeID = ""
            string Clinker_organizeID = clinkerOrganizationId;      //熟料生产线id

            // 1. 读入编辑结果。
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
                    //熟料综合电耗累计,须加权平均
                    long v_Clinker;
                    double v_Electricity;
                    v_Clinker = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["ClinkerConsumptionSum"]) + Convert.ToInt64(temp_Accumulative.Rows[i]["ClinkerConsumptionSum"]);
                    v_Electricity = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["ClinkerConsumptionSum"]) * Convert.ToDouble(temp_Accumulative.Rows[i - 1]["Clinker_ComprehensiveElectricityConsumption"])
                        + Convert.ToInt64(temp_Accumulative.Rows[i]["ClinkerConsumptionSum"]) * Convert.ToDouble(temp_Accumulative.Rows[i]["Clinker_ComprehensiveElectricityConsumption"]);
                    temp_Accumulative.Rows[i]["Clinker_ComprehensiveElectricityConsumption"] = v_Clinker > 0 ? v_Electricity / v_Clinker : 0;
                    //可比熟料综合煤耗,须加权平均
                    double v_CoalDust;
                    v_CoalDust = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["ClinkerConsumptionSum"]) * Convert.ToDouble(temp_Accumulative.Rows[i - 1]["Clinker_ComparableComprehensiveCoalDustConsumption"])
                        + Convert.ToInt64(temp_Accumulative.Rows[i]["ClinkerConsumptionSum"]) * Convert.ToDouble(temp_Accumulative.Rows[i]["Clinker_ComparableComprehensiveCoalDustConsumption"]);
                    temp_Accumulative.Rows[i]["Clinker_ComparableComprehensiveCoalDustConsumption"] = v_Clinker > 0 ? v_CoalDust / v_Clinker : 0;
                    //各品种水泥平均强度,须加权平均
                    long v_Cement;
                    double v_CementIntensityTotal;
                    v_Cement = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["CementProductionSum"]) + Convert.ToInt64(temp_Accumulative.Rows[i]["CementProductionSum"]);
                    v_CementIntensityTotal = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["CementProductionSum"]) * Convert.ToDouble(temp_Accumulative.Rows[i - 1]["CementIntensity"])
                        + Convert.ToInt64(temp_Accumulative.Rows[i]["CementProductionSum"]) * Convert.ToDouble(temp_Accumulative.Rows[i]["CementIntensity"]);
                    temp_Accumulative.Rows[i]["CementIntensity"] = v_Cement > 0 ? v_CementIntensityTotal / v_Cement : 0;

                    ////////////////////////////////////////////////////非加权计算部分必须后计算
                    temp_Accumulative.Rows[i]["ElectricityConsumption"] = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["ElectricityConsumption"])
                        + Convert.ToInt64(temp_Accumulative.Rows[i]["ElectricityConsumption"]);  //用电量
                    temp_Accumulative.Rows[i]["CementProductionSum"] = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["CementProductionSum"])
                        + Convert.ToInt64(temp_Accumulative.Rows[i]["CementProductionSum"]);  //水泥产量
                    temp_Accumulative.Rows[i]["ClinkerConsumptionSum"] = Convert.ToInt64(temp_Accumulative.Rows[i - 1]["ClinkerConsumptionSum"])
                        + Convert.ToInt64(temp_Accumulative.Rows[i]["ClinkerConsumptionSum"]);  //熟料消耗量
                }
            }


            // 添加累计行
            DataRow drLabelCount = temp_result.NewRow();
            drLabelCount["vDate"] = "累计：";
            drLabelCount["Type"] = 3;
            temp_result.Rows.Add(drLabelCount);

            // 3.	合并结果 temp_result
            temp_result.Merge(temp_Accumulative);

            // 4.	从组织表中获得海拔修正v_CoefficientAltitude
            //[CoefficientAltitude] decimal(8, 4)  DEFAULT (0) NULL,     --  海拔修正系数
            double v_CoefficientAltitude;
            DataTable temp_Organization = _dataFactory.Query("select * from System_Organization where OrganizationID='" + Clinker_organizeID + "'");
            if (temp_Organization.Rows.Count > 0) v_CoefficientAltitude = Convert.IsDBNull(temp_Organization.Rows[0]["CoefficientAltitude"]) ?
                0 : Convert.ToDouble(temp_Organization.Rows[0]["CoefficientAltitude"]);
            else v_CoefficientAltitude = 0;

            // 5.	求下列项
            foreach (DataRow dr in temp_result.Rows)
            {
                if ((int)dr["Type"] == 3)
                    continue;

                // 熟料平均配比=  熟料消耗量/水泥产量
                dr["ClinkerMatching"] = Convert.ToInt64(dr["CementProductionSum"]) > 0 ?
                    Convert.ToDouble(dr["ClinkerConsumptionSum"]) / Convert.ToDouble(dr["CementProductionSum"]) : 0;
                // 	求水泥强度修正系数
                if (Convert.ToSingle(dr["CementIntensity"]) > 0.0)
                    dr["CementIntensityCorrectionFactor"] = Math.Pow(42.5 / Convert.ToDouble(dr["CementIntensity"]), Convert.ToDouble(1) / 4);
                else dr["CementIntensityCorrectionFactor"] = 0;
                // 分步电耗=  用电量/水泥产量
                dr["DistributionPowerConsumption"] = Convert.ToInt64(dr["CementProductionSum"]) > 0 ?
                    Convert.ToDouble(dr["ElectricityConsumption"]) / Convert.ToDouble(dr["CementProductionSum"]) : 0;
                // 水泥综合电耗 =(熟料综合电耗*水泥用熟料量+用电量)/水泥产量
                dr["Cement_ComprehensiveElectricityConsumption"] = Convert.ToInt64(dr["CementProductionSum"]) > 0 ?
                    (Convert.ToDouble(dr["Clinker_ComprehensiveElectricityConsumption"]) * Convert.ToDouble(dr["ClinkerConsumptionSum"]) + Convert.ToDouble(dr["ElectricityConsumption"])) / Convert.ToDouble(dr["CementProductionSum"]) : 0;
                // 可比水泥综合电耗 =水泥综合电耗*海拔修正系数*水泥强度修正系数
                dr["Cement_ComparableComprehensiveElectricityConsumption"] =
                   Convert.ToDouble(dr["Cement_ComprehensiveElectricityConsumption"]) * v_CoefficientAltitude * Convert.ToDouble(dr["CementIntensityCorrectionFactor"]);
                // 可比水泥综合煤耗 =可比熟料综合煤耗*熟料平均配比/100
                dr["Cement_ComparableComprehensiveCoalDustConsumption"] =
                   Convert.ToDouble(dr["Clinker_ComparableComprehensiveCoalDustConsumption"]) * Convert.ToDouble(dr["ClinkerMatching"]) / 100;
                // -- 可比水泥综合能耗 =可比水泥综合煤耗+0.1229*可比水泥综合电耗
                dr["Cement_ComparableComprehensiveEnergyConsumption"] =
                   Convert.ToDouble(dr["Cement_ComparableComprehensiveCoalDustConsumption"]) + Convert.ToDouble(dr["Cement_ComparableComprehensiveElectricityConsumption"]) * 0.1229;
            }

            return temp_result;
        }



        public static DataTable CreateTableStructure(string myDataTableName)
        {
            string queryString = @"SELECT TOP 1 * FROM " + myDataTableName;
            DataTable sourceTable = _dataFactory.Query(queryString);
            DataTable result = sourceTable.Clone();

            return result;
        }

        public static string Save(string organizationId, string clinkerOrganizationId, string year, DataTable data)
        {
            string keyId = "";

            // 查询是否已存在此报表
            Query query = new Query("tz_Report");
            query.AddCriterion("OrganizationID", organizationId, SqlServerDataAdapter.Infrastruction.CriteriaOperator.Equal);
            query.AddCriterion("Date", year, SqlServerDataAdapter.Infrastruction.CriteriaOperator.Equal);
            query.AddCriterion("TableName", "report_CementYearlyPerUnitDistributionPowerConsumption", SqlServerDataAdapter.Infrastruction.CriteriaOperator.Equal);

            DataTable result = _dataFactory.Query(query);

            if (result.Rows.Count > 0)
            {
                keyId = result.Rows[0]["KeyID"].ToString();
            }
            else
            {
                keyId = Guid.NewGuid().ToString();

                string insertCommand = @"INSERT INTO [dbo].[tz_Report]([OrganizationID],[ReportID],[ReportName],[Date],[TableName],[KeyID]) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')";
                string command = string.Format(insertCommand, organizationId, Guid.NewGuid().ToString(), "水泥生产线可比单位产品能耗分析", year, "report_CementYearlyPerUnitDistributionPowerConsumption", keyId);
                _dataFactory.ExecuteSQL(command);

            }

            // 删除当前保存的结果
            Delete delete = new Delete("report_CementYearlyPerUnitDistributionPowerConsumption");
            delete.AddCriterions("KeyID", keyId, SqlServerDataAdapter.Infrastruction.CriteriaOperator.Equal);
            _dataFactory.Remove(delete);

            // 更新数据
            for (int i = data.Rows.Count - 1; i >= 0; i--)
            {
                // 建立ID
                data.Rows[i]["ID"] = Guid.NewGuid();
                // 更新KeyID
                data.Rows[i]["KeyID"] = keyId;
            }

            _dataFactory.Save("report_CementYearlyPerUnitDistributionPowerConsumption", data);

            return "保存成功";
        }
        
        /// <summary>
        /// 查询组织机构下的熟料线（组织机构ID需为分厂级或分厂级以上）
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static DataTable GetClinkerTable(string organizationId)
        {
            string command = @" SELECT B.* 
                                FROM [system_Organization] AS A, [system_Organization] AS B 
                                WHERE A.[OrganizationID] = @organizationId AND B.[LevelCode] LIKE SUBSTRING(A.[LevelCode], 1, 3) + '%' AND (B.[Type] = '熟料' OR B.[Type] IS NULL)";

            return _dataFactory.Query(command, new SqlParameter("organizationId", organizationId));
        }
    }
}
