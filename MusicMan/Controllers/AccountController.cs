using cms.dbModel.entity;
using MusicMan.Models;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System;

namespace MusicMan.Controllers
{
    public class AccountController : CoreController
    {
        AccountViewModel model;
        protected int maxLoginError = 5;
        protected string UserIP = String.Empty;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
            UserIP = Request.ServerVariables["REMOTE_ADDR"];

            model = new AccountViewModel()
            {
                Account = AccountInfo,
                Settings = ContactsInfo,
                CategoryList = db.getUsersGroupList()
            };

            #region Метатеги
            ViewBag.Title = "Авторизация";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult LogIn()
        {
            // Авторизованного пользователя направляем на главную страницу
            if (User.Identity.IsAuthenticated) return Redirect("/" + model.Account.PageName + "/");

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult reg()
        {
            // Авторизованного пользователя направляем на главную страницу
            if (User.Identity.IsAuthenticated) return Redirect("/" + model.Account.PageName + "/");

            return View(model);
        }

        public ActionResult ConfirmMail(Guid code)
        {
            string ViewName = "~/Views/Account/MsgFail.cshtml";
            
            if (!db.ConfirmMail(code))
                ViewName = "~/Views/Account/MsgFailRestore.cshtml";
            else
            {
                AccountModel User = db.getAccount(code);
                FormsAuthentication.SetAuthCookie(User.id.ToString(), false);

                return Redirect("/Account/Edit/");
            }

            return View(ViewName,model);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult MsgResult()
        {
            #region Метатеги
            ViewBag.Title = "Сменить пароль";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit()
        {
            #region Метатеги
            ViewBag.Title = "Редактирование профиля";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult change_password()
        {
            #region Метатеги
            ViewBag.Title = "Сменить пароль";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult socseti()
        {
            #region Метатеги
            ViewBag.Title = "Связь с аккаунтами соц. сетей";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }

        public ActionResult Regulations()
        {
            model.Regulations = db.getSettings().Regulations;

            #region Метатеги
            ViewBag.Title = "Пользовательское соглашение";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
            
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LogIn(LogInModel back_model)
        {
            try
            {
                // Ошибки в форме
                if (!ModelState.IsValid) return View(back_model);

                string _login = back_model.Login;
                string _pass = back_model.Pass;
                bool _remember = back_model.RememberMe;

                AccountModel AccountInfo = db.getAccount(_login);

                // Если пользователь найден
                if (AccountInfo != null)
                {
                    if (AccountInfo.Disabled)
                    {
                        // Оповещение о блокировке
                        ModelState.AddModelError("", "Пользователь заблокирован и не имеет прав на доступ в систему администрирования");
                    }
                    else if (AccountInfo.CountError && AccountInfo.LockDate.Value.AddMinutes(15) >= DateTime.Now)
                    {
                        // Оповещение о блокировке
                        ModelState.AddModelError("", "После " + maxLoginError + " неудачных попыток авторизации Ваш пользователь временно заблокирован.");
                        ModelState.AddModelError("", "————");
                        ModelState.AddModelError("", "Вы можете повторить попытку через " + (AccountInfo.LockDate.Value.AddMinutes(16) - DateTime.Now).Minutes + " минут");
                    }
                    else
                    {
                        // Проверка на совпадение пароля
                        Cripto password = new Cripto(AccountInfo.Salt, AccountInfo.Hash);
                        if (password.Verify(_pass.ToCharArray()))
                        {
                            // Удачная попытка, Авторизация
                            FormsAuthentication.SetAuthCookie(AccountInfo.id.ToString(), _remember);

                            // Записываем данные об авторизации пользователя
                            db.SuccessLogin(AccountInfo.id, UserIP);

                            Response.Redirect("/" + AccountInfo.PageName);
                        }
                        else
                        {
                            // Неудачная попытка
                            // Записываем данные о попытке авторизации и плучаем кол-во неудавшихся попыток входа
                            int attemptNum = db.FailedLogin(AccountInfo.id, UserIP);
                            if (attemptNum == maxLoginError)
                            {
                                #region Оповещение о блокировке
                                // Формируем код востановления пароля
                                Guid RestoreCode = Guid.NewGuid();
                                db.setRestorePassCode(AccountInfo.id, RestoreCode, UserIP);

                                // оповещение на e-mail
                                string Massege = String.Empty;
                                Mailer Letter = new Mailer();
                                Letter.Theme = "Блокировка пользователя";
                                Massege = "<p>Уважаемый " + AccountInfo.Name+ " " + AccountInfo.LastName + ", в системе администрирования сайта " + Request.Url.Host + " было 5 неудачных попыток ввода пароля.<br />В целях безопасности, ваш аккаунт заблокирован.</p>";
                                Massege += "<p>Для восстановления прав доступа мы сформировали для Вас ссылку, перейдя по которой, Вы сможете ввести новый пароль для вашего аккаунта и учетная запись будет разблокирована.</p>";
                                Massege += "<p>Если вы вспомнили пароль и хотите ещё раз пропробовать авторизоваться, то подождите 15 минут. Спустя это время, система позволит Вам сделать ещё попытку.</p>";
                                Massege += "<p><a href=\"http://" + Request.Url.Host + "/Admin/Account/ChangePass/" + RestoreCode + "/\">http://" + Request.Url.Host + "/Admin/Account/ChangePass/" + RestoreCode + "/</a></p>";
                                Massege += "<p>С уважением, администрация сайта!</p>";
                                Massege += "<hr><i><span style=\"font-size:11px\">Это сообщение отпралено роботом, на него не надо отвечать</i></span>";
                                Letter.MailTo = AccountInfo.Mail;
                                Letter.Text = Massege;
                                string ErrorText = Letter.SendMail();
                                #endregion
                                ModelState.AddModelError("", "После " + maxLoginError + " неудачных попыток авторизации Ваш пользователь временно заблокирован.");
                                ModelState.AddModelError("", "Вам на почту отправлено сообщение с инструкцией по разблокировки и смене пароля.");
                                ModelState.AddModelError("", "---");
                                ModelState.AddModelError("", "Если вы хотите попробовать ещё раз, подождите 15 минут.");
                            }
                            else
                            {
                                // Оповещение об ошибке
                                string attemptCount = (maxLoginError - attemptNum == 1) ? "Осталась 1 попытка" : "Осталось " + (maxLoginError - attemptNum) + " попытки";
                                ModelState.AddModelError("", "Пара логин и пароль не подходят.");
                                ModelState.AddModelError("", attemptCount + " ввода пароля.");
                            }
                        }
                    }
                }
                else
                {
                    // Оповещение о неверном логине
                    ModelState.AddModelError("", "Такой пользователь не зарегистрирован в системе.");
                    ModelState.AddModelError("", "Проверьте правильность вводимых данных.");
                }


                return View(model);
            }
            catch (HttpAntiForgeryException ex)
            {
                return View(model);
            }
        }
        public ActionResult LogIn_vk(string code)
        {
            if (String.IsNullOrEmpty(code))
            { 
                // отправляем запрос на авторизацию
                string GetCode_Url = "https://oauth.vk.com/authorize?client_id=" + Settings.vkApp + "&display=popup&redirect_uri=http://musicman.tv/Account/LogIn_VK&scope=email&response_type=code&v=5.69";
                // https://oauth.vk.com/authorize?client_id=6238622&display=popup&redirect_uri=http://posting.musicman.tv/LogIn_VK&scope=email&response_type=code&v=5.69

                Response.Redirect(GetCode_Url);  
            }
            else 
            {
                // Получаем ID пользователя и токин
                string GetTokin_Url = "https://oauth.vk.com/access_token?client_id=" + Settings.vkApp + "&client_secret=" + Settings.vkAppKey + "&redirect_uri=http://musicman.tv/Account/LogIn_VK&code=" + code;
                //https://oauth.vk.com/access_token?client_id=6238622&client_secret=povaOL5kR5tPGLp1BJRI&redirect_uri=http://posting.musicman.tv/LogIn_VK&code=
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                string json = client.DownloadString(GetTokin_Url);
                VkLoginModel vkEnterUser = JsonConvert.DeserializeObject<VkLoginModel>(json);

                // Получаем данные пользователя
                string GetUserInfo_Url = "https://api.vk.com/method/users.get?user_id=" + vkEnterUser.user_id + "&fields=domain,nickname,country,city,contacts,has_photo,connections,photo_200_orig&v=5.69";
                //https://api.vk.com/method/users.get?user_id=&fields=domain,nickname,country,city,contacts,has_photo,connections,photo_200_orig&access_token=&v=5.69
                client = new WebClient();
                client.Encoding = Encoding.UTF8;
                json = client.DownloadString(GetUserInfo_Url);
                VkUserInfo vkUser = JsonConvert.DeserializeObject<VkUserInfo>(json);
                
                AccountModel AccountInfo = db.getAccount(vkUser.response[0].id.ToString());

                // Если пользователь найден
                if (AccountInfo != null)
                {
                    // Удачная попытка, Авторизация
                    FormsAuthentication.SetAuthCookie(AccountInfo.id.ToString(), false);
                    
                    // Записываем данные об авторизации пользователя
                    db.SuccessLogin(AccountInfo.id, UserIP);

                    Response.Redirect("/" + AccountInfo.PageName);
                }
                else
                {
                    char[] _pass = (DateTime.Now.ToString("DDssmmMMyyyy")).ToCharArray();
                    Cripto password = new Cripto(_pass);
                    string NewSalt = password.Salt;
                    string NewHash = password.Hash;

                    AccountModel User = new AccountModel();
                    User.id = Guid.NewGuid();
                    User.PageName = Transliteration.Translit(vkUser.response[0].domain);
                    User.Name = vkUser.response[0].first_name;
                    User.LastName = vkUser.response[0].last_name;
                    if (vkUser.response[0].has_photo) User.Photo = vkUser.response[0].photo_200_orig;
                    User.Mail = "";
                    User.Salt = NewSalt;
                    User.Hash = NewHash;
                    User.Group = "user";
                    User.Category = new string[] { "user"};
                    User.Disabled = false;
                    User.vkId = vkUser.response[0].id.ToString();

                    db.createAccount(User, UserIP);

                    // Удачная попытка, Авторизация
                    FormsAuthentication.SetAuthCookie(User.id.ToString(), false);

                    // Записываем данные об авторизации пользователя
                    db.SuccessLogin(User.id, UserIP);

                    Response.Redirect("/" + User.PageName);
                }

                ErrorMassege userMassege = new ErrorMassege();
                userMassege.title = "Информация";
                userMassege.info = json;
                
                model.ErrorInfo = userMassege;
            }

            return View(model);
        }
        public ActionResult LogIn_facebook(string code)
        {
            if (String.IsNullOrEmpty(code))
            {
                // отправляем запрос на авторизацию
                string GetCode_Url = "https://www.facebook.com/v2.11/dialog/oauth?client_id=" + Settings.fbApp + "&redirect_uri=http://musicman.tv/Account/LogIn_facebook/";

                Response.Redirect(GetCode_Url);
            }
            else
            {
                // Получаем ID пользователя и токин
                string GetTokin_Url = "https://graph.facebook.com/oauth/access_token?client_id=" + Settings.fbApp + "&redirect_uri=http://musicman.tv/Account/LogIn_facebook/&scope=email&client_secret=" + Settings.fbAppServKey+"&code="+ code;
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                string json = client.DownloadString(GetTokin_Url);
                FbLoginModel fbEnterUser = JsonConvert.DeserializeObject<FbLoginModel>(json);

                // Получаем данные пользователя
                string GetUserInfo_Url = "https://graph.facebook.com/me?fields=id,first_name,last_name,name,email&access_token=" + fbEnterUser.access_token;
                client = new WebClient();
                client.Encoding = Encoding.UTF8;
                json = client.DownloadString(GetUserInfo_Url);
                FbUserInfo fbUser = JsonConvert.DeserializeObject<FbUserInfo>(json);

                AccountModel AccountInfo = db.getAccount(fbUser.id);

                // Если пользователь найден
                if (AccountInfo != null)
                {
                    // Удачная попытка, Авторизация
                    FormsAuthentication.SetAuthCookie(AccountInfo.id.ToString(), false);

                    // Записываем данные об авторизации пользователя
                    db.SuccessLogin(AccountInfo.id, UserIP);

                    Response.Redirect("/" + AccountInfo.PageName);
                }
                else
                {
                    char[] _pass = (DateTime.Now.ToString("DDssmmMMyyyy")).ToCharArray();
                    Cripto password = new Cripto(_pass);
                    string NewSalt = password.Salt;
                    string NewHash = password.Hash;

                    AccountModel User = new AccountModel();
                    User.id = Guid.NewGuid();
                    User.PageName = String.IsNullOrEmpty(Transliteration.Translit(fbUser.name)) ? "fb" + fbUser.id : Transliteration.Translit(fbUser.name);
                    User.Name = fbUser.first_name;
                    User.LastName = fbUser.last_name;
                    //if (fbUser.has_photo) User.Photo = fbUser.photo_200_orig;
                    User.Mail = "";
                    User.Salt = NewSalt;
                    User.Hash = NewHash;
                    User.Group = "user";
                    User.Category = new string[] { "user" };
                    User.Disabled = false;
                    User.fbId = fbUser.id;

                    db.createAccount(User, UserIP);

                    // Удачная попытка, Авторизация
                    FormsAuthentication.SetAuthCookie(User.id.ToString(), false);

                    // Записываем данные об авторизации пользователя
                    db.SuccessLogin(User.id, UserIP);

                    Response.Redirect("/" + User.PageName);
                }

                ErrorMassege userMassege = new ErrorMassege();
                userMassege.title = "Информация";
                userMassege.info = json;

                model.ErrorInfo = userMassege;
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="back_model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "registration")]
        public ActionResult reg(RegModel back_model)
        {
            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";

            if (ModelState.IsValid)
            {
                //if (db.getAccount(back_model.PageName) != null)
                //{
                //    userMassege.info = "Выбранный Вами адрес страницы используется другим пользователем. Попробуйте ещё раз.";
                //}
                //else 
                if (db.getAccount(back_model.Email) != null)
                {
                    userMassege.info = "Пользователь с таким EMail адресом уже существует.";
                }
                else
                {
                    char[] _pass = back_model.Pass.ToCharArray();
                    Cripto password = new Cripto(_pass);
                    string NewSalt = password.Salt;
                    string NewHash = password.Hash;

                    AccountModel User = new AccountModel();
                    User.id = Guid.NewGuid();
                    User.PageName = User.id.ToString().Substring(0, 8);
                    User.Name = back_model.Name;
                    User.LastName = back_model.LastName;
                    User.Category = back_model.Category;
                    User.Mail = back_model.Email;
                    User.Salt = NewSalt;
                    User.Hash = NewHash;
                    User.Group = "user";
                    User.Disabled = true;

                    db.createAccount(User, UserIP);

                    #region Оповещение
                    string Massege = String.Empty;
                    Mailer Letter = new Mailer();
                    Letter.Theme = "Регистрация на сайте "+ Settings.BaseURL;
                    Massege = "<p>Здравствуйте, " + User.Name+ " " + User.LastName + "</p>";
                    Massege += "<p>Благодарим Вас за регистрацию на сайте " + Settings.SiteTitle + Settings.SiteDesc + ". Для подтверждения регистрация и активации вашего аккаунта, пожалуйста, перейдите по ссылке.</p>";
                    Massege += "<p>Если ваша почтовая программа не поддерживает прямые переходы, Вы можете скопировать данную ссылку в адресную строку браузера.</p>";
                    Massege += "<p><a href=\"http://" + Settings.BaseURL + "/Account/ConfirmMail/" + User.id + "/\">http://" + Settings.BaseURL + "/Account/ConfirmMail/" + User.id + "/</a></p>";
                    Massege += "<p>Если Вы не проходили регистрацию на сайте "+Settings.BaseURL+" и получили это письмо случайно, пожалуйста, удалите его.</p>";
                    Massege += "<p>С уважением,<br />Администрация сайта " + Settings.BaseURL + "</p>";
                    Massege += "<hr><i><span style=\"font-size:12px\">Это сообщение отпралено роботом, на него не надо отвечать</i></span>";
                    Letter.MailTo = User.Mail;
                    Letter.Text = Massege;
                    string ErrorText = Letter.SendMail();
                    #endregion

                    userMassege.info = "Вы успешно зарегистрированы. <br />Для подтверждения вашего EMail - вам отправлено письмо,<br /> следуйте инструкциям в нем.";

                    userMassege.buttons = new ErrorMassegeBtn[]{
                        new ErrorMassegeBtn { url = "/Account/login/", text = "ок" }
                    };

                    model.ErrorInfo = userMassege;
                }
            }
            else
            {
                userMassege.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";

                userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }

            model.ErrorInfo = userMassege;

            return View(model);
        }


        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "change_pass")]
        public ActionResult change_password(PasswordModel back_model)
        {
            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";

            if (ModelState.IsValid)
            {
                string NewPass = back_model.Password;

                Cripto pass = new Cripto(NewPass.ToCharArray());
                string NewSalt = pass.Salt;
                string NewHash = pass.Hash;

                db.changePasswordUser(model.Account.id, NewSalt, NewHash, UserIP);

                return RedirectToAction("MsgResult", "Account");
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
            ViewBag.Title = "Сменить пароль";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="back_model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "edit")]
        public ActionResult Edit(AccountModel back_model, HttpPostedFileBase posted_Photo)
        {
            string SaveUrl = Settings.ContentDir + "/" + back_model.PageName + "/";

            back_model.id = model.Account.id;

            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";

            if (ModelState.IsValid)
            {
                if (back_model.Category != null)
                {
                    if (db.getAccount(back_model.PageName, model.Account.id) == null)
                    {
                        if (db.getAccount(back_model.Mail, model.Account.id) == null)
                        {
                            // Если у пользователя уже была аватарка и он её меняет на другцю - удаляем старую
                            if (!String.IsNullOrEmpty(model.Account.Photo) && String.IsNullOrEmpty(back_model.Photo))
                            {
                                // проверяем, есть ли старая аватарка или уже удалена
                                if (System.IO.File.Exists(Server.MapPath(model.Account.Photo)))
                                {
                                    System.IO.File.Delete(Server.MapPath(model.Account.Photo));
                                }
                            }

                            if (posted_Photo != null)
                            {
                                // Если у пользователя нет своей директории с файлами - создаем её
                                if (!Directory.Exists(Server.MapPath(SaveUrl)))
                                    Directory.CreateDirectory(Server.MapPath(SaveUrl));

                                // Обрабатываем и сохраняем новую аватарку
                                using (Bitmap originalPic = new Bitmap(posted_Photo.InputStream, false))
                                {
                                    Bitmap prewImg = Imaging.Resize(originalPic, 300, 350, "top", "center");
                                    prewImg.Save(Server.MapPath(SaveUrl + posted_Photo.FileName));
                                    back_model.Photo = SaveUrl + posted_Photo.FileName;
                                }
                            }

                            if (db.updateAccount(back_model, UserIP))
                                userMassege.info = "Запись обновлена";
                            else
                                userMassege.info = "Произошла ошибка, попробуйте снова.";
                        }
                        else
                            userMassege.info = "Ошибка. Пользователь с таким E-Mail уже существует.";
                    }
                    else
                        userMassege.info = "Ошибка. Имя страницы " + back_model.PageName + " принадлежит другому пользователю";
                }
                else
                    userMassege.info = "Ошибка в заполнении формы. Не указано направление деятельности. Если вы ещё не определились - можете выбрать «Пользователь»";
            }
            else
                userMassege.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";

            userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };

            model.Account = db.getAccount(back_model.PageName);
            model.ErrorInfo = userMassege;

            #region Метатеги
            ViewBag.Title = "Редактирование профиля";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View("edit", model);
        }

        public ActionResult add_vk(string code)
        {
            if (String.IsNullOrEmpty(code))
            {
                // отправляем запрос на авторизацию
                string GetCode_Url = "https://oauth.vk.com/authorize?client_id=" + Settings.vkApp + "&display=popup&redirect_uri=http://musicman.tv/Account/add_VK&scope=email&response_type=code&v=5.69";

                Response.Redirect(GetCode_Url);
            }
            else
            {
                // Получаем ID пользователя и токин
                string GetTokin_Url = "https://oauth.vk.com/access_token?client_id=" + Settings.vkApp + "&client_secret=" + Settings.vkAppKey + "&redirect_uri=http://musicman.tv/Account/add_VK&code=" + code;
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                string json = client.DownloadString(GetTokin_Url);


                VkLoginModel vkEnterUser = JsonConvert.DeserializeObject<VkLoginModel>(json);

                // Получаем данные пользователя
                string GetUserInfo_Url = "https://api.vk.com/method/users.get?user_id=" + vkEnterUser.user_id + "&fields=domain,nickname,country,city,contacts,has_photo,connections,photo_200_orig&v=5.69";
                client = new WebClient();
                client.Encoding = Encoding.UTF8;
                json = client.DownloadString(GetUserInfo_Url);
                VkUserInfo vkUser = JsonConvert.DeserializeObject<VkUserInfo>(json);

                AccountModel AccountInfo = db.getAccount(vkUser.response[0].id.ToString());

                // Если пользователь найден
                if (AccountInfo == null)
                {
                    AccountModel User = model.Account;
                    User.vkId = vkUser.response[0].id.ToString();

                    db.setAccountVK(User, UserIP);

                    Response.Redirect("/Account/Edit/");
                }

                ErrorMassege userMassege = new ErrorMassege();
                userMassege.title = "Информация";
                userMassege.info = "Аккаунт добавлен";

                model.ErrorInfo = userMassege;
            }

            return View("~/Views/Account/edit.cshtml", model);
        }
        public ActionResult add_facebook(string code)
        {
            if (String.IsNullOrEmpty(code))
            {
                // отправляем запрос на авторизацию
                string GetCode_Url = "https://www.facebook.com/v2.11/dialog/oauth?client_id=" + Settings.fbApp + "&redirect_uri=http://musicman.tv/Account/add_facebook/";
                
                Response.Redirect(GetCode_Url);
            }
            else
            {
                // Получаем ID пользователя и токин
                string GetTokin_Url = "https://graph.facebook.com/oauth/access_token?client_id=" + Settings.fbApp + "&redirect_uri=http://musicman.tv/Account/add_facebook/&scope=email&client_secret=" + Settings.fbAppServKey + "&code=" + code;
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                string json = client.DownloadString(GetTokin_Url);
                FbLoginModel fbEnterUser = JsonConvert.DeserializeObject<FbLoginModel>(json);

                // Получаем данные пользователя
                string GetUserInfo_Url = "https://graph.facebook.com/me?fields=id,first_name,last_name,name,email&access_token=" + fbEnterUser.access_token;
                client = new WebClient();
                client.Encoding = Encoding.UTF8;
                json = client.DownloadString(GetUserInfo_Url);
                FbUserInfo fbUser = JsonConvert.DeserializeObject<FbUserInfo>(json);

                AccountModel AccountInfo = db.getAccount(fbUser.id);

                // Если пользователь найден
                if (AccountInfo == null)
                {
                    AccountModel User = model.Account;
                    User.fbId = fbUser.id;

                    db.setAccountFacebook(User, UserIP);
                    
                    Response.Redirect("/Account/Edit/");
                }

                ErrorMassege userMassege = new ErrorMassege();
                userMassege.title = "Информация";
                userMassege.info = "Аккаунт добавлен";

                model.ErrorInfo = userMassege;
            }

            return View("~/Views/Account/edit.cshtml", model);
        }

        /// <summary>
        /// Закрываем сеанс работы с личным кабинетом
        /// </summary>
        /// <returns></returns>
        public ActionResult logOff()
        {
            db.insertLog(AccountInfo.id, UserIP, "log_off", AccountInfo.id, "");

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