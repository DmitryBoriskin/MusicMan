using cms.dbModel.entity;

namespace Admin.Models
{
    public class BannersViewModel : CoreViewModel
    {
        public BannerModel[] List { get; set; }
        public BannerModel Item { get; set; }
    }
}
