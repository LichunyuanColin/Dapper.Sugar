# Dapper.Sugar

[TOC]

# 1. 介绍
Dapper.Sugar基于[Dapper](https://github.com/StackExchange/Dapper "微型ORMDapper")进行封装，包含实体映射sql语句、基本查询条件映射、基本CRUD操作

基本查询
``` c#
// 创建连接
using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
{
	// 执行语句
	return DbProvider.QueryScalar<T>(conn, sql, param, commandType, sortSql, null, timeout);
}
```

# 2. 参数说明
## 2.1 动态生成语句类型
1. `SugarCommandType.Text`

   Sql文本命令，仅dapper对数组做处理，[见例1](#例1：Text Sql文本命令)

2. `SugarCommandType.QueryTableDirect`

   查询操作，sql文本为查询的表名，根据参数（param）动态生成select命令，[见例2](#例2：QueryTableDirect 查询操作)


3. `SugarCommandType.QuerySelectSql`

   查询操作，sql文本为查询部分，条件由参数（param）动态生成，[见例3](#例3：QuerySelectSql 查询操作)

4. `SugarCommandType.AddTableDirect`

   新增操作，sql文本为表名，根据参数（param）动态生成insert命令，[见例4](#例4：AddTableDirect 新增操作)

5. `SugarCommandType.UpdateTableDirect`

   修改操作，sql文本为表名，根据参数（param）动态生成update命令，[见例5](#例5：UpdateTableDirect 修改操作)

6. `SugarCommandType.StoredProcedure`

   存储过程，sql文本为存储过程名称，[见例6](#例6：StoredProcedure 存储过程)


##2.2 条件映射说明

1. 参数可为匿名对象或实体（class）对象

2. 任意类型属性为null时，动态生成语句时会忽略该属性，并不会添加进条件

3. 参数对象属性名：[AliasName_FieldName_OperateName]
  3.1 OperateName为比较操作符，仅限以下选项：

   * `_gt`：>(大于：greater than)
   * `_ge`：>=(大于等于：greater equal)
   * `_lt`：<(小于：less than)
   * `_le`：<=(小于等于：less equal)
   * `_lk`：like(模糊查询：like)
   * `_ue`：!=(不等于：unequal)

  3.2 AliasName为表的别名，目前仅限单个字符（a-z，A-Z）

  3.3 FieldName为属性名称，需要对应数据库字段名称

  3.4 OperateName和AliasName可分开或合并使用




#3. 实例
## 例1：Text Sql文本命令

查询操作，对于条件in语法可以直接写成数组
同样sql命令也可以是insert、update、delete、stored procedure

``` c#
DbProvider.QuerySingle<EmployeeModel>("select * from employee where Account=@Account and Status in @Status;", new { Account = "songjiang", Status = new int[] { 10, 5 } }, SugarCommandType.Text);

DbProvider.QuerySingle<EmployeeModel>("select * from employee where Account=@Account and Status in @Status;", new { Account = "songjiang", Status = new List<int> { 10, 5 } }, SugarCommandType.Text);
```
sql语句 ：
``` sql
select * from employee where Account=@Account and Status in (@Status1,@Status2);
```


``` c#
//新增
DbProvider.ExecuteSql("insert into employee (Account,Name,Age) values (@Account,@Name,@Age);", new { Account = "test",Name="测试", Age = 50 }, SugarCommandType.Text);
//调用存储
DbProvider.ExecuteSql("call update_employee(@p_id);", new { p_id = 1 }, SugarCommandType.Text);
```



## 例2：QueryTableDirect 查询操作

根据param生成where条件，sortSql可追加sql语句

``` c#
DbProvider.QueryList<EmployeeModel>("employee", new { Status_ge = 5, Age_gt = 48 }, SugarCommandType.QueryTableDirect, "order by id");
```
sql语句
``` sql
select * from employee where Status>=@Status_ge and Age>@Age_gt order by id;
```




## 例3：QuerySelectSql 查询操作

查询语句，不含条件，根据param生成where条件

``` c#
DbProvider.QueryPagingList<EmployeeModel>(0, 10, "select a.*,b.Alias from employee a left join employee_alias b on a.ID=b.EmployeeID", new { a_Account = "songjiang", a_Age_gt = 45, Status = new int[] { 10, 5 } }, SugarCommandType.QuerySelectSql);
```

sql语句

``` sql
select a.*,b.Alias from employee a left join employee_alias b on a.ID=b.EmployeeID where a.Account=@a_Account and a.Age>@a_Age_gt and Status in (@Status1,@Status2);
```



## 例4：AddTableDirect 新增操作

``` c#
DbProvider.ExecuteSql("employee", new { Account="ceshi", Name = "测试", Age = 20 }, SugarCommandType.AddTableDirect);
```
sql语句
``` sql
insert into employee(Account,Name,Age) values(@Account,@Name,@Age);
```



## 例5：UpdateTableDirect 修改操作

``` c#
DbProvider.ExecuteSql("employee", new { ID = 1, Name = "宋江", Age = 50 }, SugarCommandType.UpdateTableDirect);
```
sql语句
``` sql
update employee set Name=@Name,Age=@Age where ID=@ID;
```



## 例6：StoredProcedure 存储过程

存储过程，根据存储名称调用存储过程
``` c#
DbProvider.ExecuteSql("update_employee", new { p_id = 1}, SugarCommandType.StoredProcedure);
```

# 3.配置说明

## 3.1 .Net Core
数据库连接配置区分.Net Core、.Net Framework，另外配置也区分简化版（单库）、完整版（多库）配置，完整版配置优先级高于简化版
.Net Core添加多环境配置，通过参数环境参数：ASPNETCORE_ENVIRONMENT，匹配对应配置文件（如ASPNETCORE_ENVIRONMENT: Development 对应 appsettings.Development.json）

单库配置
``` json
{
  "DapperSugar": {
    "debug": true, // 调试模式，sql执行异常只有调试模式会抛出，非调试模式会截取异常
    "logsql": true,	// 记录sql语句，执行异常始终会记录日志
    "name": "mysql", // 数据库名称
    "type": "MySql", // MySql, SqlServer, PostgreSql, Oracle, SQLite
    "connectionString": "Server=localhost;Database=test;Uid=root;Pwd=aikCaBRQ#hL;CharSet=utf8mb4",
  }
}
```
多库配置
``` json
{
  "DapperSugar": {
    "debug": true, // 调试模式，sql执行异常只有调试模式会抛出，非调试模式会截取异常
    "logsql": true, // 记录sql语句，执行异常始终会记录日志
    "connectionStrings": [
      {
        "name": "mysql", // 数据库名称
        "type": "MySql", // MySql, SqlServer, PostgreSql, Oracle, SQLite
        "list": [
          {
            "name": "default", // 分库名称
            "authority": "RW", //R：读 W：写 RW：读写
            "connectionString": "Server=localhost;Database=test;Uid=root;Pwd=aikCaBRQ#hL;CharSet=utf8mb4"
          }
        ]
      }
    ]
  }
}
```

## 3.2 .Net Framework
单库配置
``` xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="DapperSugar" type="Dapper.Sugar.DapperSugarSection, Dapper.Sugar" />
  </configSections>
  <!--type 可选 MySql, SqlServer, PostgreSql, Oracle, SQLite-->
  <DapperSugar debug="true" logsql="true" name="mysql" type="MySql" connectionString="Server=localhost;Database=test;Uid=root;Pwd=aikCaBRQ#hL;CharSet=utf8mb4">
  </DapperSugar>
</configuration>
```
多库配置
``` xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="DapperSugar" type="Dapper.Sugar.DapperSugarSection, Dapper.Sugar" />
  </configSections>
  <DapperSugar debug="true" logsql="true">
    <connectionStrings>
      <!--type 可选 MySql, SqlServer, PostgreSql, Oracle, SQLite-->
      <add name="mysql" type="MySql">
        <list>
          <add name="default" authority="RW" connectionString="Server=localhost;Database=test;Uid=root;Pwd=aikCaBRQ#hL;CharSet=utf8mb4" />
        </list>
      </add>
    </connectionStrings>
  </DapperSugar>
</configuration>
```

# 4.实例
``` c#
public class DbHelp
{
    public static DbProvider DbProvider = DbProvider.CreateDbProvide("mysql");

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
```


