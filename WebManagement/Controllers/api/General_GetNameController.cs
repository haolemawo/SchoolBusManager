﻿using Microsoft.AspNetCore.Mvc;

using System.Collections;

using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;
using WBPlatform.WebManagement.Tools;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route("api/gen/GetName")]
    public class Gen_GetName : WebAPIController
    {
        [HttpGet]
        public IEnumerable Get(string UserID)
        {
            if (Sessions.OnSessionReceived(Request.Cookies["Session"], Request.Headers["User-Agent"], out UserObject SessionUser))
            {
                if (string.IsNullOrEmpty(UserID))
                {
                    return RequestIllegal; 
                }
                else
                {
                    switch (DatabaseOperation.QuerySingleData(new DBQuery().WhereEqualTo("objectId", UserID), out UserObject user))
                    {
                        case DBQueryStatus.INTERNAL_ERROR:
                        case DBQueryStatus.MORE_RESULTS: return InternalError;
                        case DBQueryStatus.NO_RESULTS: return SpecialisedInfo($"未知用户({UserID})");
                        default: return SpecialisedInfo($"{user.RealName}({user.objectId})");
                    }
                }
            }
            else return SessionError;
        }
    }
}
