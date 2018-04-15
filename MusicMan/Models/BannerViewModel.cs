using cms.dbModel.entity;

namespace MusicMan.Models
{
    public class BannerViewModel : CoreViewModel
    {
        public BannerModel[] List { get; set; }
        public BannerModel Item { get; set; }

        //public Sections_list[] SectionsList { get; set; }
    }
}
