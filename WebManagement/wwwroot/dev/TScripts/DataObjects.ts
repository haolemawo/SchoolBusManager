class GetBusResultData {
    Bus: SchoolBusObject = new SchoolBusObject();
    AHChecked = 0;
    LSChecked = 0;
    CSChecked = 0;
    DirectGoHome = 0;
    Total = 0;
}

class GetStudentsData {
    StudentList: StudentObject[] = [new StudentObject()];
    Count: number = 0;
}


class NewReportData {
    Report: BusReportObject = new BusReportObject();
}

class GetUserNameData {
    Name: string = ""
}

class SignStudentData {
    Student: StudentObject = new StudentObject();
    SignMode: string = "";
    SignResult: boolean = false;
}

class PureMessageData {
    Message: string = "";
}

abstract class DataObject {
    CreatedAt: string = "";
    UpdatedAt: string = "";
    ObjectId: string = "";
}

class Point {
    X: number = 0.0;
    Y: number = 0.0;
}

class UserGroup {
    IsBusManager: boolean = false;
    IsClassTeacher: boolean = false;
    IsParent: boolean = false;
}

class UserObject extends DataObject {
    CurrentPoint: Point = new Point();
    HeadImagePath: string = "";
    Password: string = "";
    PhoneNumber: string = "";
    RealName: string = "";
    Sex: string = "";
    UserGroup: UserGroup = new UserGroup();
    UserName: string = "";
}

class SchoolBusObject extends DataObject {
    BusName: string = "";
    TeacherID: string = "";
    AHChecked: boolean = false;
    CSChecked: boolean = false;
    LSChecked: boolean = false;
}


class StudentObject extends DataObject {
    StudentName: string = "";
    BusID: string = "";
    Sex: string = "";
    ClassID: string = "";
    LSChecked: boolean = false;
    CSChecked: boolean = false;
    AHChecked: boolean = false;
    TakingBus: boolean = false;
    DirectGoHome: boolean = false;
}

class BusReportObject {
    TeacherID: string = "";
    BusID: string = "";
    ReportType: string = "";
    OtherData: string = "";
}
