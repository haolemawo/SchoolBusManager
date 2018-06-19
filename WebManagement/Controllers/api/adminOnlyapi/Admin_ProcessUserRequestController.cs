﻿using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;
using WBPlatform.WebManagement.Tools;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route("api/admin/ProcessUserRequest")]
    public class Admin_ProcessUserRequest : Controller
    {
        [HttpGet]
        public IEnumerable Get(string reqId, string mode, string detail)
        {
            if (Sessions.OnSessionReceived(Request.Cookies["Session"], Request.Headers["User-Agent"], out UserObject SessionUser))
            {
                if (SessionUser.UserGroup.IsAdmin)
                {
                    switch (Database.Database.QuerySingleData(new DBQuery().WhereEqualTo("objectId", reqId), out UserChangeRequest request))
                    {
                        case DatabaseOperationResult.ONE_RESULT:
                            request.SolverID = SessionUser.objectId;
                            switch (mode)
                            {
                                case "0":
                                    //Accepted
                                    request.Status = UserChangeRequestProcessStatus.Accepted;
                                    break;
                                case "1":
                                    //Refused
                                    UserChangeRequestRefusedReasons reason = (UserChangeRequestRefusedReasons)Enum.Parse(typeof(UserChangeRequestRefusedReasons), detail);
                                    request.Status = UserChangeRequestProcessStatus.Refused;
                                    request.ProcessResultReason = reason;
                                    break;
                                default: return WebAPIResponseCollections.RequestIllegal;
                            }
                            if (Database.Database.UpdateData(request) == DatabaseOperationResult.INTERNAL_ERROR) return WebAPIResponseCollections.DatabaseError;
                            if (request.Status == UserChangeRequestProcessStatus.Accepted)
                            {
                                switch (Database.Database.QuerySingleData(new DBQuery().WhereEqualTo("objectId", request.UserID), out UserObject user))
                                {
                                    case DatabaseOperationResult.ONE_RESULT:
                                        switch (request.RequestTypes)
                                        {
                                            case UserChangeRequestTypes.真实姓名:
                                                user.RealName = request.NewContent;
                                                break;
                                            case UserChangeRequestTypes.手机号码:
                                                user.PhoneNumber = request.NewContent;
                                                break;
                                            default: return WebAPIResponseCollections.SpecialisedInfo("提交成功，部分内容需要手动修改");
                                        }
                                        MessagingSystem.onChangeRequest_Solved(request, SessionUser);
                                        break;
                                    default: return WebAPIResponseCollections.DatabaseError;
                                }
                            }
                            return WebAPIResponseCollections.SpecialisedInfo("提交成功");
                        case DatabaseOperationResult.NO_RESULTS:
                        case DatabaseOperationResult.MORE_RESULTS:
                        case DatabaseOperationResult.INTERNAL_ERROR:
                        default: return WebAPIResponseCollections.DatabaseError;
                    }
                }
                else return WebAPIResponseCollections.UserGroupError;
            }
            else return WebAPIResponseCollections.SessionError;
        }
    }
}
