using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilkin.Com.Executor.CollectionExecutor;

namespace Wilkin.Com.Executor.CollectionExecutor
{
    public abstract class CollectionExecutor<T> : ExecutorBase where T : ExecuteUnitDTOBase
    {
        protected override sealed void DoWork(DoWorkDTO dto)
        {
            var list = GetUnitList(dto).ToList();
            int index = 0;
            int count = list.Count();
            int count_success = 0;
            int count_failure = 0;
            int count_error = 0;
            ReportState(new ReportStateDTO() { StateDescription = string.Format("共读取到{0}个待处理项,开始处理...", count) });
            foreach (T current in list)
            {
                index++;
                UnitHandleResultEnum unitHandleType = UnitHandleResultEnum.Success;
                try
                {
                    CommonReturnType result = UnitHandle(current, dto);

                    if (!result.IsSuccess)
                    {
                        unitHandleType = UnitHandleResultEnum.Failure;
                        count_failure++;
                        Log(new LogDTO()
                        {
                            Text = string.Format("{0}:失败:{1}", current.Key, result.Message)
                        });
                    }
                    else
                    {
                        unitHandleType = UnitHandleResultEnum.Success;
                        count_success++;
                    }
                }
                catch (Exception ex)
                {
                    unitHandleType = UnitHandleResultEnum.Error;
                    count_error++;
                    Log(new LogDTO()
                    {
                        Text = string.Format("{0}:异常:{1}", current.Key, ex.Message)
                    });
                }

                ReportState(new ReportStateDTO() { StateDescription = string.Format("第{0}/{1}个${2}${3}__当前共成功{4}个，失败{5}个，异常{6}个", index, count, current.Key, GetUnitHanleResultStr(unitHandleType), count_success, count_failure, count_error) });
            }
            
            Log(new LogDTO()
            {
                Text = string.Format("共成功{0}个，失败{1}个，异常{2}个", count_success, count_failure, count_error)
            });

            
        }

        private string GetUnitHanleResultStr(UnitHandleResultEnum e)
        {
            string result = "";
            switch (e)
            {
                case UnitHandleResultEnum.Success:
                    result = "成功";
                    break;
                case UnitHandleResultEnum.Failure:
                    result = "失败";
                    break;
                case UnitHandleResultEnum.Error:
                    result = "异常";
                    break;
            }
            return result;
        }

        protected abstract CommonReturnType UnitHandle(T unit, DoWorkDTO dto);
        protected abstract List<T> GetUnitList(DoWorkDTO dto);
    }
}
