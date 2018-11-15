/// <reference path="./lib/cryptojs.d.ts" />
/// <reference path="./index.d.ts" />
/// <reference path="./DataObjects.ts" />
/// <reference path="./Dictionary.ts" />

"use strict";
interface String {
    format(...args: any[]): string;
}
String.prototype.format = function (args: any): string { var result = this; if (arguments.length > 0) { if (arguments.length == 1 && typeof (args) == "object") { for (var key in args) { if (args[key] != undefined) { var reg = new RegExp("({" + key + "})", "g"); result = result.replace(reg, args[key]); } } } else { for (var i = 0; i < arguments.length; i++) { if (arguments[i] != undefined) { var reg = new RegExp("({)" + i + "(})", "g"); result = result.replace(reg, arguments[i]); } } } } return result; }
{

    function deserialize(json: any, instance: any) {
        for (var prop in json) {
            if (!json.hasOwnProperty(prop)) continue;
            if (typeof json[prop] === 'object') deserialize(json[prop], instance[prop]);
            else if (instance != undefined && instance[prop] != undefined) instance[prop] = json[prop];
        }
    }

    class ResultData<T>{
        constructor(data: { new(): T }) { this.Data = new data(); }
        ErrCode: number = 0;
        ErrMessage: string = "";
        Data: T;
    }

    class Networking {
        public constructor(XTag: string) {
            this.XTag = XTag;
        }
        XTag: string = "";
        ASync: boolean = false;
        AjaxGet<T>(OutputDataType: { new(): T }, URL: string): T {
            var _result;
            $.ajax(
                {
                    async: this.ASync,
                    headers: { "X-WoodenBench-Protection": this.XTag },
                    url: localStorage.getItem("_") + URL,
                    success: (data) => { _result = data; },
                    error: () => { _result = false; }
                });
            if (_result === false) { console.error("Get Value from " + URL + "Failed!"); return null; }
            var result: ResultData<T> = new ResultData(OutputDataType);
            //deserialize(_result, result);
            $.extend(result, _result);
            if (result.ErrCode == 0) return result.Data;
            console.warn("API Call At " + URL + " ResultCode=" + result.ErrCode);
            console.warn(result);
        }
        //AjaxPost(URL: string, POSTData: any, CallBack: Function): void {
        //    $.ajax({
        //        url: URL,
        //        type: 'POST',
        //        data: POSTData,
        //        success: function (data2: JSON) { CallBack(data2); },
        //        error: function () { CallBack(false); }
        //    });
        //}
    }

    class WoodenBenchWeb {
        CurrentUser: UserObject = new UserObject();
        private ApiTicket: string;
        private Session: string;
        private Network: Networking;
        private Config: TSDictionary<string, string>;

        Initialise(UserConfig: any, APITicket: string, RouteKey: string[], RouteValue: string[]) {
            this.Session = getCookie("Session");
            this.ApiTicket = APITicket;
            this.Config = new TSDictionary<string, string>(RouteKey, RouteValue);
            this.Network = new Networking(CryptoJS.SHA384("{0}:{1}:{2}".format(this.Session, this.ApiTicket, window.navigator.userAgent)).toString());
            localStorage.setItem("_", location.protocol + "//" + location.host);
            deserialize(UserConfig, this.CurrentUser);
        }

        GetMgmtBus(): GetBusResultData { return this.Network.AjaxGet(GetBusResultData, this.Config.GetValue("API_GetBuses").format(this.CurrentUser.ObjectId)); }
        GetName(UserID: string): GetUserNameData { return this.Network.AjaxGet(GetUserNameData, this.Config.GetValue("API_GetName").format(UserID)); }
        GetStudents(BusID: string, TeacherID: string): GetStudentsData { return this.Network.AjaxGet(GetStudentsData, this.Config.GetValue("API_GetStudents").format(BusID, TeacherID, this.Session)); }
        QueryStudents(BusID: string, Column: string, Content: string): GetStudentsData { return this.Network.AjaxGet(GetStudentsData, this.Config.GetValue("API_QueryStudents").format(BusID, Column, Content)); }
        BusIssueReport(BusID: string, Type: string, Content: string): NewReportData { return this.Network.AjaxGet(NewReportData, this.Config.GetValue("API_BusIssueReport").format(BusID, this.CurrentUser.ObjectId, Type, Content)); }
        SignStudent(BusID: string, StudentID: string, Mode: string, Value: string): SignStudentData { return this.Network.AjaxGet(SignStudentData, this.Config.GetValue("API_SignStudent").format(BusID, btoa("{0};{1};{2};{3}".format(Mode, Value, this.CurrentUser.ObjectId, StudentID)))); }
        GetClassStudents(ClassID: string): GetStudentsData { return this.Network.AjaxGet(GetStudentsData, this.Config.GetValue("API_GetClassStudents").format(ClassID, this.CurrentUser.ObjectId)); }
        GetMyChild(): GetStudentsData { return this.Network.AjaxGet(GetStudentsData, this.Config.GetValue("API_GetMyChildren").format(this.CurrentUser.ObjectId)); }
        SetStudentState(StudentID: string, State: boolean): SignStudentData { return this.Network.AjaxGet(SignStudentData, this.Config.GetValue("API_SetStudentState").format(StudentID, State)); }
        GenerateReport(scope: string): string { return this.Network.AjaxGet(PureMessageData, this.Config.GetValue("API_GenerateReport").format(scope)).Message; }
        NewWeekRecord(): string { return this.Network.AjaxGet(PureMessageData, this.Config.GetValue("API_NewWeek")).Message; }
        SendMessage(targ: string, msg: string): string { return this.Network.AjaxGet(PureMessageData, this.Config.GetValue("API_SendMessage").format(targ, msg)).Message; }
        SetWeekType(type: string): string { return this.Network.AjaxGet(PureMessageData, this.Config.GetValue("API_SetWeekType").format(type)).Message; }
        //ProsessUCR()
    }
    var wbWeb = new WoodenBenchWeb();

    //function randomString(len: number) {
    //    len = len || 16;
    //    var $chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890';
    //    var maxPos = $chars.length;
    //    var pwd = "";
    //    for (var i = 0; i < len; i++) { pwd += $chars.charAt(Math.floor(Math.random() * maxPos)); }
    //    return pwd;
    //}

    function setCookie(name: string, value: string): void {
        var TimeMins = 40;
        var exp = new Date();
        exp.setTime(exp.getTime() + TimeMins * 60 * 1000);
        document.cookie = name + "=" + escape(value) + ";expires=" + exp.toUTCString() + ";path=/";
    }

    function getCookie(name: string): string {
        var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
        if (arr = document.cookie.match(reg)) return unescape(arr[2]);
        else return null;
    }

    function delCookie(name: string): void {
        var exp = new Date();
        exp.setTime(exp.getTime() - 1);
        var cval = getCookie(name);
        if (cval !== null) document.cookie = name + "=nothing;expires=" + exp.toUTCString() + ";path=/";
    }

    function GetURLOption(option: string): string {
        "use strict";
        var reg = new RegExp("(^|&)" + option + "=([^&]*)(&|$)");
        var r = location.href.substr(location.href.indexOf("?") + 1).match(reg);
        if (r === null) return "nullStr";
        return r[2];
    }
}