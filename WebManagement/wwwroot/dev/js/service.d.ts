/// <reference path="../../../TScripts/lib/cryptojs.d.ts" />
/// <reference path="C:/Users/lhy20/AppData/Local/Microsoft/TypeScript/3.0/node_modules/@types/jquery/index.d.ts" />
declare class GetBusResultData {
    Bus: SchoolBusObject;
    AHChecked: number;
    LSChecked: number;
    CSChecked: number;
    Total: number;
}
declare class GetStudentsData {
    StudentList: StudentObject[];
    Count: number;
}
declare class NewReportData {
    Report: BusReportObject;
}
declare class GetUserNameData {
    Name: string;
}
declare class SignStudentData {
    Student: StudentObject;
    SignMode: string;
    SignResult: boolean;
}
declare abstract class DataObject {
    CreatedAt: string;
    UpdatedAt: string;
    ObjectId: string;
}
declare class Point {
    X: number;
    Y: number;
}
declare class UserGroup {
    IsBusManager: boolean;
    IsClassTeacher: boolean;
    IsParent: boolean;
}
declare class UserObject extends DataObject {
    CurrentPoint: Point;
    HeadImagePath: string;
    Password: string;
    PhoneNumber: string;
    RealName: string;
    Sex: string;
    UserGroup: UserGroup;
    UserName: string;
}
declare class SchoolBusObject extends DataObject {
    BusName: string;
    TeacherID: string;
    AHChecked: boolean;
    CSChecked: boolean;
    LSChecked: boolean;
}
declare class StudentObject extends DataObject {
    StudentName: string;
    BusID: string;
    Sex: string;
    ClassID: string;
    LSChecked: boolean;
    CSChecked: boolean;
    AHChecked: boolean;
}
declare class BusReportObject {
    TeacherID: string;
    BusID: string;
    ReportType: string;
    OtherData: string;
}
declare class TSDictionary<Tkey, TValue> {
    private keys;
    private values;
    constructor(Keys: Tkey[], Values: TValue[]);
    Add(key: Tkey, value: TValue): void;
    Remove(key: Tkey): void;
    GetValue(key: Tkey): TValue;
    SetValue(key: Tkey, value: any): boolean;
    ContainsKey(key: Tkey): boolean;
}
interface String {
    format(...args: any[]): string;
}
declare function CreateInstance<T>(c: {
    new (): T;
}): T;
declare function deserialize(json: any, instance: any): void;
declare class ResultData<T> {
    constructor(data: {
        new (): T;
    });
    ErrCode: number;
    ErrMessage: string;
    Data: T;
}
declare class Networking {
    constructor(XTag: string);
    AjaxGet<T>(OutputDataType: {
        new (): T;
    }, URL: string): T;
}
declare class WoodenBenchWeb {
    CurrentUser: UserObject;
    private ApiTicket;
    private Session;
    private Network;
    private Config;
    Initialise(UserConfig: any, APITicket: string, RouteKey: string[], RouteValue: string[]): void;
    GetMgmtBus(): GetBusResultData;
    GetName(UserID: string): GetUserNameData;
    GetStudents(BusID: string, TeacherID: string): GetStudentsData;
    QueryStudents(BusID: string, Column: string, Content: string): GetStudentsData;
    BusIssueReport(BusID: string, Type: string, Content: string): NewReportData;
    SignStudent(BusID: string, StudentID: string, Mode: string, Value: string): SignStudentData;
    GetClassStudents(ClassID: string): GetStudentsData;
    GetMyChild(): GetStudentsData;
}
declare function randomString(len: number): string;
declare function setCookie(name: string, value: string): void;
declare var wbWeb: WoodenBenchWeb;
declare function getCookie(name: string): string;
declare function delCookie(name: string): void;
declare function GetURLOption(option: string): string;
