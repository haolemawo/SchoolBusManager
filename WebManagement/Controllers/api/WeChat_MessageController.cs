﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.IO;
using System.Text;

using WBPlatform.Config;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;
using WBPlatform.WebManagement.Tools;

namespace WBPlatform.WebManagement.Controllers
{
    [Produces("application/json")]
    [Route("api/WeChatMessage")]
    public class WeChat_MessageController : APIController
    {

        /// <summary>
        /// USED TO SERIFY THE WECHAR MESSAGE <![CDATA[WECHAT_MESSAGE_VERIFY]]>
        /// </summary>
        /// <param name="msg_signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="echostr"></param>
        [HttpGet]
        public void Get(string msg_signature, string timestamp, string nonce, string echostr)
        {
            WeChatEncryptionErrorCode ret = 0;
            string sEchoStr = "";
            ret = WeChatHelper.WeChatEncryptor.VerifyURL(msg_signature, timestamp, nonce, echostr, ref sEchoStr);
            if (ret != 0)
            {
                return;
            }
            Response.WriteAsync(sEchoStr);
        }

        /// <summary>
        /// Whatever they are sending....
        /// </summary>
        /// <param name="msg_signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        [HttpPost]
        public void POST(string msg_signature, string timestamp, string nonce)
        {
            MemoryStream ms = new MemoryStream();
            Request.Body.CopyTo(ms);
            string XML_Message = "";
            string _message = Encoding.UTF8.GetString(ms.ToArray());
            WeChatEncryptionErrorCode ret = WeChatHelper.WeChatEncryptor.DecryptMsg(msg_signature, timestamp, nonce, _message, ref XML_Message);
            if (ret !=  WeChatEncryptionErrorCode.OK)
            {
                Response.StatusCode = 500;
                LW.E("WeChat Message Decrypt Failed!! " + _message);
                Response.WriteAsync("");
                return;
            }
            WeChatMessageSystem.AddToRecvList(new WeChatRcvdMessage(XML_Message, DateTime.Now));
            string outMessage = "";
            WeChatEncryptionErrorCode _ = WeChatHelper.WeChatEncryptor.EncryptMsg(XConfig.Messages["WeChatMessageProcessing"], timestamp, nonce, ref outMessage);
            Response.StatusCode = 200;
            Response.WriteAsync(outMessage);
        }
    }
}
