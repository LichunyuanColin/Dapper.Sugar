using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dapper.Sugar
{
    /// <summary>
    /// 配置
    /// </summary>
    public class Config
    {
        internal static Config Instance = null;
        static Config()
        {
            Instance = GetConfig();
        }

        static Config GetConfig()
        {
            var builder = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            //var settings = configuration.GetSection("appSettings");
            var sugar = configuration.GetSection("dapperSugar");
            if (sugar == null)
            {
                throw new Exception("缺少[dapperSugar]配置");
            }

            bool debug = false;
            bool logSql = false;
            bool.TryParse(sugar.GetSection("debug").Value, out debug);
            bool.TryParse(sugar.GetSection("logsql").Value, out logSql);

            Config result = new Config()
            {
                Debug = debug,
                LogSql = logSql,
                ConnectionList = new List<ConnectionConfig>(),
            };

            var connectionStrings = sugar.GetSection("connectionStrings");

            var list = connectionStrings.GetChildren();
            if (list.Count() == 0)
            {
                throw new Exception("缺少[dapperSugar: connectionStrings]配置");
            }

            foreach (var item in list)
            {
                var name = item.GetSection("name").Value;
                var type = item.GetSection("type").Value;
                var connectionList = item.GetSection("list").GetChildren();

                if (string.IsNullOrEmpty(name))
                    throw new Exception("缺少[dapperSugar: connectionStrings: name]配置");

                if (string.IsNullOrEmpty(type))
                    throw new Exception("缺少[dapperSugar: connectionStrings: type]配置");

                DataBaseType dbType;
                if (!Enum.TryParse<DataBaseType>(type, out dbType))
                    throw new Exception("[dapperSugar: connectionStrings: type]配置错误");

                if (connectionList.Count() == 0)
                    throw new Exception("缺少[dapperSugar: connectionStrings: list]配置");

                name = name.ToLower();

                var connectionItem = new ConnectionConfig
                {
                    Name = name,
                    Type = dbType,
                    ReadList = new Dictionary<string, string>(),
                    WriteList = new Dictionary<string, string>(),
                };

                foreach (var child in connectionList)
                {
                    var connName = child.GetSection("name").Value;

                    var connAuthority = child.GetSection("authority").Value;

                    var connString = child.GetSection("connectionString").Value;

                    if (string.IsNullOrEmpty(connName))
                        throw new Exception("缺少[dapperSugar: connectionStrings: list: name]配置");

                    if (string.IsNullOrEmpty(connAuthority))
                        throw new Exception("缺少[dapperSugar: connectionStrings: list: authority]配置");

                    if (string.IsNullOrEmpty(connString))
                        throw new Exception("缺少[dapperSugar: connectionStrings: list: connectionString]配置");

                    connName = connName.ToLower();
                    connAuthority = connAuthority.ToLower();

                    if (connAuthority.Contains("w"))
                    {
                        if (connectionItem.WriteList.ContainsKey(connName))
                            throw new Exception("配置[dapperSugar: connectionStrings: list: name]重复");

                        connectionItem.WriteList.Add(connName, connString);
                    }

                    if (connAuthority.Contains("r"))
                    {
                        if (connectionItem.ReadList.ContainsKey(connName))
                            throw new Exception("配置[dapperSugar: connectionStrings: list: name]重复");

                        connectionItem.ReadList.Add(connName, connString);
                    }
                }
                //CreateDbProviders(name, type, connectionString);
                result.ConnectionList.Add(connectionItem);
            }

            return result;
        }

        /// <summary>
        /// 是否调试模式（true：如果出现错误会抛出异常）
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// 记录sql语句
        /// </summary>
        public bool LogSql { get; set; }

        /// <summary>
        /// 数据库连接配置
        /// </summary>
        public List<ConnectionConfig> ConnectionList { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public enum DataBaseType
        {
            /// <summary>
            /// 
            /// </summary>
            MySql = 1,
            /// <summary>
            /// 
            /// </summary>
            SqlServer = 2,
            /// <summary>
            /// 
            /// </summary>
            PostgreSql = 3,
            /// <summary>
            /// 
            /// </summary>
            Oracle = 4
        }

        /// <summary>
        /// 数据库权限
        /// </summary>
        [Flags]
        public enum DataBaseAuthority
        {
            /// <summary>
            /// 读取
            /// </summary>
            Read = 1,
            /// <summary>
            /// 写入
            /// </summary>
            Write = 2,
        }

        /// <summary>
        /// 连接配置
        /// </summary>
        public class ConnectionConfig
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 数据库类型
            /// </summary>
            public DataBaseType Type { get; set; }

            ///// <summary>
            ///// 数据库连接字符串
            ///// </summary>
            //public string ConnectionString { get; set; }

            /// <summary>
            /// 数据库连接字符串
            /// </summary>
            public Dictionary<string, string> ReadList { get; set; }

            /// <summary>
            /// 数据库连接字符串
            /// </summary>
            public Dictionary<string, string> WriteList { get; set; }
        }
    }
}
