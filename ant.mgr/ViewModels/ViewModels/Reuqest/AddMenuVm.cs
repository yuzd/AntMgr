using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModels.Reuqest
{
    public class AddMenuVm
    {
        /// <summary>
        /// 父节点Tid
        /// </summary>
        public long ParentTid { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderRule { get; set; }

        /// <summary>
        /// 样式
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public long Tid { get; set; }
    }
}
