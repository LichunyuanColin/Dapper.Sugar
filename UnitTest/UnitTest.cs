using Dapper;
using Dapper.Sugar;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace UnitTest_NetCore
{
    [TestClass]
    public class UnitTest
    {
        List<EmployeeModel> list = new List<EmployeeModel>()
        {
            new EmployeeModel
            {
                Account = "qinming",
                Name = "����",
                Age = 36,
                Status = EmployeeModel.EnumStatus.Complate,
            },
            new EmployeeModel
            {
                Account = "huyanzhuo",
                Name = "������",
                Age = 36,
                Status = EmployeeModel.EnumStatus.Complate,
            },
            new EmployeeModel
            {
                Account = "huarong",
                Name = "����",
                Age = 34,
                Status = EmployeeModel.EnumStatus.Complate,
            },
            new EmployeeModel
            {
                Account = "caijin",
                Name = "���",
                Age = 34,
                Status = EmployeeModel.EnumStatus.Complate,
            },
            new EmployeeModel
            {
                Account = "liying",
                Name = "��Ӧ",
                Age = 34,
                Status = EmployeeModel.EnumStatus.Complate,
            },
            new EmployeeModel
            {
                Account = "zhutong",
                Name = "����",
                Age = 34,
                Status = EmployeeModel.EnumStatus.Complate,
            },
            new EmployeeModel
            {
                Account = "luzhishen",
                Name = "³����",
                Age = 34,
                Status = EmployeeModel.EnumStatus.Complate,
            },
            new EmployeeModel
            {
                Account = "wusong",
                Name = "����",
                Age = 34,
                Status = EmployeeModel.EnumStatus.Complate,
            }
        };


        #region ����

        [TestMethod]
        public void Test()
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
        public void Test1()
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
        public void Test2()
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

        /// <summary>
        /// ����-����-������������
        /// </summary>
        [TestMethod]
        public void TestAdd1()
        {
            int result = 0;
            long id = 0;
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

                id = DbHelp.DbProvider.QueryAutoIncrement(conn);

                addInfo = DbHelp.DbProvider.QuerySingle<EmployeeModel>(conn, "employee", new { Id = id }, SugarCommandType.QueryTableDirect);
            }

            Assert.AreEqual(result, 1, "����-����-������������");

            Assert.IsNotNull(addInfo, "����-����-������������");

            Assert.IsTrue(
                addInfo.Account == employee.Account
                && addInfo.Name == employee.Name
                && addInfo.Age == employee.Age
                && addInfo.Status == employee.Status, "����-����-������������");
        }

        /// <summary>
        /// ����-����-�����������
        /// </summary>
        [TestMethod]
        public void TestAdd2()
        {
            int result = 0;
            long id = 0;
            var employeeList = list.Skip(1).Take(2).ToList();
            List<EmployeeModel> addlist;

            using (var conn = DbHelp.DbProvider.CreateConnection())
            {
                result = DbHelp.ExecuteSql("employee", employeeList.Select(t => new
                {
                    Account = t.Account,
                    Name = t.Name,
                    Age = t.Age,
                    ig_Status = t.Status,
                    sq_Status = "@ig_Status"
                }), SugarCommandType.AddTableDirect);

                id = DbHelp.DbProvider.QueryAutoIncrement(conn);

                addlist = DbHelp.DbProvider.QueryList<EmployeeModel>(conn, "employee", new { Id_lt = id }, SugarCommandType.QueryTableDirect, "Order By Id desc limit 2").ToList();
            }

            Assert.AreEqual(result, 2, "��-����-�����������");

            Assert.IsTrue(addlist.Count == 2
                && addlist[0].Account == employeeList[1].Account
                && addlist[0].Name == employeeList[1].Name
                && addlist[0].Age == employeeList[1].Age
                && addlist[0].Status == employeeList[1].Status

                && addlist[1].Account == employeeList[0].Account
                && addlist[1].Name == employeeList[0].Name
                && addlist[1].Age == employeeList[0].Age
                && addlist[1].Status == employeeList[0].Status
                , "��-����-�����������");
        }

        /// <summary>
        /// ����-����-����ʵ�����
        /// </summary>
        [TestMethod]
        public void TestAdd3()
        {
            int result = 0;
            long id = 0;
            var employee = list.Skip(3).Take(1).First();
            EmployeeModel addInfo = null;

            using (var conn = DbHelp.DbProvider.CreateConnection())
            {
                result = DbHelp.ExecuteSql("employee", employee, SugarCommandType.AddTableDirect);

                id = DbHelp.DbProvider.QueryAutoIncrement(conn);

                addInfo = DbHelp.DbProvider.QuerySingle<EmployeeModel>(conn, "select * from employee", new { Id = id }, SugarCommandType.QuerySelectSql);
            }

            Assert.AreEqual(result, 1, "����-����-����ʵ�����");

            Assert.IsNotNull(addInfo, "����-����-������������");

            Assert.IsTrue(
                addInfo.Account == employee.Account
                && addInfo.Name == employee.Name
                && addInfo.Age == employee.Age
                && addInfo.Status == employee.Status, "����-����-������������");
        }

        /// <summary>
        /// ����-����-�����ʵ�����
        /// </summary>
        [TestMethod]
        public void TestAdd4()
        {
            int result = 0;
            long id = 0;
            var employeeList = list.Skip(4).Take(2).ToList();
            List<EmployeeModel> addlist;

            using (var conn = DbHelp.DbProvider.CreateConnection())
            {
                result = DbHelp.ExecuteSql("employee", employeeList, SugarCommandType.AddTableDirect);

                id = DbHelp.DbProvider.QueryAutoIncrement(conn);

                addlist = DbHelp.DbProvider.QueryList<EmployeeModel>(conn, "employee", new { Id_lt = id }, SugarCommandType.QueryTableDirect, "Order By Id desc limit 2").ToList();
            }

            Assert.AreEqual(result, 2, "��-����-�����������");

            Assert.IsTrue(addlist.Count == 2
                && addlist[0].Account == employeeList[1].Account
                && addlist[0].Name == employeeList[1].Name
                && addlist[0].Age == employeeList[1].Age
                && addlist[0].Status == employeeList[1].Status

                && addlist[1].Account == employeeList[0].Account
                && addlist[1].Name == employeeList[0].Name
                && addlist[1].Age == employeeList[0].Age
                && addlist[1].Status == employeeList[0].Status
                , "��-����-�����������");
        }


        /// <summary>
        /// �޸�-����-��������
        /// </summary>
        [TestMethod]
        public void TestUpdate1()
        {
            string name = "¬����2";
            using (var conn = DbHelp.DbProvider.CreateConnection())
            {
                var info = DbHelp.DbProvider.QuerySingle<EmployeeModel>(conn, "employee", new { sq_Id = "Id = 2" }, SugarCommandType.QueryTableDirect);

                Assert.IsNotNull(info, "��ѯ��������");

                var result = DbHelp.DbProvider.ExecuteSql(conn, "employee", new
                {
                    Id = 2,
                    Account = "lujunyi",
                    Name = name,
                    sq_Age = "Age + 2",
                    Status = 20
                }, SugarCommandType.UpdateTableDirect);

                Assert.AreEqual(result, 1, "�޸�-����-��������");

                var info2 = DbHelp.DbProvider.QueryList<EmployeeModel>(conn, "employee", new
                {
                    Id_gt = 0,
                    sq_1 = "(Id = 2",
                    sq_2 = "or Id = 3",
                    sq_3 = ")",
                    Status = 20
                }, SugarCommandType.QueryTableDirect, "Order By Id Asc").ToList();

                Assert.AreEqual(info2.Count, 2, "��ѯ�������");

                Assert.AreEqual(info2[0].Age, info.Age + 2, "�޸�-����-��������");

                Assert.AreEqual(info2[0].Name, name, "�޸�-����-��������");
            }
        }

        /// <summary>
        /// �޸�-����-���ʵ�����
        /// </summary>
        [TestMethod]
        public void TestUpdate2()
        {
            var employeeList = new List<EmployeeModel>
            {
                new EmployeeModel
                {
                    Id = 1,
                    Account = "songjiang",
                    Name = "�ν�",
                    Age = 50,
                    Status = EmployeeModel.EnumStatus.Complate
                },
                new EmployeeModel
                {
                    Id = 2,
                    Account = "lujunyi",
                    Name = "¬����",
                    Age = 48,
                    Status = EmployeeModel.EnumStatus.Disable
                }
            };

            using (var conn = DbHelp.DbProvider.CreateConnection())
            {
                var result = DbHelp.ExecuteSql("employee", employeeList, SugarCommandType.UpdateTableDirect);

                var addlist = DbHelp.DbProvider.QueryList<EmployeeModel>(conn, "employee", new { Id = new int[] { 1, 2 } }, SugarCommandType.QueryTableDirect, "Order By Id Asc").ToList();

                Assert.AreEqual(result, 2, "�޸�-����-���ʵ�����");

                Assert.IsTrue(addlist.Count == 2
                    && addlist[0].Account == employeeList[0].Account
                    && addlist[0].Name == employeeList[0].Name
                    && addlist[0].Age == employeeList[0].Age
                    && addlist[0].Status == employeeList[0].Status

                    && addlist[1].Account == employeeList[1].Account
                    && addlist[1].Name == employeeList[1].Name
                    && addlist[1].Age == employeeList[1].Age
                    && addlist[1].Status == employeeList[1].Status
                    , "�޸�-����-���ʵ�����");
            }
        }

        #endregion

        #region ��ѯ

        /// <summary>
        /// ��ѯ��������
        /// </summary>
        [TestMethod]
        public void TestQuery1()
        {
            //��ѯ��������
            var result = DbHelp.QuerySingle<EmployeeModel>("employee", new
            {
                ig_Account = "lujunyi",
                sq_Account = "Account=@ig_Account",
                Name_lk = "¬%",
                Status = new int[] { 10 },
                Age_gt = 47,
                Age_lt = 49
            }, SugarCommandType.QueryTableDirect);

            Assert.IsNotNull(result, "��ѯ��������");

            Assert.AreEqual(result.Id, 2, "��ѯ��������");
        }

        /// <summary>
        /// ��ѯ�������
        /// </summary>
        [TestMethod]
        public void TestQuery2()
        {
            //��ѯ�������
            var result = DbHelp.QueryList<EmployeeModel>("employee", new { Age_ge = 50, Age_le = 50 }, SugarCommandType.QueryTableDirect).ToList();

            Assert.AreEqual(result.Count, 1, "��ѯ�������");

            Assert.AreEqual(result[0].Id, 1, "��ѯ�������");
        }

        /// <summary>
        /// ������ѯ�������
        /// </summary>
        [TestMethod]
        public void TestQuery3()
        {
            //������ѯ�������
            //new { ue_e_Id = 1, ge_e_Age = 48 }
            var param = new { e_Id_ue = 1, e_Age_ge = 48 };
            //dynamic param = new ExpandoObject();
            //param.e_Age_ge = 48;
            //param.ue_e_Id = 1;
            //param.ge_e_Age = 48;
            var result = DbHelp.QueryList<EmployeeModel>("select * from employee e where", param, SugarCommandType.QuerySelectSql).ToList();

            Assert.AreEqual(result.Count, 1, "������ѯ����������");

            Assert.AreEqual(result[0].Id, 2, "������ѯ����������");
        }

        /// <summary>
        /// ��������ѯ�������
        /// </summary>
        [TestMethod]
        public void TestQuery4()
        {
            //��������ѯ�������
            var result = DbHelp.QueryList<EmployeeModel>("select * from employee e where e.Status=20 and", new { e_Age_ge = 48 }, SugarCommandType.QuerySelectSql).ToList();

            Assert.AreEqual(result.Count, 1, "��������ѯ����������");

            Assert.AreEqual(result[0].Id, 1, "��������ѯ����������");
        }

        /// <summary>
        /// ��ѯ����
        /// </summary>
        [TestMethod]
        public void TestQuery5()
        {
            //������ѯ�������
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

            Assert.AreEqual(result, 1, "��ѯ��������");

            Assert.AreEqual(result2.Count, 1, "��������ѯ����������");

            Assert.AreEqual(result2[0].Id, 1, "��������ѯ����������");

        }

        /// <summary>
        /// ��ҳ��ѯ������� - �ڴ��ҳ
        /// </summary>
        [TestMethod]
        public void TestQueryPaging1()
        {
            //��������ѯ�������
            var result = DbHelp.QueryPagingList<EmployeeModel>(1, 2, "select * from employee e where e.Status>0 and", new { e_Id_ue = 1 }, SugarCommandType.QuerySelectSql);

            Assert.AreEqual(result.List.Count(), 2, "��������ѯ����������");

            Assert.AreEqual(result.List.FirstOrDefault().Id, 4, "��������ѯ����������");

        }

        /// <summary>
        /// ��ҳ��ѯ������� - limit��ҳ
        /// </summary>
        [TestMethod]
        public void TestQueryPaging2()
        {
            //��������ѯ�������
            var result = DbHelp.QueryPagingList2<EmployeeModel>(1, 2, "select * from employee e where e.Status>0 and", new { e_Id_ue = 1 }, SugarCommandType.QuerySelectSql);

            Assert.AreEqual(result.List.Count(), 2, "��������ѯ����������");

            Assert.AreEqual(result.List.FirstOrDefault().Id, 4, "��������ѯ����������");

        }

        #endregion

        #region �洢����

        /// <summary>
        /// ���ô洢����
        /// </summary>
        [TestMethod]
        public void TestStoredProcedure1()
        {
            //�洢���̣����ݴ洢���Ƶ��ô洢����
            var p = new DynamicParameters();
            p.Add("@startId", 6);

            var result = DbHelp.ExecuteSql("delete_data", p, SugarCommandType.StoredProcedure);

            Assert.AreEqual(result, 6, "���ô洢���̴���");
        }

        /// <summary>
        /// ���ô洢����
        /// </summary>
        [TestMethod]
        public void TestStoredProcedure2()
        {
            //�洢���̣����ݴ洢���Ƶ��ô洢����
            var result = DbHelp.ExecuteSql("delete_data", new { startId = 6 }, SugarCommandType.StoredProcedure);

            Assert.AreEqual(result, 0, "���ô洢���̴���");
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
            /// ɾ��
            /// </summary>
            Delete = 0,

            /// <summary>
            /// ����
            /// </summary>
            Disable = 10,

            /// <summary>
            /// �ɹ�
            /// </summary>
            Complate = 20
        }
        public class AA
        {
            /// <summary>
            /// 
            /// </summary>
            public string Name { get; set; }
        }
    }
}
