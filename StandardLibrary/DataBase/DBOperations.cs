using System;
using System.Collections.Generic;
using System.Linq;
using WBPlatform.DataBase_ng;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace WBPlatform.Database
{
    public static class DataBaseOperation
    {
        public static void Initialise(DataBaseContext context) { Context = context; }
        private static DataBase_ng.DataBaseContext Context { get; set; }

        public static DBQueryStatus QuerySingle<T>(Func<T, bool> where, out T Result) where T : DataTableObject<T>, new()
        {
            var _result = QueryMultiple(where, out List<T> results);
            Result = _result > 0 ? results[0] : null;
            return GetResultEnum(results.Count);
        }

        public static DBQueryStatus QueryMultiple<T>(Func<T, bool> where, out List<T> Result, int queryLimit = 1000, int skip = 0) where T : DataTableObject<T>, new()
        {
            Result = Context.Set<T>().Where(where).ToList();
            return GetResultEnum(Result.Count);
        }
        public static DBQueryStatus QueryAll<T>(out List<T> Results) where T : DataTableObject<T>, new()
        {
            Results = Context.Set<T>().ToList();
            return GetResultEnum(Results.Count);
        }
        private static DBQueryStatus GetResultEnum(int count)
        {
            return count == 1
                ? DBQueryStatus.ONE_RESULT
                : count >= 1
                    ? DBQueryStatus.MORE_RESULTS
                    : count == 0
                        ? DBQueryStatus.NO_RESULTS
                        : DBQueryStatus.INTERNAL_ERROR;
        }
        public static DBQueryStatus UpdateData<T>(T item) where T : DataTableObject<T>, new() => UpdateData(ref item);
        public static DBQueryStatus UpdateData<T>(ref T item) where T : DataTableObject<T>, new()
        {
            string oid = item.ObjectId;
            var entry = Context.Set<T>().Update(item);
            Context.SaveChanges();
            return QuerySingle(t => t.ObjectId == oid, out item);
        }
        public static DBQueryStatus CreateData<T>(T data) where T : DataTableObject<T>, new() => CreateData(ref data);
        public static DBQueryStatus CreateData<T>(ref T data) where T : DataTableObject<T>, new() => CreateData(data, out data);
        public static DBQueryStatus CreateData<T>(T data, out T dataOut) where T : DataTableObject<T>, new()
        {
            string oid = Cryptography.RandomString(10, false);
            data.ObjectId = oid;
            var entry = Context.Set<T>().Add(data);
            Context.SaveChanges();
            return QuerySingle(t => t.ObjectId == oid, out dataOut);
        }
    }
}
