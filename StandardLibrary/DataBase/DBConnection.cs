using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using WBPlatform.Config;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;

namespace WBPlatform.Database.Connection
{
    public static class DBConnectionBuilder
    {
        public static DbConnection Connection { get; private set; }
        public static DbConnection InitialiseDBConnection()
        {
            L.E("Current Directory: " + Directory.GetCurrentDirectory());
            SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
            L.I("Start Initiallising Database Connections.....");
            conn.DataSource = XConfig.Current.Database.SQLServerIP + "," + XConfig.Current.Database.SQLServerPort;
            conn.UserID = XConfig.Current.Database.DatabaseUserName;
            conn.Password = XConfig.Current.Database.DatabasePassword;
            conn.TrustServerCertificate = true;

            L.I("DB Connection String Loaded!");
            DbConnection sqlConnection = new SqlConnection(conn.ConnectionString);
            sqlConnection.Open();
            L.I("DB Connection Opened!");
            Connection = sqlConnection;
            return sqlConnection;
        }
    }
}
