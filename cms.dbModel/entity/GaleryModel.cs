using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    public class GaleryModel
    {
        public PhotoModel[] List;
    }

    public class PhotoModel
    {
        public Guid Id { get; set; }
        public Guid WorkId { get; set; }
        public string Preview { get; set; }
        public string Url { get; set; }
        public int sort { get; set; }
    }
}