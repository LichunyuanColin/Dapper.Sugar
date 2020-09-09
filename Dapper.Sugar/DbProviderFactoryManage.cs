using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Text;
using static Dapper.Sugar.Config;

namespace Dapper.Sugar
{
    /// <summary>
    /// 
    /// </summary>
    public class DbProviderFactoryManage
    {
        private static Dictionary<Config.DataBaseType, DbProviderFactory> _dbProviderFactory = new Dictionary<Config.DataBaseType, DbProviderFactory>();

        private static Type GetType(string typeString)
        {
            string[] typeStrings = typeString.Split(',');
            if (typeStrings.Length < 2)
                throw new DapperSugarConfigException("typeString错误");

            string typeName = typeStrings[0].Trim();
            string assemblyName = typeStrings[1].Trim();
            var assName = new AssemblyName { Name = assemblyName };
            return Assembly.Load(assName).GetType(typeName);
        }

        internal static DbProviderFactory GetDbProviderFactory(DataBaseType type)
        {
            if (!_dbProviderFactory.ContainsKey(type))
            {
                string typeName = null;
                switch (type)
                {
                    case DataBaseType.MySql:
                        typeName = "MySql.Data.MySqlClient.MySqlClientFactory,MySql.Data";
                        break;
                    case DataBaseType.SqlServer:
                        typeName = "System.Data.SqlClient.SqlClientFactory,System.Data.SqlClient";
                        break;
                    case DataBaseType.PostgreSql:
                        typeName = "MySql.Data.MySqlClient.MySqlClientFactory,MySql.Data";
                        break;
                    case DataBaseType.Oracle:
                        typeName = "Oracle.ManagedDataAccess.Client.OracleClientFactory,Oracle.ManagedDataAccess";
                        break;
                    case DataBaseType.SQLite:
                        typeName = "System.Data.SQLite.SQLiteFactory,System.Data.SQLite";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type));
                }
                _dbProviderFactory.Add(type, GetType(typeName).GetField("Instance").GetValue(null) as DbProviderFactory);
            }
            return _dbProviderFactory[type];
        }

    }
}
