using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MusicMan.Models
{
    public abstract class CoreViewModel
    {
        public AccountModel Account { get; set; }
        
        /// <summary>
        /// Название контроллера
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Название актина
        /// </summary>
        public string ActionName { get; set; }

        public SettingsModel Settings { get; set; }

        public ErrorMassege ErrorInfo { get; set; }
    }

    #region VK
    public class VkLoginModel
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string user_id { get; set; }
    }
    public class VkUserInfo
    {
        public List<Response> response { get; set; }
    }
    public class Response
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string nickname { get; set; }
        public string domain { get; set; }
        public catalog city { get; set; }
        public catalog country { get; set; }
        public string photo_200_orig { get; set; }
        public bool has_photo { get; set; }
        public int hidden { get; set; }
    }    
    public class catalog
    {
        public int id { get; set; }
        public string title { get; set; }
    }
    #endregion
    #region Facebook
    public class FbLoginModel
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
    public class FbUserInfo
    {
        //public List<Response> response { get; set; }
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }
    #endregion

    public class postMassege
    {
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string Massege { get; set; }
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

    // Pager (постраничный навигатор)
    public class PagerModel
    {
        public string text { get; set; }
        public string url { get; set; }
        public bool isChecked { get; set; }
    }


    public class ErrorViewModel
    {
        public Int32? HttpCode { get; set; }
        public String Title { get; set; }
        public String Message { get; set; }
        public String BackUrl { get; set; }
    }
}