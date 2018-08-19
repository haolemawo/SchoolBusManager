using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using WBPlatform.Database;
using WBPlatform.DesktopClient.DelegateClasses;
using WBPlatform.DesktopClient.StaticClasses;
using WBPlatform.DesktopClient.Views;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

using static WBPlatform.DesktopClient.StaticClasses.GlobalFunctions;

namespace WBPlatform.DesktopClient.Users
{
    public static class UserActivity
    {
        public static bool ChangePassWord(UserObject NowUser, string OriPasswrd, string NewPasswrd)
        {
            if (OriPasswrd.SHA256Encrypt() != CurrentUser.Password)
            {
                LW.E("ChangePassword Request Failed, Reason: Original Password Incorrect....");
                return false;
            }
            else
            {
                NowUser.Password = NewPasswrd.SHA256Encrypt();
                if (DataBaseOperation.UpdateData(ref NowUser, new DBQuery()
                    .WhereEqualTo("objectId", CurrentUser.ObjectId)
                    .WhereEqualTo("Password", OriPasswrd.SHA256Encrypt())
                    .WhereEqualTo("Username", CurrentUser.UserName)) == DBQueryStatus.ONE_RESULT)
                {
                    LW.I("Change Password Success!");
                    return true;
                }
                else
                {
                    LW.I("Change Password Failed!");
                    return false;
                }
            }
        }

        public static bool LogOut()
        {
            CurrentUser.SetDefault();
            GC.Collect();
            return true;
        }

        public static bool Login(string xUserName, string xPassword, out UserObject user)
        {
            xUserName = xUserName.ToLower();
            string HashedPs = xPassword.SHA256Encrypt();
            DBQuery UserNameQuery = new DBQuery();
            UserNameQuery.WhereEqualTo("Username", xUserName);
            UserNameQuery.WhereEqualTo("Password", HashedPs);
            switch (DataBaseOperation.QuerySingleData(UserNameQuery, out user))
            {
                case DBQueryStatus.INTERNAL_ERROR:
                    LW.E("Internal DataBase Error");
                    break;
                case DBQueryStatus.NO_RESULTS:
                    LW.E("No User Found");
                    break;
                case DBQueryStatus.ONE_RESULT:
                    LW.E("User Found");
                    return true;
                case DBQueryStatus.MORE_RESULTS:
                    LW.E("WTF Exception....");
                    break;
                default:
                    break;
            }
            return false;

        }
    }
}
