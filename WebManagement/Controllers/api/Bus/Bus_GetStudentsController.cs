using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Linq;
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
            ////user.BusID == BusID &&
            //DBQuery BusQuery = new DBQuery();
            //BusQuery.WhereIDIs(BusID);
            //BusQuery.WhereEqualTo("TeacherObjectID", TeacherID);
            if (DataBaseOperation.QueryMultiple(b => b.ObjectId == BusID && b.Teacher.ObjectId == TeacherID, out List<SchoolBusObject> BusList) != DBQueryStatus.ONE_RESULT) return InternalError;

            string[] weekType = ServerConfig.Current["WeekType"] == "big" ? new string[] { "0", "1", "2" } : new string[] { "0", "2" };

            switch (DataBaseOperation.QueryMultiple(b => b.ObjectId == BusList[0].ObjectId && weekType.Contains(((int)b.WeekType).ToString()), out List<StudentObject> StudentList))
            {
                case DBQueryStatus.INTERNAL_ERROR: return DataBaseError;
                case DBQueryStatus.INJECTION_DETECTED: return RequestIllegal;
                default: return Json(new { StudentList.Count, StudentList });
            }
        }
    }
}