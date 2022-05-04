using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest_NetCore
{
    [TestClass]
    public class SqlTest
    {
        /// <summary>
        /// 查询语句
        /// </summary>
        [TestMethod]
        public void TestQuerySQL()
        {
            string strNull = null;
            var sql1 = DbHelp.DbProvider.Builder.GetSelectSqlFromSelectSql("SELECT * FROM `employee`", new
            {
                Id = 2,
                ig_1 = "aaa",
                sq_Account = "Account = 'lujunyi'",
                sq_ignore = strNull,
                Id_ue = 3,
                Id_ne = 4,
                Name_lk = "卢俊%",
                Age_gt = 10,
                Age_ge = 15,

                Age_lt = 40,
                Age_le = 35,

                Status = new int[] { 10, 20 }
            });

            var sql2 = DbHelp.DbProvider.Builder.GetSelectSqlFromSelectSql("SELECT * FROM `employee` WHERE", new
            {
                Id = 2,
                ig_1 = "aaa",
                sq_Account = "Account = 'lujunyi'",
                Id_ue = 3,
                Id_ne = 4,
                Name_lk = "卢俊%",
                Age_gt = 10,
                Age_ge = 15,

                Age_lt = 40,
                Age_le = 35,

                Status = new int[] { 10, 20 }
            });

            var sql3 = DbHelp.DbProvider.Builder.GetSelectSqlFromTableDirect("employee", new
            {
                Id = 2,
                ig_1 = "aaa",
                sq_Account = "Account = 'lujunyi'",
                Id_ue = 3,
                Id_ne = 4,
                Name_lk = "卢俊%",
                Age_gt = 10,
                Age_ge = 15,

                Age_lt = 40,
                Age_le = 35,

                Status = new int[] { 10, 20 }
            });

            Assert.AreEqual("SELECT * FROM `employee` WHERE `Id` = @Id AND Account = 'lujunyi' AND `Id` <> @Id_ue AND `Id` <> @Id_ne AND `Name` like @Name_lk AND `Age` > @Age_gt AND `Age` >= @Age_ge AND `Age` < @Age_lt AND `Age` <= @Age_le AND `Status` in @Status", sql1.Trim());
            Assert.AreEqual("SELECT * FROM `employee` WHERE `Id` = @Id AND Account = 'lujunyi' AND `Id` <> @Id_ue AND `Id` <> @Id_ne AND `Name` like @Name_lk AND `Age` > @Age_gt AND `Age` >= @Age_ge AND `Age` < @Age_lt AND `Age` <= @Age_le AND `Status` in @Status", sql2.Trim());
            Assert.AreEqual("SELECT * FROM `employee` WHERE `Id` = @Id AND Account = 'lujunyi' AND `Id` <> @Id_ue AND `Id` <> @Id_ne AND `Name` like @Name_lk AND `Age` > @Age_gt AND `Age` >= @Age_ge AND `Age` < @Age_lt AND `Age` <= @Age_le AND `Status` in @Status", sql3.Trim());
        }

        /// <summary>
        /// 新增语句
        /// </summary>
        [TestMethod]
        public void TestInsertSQL()
        {
            var sql1 = DbHelp.DbProvider.Builder.GetInsertSql("employee", new EmployeeModel
            {
                Id = 8,
                Account = "lujunyi",
                Name = "卢俊义",
                Age = 50,
                Status = EmployeeModel.EnumStatus.Complate,
            });

            var sql2 = DbHelp.DbProvider.Builder.GetInsertSql("employee", new
            {
                Id = 8,
                Account = "lujunyi",
                Name = "卢俊义",
                sq_Age = "50",
                Status = EmployeeModel.EnumStatus.Complate,
                ig_A = 1,
            });

            Assert.AreEqual("INSERT INTO `employee`(`Id`,`Account`,`Name`,`Age`,`Status`) VALUES(@Id,@Account,@Name,@Age,@Status);", sql1.Trim());
            Assert.AreEqual("INSERT INTO `employee`(`Id`,`Account`,`Name`,`Age`,`Status`) VALUES(@Id,@Account,@Name,50,@Status);", sql2.Trim());
        }

        /// <summary>
        /// 修改语句
        /// </summary>
        [TestMethod]
        public void TestUpdateSql()
        {
            var sql1 = DbHelp.DbProvider.Builder.GetUpdateSql("employee", new EmployeeModel
            {
                Id = 8,
                Account = "lujunyi",
                Name = "卢俊义",
                Age = 50,
                Status = EmployeeModel.EnumStatus.Complate,
            });

            var sql2 = DbHelp.DbProvider.Builder.GetUpdateSql("employee", new
            {
                Id = 8,
                Account = "lujunyi",
                Name = "卢俊义",
                sq_Age = "50",
                Status = EmployeeModel.EnumStatus.Complate,
                ig_A = 1,
            });

            Assert.AreEqual("UPDATE `employee` SET `Account` = @Account,`Name` = @Name,`Age` = @Age,`Status` = @Status WHERE `Id` = @Id;", sql1.Trim());
            Assert.AreEqual("UPDATE `employee` SET `Account` = @Account,`Name` = @Name,`Age`=50,`Status` = @Status WHERE `Id` = @Id;", sql2.Trim());
        }

        [TestMethod]
        public void TestConditionSql()
        {
            string text = "6050";
            var sql = DbHelp.DbProvider.Builder.GetConditionSqlByParam(new
            {
                sq_no = text != null ? "(callno like @ig_no or recno like @ig_no)" : null,
                ig_no = new long[] { 1, 2 },
                order_id = 1,
                order_gd = 2,
                order_ld = 3,
            });
            Assert.AreEqual("(callno like @ig_no or recno like @ig_no) AND `order_id` = @order_id AND `order_gd` = @order_gd AND `order_ld` = @order_ld", sql.Trim());
        }
    }
}
