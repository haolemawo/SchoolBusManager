using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;

using WBPlatform.Database;
using WBPlatform.TableObject;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(getClassStudentsRoute)]
    public class GetClassStudentsController : APIController
    {
        [HttpGet]
        public JsonResult Get(string ClassID, string TeacherID)
        {
            if (!ValidateSession()) return SessionError;
            if (!(/*CurrentUser.ClassList.Contains(ClassID) &&*/ CurrentUser.ObjectId == TeacherID)) return UserGroupError;

            //DBQuery StudentQuery = new DBQuery();
            //StudentQuery.WhereEqualTo("ClassID", ClassID);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            switch (DataBaseOperation.QueryMultiple(s => s.Class.ObjectId == ClassID, out List<StudentObject> StudentList))
            {
                case DBQueryStatus.INTERNAL_ERROR: return InternalError;
                default: return Json(new { StudentList.Count, StudentList });
            }
        }
    }
}