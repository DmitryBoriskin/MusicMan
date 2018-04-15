using cms.dbModel.entity;
using MusicMan.Models;
using System;
using System.Web.Mvc;

namespace MusicMan.Controllers
{
    public class BlogController : CoreController
    {
        MaterialsViewModel model;
        FilterParams filter;

        int page_size = 40;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            model = new MaterialsViewModel()
            {
                Account = AccountInfo,
                Settings = ContactsInfo
            };

            #region Метатеги
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Новости";
            
            // Наполняем фильтр значениями
            filter = getFilter(page_size);
            // Наполняем модель данными
            model.List = db.getMaterialsList(filter);

            return View(model);
        }


        ///// <summary>
        ///// Форма редактирования записи
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult Item(int year, int manth, int day, string alias)
        //{
        //    model.Item = db.getMaterial(year, manth, day, alias);
            
        //    return View("Item", model);
        //}
    }
}