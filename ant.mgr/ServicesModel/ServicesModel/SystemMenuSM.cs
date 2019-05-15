using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbModel;
using Mapping;
using Newtonsoft.Json;

namespace ServicesModel
{
    public class SystemMenuSM:IMapFrom<SystemMenu>
    {
        public SystemMenuSM()
        {
            ChildMunuList = new List<SystemMenuSM>();
        }
       
        public long Tid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string IdExtend {
            get
            {
                if (Tid < 1)
                {
                    return ActionId;
                }
                else
                {
                    return Tid.ToString();
                }
            }
        }
        public string ActionId { get; set; }
        //public string NameAttribute {
        //    get { return "<a onclick='alert(1)'> " + Name + "</a>"; }
        //}
        public string Ico { get; set; }

        public string Url { get; set; }

        public int? OrderRule { get; set; }

        public int? Level { get; set; }
       

        public long ParentTid { get; set; }

        public string Class { get; set; }

        [JsonProperty("nodes")]
        public List<SystemMenuSM> ChildMunuList { get; set; }

        [JsonProperty("children")]
        public List<SystemMenuSM> ChildMunuList2 {
            get { return this.ChildMunuList; }
        }

        [JsonProperty("checked")]
        public bool HasMenu { get; set; }   


    }
}
