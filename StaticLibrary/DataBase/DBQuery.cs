using System.Collections.Generic;
using System.Linq;

namespace WBPlatform.Database
{
    public class DBQuery
    {
        public DBQuery() : this("updatedAt", false) { }
        public DBQuery(string _SortedBy, bool IsAscending)
        {
            EqualTo = new Dictionary<string, object>();
            Contains = new Dictionary<string, string>();
            ContainedInArray = new Dictionary<string, string[]>();
            SortedBy(_SortedBy);
            _Ascending = IsAscending;
        }

        public bool _Ascending { get; set; }
        public int _Limit { get; set; }
        public int _Skip { get; set; }
        public string _SortedBy { get; set; }

        public Dictionary<string, object> EqualTo { get; private set; }
        public Dictionary<string, string> Contains { get; private set; }
        public Dictionary<string, string[]> ContainedInArray { get; private set; }

        public DBQuery WhereValueContainedInArray<T>(string column, params T[] values) { ContainedInArray.Add(column, (from _ in values select _.ToString()).ToArray()); return this; }
        public DBQuery SortedBy(string column) { _SortedBy = column; return this; }
        public DBQuery WhereEqualTo(string column, object value) { EqualTo.Add(column, value); return this; }
        public DBQuery WhereRecordContainsValue(string column, string value) { Contains.Add(column, value); return this; }
        public DBQuery Limit(int limit) { _Limit = limit; return this; }
        public DBQuery Skip(int skip) { _Skip = skip; return this; }
        public DBQuery Ascending() { _Ascending = true; return this; }
        public DBQuery Descending() { _Ascending = false; return this; }
    }
}
