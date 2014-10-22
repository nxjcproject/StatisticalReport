using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatisticalReport.Infrastructure.Configuration
{
    public static class ConnectionStringFactory
    {
        //private static string _connString = "Data Source=QH-20140814XCYI;Initial Catalog=NXJC;Integrated Security=True";
        private static string _connString = "Data Source=DEC-WINSVR12;Initial Catalog=NXJC_DEVELOP;User Id=sa; Password = jsh123+";
        public static string NXJCConnectionString { get { return _connString; } }
    }
}
