namespace WBPlatform.StaticClasses
{
    public enum InternalMessageTypes
    {
        UCR_Created_TO_ADMIN = 0,
        UCR_Created__TO_User = 1,
        UCR_Procceed_TO_User = 3,
        //User__Pending_Verify = 4, User__Finishd_Verify = 5,
        Bus_Status_Report = 6,
        Admin_WeekReport_Gen = 8,
        Admin_ResetAllRecord = 9,
        Admin_WeChat_SendMsg = 10
    }
    public enum ServerAction
    {
        WeChatLogin_PreExecute,
        WeChatLogin_PostExecute,

        Home_Index,
        MyAccount_UserRegister,
        Home_BugReport,

        BusManage_Index,
        BusManage_SignStudents,
        BusManage_CodeGenerate,
        BusManage_DataModify,
        BusManage_WeekIssue,

        MyChild_Index,
        MyChild_MarkAsArrived,

        MyClass_Index,

        MyAccount_Index,
        MyAccount_CreateChangeRequest,

        General_ViewStudent,
        General_ViewChangeRequests,

        Manage_Index,
        Manage_UserManage,
        Manage_VerifyChangeRequest,

        INTERNAL_ERROR
    }

    public enum TicketUsage
    {
        WeChatLogin,
        UserRegister,
        AddPassword
    }
    public enum WeChatEvent
    {
        subscribe,
        enter_agent,
        LOCATION,
        batch_job_result,
        change_contact,
        click,
        view,
        scancode_push,
        scancode_waitmsg,
        pic_sysphoto,
        pic_photo_or_album,
        pic_weixin,
        location_select
    }
    public enum WeChatRMsg { text, image, voice, video, location, link, EVENT /*, _INJECTION_DEVELOPER_ERROR_REPORT*/ }
    public enum WeChatSMsg { text, image, voice, video, file, textcard, news, mpnews }
    public enum WeChatEncryptionErrorCode
    {
        OK = 0,
        ValidateSignature_Error = -40001,
        ParseXml_Error = -40002,
        ComputeSignature_Error = -40003,
        IllegalAesKey = -40004,
        ValidateCorpid_Error = -40005,
        EncryptAES_Error = -40006,
        DecryptAES_Error = -40007,
        IllegalBuffer = -40008,
        EncodeBase64_Error = -40009,
        DecodeBase64_Error = -40010
    };
}