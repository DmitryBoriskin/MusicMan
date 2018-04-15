using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    public class statModel
    {
        public DateTime Date { get; set; }
        public int AllUsers{ get; set; }
        public int UsersVk { get; set; }
        public int UsersFb { get; set; }
        public int Works { get; set; }
    }
}
