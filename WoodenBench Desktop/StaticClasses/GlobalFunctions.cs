using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Forms;

using WBPlatform.Database;
using WBPlatform.DesktopClient.Users;
using WBPlatform.DesktopClient.Views;
using WBPlatform.StaticClasses;
using WBPlatform.Config;
using WBPlatform.TableObject;
using WBPlatform.Logging;

namespace WBPlatform.DesktopClient.StaticClasses
{
    public class CurrentInstance
    {
        public static UserObject CurrentUser { get; set; } = UserObject.Default;
    }
}