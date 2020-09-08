using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace UnitTest_Net451
{
    [TestClass]
    public class PerformanceTest
    {
        [TestMethod]
        public void TestQueryPerformance()
        {
            Stopwatch stopwatch1 = new Stopwatch();

            DbHelp.DbProvider.Builder.GetSelectSqlFromTableDirect("employee", new
            {
                Status = new List<int> { 20, 10 },
                sq_Age = "@ig_Age",
                Name_lk = "%sa%",
            });

            stopwatch1.Start();

            DbHelp.DbProvider.Builder.GetSelectSqlFromTableDirect("employee", new
            {
                Account1 = "sa",
                Status = new List<int> { 20, 10 },
                ig_Age = 20,
                sq_Age = "@ig_Age",
                Age_gt = 12,
                Name_lk = "%sa%",
            });

            stopwatch1.Stop();

            Console.WriteLine(stopwatch1.Elapsed.Ticks);
        }

        [TestMethod]
        public void TestInsertPerformance()
        {
            Stopwatch stopwatch1 = new Stopwatch();

            DbHelp.DbProvider.Builder.GetInsertSql("employee", new
            {
                Account = "sa",
            });

            stopwatch1.Start();

            var s = DbHelp.DbProvider.Builder.GetInsertSql("employee", new EmployeeModel
            {
                Id = 1,
                Account = "lujunyi",
                Name = "卢俊义",
                Status = EmployeeModel.EnumStatus.Complate,
                Age = 20,
            });

            stopwatch1.Stop();

            Console.WriteLine(stopwatch1.Elapsed.Ticks);
        }

        [TestMethod]
        public void TestUpdatePerformance()
        {
            Stopwatch stopwatch1 = new Stopwatch();

            DbHelp.DbProvider.Builder.GetUpdateSql("employee", new
            {
                Account = "sa",
            });

            stopwatch1.Start();

            var s = DbHelp.DbProvider.Builder.GetUpdateSql("employee", new
            {
                Account1 = "sa",
                Status = new List<int> { 20, 10 },
                ig_Age = 20,
                sq_Age = "@ig_Age",
                Age_gt = 12,
                Name_lk = "%sa%",
            });

            stopwatch1.Stop();

            Console.WriteLine(stopwatch1.Elapsed.Ticks);
        }

        [TestMethod]
        public void Test()
        {
            string aa = "Status_gt";
            string cc = "gt_Status";

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < 100; i++)
            {
                var b = aa.EndsWith("_gt");
            }

            stopwatch.Stop();

            Console.WriteLine(stopwatch.Elapsed.Ticks);

            stopwatch.Reset();

            stopwatch.Start();

            for (int i = 0; i < 100; i++)
            {
                var b = aa[aa.Length - 3] == '_' && aa[aa.Length - 2] == 'g' && aa[aa.Length - 1] == 't';
            }

            stopwatch.Stop();

            Console.WriteLine(stopwatch.Elapsed.Ticks);

            stopwatch.Reset();

            stopwatch.Start();

            for (int i = 0; i < 100; i++)
            {
                var b = cc.Substring(0, 3) == "gt_";
            }

            stopwatch.Stop();

            Console.WriteLine(stopwatch.Elapsed.Ticks);

            stopwatch.Reset();

            stopwatch.Start();

            for (int i = 0; i < 100; i++)
            {
                var b = cc[0] == 'g' && cc[1] == 't' && cc[2] == '_';
            }

            stopwatch.Stop();

            Console.WriteLine(stopwatch.Elapsed.Ticks);
        }
    }
}
