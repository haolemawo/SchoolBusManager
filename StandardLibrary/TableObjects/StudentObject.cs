using Newtonsoft.Json;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using WBPlatform.Database.IO;
using WBPlatform.StaticClasses;

namespace WBPlatform.TableObject
{
    public class StudentObject : DataTableObject<StudentObject>
    {

        public string StudentName { get; set; }

        [ForeignKey("BusID")]
        public SchoolBusObject Bus { get; set; }
        public string Sex { get; set; }

        [ForeignKey("ClassID")]
        public ClassObject Class { get; set; }
        public bool LSChecked { get; set; }
        public bool CSChecked { get; set; }
        public bool AHChecked { get; set; }
        public bool TakingBus { get; set; }
        public StudentBigWeekMode WeekType { get; set; }
        public DirectGoHomeMode DirectGoHome { get; set; }
    }
}
