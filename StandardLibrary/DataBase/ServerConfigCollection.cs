using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using WBPlatform.Database;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace WBPlatform.Config
{
    public static class ServerConfig
    {
        public static ServerConfigCollection Current { get; } = new ServerConfigCollection();


        public class ServerConfigCollection
        {
            public class ConfigObject : DataTableObject<ConfigObject>
            {
                public string PropContent { get; set; }
                public string PropName { get; set; }
            }

            private ConcurrentDictionary<string, ConfigObject> _collection;
            public string this[string key]
            {
                get
                {
                    _collection.TryGetValue(key, out ConfigObject conf);
                    return conf?.PropContent ?? "_null_";
                }
                set => _collection.AddOrUpdate(key, new ConfigObject() { PropName = key, PropContent = value }, new Func<string, ConfigObject, ConfigObject>((str, conf) => { conf.PropContent = value; return conf; }));
            }

            public bool GetConfig()
            {
                if (DataBaseOperation.QueryAll(out List<ConfigObject> configs) >= DBQueryStatus.NO_RESULTS)
                {
                    _collection = new ConcurrentDictionary<string, ConfigObject>(configs.ToDictionary(c => c.PropName));
                    return true;
                }
                else
                {
                    L.E("Exception occured while loading ConfigData");
                    return false;
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
    public static class ServerConfigCollectionExtensions
    {
        public static bool IsBigWeek(this ServerConfig.ServerConfigCollection collection) => collection["WeekType"] == "big";
        public static void SetWeekType(this ServerConfig.ServerConfigCollection collection, bool isBigWeek) => collection["WeekType"] = isBigWeek ? "big" : "small";
    }
}
