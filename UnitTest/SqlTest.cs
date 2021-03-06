﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var sql1 = DbHelp.DbProvider.Builder.GetSelectSqlFromSelectSql("SELECT * FROM `employee`", new
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

            Assert.AreEqual(sql1.Trim(), "SELECT * FROM `employee` WHERE `Id` = @Id AND Account = 'lujunyi' AND `Id` <> @Id_ue AND `Id` <> @Id_ne AND `Name` like @Name_lk AND `Age` > @Age_gt AND `Age` >= @Age_ge AND `Age` < @Age_lt AND `Age` <= @Age_le AND `Status` in @Status");
            Assert.AreEqual(sql2.Trim(), "SELECT * FROM `employee` WHERE `Id` = @Id AND Account = 'lujunyi' AND `Id` <> @Id_ue AND `Id` <> @Id_ne AND `Name` like @Name_lk AND `Age` > @Age_gt AND `Age` >= @Age_ge AND `Age` < @Age_lt AND `Age` <= @Age_le AND `Status` in @Status");
            Assert.AreEqual(sql3.Trim(), "SELECT * FROM `employee` WHERE `Id` = @Id AND Account = 'lujunyi' AND `Id` <> @Id_ue AND `Id` <> @Id_ne AND `Name` like @Name_lk AND `Age` > @Age_gt AND `Age` >= @Age_ge AND `Age` < @Age_lt AND `Age` <= @Age_le AND `Status` in @Status");
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

            Assert.AreEqual(sql1.Trim(), "INSERT INTO `employee`(`Id`,`Account`,`Name`,`Age`,`Status`) VALUES(@Id,@Account,@Name,@Age,@Status);");
            Assert.AreEqual(sql2.Trim(), "INSERT INTO `employee`(`Id`,`Account`,`Name`,`Age`,`Status`) VALUES(@Id,@Account,@Name,50,@Status);");
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

            Assert.AreEqual(sql1.Trim(), "UPDATE `employee` SET `Account` = @Account,`Name` = @Name,`Age` = @Age,`Status` = @Status WHERE `Id` = @Id;");
            Assert.AreEqual(sql2.Trim(), "UPDATE `employee` SET `Account` = @Account,`Name` = @Name,`Age`=50,`Status` = @Status WHERE `Id` = @Id;");
        }
    }
}
