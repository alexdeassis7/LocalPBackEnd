using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharedBusiness.Mail
{
    public static class MailService
    {
        public static void SendMail(string strSubject, string strData, IEnumerable<string> toEmail, IEnumerable<string> attachmentsFiles = null)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString;
            var isQA = connectionString.IndexOf("LocalPaymentPROD") == -1;
            
            if (isQA)
                strSubject = "QA - " + strSubject;
//#if DEBUG
//            return;
//#endif
            var envioEmailsActivo = ConfigurationManager.AppSettings["EnvioEmails"];
            if (envioEmailsActivo != null)
            {
                if (!bool.Parse(envioEmailsActivo))
                    return;
            }

            using (var mail = new MailMessage())
            {

                var userNameEmail = ConfigurationManager.AppSettings["UserNameEmail"];

                var credentialApiUserApiKey = ConfigurationManager.AppSettings["EmailCredentialUserApiKey"];
                var credentialApiPassApiKey = ConfigurationManager.AppSettings["EmailCredentialPassApiKey"];

                var emailPort = Convert.ToInt32(ConfigurationManager.AppSettings["EmailPort"]);
                var cantPaginacion = Convert.ToInt32(ConfigurationManager.AppSettings["CantPaginacionEmail"]);

                if (cantPaginacion == 0)
                    cantPaginacion = 1;

                var smtpServer = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"]);
                smtpServer.Port = emailPort;

                if (attachmentsFiles != null)
                    attachmentsFiles.ToList().ForEach(x => mail.Attachments.Add(new Attachment(x)));

                var emailsToSend = toEmail.Distinct().Where(x => IsValidEmail(x)).ToList();

                if (emailsToSend.Count() > 0)
                {
                    while (emailsToSend.Count() > 0)
                    {
                        var emailsTake = emailsToSend.Take(cantPaginacion).ToList();
                        smtpServer.UseDefaultCredentials = true;
                        smtpServer.Credentials = new System.Net.NetworkCredential(credentialApiUserApiKey, credentialApiPassApiKey);
                        smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpServer.EnableSsl = true;

                        mail.From = new MailAddress(userNameEmail);
                        if (emailsTake.Count > 1)
                            mail.Bcc.Add(string.Join(",", emailsTake));
                        else
                            mail.To.Add(emailsTake.First());
                        mail.Subject = strSubject;
                        mail.IsBodyHtml = true;
                        mail.Body = strData;
                        smtpServer.Send(mail);
                        mail.Bcc.Clear();
                        mail.To.Clear();

                        emailsToSend.RemoveRange(0, cantPaginacion < emailsToSend.Count ? cantPaginacion : emailsToSend.Count);
                    }
                }
            }
        }
        public static async Task SendMailAsync(string strSubject, string strData, IEnumerable<string> toEmail, IEnumerable<string> attachmentsFiles = null)
        {
            using (var mail = new MailMessage())
            {
                var userNameEmail = ConfigurationManager.AppSettings["UserNameEmail"];

                var credentialUser = ConfigurationManager.AppSettings["EmailCredentialUserApiKey"];
                var credentialPass = ConfigurationManager.AppSettings["EmailCredentialPassApiKey"];

                var emailPort = Convert.ToInt32(ConfigurationManager.AppSettings["EmailPort"]);
                var cantPaginacion = Convert.ToInt32(ConfigurationManager.AppSettings["CantPaginacionEmail"]);

                var smtpServer = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"]);
                smtpServer.Port = emailPort;
                var emailsToSend = toEmail.Distinct().Where(x => IsValidEmail(x)).ToList();

                if (attachmentsFiles != null)
                    attachmentsFiles.ToList().ForEach(x => mail.Attachments.Add(new Attachment(x)));

                if (emailsToSend.Count() > 0)
                {
                    while (emailsToSend.Count() > 0)
                    {
                        var emailsTake = emailsToSend.Take(cantPaginacion).ToList();
                        smtpServer.Credentials = new System.Net.NetworkCredential(credentialUser, credentialPass);
                        smtpServer.EnableSsl = true;
                        mail.From = new MailAddress(userNameEmail);
                        if (emailsTake.Count > 1)
                            mail.Bcc.Add(string.Join(",", emailsTake));
                        else
                            mail.To.Add(emailsTake.First());
                        mail.Subject = strSubject;
                        mail.IsBodyHtml = true;
                        mail.Body = strData;

                        await smtpServer.SendMailAsync(mail);

                        mail.Bcc.Clear();
                        mail.To.Clear();

                        emailsToSend.RemoveRange(0, cantPaginacion < emailsToSend.Count ? cantPaginacion : emailsToSend.Count);
                    }
                }
            }
        }

        private static string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            var idn = new IdnMapping();

            string domainName = match.Groups[2].Value;

            domainName = idn.GetAscii(domainName);

            return match.Groups[1].Value + domainName;
        }

        public static bool IsValidEmail(string strIn)
        {
            var invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names. 
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format. 
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
