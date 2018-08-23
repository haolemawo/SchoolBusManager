
using Microsoft.AspNetCore.Mvc;

using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(signStudentsRoute)]
    public class Bus_SignStudentsController : APIController
    {
        [HttpGet]
        public JsonResult GET(string BusID, string Data)
        {
            //THIS FUNCTION IS SHARED BY BUSTEACHER AND PARENTS
            if (!ValidateSession()) return SessionError;
            if (!CurrentUser.UserGroup.IsParent && !CurrentUser.UserGroup.IsBusManager) return UserGroupError;

            string str = Cryptography.Base64Decode(Data);
            if (!str.Contains(";")) return RequestIllegal;
            string[] DataCollection = str.Split(';');
            if (DataCollection.Length != 4) return RequestIllegal;

            switch (DataBaseOperation.QuerySingleData(new DBQuery().WhereEqualTo("objectId", BusID).WhereEqualTo("TeacherObjectID", DataCollection[2]), out SchoolBusObject Bus))
            {
                case DBQueryStatus.INTERNAL_ERROR: return InternalError;
                case DBQueryStatus.NO_RESULTS: return DataBaseError;
                default:
                    string StudentID = DataCollection[3];
                    DBQuery _stuQuery = new DBQuery();
                    _stuQuery.WhereEqualTo("objectId", StudentID);
                    _stuQuery.WhereEqualTo("BusID", BusID);
                    switch (DataBaseOperation.QuerySingleData(_stuQuery, out StudentObject Student))
                    {
                        case DBQueryStatus.INTERNAL_ERROR: return InternalError;
                        case DBQueryStatus.NO_RESULTS: return DataBaseError;
                        default:
                            if (!bool.TryParse(DataCollection[1], out bool Value)) return RequestIllegal;
                            string SType = DataCollection[0];
                            if (SType.ToLower() == "leave") Student.LSChecked = Value;
                            else if (SType.ToLower() == "pleave") Student.AHChecked = Value;
                            else if (SType.ToLower() == "come") Student.CSChecked = Value;
                            else return RequestIllegal;
                            if (DataBaseOperation.UpdateData(ref Student) == DBQueryStatus.ONE_RESULT)
                            {
                                var result = new { Student, SignMode = SType, SignResult = Value };
                                return Json(result);
                            }
                            else return DataBaseError;
                    }
            }
        }
    }
}
