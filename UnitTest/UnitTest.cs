using Dapper;
using Dapper.Sugar;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace UnitTest
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
        /// 新增-表名-单个匿名对象
        /// </summary>
        [TestMethod]
        public void TestAdd1()
        {
            var result = DbHelp.ExecuteSql("employee", new
            {
                /*Id = 7,*/
                Account = "qinming",
                Name = "秦明",
                sq_Age = "38",
                sq_Status = "20"
            }, SugarCommandType.AddTableDirect);

            Assert.AreEqual(result, 1, "新增-表名-单个匿名对象");
        }

        /// <summary>
        /// 新增-表名-多个匿名对象
        /// </summary>
        [TestMethod]
        public void TestAdd2()
        {
            var result = DbHelp.ExecuteSql("employee", list.Skip(1).Take(2).Select(t => new
            {
                Account = t.Account,
                Name = t.Name,
                Age = t.Age,
                ig_Status = t.Status,
                sq_Status = "@ig_Status"
            }).ToList(), SugarCommandType.AddTableDirect);

            Assert.AreEqual(result, 2, "增-表名-多个匿名对象");
        }

        /// <summary>
        /// 新增-表名-单个实体对象
        /// </summary>
        [TestMethod]
        public void TestAdd3()
        {
            var result = DbHelp.ExecuteSql("employee", list.Skip(3).Take(1).First(), SugarCommandType.AddTableDirect);

            Assert.AreEqual(result, 1, "新增-表名-单个实体对象");
        }

        /// <summary>
        /// 新增-表名-多个个实体对象
        /// </summary>
        [TestMethod]
        public void TestAdd4()
        {
            var result = DbHelp.ExecuteSql("employee", list.Skip(4).Take(2).ToList(), SugarCommandType.AddTableDirect);

            Assert.AreEqual(result, 2, "新增-表名-多个个实体对象");
        }


        /// <summary>
        /// 修改-表名-匿名对象
        /// </summary>
        [TestMethod]
        public void TestUpdate1()
        {
            using (var conn = DbHelp.DbProvider.CreateConnection())
            {
                var info = DbHelp.DbProvider.QuerySingle<EmployeeModel>(conn, "employee", new { sq_Id = "Id = 1" }, SugarCommandType.QueryTableDirect);

                Assert.IsNotNull(info, "查询单个对象");

                var result = DbHelp.DbProvider.ExecuteSql(conn, "employee", new
                {
                    Id = 1,
                    Account = "songjiang",
                    Name = "宋江",
                    sq_Age = "Age + 2",
                    Status = 20
                }, SugarCommandType.UpdateTableDirect);

                Assert.AreEqual(result, 1, "修改-表名-匿名对象");

                var info2 = DbHelp.DbProvider.QueryList<EmployeeModel>(conn, "employee", new
                {
                    Id_gt = 0,
                    sq_1 = "(Id = 1",
                    sq_2 = "or Id = 2",
                    sq_3 = ")",
                    Status = 20
                }, SugarCommandType.QueryTableDirect);

                Assert.AreEqual(info2.Count, 2, "查询多个对象");

                Assert.AreEqual(info2[0].Age, info.Age + 2, "修改-表名-匿名对象");
            }
        }

        /// <summary>
        /// 修改-表名-多个实体对象
        /// </summary>
        [TestMethod]
        public void TestUpdate2()
        {
            var result = DbHelp.ExecuteSql("employee", new List<EmployeeModel>
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
            }, SugarCommandType.UpdateTableDirect);

            Assert.AreEqual(result, 2, "修改-表名-多个实体对象");

        }

        #endregion

        #region 查询

        /// <summary>
        /// 查询单个对象
        /// </summary>
        [TestMethod]
        public void TestQuery1()
        {
            //查询单个对象
            var result = DbHelp.QuerySingle<EmployeeModel>("employee", new
            {
                ig_Account = "songjiang",
                sq_Account = "Account=@ig_Account",
                Name_lk = "宋%",
                Status = new int[] { 20 },
                Age_gt = 40,
                Age_lt = 52
            }, SugarCommandType.QueryTableDirect);

            Assert.IsNotNull(result, "查询单个对象");

            Assert.AreEqual(result.Id, 1, "查询单个对象");
        }

        /// <summary>
        /// 查询多个对象
        /// </summary>
        [TestMethod]
        public void TestQuery2()
        {
            //查询多个对象
            var result = DbHelp.QueryList<EmployeeModel>("employee", new { Age_ge = 50, Age_le = 50 }, SugarCommandType.QueryTableDirect);

            Assert.AreEqual(result.Count, 1, "查询多个对象");
        }

        /// <summary>
        /// 别名查询多个对象
        /// </summary>
        [TestMethod]
        public void TestQuery3()
        {
            //别名查询多个对象
            //new { ue_e_Id = 1, ge_e_Age = 48 }
            dynamic param = new { e_Id_ue = 1, e_Age_ge = 48 };
            //dynamic param = new ExpandoObject();
            //param.e_Age_ge = 48;
            //param.ue_e_Id = 1;
            //param.ge_e_Age = 48;
            var result = DbHelp.QueryList<EmployeeModel>("select * from employee e where", param, SugarCommandType.QuerySelectSql);

            Assert.AreEqual(result.Count, 1, "别名查询多个对象错误");

        }

        /// <summary>
        /// 带条件查询多个对象
        /// </summary>
        [TestMethod]
        public void TestQuery4()
        {
            //带条件查询多个对象
            var result = DbHelp.QueryList<EmployeeModel>("select * from employee e where e.Status=20 and", new { e_Age_ge = 48 }, SugarCommandType.QuerySelectSql);

            Assert.AreEqual(result.Count, 1, "带条件查询多个对象错误");

            Assert.AreEqual(result[0].Id, 1, "带条件查询多个对象错误");
        }

        /// <summary>
        /// 查询个数
        /// </summary>
        [TestMethod]
        public void TestQuery5()
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

                result2 = DbHelp.DbProvider.QueryList<EmployeeModel>(conn, "select * from employee e where e.Status=20 and", new { e_Age_ge = 48 }, SugarCommandType.QuerySelectSql);
            }

            Assert.AreEqual(result, 1, "查询个数错误");

            Assert.AreEqual(result2.Count, 1, "带条件查询多个对象错误");

            Assert.AreEqual(result2[0].Id, 1, "带条件查询多个对象错误");

        }

        /// <summary>
        /// 分页查询多个对象 - 内存分页
        /// </summary>
        [TestMethod]
        public void TestQueryPaging1()
        {
            //带条件查询多个对象
            var result = DbHelp.QueryPagingList<EmployeeModel>(1, 2, "select * from employee e where e.Status>0 and", new { e_Id_ue = 1 }, SugarCommandType.QuerySelectSql);

            Assert.AreEqual(result.List.Count, 2, "带条件查询多个对象错误");

            Assert.AreEqual(result.List[0].Id, 4, "带条件查询多个对象错误");

        }

        /// <summary>
        /// 分页查询多个对象 - limit分页
        /// </summary>
        [TestMethod]
        public void TestQueryPaging2()
        {
            //带条件查询多个对象
            var result = DbHelp.QueryPagingList2<EmployeeModel>(1, 2, "select * from employee e where e.Status>0 and", new { e_Id_ue = 1 }, SugarCommandType.QuerySelectSql);

            Assert.AreEqual(result.List.Count, 2, "带条件查询多个对象错误");

            Assert.AreEqual(result.List[0].Id, 4, "带条件查询多个对象错误");

        }

        #endregion

        #region 存储过程

        /// <summary>
        /// 调用存储过程
        /// </summary>
        [TestMethod]
        public void TestStoredProcedure1()
        {
            //存储过程，根据存储名称调用存储过程
            var p = new DynamicParameters();
            p.Add("@startId", 6);

            var result = DbHelp.ExecuteSql("delete_data", p, SugarCommandType.StoredProcedure);

            Assert.AreEqual(result, 6, "调用存储过程错误");
        }

        /// <summary>
        /// 调用存储过程
        /// </summary>
        [TestMethod]
        public void TestStoredProcedure2()
        {
            //存储过程，根据存储名称调用存储过程
            var result = DbHelp.ExecuteSql("delete_data", new { startId = 6 }, SugarCommandType.StoredProcedure);

            Assert.AreEqual(result, 0, "调用存储过程错误");
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
