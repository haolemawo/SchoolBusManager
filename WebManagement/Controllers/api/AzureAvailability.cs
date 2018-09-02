using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WBPlatform.Database.Connection;
using WBPlatform.WebManagement.Tools;

namespace WBPlatform.WebManagement.Controllers
{
    [ApiController]
    [Route("api/Azure/Availability")]
    public class AvailabilityController : APIController
    {
        [RequireHttps]
        public void Get()
        {
            Response.StatusCode = 500;
            var m = WeChatMessageSystem.Status();
            if (!DatabaseSocketsClient.Connected)
            {
                Response.WriteAsync("DataBase Error");
            }
            else if (!MessagingSystem.GetStatus)
            {
                Response.WriteAsync("Messaging System Nor Working");
            }
            else if (!m.Item1)
            {
                Response.WriteAsync("WeChat Message RCVD Thread Not Working");
            }
            else if (!m.Item2)
            {
                Response.WriteAsync("WeChat Message SEND Thread Not Working");
            }
            else if (!WeChatMessageBackupService.GetStatus)
            {
                Response.WriteAsync("WeChat Message Backup Service Not Working");
            }
            else
            {
                Response.StatusCode = 208;
                Response.WriteAsync("Status:Alive");
            }
        }
    }
}
