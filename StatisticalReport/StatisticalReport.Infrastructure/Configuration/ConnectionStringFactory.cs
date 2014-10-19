using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatisticalReport.Infrastructure.Configuration
{
    public static class ConnectionStringFactory
    {
        private static string _connString = "Data Source=QH-20140814XCYI;Initial Catalog=NXJC;Integrated Security=True";

        public static string NXJCConnectionString { get { return _connString; } }
    }
}
