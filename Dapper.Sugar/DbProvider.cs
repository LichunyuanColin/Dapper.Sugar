using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using static Dapper.Sugar.Config;

namespace Dapper.Sugar
{
    public partial class DbProvider
    {
        private static Dictionary<string, DbProvider> _dbProviders = new Dictionary<string, DbProvider>();

        private DbProvider(string name)
        {
            Connection = Config.Instance.ConnectionList.FirstOrDefault(t => t.Name == name);

            if (Connection == null)
                throw new Exception("缺少对应name的ConnectionStrings配置");

            this.Factory = DbProviderFactoryManage.GetDbProviderFactory(this.Connection.Type);
            this.Builder = SqlBuilder.GetSqlBuilder(this.Connection.Type, this.Factory);
        }

        /// <summary>
        /// 连接
        /// </summary>
        public ConnectionConfig Connection { get; }

        /// <summary>
        /// 
        /// </summary>

        public DbProviderFactory Factory { get; }

        /// <summary>
        /// 
        /// </summary>
        public ISqlBuilder Builder { get; }

        /// <summary>
        /// 获取数据库访问支持
        /// </summary>
        /// <param name="name">ConnectionStrings配置名称</param>
        /// <returns></returns>
        public static DbProvider CreateDbProvide(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            name = name.ToLower();
            if (!_dbProviders.ContainsKey(name))
            {
                _dbProviders[name] = new DbProvider(name);
            }
            return _dbProviders[name];
        }

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="authority">访问权限</param>
        /// <returns></returns>
        public DbConnection CreateConnection(DataBaseAuthority? authority = null)
        {
            var conn = Factory.CreateConnection();

            if (authority == DataBaseAuthority.Read)
            {
                if (Connection.ReadList.Keys.Count == 1)
                {
                    conn.ConnectionString = Connection.ReadList[Connection.WriteList.Keys.First()];
                }
                else
                {
                    conn.ConnectionString = Connection.ReadList[Connection.WriteList.Keys.ToList()[new Random().Next(0, Connection.ReadList.Count)]];
                }
            }
            else
            {
                if (Connection.WriteList.Keys.Count == 1)
                {
                    conn.ConnectionString = Connection.WriteList[Connection.WriteList.Keys.First()];
                }
                else
                {
                    conn.ConnectionString = Connection.WriteList[Connection.WriteList.Keys.ToList()[new Random().Next(0, Connection.WriteList.Count)]];
                }
            }

            return conn;
        }

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="authority">访问权限</param>
        /// <returns></returns>
        public DbConnection CreateOpenConnection(DataBaseAuthority? authority = null)
        {
            var conn = Factory.CreateConnection();

            if (authority == DataBaseAuthority.Read)
            {
                if (Connection.ReadList.Keys.Count == 1)
                {
                    conn.ConnectionString = Connection.ReadList[Connection.WriteList.Keys.First()];
                }
                else
                {
                    conn.ConnectionString = Connection.ReadList[Connection.WriteList.Keys.ToList()[new Random().Next(0, Connection.ReadList.Count)]];
                }
            }
            else
            {
                if (Connection.WriteList.Keys.Count == 1)
                {
                    conn.ConnectionString = Connection.WriteList[Connection.WriteList.Keys.First()];
                }
                else
                {
                    conn.ConnectionString = Connection.WriteList[Connection.WriteList.Keys.ToList()[new Random().Next(0, Connection.WriteList.Count)]];
                }
            }

            conn.Open();

            return conn;
        }

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="name">connectionString配置名称</param>
        /// <returns></returns>
        public DbConnection CreateConnection(string name)
        {
            var conn = Factory.CreateConnection();
            if (Connection.ReadList.ContainsKey(name))
            {
                conn.ConnectionString = Connection.ReadList[name];
            }
            else if (Connection.WriteList.ContainsKey(name))
            {
                conn.ConnectionString = Connection.WriteList[name];
            }
            else
            {
                throw new ArgumentException($"缺少对应name的connectionString配置");
            }
            return conn;
        }


        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="name">connectionString配置名称</param>
        /// <returns></returns>
        public DbConnection CreateOpenConnection(string name)
        {
            var conn = Factory.CreateConnection();
            if (Connection.ReadList.ContainsKey(name))
            {
                conn.ConnectionString = Connection.ReadList[name];
            }
            else if (Connection.WriteList.ContainsKey(name))
            {
                conn.ConnectionString = Connection.WriteList[name];
            }
            else
            {
                throw new ArgumentException($"缺少对应name的connectionString配置");
            }

            conn.Open();

            return conn;
        }

    }
}