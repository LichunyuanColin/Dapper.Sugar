using System.Data;

namespace Dapper.Sugar
{
    /// <summary>
    /// 命令类型
    /// </summary>
    public interface ICommandInfo
    {
        /// <summary>
        /// 命令类型
        /// </summary>
        SugarCommandType CommandType { get; set; }
        /// <summary>
        /// 限制影响结果（-1：影响到的行数必须大于0，否则回滚事务 0：不限制影响行数 > 0：限制影响到的行数必须为固定数）
        /// </summary>
        int EffectRows { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        object Param { get; set; }
        /// <summary>
        /// sql语句
        /// </summary>
        string SqlText { get; set; }
        //object ConditionParam { get; }
        /// <summary>
        /// 过期时间（秒）
        /// </summary>
        int? Timeout { get; set; }
        //IDbTransaction Transaction { get; set; }
    }
}