using MusicMan.Models;
using System;
using System.Web.Mvc;

namespace MusicMan.Controllers
{
    public class PageController : CoreController
    {
        PageViewModel model;
        FilterParams filter;

        int page_size = 40;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            model = new PageViewModel()
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
        public ActionResult Index(string path)
        {
            ViewBag.UserId = path;
            
            #region Метатеги
            ViewBag.Title = "О проекте";
            ViewBag.PageInfo = db.getSettings().Info;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Music(string path)
        {
            ViewBag.UserId = path;

            #region Метатеги
            ViewBag.Title = "Музыка";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Video(string path)
        {
            ViewBag.UserId = path;

            #region Метатеги
            ViewBag.Title = "Видео";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }

        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Events(string path)
        {
            ViewBag.UserId = path;

            #region Метатеги
            ViewBag.Title = "Афиша";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }
        
        /// <summary>
        /// Сраница по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Contacts(string path)
        {
            ViewBag.UserId = path;
            ViewBag.PageInfo = db.getSettings().Contacts;

            #region Метатеги
            ViewBag.Title = "Контакты";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }

        [HttpPost]
        public ActionResult Contacts(postMassege backModel)
        {
            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";

            if (backModel != null)
            {
                string PrivateKey = Settings.SecretKey;
                string EncodedResponse = Request["g-Recaptcha-Response"];
                bool IsCaptchaValid = (ReCaptchaClass.Validate(PrivateKey, EncodedResponse).ToLower() == "true" ? true : false);

                //db.createMassege(backModel);
                if (IsCaptchaValid)
                { 
                    #region Оповещение
                    string Massege = String.Empty;
                    Mailer Letter = new Mailer();
                    Letter.Theme = "Сообщение с сайта " + Settings.BaseURL;
                    Massege = "<p>Здравствуйте, Администратор.</p>";
                    Massege += "<p>Пользователь " + backModel.Name + ":</p>";
                    if (!String.IsNullOrEmpty(backModel.Country)) Massege += "<p><b>Проживает:</b> " + backModel.Country + ":</p>";
                    if (!String.IsNullOrEmpty(backModel.Mail)) Massege += "<p><b>Email:</b> " + backModel.Mail + ":</p>";
                    if (!String.IsNullOrEmpty(backModel.Phone)) Massege += "<p><b>Телефон:</b> " + backModel.Phone + ":</p>";
                    Massege += "<p><b>Написал сообщение:</b><br/> " + backModel.Massege + ":</p>";
                    Massege += "<hr><i><span style=\"font-size:12px\">Это сообщение отпралено роботом, на него не надо отвечать</i></span>";
                    Letter.MailTo = ContactsInfo.Mail;
                    Letter.Text = Massege;
                    string ErrorText = Letter.SendMail();
                    #endregion

                    return RedirectToAction("SendMail", "Page");
                }

                userMassege.info = "Не пройдена проверка \"Я не робот\".";
            }
            else
            {
                userMassege.info = "Произошла ошибка, попробуйте снова.";
            }

            userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };

            model.ErrorInfo = userMassege;

            #region Метатеги
            ViewBag.Title = "Контакты";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }
                
        public ActionResult Advertising(string path)
        {
            ViewBag.UserId = path;
            ViewBag.PageInfo = db.getSettings().Advertising;

            #region Метатеги
            ViewBag.Title = "Реклама на сайте";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }

        public ActionResult Helping(string path)
        {
            ViewBag.UserId = path;
            ViewBag.PageInfo = db.getSettings().Helping;

            #region Метатеги
            ViewBag.Title = "Помощь проекту";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }

        public ActionResult Regulations()
        {
            ViewBag.PageInfo = "";

            #region Метатеги
            ViewBag.Title = "Правила пользования сайтом";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }

        public ActionResult Producing()
        {
            ViewBag.PageInfo = "";

            #region Метатеги
            ViewBag.Title = "Продюсирование";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }

        public ActionResult Partners()
        {
            ViewBag.PageInfo = "";

            #region Метатеги
            ViewBag.Title = "Партнеры";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }

        public ActionResult Mission()
        {
            ViewBag.PageInfo = "";

            #region Метатеги
            ViewBag.Title = "Наша миссия";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }

        public ActionResult SendMail()
        {
            #region Метатеги
            ViewBag.Title = "Сообщение редактору сайта";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }
    }
}