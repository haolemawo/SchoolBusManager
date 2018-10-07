using System.ComponentModel.DataAnnotations.Schema;
using WBPlatform.Database.IO;
using WBPlatform.StaticClasses;

namespace WBPlatform.TableObject
{
    public class ClassObject : DataTableObject<ClassObject>
    {
        public string CDepartment { get; set; }
        public string CGrade { get; set; }
        public string CNumber { get; set; }

        [ForeignKey("TeacherID")]
        public UserObject Teacher { get; set; }
    }
}
