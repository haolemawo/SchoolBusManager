using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text;

using WBPlatform.StaticClasses;

namespace WBPlatform.WebManagement.Controllers
{
    [ApiController]
    [Route(getJSConfigRoute)]
    public class GetConfigController : APIController
    {
        [HttpGet]
        public IActionResult Get()
        {
            string FileString = "";
            if (ValidateSession())
            {
                FileString += $"var keys = {APIRoutes.Keys.ToArray().ToParsedString()};\r\n";
                FileString += $"var values = {APIRoutes.Values.ToArray().ToParsedString()};\r\n";

                string apiTcket = Cryptography.RandomString(32, false).ToLower();
                CurrentIdentity.SetApiTicket(apiTcket);
                FileString += $"wbWeb.Initialise({CurrentUser.ToParsedString().Replace(CurrentUser.Password, "Looking for Password?")},\"{apiTcket}-{CurrentIdentity.Identity.Name}\", keys, values);\r\n";
                FileString += "json = null;";
            }
            else
            {
                FileString += "var wbWeb = \"Login Needed!\"";
            }
            byte[] fileBytes = Encoding.UTF8.GetBytes(FileString);
            return File(fileBytes, "application/javascript");
        }
    }
}
