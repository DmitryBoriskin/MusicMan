using Admin.Models;
using cms.dbModel.entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class NewsController : CoreController
    {
        WorksViewModel model;
        FilterParams filter;

        int page_size = 40;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
             model = new WorksViewModel()
             {
                 Account = AccountInfo
             };

            #region Ссылки
            ViewBag.createBtn = createBtn.ToLower();
            ViewBag.cancelBtn = cancelBtn.ToLower();
            ViewBag.clearBtn = clearBtn.ToLower();
            #endregion
            #region Метатеги
            ViewBag.Title = "Новости";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        public ActionResult Index(string category, string type)
        {
            // Наполняем фильтр значениями
            filter = getFilter(page_size);
            filter.Type = "text";
            filter.Main = true;

            // Наполняем модель данными
            model.List = _cmsRepository.getWorkList(filter);
            model.Users = new SelectList(_cmsRepository.getUsersList(), "value", "text", filter.User);

            return View(model);
        }

        /// <summary>
        /// Форма редактирования записи
        /// </summary>
        /// <returns></returns>
        public ActionResult Item(Guid Id)
        {
            model.Item = _cmsRepository.getWork(Id);
            if (model.Item == null)
                model.Item = new WorkModel()
                {
                    Date = DateTime.Now,
                    UserId = model.Account.id,
                    Main = true
                };
            model.Users = new SelectList(_cmsRepository.getUsersList(), "link", "text", model.Item.UserId.ToString());
            model.WorkTypes = new SelectList(_cmsRepository.getWorksTypes(), "value", "text", "text");

            return View("Item", model);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Item(Guid Id, WorksViewModel bindData, HttpPostedFileBase posted_Preview, HttpPostedFileBase posted_Video, HttpPostedFileBase posted_Audio, IEnumerable<HttpPostedFileBase> gallery)
        {
            bindData.Item.Id = Id;

            string savePath = Settings.ContentDir + "/" + model.Account.PageName + "/work-" + Id + "/";


            ErrorMassege userMessage = new ErrorMassege();
            userMessage.title = "Информация";

            if (ModelState.IsValid)
            {
                if (!Directory.Exists(Server.MapPath(savePath))) { Directory.CreateDirectory(Server.MapPath(savePath)); }

                #region Аудиозапись
                if (posted_Audio != null && posted_Audio.ContentLength > 0)
                {
                    string fileExtension = posted_Audio.FileName.Substring(posted_Audio.FileName.LastIndexOf(".")).ToLower();

                    var validExtension = "mp3".Split(',');
                    if (validExtension.Contains(fileExtension.Replace(".", "")))
                    {
                        posted_Audio.SaveAs(Server.MapPath(savePath + posted_Audio.FileName));
                        bindData.Item.Audio = savePath + posted_Audio.FileName;
                    }
                }
                #endregion
                #region Видео
                if (posted_Video != null && posted_Video.ContentLength > 0)
                {
                    string fileExtension = posted_Video.FileName.Substring(posted_Video.FileName.LastIndexOf(".")).ToLower();

                    var validExtension = "mp4".Split(',');
                    if (validExtension.Contains(fileExtension.Replace(".", "")))
                    {
                        posted_Video.SaveAs(Server.MapPath(savePath + posted_Video.FileName));
                        bindData.Item.Video = savePath + posted_Video.FileName;
                    }
                }
                #endregion
                #region Ссылка
                if (!String.IsNullOrEmpty(bindData.Item.Url))
                {
                    string VideoId = String.Empty;
                    WebClient client = new WebClient();
                    client.Encoding = Encoding.UTF8;

                    if (bindData.Item.Url.IndexOf("rutube") > -1)
                    {
                        //https://rutube.ru/video/c7895effdccd3b74e9e5942f017d73bf/?pl_id=1721&pl_type=source
                        VideoId = bindData.Item.Url.Substring(0, bindData.Item.Url.LastIndexOf("/"));
                        VideoId = VideoId.Substring(VideoId.LastIndexOf("/") + 1);

                        string json = client.DownloadString("http://rutube.ru/api/video/" + VideoId + "/?format=json");
                        Admin.Models.Rutube.RutubeVideo VideoInfo = JsonConvert.DeserializeObject<Admin.Models.Rutube.RutubeVideo>(json);

                        bindData.Item.Url = VideoInfo.embed_url;
                        bindData.Item.Preview = VideoInfo.thumbnail_url;
                    }
                    else if (bindData.Item.Url.IndexOf("youtube") > -1)
                    {
                        if (bindData.Item.Url.IndexOf("watch") > -1)
                        {
                            VideoId = bindData.Item.Url.Substring(bindData.Item.Url.IndexOf("?v=") + 3);
                            VideoId = (VideoId.IndexOf("&") > -1) ? VideoId.Substring(0, VideoId.IndexOf("&")) : VideoId;
                        }
                        else
                        {
                            VideoId = bindData.Item.Url.Substring(bindData.Item.Url.LastIndexOf("/") + 1);
                        }

                        bindData.Item.Url = "https://www.youtube.com/embed/" + VideoId+ "?enablejsapi=1";
                        bindData.Item.Preview = "https://img.youtube.com/vi/" + VideoId + "/0.jpg";
                    }
                    else if (bindData.Item.Url.IndexOf("vimeo") > -1)
                    {
                        VideoId = bindData.Item.Url.Substring(bindData.Item.Url.LastIndexOf("/") + 1);
                        VideoId = (VideoId.IndexOf("?") > -1) ? VideoId.Substring(0, VideoId.IndexOf("?")) : VideoId;

                        string json = client.DownloadString("http://vimeo.com/api/v2/video/" + VideoId + ".json");
                        json = json.Substring(1, json.Length - 2);
                        Admin.Models.Vimeo.VimeoVideo VideoInfo = JsonConvert.DeserializeObject<Admin.Models.Vimeo.VimeoVideo>(json);

                        bindData.Item.Url = "https://player.vimeo.com/video/" + VideoId;
                        bindData.Item.Preview = VideoInfo.thumbnail_medium.Replace("200x150", "_480x360");
                    }
                    //else if (newItem.Url.IndexOf("youtube") > -1)
                    //{
                    //    /// https://vk.com/video-133976114_456239103
                    //    //< iframe src = "//vk.com/video_ext.php?oid=-133976114&id=456239103&hash=f0dbc32e33bfb8a2&hd=2" width = "853" height = "480" frameborder = "0" allowfullscreen ></ iframe >
                    //}

                }
                #endregion
                #region Сохраняем картинку
                if (posted_Preview != null && posted_Preview.ContentLength > 0)
                {
                    string fileExtension = posted_Preview.FileName.Substring(posted_Preview.FileName.LastIndexOf(".")).ToLower();

                    var validExtension = "jpg,jpeg,png,gif".Split(',');
                    if (validExtension.Contains(fileExtension.Replace(".", "")))
                    {
                        ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                        EncoderParameters myEncoderParameters = new EncoderParameters(1);
                        myEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                        Bitmap _File = (Bitmap)Bitmap.FromStream(posted_Preview.InputStream);

                        string orientation = string.Empty;

                        _File = Imaging.Resize(_File, 830, 465, "top", "center");

                        string filePath = Server.MapPath(savePath + "Preview" + fileExtension);

                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);
                        _File.Save(filePath, myImageCodecInfo, myEncoderParameters);
                        _File.Dispose();

                        bindData.Item.Preview = savePath + "Preview" + fileExtension;
                    }
                }
                #endregion

                bool res = _cmsRepository.creditWork(bindData.Item, model.Account.id, Request.ServerVariables["REMOTE_ADDR"]);

                #region Фотогалерея
                if (!Directory.Exists(Server.MapPath(savePath))) { Directory.CreateDirectory(Server.MapPath(savePath)); }

                int photo_num = 1;
                if (gallery != null) { 
                    foreach (HttpPostedFileBase _file in gallery)
                    {
                        if (_file != null)
                        {
                            using (Bitmap originalPic = new Bitmap(_file.InputStream, false))
                            {
                                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                myEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 60L);

                                Bitmap newImg = Imaging.Resize(originalPic, 1500, 1000);
                                newImg.Save(Server.MapPath(savePath + _file.FileName), myImageCodecInfo, myEncoderParameters);
                                Bitmap prewImg = Imaging.Resize(originalPic, 200, 200, "top", "center");
                                prewImg.Save(Server.MapPath(savePath + _file.FileName.Replace(".", "_preview.")), myImageCodecInfo, myEncoderParameters);

                                PhotoModel _photo = new PhotoModel
                                {
                                    Id = Guid.NewGuid(),
                                    WorkId = Id,
                                    Url = savePath + _file.FileName,
                                    Preview = savePath + _file.FileName.Replace(".", "_preview."),
                                    sort = photo_num
                                };

                                _cmsRepository.addPhoto(_photo);

                                photo_num++;
                            }
                        }


                    }
                }
                #endregion

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

            model.Item = _cmsRepository.getWork(Id);
            model.Users = new SelectList(_cmsRepository.getUsersList(), "link", "text", bindData.Item.UserId.ToString());
            model.WorkTypes = new SelectList(_cmsRepository.getWorksTypes(), "value", "text", "text");

            return View("Item", model);
        }

        /// <summary>
        /// Формируем строку фильтра
        /// </summary>
        /// <param name="title_serch">Поиск по названию</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(string searchtext, string person, string size, DateTime? date, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = addFiltrParam(query, "searchtext", searchtext);
            query = (person == null) ? addFiltrParam(query, "person", "") : addFiltrParam(query, "person", person.ToLower());
            query = (date == null) ? addFiltrParam(query, "date", String.Empty) : addFiltrParam(query, "date", ((DateTime)date).ToString("dd.MM.yyyy").ToLower());
            query = (dateend == null) ? addFiltrParam(query, "dateend", String.Empty) : addFiltrParam(query, "dateend", ((DateTime)dateend).ToString("dd.MM.yyyy").ToString().ToLower());
            query = addFiltrParam(query, "page", String.Empty);
            query = addFiltrParam(query, "size", size);

            return Redirect(StartUrl + query);
        }

        [HttpPost]
        public ActionResult Delete(Guid Id)
        {
            var res = _cmsRepository.deleteWork(Id, model.Account.id, Request.ServerVariables["REMOTE_ADDR"]);

            return Redirect(StartUrl + Request.Url.Query);
        }
    }
}