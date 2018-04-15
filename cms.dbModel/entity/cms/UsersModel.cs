using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    public class UsersList 
    {
        public User[] Data;
        public Pager Pager;
    }

    public class User
    {
        [Required]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Поле «Группа» не должно быть пустым.")]
        public string Group { get; set; }
        public string GroupName { get; set; }

        public string[] Category { get; set; }

        [Required]
        public string PageName { get; set; }
        public string vkId { get; set; }
        public string fbId { get; set; }

        public DateTime RegDate { get; set; }
        [Required(ErrorMessage = "Поле «Фамилия» не должно быть пустым.")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Поле «Имя» не должно быть пустым.")]
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Photo { get; set; }
        public string EMail { get; set; }
        public string Phone { get; set; }

        public string Info { get; set; }

        public string Salt { get; set; }
        public string Hash { get; set; }

        [Required]
        public bool Disabled { get; set; }

        public DopParams[] WorkCounts { get; set; }
    }
        
    public class PasswordModel
    {
        [Required(ErrorMessage = "Поле Пароль» не должно быть пустым.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Длина пароля должна быть от 6 до 16 символов")]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{6,16}", ErrorMessage = "Пароль имеет не правильный формат")]
        [DataType(DataType.Password)]
        public virtual string Password { get; set; }

        [Required(ErrorMessage = "Поле «Подтверждение пароля» не должно быть пустым.")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public virtual string PasswordConfirm { get; set; }
    }
   
    public class DopParams
    {
        public string Title { get; set; }
        public string Key { get; set; }
    }
}
