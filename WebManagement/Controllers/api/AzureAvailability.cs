using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WBPlatform.Database.Connection;
using WBPlatform.StaticClasses;
using WBPlatform.WebManagement.Tools;

namespace WBPlatform.WebManagement.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/Azure/Availability")]
    public class AvailabilityController : APIController
    {
        [RequireHttps]
        [HttpGet]
        public IActionResult Get()
        {
            Response.StatusCode = 500;
            var m = WeChatMessageSystem.Status();
            if (!DatabaseSocketsClient.Connected)
            {
                return Json("DataBase Error");
            }
            else if (!MessagingSystem.GetStatus)
            {
                return Json("Messaging System Nor Working");
            }
            else if (!m.Item1)
            {
                return Json("WeChat Message RCVD Thread Not Working");
            }
            else if (!m.Item2)
            {
                return Json("WeChat Message SEND Thread Not Working");
            }
            else if (!WeChatMessageBackupService.GetStatus)
            {
                return Json("WeChat Message Backup Service Not Working");
            }
            else
            {
                Response.StatusCode = 200;
                return Json(418, StatusMonitor.ReportObject, "Status:Alive");
            }
        }
    }
}
