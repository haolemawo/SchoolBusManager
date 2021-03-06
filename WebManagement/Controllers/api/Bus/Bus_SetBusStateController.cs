﻿
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
            if (!CurrentUser.UserGroup.IsParent && !CurrentUser.UserGroup.IsClassTeacher) return UserGroupError;

            if (DataBaseOperation.QuerySingle(new DBQuery().WhereIDIs(studentId), out StudentObject Student) != DBQueryStatus.ONE_RESULT)
                return RequestIllegal;

            //Check if this student is onder user's control;
            bool flag = false;

            if (!flag && CurrentUser.UserGroup.IsParent && CurrentUser.ChildList.Contains(Student.ObjectId)) flag = true;

            if (!flag && CurrentUser.UserGroup.IsClassTeacher)
            {
                if (DataBaseOperation.QueryMultiple(new DBQuery().WhereEqualTo("TeacherID", CurrentUser.ObjectId), out System.Collections.Generic.List<ClassObject> Class) == DBQueryStatus.NO_RESULTS)
                    return RequestIllegal;
                foreach (var item in Class)
                {
                    flag = flag || Student.ClassID == item.ObjectId;
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
