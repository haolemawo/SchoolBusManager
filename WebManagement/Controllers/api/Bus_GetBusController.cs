using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;

using WBPlatform.Database;
using WBPlatform.TableObject;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(getBusesRoute)]
    public class Bus_GetBusController : APIController
    {
        [HttpGet]
        public JsonResult Get(string UserID, string Session)
        {
            if (ValidateSession())
            {
                if (CurrentUser.ObjectId == UserID && CurrentUser.UserGroup.IsBusManager)//&& SessionUser.UserGroup.BusID == BusID
                {
                    switch (DataBaseOperation.QueryMultipleData(new DBQuery().WhereEqualTo("TeacherObjectID", UserID), out List<SchoolBusObject> BusList))
                    {
                        case DBQueryStatus.INTERNAL_ERROR: return InternalError;
                        default:
                            if (BusList.Count == 0)
                            {
                                BusList.Add(new SchoolBusObject() { ObjectId = "0000000000", BusName = "未找到校车", TeacherID = CurrentUser.ObjectId });
                            }
                            int _LSChecked = 0, _CSChecked = 0, _AHChecked = 0;
                            switch (DataBaseOperation.QueryMultipleData(new DBQuery().WhereEqualTo("BusID", BusList[0].ObjectId), out List<StudentObject> StudentList))
                            {
                                case DBQueryStatus.INTERNAL_ERROR: return InternalError;
                                default:
                                    foreach (StudentObject item in StudentList)
                                    {
                                        _LSChecked = item.LSChecked ? _LSChecked + 1 : _LSChecked;
                                        _CSChecked = item.CSChecked ? _CSChecked + 1 : _CSChecked;
                                        _AHChecked = item.AHChecked ? _AHChecked + 1 : _AHChecked;
                                    }
                                    var result = new
                                    {
                                        Bus = BusList[0],
                                        AHChecked = _AHChecked,
                                        LSChecked = _LSChecked,
                                        CSChecked = _CSChecked,
                                        Total = StudentList.Count
                                    };
                                    
                                    return Json(result);
                            }
                    }
                }
                else return RequestIllegal;
            }
            else return SessionError;
        }
    }
}
