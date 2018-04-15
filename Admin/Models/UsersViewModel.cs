using cms.dbModel.entity;
using System.Web.Mvc;

namespace Admin.Models
{
    public class UsersViewModel : CoreViewModel
    {
        public UsersList List { get; set; }
        public User Item { get; set; }


        public SelectList GroupList { get; set; }
        public Catalog_list[] CategoryList { get; set; }
        
        public PasswordModel Password { get; set; }
    }
}
