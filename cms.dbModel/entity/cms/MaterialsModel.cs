﻿using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    public class MaterialsList
    {
        public MaterialsModel[] Data;
        public Pager Pager;
    }

    public class MaterialsModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Поле «Заголовок» не должно быть пустым.")]
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Preview { get; set; }
        public string Url { get; set; }
        public string UrlName { get; set; }
        public string Text { get; set; }
        [Required]
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
        public int Year { get; set; }
        public int Manth { get; set; }
        public int Day { get; set; }
        public string Keyw { get; set; }
        public string Desc { get; set; }
        [Required]
        public bool Important { get; set; }
        [Required]
        public bool Disabled { get; set; }
    }
}
