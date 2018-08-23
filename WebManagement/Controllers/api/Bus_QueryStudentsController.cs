using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;

using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(queryStudentsRoute)]
    public class QueryStudentsController : APIController
    {
        [HttpGet]
        public JsonResult Get(string BusID, string Column, string Content)
        {
            switch (DataBaseOperation.QueryMultipleData(new DBQuery().WhereEqualTo("objectId", BusID), out List<SchoolBusObject> BusList))
            {
                case DBQueryStatus.INTERNAL_ERROR: return InternalError;
                case DBQueryStatus.NO_RESULTS: return DataBaseError;
                default:
                    {
                        object Equals2Obj = Content;
                        if (int.TryParse((string)Equals2Obj, out int EqInt)) Equals2Obj = EqInt;
                        else if (((string)Equals2Obj).ToLower() == "true") Equals2Obj = true;
                        else if (((string)Equals2Obj).ToLower() == "false") Equals2Obj = false;

                        DBQuery query2 = new DBQuery().WhereEqualTo("BusID", BusList[0].ObjectId).WhereEqualTo(Column, Equals2Obj);

                        switch (DataBaseOperation.QueryMultipleData(query2, out List<StudentObject> StudentList))
                        {
                            case DBQueryStatus.INTERNAL_ERROR: return InternalError;
                            default: return Json(0, new { StudentList.Count, StudentList }, "null");
                        }
                    }
            }
        }
    }
}
