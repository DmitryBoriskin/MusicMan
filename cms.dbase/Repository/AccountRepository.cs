using cms.dbase.models;
using cms.dbModel;
using cms.dbModel.entity;
using LinqToDB;
using System;
using System.Linq;

namespace cms.dbase
{
    public class AccountRepository : abstract_AccountRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;
        /// <summary>
        /// Конструктор
        /// </summary>
        public AccountRepository()
        {
            _context = "defaultConnection";
        }
        public AccountRepository(string ConnectionString)
        {
            _context = ConnectionString;
        }
        
        
        /// <summary>
        /// Получаем данные об пользователе по email или id
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public override AccountModel getCmsAccount(string Email)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.userss.
                    Where(w => w.c_email == Email && w.b_disabled == false && w.f_group=="admin").
                    Select(s => new AccountModel
                    {
                        id = s.id,
                        PageName = s.c_page_name,
                        Mail = s.c_email,
                        Salt = s.c_salt,
                        Hash = s.c_hash,
                        Group = s.f_group,
                        Photo = s.c_photo,
                        CountError = (s.n_error_count >= 5),
                        LockDate = s.d_try_login,
                        Disabled = s.b_disabled
                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        public override AccountModel getCmsAccount(Guid Id)
        {
            using (var db = new mmBase(_context))
            {
                var data = db.userss.
                    Where(w => w.id == Id).
                    Select(s => new AccountModel
                    {
                        id = s.id,
                        Name = s.c_first_name,
                        LastName = s.c_last_name,
                        PageName = s.c_page_name,
                        Mail = s.c_email,
                        Salt = s.c_salt,
                        Hash = s.c_hash,
                        Group = s.f_group,
                        Photo = s.c_photo,
                        Disabled = s.b_disabled

                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }

        /// <summary>
        /// Проверка существования пользователя
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public override bool getCmsAccountCode(Guid Code)
        {
            using (var db = new mmBase(_context))
            {
                 bool result = false;

                int count = db.userss.Where(w => w.с_change_pass_code == Code).Count();
                if (count > 0) result = true;

                return result;
            }
        }

        /// <summary>
        /// Права пользователей
        /// </summary>
        /// <returns></returns>
        //public override ResolutionsModel getCmsUserResolutioInfo(Guid _userId, string _pageUrl)
        //{

        //    using (var db = new mmBase(_context))
        //    {
        //        var data = db.sv_resolutionss
        //            .Where(w => (w.c_alias == _pageUrl && w.c_user_id == _userId))
        //            .Select(s => new ResolutionsModel
        //            {
        //                Title = s.c_title,
        //                Read = s.b_read,
        //                Write = s.b_write,
        //                Change = s.b_change,
        //                Delete = s.b_delete
        //            }).ToArray();
        //        if (!data.Any()) { return null; }
        //        else { return data.First(); }
        //    }
        //}

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

                //// Логирование
                //insertLog(id, IP, "change_pass", id, String.Empty, "Users", "Восстановление пароля");
            }
        }

        public override void changePasByCode(Guid Code, string Salt, string Hash, string IP)
        {
            using (var db = new mmBase(_context))
            {
                //string logTitle = db.userss.Where(w => w.с_change_pass_code == Code).Select(s => s.c_fio).First().ToString();
                Guid accountId = db.userss.Where(w => w.с_change_pass_code == Code).Select(s => s.id).First();
                var data = db.userss.Where(w => w.с_change_pass_code == Code);

                Guid? change_pass_code = null;

                if (data != null)
                {
                    data.Where(w => w.с_change_pass_code == Code)
                    .Set(p => p.c_salt, Salt)
                    .Set(p => p.c_hash, Hash)
                    .Set(p => p.n_error_count, 0)
                    .Set(u => u.с_change_pass_code, change_pass_code)
                    .Update();

                    //// логирование
                    //insertLog(accountId, IP, "change_pass", accountId, String.Empty, "Users", logTitle);
                }
            }
        }


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

                //// Логирование
                //insertLog(id, IP, "login", id, String.Empty, "Users", "Авторизация в CMS");
            }
        }
        /// <summary>
        /// Записываем неудачную попытку входа
        /// </summary>
        /// <param name="id"></param>
        /// <param name="IP"></param>
        public override int FailedLogin(Guid id, string IP)
        {
            using (var db = new mmBase(_context))
            {
                int Num = db.userss.Where(w => w.id == id).ToArray().First().n_error_count + 1;

                var data = db.userss.Where(w => w.id == id)
                        .Set(u => u.n_error_count, Num)
                        .Set(u => u.d_try_login, DateTime.Now)
                        .Update();

                //// Логирование
                //insertLog(id, IP, "failed_login", id, String.Empty, "Users", "Неудачная попытка входа");

                if (Num == 5)
                {
                    //// Логирование
                    //insertLog(id, IP, "account_lockout", id, String.Empty, "Users", "Блокировка аккаунта");
                }

                return Num;
            }
        }
        /// <summary>
        /// записываем код востановления пароля
        /// </summary>
        /// <param name="id">id аккаунта</param>
        /// <param name="Code">код восстановления</param>
        /// <param name="IP"></param>
        public override void setRestorePassCode(Guid id, Guid Code, string IP)
        {
            using (var db = new mmBase(_context))
            {
                DateTime? ErrorDate = null;

                var data = db.userss.Where(w => w.id == id)
                    .Set(u => u.с_change_pass_code, Code)
                    .Update();

                //// Логирование
                //insertLog(id, IP, "reqest_change_pass", id, String.Empty, "Users", "Восстановление пароля");
            }
        }
    }
}
