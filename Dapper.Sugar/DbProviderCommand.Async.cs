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

        #region 基础方法

        #region 查询

        /// <summary>
        /// 基础查询列表
        /// </summary>
        private async Task<IEnumerable<T>> BaseQueryAsync<T>(DbConnection conn, string sql, object param, CommandType commandType, bool buffered, IDbTransaction transaction, int? timeout)
        //where T : class
        {
            try
            {
                if (Config.Instance.LogSql)//写入日志
                    Log.InfoSql(sql, param);
                // await OpenConnectionAsync(conn);
                return await conn.QueryAsync<T>(new CommandDefinition(sql, param, transaction, timeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //if (Config.Instance.LogSql)//写入日志
                Log.ErrorSql(sql, param, ex);
                if (Config.Instance.Debug)
                    throw new DapperSugarException($"SQL命令[ {sql} ]执行出错，错误信息：{ex.Message}！", ex);
                else
                {
                    ExceptionCallBack?.Invoke(ex);
                    return new List<T>(0);
                }
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
        private async Task<T> BaseQueryScalarAsync<T>(DbConnection conn, string sql, object param, CommandType commandType, IDbTransaction transaction, int? timeout)
        //where T : struct
        {
            try
            {
                if (Config.Instance.LogSql)//写入日志
                    Log.InfoSql(sql, param);
                // await OpenConnectionAsync(conn);
                return await conn.ExecuteScalarAsync<T>(sql, param, transaction, timeout, commandType).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.ErrorSql(sql, param, ex);
                if (Config.Instance.Debug)
                    throw new DapperSugarException($"SQL命令[ {sql} ]执行出错，错误信息：{ex.Message}！", ex);
                else
                {
                    ExceptionCallBack?.Invoke(ex);
                    return default(T);
                }
            }
        }

        /// <summary>
        /// 基础查询列表(1)
        /// </summary>
        private Task<IEnumerable<T>> QueryDataAsync<T>(DbConnection conn, string sql, object param, SugarCommandType commandType, string sortSql, bool buffered, IDbTransaction transaction, int? timeout)
        //where T : class
        {
            var (SqlText, CommandType) = TranslateSelectSql(sql, param, commandType, sortSql);

            return BaseQueryAsync<T>(conn, SqlText, param, CommandType, buffered, transaction, timeout);
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
        private Task<IEnumerable<TReturn>> QueryDataAsync<TFirst, TSecond, TReturn>(DbConnection conn, Func<TFirst, TSecond, TReturn> map, string sql, object param, SugarCommandType commandType, string sortSql, string splitOn, bool buffered, IDbTransaction transaction, int? timeout)
        //where TFirst : class
        //where TSecond : class
        //where TReturn : class
        {
            var (SqlText, CommandType) = TranslateSelectSql(sql, param, commandType, sortSql);

            try
            {
                if (Config.Instance.LogSql)//写入日志
                    Log.InfoSql(sql, param);
                // await OpenConnectionAsync(conn);
                return conn.QueryAsync<TFirst, TSecond, TReturn>(SqlText, map, param, transaction, buffered, splitOn ?? Builder.DefaultTableKey, timeout, CommandType);
            }
            catch (Exception ex)
            {
                Log.ErrorSql(SqlText, param, ex);
                if (Config.Instance.Debug)
                    throw new DapperSugarException($"SQL命令[ {SqlText} ]执行出错，错误信息：{ex.Message}！", ex);
                else
                {
                    ExceptionCallBack?.Invoke(ex);
                    return Task.FromResult<IEnumerable<TReturn>>(new List<TReturn>(0));
                }
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
        private Task<IEnumerable<TReturn>> QueryDataAsync<TFirst, TSecond, TThird, TReturn>(DbConnection conn, Func<TFirst, TSecond, TThird, TReturn> map, string sql, object param, SugarCommandType commandType, string sortSql, string splitOn, bool buffered, IDbTransaction transaction, int? timeout)
        //where TFirst : class
        //where TSecond : class
        //where TThird : class
        //where TReturn : class
        {
            var (SqlText, CommandType) = TranslateSelectSql(sql, param, commandType, sortSql);

            try
            {
                if (Config.Instance.LogSql)//写入日志
                    Log.InfoSql(sql, param);
                // await OpenConnectionAsync(conn);
                return conn.QueryAsync<TFirst, TSecond, TThird, TReturn>(SqlText, map, param, transaction, buffered, splitOn ?? Builder.DefaultTableKey, timeout, CommandType);
            }
            catch (Exception ex)
            {
                Log.ErrorSql(SqlText, param, ex);
                if (Config.Instance.Debug)
                    throw new DapperSugarException($"SQL命令[ {SqlText} ]执行出错，错误信息：{ex.Message}！", ex);
                else
                {
                    ExceptionCallBack?.Invoke(ex);
                    return Task.FromResult<IEnumerable<TReturn>>(new List<TReturn>(0));
                }
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
        private Task<IEnumerable<TReturn>> QueryDataAsync<TFirst, TSecond, TThird, TFourth, TReturn>(DbConnection conn, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string sql, object param, SugarCommandType commandType, string sortSql, string splitOn, bool buffered, IDbTransaction transaction, int? timeout)
        //where TFirst : class
        //where TSecond : class
        //where TThird : class
        //where TFourth : class
        //where TReturn : class
        {
            var (SqlText, CommandType) = TranslateSelectSql(sql, param, commandType, sortSql);
            try
            {
                if (Config.Instance.LogSql)//写入日志
                    Log.InfoSql(sql, param);
                // await OpenConnectionAsync(conn);
                return conn.QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(SqlText, map, param, transaction, buffered, splitOn ?? Builder.DefaultTableKey, timeout, CommandType);
            }
            catch (Exception ex)
            {
                Log.ErrorSql(SqlText, param, ex);
                if (Config.Instance.Debug)
                    throw new DapperSugarException($"SQL命令[ {SqlText} ]执行出错，错误信息：{ex.Message}！", ex);
                else
                {
                    ExceptionCallBack?.Invoke(ex);
                    return Task.FromResult<IEnumerable<TReturn>>(new List<TReturn>(0));
                }
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
        private Task<IEnumerable<TReturn>> QueryDataAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(DbConnection conn, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string sql, object param, SugarCommandType commandType, string sortSql, string splitOn, bool buffered, IDbTransaction transaction, int? timeout)
        //where TFirst : class
        //where TSecond : class
        //where TThird : class
        //where TFourth : class
        //where TFifth : class
        //where TReturn : class
        {
            var (SqlText, CommandType) = TranslateSelectSql(sql, param, commandType, sortSql);
            try
            {
                if (Config.Instance.LogSql)//写入日志
                    Log.InfoSql(sql, param);
                // await OpenConnectionAsync(conn);
                return conn.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(SqlText, map, param, transaction, buffered, splitOn ?? Builder.DefaultTableKey, timeout, CommandType);
            }
            catch (Exception ex)
            {
                Log.ErrorSql(SqlText, param, ex);
                if (Config.Instance.Debug)
                    throw new DapperSugarException($"SQL命令[ {SqlText} ]执行出错，错误信息：{ex.Message}！", ex);
                else
                {
                    ExceptionCallBack?.Invoke(ex);
                    return Task.FromResult<IEnumerable<TReturn>>(new List<TReturn>(0));
                }
            }
        }

        /// <summary>
        /// 基础分页查询列表（limit分页）
        /// </summary>
        private async Task<PagingList<T>> QueryPagingDataAsync<T>(DbConnection conn, int pageNumber, int pageSize, string sql, object param, SugarCommandType commandType, string sortSql, bool buffered, IDbTransaction transaction, int? timeout)
            where T : class
        {
            if (commandType == SugarCommandType.StoredProcedure)
                throw new ArgumentException("commandType参数不接受StoredProcedure！");

            var (SqlText, CommandType) = TranslateSelectSql(sql, param, commandType, null);

            var (TotalSql, DataSql) = this.Builder.GetPagingSql(SqlText, sortSql, pageNumber, pageSize);

            var total = BaseQueryScalarAsync<int>(conn, TotalSql, param, CommandType.Text, transaction, timeout);

            //if (total == 0)
            //{
            //    return new PagingList<T>(new List<T>(0), 0);
            //}

            var result = BaseQueryAsync<T>(conn, DataSql, param, CommandType.Text, buffered, transaction, timeout);

            return new PagingList<T>((await result.ConfigureAwait(false)).ToList(), await total.ConfigureAwait(false));
        }

        #endregion

        #region 工具

        /// <summary>
        /// 查询自增
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="fieldName">字段名称（仅限PostgreSql需要）</param>
        /// <returns></returns>
        public Task<int> QueryAutoIncrementAsync(DbConnection conn, string fieldName = null)
        {
            return BaseQueryScalarAsync<int>(conn, this.Builder.GetAutoIncrement(fieldName), null, CommandType.Text, null, null);
        }

        /// <summary>
        /// 查询自增
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="fieldName">字段名称（仅限PostgreSql需要）</param>
        /// <returns></returns>
        public Task<long> QueryLongAutoIncrementAsync(DbConnection conn, string fieldName = null)
        {
            return BaseQueryScalarAsync<long>(conn, this.Builder.GetAutoIncrement(fieldName), null, CommandType.Text, null, null);
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
        public Task<T> QueryScalarAsync<T>(DbConnection conn, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, IDbTransaction transaction = null, int? timeout = null)
        //where T : struct
        {
            var (SqlText, CommandType) = TranslateSelectSql(sql, param, commandType, sortSql);

            return BaseQueryScalarAsync<T>(conn, SqlText, param, CommandType, transaction, timeout);
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
        public async Task<T> QuerySingleAsync<T>(DbConnection conn, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where T : class
        {
            var result = await QueryDataAsync<T>(conn, sql, param, commandType, sortSql, buffered, transaction, timeout).ConfigureAwait(false);
            return result.FirstOrDefault();
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
        public async Task<TReturn> QuerySingleAsync<TFirst, TSecond, TReturn>(DbConnection conn, Func<TFirst, TSecond, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TReturn : class
        {
            var result = await QueryDataAsync<TFirst, TSecond, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, transaction, timeout).ConfigureAwait(false);
            return result.FirstOrDefault<TReturn>();
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
        public async Task<TReturn> QuerySingleAsync<TFirst, TSecond, TThird, TReturn>(DbConnection conn, Func<TFirst, TSecond, TThird, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TReturn : class
        {
            var result = await QueryDataAsync<TFirst, TSecond, TThird, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, transaction, timeout).ConfigureAwait(false);
            return result.FirstOrDefault<TReturn>();
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
        public async Task<TReturn> QuerySingleAsync<TFirst, TSecond, TThird, TFourth, TReturn>(DbConnection conn, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TFourth : class
            where TReturn : class
        {
            var result = await QueryDataAsync<TFirst, TSecond, TThird, TFourth, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, transaction, timeout).ConfigureAwait(false);
            return result.FirstOrDefault<TReturn>();
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
        public async Task<TReturn> QuerySingleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(DbConnection conn, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TFourth : class
            where TFifth : class
            where TReturn : class
        {
            var result = await QueryDataAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, transaction, timeout).ConfigureAwait(false);
            return result.FirstOrDefault<TReturn>();
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
        public Task<IEnumerable<T>> QueryListAsync<T>(DbConnection conn, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
        //where T : class
        {
            return QueryDataAsync<T>(conn, sql, param, commandType, sortSql, buffered, transaction, timeout);
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
        public Task<IEnumerable<TReturn>> QueryListAsync<TFirst, TSecond, TReturn>(DbConnection conn, Func<TFirst, TSecond, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
        //where TFirst : class
        //where TSecond : class
        //where TReturn : class
        {
            return QueryDataAsync<TFirst, TSecond, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, transaction, timeout);
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
        public Task<IEnumerable<TReturn>> QueryListAsync<TFirst, TSecond, TThird, TReturn>(DbConnection conn, Func<TFirst, TSecond, TThird, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
        //where TFirst : class
        //where TSecond : class
        //where TThird : class
        //where TReturn : class
        {
            return QueryDataAsync<TFirst, TSecond, TThird, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, transaction, timeout);
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
        public Task<IEnumerable<TReturn>> QueryListAsync<TFirst, TSecond, TThird, TFourth, TReturn>(DbConnection conn, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
        //where TFirst : class
        //where TSecond : class
        //where TThird : class
        //where TFourth : class
        //where TReturn : class
        {
            return QueryDataAsync<TFirst, TSecond, TThird, TFourth, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, transaction, timeout);
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
        public Task<IEnumerable<TReturn>> QueryListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(DbConnection conn, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
        //where TFirst : class
        //where TSecond : class
        //where TThird : class
        //where TFourth : class
        //where TFifth : class
        //where TReturn : class
        {
            return QueryDataAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, transaction, timeout);
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
        public async Task<PagingList<T>> QueryPagingListAsync<T>(DbConnection conn, int pageNumber, int pageSize, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where T : class
        {
            var list = await QueryDataAsync<T>(conn, sql, param, commandType, sortSql, buffered, transaction, timeout).ConfigureAwait(false);
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
        public Task<PagingList<T>> QueryPagingListAsync2<T>(DbConnection conn, int pageNumber, int pageSize, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, IDbTransaction transaction = null, int? timeout = null)
            where T : class
        {
            return QueryPagingDataAsync<T>(conn, pageNumber, pageSize, sql, param, commandType, sortSql, buffered, transaction, timeout);
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
        //public  PagingList<TReturn> QueryPagingList<TFirst, TSecond, TReturn>(DbConnection conn, int pageNumber, int pageSize, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, string splitOn = SPLITON, CommandType? commandType = null)
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
        //public  PagingList<TReturn> QueryPagingList<TFirst, TSecond, TThird, TReturn>(DbConnection conn, int pageNumber, int pageSize, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, string splitOn = SPLITON, CommandType? commandType = null)
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
        //public  PagingList<TReturn> QueryPagingList<TFirst, TSecond, TThird, TFourth, TReturn>(DbConnection conn, int pageNumber, int pageSize, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, string splitOn = SPLITON, CommandType? commandType = null)
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
        //public  PagingList<TReturn> QueryPagingList<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(DbConnection conn, int pageNumber, int pageSize, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null, string splitOn = SPLITON, CommandType? commandType = null)
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
        public Task<int> ExecuteSqlAsync(DbConnection conn, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, IDbTransaction transaction = null, int? timeout = null)
        {
            return ExecuteSqlAsync(conn, new CommandInfo(sql, param, commandType, timeout), transaction);
        }

        /// <summary>
        /// 执行命令(返回影响行数，-1为执行失败)
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="command">命令</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        public Task<int> ExecuteSqlAsync(DbConnection conn, CommandInfo command, IDbTransaction transaction = null)
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
                return conn.ExecuteAsync(command.SqlText, command.Param, transaction, command.Timeout,
                    command.CommandType == SugarCommandType.StoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
            }
            catch (Exception ex)
            {
                //if (Config.Instance.LogSql)//写入日志
                if (command.CommandType == SugarCommandType.StoredProcedure)
                    Log.ErrorProcedure(command.SqlText, command.Param, ex);
                else
                    Log.ErrorSql(command.SqlText, command.Param, ex);

                if (transaction != null || Config.Instance.Debug)
                    throw new DapperSugarException($"{(command.CommandType == SugarCommandType.StoredProcedure ? "Stored Procedure：" : "Sql：")}[ {command.SqlText} ]执行出错，错误信息：{ex.Message}！", ex);
                else
                {
                    ExceptionCallBack?.Invoke(ex);
                    return Task.FromResult<int>(0);
                }
            }
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="commands">命令集合</param>
        /// <returns></returns>
        public async Task<bool> ExecuteSqlTranAsync(CommandCollection commands)
        {
            if (commands.Count <= 0)
                throw new DapperSugarException("commands数目为0");

            using (var conn = this.CreateConnection(Config.DataBaseAuthority.Write))
            {
                int i = 0;
                IDbTransaction trans = null;
                try
                {
                    await conn.OpenAsync().ConfigureAwait(false);
                    trans = conn.BeginTransaction();
                    for (; i < commands.Count; i++)
                    {
                        /*command = */
                        this.TranslateCommand(commands[i]);

                        //写入日志
                        if (Config.Instance.LogSql)
                            Log.InfoSql(commands[i].SqlText, commands[i].Param);
                        //conn.Execute(CommandDefinition)
                        int affected_rows = await conn.ExecuteAsync(commands[i].SqlText, commands[i].Param, trans, commands[i].Timeout,
                            commands[i].CommandType == SugarCommandType.StoredProcedure ? CommandType.StoredProcedure : CommandType.Text).ConfigureAwait(false);
                        //检测影响行数大于0
                        if (commands[i].EffectRows == -1)
                        {
                            if (affected_rows == 0)
                            {
                                //if (trans != null)
                                //    trans.Rollback();
                                Log.ErrorSql($"第[ {i} ]条SQL命令[ {commands[i].SqlText} ]执行出错，错误信息：执行语句影响行数为0", commands[i].Param, null);
                                throw new DapperSugarException($"第[ {i} ]条SQL命令[ {commands[i].SqlText} ]执行出错，错误信息：执行语句影响行数为0");
                            }
                        }
                        else if (commands[i].EffectRows > 0)
                        {
                            if (affected_rows != commands[i].EffectRows)
                            {
                                //if (trans != null)
                                //    trans.Rollback();
                                Log.ErrorSql($"第[ {i} ]条SQL命令[ {commands[i].SqlText} ]执行出错，错误信息：执行语句影响行数为{affected_rows}，不等于影响行数{commands[i].EffectRows}的限制", commands[i].Param, null);
                                throw new DapperSugarException($"第[ {i} ]条SQL命令[ {commands[i].SqlText} ]执行出错，错误信息：执行语句影响行数为{affected_rows}，不等于影响行数{commands[i].EffectRows}的限制");
                            }
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (DapperSugarException ex)
                {
                    if (trans != null)
                        trans.Rollback();
                    if (Config.Instance.Debug)
                        throw ex;
                    else
                    {
                        ExceptionCallBack?.Invoke(ex);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    //出现异常，事务Rollback
                    if (trans != null)
                        trans.Rollback();
                    Log.ErrorSql($"第[ {i} ]条SQL命令[ {commands[i].SqlText} ]执行出错，错误信息：{ex.Message}", commands[i].Param, ex);
                    if (Config.Instance.Debug)
                        throw new DapperSugarException($"第[ {i} ]条SQL命令[ {commands[i].SqlText} ]执行出错，错误信息：{ex.Message}", ex);
                    else
                    {
                        ExceptionCallBack?.Invoke(ex);
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="runFun"></param>
        /// <returns></returns>
        public async Task<bool> ExecuteSqlTranAsync(Func<DbConnection, DbTransaction, Task<bool>> runFun)
        {
            using (var conn = this.CreateConnection(Config.DataBaseAuthority.Write))
            {
                DbTransaction trans = null;
                try
                {
                    await conn.OpenAsync().ConfigureAwait(false);
                    trans = conn.BeginTransaction();

                    if (await runFun(conn, trans))
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
                catch (DapperSugarException ex)
                {
                    //出现异常，事务Rollback
                    if (trans != null)
                        trans.Rollback();

                    if (Config.Instance.Debug)
                        throw new DapperSugarException($"事务执行出错，错误信息：{ex.Message}", ex);
                    else
                    {
                        ExceptionCallBack?.Invoke(ex);
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
                        throw new DapperSugarException($"事务执行出错，错误信息：{ex.Message}", ex);
                    else
                    {
                        ExceptionCallBack?.Invoke(ex);
                        return false;
                    }
                }
            }
        }

        #endregion
    }
}