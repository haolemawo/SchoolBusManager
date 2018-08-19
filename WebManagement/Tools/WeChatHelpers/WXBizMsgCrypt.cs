using System;
using System.Collections;
//using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using WBPlatform.StaticClasses;
//-40001 ： 签名验证错误
//-40002 :  xml解析失败
//-40003 :  sha加密生成签名失败
//-40004 :  AESKey 非法
//-40005 :  corpid 校验错误
//-40006 :  AES 加密失败
//-40007 ： AES 解密失败
//-40008 ： 解密后得到的buffer非法
//-40009 :  base64加密异常
//-40010 :  base64解密异常
namespace WBPlatform.WebManagement.Tools
{
    public class WeChatXMLHelper
    {
        private readonly string M_sToken;
        private string M_sEncodingAESKey;
        private readonly string M_sCorpID;

        //构造函数
        // @param sToken: 企业微信后台，开发者设置的Token
        // @param sEncodingAESKey: 企业微信后台，开发者设置的EncodingAESKey
        // @param sCorpID: 企业号的CorpID
        public WeChatXMLHelper(string sToken, string sEncodingAESKey, string sCorpID)
        {
            M_sToken = sToken;
            M_sCorpID = sCorpID;
            M_sEncodingAESKey = sEncodingAESKey;
        }

        //验证URL
        // @param sMsgSignature: 签名串，对应URL参数的msg_signature
        // @param sTimeStamp: 时间戳，对应URL参数的timestamp
        // @param sNonce: 随机串，对应URL参数的nonce
        // @param sEchoStr: 随机串，对应URL参数的echostr
        // @param sReplyEchoStr: 解密之后的echostr，当return返回0时有效
        // @return：成功0，失败返回对应的错误码
        public WeChatEncryptionErrorCode VerifyURL(string sMsgSignature, string sTimeStamp, string sNonce, string sEchoStr, ref string sReplyEchoStr)
        {
            WeChatEncryptionErrorCode ret = 0;
            if (M_sEncodingAESKey.Length != 43)
            {
                return WeChatEncryptionErrorCode.IllegalAesKey;
            }
            ret = VerifySignature(M_sToken, sTimeStamp, sNonce, sEchoStr, sMsgSignature);
            if (0 != ret)
            {
                return ret;
            }
            sReplyEchoStr = "";
            string cpid = "";
            try
            {
                sReplyEchoStr = WeChatCryptography.AES_decrypt(sEchoStr, M_sEncodingAESKey, ref cpid); //m_sCorpID);
            }
            catch (Exception)
            {
                sReplyEchoStr = "";
                return WeChatEncryptionErrorCode.DecryptAES_Error;
            }
            if (cpid != M_sCorpID)
            {
                sReplyEchoStr = "";
                return WeChatEncryptionErrorCode.ValidateCorpid_Error;
            }
            return 0;
        }

        // 检验消息的真实性，并且获取解密后的明文
        // @param sMsgSignature: 签名串，对应URL参数的msg_signature
        // @param sTimeStamp: 时间戳，对应URL参数的timestamp
        // @param sNonce: 随机串，对应URL参数的nonce
        // @param sPostData: 密文，对应POST请求的数据
        // @param sMsg: 解密后的原文，当return返回0时有效
        // @return: 成功0，失败返回对应的错误码
        public WeChatEncryptionErrorCode DecryptMsg(string sMsgSignature, string sTimeStamp, string sNonce, string sPostData, ref string sMsg)
        {
            if (M_sEncodingAESKey.Length != 43)
            {
                return WeChatEncryptionErrorCode.IllegalAesKey;
            }
            XmlDocument doc = new XmlDocument();
            XmlNode root;
            string sEncryptMsg;
            try
            {
                doc.LoadXml(sPostData);
                root = doc.FirstChild;
                sEncryptMsg = root["Encrypt"].InnerText;
            }
            catch (Exception)
            {
                return WeChatEncryptionErrorCode.ParseXml_Error;
            }
            //verify signature
            WeChatEncryptionErrorCode ret = 0;
            ret = VerifySignature(M_sToken, sTimeStamp, sNonce, sEncryptMsg, sMsgSignature);
            if (ret != 0) return ret;
            //decrypt
            string cpid = "";
            try
            {
                sMsg = WeChatCryptography.AES_decrypt(sEncryptMsg, M_sEncodingAESKey, ref cpid);
            }
            catch (FormatException)
            {
                sMsg = "";
                return WeChatEncryptionErrorCode.DecodeBase64_Error;
            }
            catch (Exception)
            {
                sMsg = "";
                return WeChatEncryptionErrorCode.DecryptAES_Error;
            }
            return cpid != M_sCorpID ? WeChatEncryptionErrorCode.ValidateCorpid_Error : 0;
        }

