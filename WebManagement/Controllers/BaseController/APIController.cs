using Microsoft.AspNetCore.Mvc;

using System;
using WBPlatform.StaticClasses;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using WBPlatform.Config;
using System.ComponentModel;
using System.Collections.Generic;

namespace WBPlatform.WebManagement.Controllers
{
    public class APIController : BaseController
    {
        protected const string userRegisterRoute = "api/users/Register";
        protected const string WeChat_Interface_Route = "api/WeChatMessage";
        protected const string userChangeRoute = "api/users/Change";
        protected const string getQRCode = "/api/QRCode";
        protected const string ADMIN_queryUserRoute = "/api/admin/QueryUsers";
        protected const string ADMIN_procUserRequestRoute = "/api/admin/ProcessUserRequest";

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

        protected static Dictionary<string, (string, string)> APIRoutes { get; private set; } = new Dictionary<string, (string, string)>
        {
            {"API_GetBuses", (getBusesRoute, "?UserID={0}&Session={1}") },
            {"API_GetName", (getUNameRoute, "?UserID={0}") },
            {"API_GetStudents", (getBusStudentsRoute, "?BusID={0}&TeacherID={1}&Session={2}") },
            {"API_QueryStudents", (queryStudentsRoute, "?BusID={0}&Column={1}&Content={2}") },
            {"API_BusIssueReport", (newBusReportRoute, "?BusID={0}&TeacherID={1}&ReportType={2}&Content={3}") },
            {"API_SignStudent", (signStudentsRoute, "?BusID={0}&Data={1}") },
            {"API_GetClassStudents", (getClassStudentsRoute, "?ClassID={0}&TeacherID={1}") },
            {"API_GetMyChildren",(getMyChildRoute, "?parentId={0}")  }
        };
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings() { ContractResolver = new DefaultContractResolver() };
        private class JsonResultEntity
        {
            public JsonResultEntity(int errCode, string errMessage)
            {
                ErrCode = errCode;
                ErrMessage = errMessage ?? throw new ArgumentNullException(nameof(errMessage));
                Data = new { };
            }
            public object Data { get; }
            public int ErrCode { get; }
            public string ErrMessage { get; }
        }
        private JsonResultEntity JSessionErrorEntity => new JsonResultEntity(1, XConfig.Messages["SessionError"]);
        private JsonResultEntity JDBErrorErrorEntity => new JsonResultEntity(995, XConfig.Messages["DataBaseError"]);
        private JsonResultEntity JPermissionDeniedEntity => new JsonResultEntity(996, XConfig.Messages["UserPermissionDenied"]);
        private JsonResultEntity JRequestIlligleEntity => new JsonResultEntity(999, XConfig.Messages["RequestIllegal"]);
        private JsonResultEntity JUnknownInternalExceptionEntity => new JsonResultEntity(999, XConfig.Messages["RequestIllegal"]);

        //There are some pre-defined JSON examples.
        //Errors
        public JsonResult SessionError => Json(JSessionErrorEntity);
        public JsonResult DataBaseError => Json(JDBErrorErrorEntity);
        public JsonResult UserGroupError => Json(JPermissionDeniedEntity);
        public JsonResult RequestIllegal => Json(JRequestIlligleEntity);

        //Infos.
        public JsonResult SpecialisedInfo(string Message) => Json(new JsonResultEntity(0, Message));

        //Unknown Internal Error.
        public JsonResult InternalError => Json(JUnknownInternalExceptionEntity);

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

        public new JsonResult Json(object data)
        {
            return Json(0, data, "null");
        }
    }
}
