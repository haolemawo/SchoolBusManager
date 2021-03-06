﻿using Microsoft.AspNetCore.Mvc;

using System;

using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;
using WBPlatform.WebManagement.Tools;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route(newBusReportRoute)]
    public class Bus_ReportController : APIController
    {
        [HttpGet]
        public JsonResult GET(string BusID, string TeacherID, string ReportType, string Content)
        {
            if (!ValidateSession()) return SessionError;
            if (TeacherID != CurrentUser.ObjectId) return RequestIllegal;
            if (DataBaseOperation.QuerySingle(new DBQuery().WhereIDIs(BusID).WhereEqualTo("TeacherObjectID", TeacherID), out SchoolBusObject bus) != DBQueryStatus.ONE_RESULT) return RequestIllegal;

            BusReport busReport = new BusReport
            {
                BusID = BusID,
                TeacherID = TeacherID,
                ReportType = (BusReportTypeE)Convert.ToInt32(ReportType),
                OtherData = Content
            };
            if (DataBaseOperation.CreateData(ref busReport) != DBQueryStatus.ONE_RESULT) return DataBaseError;
            
            InternalMessage message_TP = new InternalMessage(InternalMessageTypes.Bus_Status_Report, CurrentUser, busReport, BusID);
            MessagingSystem.AddMessageProcesses(message_TP);
            return Json(new { Report = busReport });
        }
    }
}
