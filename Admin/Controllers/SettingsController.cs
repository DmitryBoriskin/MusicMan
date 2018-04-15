using Admin.Models;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class SettingsController : CoreController
    {
        SettingsViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new SettingsViewModel()
            {
                Account = AccountInfo
            };

            #region Метатеги
            ViewBag.Title = "Настройки сайта";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }
               
        /// <summary>
        /// Форма редактирования записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            model.Item = _cmsRepository.getSettings();

            return View("Index", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(SettingsViewModel back_model)
        {
            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";

            if (ModelState.IsValid)
            {
                _cmsRepository.updateSettings(back_model.Item, AccountInfo.id, Request.ServerVariables["REMOTE_ADDR"]);
                userMassege.info = "Запись обновлена";

                userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "/settings/", text = "ок", action = "false" }
                };
            }
            else
            {
                userMassege.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";

                userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "/settings/", text = "ок", action = "false" }
                };
            }

            model.Item = _cmsRepository.getSettings();
            model.ErrorInfo = userMassege;

            return View("Index", model);
        }
    }
}