using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Sugar
{
    /// <summary>
    /// 异常
    /// </summary>
    public class DapperSugarException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public DapperSugarException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">错误信息</param>
        public DapperSugarException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="innerException">上一个异常</param>
        public DapperSugarException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// 配置异常
    /// </summary>
    public class DapperSugarConfigException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public DapperSugarConfigException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">错误信息</param>
        public DapperSugarConfigException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="innerException">上一个异常</param>
        public DapperSugarConfigException(string message, Exception innerException) : base(message, innerException) { }
    }
}
