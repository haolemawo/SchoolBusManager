using ClosedXML.Excel;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using WBPlatform.Config;
using WBPlatform.Database;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace WBPlatform.WebManagement.Tools
{
    public static class MessagingSystem
    {
        public static int GetCount => MessageList.Count;
        public static bool GetStatus => ProcThread.IsAlive;
        private static ConcurrentQueue<InternalMessage> MessageList { get; set; } = new ConcurrentQueue<InternalMessage>();
        private static Thread ProcThread = new Thread(new ThreadStart(_ProcThread));

        public static void StartProcessThread()
        {
            ProcThread.Start();
            L.I("CoreMessaging System Started!");
        }

        public static void AddMessageProcesses(params InternalMessage[] message) { foreach (var item in message) { MessageList.Enqueue(item); } }

        private static void _ProcThread()
        {
            while (true)
            {
                if (!MessageList.TryDequeue(out InternalMessage message)) Thread.Sleep(500);
                else if (!_ProcessMessage(message)) MessageList.Enqueue(message);
                else Thread.Sleep(100);
            }
        }

        private static bool _ProcessMessage(InternalMessage message)
        {
            switch (message._Type)
            {
                case GlobalMessageTypes.UCR_Created_TO_ADMIN: return ProcessUCR_CreateToAdmin(message);
                case GlobalMessageTypes.UCR_Created__TO_User: return ProcessUCR_CreatedToUser(message);
                case GlobalMessageTypes.UCR_Procceed_TO_User: return ProcessUCRToUser(message);
                case GlobalMessageTypes.User__Pending_Verify: return UserVerify(message);
                case GlobalMessageTypes.Admin_WeekReport_Gen: return GenerateWeekReport(message);
                case GlobalMessageTypes.Admin_ResetAllRecord: return ResetRecord(message);
                case GlobalMessageTypes.Admin_WeChat_SendMsg: return SendMessage(message);

                case GlobalMessageTypes.Bus_Status_Report_TC:
                case GlobalMessageTypes.Bus_Status_Report_TP:
                    return BusIssueReport(message);

                case GlobalMessageTypes.User__Finishd_Verify:
                case GlobalMessageTypes.UCR_Procced_TO_ADMIN:
                    //throw new NotSupportedException("鬼知道你干嘛要调这个函数");
                    return true;


                default: throw new NotSupportedException("不支持就是不支持……");
            }
        }

        private static bool SendMessage(InternalMessage message)
        {
            string MessageString = message.DataObject as string;
            if (DataBaseOperation.QueryMultiple(new DBQuery().Limit(5000), out List<UserObject> _usr) <= 0) { L.E("No Users Found???"); return false; }
            WeChatSentMessage wxMsg = new WeChatSentMessage(WeChatSMsg.text, null, "来自管理员 " + message.User.RealName + "的消息：\r\n" + MessageString, null, null);
            switch (message.Identifier)
            {
                case "all":
                    wxMsg.toUser = (from _ in _usr select _.UserName).ToArray();
                    break;
                case "bteachers":
                    wxMsg.toUser = (from _ in _usr where _.UserGroup.IsBusManager select _.UserName).ToArray();
                    break;
                case "cteachers":
                    wxMsg.toUser = (from _ in _usr where _.UserGroup.IsClassTeacher select _.UserName).ToArray();
                    break;
                case "parents":
                    wxMsg.toUser = (from _ in _usr where _.UserGroup.IsParent select _.UserName).ToArray();
                    break;
                default:
                    L.E("Unknown SendMessage Identifier " + message.Identifier);
                    return true;
            }
            WeChatMessageSystem.AddToSendList(wxMsg);
            return true;
        }

        private static bool ResetRecord(InternalMessage message)
        {
            if (DataBaseOperation.QueryMultiple(new DBQuery().Limit(5000), out List<SchoolBusObject> _s) <= 0) { L.E("No Bus Found???"); return false; }
            foreach (var item in _s)
            {
                item.AHChecked = false;
                item.CSChecked = false;
                item.LSChecked = false;
                if (DataBaseOperation.UpdateData(item) == DBQueryStatus.ONE_RESULT) L.I("Succeed Reset Record: Bus->" + item.BusName + ":" + item.ObjectId);
                else { L.E("Failed To Reset Bus Record: " + item.ToParsedString()); return false; }
            }
            if (DataBaseOperation.QueryMultiple(new DBQuery().Limit(5000), out List<StudentObject> _st) <= 0) { L.E("No Students Found???"); return false; }
            foreach (var item in _st)
            {
                item.AHChecked = false;
                item.CSChecked = false;
                item.LSChecked = false;
                if (DataBaseOperation.UpdateData(item) == DBQueryStatus.ONE_RESULT) L.I("Succeed Reset Record: Stu->" + item.StudentName + ":" + item.ObjectId);
                else { L.E("Failed To Reset Student Record: " + item.ToParsedString()); return false; }
            }
            WeChatSentMessage msgReset = new WeChatSentMessage(WeChatSMsg.text, null, "操作：开始新一周记录 已经完成！", null, message.User.UserName);
            WeChatMessageSystem.AddToSendList(msgReset);
            return true;
        }
        private static bool GenerateWeekReport(InternalMessage message)
        {
            var dirInfo = Directory.CreateDirectory("reports");
            string ReportVisibleName = "班车统计报告";
            string ReportFilePath = dirInfo.FullName + "//" + message.DataObject + "-GenBy-" + message.User.GetIdentifiableCode() + "-At-" + message.Identifier + ".xlsx";

            if (DataBaseOperation.QueryMultiple(new DBQuery().Limit(5000), out List<SchoolBusObject> _s) <= 0) { L.E("No Bus Found???"); return false; }
            if (DataBaseOperation.QueryMultiple(new DBQuery().Limit(5000), out List<StudentObject> _st) <= 0) { L.E("No Students Found???"); return false; }
            if (DataBaseOperation.QueryMultiple(new DBQuery().Limit(5000), out List<ClassObject> _c) <= 0) { L.E("No Classes Found???"); return false; }
            if (DataBaseOperation.QueryMultiple(new DBQuery().Limit(5000), out List<UserObject> _usr) <= 0) { L.E("No Users Found???"); return false; }

            Dictionary<string, SchoolBusObject> SchoolBusDictionary = _s.ToDictionary();
            Dictionary<string, ClassObject> ClassDictionary = _c.ToDictionary();
            Dictionary<string, StudentObject> StudentsDictionary = _st.ToDictionary();
            Dictionary<string, UserObject> UsersDictionary = _usr.ToDictionary();

            void FillStudentsDataIntoSheet(IXLWorksheet sheet, StudentObject[] _stus)
            {
                sheet.Row(1).SetValues("学部", "年级", "班级", "班主任", "学生姓名", "性别", "班车方向", "本周是否坐班车", "带车老师", "离校签到", "返校签到", "免接送状态", "到家签到", "家长信息");
                for (int i = 0; i < _stus.Length; i++)
                {
                    var _student = _stus[i];
                    if (_stus[i].ClassID == null)
                    {
                        continue;
                    }
                    var _class = ClassDictionary[_stus[i].ClassID];
                    if (_class.TeacherID == null)
                    {
                        continue;
                    }
                    var _classTeacher = UsersDictionary[_class.TeacherID];
                    var _bus = SchoolBusDictionary[_student.BusID];
                    var _busTeacher = UsersDictionary[_bus.TeacherID];
                    var _parents = from k in UsersDictionary where k.Value.ChildList.Contains(_student.ObjectId) select k.Value;

                    List<string> values = new List<string>
                    {
                        _class.CDepartment,_class.CGrade,_class.CNumber,
                        _classTeacher.RealName,
                        _student.StudentName,
                        _student.Sex == "M" ? "男" : "女",
                        _bus.BusName,
                        _student.TakingBus ? "是" : "否"
                    };

                    if (_student.TakingBus)
                    {
                        values.Add(_busTeacher.RealName);
                        values.Add(_student.LSChecked ? "签到" : "未签到");
                        values.Add(_student.AHChecked ? "签到" : "未签到");
                    }
                    else values.AddRange(new string[] { "---", "---", "---" });

                    values.Add(_student.DirectGoHome == DirectGoHomeMode.NotSet ? "未设置" : _student.DirectGoHome == DirectGoHomeMode.DirectlyGoHome ? "免接送" : "非免接送");
                    values.Add(_student.TakingBus ? (_student.CSChecked ? "签到" : "未签到") : "---");
                    values.Add(string.Join(';', from _ in _parents select _.RealName + "(" + _.PhoneNumber + ")"));
                    sheet.Row(i + 2).SetValues(values.ToArray());
                }
                sheet.Columns().AdjustToContents();
                sheet.SetContentColor();
                sheet.SetHeaderColor();
                sheet.SetGrid();
            }
            XLWorkbook wb = new XLWorkbook();
            wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            wb.CalculateMode = XLCalculateMode.Auto;
            wb.Properties.Author = "WoodenBench For SchoolBus Service";
            wb.Properties.Category = "Weekly Report File";
            switch (((message.DataObject as string) ?? "all").ToLower())
            {
                case "class":
                    ReportVisibleName += "(按班级分类).xlsx";
                    foreach (var _class in ClassDictionary.Values)
                    {
                        string sheetName = _class.CDepartment + "-" + _class.CGrade + "-" + _class.CNumber;
                        if (wb.Worksheets.Contains(sheetName))
                        {
                            sheetName += Cryptography.CurrentTimeStamp;
                        }
                        var sheetClass = wb.Worksheets.Add(sheetName);
                        FillStudentsDataIntoSheet(sheetClass, (from x in StudentsDictionary.Values where x.ClassID == _class.ObjectId select x).ToArray());
                    }
                    break;
                case "bus":
                    ReportVisibleName += "(按班车分类).xlsx";
                    foreach (var _bus in SchoolBusDictionary.Values)
                    {
                        if (wb.Worksheets.Contains(_bus.BusName))
                        {
                            _bus.BusName += Cryptography.CurrentTimeStamp;
                        }
                        var sheetBus = wb.Worksheets.Add(_bus.BusName);
                        FillStudentsDataIntoSheet(sheetBus, (from x in StudentsDictionary.Values where x.BusID == _bus.ObjectId select x).ToArray());
                    }
                    break;
                case "all":
                    ReportVisibleName += "(总表).xlsx";
                    var sheetAll = wb.Worksheets.Add("总学生表");
                    FillStudentsDataIntoSheet(sheetAll, StudentsDictionary.Values.ToArray());
                    break;
                default: L.E("WTF This Week Report Scope is???? " + message.DataObject); return true;
            }
            wb.SaveAs(ReportFilePath);
            var mediaId = WC_FileUploader.Upload(ReportFilePath, ReportVisibleName);
            WeChatSentMessage fileMsg = new WeChatSentMessage(WeChatSMsg.file, null, mediaId, null, message.Identifier == "##########" ? "liuhaoyu" : message.User.UserName);
            WeChatMessageSystem.AddToSendList(fileMsg);
            if (message.Identifier != "##########")
            {
                WeChatSentMessage msg2 = new WeChatSentMessage(WeChatSMsg.text, null, "报表已经准备好了，请查看！\r\n提示：报表数据量较大，建议发送到电脑或使用第三方查看器打开", null, message.User.UserName);
                WeChatMessageSystem.AddToSendList(msg2);
            }
            return true;
        }

        private static bool ProcessUCR_CreateToAdmin(InternalMessage message)
        {
            if ((int)GetAdminUsers(out List<UserObject> adminUsers_UCR_Request) < 1)
            {
                L.W("No Administrator found!! thus no UserRequest can be solved!");
                return false;
            }
            WeChatSentMessage UCR_Created_TO_ADMIN_Msg = new WeChatSentMessage(
                WeChatSMsg.textcard,
                "管理员通知",
                $"你有一条来自 {message.User.RealName} 的工单有待处理：\r\n{message.User.RealName}请求把 {((UserChangeRequest)message.DataObject).RequestTypes.ToString()} 修改成{((UserChangeRequest)message.DataObject).NewContent }\r\n请尽快处理！",
                XConfig.Current.WebSiteAddress + "/Manage/ChangeRequest?arg=manage&reqId=" + message.Identifier,
                (from usr in adminUsers_UCR_Request select usr.UserName).ToArray());
            WeChatMessageSystem.AddToSendList(UCR_Created_TO_ADMIN_Msg);
            return true;
        }
        private static bool ProcessUCR_CreatedToUser(InternalMessage message)
        {
            WeChatSentMessage UCR_Created_TO_User_Msg = new WeChatSentMessage(WeChatSMsg.textcard, "工单提交成功！",
                            "你申请修改账户 " + ((UserChangeRequest)message.DataObject).RequestTypes.ToString() + " 信息的工单已经提交成功！\r\n" +
                            "工单编号：" + ((UserChangeRequest)message.DataObject).ObjectId + "\r\n" +
                            "状态：正在等待审核", XConfig.Current.WebSiteAddress + "/Manage/ChangeRequest?arg=my&reqId=" + message.Identifier, message.User.UserName);
            WeChatMessageSystem.AddToSendList(UCR_Created_TO_User_Msg);
            return true;
        }
        private static bool ProcessUCRToUser(InternalMessage message)
        {
            switch (DataBaseOperation.QuerySingle(new DBQuery().WhereEqualTo("objectId", message.Identifier), out UserObject requestSender))
            {
                case DBQueryStatus.ONE_RESULT:
                    string stat = ((UserChangeRequest)message.DataObject).Status == UCRProcessStatus.Accepted ? "审核通过" : "未通过";
                    WeChatSentMessage _WMessage = new WeChatSentMessage(WeChatSMsg.textcard, "工单状态提醒",
                        "你申请修改账户 " + ((UserChangeRequest)message.DataObject).RequestTypes.ToString() + " 信息的工单发生了状态变动！\r\n" +
                            "工单编号：" + ((UserChangeRequest)message.DataObject).ObjectId + "\r\n" +
                            "审核结果：" + stat + "\r\n请点击查看详细内容", XConfig.Current.WebSiteAddress + "/Manage/ChangeRequest?arg=my&reqId=" + ((UserChangeRequest)message.DataObject).ObjectId, requestSender.UserName);
                    WeChatMessageSystem.AddToSendList(_WMessage);
                    return true;
                case DBQueryStatus.INTERNAL_ERROR:
                case DBQueryStatus.NO_RESULTS:
                case DBQueryStatus.MORE_RESULTS:
                default:
                    L.W("Failed to get user who requested to change something.... userId=" + message.Identifier);
                    return false;
            }
        }
        private static bool UserVerify(InternalMessage message)
        {
            if ((int)GetAdminUsers(out List<UserObject> adminUsers_createUser) < 1)
            {
                L.W("No Administrator found!! thus no Register Request can be solved!");
                return false;
            }
            string escapedString = (string)PublicTools.EncodeString(message.DataObject.ToString());
            string URL = Convert.ToBase64String(Encoding.UTF8.GetBytes(escapedString), Base64FormattingOptions.None);
            WeChatSentMessage userVerifyMsg = new WeChatSentMessage(WeChatSMsg.textcard, "新用户注册审核通知",
                $"有一位新用户在{message.User.CreatedAt.ToString()}申请了注册用户，请审核！\r\n提供的姓名：{message.User.RealName}\r\n手机号码：{message.User.PhoneNumber}",
                XConfig.Current.WebSiteAddress + "/Manage/UserManage?from=userCreate&mode=edit&uid=" + message.User.ObjectId + "&msg=" + URL,
                (from usr in adminUsers_createUser select usr.UserName).ToArray());
            WeChatMessageSystem.AddToSendList(userVerifyMsg);
            return true;
        }
        private static bool BusIssueReport(InternalMessage message)
        {
            var _report = message.DataObject as BusReport;
            var busId = message.Identifier;
            var _busTeacher = message.User;

            if ((int)DataBaseOperation.QueryMultiple(new DBQuery().WhereEqualTo("BusID", busId), out List<StudentObject> students) < 1)
            {
                L.W("Failed to query Students List in specific bus ID: " + busId);
                return false;
            }
            if (message._Type == GlobalMessageTypes.Bus_Status_Report_TC)
            {
                //To Class Teacher
                string[] ClassList = (from _stu in students select _stu.ClassID).Distinct().ToArray();
                if ((int)DataBaseOperation.QueryMultiple(new DBQuery().WhereValueContainedInArray("objectId", ClassList), out List<ClassObject> classes) < 1)
                {
                    L.W("Failed to query Classes from ClassList..." + string.Join(';', ClassList));
                    return false;
                }
                foreach (ClassObject _class in classes)
                {
                    if (DataBaseOperation.QuerySingle(new DBQuery().WhereEqualTo("objectId", _class.TeacherID), out UserObject _ClassTeacher) != DBQueryStatus.ONE_RESULT)
                    {
                        L.W("Failed to get ClassTeacher of ClassID: " + _class.ObjectId);
                    }
                    string[] _StudentInClass = (from _stu in students where _stu.ClassID == _class.ObjectId select _stu.StudentName).ToArray();
                    WeChatSentMessage busReportMsg_Teacher = new WeChatSentMessage(WeChatSMsg.text, null,
                        $"{_ClassTeacher.RealName}: \r\n" +
                        $"你的班级 {_class.CDepartment} {_class.CGrade} {_class.CNumber} \r\n" +
                        $"有 {_StudentInClass.Length} 名学生受到班车 {_report.ReportType} 影响: \r\n" +
                        $"原因：{_report.OtherData}\r\n" +
                        $"学生列表: {string.Join(",", _StudentInClass)}", null, _ClassTeacher.UserName);
                    WeChatMessageSystem.AddToSendList(busReportMsg_Teacher);
                }
                return true;
            }
            else if (message._Type == GlobalMessageTypes.Bus_Status_Report_TP)
            {
                //To Parents....
                List<UserObject> AllParents = new List<UserObject>();
                foreach (StudentObject studentObject in students)
                {
                    if ((int)DataBaseOperation.QueryMultiple(new DBQuery().WhereRecordContainsValue("ChildIDs", studentObject.ObjectId), out List<UserObject> _Parents) < 1)
                    {
                        L.W("Failed to get Child's parent.. ChildID: " + studentObject.ObjectId);
                        continue;
                    }
                    AllParents.AddRange(_Parents);
                }
                AllParents = AllParents.Distinct(DataTableComparer<UserObject>.Default).ToList();
                foreach (UserObject _parent in AllParents)
                {
                    string[] _ChildrenList = (from _stu in students where _parent.ChildList.Contains(_stu.ObjectId) select _stu.StudentName).Distinct().ToArray();
                    WeChatSentMessage busReportMsg_Parent = new WeChatSentMessage(WeChatSMsg.text, null,
                        $"{_parent.RealName}: \r\n" +
                        $"你的 {_ChildrenList.Length} 个孩子受到班车 {_report.ReportType} 影响\r\n" +
                        $"原因: {_report.OtherData}\r\n" +
                        $"受影响的孩子: {string.Join(",", _ChildrenList)}", null, _parent.UserName);
                    WeChatMessageSystem.AddToSendList(busReportMsg_Parent);
                }
                return true;
            }
            else
            {
                L.E("MessageSystem->BusStatusReport: This Error may never hit...");
                return false;
            }
        }

        private static DBQueryStatus GetAdminUsers(out List<UserObject> adminUsers) => DataBaseOperation.QueryMultiple(new DBQuery().WhereEqualTo("isAdmin", true), out adminUsers);
    }
}
