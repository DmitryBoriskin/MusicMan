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
    public class BannersController : CoreController
    {
        BannersViewModel model;
        FilterParams filter;

        int page_size = 10;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new BannersViewModel()
            {
                Account = AccountInfo
            };

            #region Ссылки
            ViewBag.createBtn = createBtn.ToLower();
            ViewBag.cancelBtn = cancelBtn.ToLower();
            ViewBag.clearBtn = clearBtn.ToLower();
            #endregion
            #region Метатеги
            ViewBag.Title = "Баннеры";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        public ActionResult Index(string category, string type)
        {
            // Наполняем фильтр значениями
            filter = getFilter(page_size);
            // Наполняем модель данными
            model.List = _cmsRepository.getBannersList(filter);

            return View(model);
        }
        
        /// <summary>
        /// Форма редактирования записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid Id)
        {
            model.Item = _cmsRepository.getBanner(Id);
            if (model.Item == null)
                model.Item = new BannerModel()
                {
                    Id = Id,
                    Date = DateTime.Now
                };
            return View("Item", model);
        }
        
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Item(Guid Id, BannersViewModel bindData, HttpPostedFileBase posted_Image)
        {
            bindData.Item.Id = Id;
            //if (bindData.Item.DateEnd)

            ErrorMassege userMessage = new ErrorMassege();
            userMessage.title = "Информация";

            if (ModelState.IsValid)
            {
                #region Сохраняем картинку
                // путь для сохранения изображения //Preview image
                string savePath = Settings.ContentDir + "image_link/" + Id + "/";
                if (posted_Image != null && posted_Image.ContentLength > 0)
                {
                    string fileExtension = posted_Image.FileName.Substring(posted_Image.FileName.LastIndexOf(".")).ToLower();

                    var validExtension = "jpg,jpeg,png,gif".Split(',');
                    if (validExtension.Contains(fileExtension.Replace(".", "")))
                    {
                        ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                        EncoderParameters myEncoderParameters = new EncoderParameters(1);
                        myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                        Bitmap _File = (Bitmap)Bitmap.FromStream(posted_Image.InputStream);

                        string orientation = string.Empty;

                        _File = Imaging.Resize(_File, 282, 155, "top", "center");

                        if (!Directory.Exists(Server.MapPath(savePath))) { Directory.CreateDirectory(Server.MapPath(savePath)); }

                        string filePath = Server.MapPath(savePath + "image" + fileExtension);

                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);
                        _File.Save(filePath, myImageCodecInfo, myEncoderParameters);
                        _File.Dispose();

                        bindData.Item.Image = savePath + "image" + fileExtension;
                    }
                }
                #endregion

                bool res = _cmsRepository.creditBanner(bindData.Item, model.Account.id, Request.ServerVariables["REMOTE_ADDR"]);

                //Сообщение пользователю
                if (res)
                    userMessage.info = "Запись добавлена";
                else
                    userMessage.info = "Запись обновлена";

                userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = StartUrl + Request.Url.Query, text = "Вернуться в список" },
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                 };
            }
            else
            {
                userMessage.info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом.";

                userMessage.buttons = new ErrorMassegeBtn[]{
                     new ErrorMassegeBtn { url = "#", text = "ок", action = "false" }
                 };
            }

            model.ErrorInfo = userMessage;
            model.Item = bindData.Item;

            return View("Item", model);
        }
        
        [HttpPost]
        public ActionResult Delete(Guid Id)
        {
            var res = _cmsRepository.deleteBanner(Id);

            return Redirect(StartUrl + Request.Url.Query);

        }
    }
}