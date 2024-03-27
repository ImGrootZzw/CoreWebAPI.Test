using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace CoreWebAPI.Common.Helper
{
    public class EmailHelper
    {
        private readonly MailMessage MailMessage;   //主要处理发送邮件的内容（如：收发人地址、标题、主体、图片等等）
        private readonly SmtpClient SmtpClient; //主要处理用smtp方式发送此邮件的配置信息（如：邮件服务器、发送端口号、验证方式等等）
        private readonly bool EnablePwdAuthentication;  //是否对发件人邮箱进行密码验证
        private readonly string SenderUsername;   //发件箱的用户名（即@符号前面的字符串，例如：hello@163.com，用户名为：hello）
        private readonly string SenderPassword;    //发件箱的密码

        ///<summary>
        /// 构造函数
        ///</summary>
        ///<param name="server">发件箱的邮件服务器地址（IP形式或字符串形式均可）</param>
        ///<param name="port">发送邮件所用的端口号（htmp协议默认为25）</param>
        ///<param name="enableSsl">true表示对邮件内容进行socket层加密传输，false表示不加密</param>
        ///<param name="pwdCheckEnable">true表示对发件人邮箱进行密码验证，false表示不对发件人邮箱进行密码验证</param>
        ///<param name="username">发件箱的用户名（即@符号前面的字符串，例如：hello@163.com，用户名为：hello）</param>
        ///<param name="password">发件人邮箱密码</param>
        ///<param name="fromMail">发件人邮箱地址</param>
        ///<param name="toMail">收件人地址（可以是多个收件人，程序中是以“,"进行区分的）</param>
        ///<param name="subject">邮件标题</param>
        ///<param name="emailBody">邮件内容</param>
        ///<param name="isBodyHtml">邮件内容是否为html格式</param>
        ///<param name="attachment">邮件附件,路径以分号分隔</param>
        public EmailHelper(string server, string port, bool enableSsl, bool pwdCheckEnable, string username, string password, string fromMail, string toMail, string subject, string emailBody, bool isBodyHtml, string attachment)
        {
            this.EnablePwdAuthentication = pwdCheckEnable;
            this.SenderUsername = username;
            this.SenderPassword = password;

            SmtpClient = new SmtpClient()
            {
                Host = server,
                Port = Convert.ToInt32(port),
                UseDefaultCredentials = false,
                EnableSsl = enableSsl
            };

            MailMessage = new MailMessage()
            {
                From = new MailAddress(fromMail),
                Subject = subject,
                Body = emailBody,
                IsBodyHtml = isBodyHtml,
                BodyEncoding = System.Text.Encoding.UTF8,
                Priority = MailPriority.Normal
            };
            MailMessage.To.Add(toMail);
            if (attachment != "")
            {
                AddAttachments(attachment);
            }
        }

        ///<summary>
        /// 添加附件
        ///</summary>
        ///<param name="attachmentsPath">附件的路径集合，以分号分隔</param>
        public void AddAttachments(string attachmentsPath)
        {
            string[] path = attachmentsPath.Split(';'); //以什么符号分隔可以自定义
            Attachment data;
            ContentDisposition disposition;
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] != "")
                {
                    data = new Attachment(path[i], MediaTypeNames.Application.Octet);
                    disposition = data.ContentDisposition;
                    disposition.CreationDate = File.GetCreationTime(path[i]);
                    disposition.ModificationDate = File.GetLastWriteTime(path[i]);
                    disposition.ReadDate = File.GetLastAccessTime(path[i]);
                    MailMessage.Attachments.Add(data);
                }
            }
        }

        ///<summary>
        /// 邮件的发送
        ///</summary>
        public void Send(out bool canpass , out string errormessage)
        {
            try
            {
                canpass = true;
                errormessage = "";
                if (MailMessage != null)
                {
                    if (this.EnablePwdAuthentication)
                    {
                        System.Net.NetworkCredential nc = new System.Net.NetworkCredential(this.SenderUsername, this.SenderPassword);
                        //SmtpClient.Credentials = new System.Net.NetworkCredential(this.SenderUsername, this.SenderPassword);
                        //NTLM: Secure Password Authentication in Microsoft Outlook Express
                        SmtpClient.Credentials = nc.GetCredential(SmtpClient.Host, SmtpClient.Port, "NTLM");
                    }
                    else
                    {
                        SmtpClient.Credentials = new System.Net.NetworkCredential(this.SenderUsername, this.SenderPassword);
                    }
                    SmtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnRemoteCertificateValidationCallback);
                    SmtpClient.Send(MailMessage);
                }
            }
            catch (Exception ex)
            {
                canpass = false;
                errormessage = ex.Message + "-InnerException:" + (ex.InnerException != null ? ex.InnerException.Message : "");
            }
        }

        private bool OnRemoteCertificateValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
