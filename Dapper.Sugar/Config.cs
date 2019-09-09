using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if NET45
using System.Configuration;
#else
using Microsoft.Extensions.Configuration;
#endif
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
#if NET45
            DapperSugarSection section = (DapperSugarSection)ConfigurationManager.GetSection("DapperSugar");

            Config result = new Config()
            {
                Debug = section.Debug,
                LogSql = section.LogSql,
                ConnectionList = null
            };

            if (section.ConnectionStrings?.Count > 0)
            {
                result.ConnectionList = new List<ConnectionConfig>(section.ConnectionStrings.Count);
                foreach (DapperSugarSection.ConnectionStringsElement item in section.ConnectionStrings)
                {
                    if (item.List?.Count == 0)
                        throw new DapperSugarConfigException("缺少[dapperSugar > connectionStrings > list]配置");

                    var read = new Dictionary<string, string>();
                    var write = new Dictionary<string, string>();
                    string connAuthority = null;

                    foreach (DapperSugarSection.ConnectionStringsElement.ListElement conn in item.List)
                    {
                        connAuthority = conn.Authority.ToLower();
                        if (connAuthority.Contains("w"))
                        {
                            if (write.ContainsKey(conn.Name))
                                throw new DapperSugarConfigException($"配置[dapperSugar > connectionStrings > list > name \"{conn.Name}\"]重复");

                            write.Add(conn.Name, conn.ConnectionString);
                        }

                        if (connAuthority.Contains("r"))
                        {
                            if (read.ContainsKey(conn.Name))
                                throw new DapperSugarConfigException($"配置[dapperSugar > connectionStrings > list > name \"{conn.Name}\"]重复");

                            read.Add(conn.Name, conn.ConnectionString);
                        }
                    }

                    result.ConnectionList.Add(new ConnectionConfig
                    {
                        Name = item.Name,
                        Type = item.Type,
                        ReadList = read,
                        WriteList = write,
                    });
                }
            }
            else if (section.Type.HasValue && !string.IsNullOrEmpty(section.ConnectionString))
            {
                string name = section.Name ?? "";
                Dictionary<string, string> dic = new Dictionary<string, string> { { name, section.ConnectionString } };
                result.ConnectionList = new List<ConnectionConfig>()
                {
                    new ConnectionConfig {
                        Name = name,
                        Type = section.Type.Value,
                        ReadList = dic,
                        WriteList = dic,
                    }
                };
            }
            else
            {
                throw new ArgumentException("缺少[dapperSugar > connectionStrings]配置");
            }

            return result;
