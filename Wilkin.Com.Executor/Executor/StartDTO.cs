using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wilkin.Com.Executor
{
    public class StartDTO
    {
        public DoWorkDTO DoWorkObj { get; set; }

        /// <summary>
        /// 日志路径，必须要传，执行器必须有日志文件
        /// </summary>
        public string LogFullPath { get; set; }
    }
}
