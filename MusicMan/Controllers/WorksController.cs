using cms.dbase;
using cms.dbModel.entity;
using MusicMan.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MusicMan.Controllers
{
    public class WorksController : CoreController
    {

        WorksViewModel model;
        FilterParams filter;
        protected int maxLoginError = 5;
        int page_size = 40;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new WorksViewModel()
            {
                Account = AccountInfo,
                Settings = ContactsInfo
            };

            #region Метатеги
            ViewBag.Title = "Авторизация";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }
        
        /// <summary>
        /// Страница списка пользователей
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public ActionResult Index(string type)
        {
            // Наполняем фильтр значениями
            filter = getFilter(page_size);
            filter.Type = String.IsNullOrEmpty(type) ? null : type;
            filter.Sort = "main";

            model.List = db.getWorkList(filter);

            #region Метатеги
            if (type == "music") ViewBag.Title = "Музыка";
            else if (type == "video") ViewBag.Title = "Видео";
            else ViewBag.Title = "Записи пользователей";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(type, model);
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            Response.ContentType = "application/json; charset=utf-8";

            if (model.Account != null)
            {
                WorkModel Item = db.getWork(id);

                if (Item.UserId == model.Account.id)
                {
                    db.deleteWork(id, Request.ServerVariables["REMOTE_ADDR"]);
                    return Json(new { Result = "1" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Result = "0" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { Result = "0" }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult Like(Guid Id)
        {
            Response.ContentType = "application/json; charset=utf-8";

            int count = db.getLikes(Id);

            if (AccountInfo != null)
            {
                if (Id != Guid.Empty)
                {
                    count = db.setLike(AccountInfo.id, Id);
                    string status = db.checkLike(AccountInfo.id, Id) ? "active" : String.Empty;

                    return Json(new { Result = "", Сount = count.ToString(), Status = status }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Result = "", Сount = count.ToString(), Status = "" }, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(new { Result = "    <span class=\"label label-danger\">Оценивать работы могут только зарегистрированные пользователи.</span>", Сount = count.ToString(), Status = "" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}