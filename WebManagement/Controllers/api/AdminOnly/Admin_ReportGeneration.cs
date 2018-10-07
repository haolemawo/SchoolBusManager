
using Microsoft.AspNetCore.Mvc;

using System;

using WBPlatform.StaticClasses;
using WBPlatform.WebManagement.Tools;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(ADMIN_weekReportGenerate)]
    public class Admin_GenerateReport : APIController
    {
        [HttpGet]
        public JsonResult Get(string scope)
        {
            if (!ValidateSession()) return SessionError;
            if (!CurrentUser.IsAdmin) return UserGroupError;
            InternalMessage reportInternalMessage = new InternalMessage()
            {
                User = CurrentUser,
                _Type = InternalMessageTypes.Admin_WeekReport_Gen,
                DataObject = scope,
                Identifier = DateTime.Now.ToFileNameString()
            };
            MessagingSystem.AddMessageProcesses(reportInternalMessage);
            return SpecialisedInfo("请等待几分钟，报表将会发送到您的微信中！");
        }
    }
}

