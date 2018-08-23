using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Linq;
using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(getBusStudentsRoute)]
    public class GetStudentsController : APIController
    {
        [HttpGet] 
        public JsonResult Get(string BusID, string TeacherID, string Session)
        {
            if (!ValidateSession()) return SessionError;
            if (!(CurrentUser.ObjectId == TeacherID)) return UserGroupError;
            //user.UserGroup.BusID == BusID &&
            DBQuery BusQuery = new DBQuery();
            BusQuery.WhereEqualTo("objectId", BusID);
            BusQuery.WhereEqualTo("TeacherObjectID", TeacherID);
            if (DataBaseOperation.QueryMultipleData(BusQuery, out List<SchoolBusObject> BusList) == DBQueryStatus.INTERNAL_ERROR) return InternalError;

            if (BusList.Count == 0)
            {
                return DataBaseError;
            }
            DBQuery StudentQuery = new DBQuery();
            StudentQuery.WhereEqualTo("BusID", BusList[0].ObjectId);
            switch (DataBaseOperation.QueryMultipleData(StudentQuery, out List<StudentObject> StudentList))
            {
                case DBQueryStatus.INTERNAL_ERROR: return DataBaseError;
                default: return Json(new { StudentList.Count, StudentList });
            }
        }
    }
}