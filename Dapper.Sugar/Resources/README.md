# Dapper.Sugar

[TOC]

#1. ����
Dapper.Sugar����[Dapper](https://github.com/StackExchange/Dapper "΢��ORMDapper")���з�װ������ʵ��ӳ��sql��䡢������ѯ����ӳ�䡢����CRUD����

  

#2. ����˵��
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
DbHelp.QuerySingle<EmployeeModel>("select * from employee where Account=@Account and Status in @Status;", new { Account = "songjiang", Status = new int[] { 10, 5 } }, SugarCommandType.Text);

DbHelp.QuerySingle<EmployeeModel>("select * from employee where Account=@Account and Status in @Status;", new { Account = "songjiang", Status = new List<int> { 10, 5 } }, SugarCommandType.Text);
```
sql��� ��
``` mysql
select * from employee where Account=@Account and Status in (@Status1,@Status2);
```


```c#
//����
DbHelp.ExecuteSql("insert into employee (Account,Name,Age) values (@Account,@Name,@Age);", new { Account = "test",Name="����", Age = 50 }, SugarCommandType.Text);
//���ô洢
DbHelp.ExecuteSql("call update_employee(@p_id);", new { p_id = 1 }, SugarCommandType.Text);
```



## ��2��QueryTableDirect ��ѯ����

����param����where������sortSql��׷��sql���

```c#
DbHelp.QueryList<EmployeeModel>("employee", new { Status_ge = 5, Age_gt = 48 }, SugarCommandType.QueryTableDirect, "order by id");
```
sql���
```mysql
select * from employee where Status>=@Status_ge and Age>@Age_gt order by id;
```




## ��3��QuerySelectSql ��ѯ����

��ѯ��䣬��������������param����where����

```c#
DbHelp.QueryPagingList<EmployeeModel>(0, 10, "select a.*,b.Alias from employee a left join employee_alias b on a.Id=b.EmployeeId", new { a_Account = "songjiang", a_Age_gt = 45, Status = new int[] { 10, 5 } }, SugarCommandType.QuerySelectSql);
```

sql���

```mysql
select a.*,b.Alias from employee a left join employee_alias b on a.Id=b.EmployeeId where a.Account=@a_Account and a.Age>@a_Age_gt and Status in (@Status1,@Status2);
```



## ��4��AddTableDirect ��������

```c#
DbHelp.ExecuteSql("employee", new { Account="ceshi", Name = "����", Age = 20 }, SugarCommandType.AddTableDirect);
```
sql���
```mysql
insert into employee(Account,Name,Age) values(@Account,@Name,@Age);
```



## ��5��UpdateTableDirect �޸Ĳ���

```c#
DbHelp.ExecuteSql("employee", new { Id = 1, Name = "�ν�", Age = 50 }, SugarCommandType.UpdateTableDirect);
```
sql���
```mysql
update employee set Name=@Name,Age=@Age where Id=@Id;
```



## ��6��StoredProcedure �洢����

�洢���̣����ݴ洢���Ƶ��ô洢����
```c#
DbHelp.ExecuteSql("update_employee", new { p_id = 1}, SugarCommandType.StoredProcedure);
```





