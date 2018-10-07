using System.ComponentModel.DataAnnotations.Schema;
using WBPlatform.Database.IO;
using WBPlatform.StaticClasses;

namespace WBPlatform.TableObject
{
    [Table("BusReports")]
    public class BusReport : DataTableObject<BusReport>
    {
        [ForeignKey("TeacherID")]
        public UserObject Teacher { get; set; }

        [ForeignKey("BusID")]
        public SchoolBusObject Bus { get; set; }
        public BusReportTypeE ReportType { get; set; }
        public string OtherData { get; set; }
    }
}