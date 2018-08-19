﻿using Microsoft.AspNetCore.Mvc;

using System;
using WBPlatform.StaticClasses;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using WBPlatform.Config;

namespace WBPlatform.WebManagement.Controllers

{
    public class APIController : BaseController
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings() { ContractResolver = new DefaultContractResolver() };
        private class JsonResultEntity
        {
            public JsonResultEntity(int errCode, string errMessage)
            {
                ErrCode = errCode;
                ErrMessage = errMessage ?? throw new ArgumentNullException(nameof(errMessage));
            }

            public int ErrCode { get; }
            public string ErrMessage { get; }
        }
        //There are some pre-defined JSON examples.
        //Errors
        public JsonResult SessionError => Json(new JsonResultEntity(1, XConfig.Messages["SessionError"]));
        public JsonResult DataBaseError => Json(new JsonResultEntity(995, XConfig.Messages["DataBaseError"]));
        public JsonResult UserGroupError => Json(new JsonResultEntity(996, XConfig.Messages["UserPermissionDenied"]));
        public JsonResult RequestIllegal => Json(new JsonResultEntity(999, XConfig.Messages["RequestIllegal"]));

        //Infos.
        public JsonResult SpecialisedInfo(string Message) => Json(new JsonResultEntity(0, Message));
        public JsonResult SpecialisedError(string Message) => Json(new JsonResultEntity(998, Message));

        //Unknown Internal Error.
        public JsonResult InternalError => Json(new JsonResultEntity(-1, XConfig.Messages["UnknownInternalException"]));

        public override JsonResult Json(object data) => base.Json(data, settings);
    }
}
