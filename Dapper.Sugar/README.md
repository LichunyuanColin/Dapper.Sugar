# Dapper.Sugar

[TOC]

# 1. ����
Dapper.Sugar����[Dapper](https://github.com/StackExchange/Dapper "΢��ORMDapper")���з�װ������ʵ��ӳ��sql��䡢������ѯ����ӳ�䡢����CRUD����

������ѯ
``` c#
// ��������
using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
{
	// ִ�����
	return DbProvider.QueryScalar<T>(conn, sql, param, commandType, sortSql, null, timeout);
}
```

# 2. ����˵��
## 2.1 ��̬�����������
1. `SugarCommandType.Text`

   Sql�ı������dapper������������[����1](#��1��Text Sql�ı�����)

2. `SugarCommandType.QueryTableDirect`

   ��ѯ������sql�ı�Ϊ��ѯ�ı��������ݲ�����param����̬����select���[����2](#��2��QueryTableDirect ��ѯ����)


3. `SugarCommandType.QuerySelectSql`

   ��ѯ������sql�ı�Ϊ��ѯ���֣������ɲ�����param����̬���ɣ�[����3](#��3��QuerySelectSql ��ѯ����)

4. `SugarCommandType.AddTableDirect`

   ����������sql�ı�Ϊ���������ݲ�����param����̬����insert���[����4](#��4��AddTableDirect ��������)

5. `SugarCommandType.UpdateTableDirect`

   �޸Ĳ�����sql�ı�Ϊ���������ݲ�����param����̬����update���[����5](#��5��UpdateTableDirect �޸Ĳ���)

6. `SugarCommandType.StoredProcedure`

   �洢���̣�sql�ı�Ϊ�洢�������ƣ�[����6](#��6��StoredProcedure �洢����)


##2.2 ����ӳ��˵��

1. ������Ϊ���������ʵ�壨class������

2. ������������Ϊnullʱ����̬�������ʱ����Ը����ԣ���������ӽ�����

3. ����������������[AliasName_FieldName_OperateName]
  3.1 OperateNameΪ�Ƚϲ���������������ѡ�

   * `_gt`��>(���ڣ�greater than)
   * `_ge`��>=(���ڵ��ڣ�greater equal)
   * `_lt`��<(С�ڣ�less than)
   * `_le`��<=(С�ڵ��ڣ�less equal)
   * `_lk`��like(ģ����ѯ��like)
   * `_ue`��!=(�����ڣ�unequal)

  3.2 AliasNameΪ��ı�����Ŀǰ���޵����ַ���a-z��A-Z��

  3.3 FieldNameΪ�������ƣ���Ҫ��Ӧ���ݿ��ֶ�����

  3.4 OperateName��AliasName�ɷֿ���ϲ�ʹ��




#3. ʵ��
## ��1��Text Sql�ı�����

��ѯ��������������in�﷨����ֱ��д������
ͬ��sql����Ҳ������insert��update��delete��stored procedure

``` c#
DbProvider.QuerySingle<EmployeeModel>("select * from employee where Account=@Account and Status in @Status;", new { Account = "songjiang", Status = new int[] { 10, 5 } }, SugarCommandType.Text);

DbProvider.QuerySingle<EmployeeModel>("select * from employee where Account=@Account and Status in @Status;", new { Account = "songjiang", Status = new List<int> { 10, 5 } }, SugarCommandType.Text);
```
sql��� ��
``` sql
select * from employee where Account=@Account and Status in (@Status1,@Status2);
```


``` c#
//����
DbProvider.ExecuteSql("insert into employee (Account,Name,Age) values (@Account,@Name,@Age);", new { Account = "test",Name="����", Age = 50 }, SugarCommandType.Text);
//���ô洢
DbProvider.ExecuteSql("call update_employee(@p_id);", new { p_id = 1 }, SugarCommandType.Text);
```



## ��2��QueryTableDirect ��ѯ����

����param����where������sortSql��׷��sql���

``` c#
DbProvider.QueryList<EmployeeModel>("employee", new { Status_ge = 5, Age_gt = 48 }, SugarCommandType.QueryTableDirect, "order by id");
```
sql���
``` sql
select * from employee where Status>=@Status_ge and Age>@Age_gt order by id;
```




## ��3��QuerySelectSql ��ѯ����

��ѯ��䣬��������������param����where����

``` c#
DbProvider.QueryPagingList<EmployeeModel>(0, 10, "select a.*,b.Alias from employee a left join employee_alias b on a.ID=b.EmployeeID", new { a_Account = "songjiang", a_Age_gt = 45, Status = new int[] { 10, 5 } }, SugarCommandType.QuerySelectSql);
```

sql���

``` sql
select a.*,b.Alias from employee a left join employee_alias b on a.ID=b.EmployeeID where a.Account=@a_Account and a.Age>@a_Age_gt and Status in (@Status1,@Status2);
```



## ��4��AddTableDirect ��������

``` c#
DbProvider.ExecuteSql("employee", new { Account="ceshi", Name = "����", Age = 20 }, SugarCommandType.AddTableDirect);
```
sql���
``` sql
insert into employee(Account,Name,Age) values(@Account,@Name,@Age);
```



## ��5��UpdateTableDirect �޸Ĳ���

``` c#
DbProvider.ExecuteSql("employee", new { ID = 1, Name = "�ν�", Age = 50 }, SugarCommandType.UpdateTableDirect);
```
sql���
``` sql
update employee set Name=@Name,Age=@Age where ID=@ID;
```



## ��6��StoredProcedure �洢����

�洢���̣����ݴ洢���Ƶ��ô洢����
``` c#
DbProvider.ExecuteSql("update_employee", new { p_id = 1}, SugarCommandType.StoredProcedure);
```

# 3.����˵��

## 3.1 .Net Core
���ݿ�������������.Net Core��.Net Framework����������Ҳ���ּ򻯰棨���⣩�������棨��⣩���ã��������������ȼ����ڼ򻯰�
.Net Core��Ӷ໷�����ã�ͨ����������������ASPNETCORE_ENVIRONMENT��ƥ���Ӧ�����ļ�����ASPNETCORE_ENVIRONMENT: Development ��Ӧ appsettings.Development.json��

��������
``` json
{
  "DapperSugar": {
    "debug": true, // ����ģʽ��sqlִ���쳣ֻ�е���ģʽ���׳����ǵ���ģʽ���ȡ�쳣
    "logsql": true,	// ��¼sql��䣬ִ���쳣ʼ�ջ��¼��־
    "name": "mysql", // ���ݿ�����
    "type": "MySql", // MySql, SqlServer, PostgreSql, Oracle, SQLite
    "connectionString": "Server=localhost;Database=test;Uid=root;Pwd=aikCaBRQ#hL;CharSet=utf8mb4",
  }
}
```
�������
``` json
{
  "DapperSugar": {
    "debug": true, // ����ģʽ��sqlִ���쳣ֻ�е���ģʽ���׳����ǵ���ģʽ���ȡ�쳣
    "logsql": true, // ��¼sql��䣬ִ���쳣ʼ�ջ��¼��־
    "connectionStrings": [
      {
        "name": "mysql", // ���ݿ�����
        "type": "MySql", // MySql, SqlServer, PostgreSql, Oracle, SQLite
        "list": [
          {
            "name": "default", // �ֿ�����
            "authority": "RW", //R���� W��д RW����д
            "connectionString": "Server=localhost;Database=test;Uid=root;Pwd=aikCaBRQ#hL;CharSet=utf8mb4"
          }
        ]
      }
    ]
  }
}
```

## 3.2 .Net Framework
��������
``` xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="DapperSugar" type="Dapper.Sugar.DapperSugarSection, Dapper.Sugar" />
  </configSections>
  <DapperSugar debug="true" logsql="true" name="mysql" type="MySql" connectionString="Server=localhost;Database=test;Uid=root;Pwd=aikCaBRQ#hL;CharSet=utf8mb4">
  </DapperSugar>
</configuration>
```
�������
``` xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="DapperSugar" type="Dapper.Sugar.DapperSugarSection, Dapper.Sugar" />
  </configSections>
  <DapperSugar debug="true" logsql="true">
    <connectionStrings>
      <add name="mysql" type="MySql">
        <list>
          <add name="default" authority="RW" connectionString="Server=localhost;Database=test;Uid=root;Pwd=aikCaBRQ#hL;CharSet=utf8mb4" />
        </list>
      </add>
    </connectionStrings>
  </DapperSugar>
</configuration>
```

# 4.ʵ��
``` c#
public class DbHelp
{
    public static DbProvider DbProvider = DbProvider.CreateDbProvide("mysql");

    #region ��ѯ

    /// <summary>
    /// ��ѯ����Dataset���ؽ���ģ�
    /// </summary>
    /// <param name="sql">sql���</param>
    /// <param name="parms">����</param>
    /// <returns>ʧ�ܷ���null</returns>
    public static DataSet Query(string sql, params DbParameter[] parms)
    {
        using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
        {
            return DbProvider.Query(conn, sql, parms);
        }
    }

    /// <summary>
    /// ��ѯ������ֵ������ڶ��ȡ�������У�
    /// </summary>
    /// <typeparam name="T">����ʵ��</typeparam>
    /// <param name="sql">sql���</param>
    /// <param name="param">���� lt_: &lt;(С��)  le_: &lt;=(С�ڵ���)  gt_: &gt;(����)  ge_: &gt;=(���ڵ���)  lk_: like(ģ����ѯ)  ue_��!=(������)</param>
    /// <param name="commandType">��������</param>
    /// <param name="sortSql">�������</param>
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
    /// ��ѯ����ʵ��
    /// </summary>
    /// <typeparam name="T">����ʵ��</typeparam>
    /// <param name="sql">sql���</param>
    /// <param name="param">���� lt_: &lt;(С��)  le_: &lt;=(С�ڵ���)  gt_: &gt;(����)  ge_: &gt;=(���ڵ���)  lk_: like(ģ����ѯ)  ue_��!=(������)</param>
    /// <param name="commandType">��������</param>
    /// <param name="sortSql">�������</param>
    /// <param name="buffered">�Ƿ񻺴�</param>
    /// <param name="timeout">����ʱ�䣨�룩</param>
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
    /// ��ѯ�б�(���model)
    /// </summary>
    /// <typeparam name="T">����ʵ��</typeparam>
    /// <param name="sql">sql���</param>
    /// <param name="param">���� lt_: &lt;(С��)  le_: &lt;=(С�ڵ���)  gt_: &gt;(����)  ge_: &gt;=(���ڵ���)  lk_: like(ģ����ѯ)  ue_��!=(������)</param>
    /// <param name="commandType">��������</param>
    /// <param name="sortSql">�������</param>
    /// <param name="buffered">�Ƿ񻺴�</param>
    /// <param name="timeout">����ʱ�䣨�룩</param>
    /// <returns></returns>
    public static List<T> QueryList<T>(string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, string sortSql = null, bool buffered = true, int? timeout = null)
        where T : class
    {
        using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
        {
            return DbProvider.QueryList<T>(conn, sql, param, commandType, sortSql, buffered, null, timeout);
        }
    }

    /// <summary>
    /// ��ҳ��ѯ�б�(1) - �ڴ��ҳ
    /// </summary>
    /// <typeparam name="T">����ʵ��</typeparam>
    /// <param name="pageNumber">��ǰҳ</param>
    /// <param name="pageSize">ÿҳ��¼��</param>
    /// <param name="sql">sql���</param>
    /// <param name="param">���� lt_: &lt;(С��)  le_: &lt;=(С�ڵ���)  gt_: &gt;(����)  ge_: &gt;=(���ڵ���)  lk_: like(ģ����ѯ)  ue_��!=(������)</param>
    /// <param name="commandType">��������</param>
    /// <param name="sortSql">�������</param>
    /// <param name="buffered">�Ƿ񻺴�</param>
    /// <param name="timeout">����ʱ�䣨�룩</param>
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
    /// ��ҳ��ѯ�б�(1) - limi��ҳ
    /// </summary>
    /// <typeparam name="T">����ʵ��</typeparam>
    /// <param name="pageNumber">��ǰҳ</param>
    /// <param name="pageSize">ÿҳ��¼��</param>
    /// <param name="sql">sql���</param>
    /// <param name="param">���� lt_: &lt;(С��)  le_: &lt;=(С�ڵ���)  gt_: &gt;(����)  ge_: &gt;=(���ڵ���)  lk_: like(ģ����ѯ)  ue_��!=(������)</param>
    /// <param name="commandType">��������</param>
    /// <param name="sortSql">�������</param>
    /// <param name="buffered">�Ƿ񻺴�</param>
    /// <param name="timeout">����ʱ�䣨�룩</param>
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

    #region ����

    /// <summary>
    /// ִ������(����Ӱ��������-1Ϊִ��ʧ��)
    /// </summary>
    /// <param name="sql">sql���</param>
    /// <param name="param">���� lt_: &lt;(С��)  le_: &lt;=(С�ڵ���)  gt_: &gt;(����)  ge_: &gt;=(���ڵ���)  lk_: like(ģ����ѯ)  ue_��!=(������)</param>
    /// <param name="commandType">��������</param>
    /// <param name="timeout">����ʱ�䣨�룩</param>
    /// <returns></returns>
    public static int ExecuteSql(string sql, object param = null, SugarCommandType commandType = SugarCommandType.Text, int? timeout = null)
    {
        using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
        {
            return DbProvider.ExecuteSql(conn, new CommandInfo(sql, param, commandType, timeout));
        }
    }

    /// <summary>
    /// ִ������(����Ӱ��������-1Ϊִ��ʧ��)
    /// </summary>
    /// <param name="command">����</param>
    /// <returns></returns>
    public static int ExecuteSql(CommandInfo command)
    {
        using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
        {
            return DbProvider.ExecuteSql(conn, command);
        }
    }

    /// <summary>
    /// ִ������
    /// </summary>
    /// <param name="commands">�����</param>
    /// <returns></returns>
    public static bool ExecuteSqlTran(CommandCollection commands)
    {
        return DbProvider.ExecuteSqlTran(commands);
    }

    #endregion
}
```


