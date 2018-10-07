using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using WBPlatform.Database;
using WBPlatform.Config;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;
using WBPlatform.WebManagement.Tools;

namespace WBPlatform.WebManagement.Controllers
{
    public class BusManagerController : ViewController
    {
        public const string ControllerName = "BusManager";
        public override IActionResult Index()
        {
            ViewData["where"] = HomeController.ControllerName;
            return ValidateSession()
                ? CurrentUser.IsBusManager
                    ? View()
                    : PermissionDenied(ServerAction.BusManage_Index, XConfig.Messages["NotBusTeacher"], ResponceCode.Default)
                : LoginFailed("/" + ControllerName + "/");
        }

        public IActionResult WeekIssue()
        {
            ViewData["where"] = ControllerName;
            return ValidateSession()
                ? View()
                : LoginFailed("/" + ControllerName + "/WeekIssue");
        }

        public IActionResult SignStudent(string signmode)
        {
            ViewData["where"] = ControllerName;
            ViewData["SignMode"] = signmode;
            if (ValidateSession())
            {
                ViewData["cUser"] = CurrentUser.ToString();
                if (Request.Cookies["SignMode"] == signmode)
                {
                    DataBaseOperation.QuerySingle(b => b.Teacher.ObjectId == CurrentUser.ObjectId  /*new DBQuery().WhereEqualTo("TeacherObjectID", CurrentUser.ObjectId)*/, out SchoolBusObject busObject);
                    if (busObject == null)
                    {
                        busObject = new SchoolBusObject() { ObjectId = "0000000000", BusName = "未找到班车", Teacher = CurrentUser };
                    }
                    ViewData["cBus"] = busObject.ObjectId;
                    ViewData["mode"] = signmode;
                    return View();
                }
                else return RequestIllegal(ServerAction.BusManage_SignStudents, XConfig.Messages["TokenTimeout"]);
            }
            else
            {
                return LoginFailed("/" + ControllerName + "/SignStudent?signmode=" + signmode);
            }
        }

        public IActionResult ArriveHomeScan()
        {
            ViewData["where"] = ControllerName;
            if (ValidateSession())
            {
                if (CurrentUser.IsBusManager)
                {
                    DataBaseOperation.QuerySingle(s => s.Teacher.ObjectId == CurrentUser.ObjectId, out SchoolBusObject busObject);
                    ViewData["cBus"] = busObject.ObjectId;
                    ViewData["cTeacher"] = CurrentUser.ObjectId;
                }
                else return PermissionDenied(ServerAction.BusManage_CodeGenerate, XConfig.Messages.UserPermissionDenied);
            }
            else
            {
                return LoginFailed("/" + ControllerName + "/ArriveHomeScan");
            }
            return View();
        }

