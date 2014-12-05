using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticalReport.Infrastructure.Configuration
{
    public static class ConnectionStringFactory
    {
        // 大哥
        //private static string _connString = "Data Source=QH-20140814XCYI;Initial Catalog=NXJC;Integrated Security=True";
        // 台式机
        //private static string _connString = "Data Source=DEC-WINSVR12;Initial Catalog=NXJC_DEVELOP;User Id=sa;Password=jsh123+";
        // 笔记本
        //private static string _connString = "Data Source=Lenovo-PC;Initial Catalog=NXJC_DEVELOP;User Id=sa;Password=jsh123+";
        // 现场
        //private static string _connString = "Data Source=Lenovo-PC;Initial Catalog=NXJC_DEVELOP;User Id=sa;Password=jsh123+";

        //public static string NXJCConnectionString { get { return _connString; } }
        public static string NXJCConnectionString { get { return ConfigurationManager.ConnectionStrings["ConnNXJC"].ToString(); } }
    }
}
