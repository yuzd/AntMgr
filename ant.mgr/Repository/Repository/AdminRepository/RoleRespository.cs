using AntData.ORM;
using AntData.ORM.Data;
using Autofac.Annotation;
using Castle.DynamicProxy;
using Configuration;
using Infrastructure.StaticExt;
using Newtonsoft.Json;
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
    /// 角色权限管理
    /// </summary>
    [Bean(typeof(IRoleRespository), Interceptor = typeof(AsyncInterceptor))]
    public class RoleRespository : BaseRepository<SystemRole>, IRoleRespository
    {
        /// <summary>
        /// 页面与接口关联
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> AddRoleActions(RoleAction model)
        {
            if (model == null || model.MenuId < 1 || string.IsNullOrEmpty(model.ActionId)) return Tip.BadRequest;
            await this.Entitys.SystemPageAction.Where(r => r.MenuTid.Equals(model.MenuId) && r.ActionId.Equals(model.ActionId)).DeleteAsync();
            var list = model.ActionList.Select(r => new SystemPageAction
            {
                DataChangeLastTime = DateTime.Now,
                ActionId = model.ActionId,
                MenuTid = model.MenuId,
                ActionName = r.Split(',')[1],
                ControlName = r.Split(',')[0]
            }).ToList();

            if (list.Count > 0)
            {
                this.DB.BulkCopy(list);
            }

            return string.Empty;
        }

        /// <summary>
        /// 权限与接口关联配置 获取某一个菜单的某一个功能配置的接口列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<List<SystemPageAction>> GetRoleActions(RoleAction model)
        {
            if (model == null || model.MenuId < 1 || string.IsNullOrEmpty(model.ActionId)) return new List<SystemPageAction>();

            var list = await Entitys.SystemPageAction.Where(r => r.MenuTid.Equals(model.MenuId) && r.ActionId.Equals(model.ActionId)).ToListAsync();
            return list;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <returns></returns>
        public async Task<string> DeleteRole(long tid, Token user)
        {
            if (tid < 1) return Tip.BadRequest;


            var role = await this.Entity.FirstOrDefaultAsync(r => r.Tid.Equals(tid));
            if (role == null)
            {
                return Tip.RoleNotExist;
            }

            if (!GlobalSetting.GoldList.Contains(user.Eid) && !role.CreateUser.Equals(user.Eid))
            {
                return "非自己创建的角色,没有权限删除";
            }
            //查看是否有该角色的用户
            var existRoleUser = Entitys.SystemUsers.Any(r => r.IsActive && r.RoleTid.Equals(tid));
            if (existRoleUser)
            {
                return "当前角色下有用户,不能删除!";
            }

            var rt = this.Entity.Where(r => r.Tid.Equals(tid)).Delete() > 0;
            if (!rt)
            {
                return Tip.UpdateError;
            }

            return string.Empty;

        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        public async Task<Tuple<long, List<SystemRole>>> GetList(RoleVm model, Token userToken)
        {
            if (model == null)
            {
                return new Tuple<long, List<SystemRole>>(0, new List<SystemRole>());
            }

            var totalQuery = this.Entity.Where(r => r.IsActive);
            var listQuery = this.Entity.Where(r => r.IsActive);
            if (!string.IsNullOrEmpty(model.RoleName))
            {
                totalQuery = totalQuery.Where(r => r.RoleName.Contains(model.RoleName));
                listQuery = listQuery.Where(r => r.RoleName.Contains(model.RoleName));
            }
            if (!string.IsNullOrEmpty(model.CreateUser))
            {
                totalQuery = totalQuery.Where(r => r.CreateUser.Contains(model.CreateUser));
                listQuery = listQuery.Where(r => r.CreateUser.Contains(model.CreateUser));
            }
            if (!GlobalSetting.GoldList.Contains(userToken.Eid))
            {
                totalQuery = totalQuery.Where(r => r.CreateUser.Equals(userToken.Eid) || r.CreateRoleTid.Equals(userToken.RoleTid));
                listQuery = listQuery.Where(r => r.CreateUser.Equals(userToken.Eid)|| r.CreateRoleTid.Equals(userToken.RoleTid));
            }

            var total = totalQuery.CountAsync();

            var roleList = await listQuery.DynamicOrderBy(string.IsNullOrEmpty(model.OrderBy) ? "DataChangeLastTime" : model.OrderBy,
                            model.OrderSequence)
                            .Skip((model.PageIndex - 1) * model.PageSize)
                            .Take(model.PageSize)
                            .ToListAsync();
            return new Tuple<long, List<SystemRole>>(await total, roleList);
        }

        /// <summary>
        /// 新增Role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<string> AddRole(AddRoleVm role, Token user)
        {
            if (role == null)
            {
                return Tip.BadRequest;
            }

            bool isUpdate = role.Tid > 0;

            if (!isUpdate)
            {
                var en = await this.Entity.FirstOrDefaultAsync(r => r.IsActive && r.RoleName.Equals(role.RoleName.Trim()));
                if (en != null)
                {
                    return Tip.RoleNameIsExists;
                }
            }
            else
            {
                var en = await this.Entity.FirstOrDefaultAsync(r => r.IsActive && !r.Tid.Equals(role.Tid) && r.RoleName.Equals(role.RoleName.Trim()));
                if (en != null)
                {
                    return Tip.RoleNameIsExists;
                }
            }


            var menuRights = RightsHelper.SumRights(role.Ids.ToList());
            var actionList = role.Actions.GroupBy(r => r.MenuId)
                                        .Select(r => new MenuActionSM
                                        {
                                            MenuTid = r.Key,
                                            ActionList = r.Select(y => new ActionSM
                                            {
                                                ActionId = y.ActionId,
                                                ActionName = y.ActionName
                                            }).ToList()
                                        }).ToList();
            var systemRole = new SystemRole
            {
                ActionList = JsonConvert.SerializeObject(actionList),
                IsActive = true,
                MenuRights = menuRights.ToString(),
                RoleName = role.RoleName,
                Description = role.RoleDesc,
                CreateUser = user.Eid,
                CreateRoleTid = user.RoleTid
            };

            if (!isUpdate)
            {
                var saveResult = this.Save(systemRole) > 0;
                if (!saveResult)
                {
                    return Tip.InserError;
                }
            }
            else
            {
                var update = false;
                this.DB.UseTransaction(con =>
                {
                    //更新角色
                    var updateResult = con.Tables.SystemRole.Where(r => r.Tid.Equals(role.Tid))
                                                     .Set(r => r.DataChangeLastTime, DateTime.Now)
                                                     .Set(r => r.RoleName, systemRole.RoleName)
                                                     .Set(r => r.Description, systemRole.Description)
                                                     .Set(r => r.MenuRights, systemRole.MenuRights)
                                                     .Set(r => r.ActionList, systemRole.ActionList)
                                                     .Update() > 0;

                    if (!updateResult)
                    {
                        return false;
                    }

                    //更新所有角色下的用户菜单权限
                    con.Tables.SystemUsers.Where(r => r.RoleTid.Equals(role.Tid))
                       .Set(r => r.MenuRights, systemRole.MenuRights)
                       .Set(r => r.DataChangeLastTime, DateTime.Now)
                       .Update();

                    update = true;
                    return true;
                });

                if (!update)
                {
                    return Tip.UpdateError;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取所有的角色
        /// </summary>
        /// <returns></returns>
        public async Task<List<SystemRole>> GetAllRoleList(Token user)
        {
            var isGod = GlobalSetting.GoldList.Contains(user.Eid);
            if (!isGod)
            {
                return await this.Entity.Where(r => r.IsActive && (r.CreateUser.Equals(user.Eid) || r.Tid.Equals(user.RoleTid))).ToListAsync();
            }
            return await this.Entity.Where(r => r.IsActive).ToListAsync();
        }

        /// <summary>
        /// 获取当前页面的action集合
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="RoleTid"></param>
        /// <param name="currentMenuTid"></param>
        /// <returns></returns>
        public async Task<SystemAction> GetSystemUserActions(string eid, long RoleTid, long currentMenuTid)
        {
            var result = new SystemAction
            {
                IsGod = false,
                ActionList = new List<string>()
            };

            if (GlobalSetting.GoldList.Contains(eid))
            {
                result.IsGod = true;
                return result;
            }

            if (string.IsNullOrEmpty(eid) || RoleTid < 1 || currentMenuTid < 1)
            {
                return result;
            }


            var role = await this.Entity.FirstOrDefaultAsync(r => r.Tid.Equals(RoleTid));
            if (role == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(role.ActionList))
            {
                role.ActionList = "[]";
            }
            var actionList = JsonConvert.DeserializeObject<List<MenuActionSM>>(role.ActionList)
                                                .ToDictionary(r => r.MenuTid, g => g.ActionList);

            if (!actionList.ContainsKey(currentMenuTid))
            {
                return result;
            }
            result.IsGod = false;
            result.ActionList = actionList[currentMenuTid].Select(r => r.ActionId).ToList();

            return result;
        }

    }
}
