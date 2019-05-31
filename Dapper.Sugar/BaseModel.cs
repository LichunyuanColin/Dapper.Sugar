using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Sugar
{
    /// <summary>
    /// 分页数据类型接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPagingList<T>
    {
        /// <summary>
        /// 总个数
        /// </summary>
        int Total { get; set; }
        /// <summary>
        /// 数据（数组）
        /// </summary>
        List<T> List { get; set; }
    }

    /// <summary>
    /// 分页数据类型
    /// </summary>
    /// <typeparam name="T">List类型</typeparam>
    public class PagingList<T> : IPagingList<T>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PagingList()
        {
            List = null;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="list">数据（数组）</param>
        /// <param name="total">总个数</param>
        public PagingList(List<T> list, int total)
        {
            List = list;
            Total = total;
        }

        /// <summary>
        /// 总个数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 数据（数组）
        /// </summary>
        public List<T> List { get; set; }

    }

    /// <summary>
    /// 分页数据类型（带扩展数据，实体）
    /// </summary>
    /// <typeparam name="T">List类型</typeparam>
    /// <typeparam name="ExtendT">Extend类型</typeparam>
    public class PagingListExtendData<T, ExtendT> : IPagingList<T>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PagingListExtendData()
        {
            List = null;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">数据</param>
        public PagingListExtendData(IPagingList<T> data)
        {
            List = data.List;
            Total = data.Total;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="list">数据（数组）</param>
        /// <param name="total">总个数</param>
        public PagingListExtendData(List<T> list, int total)
        {
            List = list;
            Total = total;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="list">数据（数组）</param>
        /// <param name="total">总个数</param>
        /// <param name="extend">扩展数据</param>
        public PagingListExtendData(List<T> list, int total, ExtendT extend)
        {
            this.List = list;
            this.Total = total;
            this.Extend = extend;
        }

        /// <summary>
        /// 总个数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 数据（数组）
        /// </summary>
        public List<T> List { get; set; }

        /// <summary>
        /// 扩展数据
        /// </summary>
        public ExtendT Extend { get; set; }
    }

    /// <summary>
    /// 分页数据类型（带扩展数据，数组）
    /// </summary>
    /// <typeparam name="T">List类型</typeparam>
    /// <typeparam name="ExtendT">Extend类型</typeparam>
    public class PagingListExtendList<T, ExtendT> : IPagingList<T>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PagingListExtendList()
        {
            this.List = null;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">数据</param>
        public PagingListExtendList(IPagingList<T> data)
        {
            this.List = data.List;
            this.Total = data.Total;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="extendList">扩展数据</param>
        public PagingListExtendList(IPagingList<T> data, ExtendT extendList)
        {
            this.List = data.List;
            this.Total = data.Total;
            this.Extend = extendList;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="list">数据（数组）</param>
        /// <param name="total">总个数</param>
        public PagingListExtendList(List<T> list, int total)
        {
            this.List = list;
            this.Total = total;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="list">数据（数组）</param>
        /// <param name="total">总个数</param>
        /// <param name="extendList">扩展数据</param>
        public PagingListExtendList(List<T> list, int total, ExtendT extendList)
        {
            this.List = list;
            this.Total = total;
            this.Extend = extendList;
        }

        /// <summary>
        /// 总个数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 数据（数组）
        /// </summary>
        public List<T> List { get; set; }

        /// <summary>
        /// 扩展数据（数组）
        /// </summary>
        public ExtendT Extend { get; set; }
    }

    #region 特性

    /// <summary>
    /// 新增忽略此属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class IgnoreAddAttribute : Attribute
    {
    }

    /// <summary>
    /// 修改忽略此属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class IgnoreUpdateAttribute : Attribute
    {
    }

    /// <summary>
    /// sql自动生成属性设置
    /// </summary>
    public sealed class SqlAttribute : Attribute
    {
        /// <summary>
        /// 修改语句条件主键(大小写敏感)
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 新增、修改语句表名称（大小写不敏感）
        /// </summary>
        public string TableName { get; set; }
    }

    #endregion
}