        private IActionResult CheckFlag(DBQueryStatus flag, bool isSingleRequest, string info)
        {
            switch (flag)
            {
                case DBQueryStatus.INTERNAL_ERROR: return DatabaseError(ServerAction.General_ViewStudent, string.Join("", info, ":", flag));
                case DBQueryStatus.MORE_RESULTS:
                    if (isSingleRequest) DatabaseError(ServerAction.General_ViewStudent, string.Join("", info, ":", flag));
                    return null;
                default: return null;
            }
        }
        /// <summary>
        /// TODO: If Parents Does not exist.. or Bus does not exist,, give a special warning instead of an functionless ERROR....
        /// </summary>
        /// <param name="StudentID"></param>
        /// <param name="ClassID"></param>
        /// <param name="BusID"></param>
        /// <returns></returns>
        public IActionResult ViewStudent(string StudentID, string ClassID, string BusID, string from)
        {
            if (ValidateSession())
            {
                if (string.IsNullOrEmpty(from))
                {
                    return RequestIllegal(ServerAction.General_ViewStudent, XConfig.Messages.RequestIllegal);
                }

                ViewData["where"] = from;
                // User Group Check
                if (CurrentUser.IsParent || CurrentUser.IsClassTeacher || CurrentUser.IsBusManager || CurrentUser.IsAdmin)
                {
                    DBQueryStatus flag;
                    IActionResult result = null;

                    ViewStudentInfo info = new ViewStudentInfo();

                    //Search student with spec ClassID and StudentID and BusID
                    flag = DataBaseOperation.QuerySingle(s => s.ObjectId == StudentID && s.Class.ObjectId == ClassID && s.Bus.ObjectId == BusID /*new DBQuery().WhereIDIs(StudentID).WhereEqualTo("ClassID", ClassID).WhereEqualTo("BusID", BusID)*/, out StudentObject Student);
                    result = CheckFlag(flag, true, "GetStudentBy_CID_BID_SID");
                    if (result != null) return result;
                    if (Student != null)
                    {
                        info.StudentFound = true;
                        info._student = Student;

                        //Get Class information with ClassID
                        flag = DataBaseOperation.QuerySingle(c => c.ObjectId == Student.Class.ObjectId /*new DBQuery().WhereIDIs(Student.Class.ObjectId)*/, out ClassObject Class);
                        result = CheckFlag(flag, true, "GetClassBy_CID");
                        if (result != null) return result;
                        else
                        {
                            if (flag == DBQueryStatus.NO_RESULTS)
                            {
                                info.ClassFound = false;
                                info._class = null;
                            }
                            else
                            {
                                info.ClassFound = true;
                                info._class = Class;
                                //Get Class Teacher Information
                                flag = DataBaseOperation.QuerySingle(t => t.ObjectId == Class.Teacher.ObjectId/*new DBQuery().WhereIDIs(Class.Teacher.ObjectId)*/, out UserObject Teacher);
                                result = CheckFlag(flag, true, "GetClassTeacherBy_CID_BID_SID");
                                if (result != null) return result;
                                else
                                {
                                    if (flag == DBQueryStatus.NO_RESULTS)
                                    {
                                        info.ClassTeacherFound = false;
                                        info._CTeacher = null;
                                    }
                                    else
                                    {
                                        info.ClassTeacherFound = true;
                                        info._CTeacher = Teacher;
                                    }
                                }
                            }
                        }

                        //Get Parents
                        flag = DataBaseOperation.QueryMultiple(p => p.ChildList.Contains(Student.ObjectId), out List<UserObject> Parents);
                        result = CheckFlag(flag, false, "GetParentsBy_UID");
                        if (result != null) return result;
                        else
                        {
                            if (flag == DBQueryStatus.NO_RESULTS)
                            {
                                info.ParentsCount = 0;
                                info._Parents = null;
                            }
                            else
                            {
                                info.ParentsCount = Parents.Count;
                                info._Parents = Parents.ToArray();
                            }
                        }


                        // Get SchoolBus
                        flag = DataBaseOperation.QuerySingle(b => b.ObjectId == Student.Bus.ObjectId, out SchoolBusObject Bus);
                        result = CheckFlag(flag, true, "GetBusBy_BID");
                        if (result != null) return result;
                        else
                        {
                            if (flag == DBQueryStatus.NO_RESULTS)
                            {
                                info.BusFound = false;
                                info._schoolbus = null;
                            }
                            else
                            {
                                info.BusFound = true;
                                info._schoolbus = Bus;
                                // Get SchoolBus Teacher.
                                flag = DataBaseOperation.QuerySingle(u => u.ObjectId == Bus.Teacher.ObjectId, out UserObject BusTeacher);
                                result = CheckFlag(flag, true, "GetBusTeacherBy_UID");
                                if (result != null) return result;
                                else
                                {
                                    if (flag == DBQueryStatus.NO_RESULTS)
                                    {
                                        info.BusTeacherFound = false;
                                        info._BTeacher = null;
                                    }
                                    else
                                    {
                                        info.BusTeacherFound = true;
                                        info._BTeacher = BusTeacher;
                                    }
                                }
                            }
                        }

                        //        Is in user's class?                           Is in user's Bus??                      Is user's child??                Or the god...
                        return /*CurrentUser.ClassList.Contains(Student.Class.ObjectId) || */CurrentUser.ObjectId == Bus.Teacher.ObjectId || CurrentUser.ChildList.Contains(Student.ObjectId) || CurrentUser.IsAdmin
                            ? View(info)
                            : PermissionDenied(ServerAction.General_ViewStudent, XConfig.Messages.UserPermissionDenied);
                    }
                    else return DatabaseError(ServerAction.General_ViewStudent, XConfig.Messages["WrongDataReturnedFromDatabase"]);
                }
                else return PermissionDenied(ServerAction.General_ViewStudent, XConfig.Messages.UserPermissionDenied);
            }

            //Return to Home because this is privacy-related function
            else
            {

                return LoginFailed("/");
            }
        }
    }
}
