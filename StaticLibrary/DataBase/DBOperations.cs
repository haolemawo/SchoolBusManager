using System;
using System.Collections.Generic;
using System.Net;

using WBPlatform.Config;
using WBPlatform.Database.Connection;
using WBPlatform.Database.IO;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace WBPlatform.Database
{
    public static class DataBaseOperation
    {
        private static readonly object LOCKER = new object();
        //private static string QueryToken = "";
        public static bool isInitiallised = false;
        public static string MessageId => Cryptography.RandomString(5, false);
        public static void InitialiseClient()
        {
            LW.I("Started Initialise Database Server Connection....");
            bool conn = DatabaseSocketsClient.Initialise(IPAddress.Parse(XConfig.Current.Database.DBServerIP), XConfig.Current.Database.DBServerPort);
            while (!conn)
            {
                LW.E("DBServer Initial Connection Failed!");
                conn = DatabaseSocketsClient.Initialise(IPAddress.Parse(XConfig.Current.Database.DBServerIP), XConfig.Current.Database.DBServerPort);
            }
        }
        public static DBQueryStatus QuerySingle<T>(DBQuery query, out T Result) where T : DataTableObject<T>, new()
        {
            //query.Limit(1);
            //DBQueryStatus databaseOperationResult = _DBRequestInternal(new T().Table, DBVerbs.QuerySingle, query, null, out DBInput[] input);
            //if (databaseOperationResult == DBQueryStatus.ONE_RESULT)
            //{
            //    T t = new T();
            //    t.ReadFields(input[0]);
            //    Result = t;
            //    return databaseOperationResult;
            //}
            //else
            //{
            //    Result = null;
            //    return databaseOperationResult;
            //}
            var _Status = QueryMultiple(query, out List<T> results, 1);
            Result = _Status > 0 ? results[0] : null;
            return _Status;
        }


        public static DBQueryStatus QueryMultiple<T>(DBQuery query, out List<T> Result, int queryLimit = 100, int skip = 0) where T : DataTableObject<T>, new()
        {
            query.Limit(queryLimit);
            query.Skip(skip);
            DBQueryStatus databaseOperationResult = _DBRequestInternal(new T().Table, DBVerbs.QueryMulti, query, null, out DataBaseIO[] inputs);
            if (databaseOperationResult >= 0)
            {
                Result = new List<T>();
                foreach (DataBaseIO item in inputs)
                {
                    T t = new T();
                    t.ReadFields(item);
                    Result.Add(t);
                }
            }
            else Result = null;
            return databaseOperationResult;
        }
        //public static DBQueryStatus DeleteData(string Table, string ObjectID)
        //{
        //    DBQueryStatus result = _DBRequestInternal(Table, DBVerbs.Delete, new DBQuery().WhereEqualTo("objectId", ObjectID), null, out DataBaseIO[] inputs);
        //    return result;
        //}

        public static DBQueryStatus UpdateData<T>(T item) where T : DataTableObject<T>, new() => UpdateData(ref item);
        public static DBQueryStatus UpdateData<T>(ref T item) where T : DataTableObject<T>, new() => UpdateData(ref item, null);
        public static DBQueryStatus UpdateData<T>(ref T item, DBQuery query) where T : DataTableObject<T>, new()
        {
            if (query == null)
            {
                query = new DBQuery().WhereEqualTo("objectId", item.ObjectId);
            }
            query.Limit(1);
            DataBaseIO output = new DataBaseIO();
            item.WriteObject(output, false);
            var _result = _DBRequestInternal(item.Table, DBVerbs.Update, query, output, out DataBaseIO[] inputs);
            if (_result != DBQueryStatus.ONE_RESULT)
            {
                LW.E("UpdateData Process Failed!");
                return DBQueryStatus.INTERNAL_ERROR;
            }
            item = new T();
            item.ReadFields(inputs[0]);
            return _result;
        }

        public static DBQueryStatus CreateData<T>(ref T data) where T : DataTableObject<T>, new() => CreateData(data, out data);
        private static DBQueryStatus CreateData<T>(T data, out T dataOut) where T : DataTableObject<T>, new()
        {
            DataBaseIO output = new DataBaseIO();
            data.ObjectId = Cryptography.RandomString(10, false);
            data.WriteObject(output, false);
            DBQueryStatus rst = _DBRequestInternal(data.Table, DBVerbs.Create, null, output, out DataBaseIO[] inputs);
            if (rst == DBQueryStatus.INTERNAL_ERROR)
            {
                dataOut = null;
                return rst;
            }
            T t = new T();
            t.ReadFields(inputs[0]);
            dataOut = t;
            return rst;
        }

        private static DBQueryStatus _DBRequestInternal(string Table, DBVerbs operation, DBQuery query, DataBaseIO output, out DataBaseIO[] results)
        {
            try
            {
                //We gonna throw some exceptions!
                if ((operation == DBVerbs.QueryMulti || operation == DBVerbs.QuerySingle || operation == DBVerbs.Update || operation == DBVerbs.Delete) && query == null)
                    throw new ArgumentNullException("When using Query Single/Multi and Change, Delete. Arg: query cannot be null");

                if ((operation == DBVerbs.Create || operation == DBVerbs.Update) && output == null)
                    throw new ArgumentNullException("When using Query Create and Change. Arg: output cannot be null");

                DataBaseSocketIO internalQuery = new DataBaseSocketIO { Verb = operation, TableName = Table };
                switch (operation)
                {
                    case DBVerbs.Create:
                        internalQuery.DBObjects = output.MoveToArray();
                        break;
                    case DBVerbs.QuerySingle:
                    case DBVerbs.QueryMulti:
                        internalQuery.Query = query;
                        break;
                    case DBVerbs.Update:
                        internalQuery.DBObjects = output.MoveToArray();
                        internalQuery.Query = query;
                        break;
                    case DBVerbs.Delete:
                        internalQuery.Query = query;
                        break;
                }

                string internalQueryString = internalQuery.ToParsedString();

                string _MessageId = MessageId;
                if (!DatabaseSocketsClient.SendCommand(internalQueryString, _MessageId, out string rcvdData))
                {
                    results = null;
                    throw new DataBaseException("Database is not connected currently...");
                }

                if (!rcvdData.ToParsedObject(out DataBaseSocketIO reply)) throw new DataBaseException("DBInternalReply is null");

                //time to throw exceptions! (again)
                switch (reply.ResultCode)
                {
                    case DBQueryStatus.INJECTION_DETECTED: throw new DataBaseException("INJECTION DETECTED.", reply.Exception);
                    case DBQueryStatus.INTERNAL_ERROR: throw new DataBaseException("Database Server Internal Error", reply.Exception);
                }

                switch (operation)
                {
                    case DBVerbs.QueryMulti:
                        results = reply.DBObjects ?? throw new DataBaseException("Query DBObjects should have non-null result.");
                        break;

                    case DBVerbs.Create:
                    case DBVerbs.Update:
                    case DBVerbs.QuerySingle:
                        if (reply.DBObjects.Length != 1) throw new DataBaseException("Create & Update & QuerySingle expect 1 result.");
                        results = reply.DBObjects;
                        break;

                    case DBVerbs.Delete:
                        results = null;
                        break;

                    //Who knows what the hell it is...
                    default: throw new DataBaseException("Database Operation is not Supported!");
                }
                return reply.ResultCode;
            }
            catch (Exception ex)
            {
                results = null;
                LW.E(ex.ToParsedString());
                return DBQueryStatus.INTERNAL_ERROR;
            }
        }
    }
}
