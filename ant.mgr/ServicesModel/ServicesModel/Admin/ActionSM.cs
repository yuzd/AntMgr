using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesModel
{
    public class MenuActionSM
    {
        public long MenuTid { get; set; }

        public List<ActionSM> ActionList { get; set; }
    }

    public class ActionSM
    {
        public long MenuId { get; set; }
        public string ActionId { get; set; }

        public string ActionName { get; set; }
    }

    public class SystemAction
    {
        public bool IsGod { get; set; }

        public List<string> ActionList { get; set; }
    }
}
