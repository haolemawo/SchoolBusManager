using Newtonsoft.Json;
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
        public int _Limit { get; set; } = -1;
        public int _Skip { get; set; } = -1;
        public string _SortedBy { get; set; }

        [JsonIgnore]
        public bool AnyThing => EqualTo.Count > 0 || Contains.Count > 0 || ContainedInArray.Count > 0;

        public Dictionary<string, object> EqualTo { get; private set; }
        public Dictionary<string, string> Contains { get; private set; }
        public Dictionary<string, string[]> ContainedInArray { get; private set; }
        
        public DBQuery WhereValueContainedInArray(string column, IEnumerable<string> values) { ContainedInArray.Add(column, (from _ in values select _).ToArray()); return this; }
        public DBQuery SortedBy(string column) { _SortedBy = column; return this; }
        public DBQuery WhereEqualTo(string column, object value) { EqualTo.Add(column, value); return this; }
        public DBQuery WhereRecordContainsValue(string column, string value) { Contains.Add(column, value); return this; }
        public DBQuery Limit(int limit) { _Limit = limit; return this; }
        public DBQuery Skip(int skip) { _Skip = skip; return this; }
        public DBQuery Ascending() { _Ascending = true; return this; }
        public DBQuery Descending() { _Ascending = false; return this; }
    }
}
