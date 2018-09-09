using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System.Collections.Generic;

using WBPlatform.Config;

namespace WBPlatform.WebManagement.Controllers
{
    public class APIController : BaseController
    {
        public readonly JsonResult SessionError;
        public readonly JsonResult DataBaseError;
        public readonly JsonResult UserGroupError;
        public readonly JsonResult RequestIllegal;
        //Unknown Internal Error.
        public readonly JsonResult InternalError;
        public APIController()
        {
            SessionError = Json(1, null, XConfig.Messages["SessionError"]);
            DataBaseError = Json(995, null, XConfig.Messages["DataBaseError"]);
            UserGroupError = Json(996, null, XConfig.Messages["UserPermissionDenied"]);
            RequestIllegal = Json(999, null, XConfig.Messages["RequestIllegal"]);
            InternalError = Json(999, null, XConfig.Messages["UnknownException"]);
        }
        protected const string userRegisterRoute = "api/users/Register";
        protected const string WeChat_Interface_Route = "api/WeChatMessage";
        protected const string userChangeRoute = "api/users/Change";
        protected const string getQRCode = "/api/QRCode";

        //JS Interactive
        protected const string getBusesRoute = "/api/bus/GetBuses";
        protected const string getUNameRoute = "/api/gen/GetName";
        protected const string getBusStudentsRoute = "/api/bus/GetStudents";
        protected const string queryStudentsRoute = "/api/bus/QueryStudents";
        protected const string newBusReportRoute = "/api/bus/NewIssueReport";
        protected const string signStudentsRoute = "/api/bus/SignStudents";
        protected const string getClassStudentsRoute = "/api/class/getStudents";
        protected const string getMyChildRoute = "/api/parent/getMyChild";
        protected const string getJSConfigRoute = "/api/CurrentUserConfig.js";
        protected const string setStudentState = "/api/student/SetState";
        protected const string refreshConfig = "/api/refreshConfig";
        protected const string ADMIN_queryUserRoute = "/api/admin/QueryUsers";
        protected const string ADMIN_procUserRequestRoute = "/api/admin/ProcessUserRequest";
        protected const string ADMIN_weekReportGenerate = "/api/admin/generateWeekReport";
        protected const string ADMIN_NewWeek = "/api/admin/newWeek";
        protected const string ADMIN_SendMessage = "/api/admin/sendMessage";

        protected static Dictionary<string, string> APIRoutes { get; private set; } = new Dictionary<string, string>
        {
            {"API_GetBuses",                getBusesRoute               +    "?UserID={0}&Session={1}" },
            {"API_GetName",                 getUNameRoute               +    "?UserID={0}" },
            {"API_GetStudents",             getBusStudentsRoute         +    "?BusID={0}&TeacherID={1}&Session={2}" },
            {"API_QueryStudents",           queryStudentsRoute          +    "?BusID={0}&Column={1}&Content={2}" },
            {"API_BusIssueReport",          newBusReportRoute           +    "?BusID={0}&TeacherID={1}&ReportType={2}&Content={3}" },
            {"API_SignStudent",             signStudentsRoute           +    "?BusID={0}&Data={1}" },
            {"API_GetClassStudents",        getClassStudentsRoute       +    "?ClassID={0}&TeacherID={1}" },
            {"API_GetMyChildren",           getMyChildRoute             +    "?parentId={0}"  },
            {"API_SetStudentState",         setStudentState             +    "?studentId={0}&state={1}" },
            {"API_SendMessage",             ADMIN_SendMessage           +    "?targ={0}&msg={1}"  },
            {"API_GenerateReport",          ADMIN_weekReportGenerate    +    "?scope={0}"  },
            {"API_RefreshConfig",           refreshConfig               +    ""  },
            {"API_NewWeek",                 ADMIN_NewWeek               +    ""  }
        };
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings() { ContractResolver = new DefaultContractResolver() };

        public JsonResult SpecialisedInfo(string Message) => Json(0, new { Message }, "null");

        public JsonResult Json(int ErrorCode, object data, string Message = "null")
        {
            var result = new
            {
                ErrCode = ErrorCode,
                ErrMessage = Message,
                Data = data
            };
            return base.Json(result, settings);
        }

        public new JsonResult Json(object data) => Json(0x0, data, "null");
    }
}
