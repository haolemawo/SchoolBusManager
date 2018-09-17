using Microsoft.AspNetCore.Mvc;

using System.Collections;

using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;
using WBPlatform.WebManagement.Tools;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(getUNameRoute)]
    public class Gen_GetName : APIController
    {
        [HttpGet]
        public JsonResult Get(string UserID)
        {
            if (!ValidateSession()) return SessionError;
            if (string.IsNullOrEmpty(UserID)) return RequestIllegal;
            string uName = "";
            switch (DataBaseOperation.QuerySingle(new DBQuery().WhereIDIs(UserID), out UserObject user))
            {
                case DBQueryStatus.INTERNAL_ERROR:
                case DBQueryStatus.MORE_RESULTS:
                    return InternalError;
                case DBQueryStatus.NO_RESULTS:
                    uName = $"未知用户({UserID})";
                    break;
                default:
                    uName = $"{user.RealName}({user.ObjectId})";
                    break;
            }
            return Json(new { Name = uName });
        }
    }
}
