﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using WBPlatform.Config;

using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.Logging;
using WBPlatform.TableObject;
using WBPlatform.WebManagement.Tools;
using System.Linq;

namespace WBPlatform.WebManagement.Controllers
{
    public class ManageController : ViewController
    {
        public const string ControllerName = "Manage";
        public override IActionResult Index()
        {
            ViewData["where"] = HomeController.ControllerName;
            if (ValidateSession())
            {
                if (!CurrentUser.UserGroup.IsAdmin)
                {
                    L.E("Someone trying access illegal page!, Page: index, user:" + CurrentUser.UserName + ", possible referer:" + Request.Headers["Referer"]);
                    return RequestIllegal(ServerAction.Manage_Index, "试图访问管理页面", ResponceCode.PermisstionDenied);
                }
                return View(CurrentUser);
            }
            else return LoginFailed("/" + ControllerName);
        }
        public IActionResult SendMessage(string targ)
        {
            ViewData["where"] = ControllerName;
            if (ValidateSession())
            {
                if (!CurrentUser.UserGroup.IsAdmin)
                {
                    L.E("Someone trying access illegal page!, Page: messageSend, user:" + CurrentUser.UserName + ", possible referer:" + Request.Headers["Referer"]);
                    return RequestIllegal(ServerAction.Manage_Index, "试图访问管理页面", ResponceCode.PermisstionDenied);
                }
                return View();
                //return targ == "bteachers" || targ == "cteachers" || targ == "parents" || targ == "allusers" ?
                //    View() :
                //    RequestIllegal(ServerAction.Manage_Index, "请求所带参数无效：参数名 targ", ResponceCode.RequestIllegal);
            }
            else return LoginFailed("/" + ControllerName);
        }
        //public IActionResult UserManage(string mode, string from, string uid, string msg)
        //{
        //    ViewData["where"] = ControllerName;
        //    if (ValidateSession())
        //    {
        //        if (!CurrentUser.UserGroup.IsAdmin)
        //        {
        //            LW.E("Someone trying access illegal page!, Page: UserManage, user:" + CurrentUser.UserName + ", possible referer:" + Request.Headers["Referer"]);
        //            return NotFound();
        //        }
        //        ViewData["mode"] = mode;
        //        if (mode == "edit")
        //        {
        //            ViewData["from"] = from;
        //            string targetId = uid;
        //            string message = (string)PublicTools.DecodeObject(Encoding.UTF8.GetString(Convert.FromBase64String(msg ?? "")));
        //            ViewData["registerMsg"] = message;
        //            return DataBaseOperation.QuerySingleData(new DBQuery().WhereIDIs(uid), out UserObject _user) == DBQueryStatus.ONE_RESULT
        //                ? View(_user)
        //                : NotFoundError(ServerAction.INTERNAL_ERROR, XConfig.Messages["NoUserFoundByGivenID"]);
        //        }
        //        else if (mode == "query") return View();
        //        else throw new NotSupportedException("mode not supported!");
        //    }
        //    else return LoginFailed($"/Manage/UserManage?mode={mode}&from={from}&uid={uid}&msg={msg}");
        //}

        public IActionResult ChangeRequest(string arg, string reqId)
        {
            if (ValidateSession())
            {
                if (!string.IsNullOrEmpty(arg))
                {
                    switch (arg)
                    {
                        case "my":
                            ViewData["mode"] = "my";
                            ViewData["where"] = AccountController.ControllerName;
                            if (string.IsNullOrEmpty(reqId))
                            {
                                // MY LIST
                                switch (DataBaseOperation.QueryMultiple(new DBQuery().WhereEqualTo("UserID", CurrentUser.ObjectId), out List<UserChangeRequest> requests))
                                {
                                    case DBQueryStatus.INTERNAL_ERROR: return DatabaseError(ServerAction.General_ViewChangeRequests, XConfig.Messages.InternalDataBaseError);
                                    default:
                                        ViewData["count"] = requests.Count;
                                        ViewData["list"] = requests.ToArray();
                                        return base.View();
                                }
                            }
                            else
                            {
                                // MY SINGLE Viewer
                                switch (DataBaseOperation.QuerySingle(new DBQuery().WhereEqualTo("UserID", CurrentUser.ObjectId).WhereIDIs(reqId), out UserChangeRequest requests))
                                {
                                    case DBQueryStatus.INTERNAL_ERROR:
                                    case DBQueryStatus.NO_RESULTS:
                                    case DBQueryStatus.MORE_RESULTS:
                                        return DatabaseError(ServerAction.General_ViewChangeRequests, XConfig.Messages.InternalDataBaseError);
                                    default:
                                        return base.View(requests);
                                }
                            }
                        case "manage":
                            ViewData["where"] = ControllerName;
                            ViewData["mode"] = "manage";
                            if (!CurrentUser.UserGroup.IsAdmin)
                            {
                                L.E("Someone trying access illegal page!, Page: changeRequest: arg=manage, user:" + CurrentUser.UserName + ", possible referer:" + Request.Headers["Referer"]);
                                return NotFound();
                            }
                            if (string.IsNullOrEmpty(reqId))
                            {
                                switch (DataBaseOperation.QueryMultiple(new DBQuery(), out List<UserChangeRequest> requests))
                                {
                                    case DBQueryStatus.INTERNAL_ERROR:
                                        return DatabaseError(ServerAction.Manage_VerifyChangeRequest, XConfig.Messages.InternalDataBaseError);
                                    default:
                                        ViewData["list"] = requests.ToArray();
                                        return base.View();
                                }
                            }
                            else
                            {
                                switch (DataBaseOperation.QuerySingle(new DBQuery().WhereIDIs(reqId), out UserChangeRequest requests))
                                {
                                    case DBQueryStatus.INTERNAL_ERROR:
                                    case DBQueryStatus.NO_RESULTS:
                                    case DBQueryStatus.MORE_RESULTS:
                                        return DatabaseError(ServerAction.Manage_VerifyChangeRequest, XConfig.Messages.InternalDataBaseError);
                                    default:
                                        return View(requests);
                                }
                            }
                        default: return RequestIllegal(ServerAction.General_ViewChangeRequests, XConfig.Messages.ParameterUnexpected);
                    }
                }
                else return RequestIllegal(ServerAction.General_ViewChangeRequests, XConfig.Messages.ParameterUnexpected);
            }
            else return LoginFailed("/" + ControllerName + "/ChangeRequest?arg=" + arg + "&reqId=" + reqId);
        }
    }
}
