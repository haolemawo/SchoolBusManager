﻿using cn.bmob.io;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using WBServicePlatform.StaticClasses;
using WBServicePlatform.TableObject;
using static WBServicePlatform.WebManagement.Program;

namespace WBServicePlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route("api/users/Register")]
    public class User_RegisterController : Controller
    {
        [HttpPost]
        public IEnumerable POST()
        {
            Request.Form[""].ToArray();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            return dict;
        }
    }
}