using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wilkin.Com.Executor
{
    public class CommonReturnType
    {
        public bool IsSuccess { get; set; }
        public ReturnTypeCodeEnum Code { get; set; }
        public string Message { get; set; }
    }

    public enum ReturnTypeCodeEnum
    {
        None,
        Success,
        Failure,
        Error
    }
}
