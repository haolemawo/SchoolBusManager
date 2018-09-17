using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;

using WBPlatform.Config;
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
            BusQuery.WhereIDIs(BusID);
            BusQuery.WhereEqualTo("TeacherObjectID", TeacherID);
            if (DataBaseOperation.QueryMultiple(BusQuery, out List<SchoolBusObject> BusList) != DBQueryStatus.ONE_RESULT) return InternalError;

            string[] weekType = XConfig.ServerConfig["WeekType"] == "big" ? new string[] { "0", "1", "2" } : new string[] { "0", "2" };

            switch (DataBaseOperation.QueryMultiple(new DBQuery().WhereEqualTo("BusID", BusList[0].ObjectId).WhereValueContainedInArray("WeekType", weekType), out List<StudentObject> StudentList))
            {
                case DBQueryStatus.INTERNAL_ERROR: return DataBaseError;
                case DBQueryStatus.INJECTION_DETECTED: return RequestIllegal;
                default: return Json(new { StudentList.Count, StudentList });
            }
        }
    }
}