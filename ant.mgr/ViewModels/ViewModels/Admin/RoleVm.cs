using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Condition;

namespace ViewModels.Reuqest
{
    public class RoleAction
    {
        public long MenuId { get; set; }
        public string ActionId { get; set; }

        public List<string> ActionList { get; set; }

    }



    public class RoleVm : ConditionBase
    {

        public string RoleName { get; set; }
        public string CreateUser { get; set; }
       

    }


    public class AddRoleVm
    {
        public long Tid { get; set; }
        public string RoleName { get; set; }

        public string RoleDesc { get; set; }
        public List<long> Ids { get; set; }


        public List<ActionVm> Actions { get; set; }
    }
   
    public class ActionVm
    {
        public long MenuId { get; set; }
        public string ActionId { get; set; }

        public string ActionName { get; set; }
    }
}
