using Wilkin.Com.Executor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wilkin.Com.Executor
{
    public abstract class ExecutorBase
    {
        private ReportStateEventHandler _reportStateDelegate;
        private ExecutePrepareEventHandler _executePrepareDelegate;
        private ExecuteOverEventHandler _executeOverDelegate;
        private bool _isAsynchronous = true;
        private bool _isLogException = true;

        /// <summary>
        /// 是否是异步执行，默认为异步执行
        /// </summary>
        public bool IsAsynchronous
        {
            get
            {
                return _isAsynchronous;
            }
            set
            {
                _isAsynchronous = value;
            }
        }

        /// <summary>
        /// 是否记录异常，默认为是，这时将由执行器捕获异常并记录，否则将抛出由外部处理
        /// </summary>
        public bool IsLogException
        {
            get
            {
                return _isLogException;
            }
            set
            {
                _isLogException = value;
            }
        }

        /// <summary>
        /// 报告事件
        /// </summary>
        public event ReportStateEventHandler OnReportState
        {
            add
            {
                _reportStateDelegate += value;
            }
            remove
            {
                _reportStateDelegate -= value;
            }
        }

        /// <summary>
        /// 准备事件
        /// </summary>
        public event ExecutePrepareEventHandler OnExecutePrepare
        {
            add
            {
                _executePrepareDelegate += value;
            }
            remove
            {
                _executePrepareDelegate -= value;
            }
        }

        /// <summary>
        /// 执行结束事件
        /// </summary>
        public event ExecuteOverEventHandler OnExecuteOver
        {
            add
            {
                _executeOverDelegate += value;
            }
            remove
            {
                _executeOverDelegate -= value;
            }
        }


        protected string _logFilePath = "";

        /// <summary>
        /// 执行器启动，默认为异步执行，可以通过修改IsAsynchronous属性改为同步
        /// </summary>
        /// <param name="dto"></param>
        public void Start(StartDTO dto)
        {
            //准备日志文件
            InitLog(dto.LogFullPath);
            //启动
            DoWorkDTO doworkdto = dto.DoWorkObj;

            //判断是同步执行还是异步执行
            if (_isAsynchronous)
            {
                Thread mythread = new Thread
                (
                    () =>
                    {
                        StartTask(doworkdto);
                    }
                );
                mythread.Start();
            }
            else
            {
                StartTask(doworkdto);
            }
        }

        private void StartTask(DoWorkDTO doworkdto)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                ReportState(new ReportStateDTO() { StateDescription = string.Format("开始@{0}...", startTime.ToString("yyyy:MM:dd HH:mm:ss")) });
                Log(new LogDTO()
                {
                    Text = string.Format("开始@{0}...", DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"))
                });
                if (_executePrepareDelegate != null)
                {
                    _executePrepareDelegate();
                }
                DoWork(doworkdto);
                if (_executeOverDelegate != null)
                {
                    _executeOverDelegate();
                }
                DateTime endTime = DateTime.Now;
                ReportState(new ReportStateDTO() { StateDescription = string.Format("结束@{0}...", endTime.ToString("yyyy:MM:dd HH:mm:ss")) });
                Log(new LogDTO()
                {
                    Text = string.Format("结束@{0}...", DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"))
                });
            }
            catch (Exception ex)
            {
                if (!_isLogException) throw;

                string err = ex.Message;
                ReportState(new ReportStateDTO() { StateDescription = err });
                if (_executeOverDelegate != null)
                {
                    _executeOverDelegate();
                }
                Log(new LogDTO() { Text = err });
            }
        }

        protected abstract void DoWork(DoWorkDTO dto);
        protected virtual void ReportState(ReportStateDTO dto)
        {
            if (_reportStateDelegate != null)
            {
                _reportStateDelegate(dto);
            }
        }
        protected virtual void Log(LogDTO dto)
        {
            if (string.IsNullOrEmpty(_logFilePath) || dto == null)
            {
                return;
            }
            string text = string.Format("{0}{1}", dto.Text, Environment.NewLine);
            using (StreamWriter writer = File.AppendText(_logFilePath))
            {
                writer.Write(text);
                writer.Close();
            }
            //File.AppendAllText(_logFilePath, text);
        }

        private void InitLog(string logFullPath)
        {
            if (File.Exists(logFullPath)) return;

            Directory.CreateDirectory(Directory.GetParent(logFullPath).FullName);
            using (FileStream stream = File.Create(logFullPath))
            {
                stream.Close();
            }
            _logFilePath = logFullPath;
        }
    }

    public delegate void ReportStateEventHandler(ReportStateDTO dto);
    public delegate void ExecutePrepareEventHandler();
    public delegate void ExecuteOverEventHandler();
}
