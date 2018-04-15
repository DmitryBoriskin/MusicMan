using cms.dbModel.entity;
using System;
using System.ComponentModel;

namespace Admin.Models
{
    public abstract class CoreViewModel
    {
        public string DomainName { get; set; }

        public AccountModel Account { get; set; }
        public cmsLogModel Log { get; set; }
        
        public ErrorMassege ErrorInfo { get; set; }
    }
    
    // Ошибки
    public class ErrorMassege
    {
        public string title { get; set; }
        public string info { get; set; }
        public ErrorMassegeBtn[] buttons { get; set; }
    }
    public class ErrorMassegeBtn
    {
        public string url { get; set; }
        public string text { get; set; }
        [DefaultValue("default")]
        public string style { get; set; }
        public string action { get; set; }
    }

    /// <summary>
    /// Pager (постраничный навигатор)
    /// </summary>
    public class PagerModel
    {
        public string text { get; set; }
        public string url { get; set; }
        public bool isChecked { get; set; }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class ErrorViewModel
    {
        public Int32? HttpCode { get; set; }
        public String Title { get; set; }
        public String Message { get; set; }
        public String BackUrl { get; set; }
    }

    #region VK
    public class VkLoginModel
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string user_id { get; set; }
    }
    #endregion
}