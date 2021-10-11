using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Sugar;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace UnitTest_Net451
{
    [TestClass]
    public class UnitTest
    {
        List<EmployeeModel> list = new List<EmployeeModel>()
        {
            new EmployeeModel
            {
                Account = "qinming",
                Name = "秦明",
                Age = 36,
                Status = EmployeeModel.EnumStatus.Complate,
            },
            new EmployeeModel
            {
                Account = "huyanzhuo",
                Name = "呼延灼",
                Age = 36,
                Status = EmployeeModel.EnumStatus.Complate,
            },
            new EmployeeModel
            {
                Account = "huarong",
                Name = "花荣",
                Age = 34,
                Status = EmployeeModel.EnumStatus.Complate,
            },
            new EmployeeModel
            {
                Account = "caijin",
                Name = "柴进",
                Age = 34,
                Status = EmployeeModel.EnumStatus.Complate,
            },
            new EmployeeModel
            {
                Account = "liying",
                Name = "李应",
                Age = 34,
                Status = EmployeeModel.EnumStatus.Complate,
            },
            new EmployeeModel
            {
                Account = "zhutong",
                Name = "朱仝",
                Age = 34,
                Status = EmployeeModel.EnumStatus.Complate,
            },
            new EmployeeModel
            {
                Account = "luzhishen",
                Name = "鲁智深",
                Age = 34,
                Status = EmployeeModel.EnumStatus.Complate,
            },
            new EmployeeModel
            {
                Account = "wusong",
                Name = "武松",
                Age = 34,
                Status = EmployeeModel.EnumStatus.Complate,
            }
        };

        #region 操作

        /// <summary>
        /// 测试命令初始化
        /// </summary>
        [TestMethod]
        public void TestParam()
        {

            long[] delIds = new long[] { 471331679261163520 };

            CommandCollection commands = new CommandCollection();

            commands.Add("update rights set Status=0 where Id in @Ids", new
            {
                Ids = delIds
            }, EffentNextType.None);
        }

        /// <summary>
        /// 新增-表名-单个匿名对象
        /// </summary>
        [TestMethod]
        public void TestA_Add1()
        {
            int result = 0;
            var employee = list[0];
            EmployeeModel addInfo = null;

            using (var conn = DbHelp.DbProvider.CreateConnection(Config.DataBaseAuthority.Write))
            {
                result = DbHelp.DbProvider.ExecuteSql(conn, "employee", new
                {
                    /*Id = 7,*/
                    Account = employee.Account,
                    Name = employee.Name,
                    sq_Age = employee.Age.ToString(),
                    sq_Status = "20"
                }, SugarCommandType.AddTableDirect);
            }

            Assert.AreEqual(1, result, "新增-表名-单个匿名对象");
        }

        /// <summary>
        /// 新增-表名-多个匿名对象
        /// </summary>
        [TestMethod]
        public void TestA_Add2()
        {
            int result = 0;
            long id = 0;
            var employeeList = list.Skip(1).Take(2).ToList();
            using (var conn = DbHelp.DbProvider.CreateConnection())
            {
                result = DbHelp.DbProvider.ExecuteSql(conn, "employee", employeeList.Select(t => new
                {
                    Account = t.Account,
                    Name = t.Name,
                    Age = t.Age,
                    ig_Status = t.Status,
                    sq_Status = "@ig_Status"
                }), SugarCommandType.AddTableDirect);
            }

            Assert.AreEqual(2, result, "新增个数");
        }

        /// <summary>
        /// 新增-表名-单个实体对象
        /// </summary>
        [TestMethod]
        public void TestA_Add3()
        {
            int result = 0;
            var employee = list.Skip(3).Take(1).First();
            using (var conn = DbHelp.DbProvider.CreateConnection())
            {
                result = DbHelp.DbProvider.ExecuteSql(conn, "employee", employee, SugarCommandType.AddTableDirect);
            }

            Assert.AreEqual(1, result, "新增-表名-单个实体对象");
        }

        /// <summary>
        /// 新增-表名-多个个实体对象
        /// </summary>
        [TestMethod]
        public void TestA_Add4()
        {
            int result = 0;
            var employeeList = list.Skip(4).Take(2).ToList();
            using (var conn = DbHelp.DbProvider.CreateConnection())
            {
                result = DbHelp.DbProvider.ExecuteSql(conn, "employee", employeeList, SugarCommandType.AddTableDirect);
            }

            Assert.AreEqual(2, result, "新增个数");
        }

        /// <summary>
        /// 新增-表名-单个并返回自增主键
        /// </summary>
        [TestMethod]
        public void TestA_AddReturnKey()
        {
            long id = 0;
            var employee = list[0];
            EmployeeModel addInfo = null;

            using (var conn = DbHelp.DbProvider.CreateConnection(Config.DataBaseAuthority.Write))
            {
                id = DbHelp.DbProvider.AddReturnKey<int>(conn, "employee", new
                {
                    /*Id = 7,*/
                    Account = employee.Account,
                    Name = employee.Name,
                    sq_Age = employee.Age.ToString(),
                    sq_Status = "20"
                });

                addInfo = DbHelp.DbProvider.QuerySingle<EmployeeModel>(conn, "employee", new { Id = id }, SugarCommandType.QueryTableDirect);
            }

            Assert.IsNotNull(addInfo, "新增-表名-单个匿名对象");

            Assert.IsTrue(
                addInfo.Account == employee.Account
                && addInfo.Name == employee.Name
                && addInfo.Age == employee.Age
                && addInfo.Status == employee.Status, "新增-表名-单个匿名对象");
        }

        /// <summary>
        /// 修改-表名-匿名对象
        /// </summary>
        [TestMethod]
        public void TestB_Update1()
        {
            string name = "卢俊义2";
            using (var conn = DbHelp.DbProvider.CreateConnection())
            {
                var info = DbHelp.DbProvider.QuerySingle<EmployeeModel>(conn, "employee", new { sq_Id = "Id = 2" }, SugarCommandType.QueryTableDirect);

                Assert.IsNotNull(info, "查询单个对象");

                var result = DbHelp.DbProvider.ExecuteSql(conn, "employee", new
                {
                    Id = 2,
                    Account = "lujunyi",
                    Name = name,
                    sq_Age = "Age + 2",
                    Status = 20
                }, SugarCommandType.UpdateTableDirect);

                Assert.AreEqual(1, result, "修改-表名-匿名对象");

                var info2 = DbHelp.DbProvider.QueryList<EmployeeModel>(conn, "employee", new
                {
                    Id_gt = 0,
                    sq_1 = "(Id = 2",
                    sq_2 = "or Id = 3",
                    sq_3 = ")",
                    Status = 20
                }, SugarCommandType.QueryTableDirect, "Order By Id Asc").ToList();

                Assert.AreEqual(1, info2.Count, "查询数据个数");

                Assert.AreEqual(info.Age + 2, info2[0].Age, "比对修改数据Age");

                Assert.AreEqual(name, info2[0].Name, "比对修改数据Name");
            }
        }

        /// <summary>
        /// 修改-表名-多个实体对象
        /// </summary>
        [TestMethod]
        public void TestB_Update2()
        {
            var employeeList = new List<EmployeeModel>
            {
                new EmployeeModel
                {
                    Id = 1,
                    Account = "songjiang",
                    Name = "宋江",
                    Age = 50,
                    Status = EmployeeModel.EnumStatus.Complate
                },
                new EmployeeModel
                {
                    Id = 2,
                    Account = "lujunyi",
                    Name = "卢俊义",
                    Age = 48,
                    Status = EmployeeModel.EnumStatus.Disable
                }
            };

            using (var conn = DbHelp.DbProvider.CreateConnection())
            {
                var result = DbHelp.ExecuteSql("employee", employeeList, SugarCommandType.UpdateTableDirect);

                var addlist = DbHelp.DbProvider.QueryList<EmployeeModel>(conn, "employee", new { Id = new int[] { 1, 2 } }, SugarCommandType.QueryTableDirect, "Order By Id Asc").ToList();

                Assert.AreEqual(2, result, "修改-表名-多个实体对象");

                Assert.IsTrue(addlist.Count == 2
                    && addlist[0].Account == employeeList[0].Account
                    && addlist[0].Name == employeeList[0].Name
                    && addlist[0].Age == employeeList[0].Age
                    && addlist[0].Status == employeeList[0].Status

                    && addlist[1].Account == employeeList[1].Account
                    && addlist[1].Name == employeeList[1].Name
                    && addlist[1].Age == employeeList[1].Age
                    && addlist[1].Status == employeeList[1].Status
                    , "修改-表名-多个实体对象");
            }
        }

        /// <summary>
        /// 修改-表名-多个匿名实体对象
        /// </summary>
        [TestMethod]
        public void TestB_Update3()
        {
            var employeeList = new List<EmployeeModel>
            {
                new EmployeeModel
                {
                    Id = 1,
                    Account = "songjiang",
                    Name = "宋江",
                    Age = 50,
                    Status = EmployeeModel.EnumStatus.Complate
                },
                new EmployeeModel
                {
                    Id = 2,
                    Account = "lujunyi",
                    Name = "卢俊义",
                    Age = 48,
                    Status = EmployeeModel.EnumStatus.Disable
                }
            };

            List<object> paramList = new List<object>();
            foreach (var item in employeeList)
            {
                paramList.Add(new { Id = item.Id, Account = item.Account });
            }

            CommandCollection commands = new CommandCollection();
            commands.Add("employee", paramList, SugarCommandType.UpdateTableDirect);
            commands.Add("employee", list.Skip(2).Take(2).Select(t => new { Id = t.Id, Account = t.Account }).ToArray(), SugarCommandType.UpdateTableDirect);

            bool result = DbHelp.ExecuteSqlTran(commands);

            Assert.IsTrue(result, "修改-表名-多个匿名实体对象");
        }

        #endregion

        #region 查询

        /// <summary>
        /// 查询单个对象
        /// </summary>
        [TestMethod]
        public void TestC_Query1()
        {
            //查询单个对象
            var result = DbHelp.QuerySingle<EmployeeModel>("employee", new
            {
                ig_Account = "lujunyi",
                sq_Account = "Account=@ig_Account",
                Name_lk = "卢%",
                Status = new int[] { 10 },
                Age_gt = 47,
                Age_lt = 49,
            }, SugarCommandType.QueryTableDirect);

            Assert.IsNotNull(result, "查询单个对象");

            Assert.AreEqual(2, result.Id, "查询单个对象");
        }

        /// <summary>
        /// 查询多个对象
        /// </summary>
        [TestMethod]
        public void TestC_Query2()
        {
            //查询多个对象
            var result = DbHelp.QueryList<EmployeeModel>("employee", new { Age_ge = 50, Age_le = 50 }, SugarCommandType.QueryTableDirect).ToList();

            Assert.AreEqual(1, result.Count, "查询多个对象");

            Assert.AreEqual(1, result[0].Id, "查询多个对象");
        }

        /// <summary>
        /// 别名查询多个对象
        /// </summary>
        [TestMethod]
        public void TestC_Query3()
        {
            //别名查询多个对象
            //new { ue_e_Id = 1, ge_e_Age = 48 }
            var param = new { e_Id_ue = 1, e_Age_ge = 48 };
            //dynamic param = new ExpandoObject();
            //param.e_Age_ge = 48;
            //param.ue_e_Id = 1;
            //param.ge_e_Age = 48;
            var result = DbHelp.QueryList<EmployeeModel>("select * from employee e where", param, SugarCommandType.QuerySelectSql).ToList();

            Assert.AreEqual(1, result.Count, "别名查询多个对象错误");

            Assert.AreEqual(2, result[0].Id, "别名查询多个对象错误");
        }

        /// <summary>
        /// 带条件查询多个对象
        /// </summary>
        [TestMethod]
        public void TestC_Query4()
        {
            //带条件查询多个对象
            var result = DbHelp.QueryList<EmployeeModel>("select * from employee e where e.Status=20 and", new { e_Age_ge = 48 }, SugarCommandType.QuerySelectSql).ToList();

            Assert.AreEqual(1, result.Count, "带条件查询多个对象错误");

            Assert.AreEqual(1, result[0].Id, "带条件查询多个对象错误");
        }

        /// <summary>
        /// 查询个数
        /// </summary>
        [TestMethod]
        public void TestC_Query5()
        {
            //别名查询多个对象
            //new { ue_e_Id = 1, ge_e_Age = 48 }
            dynamic param = new { e_Id_ue = 1, e_Age_ge = 48 };
            //dynamic param = new ExpandoObject();
            //param.ue_e_Id = 1;
            //param.ge_e_Age = 48;
            int result = 0;
            List<EmployeeModel> result2 = null;
            using (var conn = DbHelp.DbProvider.CreateConnection())
            {
                result = DbHelp.DbProvider.QueryScalar<int>(conn, "select Count(*) from employee e where", param, SugarCommandType.QuerySelectSql);

                result2 = DbHelp.DbProvider.QueryList<EmployeeModel>(conn, "select * from employee e where e.Status=20 and", new { e_Age_ge = 48 }, SugarCommandType.QuerySelectSql).ToList();
            }

            Assert.AreEqual(1, result, "查询个数错误");

            Assert.AreEqual(1, result2.Count, "带条件查询多个对象错误");

            Assert.AreEqual(1, result2[0].Id, "带条件查询多个对象错误");

        }

        /// <summary>
        /// 分页查询多个对象 - 内存分页
        /// </summary>
        [TestMethod]
        public void TestC_QueryPaging1()
        {
            //带条件查询多个对象
            var result = DbHelp.QueryPagingList<EmployeeModel>(1, 2, "select * from employee e where e.Status>0 and", new { e_Id_ue = 1 }, SugarCommandType.QuerySelectSql);

            Assert.AreEqual(2, result.List.Count(), "带条件查询多个对象错误");

            Assert.AreEqual(4, result.List.FirstOrDefault().Id, "带条件查询多个对象错误");

        }

        /// <summary>
        /// 分页查询多个对象 - limit分页
        /// </summary>
        [TestMethod]
        public void TestC_QueryPaging2()
        {
            //带条件查询多个对象
            var result = DbHelp.QueryPagingList2<EmployeeModel>(1, 2, "select * from employee e where e.Status>0 and", new { e_Id_ue = 1 }, SugarCommandType.QuerySelectSql);

            Assert.AreEqual(2, result.List.Count(), "带条件查询多个对象错误");

            Assert.AreEqual(4, result.List.FirstOrDefault().Id, "带条件查询多个对象错误");

        }

        #endregion

        #region 存储过程

        /// <summary>
        /// 调用存储过程
        /// </summary>
        [TestMethod]
        public void TestD_StoredProcedure1()
        {
            //存储过程，根据存储名称调用存储过程
            var p = new DynamicParameters();
            p.Add("@startId", 6);

            var result = DbHelp.ExecuteSql("delete_data", p, SugarCommandType.StoredProcedure);

            Assert.AreEqual(7, result, "调用存储过程错误");
        }

        /// <summary>
        /// 调用存储过程
        /// </summary>
        [TestMethod]
        public void TestD_StoredProcedure2()
        {
            //存储过程，根据存储名称调用存储过程
            var result = DbHelp.ExecuteSql("delete_data", new { startId = 6 }, SugarCommandType.StoredProcedure);

            Assert.AreEqual(0, result, "调用存储过程错误");
        }

        #endregion
    }

    public class EmployeeModel
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public EnumStatus Status { get; set; }
        [IgnoreAdd, IgnoreUpdate]
        public DateTime CreateDate { get; set; }

        public enum EnumStatus
        {
            /// <summary>
            /// 删除
            /// </summary>
            Delete = 0,

            /// <summary>
            /// 禁用
            /// </summary>
            Disable = 10,

            /// <summary>
            /// 成功
            /// </summary>
            Complate = 20
        }
    }
}
