using cms.dbase;
using cms.dbModel.entity;
using System;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Admin.Controllers
{
    [Authorize]
    public class CoreController : Controller
    {
        /// <summary>
        /// Контекст доступа к базе данных
        /// </summary>
        protected AccountRepository _accountRepository { get; private set; }
        protected cmsRepository _cmsRepository { get; private set; }
        
        public string StartUrl;
        public string query;
        public string createBtn;
        public string cancelBtn;
        public string clearBtn;

        public AccountModel AccountInfo;
        public cmsLogModel[] LogInfo;

        
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
            string _ControllerName = filterContext.RouteData.Values["Controller"].ToString().ToLower();
            string _ActionName = filterContext.RouteData.Values["Action"].ToString().ToLower();
            
            #region Данные об авторизованном пользователе
            Guid _userId = new Guid();
            try { _userId = new Guid(System.Web.HttpContext.Current.User.Identity.Name); }
            catch { FormsAuthentication.SignOut(); }
            AccountInfo = _accountRepository.getCmsAccount(_userId);
            #endregion
            #region Для интерфейса
            StartUrl = "/" + (String)RouteData.Values["controller"] + "/";
            query = HttpUtility.UrlDecode(Request.Url.Query);
            createBtn = StartUrl + "Item/" + Guid.NewGuid() + "/" + addFiltrParam(query, "page", String.Empty);
            cancelBtn = StartUrl + query;
            clearBtn = StartUrl;
            #endregion
        }
        
        public CoreController()
        {
            _accountRepository = new AccountRepository("dbConnection");
            _cmsRepository = new cmsRepository("dbConnection");
        }
        
        public string addFiltrParam(string query, string name, string val)
        {
            string search_Param = @"\b" + name + @"=(.*?)(&|$)";
            string normal_Query = @"&$";

            Regex delParam = new Regex(search_Param, RegexOptions.CultureInvariant);
            Regex normalQuery = new Regex(normal_Query);
            query = delParam.Replace(query, String.Empty);
            query = normalQuery.Replace(query, String.Empty);
            
            if (val != String.Empty)
            {
                if (query.IndexOf("?") > -1) query += "&" + name + "=" + val;
                else query += "?" + name + "=" + val;
            }

            query = query.Replace("?&", "?").Replace("&&", "&");

            return query;
        }

        public FilterParams getFilter(int defaultPageSize = 20)
        {
            string return_url = HttpUtility.UrlDecode(Request.Url.Query);
            // если в URL номер страницы равен значению по умолчанию - удаляем его из URL
            try
            {
                return_url = (Convert.ToInt32(Request.QueryString["page"]) == 1) ? addFiltrParam(return_url, "page", String.Empty) : return_url;
            }
            catch {
                return_url = addFiltrParam(return_url, "page", String.Empty);
            }
            try {
                return_url = (Convert.ToInt32(Request.QueryString["size"]) == defaultPageSize) ? addFiltrParam(return_url, "size", String.Empty) : return_url;
            }
            catch {
                return_url = addFiltrParam(return_url, "size", String.Empty);
            }                
            return_url = (!Convert.ToBoolean(Request.QueryString["disabled"])) ? addFiltrParam(return_url, "disabled", String.Empty) : return_url;
            return_url = String.IsNullOrEmpty(Request.QueryString["searchtext"]) ? addFiltrParam(return_url, "searchtext", String.Empty) : return_url;
            // Если парамметры из адресной строки равны значениям по умолчанию - удаляем их из URL
            if (return_url.ToLower() != HttpUtility.UrlDecode(Request.Url.Query).ToLower())
                Response.Redirect(StartUrl + return_url);

            DateTime? DateNull = new DateTime?();

            FilterParams result = new FilterParams()
            {
                User = (String.IsNullOrEmpty(Request.QueryString["person"])) ? String.Empty : Request.QueryString["person"],
                Page = (Convert.ToInt32(Request.QueryString["page"]) > 0) ? Convert.ToInt32(Request.QueryString["page"]) : 1,
                Size = (Convert.ToInt32(Request.QueryString["size"]) > 0) ? Convert.ToInt32(Request.QueryString["size"]) : defaultPageSize,
                Type = (String.IsNullOrEmpty(Request.QueryString["type"])) ? String.Empty : Request.QueryString["type"],
                Categoty = (String.IsNullOrEmpty(Request.QueryString["category"])) ? String.Empty : Request.QueryString["category"],
                Group = (String.IsNullOrEmpty(Request.QueryString["group"])) ? String.Empty : Request.QueryString["group"],
                Lang = (String.IsNullOrEmpty(Request.QueryString["lang"])) ? String.Empty : Request.QueryString["lang"],
                Date = (String.IsNullOrEmpty(Request.QueryString["date"])) ? DateNull : DateTime.Parse(Request.QueryString["date"]),
                DateEnd = (String.IsNullOrEmpty(Request.QueryString["dateend"])) ? DateNull : DateTime.Parse(Request.QueryString["dateend"]),
                SearchText = (String.IsNullOrEmpty(Request.QueryString["searchtext"])) ? String.Empty : Request.QueryString["searchtext"],
                Disabled = (String.IsNullOrEmpty(Request.QueryString["disabled"])) ? false : Convert.ToBoolean(Request.QueryString["disabled"])
            };
            
            if (result.Date != DateNull && result.DateEnd == DateNull)
            {
                result.DateEnd = ((DateTime)result.Date).AddDays(1);
            }

            return result;
        }
        
        public static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            foreach (var enc in ImageCodecInfo.GetImageEncoders())
                if (enc.MimeType.ToLower() == mimeType.ToLower())
                    return enc;
            return null;
        }
    }
}