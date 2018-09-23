using Newtonsoft.Json;

using System.Collections.Generic;

using WBPlatform.Database.IO;
using WBPlatform.StaticClasses;

namespace WBPlatform.TableObject
{
    public class StudentObject : DataTableObject<StudentObject>
    {

        public string StudentName { get; set; }

        public string BusID { get; set; }
        public string Sex { get; set; }

        public string ClassID { get; set; }
        public StudentBigWeekMode WeekType { get; set; }
        public bool LSChecked { get; set; }
        public bool CSChecked { get; set; }
        public bool AHChecked { get; set; }
        public bool TakingBus { get; set; }
        public DirectGoHomeMode DirectGoHome { get; set; }

        //public string ParentsID { get; set; }


        public override string Table => TABLE_Mgr_StuData;

        public override void ReadFields(DataBaseIO input)
        {
            base.ReadFields(input);
            StudentName = input.GetString("StuName");
            BusID = input.GetString("BusID");
            Sex = input.GetString("Sex");
            ClassID = input.GetString("ClassID");
            //ParentsID = input.getString("ParentsIDs");
            CSChecked = input.GetBool("CSChecked");
            LSChecked = input.GetBool("LSChecked");
            TakingBus = input.GetBool("TakingBus");
            AHChecked = input.GetBool("CHChecked");
            WeekType = (StudentBigWeekMode)input.GetInt("WeekType");
            DirectGoHome = (DirectGoHomeMode)input.GetInt("DirectGoHome");
        }

        public override void WriteObject(DataBaseIO output, bool all)
        {
            base.WriteObject(output, all);
            output.Put("StuName", StudentName);
            output.Put("BusID", BusID);
            output.Put("Sex", Sex);
            output.Put("ClassID", ClassID);
            //output.Put("ParentsIDs", ParentsID);
            output.Put("CHChecked", AHChecked);
            output.Put("CSChecked", CSChecked);
            output.Put("LSChecked", LSChecked);
            output.Put("TakingBus", TakingBus);
            output.Put("WeekType", (int)WeekType);
            output.Put("DirectGoHome", (int)DirectGoHome);
        }
    }
}
