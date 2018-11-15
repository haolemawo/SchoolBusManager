using Microsoft.AspNetCore.Mvc;
using WBPlatform.Config;
using WBPlatform.StaticClasses;
using WBPlatform.WebManagement.Tools;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(ADMIN_SetWeekType)]
    public class Admin_SetWeekType : APIController
    {
        [HttpGet]
        public JsonResult Get(string type)
        {
            if (!ValidateSession()) return SessionError;
            if (!CurrentUser.UserGroup.IsAdmin) return UserGroupError;

            type = type.ToLower();
            if (type == "big" || type == "small")
            {
                XConfig.ServerConfig.SetWeekType(type == "big");
                XConfig.ServerConfig.SaveConfig();
            }
            else return RequestIllegal;
            return SpecialisedInfo("已经切换为：" + (XConfig.ServerConfig.IsBigWeek() ? "大周" : "小周"));
        }
    }
}

