﻿using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;

using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(ADMIN_queryUserRoute)]
    public class Admin_QueryUsers : APIController
    {
        [HttpGet]
        public JsonResult Get(string columnName, string operand, string value)
        {
            if (ValidateSession())
            {
                if (CurrentUser.UserGroup.IsAdmin)
                {
                    string _column = (string)(columnName ?? "").DecodeAsObject();
                    string _operand = (string)(operand ?? "").DecodeAsObject();
                    string _value = (string)(value ?? "").DecodeAsObject();

                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    DBQuery query = new DBQuery();

                    if (_operand == "==") query.WhereEqualTo(_column, _value);
                    else if (operand.ToLower() == "contains") query.WhereRecordContainsValue(_column, _value);
                    else return RequestIllegal;

                    if (DataBaseOperation.QueryMultiple(query, out List<UserObject> users) >= 0)
                    {
                        dict.Add("count", users.Count.ToString());
                        for (int i = 0; i < users.Count; i++)
                            dict.Add("num_" + i.ToString(), users[i].ToString());
                        dict.Add("ErrCode", "0");
                        dict.Add("ErrMessage", "null");
                        return Json(dict);
                    }
                    else return DataBaseError;
                }
                else return UserGroupError;
            }
            else return SessionError;
        }
    }
}
