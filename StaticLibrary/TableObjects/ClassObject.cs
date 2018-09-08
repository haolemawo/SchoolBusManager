using WBPlatform.Database.IO;
using WBPlatform.StaticClasses;

namespace WBPlatform.TableObject
{
    public class ClassObject : DataTableObject<ClassObject>
    {
        public string CDepartment { get; set; }
        public string CGrade { get; set; }
        public string CNumber { get; set; }

        public string TeacherID { get; set; }

        public override string Table => TABLE_Mgr_Classes;

        public override void ReadFields(DataBaseIO input)
        {
            base.ReadFields(input);
            CDepartment = input.GetString("ClassDepartment");
            CGrade = input.GetString("ClassGrade");
            CNumber = input.GetString("ClassNumber");
            TeacherID = input.GetString("TeacherID");
        }

        public override void WriteObject(DataBaseIO output, bool all)
        {
            base.WriteObject(output, all);
            output.Put("ClassDepartment", CDepartment);
            output.Put("ClassGrade", CGrade);
            output.Put("ClassNumber", CNumber);
            output.Put("TeacherID", TeacherID);
        }
    }
}
