using cms.dbModel.entity;
using System.ComponentModel.DataAnnotations;

namespace MusicMan.Models
{
    public class AccountViewModel : CoreViewModel
    {
        public AccountModel Item { get; set; }
        public WorkList AccountWorks { get; set; }
        public string Regulations { get; set; }

        public Catalog_list[] CategoryList { get; set; }
    }

    public class LogInModel 
    {
        [Required (ErrorMessage = "Поле «Логин» не должно быть пустым.")]
        public string Login { get; set; }

        [Required (ErrorMessage = "Поле «Пароль» не должно быть пустым.")]
        public string Pass { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RegModel
    {
        [Required(ErrorMessage = "Поле «E-mail» не должно быть пустым.")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Поле «E-mail» заполнено неверно.")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Поле «Адрес страницы» не должно быть пустым.")]
        //public string PageName { get; set; }
        
        public string[] Category { get; set; }

        [Required(ErrorMessage = "Поле «Имя» не должно быть пустым.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Поле «Фамилия» не должно быть пустым.")]
        public string LastName { get; set; }

        [StringLength(16, MinimumLength = 6, ErrorMessage = "Длина пароля должна быть от 6 до 16 символов")]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{6,16}", ErrorMessage = "Пароль имеет не правильный формат")]
        [DataType(DataType.Password)]
        public string Pass { get; set; }
    }


    public class RestoreModel 
    {
        [Required (ErrorMessage = "Поле «E-mail» не должно быть пустым.")]
        [RegularExpression (@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Поле «E-mail» заполнено неверно.")]
        public string Email { get; set; }
    }
}