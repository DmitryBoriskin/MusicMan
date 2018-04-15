using cms.dbase;
using cms.dbModel.entity;
using MusicMan.Models;
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
using System.Web.Security;

namespace MusicMan.Controllers
{
    public class PeoplesController : CoreController
    {

        AccountViewModel model;
        FilterParams filter;
        protected int maxLoginError = 5;
        int page_size = 100;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new AccountViewModel()
            {
                Account = AccountInfo,
                Settings = ContactsInfo
            };

            #region Метатеги
            ViewBag.Title = "Авторизация";
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }
        
        /// <summary>
        /// Страница списка пользователей
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public ActionResult Index(string group)
        {
            UserViewModel model = new UserViewModel()
            {
                Account = AccountInfo,
                Settings = ContactsInfo
            };
            // Наполняем фильтр значениями
            filter = getFilter(page_size);
            filter.Group = String.IsNullOrEmpty(group) ? null : group;

            model.List = db.getUsersList(filter);
            model.CategoryList = db.getUsersGroupList();

            #region Метатеги
            ViewBag.Title = "Все пользователи";
            try
            {
                var data = model.CategoryList.Where(w => w.value == group);

                if (data != null)
                {
                    ViewBag.Title = data.First().text;
                    data.First().selected = " active";
                }
            }
            catch { }
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion

            return View(model);
        }

        public ActionResult Item(string UserId)
        {
            // Наполняем фильтр значениями
            filter = getFilter(page_size);
            filter.User = UserId;

            model.Item = db.getAccount(UserId);
            try
            {
                if (!Directory.Exists(Server.MapPath("/Userfiles/" + model.Item.PageName)))
                    Directory.CreateDirectory(Server.MapPath("/Userfiles/" + model.Item.PageName));

                if (String.IsNullOrEmpty(model.Item.Photo))
                {
                    model.Item.Photo = "/Content/img/no-photo.png";
                }
                else if (!System.IO.File.Exists(Server.MapPath(model.Item.Photo)))
                {
                    model.Item.Photo = "/Content/img/no-photo.png";
                }
            }
            catch { }
            model.AccountWorks = db.getWorkList(filter);

            long Size = DirSize(new DirectoryInfo(Server.MapPath("/Userfiles/" + UserId))) / 1024 / 1024;

            ViewBag.DirSize = Size.ToString() + "Мб";

            return View(model);
        }

