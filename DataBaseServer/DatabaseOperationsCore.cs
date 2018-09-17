using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using WBPlatform.Config;
using WBPlatform.Database.IO;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;

namespace WBPlatform.Database.DBServer
{
    public static class DatabaseCore
    {
        private static SqlConnection sqlConnection;
        public static void InitialiseDBConnection()
        {
            SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
            L.I("Start Initiallising Database Connections.....");
            conn.DataSource = XConfig.Current.Database.SQLServerIP + "," + XConfig.Current.Database.SQLServerPort;
            conn.UserID = XConfig.Current.Database.DatabaseUserName;
            conn.Password = XConfig.Current.Database.DatabasePassword;
            conn.TrustServerCertificate = true;
            L.I("DB Connection String Loaded!");
            sqlConnection = new SqlConnection(conn.ConnectionString);
            sqlConnection.Open();
            L.I("DB Connection Opened!");
        }

        public static string ProcessRequest(DataBaseSocketIO request)
        {
            DataBaseSocketIO reply = new DataBaseSocketIO();
            try
            {
                if (request == null) throw new NullReferenceException("Null Request....");

                if (request.Verb != DBVerbs.Create)
                {
                    if (request.Query == null)
                        throw new ArgumentNullException("When using Query Single/Multi/Change/Delete. Arg: query cannot be null");
                }

                if (request.Verb == DBVerbs.Create || request.Verb == DBVerbs.Update)
                {
                    if (request.DBObjects == null)
                        throw new ArgumentNullException("When using Create and Update. Arg: output cannot be null");
                }

                int rowModified = 0;
                reply.Verb = request.Verb;
                switch (request.Verb)
                {
                    case DBVerbs.Create:
                        rowModified = CommandCreate(request.TableName, request.DBObjects[0]);
                        reply.ResultCode = (DBQueryStatus)rowModified;
                        reply.DBObjects = GetFirstRecord(request.TableName, "objectId", request.DBObjects[0]["objectId"]);
                        break;

                    case DBVerbs.QuerySingle:
                    ///There shouldn't be QuerySingle... <see cref="DataBaseOperation.QueryMultiple{T}(DBQuery, out List{T}, int, int)"/> 
                    case DBVerbs.QueryMulti:
                        var results = SQLQueryCommand(BuildQueryString(request.TableName, request.Query));
                        rowModified = results.Length;
                        reply.DBObjects = results.ToArray();
                        reply.ResultCode = results.Length >= 2 ? DBQueryStatus.MORE_RESULTS : (DBQueryStatus)results.Length;
                        break;
                    case DBVerbs.Update:
                        //Only Support first thing....
                        var dict = SQLQueryCommand(BuildQueryString(request.TableName, request.Query));
                        if (dict.Length != 1)
                        {
                            throw new KeyNotFoundException("Update: Cannot find Specific Record by Query, so Failed to update....");
                        }
                        rowModified = CommandUpdate(request.TableName, dict[0]["objectId"].ToString(), request.DBObjects[0]);
                        reply.ResultCode = (DBQueryStatus)rowModified;
                        reply.DBObjects = GetFirstRecord(request.TableName, "objectId", dict[0]["objectId"]);

                        break;
                    case DBVerbs.Delete:
                        rowModified = CommandDelete(request.TableName, request.Query.EqualTo["objectId"].ToString());
                        reply.ResultCode = (DBQueryStatus)rowModified;
                        break;
                    default:
                        //HttpUtility.UrlEncode("!@#$%^&*()_+");
                        //break;
                        throw new NotSupportedException("What The Hell you are doing....");
                }
                reply.Message = "操作成功完成(" + rowModified + ")";
            }
            catch (Exception ex)
            {
                reply.ResultCode = DBQueryStatus.INTERNAL_ERROR;
                reply.Message = ex.Message;
                reply.Exception = new DataBaseException("DBServer Process Exception", ex);
                L.E("Exception! => \r\n" + ex);
            }
            return reply.Stringify();
        }

