using Admin.Models;
using cms.dbase;
using cms.dbModel.entity;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

[AllowAnonymous]
public class PostingController : Controller
{
    /// <summary>
    /// Форма "Напомнить пароль"
    /// </summary>
    /// <returns></returns>
    public ActionResult vk(string code)
    {
        string result = code + "<br />";

        if (String.IsNullOrEmpty(code))
        {
            // отправляем запрос на авторизацию
            string GetCode_Url = "https://oauth.vk.com/authorize?client_id=" + Settings.vkApp + "&display=popup&redirect_uri=http://posting.musicman.tv/vk&scope=photos,wall,groups,offline&response_type=code&v=5.73";

            Response.Redirect(GetCode_Url);
        }
        else
        {
            // Получаем ID пользователя и токин
            string GetTokin_Url = "https://oauth.vk.com/access_token?client_id=" + Settings.vkApp + "&client_secret=" + Settings.vkAppKey + "&redirect_uri=http://posting.musicman.tv/vk&code=" + code;
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            string json = client.DownloadString(GetTokin_Url);
            VkLoginModel vkUser = JsonConvert.DeserializeObject<VkLoginModel>(json);
            
            string reqStr = string.Format("https://api.vk.com/method/wall.post?owner_id={0}&access_token={1}&message={2}", Settings.vkGroupId, vkUser.access_token, "Test massege site musicman.tv");

            result += Settings.vkGroupId + " -|- " + json + "<br />";
            result += vkUser.access_token + "<br />";
            result += reqStr + "<br />";

            //if (!string.IsNullOrEmpty(Link))
            //    reqStr += string.Format("&attachment={0}", System.Web.HttpUtility.UrlEncode(Link));

            //if (!string.IsNullOrEmpty(CapchaID))
            //    reqStr += string.Format("&captcha_sid={0}", CapchaID);

            //if (!string.IsNullOrEmpty(CapchaKey))
            //    reqStr += string.Format("&captcha_key={0}", CapchaKey);

            WebClient webClient = new WebClient();
            result += webClient.DownloadString(reqStr);

        }

        //string reqStr = string.Format("https://api.vk.com/method/wall.post?owner_id={0}&access_token={1}&from_group=1&message={2}&version=5.73", Settings.vkGroupId, "21341ff1575d8c7e8b38e2ac262581b529150159ce4b5df9bb0ef284074b50690de4e3b8e3bc0e13034f3", "Test massege site musicman.tv");

        //result += reqStr + "<br />";

        //WebClient webClient = new WebClient();
        //result += webClient.DownloadString(reqStr);

        ViewBag.Result = result;


        return View();
    }
}