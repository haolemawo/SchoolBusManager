using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

using WBPlatform.Logging;

namespace WBPlatform.Database.IO
{
    public sealed class DataBaseIO
    {
        public DataBaseIO() { }
        public DataBaseIO(Dictionary<string, object> data) { Data = new ConcurrentDictionary<string, object>(data); }
        public object this[string column] => Data[column];

        public ConcurrentDictionary<string, object> Data { get; private set; } = new ConcurrentDictionary<string, object>();

        public T Get<T>(string column) => (T)Convert.ChangeType(Data[column], typeof(T));

        public void Put(string column, object _data)
        {
            if (_data == null) { L.W("Put " + column + " as null, drop it..."); return; }
            if (_data is IList) _data = string.Join(",", _data as IList<string>);
            //Data.AddOrUpdate(column, _data, (s, o) => _data);
            if (!Data.TryAdd(column, _data)) L.E("Failed To Add Data to DataBaseIO.Data, Column Name: " + column);
        }
    }

    /// <summary>
    /// DO NOT CHANGE THE PROPERTY NAME --- 
    /// RELATED TO DBSERVER SETTINGS...
    /// </summary>
    public class DataBaseSocketIO
    {
        public DBVerbs Verb { get; set; }
        public DBQueryStatus ResultCode { get; set; }

        public string TableName { get; set; } = "";
        public DBQuery Query { get; set; }
        public DataBaseIO[] DBObjects { get; set; }

        public string Message { get; set; } = "";
        public DataBaseException Exception { get; set; }
    }

    public class DataBaseException : Exception
    {
        public DataBaseException() : base() { }
        public DataBaseException(string message) : base(message) { }
        public DataBaseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
