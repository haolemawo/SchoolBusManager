﻿using System.Collections.Generic;

using Newtonsoft.Json;

using WBPlatform.Database;
using WBPlatform.StaticClasses;

namespace WBPlatform.TableObject
{
    public class StudentObject : _DataTableObject
    {

        public string StudentName { get; set; }

        public string BusID { get; set; }
        public string Sex { get; set; }

        public string ClassID { get; set; }
        public bool LSChecked { get; set; }
        public bool CSChecked { get; set; }
        public bool AHChecked { get; set; }

        //public string ParentsID { get; set; }
        

        public override string table => WBConsts.TABLE_Mgr_StuData;

        public override void readFields(DBInput input)
        {
            base.readFields(input);
            StudentName = input.GetString("StuName");
            BusID = input.GetString("BusID");
            Sex = input.GetString("Sex");
            ClassID = input.GetString("ClassID");
            //ParentsID = input.getString("ParentsIDs");
            CSChecked = input.GetBool("CSChecked");
            LSChecked = input.GetBool("LSChecked");
            AHChecked = input.GetBool("CHChecked");
        }

        public override void write(DBOutput output, bool all)
        {
            base.write(output, all);
            output.Put("StuName", StudentName);
            output.Put("BusID", BusID);
            output.Put("Sex", Sex);
            output.Put("ClassID", ClassID);
            //output.Put("ParentsIDs", ParentsID);
            output.Put("CHChecked", AHChecked);
            output.Put("CSChecked", CSChecked);
            output.Put("LSChecked", LSChecked);
        }
        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>
            {
                { "StuID", objectId },
                { "Name", StudentName },
                { "Sex", Sex },
                { "BusID", BusID },
                { "ClassID", ClassID },
                { "ComingChecked", CSChecked.ToString().ToLower() },
                { "LeavingChecked", LSChecked.ToString() },
                { "ParentLeavingChecked", AHChecked.ToString() },
            };
        }
        public override string ToString() => JsonConvert.SerializeObject(ToDictionary());
    }
}
