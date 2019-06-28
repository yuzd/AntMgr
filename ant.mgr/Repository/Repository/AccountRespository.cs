using AntData.ORM;
using AntData.ORM.Linq;
using Autofac.Annotation;
using Castle.DynamicProxy;
using Configuration;
using Infrastructure.Logging;
using Infrastructure.StaticExt;
using Infrastructure.Web;
using Repository.Interface;
using ServicesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbModel;
using ViewModels.Reuqest;

namespace Repository
{
    /// <summary>
    /// 系统用户
    /// </summary>
    [Bean(typeof(IAccountRespository), Interceptor = typeof(AsyncInterceptor))]
    public class AccountRespository : BaseRepository<SystemUsers>, IAccountRespository
    {

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> LogOn(LogOnVM info)
        {
            try
            {
                if (info == null || string.IsNullOrEmpty(info.eid) || string.IsNullOrEmpty(info.pwd))
                {
                    return new Tuple<bool, string>(false, Tip.BadRequest);
                }
                var existUser = await this.Entity.FirstOrDefaultAsync(r => r.IsActive && r.Eid.Equals(info.eid));

                if (existUser == null)
                {
                    return new Tuple<bool, string>(false, "该账号不存在,请联系系统管理员!");
                }

                var pwd = CodingUtils.MD5(info.pwd);
                if (!existUser.Pwd.Equals(pwd))
                {
                    return new Tuple<bool, string>(false, "密码错误!");
                }


                var systemUserAndRole = await (from u in this.Entity
                                               from ro in this.Entitys.SystemRole.Where(r => r.Tid.Equals(u.RoleTid)).DefaultIfEmpty()
                                               where u.Eid.Equals(info.eid)
                                               select new { user = u, role = ro }).FirstOrDefaultAsync();

                var systemUser = systemUserAndRole != null ? systemUserAndRole.user : null;

                if (systemUser == null)
                {
                    return new Tuple<bool, string>(false, "该账号不存在,请联系系统管理员!");
                }

                if (!systemUser.IsActive)
                {
                    return new Tuple<bool, string>(false, "该账号已被禁用,请联系系统管理员!");
                }

                var role = systemUserAndRole.role;
                if (role == null)
                {
                    role = new SystemRole();
                }

                var loginIp = WebUtils.GetClientIP();
                var userAgent = WebUtils.GetUserAgent();
                var eid = info.eid.ToLower();


                //更新
                var updateQuery = this.Entity.Where(r => r.Eid.Equals(eid))
                    .Set(r => r.LoginIp, loginIp)
                    .Set(r => r.LastLoginTime, DateTime.Now)
                    .Set(r => r.UserAgent, userAgent);


                //如果role不存在 也就是没有角色 menurights却有值 
                if (string.IsNullOrEmpty(role.RoleName) && !string.IsNullOrEmpty(systemUser.MenuRights))
                {
                    updateQuery = updateQuery.Set(r => r.MenuRights, string.Empty);
                }

                var updateResult = await updateQuery.UpdateAsync() > 0;
                if (!updateResult)
                {
                    return new Tuple<bool, string>(false, "用户信息更新出错!");
                }


                WriteLoginCookie(new Token
                {
                    Code = systemUser.UserName,
                    Eid = eid,
                    MenuRights = systemUser.MenuRights,
                    RoleTid = role.Tid,
                    RoleName = role.RoleName,
                });
                return new Tuple<bool, string>(true, null);
            }
            catch (Exception ex)
            {
                LogHelper.Warn("login", ex);
                return new Tuple<bool, string>(false, "登录出错");
            }
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<Tuple<long, List<UserSM>>> GetUserList(AccountVm model, Token user)
        {
            if (model == null) { return new Tuple<long, List<UserSM>>(0, new List<UserSM>()); }

            var totalQuery = this.Entity.Where(r => r.IsActive);
            var listQuery = this.Entity.Where(r => r.IsActive);

            if (!string.IsNullOrEmpty(model.UserName))
            {
                totalQuery = totalQuery.Where(r => r.UserName.Contains(model.UserName));
                listQuery = listQuery.Where(r => r.UserName.Contains(model.UserName));
            }
            if (!string.IsNullOrEmpty(model.Eid))
            {
                totalQuery = totalQuery.Where(r => r.Eid.Contains(model.Eid));
                listQuery = listQuery.Where(r => r.Eid.Contains(model.Eid));
            }

            if (model.RoleTid > 0)
            {
                totalQuery = totalQuery.Where(r => r.RoleTid.Equals(model.RoleTid));
                listQuery = listQuery.Where(r => r.RoleTid.Equals(model.RoleTid));
            }

            var total = totalQuery.CountAsync();

            if (!GlobalSetting.GoldList.Contains(user.Eid))
            {
                //超级管理员可以查看所有

                //只能查看自己创建角色的所有用户
                listQuery = listQuery.Where(r => r.CreateRoleName.Contains("," + user.RoleTid + ",") || r.RoleTid.Equals(user.RoleTid));
            }
            var userList = await
                 (from u in listQuery
                  from role in this.Entitys.SystemRole.Where(r => r.Tid.Equals(u.RoleTid)).DefaultIfEmpty()
                  select new UserSM
                  {
                      Tid = u.Tid,
                      IsActive = u.IsActive,
                      Eid = u.Eid,
                      UserName = u.UserName,
                      LoginIp = u.LoginIp,
                      RoleTid = u.RoleTid,
                      LastLoginTime = u.LastLoginTime,
                      UserAgent = u.UserAgent,
                      DataChangeLastTime = u.DataChangeLastTime,
                      RoleName = role.RoleName,
                      RoleDesc = role.Description,
                      Phone = u.Phone,
                      CreateUser = u.CreateUser
                  })
                .DynamicOrderBy(string.IsNullOrEmpty(model.OrderBy) ? "DataChangeLastTime" : model.OrderBy,
                            model.OrderSequence)
                            .Skip((model.PageIndex - 1) * model.PageSize)
                            .Take(model.PageSize)
                            .ToListAsync();
            return new Tuple<long, List<UserSM>>(await total, userList);
        }

        /// <summary>
        /// 给用户赋予角色
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> UserAddRole(UserAddRoleVm info)
        {
            if (info == null || info.RoleTid < 1 || !string.IsNullOrEmpty(info.UserTid))
            {
                return new Tuple<bool, string>(false, Tip.BadRequest);
            }
            var role = await this.Entitys.SystemRole.FirstOrDefaultAsync(r => r.Tid.Equals(info.RoleTid));
            if (role == null)
            {
                return new Tuple<bool, string>(false, Tip.RoleNotExist);
            }

            var user = await this.Entity.FirstOrDefaultAsync(r => r.IsActive && r.Tid.Equals(info.UserTid));
            if (user == null)
            {
                return new Tuple<bool, string>(false, Tip.BadRequest);
            }

            if (GlobalSetting.GoldList.Contains(user.Eid))
            {
                return new Tuple<bool, string>(false, Tip.GodUserInvaild);
            }
            var createRoleList = new List<long>();
            GetRoleName(role, createRoleList);
            createRoleList.Reverse();
            createRoleList = createRoleList.Distinct().ToList();
            var updateResult = await this.Entity.Where(r => r.IsActive && r.Tid.Equals(info.UserTid))
                 .Set(r => r.DataChangeLastTime, DateTime.Now)
                 .Set(r => r.RoleTid, info.RoleTid)
                 .Set(r => r.MenuRights, role.MenuRights)
                 .Set(r => r.CreateRoleName, "," + string.Join(",", createRoleList) + ",")
                 .UpdateAsync() > 0;

            if (!updateResult)
            {
                return new Tuple<bool, string>(false, Tip.UpdateError);
            }
            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// 禁用用户
        /// </summary>
        /// <param name="userTid"></param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> UserDelete(long userTid)
        {
            if (userTid < 1)
            {
                return new Tuple<bool, string>(false, Tip.BadRequest);
            }

            var user = await this.Entity.FirstOrDefaultAsync(r => r.IsActive && r.Tid.Equals(userTid));
            if (user == null)
            {
                return new Tuple<bool, string>(false, Tip.BadRequest);
            }

            if (GlobalSetting.GoldList.Contains(user.Eid))
            {
                return new Tuple<bool, string>(false, Tip.GodUserInvaild);
            }

            var updateResult = await this.Entity.Where(r => r.Tid.Equals(userTid))
                .Set(r => r.DataChangeLastTime, DateTime.Now)
                .Set(r => r.IsActive, false)
                .UpdateAsync() > 0;
            if (!updateResult)
            {
                return new Tuple<bool, string>(false, Tip.UpdateError);
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// 更改用户的属性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> ChangeField(ChangeFieldVm model)
        {
            if (model == null || string.IsNullOrEmpty(model.Field) || string.IsNullOrEmpty(model.Value) || !string.IsNullOrEmpty(model.Tid))
            {
                return new Tuple<bool, string>(false, Tip.BadRequest);
            }
            IUpdatable<SystemUsers> updateQuery = null;

            updateQuery = this.Entity.Where(r => r.Tid.Equals(model.Tid))
                .Set2(model.Field, model.Value)
                .Set(r => r.DataChangeLastTime, DateTime.Now);


            var updateResult = await updateQuery.UpdateAsync() > 0;

            if (!updateResult)
            {
                return new Tuple<bool, string>(false, Tip.UpdateError);
            }
            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// 手动添加用户
        /// </summary>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> UserAdd(SystemUsers info, Token user)
        {
            if (info == null || string.IsNullOrEmpty(info.UserName) || info.RoleTid < 1)
            {
                return new Tuple<bool, string>(false, Tip.BadRequest);
            }

            info.Eid = info.Eid.ToLower();
            var existItem = await Entity.FirstOrDefaultAsync(r => r.Eid.Equals(info.Eid));
            if (existItem != null)
            {
                return new Tuple<bool, string>(false, "该员工已存在");
            }

            var role = await this.Entitys.SystemRole.FirstOrDefaultAsync(r => r.Tid.Equals(info.RoleTid));
            if (role == null)
            {
                return new Tuple<bool, string>(false, Tip.RoleNotExist);
            }

            //获取创建者角色的包括父级的名称列表
            var createRoleList = new List<long>();
            GetRoleName(role, createRoleList);
            createRoleList.Reverse();
            createRoleList = createRoleList.Distinct().ToList();
            info.IsActive = true;
            info.Pwd = !string.IsNullOrEmpty(info.Pwd) ? info.Pwd : info.Eid;
            info.Pwd = CodingUtils.MD5(info.Pwd);
            info.MenuRights = role.MenuRights;
            info.DataChangeLastTime = DateTime.Now;
            info.CreateRoleName = "," + string.Join(",", createRoleList) + ",";
            info.CreateUser = user.Eid;

            var inertResult = DB.Insert(info) > 0;
            if (!inertResult)
            {
                return new Tuple<bool, string>(false, Tip.SystemError);
            }

            return new Tuple<bool, string>(true, string.Empty);
        }


        private void GetRoleName(SystemRole role, List<long> roleList)
        {
            roleList.Add(role.Tid);
            if (!string.IsNullOrEmpty(role.CreateUser))
            {
                var user = this.Entity.FirstOrDefault(r => r.Eid.Equals(role.CreateUser));
                if (user != null && user.RoleTid > 0)
                {
                    var role1 = this.Entitys.SystemRole.FirstOrDefault(r => r.Tid.Equals(user.RoleTid));
                    if (role1 != null)
                    {
                        GetRoleName(role1, roleList);
                    }
                }
            }
        }

        #region WriteLoginCookie

        private string WriteLoginCookie(Token token)
        {
            string strName = GlobalSetting.CurrentLoginUserGuid;
            var value = token.ToJsonString();
            string strValue = CodingUtils.AesEncrypt(value);
            WebUtils.WriteCookie(strName, strValue);
            return value;
        }



        #endregion
    }
}
