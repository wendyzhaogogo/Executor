using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wilkin.Com.Executor.CollectionExecutor
{
    /// <summary>
    /// 集合执行器操作单元基类
    /// </summary>
    public class ExecuteUnitDTOBase
    {
        /// <summary>
        /// 用于标示当前项的关键字
        /// </summary>
        public virtual string Key { get; set; }
    }
}
