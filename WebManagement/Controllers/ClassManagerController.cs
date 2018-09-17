using Microsoft.AspNetCore.Mvc;

using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;
using WBPlatform.Config;

namespace WBPlatform.WebManagement.Controllers
{
    public class ClassManagerController : ViewController
    {
        public const string ControllerName = "ClassManager";
        public override IActionResult Index()
        {
            ViewData["where"] = HomeController.ControllerName;
            if (ValidateSession())
            {
                if (CurrentUser.UserGroup.IsClassTeacher && CurrentUser.ClassList.Count > 0)
                {
                    switch (DataBaseOperation.QuerySingle(new DBQuery().WhereIDIs(CurrentUser.ClassList[0]), out ClassObject myClass))
                    {
                        case DBQueryStatus.INTERNAL_ERROR: return DatabaseError(ServerAction.MyClass_Index, XConfig.Messages.InternalDataBaseError);
                        case DBQueryStatus.NO_RESULTS: return NotFoundError(ServerAction.MyClass_Index, XConfig.Messages["ClassNotFound"]);
                        case DBQueryStatus.ONE_RESULT:
                            ViewData["ClassName"] = string.Join(" ", myClass.CDepartment, myClass.CGrade, myClass.CNumber);
                            ViewData["ClassID"] = myClass.ObjectId;
                            return View();
                        default:
                            return DatabaseError(ServerAction.MyClass_Index, XConfig.Messages["MultipleClassIDsFound"]);
                    }
                }
                else return RequestIllegal(ServerAction.MyClass_Index, XConfig.Messages["NotClassTeacher"], ResponceCode.Default);
            }
            else return LoginFailed("/ClassManager/Index/");
        }
    }
}