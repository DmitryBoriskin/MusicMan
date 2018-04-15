using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

/// <summary>
/// 
/// </summary>


public class Mailer
{
    protected string server = String.Empty;
    protected int port = 25;
    protected bool ssl = false;

    protected string maillist = String.Empty;
    protected string mailfrom = String.Empty;
    protected string mailname = String.Empty;
    protected string password = String.Empty;

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


    public string MailFrom
    {
        set { mailfrom = value; }
        get { return mailfrom; }
    }

    public string MailName
    {
        set { mailname = value; }
        get { return mailname; }
    }

    public string Server
    {
        set { server = value; }
        get { return server; }
    }
    public int Port
    {
        set { port = value; }
        get { return port; }
    }

    public string Password
    {
        set { password = value; }
        get { return password; }
    }

    public bool isSsl
    {
        set { ssl = value; }
        get { return ssl; }
    }

    public string Attachments
    {
        set { attechments = value; }
        get { return attechments; }
    }

    public String Dublicate
    {
        set { dublicate = value; }
        get { return dublicate; }
    }

    public void MailFromSettings()
    {
        if (mailfrom == String.Empty || server == String.Empty || password == String.Empty)
        {
            server = Settings.mailServer;
            port = Settings.mailServerPort;
            ssl = Settings.mailServerSSL;
            mailname = Settings.MailAdresName;
            mailfrom = Settings.mailUser;
            server = Settings.mailServer;
            password = Settings.mailPass;
        }
    }


    public void MailToSettings()
    {
        if (maillist == string.Empty) maillist = Settings.MailTo;
    }

    public string SendMail()
    {
        MailFromSettings();

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
            if (normail.Count > 0) _Message.To.Add(new MailAddress(adress));
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
