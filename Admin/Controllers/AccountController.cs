using Admin.Models;
using cms.dbase;
using cms.dbModel.entity;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Admin.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        protected bool _IsAuthenticated = System.Web.HttpContext.Current.User.Identity.IsAuthenticated; 
        protected bool _DomainSecurity = false;
        protected AccountRepository _accountRepository;
        protected cmsRepository _cmsRepository;
        protected int maxLoginError = 5;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            _accountRepository = new AccountRepository("dbConnection");
            _cmsRepository = new cmsRepository("dbConnection");

            _DomainSecurity = _cmsRepository.DomainSecurity(Request.Url.Host.ToLower());
            if (!_DomainSecurity) Response.Redirect("/");

            #region Метатеги
            ViewBag.Title = "Авторизация";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        /// <summary>
        /// Форма "Напомнить пароль"
        /// </summary>
        /// <returns></returns>
        public ActionResult RestorePass()
        {
            // Авторизованного пользователя направляем на главную страницу
            if (_IsAuthenticated) return RedirectToAction("", "Main");
            else return View();
        }

        [HttpPost]
        public ActionResult RestorePass(RestoreModel model)
        {
            try
            {
                string _login = model.Email;
                AccountModel AccountInfo = _accountRepository.getCmsAccount(_login);

                // Ошибки в форме
                if (!ModelState.IsValid)
                {
                    // пустое поле
                    if (_login == null || _login == "")
                    {
                        ModelState.AddModelError("", "Поле \"E-Mail\" не заполнено. Для восстановления пароля введите адрес почты.");
                    }
                    return View(model);
                }

                // существует ли адрес
                if (AccountInfo != null)
                {
                    // Формируем код востановления пароля
                    Guid RestoreCode = Guid.NewGuid();
                    _accountRepository.setRestorePassCode(AccountInfo.id, RestoreCode, Request.ServerVariables["REMOTE_ADDR"]);

                    #region оповещение на e-mail
                    string Massege = String.Empty;
                    Mailer Letter = new Mailer();
                    Letter.Theme = "Изменение пароля";
                    Massege = "<p>Уважаемый " + AccountInfo.Name + " " + AccountInfo.LastName + ", Вы отправили запрос на смену пароля на сайте " + Request.Url.Host + ".</p>";
                    Massege += "<p>Для вас сформирована ссылка, перейдя по которой, Вы сможете ввести новый пароль для вашего аккаунта.</p>";
                    Massege += "<p><a href=\"http://" + Request.Url.Host + "/Account/ChangePass/" + RestoreCode + "/\">http://" + Request.Url.Host + "/Admin/Account/ChangePass/" + RestoreCode + "/</a></p>";
                    Massege += "<p>С уважением, администрация сайта!</p>";
                    Massege += "<hr><i><span style=\"font-size:11px\">Это сообщение отпралено роботом, на него не надо отвечать</i></span>";
                    Letter.MailTo = AccountInfo.Mail;
                    Letter.Text = Massege;
                    string ErrorText = Letter.SendMail();
                    #endregion

                    return RedirectToAction("MsgSendMail", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Адрес почты заполнен неверно. Попробуйте ещё раз");
                }
                return View();

            }
            catch (HttpAntiForgeryException ex)
            {
                return View();
            }
        }

        /// <summary>
        /// Форма "Изменить пароль"
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePass(Guid id)
        {
            // Авторизованного пользователя направляем на главную страницу
            if (_IsAuthenticated) 
                return RedirectToAction("", "Main");

            string ViewName = "~/Views/Account/ChangePass.cshtml";

            // Проверка кода востановления пароля
            if (!_accountRepository.getCmsAccountCode(id))
                ViewName = "~/Views/Account/MsgFailRestore.cshtml";


            return View(ViewName);
        }
        
        [HttpPost]
        public ActionResult ChangePass(Guid id,PasswordModel model)
        {
            if (ModelState.IsValid)
            {
                string NewPass = model.Password;

                Cripto pass = new Cripto(NewPass.ToCharArray());
                string NewSalt = pass.Salt;
                string NewHash = pass.Hash;

                _accountRepository.changePasByCode(id, NewSalt, NewHash, Request.ServerVariables["REMOTE_ADDR"]);

                return RedirectToAction("MsgResult", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Ошибки в заполнении формы.");
            }

            return View();
        }
        
        /// <summary>
        /// Сообщение об отправке письма для смены пароля
        /// </summary>
        /// <returns></returns>
        public ActionResult MsgSendMail()
        {
            // Авторизованного пользователя направляем на главную страницу
            if (_IsAuthenticated) return RedirectToAction("", "Main");
            return View();
        }

        /// <summary>
        /// Сообщение о некоректности кода востановления пароля
        /// </summary>
        /// <returns></returns>
        public ActionResult MsgFailRestore()
        {
            // Авторизованного пользователя направляем на главную страницу
            if (_IsAuthenticated) return RedirectToAction("", "Main");
            return View();
        }
        
        /// <summary>
        /// Сообщение о смене пароля
        /// </summary>
        /// <returns></returns>
        public ActionResult MsgResult()
        {
            // Авторизованного пользователя направляем на главную страницу
            if (_IsAuthenticated) return RedirectToAction("", "Main");
            return View();
        }

        /// <summary>
        /// Закрываем сеанс работы с CMS
        /// </summary>
        /// <returns></returns>
        public ActionResult logOff()
        {
            AccountModel AccountInfo = _accountRepository.getCmsAccount(new Guid(User.Identity.Name));
            //_accountRepository.insertLog(AccountInfo.id, Request.ServerVariables["REMOTE_ADDR"], "log_off", AccountInfo.id, "", "account","");

            HttpCookie MyCookie = new HttpCookie(".MusicMan");
            MyCookie.Expires = DateTime.Now.AddDays(-1d);
            MyCookie.Value = HttpUtility.UrlEncode("", System.Text.Encoding.UTF8);
            MyCookie.Domain = "." + Settings.BaseURL;
            Response.Cookies.Add(MyCookie);
            FormsAuthentication.SignOut();


            return RedirectToAction("index", "Home");
        }
    }
}