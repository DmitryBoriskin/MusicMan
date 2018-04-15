using Admin.Models;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class MainController : CoreController
    {
        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            MainViewModel model = new MainViewModel()
            {
                Account = AccountInfo,
                Statistic = _cmsRepository.getStatistic(14)
                //AccountLog = _cmsRepository.getCmsUserLog(AccountInfo.id)
            };

            #region Метатеги
            ViewBag.Title = "Главная";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }
    }
}