#else
            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", false, true)
               .AddJsonFile($"appsettings.{environmentName}.json", false, true);

            var configuration = builder.Build();
            //var settings = configuration.GetSection("appSettings");
            var sugar = configuration.GetSection("dapperSugar");
            if (sugar == null)
            {
                throw new DapperSugarConfigException("缺少[dapperSugar]配置");
            }

            bool debug = false;
            bool logSql = false;
            bool.TryParse(sugar.GetSection("debug").Value, out debug);
            bool.TryParse(sugar.GetSection("logsql").Value, out logSql);

            Config result = new Config()
            {
                Debug = debug,
                LogSql = logSql,
                ConnectionList = null,
            };

            string name = sugar.GetSection("name").Value;
            string type = sugar.GetSection("type").Value;
            DataBaseType dbType;
            string connectionString = sugar.GetSection("connectionString").Value;

            var connectionStrings = sugar.GetSection("connectionStrings").GetChildren();
            if (connectionStrings.Count() > 0)
            {
                result.ConnectionList = new List<ConnectionConfig>(connectionStrings.Count());

                foreach (var item in connectionStrings)
                {
                    name = item.GetSection("name").Value;
                    type = item.GetSection("type").Value;
                    var list = item.GetSection("list").GetChildren();

                    if (string.IsNullOrEmpty(name))
                        throw new DapperSugarConfigException("缺少[dapperSugar > connectionStrings > name]配置");

                    if (string.IsNullOrEmpty(type))
                        throw new DapperSugarConfigException("缺少[dapperSugar > connectionStrings > type]配置");

                    if (!Enum.TryParse<DataBaseType>(type, out dbType))
                        throw new DapperSugarConfigException("[dapperSugar > connectionStrings > type]配置错误");

                    if (list.Count() == 0)
                        throw new DapperSugarConfigException("缺少[dapperSugar > connectionStrings > list]配置");

                    name = name.ToLower();

                    var connectionItem = new ConnectionConfig
                    {
                        Name = name,
                        Type = dbType,
                        ReadList = new Dictionary<string, string>(),
                        WriteList = new Dictionary<string, string>(),
                    };

                    foreach (var child in list)
                    {
                        var connName = child.GetSection("name").Value;

                        var connAuthority = child.GetSection("authority").Value;

                        var connString = child.GetSection("connectionString").Value;

                        if (string.IsNullOrEmpty(connName))
                            throw new DapperSugarConfigException("缺少[dapperSugar > connectionStrings > list > name]配置");

                        if (string.IsNullOrEmpty(connAuthority))
                            throw new DapperSugarConfigException("缺少[dapperSugar > connectionStrings > list > authority]配置");

                        if (string.IsNullOrEmpty(connString))
                            throw new DapperSugarConfigException("缺少[dapperSugar > connectionStrings > list > connectionString]配置");

                        connName = connName.ToLower();
                        connAuthority = connAuthority.ToLower();

                        if (connAuthority.Contains("w"))
                        {
                            if (connectionItem.WriteList.ContainsKey(connName))
                                throw new DapperSugarConfigException($"配置[dapperSugar > connectionStrings > list > name \"{connName}\"]重复");

                            connectionItem.WriteList.Add(connName, connString);
                        }

                        if (connAuthority.Contains("r"))
                        {
                            if (connectionItem.ReadList.ContainsKey(connName))
                                throw new DapperSugarConfigException($"配置[dapperSugar > connectionStrings > list > name \"{connName}\"]重复");

                            connectionItem.ReadList.Add(connName, connString);
                        }
                    }
                    //CreateDbProviders(name, type, connectionString);
                    result.ConnectionList.Add(connectionItem);
                }

            }
            else if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(connectionString))
            {
                if (!Enum.TryParse<DataBaseType>(type, out dbType))
                    throw new DapperSugarConfigException("[dapperSugar > connectionStrings > type]配置错误");

                name = name ?? "";
                Dictionary<string, string> dic = new Dictionary<string, string> { { name, connectionString } };
                result.ConnectionList = new List<ConnectionConfig>()
                {
                    new ConnectionConfig
                    {
                        Name = name,
                        Type = dbType,
                        ReadList = dic,
                        WriteList = dic,
                    }
                };
            }
            else
                throw new DapperSugarConfigException("缺少[dapperSugar > connectionStrings]配置");

            return result;
#endif
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

