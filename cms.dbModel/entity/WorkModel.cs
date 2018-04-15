using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    public class WorkList
    {
        public WorkModel[] Data;
        public Pager Pager;
    }

    public class WorkModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string User { get; set; }

        [Required(ErrorMessage = "Поле «Название» не должно быть пустым.")]
        public Guid UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserPhoto { get; set; }
        /// <summary>
        /// varchar(512)
        /// </summary>
        [Required(ErrorMessage = "Поле «Название» не должно быть пустым.")]
        public string Title { get; set; }
        /// <summary>
        ///  varchar(1024)
        /// </summary>
        public string Preview { get; set; }
        /// <summary>
        /// varchar(1024)
        /// </summary>
        public string Audio { get; set; }
        /// <summary>
        /// varchar(1024)
        /// </summary>
        public string Video { get; set; }
        /// <summary>
        /// varchar(1024)
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// varchar(1024)
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Дата публикации
        /// </summary>
        [Display(Name = "Дата публикации")]
        [Required(ErrorMessage = "Поле «Название» не должно быть пустым.")]
        public DateTime Date { get; set; }
        /// <summary>
        /// varchar(1024)
        /// </summary>
        public string Desc { get; set; }
        public string Info { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }

        public bool Main { get; set; }

        public GaleryModel Photoalbom { get; set; }
    }
}