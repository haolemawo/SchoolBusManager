using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route("api/users/Query")]
    public class User_QueryController : Controller
    {
        [HttpGet]
        public IEnumerable Get(string ColName, string EqualsTo)
        {
            throw new NotSupportedException("I forgot what this actually does...");
            //Dictionary<string, string> dict = new Dictionary<string, string>();
            //object Equals2Obj = EqualsTo;
            //if (int.TryParse((string)Equals2Obj, out int EqInt)) Equals2Obj = EqInt;
            //else if (((string)Equals2Obj).ToLower() == "true") Equals2Obj = true;
            //else if (((string)Equals2Obj).ToLower() == "false") Equals2Obj = false;
            //DBQuery query = new DBQuery();
            //query.WhereEqualTo(ColName, Equals2Obj);
            //
            //if (DataBaseOperation.QueryMultipleData(query, out List<UserObject> list) >= 0)
            //{
            //    dict.Add("ErrCode", "0");
            //    dict.Add("ErrMessage", "null");
            //    dict.Add("count", list.Count.ToString());
            //    foreach (UserObject userObj in list)
            //    {
            //        dict.Add("num_" + list.IndexOf(userObj).ToString() + "", userObj.ToString());
            //    }
            //}
            //else
            //{
            //    dict.Add("ErrCode", "999");
            //    dict.Add("ErrMessage", "Something went wrong...");
            //}
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //return dict;
        }
    }
}
