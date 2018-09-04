"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var GetBusResultData = (function () {
    function GetBusResultData() {
        this.Bus = new SchoolBusObject();
        this.AHChecked = 0;
        this.LSChecked = 0;
        this.CSChecked = 0;
        this.DirectGoHome = 0;
        this.Total = 0;
    }
    return GetBusResultData;
}());
var GetStudentsData = (function () {
    function GetStudentsData() {
        this.StudentList = [new StudentObject()];
        this.Count = 0;
    }
    return GetStudentsData;
}());
var NewReportData = (function () {
    function NewReportData() {
        this.Report = new BusReportObject();
    }
    return NewReportData;
}());
var GetUserNameData = (function () {
    function GetUserNameData() {
        this.Name = "";
    }
    return GetUserNameData;
}());
var SignStudentData = (function () {
    function SignStudentData() {
        this.Student = new StudentObject();
        this.SignMode = "";
        this.SignResult = false;
    }
    return SignStudentData;
}());
var PureMessageData = (function () {
    function PureMessageData() {
        this.Message = "";
    }
    return PureMessageData;
}());
var DataObject = (function () {
    function DataObject() {
        this.CreatedAt = "";
        this.UpdatedAt = "";
        this.ObjectId = "";
    }
    return DataObject;
}());
var Point = (function () {
    function Point() {
        this.X = 0.0;
        this.Y = 0.0;
    }
    return Point;
}());
var UserGroup = (function () {
    function UserGroup() {
        this.IsBusManager = false;
        this.IsClassTeacher = false;
        this.IsParent = false;
    }
    return UserGroup;
}());
var UserObject = (function (_super) {
    __extends(UserObject, _super);
    function UserObject() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.CurrentPoint = new Point();
        _this.HeadImagePath = "";
        _this.Password = "";
        _this.PhoneNumber = "";
        _this.RealName = "";
        _this.Sex = "";
        _this.UserGroup = new UserGroup();
        _this.UserName = "";
        return _this;
    }
    return UserObject;
}(DataObject));
var SchoolBusObject = (function (_super) {
    __extends(SchoolBusObject, _super);
    function SchoolBusObject() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.BusName = "";
        _this.TeacherID = "";
        _this.AHChecked = false;
        _this.CSChecked = false;
        _this.LSChecked = false;
        return _this;
    }
    return SchoolBusObject;
}(DataObject));
var StudentObject = (function (_super) {
    __extends(StudentObject, _super);
    function StudentObject() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.StudentName = "";
        _this.BusID = "";
        _this.Sex = "";
        _this.ClassID = "";
        _this.LSChecked = false;
        _this.CSChecked = false;
        _this.AHChecked = false;
        _this.TakingBus = false;
        _this.DirectGoHome = false;
        return _this;
    }
    return StudentObject;
}(DataObject));
var BusReportObject = (function () {
    function BusReportObject() {
        this.TeacherID = "";
        this.BusID = "";
        this.ReportType = "";
        this.OtherData = "";
    }
    return BusReportObject;
}());
var TSDictionary = (function () {
    function TSDictionary(Keys, Values) {
        this.keys = [];
        this.values = [];
        this.keys = Keys;
        this.values = Values;
    }
    TSDictionary.prototype.Add = function (key, value) {
        this.keys.push(key);
        this.values.push(value);
    };
    TSDictionary.prototype.Remove = function (key) {
        var index = this.keys.indexOf(key, 0);
        this.keys.splice(index, 1);
        this.values.splice(index, 1);
    };
    TSDictionary.prototype.GetValue = function (key) {
        var index = this.keys.indexOf(key, 0);
        if (index != -1) {
            return this.values[index];
        }
        return null;
    };
    TSDictionary.prototype.SetValue = function (key, value) {
        var index = this.keys.indexOf(key, 0);
        if (index != -1) {
            this.keys[index] = key;
            this.values[index] = value;
            return true;
        }
        return false;
    };
    TSDictionary.prototype.ContainsKey = function (key) {
        var ks = this.keys;
        for (var i = 0; i < ks.length; ++i) {
            if (ks[i] === key)
                return true;
        }
        return false;
    };
    return TSDictionary;
}());
String.prototype.format = function (args) { var result = this; if (arguments.length > 0) {
    if (arguments.length == 1 && typeof (args) == "object") {
        for (var key in args) {
            if (args[key] != undefined) {
                var reg = new RegExp("({" + key + "})", "g");
                result = result.replace(reg, args[key]);
            }
        }
    }
    else {
        for (var i = 0; i < arguments.length; i++) {
            if (arguments[i] != undefined) {
                var reg = new RegExp("({)" + i + "(})", "g");
                result = result.replace(reg, arguments[i]);
            }
        }
    }
} return result; };
function deserialize(json, instance) {
    for (var prop in json) {
        if (!json.hasOwnProperty(prop))
            continue;
        if (typeof json[prop] === 'object')
            deserialize(json[prop], instance[prop]);
        else if (instance != undefined && instance[prop] != undefined)
            instance[prop] = json[prop];
    }
}
var ResultData = (function () {
    function ResultData(data) {
        this.ErrCode = 0;
        this.ErrMessage = "";
        this.Data = new data();
    }
    return ResultData;
}());
var Networking = (function () {
    function Networking(XTag) {
        this.XTag = "";
        this.ASync = false;
        this.XTag = XTag;
    }
    Networking.prototype.AjaxGet = function (OutputDataType, URL) {
        var _result;
        $.ajax({
            async: this.ASync,
            headers: { "X-WoodenBench-Protection": this.XTag },
            url: localStorage.getItem("_") + URL,
            success: function (data) { _result = data; },
            error: function () { _result = false; }
        });
        if (_result === false) {
            console.error("Get Value from " + URL + "Failed!");
            return null;
        }
        var result = new ResultData(OutputDataType);
        $.extend(result, _result);
        if (result.ErrCode == 0)
            return result.Data;
        console.warn("API Call At " + URL + " ResultCode=" + result.ErrCode);
        console.warn(result);
    };
    return Networking;
}());
var WoodenBenchWeb = (function () {
    function WoodenBenchWeb() {
        this.CurrentUser = new UserObject();
    }
    WoodenBenchWeb.prototype.Initialise = function (UserConfig, APITicket, RouteKey, RouteValue) {
        this.Session = getCookie("Session");
        this.ApiTicket = APITicket;
        this.Config = new TSDictionary(RouteKey, RouteValue);
        this.Network = new Networking(CryptoJS.SHA384("{0}:{1}:{2}".format(this.Session, this.ApiTicket, window.navigator.userAgent)).toString());
        localStorage.setItem("_", location.protocol + "//" + location.host);
        deserialize(UserConfig, this.CurrentUser);
    };
    WoodenBenchWeb.prototype.GetMgmtBus = function () { return this.Network.AjaxGet(GetBusResultData, this.Config.GetValue("API_GetBuses").format(this.CurrentUser.ObjectId, this.Session)); };
    WoodenBenchWeb.prototype.GetName = function (UserID) { return this.Network.AjaxGet(GetUserNameData, this.Config.GetValue("API_GetName").format(UserID)); };
    WoodenBenchWeb.prototype.GetStudents = function (BusID, TeacherID) { return this.Network.AjaxGet(GetStudentsData, this.Config.GetValue("API_GetStudents").format(BusID, TeacherID, this.Session)); };
    WoodenBenchWeb.prototype.QueryStudents = function (BusID, Column, Content) { return this.Network.AjaxGet(GetStudentsData, this.Config.GetValue("API_QueryStudents").format(BusID, Column, Content)); };
    WoodenBenchWeb.prototype.BusIssueReport = function (BusID, Type, Content) { return this.Network.AjaxGet(NewReportData, this.Config.GetValue("API_BusIssueReport").format(BusID, this.CurrentUser.ObjectId, Type, Content)); };
    WoodenBenchWeb.prototype.SignStudent = function (BusID, StudentID, Mode, Value) { return this.Network.AjaxGet(SignStudentData, this.Config.GetValue("API_SignStudent").format(BusID, btoa("{0};{1};{2};{3}".format(Mode, Value, this.CurrentUser.ObjectId, StudentID)))); };
    WoodenBenchWeb.prototype.GetClassStudents = function (ClassID) { return this.Network.AjaxGet(GetStudentsData, this.Config.GetValue("API_GetClassStudents").format(ClassID, this.CurrentUser.ObjectId)); };
    WoodenBenchWeb.prototype.GetMyChild = function () { return this.Network.AjaxGet(GetStudentsData, this.Config.GetValue("API_GetMyChildren").format(this.CurrentUser.ObjectId)); };
    WoodenBenchWeb.prototype.SetStudentState = function (StudentID, State) { return this.Network.AjaxGet(SignStudentData, this.Config.GetValue("API_SetStudentState").format(StudentID, State)); };
    WoodenBenchWeb.prototype.GenerateReport = function (scope) { return this.Network.AjaxGet(PureMessageData, this.Config.GetValue("API_GenerateReport").format(scope)).Message; };
    WoodenBenchWeb.prototype.NewWeekRecord = function () { return this.Network.AjaxGet(PureMessageData, this.Config.GetValue("API_NewWeek")).Message; };
    WoodenBenchWeb.prototype.SendMessage = function (targ, msg) { return this.Network.AjaxGet(PureMessageData, this.Config.GetValue("API_SendMessage").format(targ, msg)).Message; };
    return WoodenBenchWeb;
}());
var wbWeb = new WoodenBenchWeb();
function setCookie(name, value) {
    var TimeMins = 40;
    var exp = new Date();
    exp.setTime(exp.getTime() + TimeMins * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toUTCString() + ";path=/";
}
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
    if (arr = document.cookie.match(reg))
        return unescape(arr[2]);
    else
        return null;
}
function delCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval !== null)
        document.cookie = name + "=nothing;expires=" + exp.toUTCString() + ";path=/";
}
function GetURLOption(option) {
    "use strict";
    var reg = new RegExp("(^|&)" + option + "=([^&]*)(&|$)");
    var r = location.href.substr(location.href.indexOf("?") + 1).match(reg);
    if (r === null)
        return "nullStr";
    return r[2];
}
//# sourceMappingURL=service.js.map