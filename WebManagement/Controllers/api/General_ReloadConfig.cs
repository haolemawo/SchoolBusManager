using Microsoft.AspNetCore.Mvc;

using WBPlatform.Config;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(refreshConfig)]
    public class Gen_ReLoadConfig : APIController
    {
        [HttpGet]
        public JsonResult Get(string UserID)
        {
            XConfig.LoadAll();
            return Json(new { Result = "OK" }); 
        }
    }
}
