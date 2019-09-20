using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Dapper.Sugar
{

    /// <summary>
    /// 影响行数类型
    /// </summary>
    public enum EffentNextType
    {
        /// <summary>
        /// 对其他语句无任何影响 
        /// </summary>
        None = 0,
        /// <summary>
        /// 当前语句影响到的行数必须大于0，否则回滚事务
        /// </summary>
        ExcuteEffectRows = -1
    }

    /// <summary>
    /// 执行命令类型
    /// </summary>
    public enum SugarCommandType
    {
        /// <summary>
        /// SQL 文本命令（默认）
        /// </summary>
        Text = 1,
        /// <summary>
        /// 存储过程名称
        /// </summary>
        StoredProcedure = 2,
        /// <summary>
        /// 新增操作表名称
        /// </summary>
        AddTableDirect = 3,
        /// <summary>
        /// 修改操作表名称
        /// </summary>
        UpdateTableDirect = 4,
        /// <summary>
        /// 查询操作表名称
        /// </summary>
        QueryTableDirect = 5,
        /// <summary>
        /// 查询语句（不含where条件语句）
        /// </summary>
        QuerySelectSql = 6,
    }

    /// <summary>
    /// 执行命令实体
    /// </summary>
    public class CommandInfo : ICommandInfo
    {
        #region 私有属性

        /////// <summary>
        /////// 影响结果类型
        /////// </summary>
        ////private EffentNextType _type;
        ///// <summary>
        ///// 影响结果数据,-1：影响行数大于一  0：不限制影响行数  >0：影响行数为具体的数值
        ///// </summary>
        //private int _effectRows;
        ///// <summary>
        ///// sql语句
        ///// </summary>
        //private string _sqlText;
        ///// <summary>
        ///// 参数
        ///// </summary>
        //private object _param;
        ///// <summary>
        ///// 命令类型
        ///// </summary>
        //private SugarCommandType _commandType;
        ///// <summary>
        ///// 过期时间（秒）
        ///// </summary>
        //private int? _timeOut = null;

        #endregion

        #region 公开属性

        /// <summary>
        /// 限制影响结果（-1：影响到的行数必须大于0，否则回滚事务 0：不限制影响行数 > 0：限制影响到的行数必须为固定数）
        /// </summary>
        public int EffectRows { get; set; }
        /// <summary>
        /// sql语句
        /// </summary>
        public string SqlText { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public object Param { get; set; }
        ///// <summary>
        ///// 更新参数
        ///// </summary>
        //public object ConditionParam { get; protected set; }
        /// <summary>
        /// 命令类型
        /// </summary>
        public SugarCommandType CommandType { get; set; }

        /// <summary>
        /// 过期时间（秒）
        /// </summary>
        public int? Timeout { get; set; }

        ///// <summary>
        ///// 事务
        ///// </summary>
        //public IDbTransaction Transaction { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public CommandInfo() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="timeout">过期时间（秒）</param>
        public CommandInfo(string sqlText, object param = null, SugarCommandType commandType = SugarCommandType.Text, int? timeout = null)
        {
            SqlText = sqlText;
            CommandType = commandType;
            Param = param;
            EffectRows = 0;
            Timeout = timeout;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="type">影响结果类型</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="timeout">过期时间（秒）</param>
        public CommandInfo(string sqlText, object param, EffentNextType type, SugarCommandType commandType = SugarCommandType.Text, int? timeout = null)
        {
            SqlText = sqlText;
            CommandType = commandType;
            Param = param;
            EffectRows = (int)type;
            Timeout = timeout;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="effectRows">限制影响结果</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="timeout">过期时间（秒）</param>
        public CommandInfo(string sqlText, object param, int effectRows, SugarCommandType commandType = SugarCommandType.Text, int? timeout = null)
        {
            if (effectRows <= 0)
                throw new ArgumentException(nameof(effectRows) + "不能为小于1的数");

            SqlText = sqlText;
            CommandType = commandType;
            Param = param;
            EffectRows = effectRows;
            Timeout = timeout;
            //Transaction = transaction;
        }

        #endregion
    }

    //public class UpdateCommandInfo : CommandInfo
    //{
    //    /// <summary>
    //    /// 构造函数
    //    /// </summary>
    //    /// <param name="sqlText">sql语句</param>
    //    /// <param name="param">参数</param>
    //    /// <param name="transaction">事务</param>
    //    /// <param name="timeout">过期时间（秒）</param>
    //    public UpdateCommandInfo(string sqlText, object param, object conditionParam, IDbTransaction transaction = null, int? timeout = null)
    //        : this(sqlText, param, conditionParam, EffentNextType.None, transaction, timeout) { }

    //    /// <summary>
    //    /// 构造函数
    //    /// </summary>
    //    /// <param name="sqlText">sql语句</param>
    //    /// <param name="param">参数</param>
    //    /// <param name="type">影响结果类型</param>
    //    /// <param name="transaction">事务</param>
    //    /// <param name="timeout">过期时间（秒）</param>
    //    public UpdateCommandInfo(string sqlText, object param, object conditionParam, EffentNextType type, IDbTransaction transaction = null, int? timeout = null)
    //   : this(sqlText, param, conditionParam, (int)type) { }

    //    /// <summary>
    //    /// 构造函数
    //    /// </summary>
    //    /// <param name="sqlText">sql语句</param>
    //    /// <param name="setParam">参数</param>
    //    /// <param name="effectRows">限制影响结果</param>
    //    /// <param name="transaction">事务</param>
    //    /// <param name="timeout">过期时间（秒）</param>
    //    public UpdateCommandInfo(string sqlText, object setParam, object conditionParam, int effectRows, IDbTransaction transaction = null, int? timeout = null)
    //    {
    //        this.SqlText = sqlText;
    //        this.CommandType = SugarCommandType.UpdateTableDirect;
    //        this.Param = setParam;
    //        this.ConditionParam = conditionParam;
    //        this.EffectRows = effectRows;
    //        this.Timeout = timeout;
    //        this.Transaction = transaction;
    //    }
    //}

    /// <summary>
    /// 执行命令集合
    /// </summary>
    public sealed class CommandCollection : IEnumerable<CommandInfo>
    {
        private List<CommandInfo> _commands;
        /// <summary>
        /// 执行命令集合
        /// </summary>
        public List<CommandInfo> Commands
        {
            get
            {
                return _commands;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public CommandCollection()
        {
            _commands = new List<CommandInfo>();
        }
        /// <summary>
        /// 元素数
        /// </summary>
        public int Count
        {
            get { return _commands.Count; }
        }
        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public CommandInfo this[int index]
        {
            get
            {
                return _commands[index];
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="timeout">过期时间（秒）</param>
        public void Add(string sqlText, object param = null, int? timeout = null)
        {
            Add(sqlText, param, 0, SugarCommandType.Text, timeout);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="timeout">过期时间（秒）</param>
        public void Add(string sqlText, object param, SugarCommandType commandType, int? timeout = null)
        {
            Add(sqlText, param, EffentNextType.None, commandType, timeout);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="type">影响结果类型</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="timeout">过期时间（秒）</param>
        public void Add(string sqlText, object param, EffentNextType type, SugarCommandType commandType = SugarCommandType.Text, int? timeout = null)
        {
            _commands.Add(new CommandInfo(sqlText, param, type, commandType, timeout));
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        /// <param name="param">参数 lt_: &lt;(小于)  le_: &lt;=(小于等于)  gt_: &gt;(大于)  ge_: &gt;=(大于等于)  lk_: like(模糊查询)  ue_：!=(不等于)</param>
        /// <param name="effectRows">限制影响结果</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="timeout">过期时间（秒）</param>
        public void Add(string sqlText, object param, int effectRows, SugarCommandType commandType = SugarCommandType.Text, int? timeout = null)
        {
            _commands.Add(new CommandInfo(sqlText, param, effectRows, commandType, timeout));
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="item"></param>
        public void Add(CommandInfo item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            _commands.Add(item);
        }
        /// <summary>
        /// 添加多个
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(CommandCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("items");
            }
            if (collection.Count > 0)
                _commands.AddRange(collection.Commands);
        }
        /// <summary>
        /// 添加多个
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IEnumerable<CommandInfo> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            _commands.AddRange(collection);
        }
        /// <summary>
        /// 返回类型为 System.Collections.Generic.IEnumerable 的输入
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CommandInfo> AsEnumerable()
        {
            return _commands.AsEnumerable();
        }
        /// <summary>
        /// 返回循环访问 System.Collections.Generic.List 的枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CommandInfo> GetEnumerator()
        {
            return _commands.GetEnumerator();
        }
        /// <summary>
        /// 返回循环访问 System.Collections.Generic.List 的枚举器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _commands.GetEnumerator();
        }
    }
}
