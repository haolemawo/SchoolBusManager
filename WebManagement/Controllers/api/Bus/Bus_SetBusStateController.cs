
using Microsoft.AspNetCore.Mvc;

using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(setStudentState)]
    public class Student_SetStateController : APIController
    {
        [HttpGet]
        public JsonResult GET(string studentId, string state)
        {
            //THIS FUNCTION IS SHARED BY CLASSTEACHER AND PARENTS
            if (!ValidateSession()) return SessionError;
            if (!CurrentUser.IsParent && !CurrentUser.IsClassTeacher) return UserGroupError;

            if (DataBaseOperation.QuerySingle(s => s.ObjectId == studentId, out StudentObject Student) != DBQueryStatus.ONE_RESULT)
                return RequestIllegal;

            //Check if this student is onder user's control;
            bool flag = false;

            if (!flag && CurrentUser.IsParent && CurrentUser.ChildList.Contains(Student.ObjectId)) flag = true;

            if (!flag && CurrentUser.IsClassTeacher)
            {
                if (DataBaseOperation.QueryMultiple(c => c.Teacher.ObjectId == CurrentUser.ObjectId, out System.Collections.Generic.List<ClassObject> Class) == DBQueryStatus.NO_RESULTS)
                    return RequestIllegal;
                foreach (var item in Class)
                {
                    flag = flag || Student.Class.ObjectId == item.ObjectId;
                }
            }

            if (!flag) return RequestIllegal;
            if (!bool.TryParse(state, out bool _state)) return RequestIllegal;
            Student.TakingBus = _state;

            return DataBaseOperation.UpdateData(ref Student) != DBQueryStatus.ONE_RESULT
                ? DataBaseError
                : Json(new { Student, SignMode = "TakingBus", SignResult = Student.TakingBus });
        }
    }
}
