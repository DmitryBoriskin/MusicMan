using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    public class AccountModel
    {
        public Guid id { get; set; }
        public string PageName { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }

        public string Mail { get; set; }
        public string Salt { get; set; }
        public string Hash { get; set; }

        public string Group { get; set; }

        public string[] Category { get; set; }  
        public string Photo { get; set; }
        public string Phone { get; set; }

        public string Description { get; set; }
        public string vkId { get; set; }
        public string fbId { get; set; }
        public bool CountError { get; set; }
        public DateTime? LockDate { get; set; }
        public bool Disabled { get; set; }
    }
}

