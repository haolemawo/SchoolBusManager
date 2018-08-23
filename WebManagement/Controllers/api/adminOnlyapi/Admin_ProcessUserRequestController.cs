using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using WBPlatform.Database;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;
using WBPlatform.WebManagement.Tools;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(ADMIN_procUserRequestRoute)]
    public class Admin_ProcessUserRequest : APIController
    {
        [HttpGet]
        public JsonResult Get(string reqId, string mode, string detail)
        {
            if (!ValidateSession()) return SessionError;
            if (!CurrentUser.UserGroup.IsAdmin) return UserGroupError;
            if (DataBaseOperation.QuerySingleData(new DBQuery().WhereEqualTo("objectId", reqId), out UserChangeRequest request) != DBQueryStatus.ONE_RESULT) return DataBaseError;

            request.SolverID = CurrentUser.ObjectId;
            switch (mode)
            {
                case "0":
                    //Accepted
                    request.Status = UCRProcessStatus.Accepted;
                    break;
                case "1":
                    //Refused
                    UCRRefusedReasons reason = Enum.Parse<UCRRefusedReasons>(detail);
                    request.Status = UCRProcessStatus.Refused;
                    request.ProcessResultReason = reason;
                    break;
                default: return RequestIllegal;
            }

            if (DataBaseOperation.UpdateData(ref request) != DBQueryStatus.ONE_RESULT) return DataBaseError;

            if (request.Status != UCRProcessStatus.Accepted) return SpecialisedInfo("提交成功");

            if (DataBaseOperation.QuerySingleData(new DBQuery().WhereEqualTo("objectId", request.UserID), out UserObject user) != DBQueryStatus.ONE_RESULT) return DataBaseError;

            switch (request.RequestTypes)
            {
                case UserChangeRequestTypes.真实姓名:
                    user.RealName = request.NewContent;
                    break;
                case UserChangeRequestTypes.手机号码:
                    user.PhoneNumber = request.NewContent;
                    break;
                default:
                    return SpecialisedInfo("提交成功，部分内容需要手动修改");
            }
            if (DataBaseOperation.UpdateData(ref user) != DBQueryStatus.ONE_RESULT)
            {
                LW.E("Admin->UCRProcess: Failed to Save user data");
                return DataBaseError;
            }

            InternalMessage message_User = new InternalMessage() { _Type = GlobalMessageTypes.UCR_Procceed_TO_User, DataObject = request, User = user, ObjectId = request.UserID };
            MessagingSystem.AddMessageProcesses(message_User);

            return SpecialisedInfo("提交成功");
        }
    }
}

