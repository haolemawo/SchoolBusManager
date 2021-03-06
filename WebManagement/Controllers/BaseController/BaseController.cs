﻿using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Linq;
using System.Collections.Generic;

using WBPlatform.StaticClasses;
using WBPlatform.TableObject;
using WBPlatform.WebManagement.Tools;
using System.Security.Cryptography;

namespace WBPlatform.WebManagement.Controllers
{
    public class BaseController : Controller
    {
        public const string UID_CookieName = "identifiedUID";
        public static int SessionCount => SessionCollection.Count;
        private static AutoDictionary<string, UserIdentity> SessionCollection { get; set; } = new AutoDictionary<string, UserIdentity>();

        protected UserObject CurrentUser => CurrentIdentity.User;
        protected UserIdentity CurrentIdentity { get; private set; } = UserIdentity.Default;
        protected TelemetryClient Telemetry { get; set; } = new TelemetryClient();

        public static bool CheckSessions()
        {
            return true;
        }

        //public static string RenewSession(string SessionString, string UserAgent, UserObject _user)
        //{
        //    string _Str = GetSessionString(_user, UserAgent);
        //    lock (SessionCollection)
        //    {
        //        if (SessionCollection.ContainsKey(SessionString))
        //        {
        //            SessionCollection.Remove(SessionString);
        //        }
        //        SessionCollection.Add(_Str, new UserIdentity(UserAgent, _user));
        //        return _Str;
        //    }
        //}

        private string GetNewSession()
            => (Cryptography.RandomString(10, true) +
                CurrentUser.UserName +
                CurrentUser.UserGroup.ToString() +
                DateTime.Now.TimeOfDay.TotalMilliseconds.ToString() +
                Request.Headers["User-Agent"] +
                CurrentUser.UserGroup.ToString()).SHA512Encrypt();

        protected void UpdateUser(UserObject _user)
        {
            SessionCollection.Remove(Request.Cookies["Session"] ?? "");
            CurrentIdentity = new UserIdentity(Request.Headers["User-Agent"], _user);
            string NewSession = GetNewSession();
            Response.Cookies.Append("Session", NewSession, new CookieOptions() { Expires = DateTime.Now.AddHours(4) });
            SessionCollection.Add(NewSession, CurrentIdentity);
        }

        protected bool ValidateSession()
        {
            string Session = Request.Cookies["Session"];
            string UA = Request.Headers["User-Agent"];
            if (string.IsNullOrWhiteSpace(Session) || string.IsNullOrWhiteSpace(UA)) return false;

            //if (SessionCollection.ContainsKey(SessionString) && (UA == "JumpToken_FreeLogin" || SessionCollection[SessionString].UserAgent == UA))
            if (SessionCollection[Session] != null && SessionCollection[Session].UserAgent == UA)
            {
                lock (SessionCollection) SessionCollection[Session].SetLastActive();
                CurrentIdentity = SessionCollection[Session];
                User.AddIdentity(CurrentIdentity.Identity);

                string _userIC = CurrentUser.GetIdentifiableCode();

                Telemetry.Context.User.Id = _userIC;
                Telemetry.Context.Session.Id = Session;

                ViewData[UID_CookieName] = _userIC;
                ViewData["cUser"] = CurrentUser.Stringify().Replace(CurrentUser.Password, "Looking for Password?");
                ViewData["CurrentRequest"] = HttpContext;

                Response.Cookies.Append("ai_user", _userIC);
                Response.Cookies.Append("ai_session", HttpContext.Connection.RemoteIpAddress.ToString() + "-" + _userIC + Session?.Substring(0, 5) ?? "Unknown");
                Response.Cookies.Append("ai_authUser", CurrentUser.UserName);

                //API CALL
                if (Request.Headers["X-Requested-With"].Count == 1)
                {
                    if (Request.Headers["X-WoodenBench-Protection"].Count != 1) return false;
                    string ProtectionString = Request.Headers["X-WoodenBench-Protection"].First();
                    //CryptoJS.SHA384("{0}:{1}:{2}".format(this.Session, this.ApiTicket, window.navigator.userAgent)
                    return ProtectionString == string.Format("{0}:{1}:{2}", Session, SessionCollection[Session].ApiTicket + "-" + CurrentIdentity.Identity.Name, UA).SHA384Encrypt();
                }
                return true;
            }
            else return false;
        }
    }
}
