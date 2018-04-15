using cms.dbModel.entity;

namespace MusicMan.Models
{
    public class UserViewModel : CoreViewModel
    {
        public UsersList List { get; set; }

        public Catalog_list[] CategoryList { get; set; }
    }
}
