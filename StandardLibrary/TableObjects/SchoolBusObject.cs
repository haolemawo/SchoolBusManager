using Newtonsoft.Json;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using WBPlatform.Database.IO;
using WBPlatform.StaticClasses;

namespace WBPlatform.TableObject
{
    public class SchoolBusObject : DataTableObject<SchoolBusObject>
    {
        public string BusName { get; set; }
        [ForeignKey("TeacherID")]
        public UserObject Teacher { get; set; }

        public bool BigWeekOnly { get; set; }
        public bool AHChecked { get; set; }
        public bool CSChecked { get; set; }
        public bool LSChecked { get; set; }
    }
}
