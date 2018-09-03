using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Linq;

using WBPlatform.Config;
using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace WBPlatform.WebManagement.Controllers
{
    public class MyChildController : ViewController
    {
        public const string ControllerName = "MyChild";
        public override IActionResult Index()
        {
            ViewData["where"] = HomeController.ControllerName;
            return ValidateSession()
                ? (CurrentUser.ChildList.Count <= 0 && !CurrentUser.UserGroup.IsParent)
                    ? PermissionDenied(ServerAction.MyChild_Index, XConfig.Messages["NotParent"], ResponceCode.Default)
                    : View()
                : LoginFailed("/" + ControllerName);
        }

        public IActionResult ParentCheck(string ID)
        {
            string BusID, BusTeacherID;
            ViewData["where"] = ControllerName;
            if (ValidateSession())
            {
                if (ID == null) return RequestIllegal(ServerAction.MyChild_MarkAsArrived, XConfig.Messages.ParameterUnexpected);
                string[] IDSplit = ID.Split(";");
                if (IDSplit.Length != 2) return RequestIllegal(ServerAction.MyChild_MarkAsArrived, XConfig.Messages.RequestIllegal);
                if (!CurrentUser.UserGroup.IsParent) return PermissionDenied(ServerAction.MyChild_MarkAsArrived, XConfig.Messages["NotParent"], ResponceCode.PermisstionDenied);

                BusID = IDSplit[0];
                BusTeacherID = IDSplit[1];

                List<StudentObject> ToBeSignedStudents = new List<StudentObject>();
                switch (DataBaseOperation.QueryMultiple(new DBQuery()
                    .WhereEqualTo("BusID", BusID)
                    .WhereEqualTo("CHChecked", false)
                    .WhereValueContainedInArray("ObjectId", CurrentUser.ChildList.ToArray())
                    .WhereEqualTo("TakingBus", true), out List<StudentObject> StudentListInBus))
                {
                    case DBQueryStatus.INTERNAL_ERROR: return DatabaseError(ServerAction.MyChild_MarkAsArrived, XConfig.Messages.InternalDataBaseError);
                    case DBQueryStatus.NO_RESULTS:
                    default:
                        ToBeSignedStudents.AddRange(from _stu in StudentListInBus where _stu.DirectGoHome != DirectGoHomeMode.DirectlyGoHome select _stu);
                        break;
                }

                ViewData["ChildCount"] = ToBeSignedStudents.Count;
                for (int i = 0; i < ToBeSignedStudents.Count; i++)
                {
                    ViewData["ChildNum_" + i.ToString()] = ToBeSignedStudents[i].ToParsedString();
                }
                ViewData["cBusID"] = BusID;
                ViewData["cTeacherID"] = BusTeacherID;
                return View();
            }
            else return LoginFailed("/" + ControllerName + "/ParentCheck?ID=" + ID);
        }

        public IActionResult DirectGoHome()
        {
            ViewData["where"] = ControllerName;
            if (ValidateSession())
            {
                if (!CurrentUser.UserGroup.IsParent) return PermissionDenied(ServerAction.MyChild_MarkAsArrived, XConfig.Messages["NotParent"], ResponceCode.PermisstionDenied);

                if (DataBaseOperation.QueryMultiple(new DBQuery()
                    .WhereEqualTo("DirectGoHome", 0)
                    .WhereValueContainedInArray("ObjectId", CurrentUser.ChildList.ToArray())
                    .WhereEqualTo("TakingBus", true), out List<StudentObject> ToBeSignedStudents) == DBQueryStatus.INTERNAL_ERROR)
                    return DatabaseError(ServerAction.MyChild_MarkAsArrived, XConfig.Messages.InternalDataBaseError);

                ViewData["ChildCount"] = ToBeSignedStudents.Count;
                return View(ToBeSignedStudents);
            }
            else return LoginFailed("/" + ControllerName + "/DirectGoHomeSign");
        }
    }
}