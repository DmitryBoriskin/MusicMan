using cms.dbase.models;
using cms.dbModel;
using cms.dbModel.entity;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cms.dbase
{
    public partial class cmsRepository : abstract_cmsRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;
        /// <summary>
        /// Конструктор
        /// </summary>
        public cmsRepository()
        {
            _context = "defaultConnection";
        }
        public cmsRepository(string ConnectionString)
        {
            _context = ConnectionString;
        }

        /// <summary>
        /// Определяем драва доступа
        /// </summary>
        /// <returns></returns>
        public override bool DomainSecurity(string Domain)
        {
            using (var db = new mmBase(_context))
            {
                bool result = false;

                int count = db.settingss.Where(w => w.c_domain == Domain).Count();
                if (count > 0 || Domain.ToLower() == "localhost") result = true;

                return result;
            }
        }


        #region Settings
        public override SettingsModel getSettings()
        {
            using (var db = new mmBase(_context))
            {
                var data = db.settingss.
                    Select(s => new SettingsModel
                    {
                        Guid = s.id,
                        Domain = s.c_domain,
                        Title = s.c_title,
                        Marquee = s.c_marquee,
                        Info = s.c_info,
                        Contacts = s.c_contacts,
                        Regulations = s.c_regulations,
                        Advertising = s.c_advertising,
                        Helping = s.c_helping,
                        MailServer = s.c_mail_server,
                        MailPort = s.c_mail_port,
                        SSL = (bool)s.b_mail_ssl,
                        MailFrom = s.c_mail_from,
                        MailFromAlias = s.c_mail_from_alias,
                        MailPass = s.c_mail_pass,
                        MailTo = s.c_mail_to,

                        vk = s.c_vk,
                        facebook = s.c_facebook,
                        instagram = s.c_instagram,
                        youtube = s.c_youtube,

                        telegram = s.c_telegram,
                        viber = s.c_viber,
                        whatsapp = s.c_whatsapp,
                        skype = s.c_skype
                    });


                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override bool updateSettings(SettingsModel Item, Guid UserId, string IP)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.settingss
                    .Where(w => w.c_domain != null)
                    .Set(p => p.c_domain, Item.Domain)
                    .Set(p => p.c_title, Item.Title)
                    .Set(p => p.c_marquee, Item.Marquee)
                    .Set(p => p.c_info, Item.Info)
                    .Set(p => p.c_contacts, Item.Contacts)
                    .Set(p => p.c_regulations, Item.Regulations)
                    .Set(p => p.c_advertising, Item.Advertising)
                    .Set(p => p.c_helping, Item.Helping)
                    .Set(p => p.c_mail_server, Item.MailServer)
                    .Set(p => p.c_mail_port, Item.MailPort)
                    .Set(p => p.b_mail_ssl, Item.SSL)
                    .Set(p => p.c_mail_from, Item.MailFrom)
                    .Set(p => p.c_mail_from_alias, Item.MailFromAlias)
                    .Set(p => p.c_mail_to, Item.MailTo)

                    .Set(p => p.c_phone, Item.Phone)
                    .Set(p => p.c_vk, Item.vk)
                    .Set(p => p.c_facebook, Item.facebook)
                    .Set(p => p.c_instagram, Item.instagram)
                    .Set(p => p.c_youtube, Item.youtube)
                    .Set(p => p.c_telegram, Item.telegram)
                    .Set(p => p.c_viber, Item.viber)
                    .Set(p => p.c_whatsapp, Item.whatsapp)
                    .Set(p => p.c_skype, Item.skype)
                    .Update();

                if (!String.IsNullOrEmpty(Item.MailPass))
                {
                    var data2 = db.settingss
                    .Where(w => w.c_domain == Item.Domain)
                    .Set(p => p.c_mail_pass, Item.MailPass)
                    .Update();
                }
                return true;
            }
        }
        #endregion

        public override statModel[] getStatistic(int DayCount) {
            using (var db = new mmBase(_context))
            {
                var query = db.getStat(DayCount)
                    .Select(s => new statModel
                    {
                        Date = (DateTime)s.D_Date,
                        AllUsers = (int)s.AllUsers,
                        UsersVk = (int)s.Users_vk,
                        UsersFb = (int)s.Users_fb,
                        Works = (int)s.works
                    });

                if (query.Any())
                    return query.ToArray();
                else
                    return null;
            }
        }

        //#region PortalUsers
        public override bool check_user(Guid id)
        {
            using (var db = new mmBase(_context))
            {
                bool result = false;

                int count = db.userss.Where(w => w.id == id).Count();
                if (count > 0) result = true;

                return result;
            }
        }
        public override bool check_user(string email)
        {
            using (var db = new mmBase(_context))
            {
                bool result = false;

                int count = db.userss.Where(w => w.c_email == email).Count();
                if (count > 0) result = true;

                return result;
            }
        }

        public override Catalog_list[] getUsersList()
        {
            using (var db = new mmBase(_context))
            {
                var query = db.userss
                    .Where(w => w.b_disabled == false)
                    .OrderBy(o => new { o.c_first_name, o.c_last_name })
                    .Select(s => new Catalog_list
                    {
                        value = s.c_page_name,
                        text = s.c_first_name + " " + s.c_last_name + "(" + s.c_page_name + ")",
                        link = s.id.ToString()
                    });

                if (query.Any())
                    return query.ToArray();
                else
                    return null;
            }
        }
        public override UsersList getUsersList(FilterParams filtr)
        {
            using (var db = new mmBase(_context))
            {
                //string[] filtr, string group, bool disabeld, int page, int size
                var query = db.userss.Where(w => w.id != null);
                if ((bool)filtr.Disabled)
                {
                    query = query.Where(w => w.b_disabled == filtr.Disabled);
                }
                if (filtr.Group != String.Empty)
                {
                    query = query.Where(w => w.f_group == filtr.Group);
                }
                foreach (string param in filtr.SearchText.Split(' '))
                {
                    if (param != String.Empty)
                    {
                        query = query.Where(w => w.c_first_name.Contains(param) || w.c_last_name.Contains(param) || w.c_email.Contains(param));
                    }
                }

                query = query.OrderBy(o => new { o.c_last_name, o.c_first_name });

                if (query.Any())
                {
                    int ItemCount = query.Count();

                    var List = query.
                        Select(s => new User
                        {
                            Id = s.id,
                            RegDate = s.d_create_date,
                            Surname = s.c_last_name,
                            Name = s.c_first_name,
                            Photo = s.c_photo,
                            EMail = s.c_email,
                            Disabled = s.b_disabled
                        }).
                        Skip(filtr.Size * (filtr.Page - 1)).
                        Take(filtr.Size);

                    User[] usersInfo = List.ToArray();

                    return new UsersList
                    {
                        Data = usersInfo,
                        Pager = new Pager
                        {
                            page = filtr.Page,
                            size = filtr.Size,
                            items_count = ItemCount,
                            page_count = (ItemCount % filtr.Size > 0) ? (ItemCount / filtr.Size) + 1 : ItemCount / filtr.Size
                        }
                    };
                }
                return null;
            }
        }
        public override User getUser(Guid id)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.userss.
                    Where(w => w.id == id).
                    Select(s => new User
                    {
                        Id = s.id,
                        vkId = s.c_vk_id,
                        fbId = s.c_fb_id,
                        PageName = s.c_page_name,
                        Surname = s.c_last_name,
                        Name = s.c_first_name,
                        EMail = s.c_email,
                        Phone = s.c_phone,
                        Photo = s.c_photo,
                        Info = s.c_info,
                        Group = s.f_group,
                        Category = getUserCtegory(s.id),
                        Disabled = s.b_disabled
                    });


                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override string[] getUserCtegory(Guid id)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.users_category_links.
                    Where(w => w.f_user_id == id).
                    Select(sel => sel.f_category_id);

                if (!data.Any())
                    return null;
                else
                    return data.ToArray();
            }
        }
        public override bool saveUser(Guid id, User Item, Guid UserId, string IP)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.userss.Where(w => w.id == id);
                if (!data.Any())
                {
                    db.userss
                    .Value(p => p.id, id)
                    .Value(p => p.c_page_name, Item.PageName)
                    .Value(p => p.c_first_name, Item.Name)
                    .Value(p => p.c_last_name, Item.Surname)
                    .Value(p => p.f_group, Item.Group)
                    .Value(p => p.c_email, Item.EMail)
                    .Value(p => p.c_salt, Item.Salt)
                    .Value(p => p.c_hash, Item.Hash)
                    .Value(p => p.b_disabled, Item.Disabled)
                    .Value(p => p.c_vk_id, Item.vkId)
                    .Value(p => p.c_fb_id, Item.fbId)
                    .Value(p => p.f_modify_user_id, UserId)
                    .Value(p => p.c_modify_user_ip, IP)
                   .Insert();

                    return true;
                }
                else
                {
                    db.userss.Where(w => w.id == Item.Id)
                        .Set(p => p.c_page_name, Item.PageName)
                        .Set(p => p.c_first_name, Item.Name)
                        .Set(p => p.c_last_name, Item.Surname)
                        .Set(p => p.c_photo, Item.Photo)
                        .Set(p => p.c_email, Item.EMail)
                        .Set(p => p.f_group, Item.Group)
                        .Set(p => p.c_phone, Item.Phone)
                        .Set(p => p.c_info, Item.Info)
                        .Set(p => p.b_disabled, Item.Disabled)
                        .Update();
                    
                    db.users_category_links.Where(w => w.f_user_id == Item.Id).Delete();
                    if (Item.Category != null)
                    { 
                        foreach (string CategoryId in Item.Category)
                        {
                            db.users_category_links
                                .Value(p => p.f_user_id, Item.Id)
                                .Value(p => p.f_category_id, CategoryId)
                                .Insert();
                        }
                    }
                    return false;
                }
            }
        }
        public override bool deleteUser(Guid id, Guid UserId, string IP)
        {
            using (var db = new mmBase(_context))
            {
                db.userss.Where(w => w.id == id).Delete();
                return true;
            }
        }

        public override void changePassword(Guid id, string Salt, string Hash, Guid UserId, string IP)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.userss.Where(w => w.id == id);

                if (data != null)
                {
                    data.Where(w => w.id == id)
                    .Set(p => p.c_salt, Salt)
                    .Set(p => p.c_hash, Hash)
                    .Update();
                }
            }
        }
        
        public override Catalog_list[] getUsersGroupList()
        {
            using (var db = new mmBase(_context))
            {
                var data = db.users_groups.
                    Select(s => new Catalog_list
                    {
                        text = s.c_title,
                        value = s.c_alias
                    });

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }
        public override Catalog_list[] getUsersCategorys()
        {
            using (var db = new mmBase(_context))
            {
                var data = db.users_categorys.
                    OrderBy(o => o.n_sort).
                    Select(s => new Catalog_list
                    {
                        text = s.c_title,
                        value = s.c_alias
                    });

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }

        #region Works

        /// <summary>
        /// Получаем список работ
        /// </summary>
        /// <param name="filtr"></param>
        /// <returns></returns>
        public override WorkList getWorkList(FilterParams filtr)
        {
            using (var db = new mmBase(_context))
            {
                var query = db.sv_workss.Where(w => w.f_user != null);

                if (!String.IsNullOrEmpty(filtr.User))
                {
                    query = query.Where(w => w.f_user == filtr.User);
                }

                if (!String.IsNullOrEmpty(filtr.Type))
                {
                    query = query.Where(w => w.f_type == filtr.Type);
                }

                if (filtr.Main != null)
                {
                    query = query.Where(w => w.b_main == filtr.Main);
                }

                foreach (string param in filtr.SearchText.Split(' '))
                {
                    if (param != String.Empty)
                    {
                        query = query.Where(w => w.c_title.Contains(param));
                    }
                }

                if (!String.IsNullOrEmpty(filtr.Sort))
                    if (filtr.Sort == "main")
                        query = query.OrderByDescending(o => new { o.b_main, o.d_date });
                    else
                        query = query.OrderByDescending(o => new { o.d_date });
                else
                    query = query.OrderByDescending(o => new { o.d_date });

                int ItemCount = query.Count();

                var List = query.
                    Select(s => new WorkModel
                    {
                        Id = s.id,
                        Type = s.f_type,
                        UserId = s.f_user_id,
                        User = s.f_user,
                        UserName = s.c_user_name + " "+ s.c_first_name,
                        UserPhoto = s.f_user_photo,
                        Title = s.c_title,
                        Desc = s.c_desc,
                        Info = s.c_info,
                        Preview = s.c_preveiw,
                        Url = s.c_url,
                        Date = s.d_date,
                        Main = s.b_main
                    }).
                    Skip(filtr.Size * (filtr.Page - 1)).
                    Take(filtr.Size);

                WorkModel[] worksInfo = List.ToArray();

                return new WorkList
                {
                    Data = worksInfo,
                    Pager = new Pager
                    {
                        page = filtr.Page,
                        size = filtr.Size,
                        items_count = ItemCount,
                        page_count = (ItemCount % filtr.Size > 0) ? (ItemCount / filtr.Size) + 1 : ItemCount / filtr.Size
                    }
                };
            }
        }
        public override WorkModel getWork(Guid Id)
        {
            using (var db = new mmBase(_context))
            {
                var query = db.workss.Where(w => w.id == Id);

                if (query.Any())
                {
                    var data = query.
                        Select(s => new WorkModel
                        {
                            Id = s.id,
                            UserId = s.f_user,
                            Type = s.f_type,
                            Title = s.c_title,
                            Desc = s.c_desc,
                            Info = s.c_info,
                            Preview = s.c_preveiw,
                            Url = s.c_url,
                            Audio = s.c_audio,
                            Video = s.c_video,
                            Code = s.c_video_code,
                            Date = s.d_date,
                            Main = s.b_main,
                            Photoalbom = getPhotoAlbom(s.id)
                        }).
                        FirstOrDefault();

                    return data;
                }
                return null;
            }
        }

        public override bool creditWork(WorkModel workItem, Guid UserId, string IP)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.workss.Where(w => w.id == workItem.Id);
                if (!data.Any())
                {
                    db.workss
                    .Value(p => p.id, workItem.Id)
                    .Value(p => p.c_title, workItem.Title)
                    .Value(p => p.c_desc, workItem.Desc)
                    .Value(p => p.c_info, workItem.Info)
                    .Value(p => p.c_preveiw, workItem.Preview)
                    .Value(p => p.c_url, workItem.Url)
                    .Value(p => p.c_audio, workItem.Audio)
                    .Value(p => p.c_video, workItem.Video)
                    .Value(p => p.c_video_code, workItem.Code)
                    .Value(p => p.f_user, UserId)
                    .Value(p => p.d_date, workItem.Date)
                    .Value(p => p.f_type, workItem.Type)
                    .Value(p => p.f_modify_user_id, UserId)
                    .Value(p => p.c_modify_user_ip, IP)
                    .Value(p => p.d_modify_date, DateTime.Now)
                    .Value(p => p.b_main, workItem.Main)
                   .Insert();

                    return true;
                }
                else
                {
                    db.workss.Where(w => w.id == workItem.Id)
                   .Set(p => p.c_title, workItem.Title)
                   .Set(p => p.c_desc, workItem.Desc)
                   .Set(p => p.c_info, workItem.Info)
                   .Set(p => p.c_preveiw, workItem.Preview)
                   .Set(p => p.c_url, workItem.Url)
                   .Set(p => p.c_audio, workItem.Audio)
                   .Set(p => p.c_video, workItem.Video)
                   .Set(p => p.c_video_code, workItem.Code)
                   .Set(p => p.f_user, workItem.UserId)
                   .Set(p => p.d_date, workItem.Date)
                   .Set(p => p.f_type, workItem.Type)
                   .Set(p => p.b_main, workItem.Main)
                   .Set(p => p.f_modify_user_id, UserId)
                   .Set(p => p.c_modify_user_ip, IP)
                   .Set(p => p.d_modify_date, DateTime.Now)
                   .Update();

                    return false;
                }
            }
        }
        public override bool deleteWork(Guid Id, Guid UserId, string IP)
        {
            try
            {
                using (var db = new mmBase(_context))
                {
                    works data = db.workss.Where(p => p.id == Id).SingleOrDefault();
                    if (data == null)
                    {
                        throw new Exception("Запись с таким Id не найдена");
                    }

                    using (var tran = db.BeginTransaction())
                    {
                        db.Delete(data);
                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                return false;
            }
        }

        public override Catalog_list[] getWorksTypes()
        {
            using (var db = new mmBase(_context))
            {
                var query = db.works_types
                    .Where(w => w.id != null)
                    .OrderBy(o => new { o.c_name })
                    .Select(s => new Catalog_list
                    {
                        value = s.id,
                        text = s.c_name
                    });

                if (query.Any())
                    return query.ToArray();
                else
                    return null;
            }
        }

        public override GaleryModel getPhotoAlbom(Guid workId)
        {
            using (var db = new mmBase(_context))
            {
                GaleryModel Model = new GaleryModel();

                var data = db.photos.
                    Where(w => w.f_work_id == workId).
                    OrderBy(o => o.n_sort).
                    Select(s => new PhotoModel
                    {
                        Id = s.id,
                        Preview = s.c_previev,
                        Url = s.c_url
                    });
                if (data.Any())
                {
                    Model.List = data.ToArray();
                }

                return Model;
            }
        }
        public override PhotoModel getPhotoItem(Guid id)
        {
            using (var db = new mmBase(_context))
            {
                var query = db.photos.Where(w => w.id == id);
                if (query.Any())
                {
                    return query.Select(s => new PhotoModel
                    {
                        Id = s.id,
                        Url = s.c_url,
                        Preview = s.c_previev
                    }).SingleOrDefault();

                }
                return null;
            }
        }
        
        public override void addPhoto(PhotoModel photoItem)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.photos.Where(w => w.id == photoItem.Id);
                if (!data.Any())
                {
                    db.photos
                    .Value(p => p.id, photoItem.Id)
                    .Value(p => p.c_previev, photoItem.Preview)
                    .Value(p => p.c_url, photoItem.Url)
                    .Value(p => p.f_work_id, photoItem.WorkId)
                    .Value(p => p.n_sort, photoItem.sort)
                   .Insert();
                }
            }
        }
        public override bool delPhotoItem(Guid id)
        {
            using (var db = new mmBase(_context))
            {
                using (var tran = db.BeginTransaction())
                {

                    var data = db.photos.Where(w => w.id == id);
                    if (data.Any())
                    {
                        Guid AlbumId = data.Single().f_work_id;
                        int ThisSort = data.Single().n_sort;
                        // удаление фотографии
                        data.Delete();
                        //корректировка порядка
                        db.photos
                            .Where(w => (w.f_work_id == AlbumId && w.n_sort > ThisSort))
                            .Set(u => u.n_sort, u => u.n_sort - 1)
                            .Update();
                        tran.Commit();
                        return true;
                    }
                    return false;
                }

            }
        }
        public override bool sortingPhotos(Guid id, int num)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.photos.Where(w => w.id == id).Select(s => new PhotoModel { WorkId = s.f_work_id, sort = s.n_sort }).First();
                var AlbumId = data.WorkId;

                if (num > data.sort)
                {
                    db.photos.Where(w => w.f_work_id == AlbumId && w.n_sort > data.sort && w.n_sort <= num)
                        .Set(p => p.n_sort, p => p.n_sort - 1)
                        .Update();
                }
                else
                {
                    db.photos.Where(w => w.f_work_id == AlbumId && w.n_sort < data.sort && w.n_sort >= num)
                        .Set(p => p.n_sort, p => p.n_sort + 1)
                        .Update();
                }
                db.photos
                    .Where(w => w.id == id)
                    .Set(s => s.n_sort, num)
                    .Update();
            }
            return true;
        }
        #endregion

        #region Slider
        public override BannerModel[] getSlider(FilterParams filtr)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.sliders.
                    Where(w => w.id !=null).
                    OrderBy(o => o.n_sort).
                    Select(s => new BannerModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Link = s.c_link,
                        Target = s.b_target,
                        Image = s.c_image_url,
                        Date = s.d_date,
                        DateEnd = s.d_date_end,
                        Disabled = s.b_disabled
                    });
                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }
        public override BannerModel getSlide(Guid id)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.sliders.
                    Where(w => w.id == id).
                    Select(s => new BannerModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Link = s.c_link,
                        Target = s.b_target,
                        Image = s.c_image_url,
                        Date = s.d_date,
                        DateEnd = s.d_date_end,
                        Disabled = s.b_disabled
                    });


                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override bool creditSlider(BannerModel Item, Guid UserId, string IP)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.sliders.Where(w => w.id == Item.Id);
                if (!data.Any())
                {
                    db.sliders
                    .Value(p => p.id, Item.Id)
                    .Value(p => p.c_title, Item.Title)
                    .Value(p => p.c_link, Item.Link)
                    .Value(p => p.c_image_url, Item.Image)
                    .Value(p => p.b_target, Item.Target)
                    .Value(p => p.d_date, Item.Date)
                    .Value(p => p.d_date_end, Item.DateEnd)
                    .Value(p => p.b_disabled, Item.Disabled)
                   .Insert();

                    return true;
                }
                else
                {
                    db.sliders.Where(w => w.id == Item.Id)
                   .Set(p => p.c_title, Item.Title)
                   .Set(p => p.c_link, Item.Link)
                   .Set(p => p.c_image_url, Item.Image)
                   .Set(p => p.b_target, Item.Target)
                   .Set(p => p.d_date, Item.Date)
                   .Set(p => p.d_date_end, Item.DateEnd)
                   .Set(p => p.b_disabled, Item.Disabled)
                   .Update();

                    return false;
                }
                return true;
            }
        }
        public override bool deleteSlide(Guid id)
        {
            using (var db = new mmBase(_context))
            {
                db.sliders.Where(w => w.id == id).Delete();
                return true;
            }
        }
        #endregion
        
        #region Banners
        public override BannerModel[] getBannersList(FilterParams filtr)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.bannerss.
                    Where(w => w.id != null).
                    OrderBy(o => o.n_sort).
                    Select(s => new BannerModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Link = s.c_link,
                        Target = s.b_target,
                        Image = s.c_image_url,
                        Date = s.d_date,
                        DateEnd = s.d_date_end,
                        Disabled = s.b_disabled
                    });
                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }
        public override BannerModel getBanner(Guid id)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.bannerss.
                    Where(w => w.id == id).
                    Select(s => new BannerModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Link = s.c_link,
                        Target = s.b_target,
                        Image = s.c_image_url,
                        Date = s.d_date,
                        DateEnd = s.d_date_end,
                        Disabled = s.b_disabled
                    });


                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override bool creditBanner(BannerModel Item, Guid UserId, string IP)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.bannerss.Where(w => w.id == Item.Id);
                if (!data.Any())
                {
                    db.bannerss
                    .Value(p => p.id, Item.Id)
                    .Value(p => p.c_title, Item.Title)
                    .Value(p => p.c_link, Item.Link)
                    .Value(p => p.c_image_url, Item.Image)
                    .Value(p => p.b_target, Item.Target)
                    .Value(p => p.d_date, Item.Date)
                    .Value(p => p.d_date_end, Item.DateEnd)
                    .Value(p => p.b_disabled, Item.Disabled)
                   .Insert();

                    return true;
                }
                else
                {
                    db.bannerss.Where(w => w.id == Item.Id)
                   .Set(p => p.c_title, Item.Title)
                   .Set(p => p.c_link, Item.Link)
                   .Set(p => p.c_image_url, Item.Image)
                   .Set(p => p.b_target, Item.Target)
                   .Set(p => p.d_date, Item.Date)
                   .Set(p => p.d_date_end, Item.DateEnd)
                   .Set(p => p.b_disabled, Item.Disabled)
                   .Update();

                    return false;
                }
                return true;
            }
        }
        public override bool deleteBanner(Guid id)
        {
            using (var db = new mmBase(_context))
            {
                db.bannerss.Where(w => w.id == id).Delete();
                return true;
            }
        }
        #endregion
    }
}
