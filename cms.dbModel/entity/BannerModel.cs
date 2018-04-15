using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    public class BannersList
    {
        public BannerModel[] Data;
        public Pager Pager;
    }

    public class BannerModel
    {
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Название баннера")]
        public string Title { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }
        public bool Target { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DateEnd { get; set; }
        public bool Disabled { get; set; }
    }
}