using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Sugar;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace TestCore.Controllers
{
    /// <summary>
    /// 用户
    /// </summary>    
    public partial class UserModel
    {

        /// <summary>
        /// 编号
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCardNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string WXOpenId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string WXUnionId { get; set; }

        /// <summary>
        /// 地域编号
        /// </summary>
        public int? RegionId { get; set; }

        /// <summary>
        /// 最近一次登录时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 状态[0：删除 5：禁用 20：正常]
        /// </summary>
        [IgnoreUpdate]
        public int Status { get; set; }

        /// <summary>
        /// 类型[0：默认]
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [IgnoreAdd]
        [IgnoreUpdate]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [JsonIgnore]
        [IgnoreAdd]
        [IgnoreUpdate]
        public DateTime UpdateTime { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public static DbProvider DbProvider = DbProvider.CreateDbProvide("mysql");

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {

            using (DbConnection conn = DbProvider.CreateConnection(Config.DataBaseAuthority.Read))
            {
                var a = DbProvider.QuerySingle<UserModel>(conn, "user", new { AAAA = 1 }, SugarCommandType.QueryTableDirect);
            }

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
