using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models
{
    public class MainViewModel : CoreViewModel
    {
        public cmsLogModel[] AccountLog { get; set; }

        public statModel[] Statistic { get; set; }
    }
}