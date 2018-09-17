using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WBPlatform.Database.IO;
using WBPlatform.Logging;
using System.Linq;
using WBPlatform.StaticClasses;
using WBPlatform.Database;
using WBPlatform.TableObject;

namespace WBPlatform.Config
{
    public class ServerConfigCollection
    {
        public class ConfigObject : DataTableObject<ConfigObject>
        {
            public string PropContent { get; set; }
            public string PropName { get; set; }
            public override string Table => TABLE_ServerConfig;

            public override void ReadFields(DataBaseIO input)
            {
                L.I("Get New Config...");
                L.I(input.Stringify());
                base.ReadFields(input);
                PropContent = input.GetString("PropContent");
                PropName = input.GetString("PropName");
            }

            public override void WriteObject(DataBaseIO output, bool all)
            {
                base.WriteObject(output, all);
                output.Put("PropContent", PropContent);
                output.Put("PropName", PropName);
                L.I("Writing Config...");
                L.I(output.Stringify());
            }
        }

        private ConcurrentDictionary<string, ConfigObject> _collection;
        public string this[string key]
        {
            get
            {
                _collection.TryGetValue(key, out ConfigObject conf);
                return conf?.PropContent ?? "_null_";
            }
            set => _collection.AddOrUpdate(key, new ConfigObject() { PropName = key, PropContent = value }, new System.Func<string, ConfigObject, ConfigObject>((str, conf) => { conf.PropContent = value; return conf; }));
        }

        public void GetConfig()
        {
            if (DataBaseOperation.QueryMultiple(new DBQuery().Limit(5000), out List<ConfigObject> configs) >= DBQueryStatus.NO_RESULTS)
            {
                _collection = new ConcurrentDictionary<string, ConfigObject>(configs.ToDictionary((c) => c.PropName));
            }
            else
            {
                L.E("No Config Loaded! See Logs...");
                return;
            }
        }

        public void SaveConfig()
        {
            bool isSucceed = true;
            L.I("Saving config");
            foreach (var item in _collection)
            {
                if (!string.IsNullOrWhiteSpace(item.Value.ObjectId))
                {
                    if (DataBaseOperation.UpdateData(item.Value) != DBQueryStatus.ONE_RESULT)
                    {
                        isSucceed = false;
                        L.E("Save Config Error! " + item.Stringify());
                    }
                }
                else
                {
                    if (DataBaseOperation.CreateData(item.Value) != DBQueryStatus.ONE_RESULT)
                    {
                        isSucceed = false;
                        L.E("Create Config Error! " + item.Stringify());
                    }
                }
            }
            if (isSucceed)
            {
                GetConfig();
            }
        }
    }
}
