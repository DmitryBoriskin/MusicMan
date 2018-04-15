using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

/// <summary>
/// 
/// </summary>


public class Mailer 
    {

        /// <summary>
        /// Контекст доступа к базе данных
        /// </summary>
        //protected dbRepository _repository { get; private set; }
        //protected SettingsViewModel model = new SettingsViewModel();
    
        protected string server = Settings.mailServer;
        protected int port = Settings.mailServerPort;
        protected bool ssl = Settings.mailServerSSL;

        protected string maillist = Settings.MailTo;
        protected string mailfrom = Settings.mailUser;
        protected string mailname = Settings.MailAdresName;
        protected string password = Settings.mailPass;

        protected string theme = "Обратная связь";
        protected string text = String.Empty;
        protected string styles = String.Empty;
    
    protected string attechments = String.Empty;
        protected string results = String.Empty;
        protected string dublicate = String.Empty;

        public string MailTo
        {
            set { maillist = value; }
            get { return maillist; }
        }

        public String Dublicate
        {
            set { dublicate = value; }
            get { return dublicate; }
        }

        public string Theme
        {
            set { theme = value; }
            get { return theme; }
        }

        public string Text
        {
            set { text = value; }
            get { return text; }
        }

        public string Attachments
        {
            set { attechments = value; }
            get { return attechments; }
        }

        public string SendMail()
        {
            //Авторизация на SMTP сервере
            SmtpClient Smtp = new SmtpClient(server, port);
            Smtp.EnableSsl = ssl;
            Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            Smtp.UseDefaultCredentials = false;
            Smtp.Credentials = new NetworkCredential(mailfrom, password);

            // Формирование сообщения

            MailMessage _Message = new MailMessage();
            _Message.From = new MailAddress(mailfrom, mailname);
            if (dublicate != String.Empty) maillist += ";" + dublicate;
            string[] MailList = maillist.Split(';');
            Regex regex = new Regex(@"\b[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}\b");

            foreach (string adress in MailList)
            {
                MatchCollection normail = regex.Matches(adress);                
                if (normail.Count>0) _Message.To.Add(new MailAddress(adress));
            }

            _Message.Subject = theme;
            _Message.BodyEncoding = System.Text.Encoding.UTF8;
            _Message.IsBodyHtml = true;
            _Message.Body = "<DOCTYPE html><html><head></head><body>" + text + "</body></html>";
            try
            {
                Smtp.Send(_Message);//отправка
                results = "Сообщение отправлено";
            }
            catch (System.Net.WebException)
            {
                throw new Exception("Ошибка при отправке");
                results = "Ошибка при отправке";
            }

            return results;
        }

    }
