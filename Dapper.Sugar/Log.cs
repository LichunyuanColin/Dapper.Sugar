using log4net;
using log4net.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dapper.Sugar
{

    /// <summary>
    /// 日志
    /// </summary>
    class Log
    {
        private static readonly ILog logInfo = null;
        private static readonly ILog logError = null;

        static Log()
        {
            var repositoryInfo = LogManager.CreateRepository("InfoLogging");
            var repositoryError = LogManager.CreateRepository("ErrorLogging");
            var file = new FileInfo("dappersugar.config");
            //log4net从log4net.config文件中读取配置信息
            XmlConfigurator.Configure(repositoryInfo, file);
            XmlConfigurator.Configure(repositoryError, file);
            logInfo = LogManager.GetLogger(repositoryInfo.Name, "");
            logError = LogManager.GetLogger(repositoryError.Name, "");
        }

        /// <summary>
        /// 记录sql信息
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        public static void InfoSql(string sql, object param = null)
        {
            logInfo.Info($"sql：[ {sql} ] param：[ {JsonConvert.SerializeObject(param)} ]");
        }

        /// <summary>
        /// 记录sql信息
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        public static void InfoProcedure(string sql, object param = null)
        {
            logInfo.Info($"store procedure：[ {sql} ] param：[ {JsonConvert.SerializeObject(param)} ]");
        }

        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="ex"></param>
        public static void Error(string content, Exception ex)
        {
            if (ex == null)
                logError.Info(content);
            else
                logError.Info(content, ex);
        }

        /// <summary>
        /// 记录sql信息
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="ex"></param>
        public static void ErrorSql(string sql, object param, Exception ex)
        {
            if (ex == null)
                logError.Info($"sql：[ {sql} ] param：[ {JsonConvert.SerializeObject(param)} ]");
            else
                logError.Info($"sql：[ {sql} ] param：[ {JsonConvert.SerializeObject(param)} ]", ex);
        }

        /// <summary>
        /// 记录存储过程信息
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="ex"></param>
        public static void ErrorProcedure(string sql, object param, Exception ex)
        {
            if (ex == null)
                logError.Info($"store procedure：[ {sql} ] param：[ {JsonConvert.SerializeObject(param)} ]");
            else
                logError.Info($"store procedure：[ {sql} ] param：[ {JsonConvert.SerializeObject(param)} ]", ex);
        }

        ///// <summary>
        ///// 记录sql信息
        ///// </summary>
        ///// <param name="sql"></param>
        ///// <param name="param"></param>
        //public static void LogSql(string sql, MySqlParameter[] param = null, Exception ex = null)
        //{
        //    if (ex == null)
        //        log.Info($"sql：[ {sql} ] param：[ {param} ]");
        //    else
        //        log.Info($"sql：[ {sql} ] param：[ {param} ]", ex);
        //}
    }
}
