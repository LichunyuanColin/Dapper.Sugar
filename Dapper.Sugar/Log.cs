using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
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
        private static readonly ILog logger = null;
        //private static readonly ILog logError = null;

        static Log()
        {
            var repository = LogManager.CreateRepository("");

            var file = new FileInfo("dappersugar.config");

            if (file.Exists)
            {
                //log4net从log4net.config文件中读取配置信息
                XmlConfigurator.Configure(repository, file);
            }
            else
            {
                PatternLayout layout = new PatternLayout()
                {
                    ConversionPattern = "%date [%thread]%-5p %c - %m %newline %exception %newline",
                };
                layout.ActivateOptions();

                //info记录
                var defaultAppender = new RollingFileAppender()
                {
                    Name = "DefaultAppender",
                    File = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "DapperSugar", "Sql") + Path.DirectorySeparatorChar,
                    LockingModel = new FileAppender.MinimalLock(),
                    DatePattern = "yyyy-MM-dd\".txt\"",
                    StaticLogFileName = false,
                    AppendToFile = true,
                    RollingStyle = RollingFileAppender.RollingMode.Date,
                    Layout = layout,
                    Threshold = Level.All,
                };
                defaultAppender.AddFilter(new log4net.Filter.LevelRangeFilter()
                {
                    LevelMax = Level.Warn,
                    LevelMin = Level.Debug
                });
                defaultAppender.ActivateOptions();

                //error记录
                var errorAppender = new RollingFileAppender()
                {
                    Name = "ErrorAppender",
                    File = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "DapperSugar", "Error") + Path.DirectorySeparatorChar,
                    LockingModel = new FileAppender.MinimalLock(),
                    DatePattern = "yyyy-MM-dd\".txt\"",
                    StaticLogFileName = false,
                    AppendToFile = true,
                    RollingStyle = RollingFileAppender.RollingMode.Date,
                    Layout = layout,
                    Threshold = Level.All,
                };
                errorAppender.AddFilter(new log4net.Filter.LevelRangeFilter()
                {
                    LevelMax = Level.Fatal,
                    LevelMin = Level.Error
                });
                errorAppender.ActivateOptions();

                repository.Threshold = Level.All;

                BasicConfigurator.Configure(repository, defaultAppender, errorAppender);
            }

            logger = LogManager.GetLogger(repository.Name, "");
        }

        /// <summary>
        /// 记录sql信息
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        public static void InfoSql(string sql, object param = null)
        {
            logger.Info($"sql：[ {sql} ] param：[ {JsonConvert.SerializeObject(param)} ]");
        }

        /// <summary>
        /// 记录sql信息
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        public static void InfoProcedure(string sql, object param = null)
        {
            logger.Info($"store procedure：[ {sql} ] param：[ {JsonConvert.SerializeObject(param)} ]");
        }

        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="ex"></param>
        public static void Error(string content, Exception ex)
        {
            if (ex == null)
                logger.Error(content);
            else
                logger.Error(content, ex);
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
                logger.Error($"sql：[ {sql} ] param：[ {JsonConvert.SerializeObject(param)} ]");
            else
                logger.Error($"sql：[ {sql} ] param：[ {JsonConvert.SerializeObject(param)} ]", ex);
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
                logger.Error($"store procedure：[ {sql} ] param：[ {JsonConvert.SerializeObject(param)} ]");
            else
                logger.Error($"store procedure：[ {sql} ] param：[ {JsonConvert.SerializeObject(param)} ]", ex);
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