        //将企业号回复用户的消息加密打包
        // @param sReplyMsg: 企业号待回复用户的消息，xml格式的字符串
        // @param sTimeStamp: 时间戳，可以自己生成，也可以用URL参数的timestamp
        // @param sNonce: 随机串，可以自己生成，也可以用URL参数的nonce
        // @param sEncryptMsg: 加密后的可以直接回复用户的密文，包括msg_signature, timestamp, nonce, encrypt的xml格式的字符串,
        //						当return返回0时有效
        // return：成功0，失败返回对应的错误码
        public WeChatEncryptionErrorCode EncryptMsg(string sReplyMsg, string sTimeStamp, string sNonce, ref string sEncryptdeXMLMsg)
        {
            if (M_sEncodingAESKey.Length != 43)
            {
                return WeChatEncryptionErrorCode.IllegalAesKey;
            }
            string raw = "";
            try
            {
                raw = WeChatCryptography.AES_encrypt(sReplyMsg, M_sEncodingAESKey, M_sCorpID);
            }
            catch (Exception)
            {
                return WeChatEncryptionErrorCode.EncryptAES_Error;
            }
            string MsgSigature = "";
            WeChatEncryptionErrorCode ret = 0;
            ret = GenarateSinature(M_sToken, sTimeStamp, sNonce, raw, ref MsgSigature);
            if (ret != WeChatEncryptionErrorCode.OK) return ret;
            sEncryptdeXMLMsg = "";
            sEncryptdeXMLMsg = sEncryptdeXMLMsg + "<xml><Encrypt><![CDATA[" + raw + "]]></Encrypt>";
            sEncryptdeXMLMsg = sEncryptdeXMLMsg + "<MsgSignature><![CDATA[" + MsgSigature + "]]></MsgSignature>";
            sEncryptdeXMLMsg = sEncryptdeXMLMsg + "<TimeStamp><![CDATA[" + sTimeStamp + "]]></TimeStamp>";
            sEncryptdeXMLMsg = sEncryptdeXMLMsg + "<Nonce><![CDATA[" + sNonce + "]]></Nonce>";
            sEncryptdeXMLMsg += "</xml>";
            return 0;
        }

        private class DictionarySort : IComparer
        {
            public int Compare(object oLeft, object oRight)
            {
                string sLeft = oLeft as string;
                string sRight = oRight as string;
                int iLeftLength = sLeft.Length;
                int iRightLength = sRight.Length;
                int index = 0;
                while (index < iLeftLength && index < iRightLength)
                {
                    if (sLeft[index] < sRight[index]) return -1;
                    else if (sLeft[index] > sRight[index]) return 1;
                    else index++;
                }
                return iLeftLength - iRightLength;

            }
        }

        //Verify Signature
        private static WeChatEncryptionErrorCode VerifySignature(string sToken, string sTimeStamp, string sNonce, string sMsgEncrypt, string sSigture)
        {
            string hash = "";
            WeChatEncryptionErrorCode ret = 0;
            ret = GenarateSinature(sToken, sTimeStamp, sNonce, sMsgEncrypt, ref hash);
            return ret != 0
                ? ret
                : hash == sSigture
                    ? 0
                    : WeChatEncryptionErrorCode.ValidateSignature_Error;
        }

        private static WeChatEncryptionErrorCode GenarateSinature(string sToken, string sTimeStamp, string sNonce, string sMsgEncrypt, ref string sMsgSignature)
        {
            ArrayList AL = new ArrayList
            {
                sToken,
                sTimeStamp,
                sNonce,
                sMsgEncrypt
            };
            AL.Sort(new DictionarySort());
            string raw = "";
            for (int i = 0; i < AL.Count; ++i)
            {
                raw += AL[i];
            }

            string hash = "";
            SHA1 sha = new SHA1CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            try
            {
                byte[] dataToHash = enc.GetBytes(raw);
                byte[] dataHashed = sha.ComputeHash(dataToHash);
                hash = BitConverter.ToString(dataHashed).Replace("-", "");
                hash = hash.ToLower();
            }
            catch (Exception)
            {
                return WeChatEncryptionErrorCode.ComputeSignature_Error;
            }
            sMsgSignature = hash;
            return  WeChatEncryptionErrorCode.OK;
        }
    }
}
