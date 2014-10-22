using StatisticalReport.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticalReport.Service
{
    public class OrganizationService
    {
        private static readonly string CONNECTION_STRING = ConnectionStringFactory.NXJCConnectionString;
        public static DataTable GetOrganizationTree()
        {
            DataSet ds = new DataSet();

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM system_Organization";

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(ds);
                }
            }

            return ds.Tables[0];
        }

        public static DataTable GetOrganizationTreeWithProductLineType()
        {
            DataSet ds = new DataSet();

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT OrganizationID, LevelCode, Name, Type FROM system_Organization";

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(ds);
                }
            }

            DataTable originalTable = ds.Tables[0];

            foreach (DataRow dr in originalTable.Rows)
            {
                if (dr["LevelCode"].ToString().Length == 7)
                {
                    string levelCodeWithProductLineType = dr["LevelCode"].ToString().Substring(0, 5) +
                        (dr["Type"].ToString() == "熟料" ? "01" :
                        dr["Type"].ToString() == "水泥磨" ? "02" :
                        dr["Type"].ToString() == "余热发电" ? "03" : "04") +
                        dr["LevelCode"].ToString().Substring(5, 2);

                    dr["LevelCode"] = levelCodeWithProductLineType;
                }
            }

            DataColumn typeCodeColumn = new DataColumn("typeCode", typeof(string));
            typeCodeColumn.Expression = "IIF(LEN(LevelCode)=9,SUBSTRING(LevelCode,1,7),'')";
            originalTable.Columns.Add(typeCodeColumn);

            DataView originalView = originalTable.DefaultView;
            DataTable levelCodeTable = originalView.ToTable(true, "typeCode", "Type");

            foreach (DataRow dr in levelCodeTable.Rows)
            {
                if (dr["typeCode"].ToString().Length != 7)
                    continue;

                DataRow typeRow = originalTable.NewRow();
                typeRow["LevelCode"] = dr["typeCode"];
                typeRow["Name"] = dr["Type"];
                originalTable.Rows.Add(typeRow);
            }

            originalView.Sort = "LevelCode";

            DataTable result = originalView.ToTable();

            foreach (DataRow dr in result.Rows)
            {
                if (dr["typeCode"].ToString().Length == 7)
                {
                    dr["Type"] = dr["LevelCode"].ToString().Substring(7, 2);
                }
            }

            result.Columns.Remove("typeCode");

            return result;
        }
    }
}