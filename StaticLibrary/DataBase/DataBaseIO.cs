using System;
using System.Collections;
using System.Collections.Generic;

using WBPlatform.Logging;

namespace WBPlatform.Database.IO
{
    public sealed class DataBaseIO
    {
        public DataBaseIO() { }
        public DataBaseIO(Dictionary<string, object> data) { Data = data; }
        public object this[string column] => Data[column];

        public Dictionary<string, object> Data { get; private set; } = new Dictionary<string, object>();

        public T GetT<T>(string Key) => (T)Convert.ChangeType(Data[Key], typeof(T));

        public void Put(string column, object _data)
        {
            if (_data == null) { LW.E("DBOutput: Put " + column + " as null, drop it..."); return; }
            if (_data is ICollection) _data = string.Join(",", (IEnumerable<string>)_data);
            if (Data.ContainsKey(column))
            {
                //Don't know why to delete first...
                //Copied the implemention of Bmob Database SDK
                Data.Remove(column);
                Data.Add(column, _data);
            }
            else Data.Add(column, _data);
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
        public DataBaseException(string message) : base(message) { }
        public DataBaseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
