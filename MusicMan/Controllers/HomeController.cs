using MusicMan.Models;
using System.Web.Mvc;

namespace MusicMan.Controllers
{
    public class HomeController : CoreController
    {
        HomeViewModel model;
        int page_size = 40;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new HomeViewModel()
            {
                Account = AccountInfo,
                Settings = ContactsInfo

            };

            ViewBag.Marquee = db.getSettings().Marquee;
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            #region Метатеги
            ViewBag.Title = "Главная";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }
        
    }
}