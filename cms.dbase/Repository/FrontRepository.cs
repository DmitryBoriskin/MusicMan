using cms.dbase.models;
using cms.dbModel;
using cms.dbModel.entity;
using LinqToDB;
using System;
using System.Linq;

namespace cms.dbase
{
    public class FrontRepository : abstract_FrontRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;
        /// <summary>
        /// Конструктор
        /// </summary>
        public FrontRepository()
        {
            _context = "defaultConnection";
        }
        public FrontRepository(string ConnectionString)
        {
            _context = ConnectionString;
        }
        
        /// <summary>
        /// Логирование действий пользователя
        /// </summary>
        /// <param name="UserId">ID пользователя</param>
        /// <param name="IP">IP адрес пользователя</param>
        /// <param name="Action">Выполняемое действие (из справочника)</param>
        /// <param name="PageId">ID Изменяемой страницы</param>
        /// <param name="Desc">Описание изменений</param>
        public override void insertLog(Guid UserId, string IP, string Action, Guid PageId, string Desc)
        {
            using (var db = new mmBase(_context))
            {
                db.logs.Insert(() => new log
                {
                    d_date = DateTime.Now,
                    f_page = PageId,
                    c_desc = Desc,
                    f_user = UserId,
                    c_ip = IP,
                    f_action = Action
                });
            }
        }

