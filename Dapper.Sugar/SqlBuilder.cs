﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using static Dapper.Sugar.Config;

namespace Dapper.Sugar
{
    /// <summary>
    /// 格式化类型（不含逻辑关系）
    /// </summary>
    public enum FormateTypeSingle
    {
        /// <summary>
        /// 表名称
        /// </summary>
        TableName,
        /// <summary>
        /// 字段名称
        /// </summary>
        FieldName,
        /// <summary>
        /// 参数名称
        /// </summary>
        ParamName,
    }

    /// <summary>
    /// 格式化类型（含逻辑关系）
    /// </summary>
    public enum FormateTypeCalculate
    {
        /// <summary>
        /// 等于
        /// </summary>
        ParamEqual = 3,
        /// <summary>
        /// 小于
        /// </summary>
        ParamLess,
        /// <summary>
        /// 小于等于
        /// </summary>
        ParamLessEqual,
        /// <summary>
        /// 大于
        /// </summary>
        ParamMore,
        /// <summary>
        /// 大于等于
        /// </summary>
        ParamMoreEqual,
        /// <summary>
        /// 模糊查询
        /// </summary>
        ParamLike,
        /// <summary>
        /// In
        /// </summary>
        ParamIn,
        /// <summary>
        /// not in
        /// </summary>
        ParamNotIn,
        /// <summary>
        /// 不等于
        /// </summary>
        ParamUnEqual
    }

    /// <summary>
    /// sql生成
    /// </summary>
    public interface ISqlBuilder
    {
        /// <summary>
        /// 表默认主键
        /// </summary>
        string DefaultTableKey { get; set; }

