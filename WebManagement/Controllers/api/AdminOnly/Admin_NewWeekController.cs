using Microsoft.AspNetCore.Mvc;

using WBPlatform.StaticClasses;
using WBPlatform.WebManagement.Tools;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(ADMIN_NewWeek)]
    public class Admin_NewWeek : APIController
    {
        [HttpGet]
        public JsonResult Get(string scope)
        {
            if (!ValidateSession()) return SessionError;
            if (!CurrentUser.IsAdmin) return UserGroupError;

            InternalMessage reportInternalMessage = new InternalMessage() { User = CurrentUser, _Type = InternalMessageTypes.Admin_WeekReport_Gen, DataObject = scope, Identifier = "##########" };
            InternalMessage resetDataInternalMessage = new InternalMessage() { User = CurrentUser, _Type = InternalMessageTypes.Admin_ResetAllRecord };
            MessagingSystem.AddMessageProcesses(reportInternalMessage, resetDataInternalMessage);

            return SpecialisedInfo("已经提交操作，操作结果将稍后发送给你");
        }
    }
}

