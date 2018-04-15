using Admin.Models;
using cms.dbModel.entity;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class UsersController : CoreController
    {
        UsersViewModel model;
        FilterParams filter;

        int page_size = 40;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
            model = new UsersViewModel()
            {
                Account = AccountInfo,
                CategoryList = _cmsRepository.getUsersCategorys()
        };

            #region Ссылки
            ViewBag.createBtn = createBtn.ToLower();
            ViewBag.cancelBtn = cancelBtn.ToLower();
            ViewBag.clearBtn = clearBtn.ToLower();
            #endregion
            #region Метатеги
            ViewBag.Title = "Пользователи сайта";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        /// <summary>
        /// Страница по умолчанию (Список)
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Наполняем фильтр значениями
            filter = getFilter(page_size);
            filter.Group = "user";
            // Наполняем модель данными
            model.List = _cmsRepository.getUsersList(filter);

            return View(model);
        }
                
        /// <summary>
        /// Форма редактирования записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid Id)
        {
            model.Item = _cmsRepository.getUser(Id);
            model.GroupList = new SelectList(_cmsRepository.getUsersGroupList(), "value", "text", model.Item.Group);

            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Item(Guid Id, UsersViewModel back_model, HttpPostedFileBase posted_Photo)
        {
            back_model.Item.Id = Id;

            ErrorMassege userMassege = new ErrorMassege();
            userMassege.title = "Информация";

            if (ModelState.IsValid)
            {

                #region Сохраняем картинку
                // путь для сохранения изображения //Preview image
                string savePath = Settings.ContentDir + "/" + back_model.Item.PageName + "/";
                if (posted_Photo != null && posted_Photo.ContentLength > 0)
                {
                    string fileExtension = posted_Photo.FileName.Substring(posted_Photo.FileName.LastIndexOf(".")).ToLower();

                    var validExtension = "jpg,jpeg,png,gif".Split(',');
                    if (validExtension.Contains(fileExtension.Replace(".", "")))
                    {
                        ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                        EncoderParameters myEncoderParameters = new EncoderParameters(1);
                        myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                        Bitmap _File = (Bitmap)Bitmap.FromStream(posted_Photo.InputStream);

                        string orientation = string.Empty;

                        _File = Imaging.Resize(_File, 300, 350, "top", "center");

                        if (!Directory.Exists(Server.MapPath(savePath))) { Directory.CreateDirectory(Server.MapPath(savePath)); }

                        string filePath = Server.MapPath(savePath + posted_Photo.FileName);

                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);
                        _File.Save(filePath, myImageCodecInfo, myEncoderParameters);
                        _File.Dispose();

                        back_model.Item.Photo = savePath + posted_Photo.FileName;
                    }
                }
                #endregion
                if (_cmsRepository.check_user(Id))
                {
                    _cmsRepository.saveUser(Id, back_model.Item, AccountInfo.id, Request.ServerVariables["REMOTE_ADDR"]);
                    userMassege.info = "Запись обновлена";
                }
                else if (!_cmsRepository.check_user(back_model.Item.EMail))
                {
                    char[] _pass = back_model.Password.Password.ToCharArray();
                    Cripto password = new Cripto(_pass);
                    string NewSalt = password.Salt;
                    string NewHash = password.Hash;

                    back_model.Item.Salt = NewSalt;
                    back_model.Item.Hash = NewHash;

                    _cmsRepository.saveUser(Id, back_model.Item, AccountInfo.id, Request.ServerVariables["REMOTE_ADDR"]);

                    userMassege.info = "Запись добавлена";
                }
                else
                {
                    userMassege.info = "Пользователь с таким EMail адресом уже существует.";
                }

                userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "вернуться в список" },
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }
            else
            {
                userMassege.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";

                userMassege.buttons = new ErrorMassegeBtn[]{
                    new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                };
            }

            model.Item = _cmsRepository.getUser(Id);
            model.GroupList = new SelectList(_cmsRepository.getUsersGroupList(), "value", "text", model.Item.Group);
            model.ErrorInfo = userMassege;

            return View("Item", model);
        }


        /// <summary>
        /// Формируем строку фильтра
        /// </summary>
        /// <param name="title_serch">Поиск по названию</param>
        /// <param name="search-btn">Поиск по доменному имени</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(string searchtext, bool disabled, string size)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "searchtext", searchtext);
            query = addFiltrParam(query, "disabled", disabled.ToString().ToLower());
            query = addFiltrParam(query, "page", String.Empty);
            query = addFiltrParam(query, "size", size);
            
            return Redirect(StartUrl + query);
        }
        
        [HttpPost]
        public ActionResult Delete(Guid Id)
        {
            _cmsRepository.deleteUser(Id, AccountInfo.id, Request.ServerVariables["REMOTE_ADDR"]);

            return Redirect(StartUrl + Request.Url.Query);
        }
    }
}