        /// <summary>
        /// Получаем данные об пользователе по email
        /// </summary>
        /// <param name="Id">ID пользователя</param>
        /// <returns></returns>
        public override AccountModel getAccount(Guid Id)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.userss.
                    Where(w => w.id == Id && w.b_disabled == false && w.n_error_count < 5).
                    Select(s => new AccountModel
                    {
                        id = s.id,
                        PageName = s.c_page_name,
                        LastName = s.c_last_name,
                        Name = s.c_first_name,
                        Mail = s.c_email,
                        Salt = s.c_salt,
                        Hash = s.c_hash,
                        Group = s.f_group,
                        Category = getAccountcategory(s.id),
                        Photo = s.c_photo,
                        Phone = s.c_phone,
                        Description = s.c_info,
                        vkId = s.c_vk_id,
                        fbId = s.c_fb_id
                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        /// <summary>
        /// Получаем данные об пользователе по Логину (PageName или EMail)
        /// </summary>
        /// <param name="Login">Login пользователя</param>
        /// <returns></returns>
        public override AccountModel getAccount(string Login)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.userss.
                    Where(w => w.c_email == Login || w.c_page_name == Login || w.c_fb_id == Login || w.c_vk_id == Login).
                    Select(s => new AccountModel
                    {
                        id = s.id,
                        PageName = s.c_page_name,
                        Name = s.c_first_name,
                        LastName = s.c_last_name,
                        Mail = s.c_email,
                        Phone = s.c_phone,
                        Salt = s.c_salt,
                        Hash = s.c_hash,
                        Group = s.f_group,
                        Category = getAccountcategory(s.id),
                        Photo = s.c_photo,
                        Description = s.c_info,
                        CountError = (s.n_error_count >= 5),
                        LockDate = s.d_try_login,
                        Disabled = s.b_disabled
                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override AccountModel getAccount(string Login, Guid id)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.userss.
                    Where(w => (w.c_email == Login || w.c_page_name == Login) && w.id != id).
                    Select(s => new AccountModel
                    {
                        id = s.id,
                        PageName = s.c_page_name,
                        Name = s.c_first_name,
                        LastName = s.c_last_name,
                        Mail = s.c_email,
                        Phone = s.c_phone,
                        Salt = s.c_salt,
                        Hash = s.c_hash,
                        Group = s.f_group,
                        Category = getAccountcategory(s.id),
                        Photo = s.c_photo,
                        Description = s.c_info,
                        CountError = (s.n_error_count >= 5),
                        LockDate = s.d_try_login,
                        Disabled = s.b_disabled
                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }

        /// <summary>
        /// Получим группы меню для элемента карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns></returns>
        public override string[] getAccountcategory(Guid id)
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
        /// <summary>
        /// Получим группы меню для элемента карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns></returns>
        public override string[] getAccountcategoryName(Guid id)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.users_category_links.
                    Where(w => w.f_user_id == id).
                    Select(sel => sel.categorylinkusers.c_title);

                if (!data.Any())
                    return null;
                else
                    return data.ToArray();
            }
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="IP"></param>
        /// <returns></returns>
        public override bool createAccount(AccountModel Item, string IP)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.userss.Where(w => w.id == Item.id);
                if (!data.Any())
                {
                    db.userss
                    .Value(p => p.id, Item.id)
                    .Value(p => p.c_page_name, Item.PageName)
                    .Value(p => p.c_first_name, Item.Name)
                    .Value(p => p.c_last_name, Item.LastName)
                    .Value(p => p.f_group, Item.Group)
                    .Value(p => p.c_email, Item.Mail)
                    .Value(p => p.c_salt, Item.Salt)
                    .Value(p => p.c_hash, Item.Hash)
                    .Value(p => p.b_disabled, Item.Disabled)
                    .Value(p => p.c_vk_id, Item.vkId)
                    .Value(p => p.c_fb_id, Item.fbId)
                    .Value(p => p.f_modify_user_id, Item.id)
                    .Value(p => p.c_modify_user_ip, IP)
                   .Insert();

                    db.users_category_links.Where(w => w.f_user_id == Item.id).Delete();
                    foreach (string CategoryId in Item.Category)
                    {
                        db.users_category_links
                            .Value(p => p.f_user_id, Item.id)
                            .Value(p => p.f_category_id, CategoryId)
                            .Insert();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Обновление данных пользователя
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="IP"></param>
        /// <returns></returns>
        public override bool updateAccount(AccountModel Item, string IP)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.userss.Where(w => w.id == Item.id);
                if (data.Any())
                {
                    db.userss.Where(w => w.id == Item.id)
                        .Set(p => p.c_photo, Item.Photo)
                        .Set(p => p.c_page_name, Item.PageName)
                        .Set(p => p.c_first_name, Item.Name)
                        .Set(p => p.c_last_name, Item.LastName)
                        .Set(p => p.c_email, Item.Mail)
                        .Set(p => p.c_phone, Item.Phone)
                        .Set(p => p.c_info, Item.Description)
                        .Update();

                    db.users_category_links.Where(w => w.f_user_id == Item.id).Delete();
                    foreach (string CategoryId in Item.Category)
                    {
                        db.users_category_links
                            .Value(p => p.f_user_id, Item.id)
                            .Value(p => p.f_category_id, CategoryId)
                            .Insert();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Подключение профиля VK
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="IP"></param>
        /// <returns></returns>
        public override bool setAccountVK(AccountModel Item, string IP)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.userss.Where(w => w.id == Item.id);
                if (data.Any())
                {
                    db.userss.Where(w => w.id == Item.id)
                        .Set(p => p.c_vk_id, Item.vkId)
                        .Set(p => p.d_modify_date, DateTime.Now)
                        .Set(p => p.c_modify_user_ip, IP)
                        .Set(p => p.f_modify_user_id, Item.id)
                        .Update();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Подключение профиля Facebook
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="IP"></param>
        /// <returns></returns>
        public override bool setAccountFacebook(AccountModel Item, string IP)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.userss.Where(w => w.id == Item.id);
                if (data.Any())
                {
                    db.userss.Where(w => w.id == Item.id)
                        .Set(p => p.c_fb_id, Item.fbId)
                        .Set(p => p.d_modify_date, DateTime.Now)
                        .Set(p => p.c_modify_user_ip, IP)
                        .Set(p => p.f_modify_user_id, Item.id)
                        .Update();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// При удачной авторизации обнуляем попытки входа и добавляем запись в лог
        /// </summary>
        /// <param name="id">ID пользователя</param>
        /// <param name="IP">IP адрес пользователя</param>
        public override void SuccessLogin(Guid id, string IP)
        {
            using (var db = new mmBase(_context))
            {
                Guid? change_pass_code = null;

                var data = db.userss.Where(w => w.id == id)
                        .Set(u => u.n_error_count, 0)
                        .Set(u => u.d_try_login, DateTime.Now)
                        .Set(u => u.с_change_pass_code, change_pass_code)
                        .Update();

                // Логирование
                insertLog(id, IP, "login", id, "Авторизация в CMS");
            }
        }
        /// <summary>
        /// Записываем неудачную попытку входа
        /// </summary>
        /// <param name="id">ID пользователя</param>
        /// <param name="IP">IP адрес пользователя</param>
        public override int FailedLogin(Guid id, string IP)
        {
            using (var db = new mmBase(_context))
            {
                int Num = db.userss.Where(w => w.id == id).ToArray().First().n_error_count + 1;

                var data = db.userss.Where(w => w.id == id)
                        .Set(u => u.n_error_count, Num)
                        .Set(u => u.d_try_login, DateTime.Now)
                        .Update();

                return Num;
            }
        }
        /// <summary>
        /// записываем код востановления пароля
        /// </summary>
        /// <param name="id">ID пользователя</param>
        /// <param name="Code">Код восстановления</param>
        /// <param name="IP">IP адрес пользователя</param>
        public override void setRestorePassCode(Guid id, Guid Code, string IP)
        {
            using (var db = new mmBase(_context))
            {
                DateTime? ErrorDate = null;

                var data = db.userss.Where(w => w.id == id)
                    .Set(u => u.с_change_pass_code, Code)
                    .Update();
            }
        }
        /// <summary>
        /// Проверка существования пользователя
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public override bool ConfirmMail(Guid Code)
        {
            using (var db = new mmBase(_context))
            {
                bool result = false;

                int count = db.userss.Where(w => w.id == Code).Count();
                if (count > 0)
                {
                    var data = db.userss.Where(w => w.id == Code)
                        .Set(u => u.b_disabled, false)
                        .Update();

                    result = true;
                }

                return result;
            }
        }
        /// <summary>
        /// Смена пароля
        /// </summary>
        /// <param name="id">id аккаунта</param>
        /// <param name="NewSalt">открытый ключ</param>
        /// <param name="NewHash">закрытый ключ</param>
        /// <returns></returns>
        public override void changePasswordUser(Guid id, string NewSalt, string NewHash, string IP)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.userss.Where(w => w.id == id)
                    .Set(u => u.c_salt, NewSalt)
                    .Set(u => u.c_hash, NewHash)
                    .Update();
            }
        }

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

                if (query.Any())
                {
                    int ItemCount = query.Count();

                    var List = query.
                        Select(s => new WorkModel
                        {
                            Id = s.id,
                            Type = s.f_type,
                            User = s.f_user,
                            UserName = s.c_first_name + " "+ s.c_user_name,
                            UserPhoto = s.f_user_photo,
                            Title = s.c_title,
                            Desc = s.c_desc,
                            Info = s.c_info,
                            Preview = s.c_preveiw,
                            Url = s.c_url,
                            Video = s.c_video,
                            Audio = s.c_audio,
                            Date = s.d_date,
                            Photoalbom = getPhotoAlbom(s.id)
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
                return null;
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
                            Preview = s.c_preveiw,
                            Url = s.c_url,
                            Date = s.d_date,
                            Photoalbom = getPhotoAlbom(s.id)
                        }).
                        FirstOrDefault();

                    return data;
                }
                return null;
            }
        }
        public override bool createWork(WorkModel workItem, string IP)
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
                    .Value(p => p.c_preveiw, workItem.Preview)
                    .Value(p => p.c_url, workItem.Url)
                    .Value(p => p.c_audio, workItem.Audio)
                    .Value(p => p.c_video, workItem.Video)
                    .Value(p => p.f_user, workItem.UserId)
                    .Value(p => p.d_date, workItem.Date)
                    .Value(p => p.f_type, workItem.Type)
                    .Value(p => p.f_modify_user_id, workItem.UserId)
                    .Value(p => p.c_modify_user_ip, IP)
                    .Value(p => p.d_modify_date, DateTime.Now)
                   .Insert();
                    
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override bool deleteWork(Guid Id, string IP)
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

        public override bool checkLike(Guid UserId, Guid WorkId) {
            try
            {
                using (var db = new mmBase(_context))
                {
                    int Count = db.works_likes.Where(p => p.f_user_id == UserId && p.f_work_id == WorkId).Count();

                    if (Count > 0) return true;
                    else return false;
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                return false;
            }
        }
        public override int getLikes(Guid WorkId)
        {
            using (var db = new mmBase(_context))
            {
                int Count = db.works_likes.Where(p => p.f_work_id == WorkId).Count();

                return Count;
            }
        }
        public override int setLike(Guid UserId, Guid WorkId) {
            try
            {
                using (var db = new mmBase(_context))
                {
                    int Count = db.works_likes.Where(p => p.f_user_id == UserId && p.f_work_id == WorkId).Count();

                    if (Count > 0)
                    {
                        works_like data = db.works_likes.Where(p => p.f_user_id == UserId && p.f_work_id == WorkId).FirstOrDefault();
                        using (var tran = db.BeginTransaction())
                        {
                            db.Delete(data);
                            tran.Commit();
                        }
                    }
                    else
                    {
                        db.works_likes
                            .Value(p => p.f_user_id, UserId)
                            .Value(p => p.f_work_id, WorkId)
                            .Insert();
                    }

                    Count = db.works_likes.Where(p => p.f_work_id == WorkId).Count();

                    return Count;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        
        public override BannerModel[] getSlider()
        {
            using (var db = new mmBase(_context))
            {
                var data = db.sliders.
                    Where(w => w.b_disabled == false && w.d_date < DateTime.Now && (w.d_date_end == null || w.d_date_end > DateTime.Now)).
                    OrderBy(o => o.n_sort).
                    Select(s => new BannerModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Link = s.c_link,
                        Target = s.b_target,
                        Image = s.c_image_url
                    });
                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }

        /// <summary>
        /// Получаем список баннеров в данной секции
        /// </summary>
        /// <param name="sectionId">ID секции</param>
        /// <returns></returns>
        public override BannerModel[] getBannersList(string sectionId)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.bannerss.
                    Where(w => w.b_disabled == false && w.d_date < DateTime.Now && (w.d_date_end == null || w.d_date_end > DateTime.Now)).
                    OrderBy(o => o.n_sort).
                    Select(s => new BannerModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Link = s.c_link,
                        Target = s.b_target,
                        Image = s.c_image_url
                    });
                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }

        public override MaterialsList getMaterialsList(int Size)
        {
            using (var db = new mmBase(_context))
            {
                var query = db.sv_workss.Where(w => w.b_main == true && w.f_type == "text" && w.d_date < DateTime.Now);
                query = query.OrderByDescending(o => o.d_date);

                if (query.Any())
                {
                    int ItemCount = query.Count();

                    var List = query
                        .Select(s => new MaterialsModel
                        {
                            Id = s.id,
                            Title = s.c_title,
                            Preview = s.c_preveiw,
                            Text = s.c_desc,
                            Url = s.f_user,
                            Date = s.d_date,
                            Desc = s.c_desc
                        }).
                        Take(Size);

                    MaterialsModel[] materialsInfo = List.ToArray();

                    return new MaterialsList
                    {
                        Data = materialsInfo
                    };
                }
                return null;
            }
        }

        public override MaterialsList getMaterialsList(FilterParams filtr)
        {
            using (var db = new mmBase(_context))
            {
                var query = db.sv_workss.Where(w => w.b_main == true && w.f_type == "text" && w.d_date < DateTime.Now);
                query = query.OrderByDescending(o => o.d_date);

                if (query.Any())
                {
                    int ItemCount = query.Count();

                    var List = query
                        .Select(s => new MaterialsModel
                        {
                            Id = s.id,
                            Title = s.c_title,
                            Preview = s.c_preveiw,
                            Text = s.c_desc,
                            Url = s.f_user,
                            Date = s.d_date
                        }).
                        Skip(filtr.Size * (filtr.Page - 1)).
                        Take(filtr.Size);

                    MaterialsModel[] materialsInfo = List.ToArray();

                    return new MaterialsList
                    {
                        Data = materialsInfo,
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


        /// <summary>
        /// Users
        /// </summary>
        /// <returns></returns>
        public override Catalog_list[] getUsersGroupList()
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

        public override UsersList getUsersList(FilterParams filtr)
        {
            using (var db = new mmBase(_context))
            {
                var query = db.sv_userss.Where(w => w.b_disabled == false);
                if (filtr.Group != String.Empty)
                {
                    query = query.Where(w => w.f_category == filtr.Group);
                }
                foreach (string param in filtr.SearchText.Split(' '))
                {
                    if (param != String.Empty)
                    {
                        query = query.Where(w => w.c_last_name.Contains(param) || w.c_email.Contains(param));
                    }
                }
                query = query.OrderByDescending(o => new { o.d_create_date });

                if (query.Any())
                {
                    int ItemCount = query.Count();

                    var List = query.
                        Select(s => new User
                        {
                            Id = s.id,
                            Group = s.f_group,
                            PageName = s.c_page_name,
                            Name = s.c_first_name,
                            Surname = s.c_last_name,
                            EMail = s.c_email,
                            Photo = s.c_photo,
                            Category = getAccountcategoryName(s.id),
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override SettingsModel getSettings()
        {
            using (var db = new mmBase(_context))
            {
                var data = db.settingss
                    .Where(w => w.id != null)
                    .Select(s => new SettingsModel {
                        Info = s.c_info,
                        Contacts = s.c_contacts,
                        Regulations = s.c_regulations,
                        Advertising = s.c_advertising,
                        Helping = s.c_helping,
                        Marquee = s.c_marquee
                    });

                if (!data.Any()) { return null; }
                else { return data.FirstOrDefault(); }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override SettingsModel getContacts()
        {
            using (var db = new mmBase(_context))
            {
                var data = db.settingss
                    .Where(w => w.id != null)
                    .Select(s => new SettingsModel {
                        Phone = s.c_phone,
                        Mail = s.c_mail_to,

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
                else { return data.FirstOrDefault(); }
            }
        }
    }
}
