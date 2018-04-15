using cms.dbase;
using cms.dbModel.entity;
using MusicMan.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MusicMan.Controllers
{
    public class TemplatesController : Controller
    {
        protected FrontRepository db { get; private set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            db = new FrontRepository("dbConnection");
        }

        /// <summary>
        /// Метод для плстроения меню
        /// </summary>
        /// <param name="viewName">Имя представления</param>
        /// <returns></returns>
        public ActionResult Menu(string viewName = "Default")
        {
            viewName = "Templates/Menu/" + viewName;

            return View(viewName);
        }

        public ActionResult Slider(string sectionId, string viewName = "Templates/Slider/Default")
        {
            BannerModel[] viewModel = db.getSlider();

            return View(viewName, viewModel);
        }

        public ActionResult News(int ListSize, string viewName = "Templates/News/Default")
        {
            //MaterialsList viewModel = db.getMaterialsList(ListSize);

            //return View(viewName, viewModel);
            FilterParams filter = new FilterParams();
            filter.Type = "text";
            filter.SearchText = String.Empty;
            filter.Page = 1;
            filter.Size = ListSize;
            filter.Main = true;

            WorkList viewModel = db.getWorkList(filter);

            return View(viewName, viewModel);
        }

        public ActionResult Works(int ListSize, string type = "video", string viewName = "Templates/Works/Default")
        {
            FilterParams filter = new FilterParams();
            filter.Type = type;
            filter.SearchText = String.Empty;
            filter.Page = 1;
            filter.Size = ListSize;
            filter.Sort = "main";

            WorkList viewModel = db.getWorkList(filter);

            return View(viewName, viewModel);
        }

        /// <summary>
        /// Метод для плстроения меню
        /// </summary>
        /// <param name="viewName">Имя представления</param>
        /// <returns></returns>
        public ActionResult Like(Guid WorkId, Guid? UserId, string viewName = "Default")
        {
            viewName = "Templates/Like/" + viewName;

            if (UserId == null)
                ViewBag.Active = "";
            else
                ViewBag.Active = db.checkLike((Guid)UserId, WorkId) ? "active" : "";

            ViewBag.count = db.getLikes(WorkId);

            return View(viewName);
        }

        public ActionResult Peoples(int ListSize, string Group, string viewName = "Templates/Peoples/Default")
        {
            FilterParams filter = new FilterParams();
            filter.Group = String.IsNullOrEmpty(Group) ? null : Group;
            filter.SearchText = String.Empty;
            filter.Page = 1;
            filter.Size = ListSize;

            UsersList viewModel = db.getUsersList(filter);

            return View(viewName, viewModel);
        }
        
        public ActionResult Pager(Pager Model, string startUrl, string viewName = "Templates/Pager/Default")
        {
            ViewBag.PagerSize = string.IsNullOrEmpty(Request.QueryString["size"]) ? Model.size.ToString() : Request.QueryString["size"];
            string qwer = String.Empty;

            int PagerLinkSize = 2;

            int FPage = (Model.page - PagerLinkSize < 1) ? 1 : Model.page - PagerLinkSize;
            int LPage = (Model.page + PagerLinkSize > Model.page_count) ? Model.page_count : Model.page + PagerLinkSize;

            if (String.IsNullOrEmpty(startUrl)) startUrl = Request.Url.Query;

            if (FPage > 1)
            {
                qwer = qwer + "1,";
            }
            if (FPage > 2)
            {
                qwer = qwer + "*,";
            }
            for (int i = FPage; i < LPage + 1; i++)
            {
                qwer = (@i < Model.page_count) ? qwer + @i + "," : qwer + @i;
            }
            if (LPage < Model.page_count - 1)
            {
                qwer = qwer + "*,";
            }
            if (Model.page_count > LPage)
            {
                qwer = qwer + @Model.page_count;
            }


            var viewModel = qwer.Split(',').
                Where(w => w != String.Empty).
                Select(s => new PagerModel
                {
                    text = (s == "*") ? "..." : s,
                    url = (s == "*") ? String.Empty : addFiltrParam(startUrl, "page", s),
                    isChecked = (s == Model.page.ToString())
                }).ToArray();

            if (viewModel.Length < 2) viewModel = null;

            return View(viewName, viewModel);
        }

        public string addFiltrParam(string query, string name, string val)
        {
            //string search_Param = @"\b" + name + @"=[\w]*[\b]*&?";
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
            string StartUrl = "/" + (String)RouteData.Values["controller"] + "/";
            string return_url = HttpUtility.UrlDecode(Request.Url.Query);
            // если в URL номер страницы равен значению по умолчанию - удаляем его из URL
            try
            {
                return_url = (Convert.ToInt32(Request.QueryString["page"]) == 1) ? addFiltrParam(return_url, "page", String.Empty) : return_url;
            }
            catch
            {
                return_url = addFiltrParam(return_url, "page", String.Empty);
            }
            try
            {
                return_url = (Convert.ToInt32(Request.QueryString["size"]) == defaultPageSize) ? addFiltrParam(return_url, "size", String.Empty) : return_url;
            }
            catch
            {
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

    }
}