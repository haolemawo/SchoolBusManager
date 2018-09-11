using Microsoft.AspNetCore.Mvc;

using WBPlatform.StaticClasses;
using WBPlatform.WebManagement.Tools;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(ADMIN_SendMessage)]
    public class Admin_SendMessage : APIController
    {
        [HttpGet]
        public JsonResult Get(string targ, string msg)
        {
            if (!ValidateSession()) return SessionError;
            if (!CurrentUser.UserGroup.IsAdmin) return UserGroupError;
            bool flag = targ == "bteachers" || targ == "cteachers" || targ == "parents" || targ == "all";
            if (!flag) return RequestIllegal;
            InternalMessage message = new InternalMessage() { DataObject = msg, Identifier = targ, User = CurrentUser, _Type = InternalMessageTypes.Admin_WeChat_SendMsg };
            MessagingSystem.AddMessageProcesses(message);
            return SpecialisedInfo("发送成功！");
        }
    }
}