#if NET45
    /// <summary>
    /// 
    /// </summary>
    public class DapperSugarSection : ConfigurationSection
    {
        /// <summary>
        /// 是否调试
        /// </summary>
        [ConfigurationProperty("debug", DefaultValue = "false")]
        public bool Debug
        {
            get
            {
                return Convert.ToBoolean(this["debug"]);
            }
        }
        /// <summary>
        /// 是否记录sql语句
        /// </summary>
        [ConfigurationProperty("logsql", DefaultValue = "false")]
        public bool LogSql
        {
            get
            {
                return Convert.ToBoolean(this["logsql"]);
            }
        }

        /// <summary>
        /// 名称
        /// </summary>
        [ConfigurationProperty("name")]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        [ConfigurationProperty("type")]
        public Config.DataBaseType? Type
        {
            get
            {
                return this["type"] as Config.DataBaseType?;
            }
        }

        /// <summary>
        /// 数据库库连接字符串
        /// </summary>
        [ConfigurationProperty("connectionString")]
        public string ConnectionString
        {
            get { return this["connectionString"].ToString(); }
            set { this["connectionString"] = value; }
        }

        /// <summary>
        /// 数据库库连接字符串
        /// </summary>
        [ConfigurationProperty("connectionStrings", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ConnectionStringsCollection), AddItemName = "add")]
        public ConnectionStringsCollection ConnectionStrings
        {
            get { return (ConnectionStringsCollection)base["connectionStrings"]; }
        }

        ///// <summary>
        ///// 连接配置
        ///// </summary>
        //[ConfigurationProperty("connectionStrings", IsDefaultCollection = false)]
        //[ConfigurationCollection(typeof(ConnectionStringsCollection), AddItemName = "add")]
        //public ConnectionStringsCollection ConnectionStrings
        //{
        //    get
        //    {
        //        return (ConnectionStringsCollection)base["connectionStrings"];
        //    }
        //}

        /// <summary>
        /// 数据库连接配置
        /// </summary>
        public class ConnectionStringsElement : ConfigurationElement
        {
            /// <summary>
            /// 名称
            /// </summary>
            [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
            public string Name
            {
                get
                {
                    return this["name"] as string;
                }
            }

            /// <summary>
            /// 数据库类型
            /// </summary>
            [ConfigurationProperty("type", IsRequired = true)]
            public Config.DataBaseType Type
            {
                get
                {
                    return (Config.DataBaseType)this["type"];
                }
            }

            /// <summary>
            /// 连接配置
            /// </summary>
            [ConfigurationProperty("list", IsDefaultCollection = false, IsRequired = true)]
            [ConfigurationCollection(typeof(ListCollection), AddItemName = "add")]
            public ListCollection List
            {
                get
                {
                    return (ListCollection)base["list"];
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public class ListElement : ConfigurationElement
            {
                /// <summary>
                /// 名称
                /// </summary>
                [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
                public string Name
                {
                    get
                    {
                        return this["name"] as string;
                    }
                }

                /// <summary>
                /// 权限
                /// </summary>
                [ConfigurationProperty("authority", DefaultValue = "Write")]
                public string Authority
                {
                    get
                    {
                        return this["authority"] as string;
                    }
                }

                /// <summary>
                /// 数据库库连接字符串
                /// </summary>
                [ConfigurationProperty("connectionString", IsRequired = true)]
                public string ConnectionString
                {
                    get { return this["connectionString"].ToString(); }
                    set { this["connectionString"] = value; }
                }
            }

            /// <summary>
            /// list配置
            /// </summary>
            public class ListCollection : ConfigurationElementCollection
            {
                public override ConfigurationElementCollectionType CollectionType
                {
                    get
                    {
                        return ConfigurationElementCollectionType.AddRemoveClearMap;
                    }
                }

                protected override ConfigurationElement CreateNewElement()
                {
                    return new ListElement();
                }

                protected override object GetElementKey(ConfigurationElement element)
                {
                    return (element as ListElement).Name;
                }

                ///// <summary>
                ///// 遍历
                ///// </summary>
                ///// <returns></returns>
                //public IEnumerator<ListElement> GetEnumerator()
                //{
                //    return (IEnumerator<ListElement>)base.GetEnumerator();
                //}
            }
        }


        /// <summary>
        /// list配置
        /// </summary>
        public class ConnectionStringsCollection : ConfigurationElementCollection
        {
            public override ConfigurationElementCollectionType CollectionType
            {
                get
                {
                    return ConfigurationElementCollectionType.AddRemoveClearMap;
                }
            }

            protected override ConfigurationElement CreateNewElement()
            {
                return new ConnectionStringsElement();
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                return (element as ConnectionStringsElement).Name;
            }

            /// <summary>
            /// 索引器
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public ConnectionStringsElement this[int index]
            {
                get { return (ConnectionStringsElement)BaseGet(index); }
            }
        }
    }
#endif
}
