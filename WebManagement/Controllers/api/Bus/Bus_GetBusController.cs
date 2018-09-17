using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Linq;
using WBPlatform.Config;
using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(getBusesRoute)]
    public class Bus_GetBusController : APIController
    {
        [HttpGet]
        public JsonResult Get(string UserID)
        {
            if (!ValidateSession()) return SessionError;
            if (CurrentUser.ObjectId != UserID || !CurrentUser.UserGroup.IsBusManager) return RequestIllegal;
            if (DataBaseOperation.QueryMultiple(new DBQuery().WhereEqualTo("TeacherObjectID", UserID), out List<SchoolBusObject> BusList) >= DBQueryStatus.NO_RESULTS)
            {
                if (BusList.Count == 0) BusList.Add(new SchoolBusObject() { ObjectId = "0000000000", BusName = "未找到班车", TeacherID = CurrentUser.ObjectId });
            }
            else return DataBaseError;
            int _LSChecked = 0, _CSChecked = 0, _AHChecked = 0, _DirectGoHome = 0;

            string[] weekType = XConfig.ServerConfig["WeekType"] == "big" ? new string[] { "0", "1", "2" } : new string[] { "0", "2" };

            if (DataBaseOperation.QueryMultiple(new DBQuery().WhereEqualTo("BusID", BusList[0].ObjectId).WhereValueContainedInArray("WeekType", weekType), out List<StudentObject> StudentList) >= DBQueryStatus.NO_RESULTS)
            {
                foreach (StudentObject item in StudentList)
                {
                    _LSChecked = item.LSChecked ? _LSChecked + 1 : _LSChecked;
                    _CSChecked = item.CSChecked ? _CSChecked + 1 : _CSChecked;
                    _AHChecked = item.AHChecked ? _AHChecked + 1 : _AHChecked;
                    _DirectGoHome = item.DirectGoHome == DirectGoHomeMode.DirectlyGoHome ? _DirectGoHome + 1 : _DirectGoHome;
                }
                return Json(new
                {
                    Bus = BusList[0],
                    AHChecked = _AHChecked,
                    LSChecked = _LSChecked,
                    CSChecked = _CSChecked,
                    DirectGoHome = _DirectGoHome,
                    Total = StudentList.Count
                });
            }
            else return InternalError;
        }
    }
}
