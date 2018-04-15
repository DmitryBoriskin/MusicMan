using System;
using System.Configuration;

public class Settings
{
    //public const string PrevUrl = "~/";
    //public static bool DebugInfo = Convert.ToBoolean(ConfigurationManager.AppSettings["DebugInfo"]);

    public static string SiteTitle = ConfigurationManager.AppSettings["SiteTitle"];
    public static string SiteDesc = ConfigurationManager.AppSettings["SiteDesc"];

    public static string BaseURL = ConfigurationManager.AppSettings["BaseURL"];

    public static string ContentDir = ConfigurationManager.AppSettings["Root"];

    public static string fbApp = ConfigurationManager.AppSettings["FacebookApp"];
    public static string fbAppServKey = ConfigurationManager.AppSettings["FacebookServKey"];

    public static string vkApp = ConfigurationManager.AppSettings["vkApp"];
    public static string vkGroupId = ConfigurationManager.AppSettings["vkGroupId"];
    public static string vkAppKey = ConfigurationManager.AppSettings["vkAppKey"];
    public static string vkAppServKey = ConfigurationManager.AppSettings["vkAppServKey"];

    public static string mailServer = ConfigurationManager.AppSettings["mailServer"];
    public static int mailServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["MailServerPort"]);
    public static bool mailServerSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["MailServerSSL"]);
    public static string mailUser = ConfigurationManager.AppSettings["mailFrom"];
    public static string mailPass = ConfigurationManager.AppSettings["mailPass"];
    public static string mailEncoding = ConfigurationManager.AppSettings["mailEncoding"];
    public static string MailAdresName = ConfigurationManager.AppSettings["MailAdresName"];
    public static string MailTo = ConfigurationManager.AppSettings["MailTo"];
}