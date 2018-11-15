using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;

using WBPlatform.Database;
using WBPlatform.TableObject;
using WBPlatform.Config;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(getMyChildRoute)]
    public class GetMyChildController : APIController
    {
        [HttpGet]
        public JsonResult Get(string parentId)
        {
            if (!ValidateSession()) return SessionError;
            if (!(CurrentUser.ObjectId == parentId && CurrentUser.UserGroup.IsParent)) return UserGroupError;
            string[] weekType = XConfig.ServerConfig.IsBigWeek() ? new string[] { "0", "1", "2" } : new string[] { "0", "2" };
            switch (DataBaseOperation.QueryMultiple(new DBQuery().WhereValueContainedInArray("objectId", CurrentUser.ChildList).WhereValueContainedInArray("WeekType", weekType), out List<StudentObject> StudentList))
            {
                case DBQueryStatus.INTERNAL_ERROR: return InternalError;
                default: return Json(new { StudentList.Count, StudentList });
            }
        }
    }
}