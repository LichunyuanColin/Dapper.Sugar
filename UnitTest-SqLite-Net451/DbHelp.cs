using Dapper.Sugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace UnitTest_SqLite_Net451
{
    public class DbHelp
    {
        public static DbProvider DbProvider = DbProvider.CreateDbProvide("sqlite");

        #region 查询

        /// <summary>
        /// 查询（以Dataset返回结果的）
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parms">参数</param>
        /// <returns>失败返回null</returns>
        public static DataSet Query(string sql, params DbParameter[] parms)
        {
            using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
            {
                return DbProvider.Query(conn, sql, parms);
            }
        }

        /// <summary>
        /// 查询单个数值（如存在多个取首行首列）
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static T QueryScalar<T>(string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, int? timeout = null)
            where T : struct
        {
            using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
            {
                return DbProvider.QueryScalar<T>(conn, sql, param, commandType, sortSql, null, timeout);
            }
        }

        /// <summary>
        /// 查询单个实体
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static T QuerySingle<T>(string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, int? timeout = null)
            where T : class
        {
            using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
            {
                return DbProvider.QuerySingle<T>(conn, sql, param, commandType, sortSql, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 连表查询单个实体(2)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static TReturn QuerySingle<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TReturn : class
        {
            using (var conn = DbProvider.CreateConnection())
            {
                return DbProvider.QuerySingle<TFirst, TSecond, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 连表查询单个实体(3)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static TReturn QuerySingle<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TReturn : class
        {
            using (var conn = DbProvider.CreateConnection())
            {
                return DbProvider.QuerySingle<TFirst, TSecond, TThird, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 连表查询单个实体(4)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TFourth">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static TReturn QuerySingle<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TFourth : class
            where TReturn : class
        {
            using (var conn = DbProvider.CreateConnection())
            {
                return DbProvider.QuerySingle<TFirst, TSecond, TThird, TFourth, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, null, timeout);
            }
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
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static TReturn QuerySingle<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TFourth : class
            where TFifth : class
            where TReturn : class
        {
            using (var conn = DbProvider.CreateConnection())
            {
                return DbProvider.QuerySingle<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 查询列表(多个model)
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static IEnumerable<T> QueryList<T>(string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, int? timeout = null)
            where T : class
        {
            using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
            {
                return DbProvider.QueryList<T>(conn, sql, param, commandType, sortSql, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 连表查询列表(2)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static IEnumerable<TReturn> QueryList<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TReturn : class
        {
            using (var conn = DbProvider.CreateConnection())
            {
                return DbProvider.QueryList<TFirst, TSecond, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 连表查询列表(3)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static IEnumerable<TReturn> QueryList<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TReturn : class
        {
            using (var conn = DbProvider.CreateConnection())
            {
                return DbProvider.QueryList<TFirst, TSecond, TThird, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 连表查询列表(4)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TFourth">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static IEnumerable<TReturn> QueryList<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TFourth : class
            where TReturn : class
        {
            using (var conn = DbProvider.CreateConnection())
            {
                return DbProvider.QueryList<TFirst, TSecond, TThird, TFourth, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, null, timeout);
            }
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
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static IEnumerable<TReturn> QueryList<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TFourth : class
            where TFifth : class
            where TReturn : class
        {
            using (var conn = DbProvider.CreateConnection())
            {
                return DbProvider.QueryList<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 分页查询列表(1) - 内存分页
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="pageNumber">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static IPagingList<T> QueryPagingList<T>(int pageNumber, int pageSize, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, int? timeout = null)
            where T : class
        {
            using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
            {
                return DbProvider.QueryPagingList<T>(conn, pageNumber, pageSize, sql, param, commandType, sortSql, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 分页查询列表(1) - sql分页
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="pageNumber">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static IPagingList<T> QueryPagingList2<T>(int pageNumber, int pageSize, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, int? timeout = null)
            where T : class
        {
            using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
            {
                return DbProvider.QueryPagingList2<T>(conn, pageNumber, pageSize, sql, param, commandType, sortSql, buffered, null, timeout);
            }
        }

        #endregion

        #region 查询-异步

        ///// <summary>
        ///// 查询（以Dataset返回结果的）
        ///// </summary>
        ///// <param name="sql">sql语句</param>
        ///// <param name="parms">参数</param>
        ///// <returns>失败返回null</returns>
        //public static DataSet Query(string sql, params DbParameter[] parms)
        //{
        //    using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
        //    {
        //        return DbProvider.Query(conn, sql, parms);
        //    }
        //}

        /// <summary>
        /// 查询单个数值（如存在多个取首行首列）
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static Task<T> QueryScalarAsync<T>(string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, int? timeout = null)
            where T : struct
        {
            using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
            {
                return DbProvider.QueryScalarAsync<T>(conn, sql, param, commandType, sortSql, null, timeout);
            }
        }

        /// <summary>
        /// 查询单个实体
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static Task<T> QuerySingleAsync<T>(string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, int? timeout = null)
            where T : class
        {
            using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
            {
                return DbProvider.QuerySingleAsync<T>(conn, sql, param, commandType, sortSql, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 连表查询单个实体(2)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static Task<TReturn> QuerySingleAsync<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TReturn : class
        {
            using (var conn = DbProvider.CreateConnection())
            {
                return DbProvider.QuerySingleAsync<TFirst, TSecond, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 连表查询单个实体(3)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static Task<TReturn> QuerySingleAsync<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TReturn : class
        {
            using (var conn = DbProvider.CreateConnection())
            {
                return DbProvider.QuerySingleAsync<TFirst, TSecond, TThird, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 连表查询单个实体(4)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TFourth">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static Task<TReturn> QuerySingleAsync<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TFourth : class
            where TReturn : class
        {
            using (var conn = DbProvider.CreateConnection())
            {
                return DbProvider.QuerySingleAsync<TFirst, TSecond, TThird, TFourth, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, null, timeout);
            }
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
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static Task<TReturn> QuerySingleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TFourth : class
            where TFifth : class
            where TReturn : class
        {
            using (var conn = DbProvider.CreateConnection())
            {
                return DbProvider.QuerySingleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 查询列表(多个model)
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static Task<IEnumerable<T>> QueryListAsync<T>(string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, int? timeout = null)
            where T : class
        {
            using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
            {
                return DbProvider.QueryListAsync<T>(conn, sql, param, commandType, sortSql, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 连表查询列表(2)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryListAsync<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TReturn : class
        {
            using (var conn = DbProvider.CreateConnection())
            {
                return DbProvider.QueryListAsync<TFirst, TSecond, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 连表查询列表(3)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryListAsync<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TReturn : class
        {
            using (var conn = DbProvider.CreateConnection())
            {
                return DbProvider.QueryListAsync<TFirst, TSecond, TThird, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 连表查询列表(4)
        /// </summary>
        /// <typeparam name="TFirst">数据实体</typeparam>
        /// <typeparam name="TSecond">数据实体</typeparam>
        /// <typeparam name="TThird">数据实体</typeparam>
        /// <typeparam name="TFourth">数据实体</typeparam>
        /// <typeparam name="TReturn">返回数据实体</typeparam>
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryListAsync<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TFourth : class
            where TReturn : class
        {
            using (var conn = DbProvider.CreateConnection())
            {
                return DbProvider.QueryListAsync<TFirst, TSecond, TThird, TFourth, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, null, timeout);
            }
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
        /// <param name="map">委托-两个表数据逻辑处理</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType"> 命令类型 </param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="splitOn">分割两表数据的列名称</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, string splitOn = null, bool buffered = true, int? timeout = null)
            where TFirst : class
            where TSecond : class
            where TThird : class
            where TFourth : class
            where TFifth : class
            where TReturn : class
        {
            using (var conn = DbProvider.CreateConnection())
            {
                return DbProvider.QueryListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(conn, map, sql, param, commandType, sortSql, splitOn, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 分页查询列表(1) - 内存分页
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="pageNumber">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static Task<PagingList<T>> QueryPagingListAsync<T>(int pageNumber, int pageSize, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, int? timeout = null)
            where T : class
        {
            using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
            {
                return DbProvider.QueryPagingListAsync<T>(conn, pageNumber, pageSize, sql, param, commandType, sortSql, buffered, null, timeout);
            }
        }

        /// <summary>
        /// 分页查询列表(1) - sql分页
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <param name="pageNumber">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="sortSql">排序语句</param>
        /// <param name="buffered">是否缓存</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static Task<PagingList<T>> QueryPagingListAsync2<T>(int pageNumber, int pageSize, string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, int? timeout = null)
            where T : class
        {
            using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
            {
                return DbProvider.QueryPagingListAsync2<T>(conn, pageNumber, pageSize, sql, param, commandType, sortSql, buffered, null, timeout);
            }
        }

        #endregion

        #region 操作

        /// <summary>
        /// 执行命令(返回影响行数，-1为执行失败)
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static int ExecuteSql(string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, int? timeout = null)
        {
            using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
            {
                return DbProvider.ExecuteSql(conn, new CommandInfo(sql, param, commandType, timeout));
            }
        }

        /// <summary>
        /// 执行命令(返回影响行数，-1为执行失败)
        /// </summary>
        /// <param name="command">命令</param>
        /// <returns></returns>
        public static int ExecuteSql(CommandInfo command)
        {
            using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
            {
                return DbProvider.ExecuteSql(conn, command);
            }
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="commands">命令集合</param>
        /// <returns></returns>
        public static bool ExecuteSqlTran(CommandCollection commands)
        {
            return DbProvider.ExecuteSqlTran(commands);
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="runFun">执行语句</param>
        /// <returns></returns>
        public static bool ExecuteSqlTran(Func<IDbConnection, IDbTransaction, bool> runFun)
        {
            return DbProvider.ExecuteSqlTran(runFun);
        }
        #endregion

        #region 操作-异步

        /// <summary>
        /// 执行命令(返回影响行数，-1为执行失败)
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="timeout">过期时间（秒）</param>
        /// <returns></returns>
        public static Task<int> ExecuteSqlAsync(string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, int? timeout = null)
        {
            using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
            {
                return DbProvider.ExecuteSqlAsync(conn, new CommandInfo(sql, param, commandType, timeout));
            }
        }

        /// <summary>
        /// 执行命令(返回影响行数，-1为执行失败)
        /// </summary>
        /// <param name="command">命令</param>
        /// <returns></returns>
        public static Task<int> ExecuteSqlAsync(CommandInfo command)
        {
            using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
            {
                return DbProvider.ExecuteSqlAsync(conn, command);
            }
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="commands">命令集合</param>
        /// <returns></returns>
        public static Task<bool> ExecuteSqlTranAsync(CommandCollection commands)
        {
            return DbProvider.ExecuteSqlTranAsync(commands);
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="runFun">执行语句</param>
        /// <returns></returns>
        public static Task<bool> ExecuteSqlTranAsync(Func<IDbConnection, IDbTransaction, Task<bool>> runFun)
        {
            return DbProvider.ExecuteSqlTranAsync(runFun);
        }
        #endregion
    }
}
