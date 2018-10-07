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
            if (!CurrentUser.IsAdmin) return UserGroupError;

            type = type.ToLower();
            if (type == "big" || type == "small")
            {
                ServerConfig.Current["WeekType"] = type;
                ServerConfig.Current.SaveConfig();
            }
            else return RequestIllegal;
            return SpecialisedInfo("已经切换为：" + (ServerConfig.Current["WeekType"] == "big" ? "大周" : "小周"));
        }
    }
}