        private static string BuildQueryString(string TableName, DBQuery dbQuery)
        {
            string sqlCommand_Query = $"SELECT TOP({dbQuery._Limit}) * FROM {TableName} {(dbQuery.AnyThing ? "WHERE " : string.Empty)}";

            if (dbQuery.EqualTo.Count > 0)
            {
                var queriesStringCollection = from q in dbQuery.EqualTo select $"{q.Key} = '{q.Value.ToString().EncodeAsString()}'";
                sqlCommand_Query += "(" + string.Join(" AND ", queriesStringCollection) + ")";
                sqlCommand_Query += (dbQuery.ContainedInArray.Count > 0 || dbQuery.Contains.Count > 0) ? " AND " : string.Empty;
            }

            if (dbQuery.ContainedInArray.Count > 0)
            {
                List<string> containsSQLList = new List<string>();
                foreach (var item in dbQuery.ContainedInArray)
                {
                    containsSQLList.Add($"( {item.Key} IN ('{string.Join("', '", item.Value)}'))");
                }
                string finalQueryString = string.Join(" OR ", containsSQLList);

                sqlCommand_Query += "(" + finalQueryString + ")";
                sqlCommand_Query += (dbQuery.Contains.Count > 0) ? " AND " : string.Empty;
            }

            if (dbQuery.Contains.Count > 0)
            {
                var queriesStringCollection = from q in dbQuery.Contains select $"{q.Key} LIKE '%{q.Value.EncodeAsString()}%'";
                sqlCommand_Query += string.Join(" AND ", queriesStringCollection);
            }

            sqlCommand_Query += $" order by {dbQuery._SortedBy} {(dbQuery._Ascending ? "" : "desc")}";
            return sqlCommand_Query;
        }

        private static int CommandCreate(string TableName, DataBaseIO output)
        {
            string sqlCommand_Create =
                $"INSERT INTO {TableName} " +
                $"({string.Join(",", output.Data.Keys)}, createdAt, updatedAt)" +
                $" VALUES " +
                $"('{string.Join("','", from _ in output.Data.Values select _.EncodeAsString().ToString())}', '{DateTime.Now}', '{DateTime.Now}')";
            SqlCommand command_Create = new SqlCommand(sqlCommand_Create, sqlConnection);
            return command_Create.ExecuteNonQuery();
        }

        private static int CommandUpdate(string TableName, string ObjectID, DataBaseIO output)
        {
            string sqlCommand_Update =
                $"UPDATE {TableName} " +
                $"SET {string.Join(",", (from q in output.Data select $"{q.Key} = '{q.Value.EncodeAsString()}' ").ToArray())}, updatedAt = '{DateTime.Now}' " +
                $"WHERE objectId = '{ObjectID}'";

            SqlCommand command_Update = new SqlCommand(sqlCommand_Update, sqlConnection);
            return command_Update.ExecuteNonQuery();
        }

        private static DataBaseIO[] GetFirstRecord(string tableName, string Column, object Value)
            => SQLQueryCommand($"SELECT TOP(1) * FROM {tableName} WHERE {Column} = '{Value.EncodeAsString()}' ");

        private static int CommandDelete(string TableName, string ObjectID)
        {
            string sqlCommand_Del = $"DELETE FROM {TableName} WHERE objectId = '{ObjectID}'";
            SqlCommand command = new SqlCommand(sqlCommand_Del, sqlConnection);
            return command.ExecuteNonQuery();
        }

        private static DataBaseIO[] SQLQueryCommand(string sqlCommand)
        {
            SqlDataAdapter sda = new SqlDataAdapter(sqlCommand, sqlConnection);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            sda.Dispose();
            return (from Dictionary<string, object> _ in DataTableToDictionary(ds.Tables[0]) select new DataBaseIO(_)).ToArray();
        }

        public static IEnumerable<IDictionary<string, object>> DataTableToDictionary(DataTable dt)
        {
            ICollection<IDictionary<string, object>> list = new List<IDictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                IDictionary<string, object> dct = new Dictionary<string, object>();
                foreach (DataColumn column in dt.Columns)
                {
                    dct.Add(column.ColumnName, dr[column.ColumnName].DecodeAsObject());
                }
                list.Add(dct);
            }
            return list;
        }
    }
}
