using WBPlatform.Database.IO;
using WBPlatform.StaticClasses;

namespace WBPlatform.TableObject
{
    public class BusReport : DataTableObject<BusReport>
    {
        public string TeacherID { get; set; }
        public string BusID { get; set; }
        public BusReportTypeE ReportType { get; set; }
        public string OtherData { get; set; }

        public override string Table => TABLE_Mgr_WeekIssue;

        public override void ReadFields(DataBaseIO input)
        {
            base.ReadFields(input);
            TeacherID = input.GetString("ReportTeacherID");
            BusID = input.GetString("ReportBusID");
            ReportType = (BusReportTypeE)input.GetInt("ReportType");
            OtherData = input.GetString("DetailedInformation");
        }

        public override void WriteObject(DataBaseIO output, bool all)
        {
            base.WriteObject(output, all);
            output.Put("ReportTeacherID", TeacherID);
            output.Put("ReportBusID", BusID);
            output.Put("ReportType", (int)ReportType);
            output.Put("DetailedInformation", OtherData);
        }
    }
}