        /// <summary>
        /// 参数标识
        /// </summary>
        string ParamSign { get; set; }
        /// <summary>
        /// 前缀
        /// </summary>
        string SqlPrefix { get; set; }
        /// <summary>
        /// 后缀
        /// </summary>
        string SqlSuffix { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="additionalSql"></param>
        /// <returns></returns>
        string GetSelectSqlFromTableDirect(string sql, object param, string additionalSql = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="additionalSql"></param>
        /// <returns></returns>
        string GetSelectSqlFromSelectSql(string sql, object param, string additionalSql = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="defaultSql"></param>
        /// <returns></returns>
        string GetConditionSqlByParam(object param, string defaultSql = "1=1");
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        string GetFieldName(string fieldName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        string GetInsertSql(string tableName, object param);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sortSql"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        (string totalSql, string dataSql) GetPagingSql(string sql, string sortSql, int pageNumber, int pageSize);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        string GetParamName(string paramName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        string GetParamSql(FormateTypeCalculate type, string fieldName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldName"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        string GetParamSql(FormateTypeCalculate type, string fieldName, string paramName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        string GetTableName(string tableName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="param"></param>
        /// <param name="tableKey"></param>
        /// <returns></returns>
        string GetUpdateSql(string tableName, object param, string tableKey = null);
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="tableName"></param>
        ///// <param name="updateParam"></param>
        ///// <param name="conditionParam"></param>
        ///// <param name="tableKey"></param>
        ///// <returns></returns>
        //string GetUpdateSql(string tableName, object updateParam, object conditionParam);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        string GetAutoIncrement(string fieldName);
    }

    /// <summary>
    /// 生成sql语句
    /// </summary>
    abstract class SqlBuilder : ISqlBuilder
    {
        protected const string DEFAULT_CONDITION_SQL = "1=1";
        //private const string TABLE_PREFIX = "jth";//TablePrefix

        /// <summary>
        /// 表默认主键
        /// </summary>
        public string DefaultTableKey { get; set; } = "Id";

        public virtual string ParamSign { get; set; } = "@";//参数标识
        public string SqlPrefix { get; set; }//前缀
        public string SqlSuffix { get; set; }//后缀


        private static Dictionary<Config.DataBaseType, SqlBuilder> _sqlBuilder = new Dictionary<Config.DataBaseType, SqlBuilder>();

        public static SqlBuilder GetSqlBuilder(DataBaseType type, DbProviderFactory factory)
        {
            if (!_sqlBuilder.ContainsKey(type))
            {
                SqlBuilder builder = null;

                switch (type)
                {
                    case DataBaseType.MySql:
                        builder = new MySqlBuilder();
                        break;
                    case DataBaseType.SqlServer:
                        builder = new SqlServerBuilder();
                        break;
                    case DataBaseType.PostgreSql:
                        builder = new PostgreSqlBuilder();
                        break;
                    case DataBaseType.Oracle:
                        builder = new OracleBuilder();
                        break;
                    case DataBaseType.SQLite:
                        builder = new SQLiteBuilder();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type));
                }

                var command = factory.CreateCommandBuilder();

                builder.SqlPrefix = command.QuotePrefix;
                builder.SqlSuffix = command.QuoteSuffix;

                command.Dispose();

                _sqlBuilder.Add(type, builder);
            }
            return _sqlBuilder[type];
        }

        /// <summary>
        /// 判断是否数组或泛型数组
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsArrayOrList(Type type)
        {
            if (!type.IsGenericType)
                return false;

            var typeInfo = type.GetTypeInfo();

            return
               typeInfo.ImplementedInterfaces.Any(ti => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ||
               typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        #region 基础

        /// <summary>
        /// 格式化字段名称（FieldName）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public virtual string GetFieldName(string fieldName)
        {
            return $"{SqlPrefix}{fieldName}{SqlSuffix}";
        }

        /// <summary>
        /// 格式化表名称（TableName）
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public virtual string GetTableName(string tableName)
        {
            return $"{SqlPrefix}{tableName}{SqlSuffix}";
        }

        /// <summary>
        /// 格式化参数名称
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <returns></returns>
        public virtual string GetParamName(string paramName)
        {
            return $"{ParamSign}{paramName}";
        }

        /// <summary>
        /// 生成字段sql（参数）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="type">格式化类型</param>
        /// <returns></returns>
        public virtual string GetParamSql(FormateTypeCalculate type, string fieldName)
        {
            return GetParamSql(type, fieldName, fieldName);
        }

        /// <summary>
        /// 生成字段sql（参数）
        /// </summary>
        /// <param name="type">格式化类型</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="paramName">参数名称</param>
        /// <returns></returns>
        public virtual string GetParamSql(FormateTypeCalculate type, string fieldName, string paramName)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentNullException(nameof(paramName));

            string tableName = string.Empty;
            //bool containParamSign = true;
            if (fieldName.Length > 2 && fieldName[1] == '_')
            {
                //有别名
                string aliasName = fieldName.Substring(0, 1);
                fieldName = fieldName.Substring(2);
                switch (type)
                {
                    case FormateTypeCalculate.ParamEqual://等于
                        return $"{aliasName}.{SqlPrefix}{fieldName}{SqlSuffix} = {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamLess://小于
                        return $"{aliasName}.{SqlPrefix}{fieldName}{SqlSuffix} < {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamLessEqual://小于等于
                        return $"{aliasName}.{SqlPrefix}{fieldName}{SqlSuffix} <= {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamMore://大于
                        return $"{aliasName}.{SqlPrefix}{fieldName}{SqlSuffix} > {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamMoreEqual://大于等于
                        return $"{aliasName}.{SqlPrefix}{fieldName}{SqlSuffix} >= {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamLike://模糊查询
                        return $"{aliasName}.{SqlPrefix}{fieldName}{SqlSuffix} like {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamIn://In
                        return $"{aliasName}.{SqlPrefix}{fieldName}{SqlSuffix} in {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamUnEqual://不等于
                        return $"{aliasName}.{SqlPrefix}{fieldName}{SqlSuffix} <> {ParamSign}{paramName}";
                    default: throw new ArgumentOutOfRangeException(nameof(type));
                }
            }
            else
            {
                //无别名
                switch (type)
                {
                    case FormateTypeCalculate.ParamEqual://等于
                        return $"{SqlPrefix}{fieldName}{SqlSuffix} = {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamLess://小于
                        return $"{SqlPrefix}{fieldName}{SqlSuffix} < {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamLessEqual://小于等于
                        return $"{SqlPrefix}{fieldName}{SqlSuffix} <= {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamMore://大于
                        return $"{SqlPrefix}{fieldName}{SqlSuffix} > {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamMoreEqual://大于等于
                        return $"{SqlPrefix}{fieldName}{SqlSuffix} >= {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamLike://模糊查询
                        return $"{SqlPrefix}{fieldName}{SqlSuffix} like {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamIn://In
                        return $"{SqlPrefix}{fieldName}{SqlSuffix} in {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamUnEqual://不等于
                        return $"{SqlPrefix}{fieldName}{SqlSuffix} <> {ParamSign}{paramName}";
                    default: throw new ArgumentOutOfRangeException(nameof(type));
                }
            }
        }

        /// <summary>
        /// 生成查询语句sql
        /// </summary>
        /// <param name="param"></param>
        /// <param name="sql">可为tableName或selectSql</param>
        /// <param name="additionalSql"></param>
        /// <returns></returns>
        public virtual string GetSelectSqlFromTableDirect(string sql, object param, string additionalSql = null)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException(nameof(sql));

            return $"SELECT * FROM {GetTableName(sql)} WHERE {GetConditionSqlByParam(param)} {additionalSql}";
        }

        /// <summary>
        /// 生成查询语句sql
        /// </summary>
        /// <param name="param"></param>
        /// <param name="sql">可为tableName或selectSql</param>
        /// <param name="additionalSql"></param>
        /// <returns></returns>
        public virtual string GetSelectSqlFromSelectSql(string sql, object param, string additionalSql = null)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException(nameof(sql));

            //已包含and语句、已包含where语句
            sql = sql.Trim();
            if (sql.IndexOf("AND", sql.Length - 3, StringComparison.OrdinalIgnoreCase) > -1 || sql.IndexOf("WHERE", sql.Length - 5, StringComparison.OrdinalIgnoreCase) > -1)
            {
                return $"{sql} {GetConditionSqlByParam(param)} {additionalSql}";
            }
            else
                return $"{sql} WHERE {GetConditionSqlByParam(param)} {additionalSql}";
        }

        //public virtual void AddConditionSqlByField(StringBuilder sql, string name, object value)
        //{
        //    if (value == null)
        //        return;

        //    if (name.Length > 3)
        //    {
        //        if (name[2] == '_')//前缀
        //        {
        //            char start1 = name[0];
        //            char start2 = name[1];

        //            if (start1 == 'i' && start2 == 'g')
        //                return;

        //            if (start1 == 's' && start2 == 'q')
        //            {
        //                if (Config.Instance.Debug && !(value is string))
        //                {
        //                    throw new ArgumentException($"{name}只能为string类型");
        //                }

        //                string ss = (value as string).Trim();

        //                if (sql.Length > 0 && ss.Length > 2
        //                    && ss.IndexOf("AND", 0, 3, StringComparison.OrdinalIgnoreCase) < 0
        //                    && ss.IndexOf("OR", 0, 2, StringComparison.OrdinalIgnoreCase) < 0)
        //                    sql.Append(" AND ");
        //                else
        //                    sql.Append(" ");
        //                sql.Append(ss);

        //                //if (!Config.Instance.Debug)//如果不是调试模式则重置参数为null
        //                //    item.SetValue(param, null);

        //                return;
        //            }
        //        }


        //        if (name[name.Length - 3] == '_')//后缀
        //        {
        //            string filterName = name.Substring(0, name.Length - 3);
        //            char end1 = name[name.Length - 2];
        //            char end2 = name[name.Length - 1];

        //            if (end1 == 'l')
        //            {
        //                if (sql.Length > 0)
        //                    sql.Append(" AND ");

        //                switch (end2)
        //                {
        //                    case 't': sql.Append(GetParamSql(FormateTypeCalculate.ParamLess, filterName, name)); return;
        //                    case 'e': sql.Append(GetParamSql(FormateTypeCalculate.ParamLessEqual, filterName, name)); return;
        //                    case 'k': sql.Append(GetParamSql(FormateTypeCalculate.ParamLike, filterName, name)); return;
        //                    //default:
        //                    //    throw new ArgumentException("暂不支持此后缀 " + name.Substring(name.Length - 2, 2));
        //                }
        //            }
        //            else if (end1 == 'g')
        //            {
        //                if (sql.Length > 0)
        //                    sql.Append(" AND ");

        //                switch (end2)
        //                {
        //                    case 't': sql.Append(GetParamSql(FormateTypeCalculate.ParamMore, filterName, name)); return;
        //                    case 'e': sql.Append(GetParamSql(FormateTypeCalculate.ParamMoreEqual, filterName, name)); return;
        //                    //default:
        //                    //    throw new ArgumentException("暂不支持此后缀 " + name.Substring(name.Length - 2, 2));
        //                }
        //            }
        //            else if ((end1 == 'u' || end1 == 'n') && end2 == 'e')
        //            {
        //                if (sql.Length > 0)
        //                    sql.Append(" AND ");

        //                sql.Append(GetParamSql(FormateTypeCalculate.ParamUnEqual, filterName, name));
        //                return;
        //            }
        //            else if (end1 == 'i' && end2 == 'g')
        //                return;
        //            //else
        //            //    throw new ArgumentException("暂不支持此后缀 " + name.Substring(name.Length - 2, 2));
        //        }
        //    }

        //    var type = value.GetType();
        //    //判断是否数组或List<T>,IsGenericType有bug，class<T>也返回true
        //    if (type.IsArray
        //        || (type.IsGenericType
        //        && (type.Name.StartsWith("List`") || type.Name.StartsWith("IEnumerable`"))))
        //    {
        //        if (sql.Length > 0)
        //            sql.Append(" AND ");
        //        sql.Append(GetParamSql(FormateTypeCalculate.ParamIn, name));
        //        return;
        //    }

        //    if (sql.Length > 0)
        //        sql.Append(" AND ");
        //    sql.Append(GetParamSql(FormateTypeCalculate.ParamEqual, name));
        //}

        /// <summary>
        /// 参数转成条件sql（默认返回1=1）
        /// </summary>
        /// <param name="param">参数（lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="defaultSql">默认无参数时sql语句</param>
        /// <returns></returns>
        public virtual string GetConditionSqlByParam(object param, string defaultSql = "1=1")
        {
            if (param == null)
                return defaultSql;

            PropertyInfo[] props = param.GetType().GetProperties();
            if (props.Length == 0)
                return defaultSql;

            StringBuilder sql = new StringBuilder();
            foreach (PropertyInfo item in props)
            {
                var value = item.GetValue(param, null);
                if (value == null)
                    continue;


                if (item.Name.Length > 3)
                {
                    if (item.Name[2] == '_')//前缀
                    {

                        char start1 = item.Name[0];
                        char start2 = item.Name[1];

                        if (start1 == 'i' && start2 == 'g')
                            continue;

                        if (start1 == 's' && start2 == 'q')
                        {
                            //if (item.PropertyType != typeof(string))
                            //    throw new ArgumentException($"{item.Name}只能为string类型");

                            string ss = (value as string).Trim();
                            if (sql.Length > 0 && ss.Length > 2
                                && ss.IndexOf("AND", 0, 3, StringComparison.OrdinalIgnoreCase) < 0
                                && ss.IndexOf("OR", 0, 2, StringComparison.OrdinalIgnoreCase) < 0)
                                sql.Append(" AND ");
                            else
                                sql.Append(' ');
                            sql.Append(ss);

                            //if (!Config.Instance.Debug)//如果不是调试模式则重置参数为null
                            //    item.SetValue(param, null);

                            continue;
                        }
                    }


                    if (item.Name[item.Name.Length - 3] == '_')//后缀
                    {
                        string filterName = item.Name.Substring(0, item.Name.Length - 3);
                        char end1 = item.Name[item.Name.Length - 2];
                        char end2 = item.Name[item.Name.Length - 1];

                        if (end1 == 'l')
                        {
                            switch (end2)
                            {
                                case 't':
                                    if (sql.Length > 0)
                                        sql.Append(" AND ");
                                    sql.Append(GetParamSql(FormateTypeCalculate.ParamLess, filterName, item.Name));
                                    continue;
                                case 'e':
                                    if (sql.Length > 0)
                                        sql.Append(" AND ");
                                    sql.Append(GetParamSql(FormateTypeCalculate.ParamLessEqual, filterName, item.Name));
                                    continue;
                                case 'k':
                                    if (sql.Length > 0)
                                        sql.Append(" AND ");
                                    sql.Append(GetParamSql(FormateTypeCalculate.ParamLike, filterName, item.Name));
                                    continue;
                                    //default:
                                    //    throw new ArgumentException("暂不支持此后缀 " + item.Name.Substring(item.Name.Length - 2, 2));
                            }
                        }
                        else if (end1 == 'g')
                        {
                            switch (end2)
                            {
                                case 't':
                                    if (sql.Length > 0)
                                        sql.Append(" AND ");
                                    sql.Append(GetParamSql(FormateTypeCalculate.ParamMore, filterName, item.Name));
                                    continue;
                                case 'e':
                                    if (sql.Length > 0)
                                        sql.Append(" AND ");
                                    sql.Append(GetParamSql(FormateTypeCalculate.ParamMoreEqual, filterName, item.Name));
                                    continue;
                                    //default:
                                    //    throw new ArgumentException("暂不支持此后缀 " + item.Name.Substring(item.Name.Length - 2, 2));
                            }
                        }
                        else if ((end1 == 'u' || end1 == 'n') && end2 == 'e')
                        {
                            if (sql.Length > 0)
                                sql.Append(" AND ");

                            sql.Append(GetParamSql(FormateTypeCalculate.ParamUnEqual, filterName, item.Name));
                            continue;
                        }
                        else if (end1 == 'i' && end2 == 'g')
                            continue;
                        //else
                        //    throw new ArgumentException("暂不支持此后缀 " + item.Name.Substring(item.Name.Length - 2, 2));
                    }
                }

                //判断是否数组或List<T>,IsGenericType有bug，class<T>也返回true
                if (item.PropertyType.IsArray
                    || (item.PropertyType.IsGenericType
                    && (item.PropertyType.Name.StartsWith("List`") || item.PropertyType.Name.StartsWith("IEnumerable`"))))
                {
                    if (sql.Length > 0)
                        sql.Append(" AND ");
                    sql.Append(GetParamSql(FormateTypeCalculate.ParamIn, item.Name));
                    continue;
                }

                if (sql.Length > 0)
                    sql.Append(" AND ");
                sql.Append(GetParamSql(FormateTypeCalculate.ParamEqual, item.Name));
            }

            //如果参数为0，返回默认sql
            if (sql.Length == 0)
                return defaultSql;

            return sql.ToString();
        }

        /// <summary>
        /// 生成新增sql命令
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual string GetInsertSql(string tableName, object param)
        {
            Type type = param.GetType();
            if (type.IsArray)
            {
                //判断是否数组
                type = type.GetElementType();
                param = (param as object[])[0];
            }
            else if (IsArrayOrList(type))
            {
                //判断是否泛型
                //var temp = type.GetGenericArguments();
                //type = temp[temp.Length - 1];
                param = (param as IEnumerable<object>).AsList()[0];
                type = param.GetType();
            }

            PropertyInfo[] propertys = type.GetProperties();
            if (propertys.Length == 0)
                throw new ArgumentException("param");
            StringBuilder sqlField = new StringBuilder();
            StringBuilder sqlValue = new StringBuilder();
            foreach (PropertyInfo item in propertys)
            {
                if (item.Name.Length > 3 && item.Name[2] == '_')
                {
                    char start1 = item.Name[0];
                    char start2 = item.Name[1];

                    if (start1 == 'i' && start2 == 'g')
                        continue;

                    if (start1 == 's' && start2 == 'q')
                    {
                        var v = item.GetValue(param, null);
                        if (!(v is string))
                            throw new ArgumentException($"{item.Name}只能为string类型");
                        if (v != null)
                        {
                            sqlField.Append(GetFieldName(item.Name.Substring(3)));
                            sqlField.Append(",");

                            sqlValue.Append(v);
                            sqlValue.Append(",");

                            //if (!Config.Instance.Debug)//如果不是调试模式则重置参数为null
                            //    item.SetValue(param, null);
                        }
                        continue;
                    }
                }

                if (item.GetCustomAttributes(typeof(IgnoreAddAttribute), false).Length == 0)
                {
                    sqlField.Append(GetFieldName(item.Name));
                    sqlField.Append(",");

                    sqlValue.Append(GetParamName(item.Name));
                    sqlValue.Append(",");
                }
            }
            sqlField.Remove(sqlField.Length - 1, 1);
            sqlValue.Remove(sqlValue.Length - 1, 1);
            return string.Format("INSERT INTO {0}({1}) VALUES({2})", GetTableName(tableName), sqlField.ToString(), sqlValue.ToString());
        }

        /// <summary>
        /// 生成修改sql命令
        /// </summary>
        /// <param name="param"></param>
        /// <param name="tableName"></param>
        /// <param name="tableKey"></param>
        /// <returns></returns>
        public virtual string GetUpdateSql(string tableName, object param, string tableKey = null)
        {
            if (string.IsNullOrEmpty(tableKey))
                tableKey = DefaultTableKey;

            Type type = param.GetType();
            if (type.IsArray)
            {
                //判断是否数组
                type = type.GetElementType();
                param = (param as object[])[0];
            }
            else if (IsArrayOrList(type))
            {
                //判断是否泛型
                //var temp = type.GetGenericArguments();
                //type = temp[temp.Length - 1];
                param = (param as IEnumerable<object>).AsList()[0];
                type = param.GetType();
            }

            PropertyInfo[] propertys = type.GetProperties();
            if (propertys.Length == 0)
                throw new ArgumentException();
            StringBuilder sqlField = new StringBuilder();
            //StringBuilder sqlValue = new StringBuilder();
            foreach (PropertyInfo item in propertys)
            {
                if (item.Name.Length > 3 && item.Name[2] == '_')
                {
                    char start1 = item.Name[0];
                    char start2 = item.Name[1];

                    if (start1 == 'i' && start2 == 'g')
                        continue;

                    if (start1 == 's' && start2 == 'q')
                    {
                        var v = item.GetValue(param, null);
                        if (!(v is string))
                            throw new ArgumentException($"{item.Name}只能为string类型");
                        if (v != null)
                        {
                            sqlField.Append(GetFieldName(item.Name.Substring(3)));
                            sqlField.Append("=");
                            sqlField.Append(v);
                            sqlField.Append(",");

                            //if (!Config.Instance.Debug)//如果不是调试模式则重置参数为null
                            //    item.SetValue(param, null);
                        }
                        continue;
                    }
                }

                if (item.GetCustomAttributes(typeof(IgnoreUpdateAttribute), false).Length == 0 && item.Name != tableKey)
                {
                    sqlField.Append(GetParamSql(FormateTypeCalculate.ParamEqual, item.Name));
                    sqlField.Append(",");
                }
            }

            if (sqlField.Length == 0)
                throw new ArgumentException();

            sqlField.Remove(sqlField.Length - 1, 1);
            return string.Format("UPDATE {0} SET {1} WHERE {2}", GetFieldName(tableName), sqlField.ToString(), GetParamSql(FormateTypeCalculate.ParamEqual, tableKey));
        }

        //public virtual string GetUpdateSql(string tableName, object param, object conditionParam, string tableKey = TABLE_KEY)
        //{
        //    Type type = param.GetType();
        //    Type typeCondition = param.GetType();
        //    if (typeCondition.IsArray || typeCondition.Name.StartsWith("List`"))
        //        throw new SugarException(nameof(conditionParam)+"错误，不能为数组类型");

        //    if (type.IsArray)
        //    {
        //        //判断是否数组
        //        type = type.GetElementType();
        //    }
        //    else if (type.Name.StartsWith("List`"))
        //    {
        //        //判断是否泛型
        //        type = type.GetGenericArguments()[0];
        //    }
        //    PropertyInfo[] propertys = type.GetProperties();
        //    if (propertys.Length == 0)
        //        throw new ArgumentException();
        //    StringBuilder sqlField = new StringBuilder();
        //    //StringBuilder sqlValue = new StringBuilder();
        //    foreach (PropertyInfo item in propertys)
        //    {
        //        if (item.GetCustomAttributes(typeof(IgnoreUpdateAttribute), false).Length == 0 && item.Name != tableKey)
        //        {
        //            sqlField.Append(GetParamSql(FormateTypeCalculate.ParamEqual, item.Name));
        //            sqlField.Append(",");
        //        }
        //    }
        //    sqlField.Remove(sqlField.Length - 1, 1);

        //    //条件
        //    PropertyInfo[] propertysCondition = typeCondition.GetProperties();
        //    if (propertysCondition.Length == 0)
        //        throw new ArgumentException();

        //    StringBuilder sqlCondition = new StringBuilder();

        //    foreach (PropertyInfo item in propertysCondition)
        //    {
        //        if (item.GetCustomAttributes(typeof(IgnoreUpdateAttribute), false).Length == 0)
        //        {
        //            sqlCondition.Append(GetParamSql(FormateTypeCalculate.ParamEqual, item.Name));
        //            sqlCondition.Append(",");
        //        }
        //    }
        //    sqlField.Remove(sqlField.Length - 1, 1);

        //    return string.Format("UPDATE {0} SET {1} WHERE {2}", GetFieldName(tableName), sqlField.ToString(), GetParamSql(FormateTypeCalculate.ParamEqual, tableKey));
        //}

        /// <summary>
        /// 生成分页查询语句
        /// </summary>
        /// <param name="selectSql"></param>
        /// <param name="sortSql"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns>totalSql:查询语句 dataSql:分页语句（limit）</returns>
        public virtual (string totalSql, string dataSql) GetPagingSql(string selectSql, string sortSql, int pageNumber, int pageSize)
        {
            return ($"SELECT COUNT(*) from ({selectSql}) alias",
                $"{selectSql} {sortSql} LIMIT { pageNumber * pageSize},{pageSize}");
        }

        /// <summary>
        /// 获取自增主键查询语句
        /// </summary>
        /// <returns></returns>
        public virtual string GetAutoIncrement(string fieldName = null)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    class MySqlBuilder : SqlBuilder
    {
        /// <summary>
        /// 获取自增主键查询语句
        /// </summary>
        /// <returns></returns>
        public override string GetAutoIncrement(string fieldName = null)
        {
            return " ;Select Last_Insert_ID()";
        }
    }

    class SqlServerBuilder : SqlBuilder
    {
        public override (string totalSql, string dataSql) GetPagingSql(string selectSql, string sortSql, int pageNumber, int pageSize)
        {
            return ($"SELECT COUNT(*) from ({selectSql}) alias",
                $"{selectSql} {sortSql} offset {pageNumber * pageSize} rows fetch next { pageSize} rows only");
        }

        /// <summary>
        /// 获取自增主键查询语句
        /// </summary>
        /// <returns></returns>
        public override string GetAutoIncrement(string fieldName = null)
        {
            return " ;Select Scope_Identity()";
        }
    }

    class PostgreSqlBuilder : SqlBuilder
    {
        public override string GetParamSql(FormateTypeCalculate type, string fieldName, string paramName)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentNullException(nameof(paramName));

            string tableName = string.Empty;
            //bool containParamSign = true;
            if (fieldName.Length > 2 && fieldName[1] == '_')
            {
                //有别名
                string aliasName = fieldName.Substring(0, 1);
                fieldName = fieldName.Substring(2);
                switch (type)
                {
                    case FormateTypeCalculate.ParamEqual://等于
                        return $"{aliasName}.{SqlPrefix}{fieldName}{SqlSuffix} = {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamLess://小于
                        return $"{aliasName}.{SqlPrefix}{fieldName}{SqlSuffix} < {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamLessEqual://小于等于
                        return $"{aliasName}.{SqlPrefix}{fieldName}{SqlSuffix} <= {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamMore://大于
                        return $"{aliasName}.{SqlPrefix}{fieldName}{SqlSuffix} > {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamMoreEqual://大于等于
                        return $"{aliasName}.{SqlPrefix}{fieldName}{SqlSuffix} >= {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamLike://模糊查询
                        return $"{aliasName}.{SqlPrefix}{fieldName}{SqlSuffix} like {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamIn://In
                        return $"{aliasName}.{SqlPrefix}{fieldName}{SqlSuffix} = any({ParamSign}{paramName})";
                    case FormateTypeCalculate.ParamUnEqual://不等于
                        return $"{aliasName}.{SqlPrefix}{fieldName}{SqlSuffix} <> {ParamSign}{paramName}";
                    default: throw new ArgumentOutOfRangeException(nameof(type));
                }
            }
            else
            {
                //无别名
                switch (type)
                {
                    case FormateTypeCalculate.ParamEqual://等于
                        return $"{SqlPrefix}{fieldName}{SqlSuffix} = {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamLess://小于
                        return $"{SqlPrefix}{fieldName}{SqlSuffix} < {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamLessEqual://小于等于
                        return $"{SqlPrefix}{fieldName}{SqlSuffix} <= {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamMore://大于
                        return $"{SqlPrefix}{fieldName}{SqlSuffix} > {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamMoreEqual://大于等于
                        return $"{SqlPrefix}{fieldName}{SqlSuffix} >= {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamLike://模糊查询
                        return $"{SqlPrefix}{fieldName}{SqlSuffix} like {ParamSign}{paramName}";
                    case FormateTypeCalculate.ParamIn://In
                        return $"{SqlPrefix}{fieldName}{SqlSuffix} = any({ParamSign}{paramName})";
                    case FormateTypeCalculate.ParamUnEqual://不等于
                        return $"{SqlPrefix}{fieldName}{SqlSuffix} <> {ParamSign}{paramName}";
                    default: throw new ArgumentOutOfRangeException(nameof(type));
                }
            }
        }


        /// <summary>
        /// 参数转成条件sql（默认返回1=1）
        /// </summary>
        /// <param name="param">参数（lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="defaultSql">默认无参数时sql语句</param>
        /// <returns></returns>
        public override string GetConditionSqlByParam(object param, string defaultSql = "1=1")
        {
            if (param == null)
                return defaultSql;

            PropertyInfo[] props = param.GetType().GetProperties();
            if (props.Length == 0)
                return defaultSql;

            StringBuilder sql = new StringBuilder();
            foreach (PropertyInfo item in props)
            {
                var value = item.GetValue(param, null);
                if (value == null)
                    continue;


                if (item.Name.Length > 3)
                {
                    if (item.Name[2] == '_')//前缀
                    {

                        char start1 = item.Name[0];
                        char start2 = item.Name[1];

                        if (start1 == 'i' && start2 == 'g')
                            continue;

                        if (start1 == 's' && start2 == 'q')
                        {
                            //if (item.PropertyType != typeof(string))
                            //    throw new ArgumentException($"{item.Name}只能为string类型");

                            string ss = (value as string).Trim();
                            if (sql.Length > 0 && ss.Length > 2
                                && ss.IndexOf("AND", 0, 3, StringComparison.OrdinalIgnoreCase) < 0
                                && ss.IndexOf("OR", 0, 2, StringComparison.OrdinalIgnoreCase) < 0)
                                sql.Append(" AND ");
                            else
                                sql.Append(' ');
                            sql.Append(ss);

                            //if (!Config.Instance.Debug)//如果不是调试模式则重置参数为null
                            //    item.SetValue(param, null);

                            continue;
                        }
                    }


                    if (item.Name[item.Name.Length - 3] == '_')//后缀
                    {
                        string filterName = item.Name.Substring(0, item.Name.Length - 3);
                        char end1 = item.Name[item.Name.Length - 2];
                        char end2 = item.Name[item.Name.Length - 1];

                        if (end1 == 'l')
                        {
                            switch (end2)
                            {
                                case 't':
                                    if (sql.Length > 0)
                                        sql.Append(" AND ");
                                    sql.Append(GetParamSql(FormateTypeCalculate.ParamLess, filterName, item.Name));
                                    continue;
                                case 'e':
                                    if (sql.Length > 0)
                                        sql.Append(" AND ");
                                    sql.Append(GetParamSql(FormateTypeCalculate.ParamLessEqual, filterName, item.Name));
                                    continue;
                                case 'k':
                                    if (sql.Length > 0)
                                        sql.Append(" AND ");
                                    sql.Append(GetParamSql(FormateTypeCalculate.ParamLike, filterName, item.Name));
                                    continue;
                                    //default:
                                    //    throw new ArgumentException("暂不支持此后缀 " + item.Name.Substring(item.Name.Length - 2, 2));
                            }
                        }
                        else if (end1 == 'g')
                        {
                            switch (end2)
                            {
                                case 't':
                                    if (sql.Length > 0)
                                        sql.Append(" AND ");
                                    sql.Append(GetParamSql(FormateTypeCalculate.ParamMore, filterName, item.Name));
                                    continue;
                                case 'e':
                                    if (sql.Length > 0)
                                        sql.Append(" AND ");
                                    sql.Append(GetParamSql(FormateTypeCalculate.ParamMoreEqual, filterName, item.Name));
                                    continue;
                                    //default:
                                    //    throw new ArgumentException("暂不支持此后缀 " + item.Name.Substring(item.Name.Length - 2, 2));
                            }
                        }
                        else if ((end1 == 'u' || end1 == 'n') && end2 == 'e')
                        {
                            if (sql.Length > 0)
                                sql.Append(" AND ");

                            sql.Append(GetParamSql(FormateTypeCalculate.ParamUnEqual, filterName, item.Name));
                            continue;
                        }
                        else if (end1 == 'i' && end2 == 'g')
                            continue;
                        //else
                        //    throw new ArgumentException("暂不支持此后缀 " + item.Name.Substring(item.Name.Length - 2, 2));
                    }
                }

                //判断是否数组或List<T>,IsGenericType有bug，class<T>也返回true
                if (item.PropertyType.IsArray
                    || (item.PropertyType.IsGenericType
                    && (item.PropertyType.Name.StartsWith("List`") || item.PropertyType.Name.StartsWith("IEnumerable`"))))
                {
                    if (sql.Length > 0)
                        sql.Append(" = ");
                    sql.Append(GetParamSql(FormateTypeCalculate.ParamIn, item.Name));
                    continue;
                }

                if (sql.Length > 0)
                    sql.Append(" AND ");
                sql.Append(GetParamSql(FormateTypeCalculate.ParamEqual, item.Name));
            }

            //如果参数为0，返回默认sql
            if (sql.Length == 0)
                return defaultSql;

            return sql.ToString();
        }


        public override (string totalSql, string dataSql) GetPagingSql(string selectSql, string sortSql, int pageNumber, int pageSize)
        {
            return ($"SELECT COUNT(*) from ({selectSql}) alias",
                $"{selectSql} {sortSql} LIMIT { pageSize} OFFSET {pageNumber * pageSize}");
        }

        /// <summary>
        /// 获取自增主键查询语句
        /// </summary>
        /// <returns></returns>
        public override string GetAutoIncrement(string fieldName = null)
        {
            return $" ;Returning {fieldName ?? DefaultTableKey}";
        }
    }

    class OracleBuilder : SqlBuilder
    {
        public override string ParamSign { get; set; } = ":";//参数标识

        public override (string totalSql, string dataSql) GetPagingSql(string selectSql, string sortSql, int pageNumber, int pageSize)
        {
            return ($"SELECT COUNT(*) from ({selectSql}) alias",
                $@"SELECT * FROM (SELECT alias.*, ROWNUM RN FROM(
{selectSql} {sortSql}
) alias WHERE ROWNUM < {pageNumber * pageSize + pageSize + 1}) WHERE RN > {pageNumber * pageSize}");
        }


    }

    class SQLiteBuilder : SqlBuilder
    {
        public override string ParamSign { get; set; } = "@";//参数标识


        /// <summary>
        /// 生成分页查询语句
        /// </summary>
        /// <param name="selectSql"></param>
        /// <param name="sortSql"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns>totalSql:查询语句 dataSql:分页语句（limit）</returns>
        public override (string totalSql, string dataSql) GetPagingSql(string selectSql, string sortSql, int pageNumber, int pageSize)
        {
            return ($"SELECT COUNT(*) from ({selectSql}) alias",
                $"{selectSql} {sortSql} LIMIT { pageSize } OFFSET { pageNumber * pageSize}");
        }

        /// <summary>
        /// 获取自增主键查询语句
        /// </summary>
        /// <returns></returns>
        public override string GetAutoIncrement(string fieldName = null)
        {
            return " ;Select LAST_INSERT_ROWID()";
        }
    }
}
