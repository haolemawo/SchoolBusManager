using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;

using WBPlatform.Database;
using WBPlatform.TableObject;

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


            Dictionary<string, string> dict = new Dictionary<string, string>();
            switch (DataBaseOperation.QueryMultiple(new DBQuery().WhereValueContainedInArray("objectId", CurrentUser.ChildList), out List<StudentObject> StudentList))
            {
                case DBQueryStatus.INTERNAL_ERROR: return InternalError;
                default: return Json(new { StudentList.Count, StudentList });
            }
        }
    }
}