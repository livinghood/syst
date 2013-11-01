using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer.FollowUp
{
    public class GeneralFollowUp
    {
        public string ObjectID { get; set; }
        public string ObjectName { get; set; }
        public int Revenues { get; set; }
        public int Costs { get; set; }
        public int Result { get; set; }
        public DateTime Month { get; set; }
    }
}
