using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.Sugar
{
    /// <summary>
    /// 数据访问底层类
    /// </summary>
    public partial class DbProvider
    {
        /// <summary>
        /// 联合查询分割表数据标识
        /// </summary>
        private const string SPLITON = "ID";

        #region 基础方法

        #region 查询

        /// <summary>
        /// 基础查询列表
        /// </summary>
        private IEnumerable<T> BaseQuery<T>(IDbConnection conn, string sql, object param, CommandType commandType, bool buffered, IDbTransaction transaction, int? timeout)
            where T : class
        {
            try
            {
                if (Config.Instance.LogSql)//写入日志
                    Log.InfoSql(sql, param);
                OpenConnection(conn);
                return conn.Query<T>(sql, param, transaction, buffered, timeout, commandType);
            }
            catch (Exception ex)
            {
                //if (Config.Instance.LogSql)//写入日志
                Log.ErrorSql(sql, param, ex);
                if (Config.Instance.Debug)
                    throw new Exception($"SQL命令[ {sql} ]执行出错，错误信息：{ex.Message}！", ex);
                return new List<T>(0);
            }
        }

        /// <summary>
        /// 基础查询单个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="transaction"></param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        private T BaseQueryScalar<T>(IDbConnection conn, string sql, object param, CommandType commandType, IDbTransaction transaction, int? timeout)
            where T : struct
        {
            try
            {
                if (Config.Instance.LogSql)//写入日志
                    Log.InfoSql(sql, param);
                OpenConnection(conn);
                return conn.ExecuteScalar<T>(sql, param, transaction, timeout, commandType);
            }
            catch (Exception ex)
            {
                Log.ErrorSql(sql, param, ex);
                if (Config.Instance.Debug)
                    throw new Exception($"SQL命令[ {sql} ]执行出错，错误信息：{ex.Message}！", ex);
                return default(T);
            }
        }

        /// <summary>
        /// 基础查询列表(1)
        /// </summary>
        private IEnumerable<T> QueryData<T>(IDbConnection conn, string sql, object param, SugarCommandType commandType, string sortSql, bool buffered, IDbTransaction transaction, int? timeout)
            where T : class
        {
            var (SqlText, CommandType) = TranslateSelectSql(sql, param, commandType, sortSql);

            return BaseQuery<T>(conn, SqlText, param, CommandType, buffered, transaction, timeout);
        }

        /// <summary>
        /// 基础连表查询列表(2)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="conn"></param>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql"></param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered"></param>
        /// <param name="transaction"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private IEnumerable<TReturn> QueryData<TFirst, TSecond, TReturn>(IDbConnection conn, Func<TFirst, TSecond, TReturn> map, string sql, object param, SugarCommandType commandType, string sortSql, string splitOn, bool buffered, IDbTransaction transaction, int? timeout)
            where TFirst : class
            where TSecond : class
            where TReturn : class
        {
            var (SqlText, CommandType) = TranslateSelectSql(sql, param, commandType, sortSql);

            try
            {
                if (Config.Instance.LogSql)//写入日志
                    Log.InfoSql(sql, param);
                OpenConnection(conn);
                return conn.Query<TFirst, TSecond, TReturn>(SqlText, map, param, transaction, buffered, splitOn ?? SPLITON, timeout, CommandType);
            }
            catch (Exception ex)
            {
                Log.ErrorSql(SqlText, param, ex);
                if (Config.Instance.Debug)
                    throw new Exception($"SQL命令[ {SqlText} ]执行出错，错误信息：{ex.Message}！", ex);
                return new List<TReturn>(0);
            }
        }

        /// <summary>
        /// 基础连表查询列表(3)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="conn"></param>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql"></param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered"></param>
        /// <param name="transaction"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private IEnumerable<TReturn> QueryData<TFirst, TSecond, TThird, TReturn>(IDbConnection conn, Func<TFirst, TSecond, TThird, TReturn> map, string sql, object param, SugarCommandType commandType, string sortSql, string splitOn, bool buffered, IDbTransaction transaction, int? timeout)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TReturn : class
        {
            var (SqlText, CommandType) = TranslateSelectSql(sql, param, commandType, sortSql);

            try
            {
                if (Config.Instance.LogSql)//写入日志
                    Log.InfoSql(sql, param);
                OpenConnection(conn);
                return conn.Query<TFirst, TSecond, TThird, TReturn>(SqlText, map, param, transaction, buffered, splitOn ?? SPLITON, timeout, CommandType);
            }
            catch (Exception ex)
            {
                Log.ErrorSql(SqlText, param, ex);
                if (Config.Instance.Debug)
                    throw new Exception($"SQL命令[ {SqlText} ]执行出错，错误信息：{ex.Message}！", ex);
                return new List<TReturn>(0);
            }
        }

        /// <summary>
        /// 基础连表查询列表(4)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TFourth">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="conn"></param>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql"></param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered"></param>
        /// <param name="transaction"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private IEnumerable<TReturn> QueryData<TFirst, TSecond, TThird, TFourth, TReturn>(IDbConnection conn, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string sql, object param, SugarCommandType commandType, string sortSql, string splitOn, bool buffered, IDbTransaction transaction, int? timeout)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TFourth : class
            where TReturn : class
        {
            var (SqlText, CommandType) = TranslateSelectSql(sql, param, commandType, sortSql);

            try
            {
                if (Config.Instance.LogSql)//写入日志
                    Log.InfoSql(sql, param);
                OpenConnection(conn);
                return conn.Query<TFirst, TSecond, TThird, TFourth, TReturn>(SqlText, map, param, transaction, buffered, splitOn ?? SPLITON, timeout, CommandType);
            }
            catch (Exception ex)
            {
                Log.ErrorSql(SqlText, param, ex);
                if (Config.Instance.Debug)
                    throw new Exception($"SQL命令[ {SqlText} ]执行出错，错误信息：{ex.Message}！", ex);
                return new List<TReturn>(0);
            }
        }

        /// <summary>
        /// 基础连表查询列表(5)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TFourth">数据实体</typeparam>
        /// <typeparam name="TFifth">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="conn"></param>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql"></param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered"></param>
        /// <param name="transaction"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private IEnumerable<TReturn> QueryData<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(IDbConnection conn, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string sql, object param, SugarCommandType commandType, string sortSql, string splitOn, bool buffered, IDbTransaction transaction, int? timeout)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TFourth : class
            where TFifth : class
            where TReturn : class
        {
            var (SqlText, CommandType) = TranslateSelectSql(sql, param, commandType, sortSql);

            try
            {
                if (Config.Instance.LogSql)//写入日志
                    Log.InfoSql(sql, param);
                OpenConnection(conn);
                return conn.Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(SqlText, map, param, transaction, buffered, splitOn ?? SPLITON, timeout, CommandType);
            }
            catch (Exception ex)
            {
                Log.ErrorSql(SqlText, param, ex);
                if (Config.Instance.Debug)
                    throw new Exception($"SQL命令[ {SqlText} ]执行出错，错误信息：{ex.Message}！", ex);
                return new List<TReturn>(0);
            }
        }

        /// <summary>
        /// 基础分页查询列表（limit分页）
        /// </summary>
        private IPagingList<T> QueryPagingData<T>(IDbConnection conn, int pageNumber, int pageSize, string sql, object param, SugarCommandType commandType, string sortSql, bool buffered, IDbTransaction transaction, int? timeout)
            where T : class
        {
            if (commandType == SugarCommandType.StoredProcedure)
                throw new ArgumentException("commandType参数不接受StoredProcedure！");

            var (SqlText, CommandType) = TranslateSelectSql(sql, param, commandType, sortSql);

            var (TotalSql, DataSql) = this.Builder.GetPagingSql(SqlText, pageNumber, pageSize);

            var total = BaseQueryScalar<int>(conn, TotalSql, param, CommandType.Text, transaction, timeout);

            if (total == 0)
            {
                return new PagingList<T>(new List<T>(0), 0);
            }

            List<T> result = BaseQuery<T>(conn, DataSql, param, CommandType.Text, buffered, transaction, timeout).ToList();

            return new PagingList<T>(result, total);
        }

        #endregion

        #region 工具

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <param name="conn"></param>
        private void OpenConnection(IDbConnection conn)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            else if (conn.State == ConnectionState.Broken)
            {
                conn.Close();
                conn.Open();
            }
        }


        /// <summary>
        /// 分页集合
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="pageNumber">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="resource">集合</param>
        /// <returns></returns>
        public IPagingList<T> PagingList<T>(int pageNumber, int pageSize, IEnumerable<T> resource)
        {
            PagingList<T> result = new PagingList<T>()
            {
                Total = resource.Count(),
            };

            if (result.Total > 0)
                result.List = resource.Skip<T>(pageNumber * pageSize).Take<T>(pageSize).ToList();
            else
                result.List = new List<T>(0);
            return result;
        }

        /// <summary>
        /// 转换sql命令
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private void TranslateCommand(CommandInfo info)
        {
            switch (info.CommandType)
            {
                case SugarCommandType.Text:
                    break;
                case SugarCommandType.StoredProcedure:
                    break;
                case SugarCommandType.AddTableDirect:
                    info.SqlText = this.Builder.GetInsertSql(info.SqlText, info.Param);
                    //info.CommandType = SugarCommandType.Text;
                    break;
                case SugarCommandType.UpdateTableDirect:
                    info.SqlText = this.Builder.GetUpdateSql(info.SqlText, info.Param);
                    //info.CommandType = SugarCommandType.Text;
                    break;
                case SugarCommandType.QuerySelectSql:
                    info.SqlText = this.Builder.GetSelectSqlFromSelectSql(info.SqlText, info.Param);
                    break;
                case SugarCommandType.QueryTableDirect:
                    info.SqlText = this.Builder.GetSelectSqlFromTableDirect(info.SqlText, info.Param);
                    //info.CommandType = SugarCommandType.Text;
                    break;
                default: throw new ArgumentOutOfRangeException("commandType");
            }
        }

        /// <summary>
        /// 转换sql命令
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="sortSql"></param>
        /// <returns></returns>
        private (string SqlText, System.Data.CommandType CommandType) TranslateSelectSql(string sql, object param, SugarCommandType commandType, string sortSql)
        {
            switch (commandType)
            {
                case SugarCommandType.QuerySelectSql:
                    return (this.Builder.GetSelectSqlFromSelectSql(sql, param, sortSql), System.Data.CommandType.Text);
                case SugarCommandType.QueryTableDirect://通过参数动态生成sql
                    return (this.Builder.GetSelectSqlFromTableDirect(sql, param, sortSql), System.Data.CommandType.Text);
                case SugarCommandType.Text://直接sql
                    return (string.IsNullOrEmpty(sortSql) ? sql : $"{sql} {sortSql}", System.Data.CommandType.Text);
                case SugarCommandType.StoredProcedure://数据库
                    return (sql, CommandType.StoredProcedure);
                default:
                    throw new ArgumentException("commandType参数错误！");
            }
        }

        /// <summary>
        /// 查询（以Dataset返回结果的）
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parms">参数</param>
        /// <returns></returns>
        public DataSet Query(DbConnection conn, string sql, params DbParameter[] parms)
        {
            DbCommand cmd = this.Factory.CreateCommand();
            cmd.CommandText = sql;
            cmd.Connection = conn;
            foreach (DbParameter param in parms)
            {
                cmd.Parameters.Add(param);
            }
            using (DbDataAdapter da = this.Factory.CreateDataAdapter())
            {
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                try
                {
                    if (Config.Instance.LogSql)//写入日志
                        Log.InfoSql(sql, parms);
                    da.Fill(ds);
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    Log.ErrorSql(sql, parms, ex);
                    if (Config.Instance.Debug)
                        throw new Exception($"SQL命令[ {sql} ]执行出错，错误信息：{ex.Message}！", ex);
                    return null;
                }
                return ds;
            }
        }

        /// <summary>
        /// 查询自增
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="fieldName">字段名称（仅限PostgreSql需要）</param>
        /// <returns></returns>
        public long QueryAutoIncrement(IDbConnection conn, string fieldName = null)
        {
            return QueryScalar<long>(conn, this.Builder.GetAutoIncrement(fieldName), null, SugarCommandType.Text);
        }

        #endregion

        #endregion

        #region 查询

        #region 查询单个值

        /// <summary>
        /// 查询单个数值（如存在多个取首行首列）
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="conn">连接</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public T QueryScalar<T>(IDbConnection conn, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, IDbTransaction transaction = null, int? timeout = null)
            where T : struct
        {
            var (SqlText, CommandType) = TranslateSelectSql(sql, param, commandType, sortSql);

            return BaseQueryScalar<T>(conn, SqlText, param, CommandType, transaction, timeout);
        }

        #endregion

        #region 查询单个实体

        /// <summary>
        /// 查询单个实体
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="conn">连接</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public T QuerySingle<T>(IDbConnection conn, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where T : class
        {
            return QueryData<T>(conn, sql, param, commandType, sortSql, buffered, transaction, timeout).FirstOrDefault<T>();
        }

        /// <summary>
        /// 连表查询单个实体(2)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="conn">连接</param>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public TReturn QuerySingle<TFirst, TSecond, TReturn>(IDbConnection conn, Func<TFirst, TSecond, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TReturn : class
        {
            return QueryData<TFirst, TSecond, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, transaction, timeout).FirstOrDefault<TReturn>();
        }

        /// <summary>
        /// 连表查询单个实体(3)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="conn">连接</param>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public TReturn QuerySingle<TFirst, TSecond, TThird, TReturn>(IDbConnection conn, Func<TFirst, TSecond, TThird, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TReturn : class
        {
            return QueryData<TFirst, TSecond, TThird, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, transaction, timeout).FirstOrDefault<TReturn>();
        }

        /// <summary>
        /// 连表查询单个实体(4)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TFourth">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="conn">连接</param>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public TReturn QuerySingle<TFirst, TSecond, TThird, TFourth, TReturn>(IDbConnection conn, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TFourth : class
            where TReturn : class
        {
            return QueryData<TFirst, TSecond, TThird, TFourth, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, transaction, timeout).FirstOrDefault<TReturn>();
        }

        /// <summary>
        /// 连表查询单个实体(5)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TFourth">数据实体</typeparam>
        /// <typeparam name="TFifth">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="conn">连接</param>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public TReturn QuerySingle<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(IDbConnection conn, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TFourth : class
            where TFifth : class
            where TReturn : class
        {
            return QueryData<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, transaction, timeout).FirstOrDefault<TReturn>();
        }

        #endregion

        #region 查询多个实体

        /// <summary>
        /// 查询列表(多个model)
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="conn">连接</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public IEnumerable<T> QueryList<T>(IDbConnection conn, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where T : class
        {
            return QueryData<T>(conn, sql, param, commandType, sortSql, buffered, transaction, timeout).ToList();
        }

        /// <summary>
        /// 连表查询列表(2)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="conn">连接</param>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public IEnumerable<TReturn> QueryList<TFirst, TSecond, TReturn>(IDbConnection conn, Func<TFirst, TSecond, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TReturn : class
        {
            return QueryData<TFirst, TSecond, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, transaction, timeout).ToList();
        }

        /// <summary>
        /// 连表查询列表(3)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="conn">连接</param>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public IEnumerable<TReturn> QueryList<TFirst, TSecond, TThird, TReturn>(IDbConnection conn, Func<TFirst, TSecond, TThird, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TReturn : class
        {
            return QueryData<TFirst, TSecond, TThird, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, transaction, timeout).ToList();
        }

        /// <summary>
        /// 连表查询列表(4)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TFourth">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="conn">连接</param>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public IEnumerable<TReturn> QueryList<TFirst, TSecond, TThird, TFourth, TReturn>(IDbConnection conn, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TFourth : class
            where TReturn : class
        {
            return QueryData<TFirst, TSecond, TThird, TFourth, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, transaction, timeout).ToList();
        }

        /// <summary>
        /// 连表查询列表(5)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TFourth">数据实体</typeparam>
        /// <typeparam name="TFifth">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="conn">连接</param>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public IEnumerable<TReturn> QueryList<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(IDbConnection conn, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TFourth : class
            where TFifth : class
            where TReturn : class
        {
            return QueryData<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, transaction, timeout).ToList();
        }

        #endregion

        #region 分页查询列表

        /// <summary>
        /// 分页查询列表(1) - 内存分页
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="conn">连接</param>
        /// <param name="pageNumber">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public IPagingList<T> QueryPagingList<T>(IDbConnection conn, int pageNumber, int pageSize, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where T : class
        {
            var list = QueryData<T>(conn, sql, param, commandType, sortSql, buffered, transaction, timeout);
            return PagingList(pageNumber, pageSize, list);
        }

        /// <summary>
        /// 分页查询列表(1) - limi分页
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="conn">连接</param>
        /// <param name="pageNumber">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public IPagingList<T> QueryPagingList2<T>(IDbConnection conn, int pageNumber, int pageSize, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where T : class
        {
            return QueryPagingData<T>(conn, pageNumber, pageSize, sql, param, commandType, sortSql, buffered, transaction, timeout);
        }

        ///// <summary>
        ///// 连表分页查询列表(2)
        ///// </summary>
        ///// <typeparam name="TFirst">数据实体</typeparam>
        ///// <typeparam name="TSecond">数据实体</typeparam>
        ///// <typeparam name="TReturn">返回数据实体</typeparam>
        ///// <param name="conn">连接</param>
        ///// <param name="pageNumber">当前页</param>
        ///// <param name="pageSize">每页记录数</param>
        ///// <param name="sql">sql语句</param>
        ///// <param name="map">委托-两个表数据逻辑处理</param>
        ///// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        ///// <param name="splitOn">分割两表数据的列名称</param>
        ///// <param name="commandType">命令类型</param>
        ///// <returns></returns>
        //public  PagingList<TReturn> QueryPagingList<TFirst, TSecond, TReturn>(IDbConnection conn, int pageNumber, int pageSize, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, string splitOn = SPLITON, CommandType? commandType = null)
        //    where TFirst : class
        //    where TSecond : class
        //    where TReturn : class
        //{
        //    PagingList<TReturn> result = new PagingList<TReturn>();
        //    IEnumerable<TReturn> list = QueryData<TFirst, TSecond, TReturn>(conn, sql, map, param, splitOn, commandType);

        //    result.Total = list.Count();
        //    if (result.Total > 0)
        //        result.List = list.Skip<TReturn>(pageNumber * pageSize).Take<TReturn>(pageSize).ToList();
        //    return result;
        //}

        ///// <summary>
        ///// 连表分页查询列表(2)
        ///// </summary>
        ///// <typeparam name="TFirst">数据实体</typeparam>
        ///// <typeparam name="TSecond">数据实体</typeparam>
        ///// <typeparam name="TReturn">返回数据实体</typeparam>
        ///// <param name="pageNumber">当前页</param>
        ///// <param name="pageSize">每页记录数</param>
        ///// <param name="sql">sql语句</param>
        ///// <param name="map">委托-两个表数据逻辑处理</param>
        ///// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        ///// <param name="splitOn">分割两表数据的列名称</param>
        ///// <param name="commandType">命令类型</param>
        ///// <returns></returns>
        //public  PagingList<TReturn> QueryPagingList<TFirst, TSecond, TReturn>(int pageNumber, int pageSize, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, string splitOn = SPLITON, CommandType? commandType = null)
        //    where TFirst : class
        //    where TSecond : class
        //    where TReturn : class
        //{
        //    PagingList<TReturn> result = new PagingList<TReturn>();
        //    IEnumerable<TReturn> list = null;
        //    using (DbConnection conn = defaultthis.CreateConnection(Config.DataBaseAuthority.Read))
        //    {
        //        list = QueryData<TFirst, TSecond, TReturn>(conn, sql, map, param, splitOn, commandType);
        //    }
        //    result.Total = list.Count();
        //    if (result.Total > 0)
        //        result.List = list.Skip<TReturn>(pageNumber * pageSize).Take<TReturn>(pageSize).ToList();
        //    return result;
        //}

        ///// <summary>
        ///// 连表分页查询列表(3)
        ///// </summary>
        ///// <typeparam name="TFirst">数据实体</typeparam>
        ///// <typeparam name="TSecond">数据实体</typeparam>
        ///// <typeparam name="TThird">数据实体</typeparam>
        ///// <typeparam name="TReturn">返回数据实体</typeparam>
        ///// <param name="conn">连接</param>
        ///// <param name="pageNumber">当前页</param>
        ///// <param name="pageSize">每页记录数</param>
        ///// <param name="sql">sql语句</param>
        ///// <param name="map">委托-两个表数据逻辑处理</param>
        ///// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        ///// <param name="splitOn">分割两表数据的列名称</param>
        ///// <param name="commandType">命令类型</param>
        ///// <returns></returns>
        //public  PagingList<TReturn> QueryPagingList<TFirst, TSecond, TThird, TReturn>(IDbConnection conn, int pageNumber, int pageSize, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, string splitOn = SPLITON, CommandType? commandType = null)
        //    where TFirst : class
        //    where TSecond : class
        //    where TThird : class
        //    where TReturn : class
        //{
        //    PagingList<TReturn> result = new PagingList<TReturn>();
        //    IEnumerable<TReturn> list = QueryData<TFirst, TSecond, TThird, TReturn>(conn, sql, map, param, splitOn, commandType);

        //    result.Total = list.Count();
        //    if (result.Total > 0)
        //        result.List = list.Skip<TReturn>(pageNumber * pageSize).Take<TReturn>(pageSize).ToList();
        //    return result;
        //}

        ///// <summary>
        ///// 连表分页查询列表(3)
        ///// </summary>
        ///// <typeparam name="TFirst">数据实体</typeparam>
        ///// <typeparam name="TSecond">数据实体</typeparam>
        ///// <typeparam name="TThird">数据实体</typeparam>
        ///// <typeparam name="TReturn">返回数据实体</typeparam>
        ///// <param name="pageNumber">当前页</param>
        ///// <param name="pageSize">每页记录数</param>
        ///// <param name="sql">sql语句</param>
        ///// <param name="map">委托-两个表数据逻辑处理</param>
        ///// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        ///// <param name="splitOn">分割两表数据的列名称</param>
        ///// <param name="commandType">命令类型</param>
        ///// <returns></returns>
        //public  PagingList<TReturn> QueryPagingList<TFirst, TSecond, TThird, TReturn>(int pageNumber, int pageSize, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, string splitOn = SPLITON, CommandType? commandType = null)
        //    where TFirst : class
        //    where TSecond : class
        //    where TThird : class
        //    where TReturn : class
        //{
        //    PagingList<TReturn> result = new PagingList<TReturn>();
        //    IEnumerable<TReturn> list = null;
        //    using (DbConnection conn = defaultthis.CreateConnection(Config.DataBaseAuthority.Read))
        //    {
        //        list = QueryData<TFirst, TSecond, TThird, TReturn>(conn, sql, map, param, splitOn, commandType);
        //    }
        //    result.Total = list.Count();
        //    if (result.Total > 0)
        //        result.List = list.Skip<TReturn>(pageNumber * pageSize).Take<TReturn>(pageSize).ToList();
        //    return result;
        //}

        ///// <summary>
        ///// 连表分页查询列表(4)
        ///// </summary>
        ///// <typeparam name="TFirst">数据实体</typeparam>
        ///// <typeparam name="TSecond">数据实体</typeparam>
        ///// <typeparam name="TThird">数据实体</typeparam>
        ///// <typeparam name="TFourth">数据实体</typeparam>
        ///// <typeparam name="TReturn">返回数据实体</typeparam>
        ///// <param name="conn">连接</param>
        ///// <param name="pageNumber">当前页</param>
        ///// <param name="pageSize">每页记录数</param>
        ///// <param name="sql">sql语句</param>
        ///// <param name="map">委托-两个表数据逻辑处理</param>
        ///// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        ///// <param name="splitOn">分割两表数据的列名称</param>
        ///// <param name="commandType">命令类型</param>
        ///// <returns></returns>
        //public  PagingList<TReturn> QueryPagingList<TFirst, TSecond, TThird, TFourth, TReturn>(IDbConnection conn, int pageNumber, int pageSize, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, string splitOn = SPLITON, CommandType? commandType = null)
        //    where TFirst : class
        //    where TSecond : class
        //    where TThird : class
        //    where TFourth : class
        //    where TReturn : class
        //{
        //    PagingList<TReturn> result = new PagingList<TReturn>();
        //    IEnumerable<TReturn> list = QueryData<TFirst, TSecond, TThird, TFourth, TReturn>(conn, sql, map, param, splitOn, commandType);

        //    result.Total = list.Count();
        //    if (result.Total > 0)
        //        result.List = list.Skip<TReturn>(pageNumber * pageSize).Take<TReturn>(pageSize).ToList();
        //    return result;
        //}

        ///// <summary>
        ///// 连表分页查询列表(4)
        ///// </summary>
        ///// <typeparam name="TFirst">数据实体</typeparam>
        ///// <typeparam name="TSecond">数据实体</typeparam>
        ///// <typeparam name="TThird">数据实体</typeparam>
        ///// <typeparam name="TFourth">数据实体</typeparam>
        ///// <typeparam name="TReturn">返回数据实体</typeparam>
        ///// <param name="pageNumber">当前页</param>
        ///// <param name="pageSize">每页记录数</param>
        ///// <param name="sql">sql语句</param>
        ///// <param name="map">委托-两个表数据逻辑处理</param>
        ///// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        ///// <param name="splitOn">分割两表数据的列名称</param>
        ///// <param name="commandType">命令类型</param>
        ///// <returns></returns>
        //public  PagingList<TReturn> QueryPagingList<TFirst, TSecond, TThird, TFourth, TReturn>(int pageNumber, int pageSize, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, string splitOn = SPLITON, CommandType? commandType = null)
        //    where TFirst : class
        //    where TSecond : class
        //    where TThird : class
        //    where TFourth : class
        //    where TReturn : class
        //{
        //    PagingList<TReturn> result = new PagingList<TReturn>();
        //    IEnumerable<TReturn> list = null;
        //    using (DbConnection conn = defaultthis.CreateConnection(Config.DataBaseAuthority.Read))
        //    {
        //        list = QueryData<TFirst, TSecond, TThird, TFourth, TReturn>(conn, sql, map, param, splitOn, commandType);
        //    }
        //    result.Total = list.Count();
        //    if (result.Total > 0)
        //        result.List = list.Skip<TReturn>(pageNumber * pageSize).Take<TReturn>(pageSize).ToList();
        //    return result;
        //}

        ///// <summary>
        ///// 连表分页查询列表(5)
        ///// </summary>
        ///// <typeparam name="TFirst">数据实体</typeparam>
        ///// <typeparam name="TSecond">数据实体</typeparam>
        ///// <typeparam name="TThird">数据实体</typeparam>
        ///// <typeparam name="TFourth">数据实体</typeparam>
        ///// <typeparam name="TFifth">数据实体</typeparam>
        ///// <typeparam name="TReturn">返回数据实体</typeparam>
        ///// <param name="conn">连接</param>
        ///// <param name="pageNumber">当前页</param>
        ///// <param name="pageSize">每页记录数</param>
        ///// <param name="sql">sql语句</param>
        ///// <param name="map">委托-两个表数据逻辑处理</param>
        ///// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        ///// <param name="splitOn">分割两表数据的列名称</param>
        ///// <param name="commandType">命令类型</param>
        ///// <returns></returns>
        //public  PagingList<TReturn> QueryPagingList<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(IDbConnection conn, int pageNumber, int pageSize, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null, string splitOn = SPLITON, CommandType? commandType = null)
        //    where TFirst : class
        //    where TSecond : class
        //    where TThird : class
        //    where TFourth : class
        //    where TFifth : class
        //    where TReturn : class
        //{
        //    PagingList<TReturn> result = new PagingList<TReturn>();
        //    IEnumerable<TReturn> list = QueryData<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(conn, sql, map, param, splitOn, commandType);

        //    result.Total = list.Count();
        //    if (result.Total > 0)
        //        result.List = list.Skip<TReturn>(pageNumber * pageSize).Take<TReturn>(pageSize).ToList();
        //    return result;
        //}

        ///// <summary>
        ///// 连表分页查询列表(5)
        ///// </summary>
        ///// <typeparam name="TFirst">数据实体</typeparam>
        ///// <typeparam name="TSecond">数据实体</typeparam>
        ///// <typeparam name="TThird">数据实体</typeparam>
        ///// <typeparam name="TFourth">数据实体</typeparam>
        ///// <typeparam name="TFifth">数据实体</typeparam>
        ///// <typeparam name="TReturn">返回数据实体</typeparam>
        ///// <param name="pageNumber">当前页</param>
        ///// <param name="pageSize">每页记录数</param>
        ///// <param name="sql">sql语句</param>
        ///// <param name="map">委托-两个表数据逻辑处理</param>
        ///// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        ///// <param name="splitOn">分割两表数据的列名称</param>
        ///// <param name="commandType">命令类型</param>
        ///// <returns></returns>
        //public  PagingList<TReturn> QueryPagingList<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(int pageNumber, int pageSize, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null, string splitOn = SPLITON, CommandType? commandType = null)
        //    where TFirst : class
        //    where TSecond : class
        //    where TThird : class
        //    where TFourth : class
        //    where TFifth : class
        //    where TReturn : class
        //{
        //    PagingList<TReturn> result = new PagingList<TReturn>();
        //    IEnumerable<TReturn> list = null;
        //    using (DbConnection conn = defaultthis.CreateConnection(Config.DataBaseAuthority.Read))
        //    {
        //        list = QueryData<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(conn, sql, map, param, splitOn, commandType);
        //    }
        //    result.Total = list.Count();
        //    if (result.Total > 0)
        //        result.List = list.Skip<TReturn>(pageNumber * pageSize).Take<TReturn>(pageSize).ToList();
        //    return result;
        //}

        #endregion

        #endregion

        #region 操作

        /// <summary>
        /// 执行命令(返回影响行数，-1为执行失败)
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public int ExecuteSql(IDbConnection conn, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, IDbTransaction transaction = null, int? timeout = null)
        {
            return ExecuteSql(conn, new CommandInfo(sql, param, commandType, timeout), transaction);
        }

        ///// <summary>
        ///// 异步执行命令(返回影响行数，-1为执行失败)
        ///// </summary>
        ///// <param name="conn">连接</param>
        ///// <param name="sql">sql语句</param>
        ///// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        ///// <param name="commandType">命令类型</param>
        ///// <param name="transaction">事务</param>
        ///// <param name="timeout">过期时间（秒）</param>
        ///// <returns></returns>
        //public Task<int> ExecuteSqlAsync(IDbConnection conn, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, IDbTransaction transaction = null, int? timeout = null)
        //{
        //    return ExecuteSqlAsync(conn, new CommandInfo(sql, param, commandType, timeout), transaction);
        //}

        /// <summary>
        /// 执行命令(返回影响行数，-1为执行失败)
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="command">命令</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        public int ExecuteSql(IDbConnection conn, CommandInfo command, IDbTransaction transaction = null)
        {
            /*(string SqlText, CommandType CommandType) cmd =*/
            this.TranslateCommand(command);

            try
            {
                if (Config.Instance.LogSql)//写入日志
                    if (command.CommandType == SugarCommandType.StoredProcedure)
                        Log.InfoProcedure(command.SqlText, command.Param);
                    else
                        Log.InfoSql(command.SqlText, command.Param);
                OpenConnection(conn);
                return conn.Execute(command.SqlText, command.Param, transaction, command.Timeout,
                    command.CommandType == SugarCommandType.StoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
            }
            catch (Exception ex)
            {
                //if (Config.Instance.LogSql)//写入日志
                if (command.CommandType == SugarCommandType.StoredProcedure)
                    Log.ErrorProcedure(command.SqlText, command.Param, ex);
                else
                    Log.ErrorSql(command.SqlText, command.Param, ex);
                if (Config.Instance.Debug)
                {
                    throw new Exception($"{(command.CommandType == SugarCommandType.StoredProcedure ? "Stored Procedure：" : "Sql：")}[ {command.SqlText} ]执行出错，错误信息：{ex.Message}！", ex);
                }
                return -1;
            }
        }

        ///// <summary>
        ///// 异步执行命令(返回影响行数，-1为执行失败)
        ///// </summary>
        ///// <param name="conn">连接</param>
        ///// <param name="command">命令</param>
        ///// <param name="transaction">事务</param>
        ///// <returns></returns>
        //public Task<int> ExecuteSqlAsync(IDbConnection conn, CommandInfo command, IDbTransaction transaction = null)
        //{
        //    /*(string SqlText, CommandType CommandType) cmd =*/
        //    this.TranslateCommand(command);

        //    try
        //    {
        //        if (Config.Instance.LogSql)//写入日志
        //            if (command.CommandType == SugarCommandType.StoredProcedure)
        //                Log.InfoProcedure(command.SqlText, command.Param);
        //            else
        //                Log.InfoSql(command.SqlText, command.Param);
        //        OpenConnection(conn);
        //        return conn.ExecuteAsync(command.SqlText, command.Param, transaction, command.Timeout,
        //            command.CommandType == SugarCommandType.StoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
        //    }
        //    catch (Exception ex)
        //    {
        //        //if (Config.Instance.LogSql)//写入日志
        //        if (command.CommandType == SugarCommandType.StoredProcedure)
        //            Log.ErrorProcedure(command.SqlText, command.Param, ex);
        //        else
        //            Log.ErrorSql(command.SqlText, command.Param, ex);
        //        if (Config.Instance.Debug)
        //        {
        //            throw new Exception($"{(command.CommandType == SugarCommandType.StoredProcedure ? "Stored Procedure：" : "Sql：")}[ {command.SqlText} ]执行出错，错误信息：{ex.Message}！", ex);
        //        }

        //        return new Task<int>(() =>
        //        {
        //            return -1;
        //        });
        //    }
        //}

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="commands">命令集合</param>
        /// <returns></returns>
        public bool ExecuteSqlTran(CommandCollection commands)
        {
            if (commands.Count <= 0)
                throw new Exception("commands数目为0");

            using (var conn = this.CreateConnection(Config.DataBaseAuthority.Write))
            {
                int i = 0;
                IDbTransaction trans = null;
                //(string SqlText, CommandType CommandType)? command = null;
                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    for (; i < commands.Count; i++)
                    {
                        /*command = */
                        this.TranslateCommand(commands[i]);

                        //写入日志
                        if (Config.Instance.LogSql)
                            Log.InfoSql(commands[i].SqlText, commands[i].Param);
                        //conn.Execute(CommandDefinition)
                        int affected_rows = conn.Execute(commands[i].SqlText, commands[i].Param, trans, commands[i].Timeout,
                            commands[i].CommandType == SugarCommandType.StoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
                        //检测影响行数大于0
                        if (commands[i].EffectRows == -1)
                        {
                            if (affected_rows == 0)
                            {
                                if (Config.Instance.Debug)
                                    throw new Exception("执行语句影响行数为0");
                                else
                                {
                                    if (trans != null)
                                        trans.Rollback();
                                    Log.InfoSql($"第[ {i} ]条SQL命令[ {commands[i].SqlText} ]执行出错，错误信息：执行语句影响行数为0", commands[i].Param);
                                    return false;
                                }
                            }
                        }
                        else if (commands[i].EffectRows > 0)
                        {
                            if (affected_rows != commands[i].EffectRows)
                            {
                                if (Config.Instance.Debug)
                                    throw new Exception($"执行语句影响行数为{affected_rows}，不等于影响行数{commands[i].EffectRows}的设定");
                                else
                                {
                                    if (trans != null)
                                        trans.Rollback();
                                    Log.InfoSql($"第[ {i} ]条SQL命令[ {commands[i].SqlText} ]执行出错，错误信息：执行语句影响行数为{affected_rows}，不等于影响行数{commands[i].EffectRows}的设定", commands[i].Param);
                                    return false;
                                }
                            }
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    //出现异常，事务Rollback
                    if (trans != null)
                        trans.Rollback();
                    Log.ErrorSql($"第[ {i} ]条SQL命令[ {commands[i].SqlText} ]执行出错，错误信息：{ex.Message}", commands[i].Param, ex);
                    if (Config.Instance.Debug)
                        throw new Exception($"第[ {i} ]条SQL命令[ {commands[i].SqlText} ]执行出错，错误信息：{ex.Message}", ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="runFun"></param>
        /// <returns></returns>
        public bool ExecuteSqlTran(Func<DbConnection, DbTransaction, bool> runFun)
        {
            using (var conn = this.CreateConnection(Config.DataBaseAuthority.Write))
            {
                DbTransaction trans = null;
                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction();

                    if (runFun(conn, trans))
                    {
                        trans.Commit();
                        return true;
                    }
                    else
                    {
                        trans.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    //出现异常，事务Rollback
                    if (trans != null)
                        trans.Rollback();
                    Log.Error($"事务执行出错，错误信息：{ex.Message}", ex);
                    if (Config.Instance.Debug)
                        throw new Exception($"事务执行出错，错误信息：{ex.Message}", ex);
                    return false;
                }
            }
        }

        #endregion
    }
}