        public static long DirSize(DirectoryInfo d)
        {
            long Size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                Size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                Size += DirSize(di);
            }
            return (Size);
        }

        /// <summary>
        /// Возвращает Json, в котором содердится номер телефона
        /// </summary>
        /// <param name="UserId">Alias пользователя, для которого запрашивабтся данные</param>
        /// <returns>Json</returns>
        [HttpPost]
        public ActionResult getPhone(string UserId)
        {
            Response.ContentType = "application/json; charset=utf-8";

            if (!String.IsNullOrEmpty(UserId))
            {
                string phone_num = db.getAccount(UserId).Phone;
                return Json(new { Result = phone_num }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { Result = "отсутствует" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Возвращает Json, в котором содердится адрес электронной почты 
        /// </summary>
        /// <param name="UserId">Alias пользователя, для которого запрашивабтся данные</param>
        /// <returns>Json</returns>
        [HttpPost]
        public ActionResult getMail(string UserId)
        {
            Response.ContentType = "application/json; charset=utf-8";

            if (!String.IsNullOrEmpty(UserId))
            {
                string mail = db.getAccount(UserId).Mail;
                return Json(new { Result = mail }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { Result = "отсутствует" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "sand")]
        public ActionResult Save(WorkModel newItem, HttpPostedFileBase music, IEnumerable<HttpPostedFileBase> gallery)
        {
            newItem.User = model.Account.PageName;
            newItem.UserId = model.Account.id;
            newItem.Id = Guid.NewGuid();
            newItem.Date = DateTime.Now;
            newItem.Type = "text";

            string SaveUrl = Settings.ContentDir + "/" + newItem.User + "/work-" + newItem.Id + "/";
            bool result = false;

            if (!String.IsNullOrEmpty(newItem.Desc))
            {
                newItem.Desc = newItem.Desc.Replace("sex", "").Replace("xxx", "").Replace("porno", "");
                newItem.Desc = newItem.Desc.Replace("секс", "").Replace("трахает", "").Replace("трахают", "").Replace("ебет", "").Replace("ебут", "");
                newItem.Desc = newItem.Desc.Replace("минет", "").Replace("шпилит", "").Replace("сперма", "").Replace("кончает", "").Replace("кончают", "");
                newItem.Desc = newItem.Desc.Replace("насилует", "").Replace("насилуют", "").Replace("член", "").Replace("пизда", "").Replace("сука", "");
                newItem.Desc = newItem.Desc.Replace("блядь", "").Replace("проститутка", "").Replace("шалава", "").Replace("сучка", "").Replace("проститутки", "");
                newItem.Desc = newItem.Desc.Replace("пенис", "");
            }
            
            if (!Directory.Exists(Server.MapPath(SaveUrl)))
                Directory.CreateDirectory(Server.MapPath(SaveUrl));
            #region Фото            
            foreach (HttpPostedFileBase _file in gallery)
            {
                if (_file != null)
                {
                    newItem.Type = "photo";
                }
            }            
            #endregion
            #region Аудио
            if (music != null)
            {
                newItem.Type = "music";
                music.SaveAs(Server.MapPath(SaveUrl + music.FileName));
                newItem.Audio = SaveUrl + music.FileName;
            }
            #endregion
            #region Видео
            if (!String.IsNullOrEmpty(newItem.Url))
            {
                newItem.Type = "video";
                string VideoId = String.Empty;

                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;

                if (newItem.Url.IndexOf("rutube") > -1)
                {
                    //https://rutube.ru/video/c7895effdccd3b74e9e5942f017d73bf/?pl_id=1721&pl_type=source
                    VideoId = newItem.Url.Substring(0, newItem.Url.LastIndexOf("/"));
                    VideoId = VideoId.Substring(VideoId.LastIndexOf("/") + 1);

                    string json = client.DownloadString("http://rutube.ru/api/video/" + VideoId + "/?format=json");
                    MusicMan.Models.Rutube.RutubeVideo VideoInfo = JsonConvert.DeserializeObject<MusicMan.Models.Rutube.RutubeVideo>(json);

                    newItem.Url = VideoInfo.embed_url;
                    newItem.Preview = VideoInfo.thumbnail_url;
                }
                else if (newItem.Url.IndexOf("youtube") > -1)
                {
                    if (newItem.Url.IndexOf("watch") > -1)
                    {
                        VideoId = newItem.Url.Substring(newItem.Url.IndexOf("?v=") + 3);
                        VideoId = (VideoId.IndexOf("&") > -1) ? VideoId.Substring(0, VideoId.IndexOf("&")) : VideoId;
                    }
                    else
                    {
                        VideoId = newItem.Url.Substring(newItem.Url.LastIndexOf("/") + 1);
                    }

                    newItem.Url = "https://www.youtube.com/embed/" + VideoId+ "?enablejsapi=1";
                    newItem.Preview = "https://img.youtube.com/vi/" + VideoId + "/0.jpg";
                }
                else if (newItem.Url.IndexOf("vimeo") > -1)
                {
                    VideoId = newItem.Url.Substring(newItem.Url.LastIndexOf("/") + 1);
                    VideoId = (VideoId.IndexOf("?") > -1) ? VideoId.Substring(0, VideoId.IndexOf("?")) : VideoId;

                    string json = client.DownloadString("http://vimeo.com/api/v2/video/" + VideoId + ".json");
                    json = json.Substring(1, json.Length - 2);
                    MusicMan.Models.Vimeo.VimeoVideo VideoInfo = JsonConvert.DeserializeObject<MusicMan.Models.Vimeo.VimeoVideo>(json);

                    newItem.Url = "https://player.vimeo.com/video/" + VideoId;
                    newItem.Preview = VideoInfo.thumbnail_medium.Replace("200x150", "_480x360");
                }
                //else if (newItem.Url.IndexOf("youtube") > -1)
                //{
                //    /// https://vk.com/video-133976114_456239103
                //    //< iframe src = "//vk.com/video_ext.php?oid=-133976114&id=456239103&hash=f0dbc32e33bfb8a2&hd=2" width = "853" height = "480" frameborder = "0" allowfullscreen ></ iframe >
                //}

            }
            #endregion

            result = db.createWork(newItem, Request.ServerVariables["REMOTE_ADDR"]);
            #region Галерея
            int photo_num = 1;
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
                        newImg.Save(Server.MapPath(SaveUrl + _file.FileName), myImageCodecInfo, myEncoderParameters);
                        Bitmap prewImg = Imaging.Resize(originalPic, 200, 200, "top", "center");
                        prewImg.Save(Server.MapPath(SaveUrl + _file.FileName.Replace(".", "_preview.")), myImageCodecInfo, myEncoderParameters);

                        PhotoModel _photo = new PhotoModel
                        {
                            Id = Guid.NewGuid(),
                            WorkId = newItem.Id,
                            Url = SaveUrl + _file.FileName,
                            Preview = SaveUrl + _file.FileName.Replace(".", "_preview."),
                            sort = photo_num
                        };

                        db.addPhoto(_photo);

                        photo_num++;
                    }
                }


            }
            #endregion
            
            if (result) Response.Redirect("/" + model.Account.PageName);

            // Наполняем фильтр значениями
            filter = getFilter(page_size);
            filter.User = newItem.User;
            
            model.Item = db.getAccount(newItem.User);
            model.AccountWorks = db.getWorkList(filter);
            
            return View(model);
        }
        

        public ActionResult music(string UserId)
        {
            ViewBag.Title = "Музыка";

            // Наполняем фильтр значениями
            filter = getFilter(page_size);
            filter.User = UserId;
            filter.Type = "music";

            model.Item = db.getAccount(UserId);
            model.AccountWorks = db.getWorkList(filter);
            
            return View("works", model);
        }

        public ActionResult video(string UserId)
        {
            ViewBag.Title = "Видеозаписи";

            // Наполняем фильтр значениями
            filter = getFilter(page_size);
            filter.User = UserId;
            filter.Type = "video";

            model.Item = db.getAccount(UserId);
            model.AccountWorks = db.getWorkList(filter);

            return View("works", model);
        }

        public ActionResult photo(string UserId)
        {
            ViewBag.Title = "Фотоальбомы";

            // Наполняем фильтр значениями
            filter = getFilter(page_size);
            filter.User = UserId;
            filter.Type = "photo";

            model.Item = db.getAccount(UserId);
            model.AccountWorks = db.getWorkList(filter);

            return View("works", model);
        }
        
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            foreach (var enc in ImageCodecInfo.GetImageEncoders())
                if (enc.MimeType.ToLower() == mimeType.ToLower())
                    return enc;
            return null;
        }
    }
}