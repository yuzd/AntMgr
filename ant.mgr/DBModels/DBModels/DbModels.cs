using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AntData.ORM;
using AntData.ORM.Data;
using AntData.ORM.Linq;
using AntData.ORM.Mapping;

namespace DbModel
{
	/// <summary>
	/// Visx Version   : 3.2
	/// Github         : https://github.com/yuzd/AntData.ORM
	/// Database       : antmgr
	/// Data Source    : localhost
	/// Server Version : 5.7.25
	/// </summary>
	public partial class AntEntity : IEntity
	{
		/// <summary>
		/// 系统菜单表
		/// </summary>
		public IQueryable<SystemMenu>       SystemMenu       { get { return this.Get<SystemMenu>(); } }
		/// <summary>
		/// 菜单按钮
		/// </summary>
		public IQueryable<SystemPageAction> SystemPageAction { get { return this.Get<SystemPageAction>(); } }
		/// <summary>
		/// 角色表
		/// </summary>
		public IQueryable<SystemRole>       SystemRole       { get { return this.Get<SystemRole>(); } }
		/// <summary>
		/// 后台系统用户表
		/// </summary>
		public IQueryable<SystemUsers>      SystemUsers      { get { return this.Get<SystemUsers>(); } }

		private readonly DataConnection con;

		public DataConnection DbContext
		{
			get { return this.con; }
		}

		public IQueryable<T> Get<T>()
			 where T : class
		{
			return this.con.GetTable<T>();
		}

		public AntEntity(DataConnection con)
		{
			this.con = con;
		}
	}

	/// <summary>
	/// 系统菜单表
	/// </summary>
	[Table(Db="antmgr", Comment="系统菜单表", Name="system_menu")]
	public partial class SystemMenu : LinqToDBEntity
	{
		#region Column

		/// <summary>
		/// MenuId
		/// </summary>
		[Column("Tid",                DataType=AntData.ORM.DataType.Int64,    Comment="MenuId"), PrimaryKey, Identity]
		public virtual long Tid { get; set; } // bigint(20)

		/// <summary>
		/// 最后更新时间
		/// </summary>
		[Column("DataChangeLastTime", DataType=AntData.ORM.DataType.DateTime, Comment="最后更新时间"), NotNull]
		public virtual DateTime DataChangeLastTime // datetime
		{
			get { return _DataChangeLastTime; }
			set { _DataChangeLastTime = value; }
		}

		/// <summary>
		/// 是否可用
		/// </summary>
		[Column("IsActive",           DataType=AntData.ORM.DataType.Boolean,  Comment="是否可用"), NotNull]
		public virtual bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 父节点Id
		/// </summary>
		[Column("ParentTid",          DataType=AntData.ORM.DataType.Int64,    Comment="父节点Id"), NotNull]
		public virtual long ParentTid { get; set; } // bigint(20)

		/// <summary>
		/// 名称
		/// </summary>
		[Column("Name",               DataType=AntData.ORM.DataType.VarChar,  Length=50, Comment="名称"),    Nullable]
		public virtual string Name { get; set; } // varchar(50)

		/// <summary>
		/// 展示的图标
		/// </summary>
		[Column("Ico",                DataType=AntData.ORM.DataType.VarChar,  Length=100, Comment="展示的图标"),    Nullable]
		public virtual string Ico { get; set; } // varchar(100)

		/// <summary>
		/// 连接地址
		/// </summary>
		[Column("Url",                DataType=AntData.ORM.DataType.VarChar,  Length=200, Comment="连接地址"),    Nullable]
		public virtual string Url { get; set; } // varchar(200)

		/// <summary>
		/// 排序
		/// </summary>
		[Column("OrderRule",          DataType=AntData.ORM.DataType.Int32,    Comment="排序"),    Nullable]
		public virtual int? OrderRule { get; set; } // int(11)

		/// <summary>
		/// 等级
		/// </summary>
		[Column("Level",              DataType=AntData.ORM.DataType.Int32,    Comment="等级"),    Nullable]
		public virtual int? Level { get; set; } // int(11)

		/// <summary>
		/// 样式
		/// </summary>
		[Column("Class",              DataType=AntData.ORM.DataType.VarChar,  Length=100, Comment="样式"),    Nullable]
		public virtual string Class { get; set; } // varchar(100)

		#endregion

		#region Field

		private DateTime _DataChangeLastTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

		#endregion
	}

	/// <summary>
	/// 菜单按钮
	/// </summary>
	[Table(Db="antmgr", Comment="菜单按钮", Name="system_page_action")]
	public partial class SystemPageAction : LinqToDBEntity
	{
		#region Column

		/// <summary>
		/// 主键
		/// </summary>
		[Column("Tid",                DataType=AntData.ORM.DataType.Int64,    Comment="主键"), PrimaryKey, Identity]
		public virtual long Tid { get; set; } // bigint(20)

		/// <summary>
		/// 最后更新时间
		/// </summary>
		[Column("DataChangeLastTime", DataType=AntData.ORM.DataType.DateTime, Comment="最后更新时间"), NotNull]
		public virtual DateTime DataChangeLastTime // datetime
		{
			get { return _DataChangeLastTime; }
			set { _DataChangeLastTime = value; }
		}

		/// <summary>
		/// 访问路径
		/// </summary>
		[Column("MenuTid",            DataType=AntData.ORM.DataType.Int64,    Comment="访问路径"), NotNull]
		public virtual long MenuTid { get; set; } // bigint(20)

		/// <summary>
		/// ActionId
		/// </summary>
		[Column("ActionId",           DataType=AntData.ORM.DataType.VarChar,  Length=100, Comment="ActionId"),    Nullable]
		public virtual string ActionId { get; set; } // varchar(100)

		/// <summary>
		/// ActionName
		/// </summary>
		[Column("ActionName",         DataType=AntData.ORM.DataType.VarChar,  Length=255, Comment="ActionName"),    Nullable]
		public virtual string ActionName { get; set; } // varchar(255)

		/// <summary>
		/// ControlName
		/// </summary>
		[Column("ControlName",        DataType=AntData.ORM.DataType.VarChar,  Length=255, Comment="ControlName"),    Nullable]
		public virtual string ControlName { get; set; } // varchar(255)

		#endregion

		#region Field

		private DateTime _DataChangeLastTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

		#endregion
	}

	/// <summary>
	/// 角色表
	/// </summary>
	[Table(Db="antmgr", Comment="角色表", Name="system_role")]
	public partial class SystemRole : LinqToDBEntity
	{
		#region Column

		/// <summary>
		/// 最后更新时间
		/// </summary>
		[Column("DataChangeLastTime", DataType=AntData.ORM.DataType.DateTime, Comment="最后更新时间"), NotNull]
		public virtual DateTime DataChangeLastTime // datetime
		{
			get { return _DataChangeLastTime; }
			set { _DataChangeLastTime = value; }
		}

		/// <summary>
		/// 角色名称
		/// </summary>
		[Column("RoleName",           DataType=AntData.ORM.DataType.VarChar,  Length=100, Comment="角色名称"),    Nullable]
		public virtual string RoleName { get; set; } // varchar(100)

		/// <summary>
		/// 描述
		/// </summary>
		[Column("Description",        DataType=AntData.ORM.DataType.VarChar,  Length=200, Comment="描述"),    Nullable]
		public virtual string Description { get; set; } // varchar(200)

		/// <summary>
		/// 是否可用
		/// </summary>
		[Column("IsActive",           DataType=AntData.ORM.DataType.Boolean,  Comment="是否可用"), NotNull]
		public virtual bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 菜单权限
		/// </summary>
		[Column("MenuRights",         DataType=AntData.ORM.DataType.VarChar,  Length=150, Comment="菜单权限"),    Nullable]
		public virtual string MenuRights { get; set; } // varchar(150)

		/// <summary>
		/// 主键
		/// </summary>
		[Column("Tid",                DataType=AntData.ORM.DataType.Int64,    Comment="主键"), PrimaryKey, Identity]
		public virtual long Tid { get; set; } // bigint(20)

		/// <summary>
		/// 按钮等权限
		/// </summary>
		[Column("ActionList",         DataType=AntData.ORM.DataType.Text,     Length=4294967295, Comment="按钮等权限"),    Nullable]
		public virtual string ActionList { get; set; } // longtext

		/// <summary>
		/// 创建者
		/// </summary>
		[Column("CreateUser",         DataType=AntData.ORM.DataType.VarChar,  Length=20, Comment="创建者"),    Nullable]
		public virtual string CreateUser { get; set; } // varchar(20)

		/// <summary>
		/// 创建者的角色Tid
		/// </summary>
		[Column("CreateRoleTid",      DataType=AntData.ORM.DataType.Int64,    Comment="创建者的角色Tid"), NotNull]
		public virtual long CreateRoleTid { get; set; } // bigint(20)

		#endregion

		#region Field

		private DateTime _DataChangeLastTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

		#endregion
	}

	/// <summary>
	/// 后台系统用户表
	/// </summary>
	[Table(Db="antmgr", Comment="后台系统用户表", Name="system_users")]
	public partial class SystemUsers : LinqToDBEntity
	{
		#region Column

		/// <summary>
		/// 主键
		/// </summary>
		[Column("Tid",                DataType=AntData.ORM.DataType.Int64,    Comment="主键"), PrimaryKey, Identity]
		public virtual long Tid { get; set; } // bigint(20)

		/// <summary>
		/// 最后更新时间
		/// </summary>
		[Column("DataChangeLastTime", DataType=AntData.ORM.DataType.DateTime, Comment="最后更新时间"), NotNull]
		public virtual DateTime DataChangeLastTime // datetime
		{
			get { return _DataChangeLastTime; }
			set { _DataChangeLastTime = value; }
		}

		/// <summary>
		/// 是否可用
		/// </summary>
		[Column("IsActive",           DataType=AntData.ORM.DataType.Boolean,  Comment="是否可用"), NotNull]
		public virtual bool IsActive { get; set; } // tinyint(1)

		/// <summary>
		/// 登陆名
		/// </summary>
		[Column("Eid",                DataType=AntData.ORM.DataType.VarChar,  Length=36, Comment="登陆名"),    Nullable]
		public virtual string Eid { get; set; } // varchar(36)

		/// <summary>
		/// 用户名
		/// </summary>
		[Column("UserName",           DataType=AntData.ORM.DataType.VarChar,  Length=50, Comment="用户名"),    Nullable]
		public virtual string UserName { get; set; } // varchar(50)

		/// <summary>
		/// 密码
		/// </summary>
		[Column("Pwd",                DataType=AntData.ORM.DataType.VarChar,  Length=50, Comment="密码"),    Nullable]
		public virtual string Pwd { get; set; } // varchar(50)

		/// <summary>
		/// 手机号
		/// </summary>
		[Column("Phone",              DataType=AntData.ORM.DataType.VarChar,  Length=20, Comment="手机号"),    Nullable]
		public virtual string Phone { get; set; } // varchar(20)

		/// <summary>
		/// 登陆IP
		/// </summary>
		[Column("LoginIp",            DataType=AntData.ORM.DataType.VarChar,  Length=30, Comment="登陆IP"),    Nullable]
		public virtual string LoginIp { get; set; } // varchar(30)

		/// <summary>
		/// 菜单权限
		/// </summary>
		[Column("MenuRights",         DataType=AntData.ORM.DataType.VarChar,  Length=150, Comment="菜单权限"),    Nullable]
		public virtual string MenuRights { get; set; } // varchar(150)

		/// <summary>
		/// 角色Tid(一个人只有一个角色)
		/// </summary>
		[Column("RoleTid",            DataType=AntData.ORM.DataType.Int64,    Comment="角色Tid(一个人只有一个角色)"), NotNull]
		public virtual long RoleTid { get; set; } // bigint(20)

		/// <summary>
		/// 最后登录系统时间
		/// </summary>
		[Column("LastLoginTime",      DataType=AntData.ORM.DataType.DateTime, Comment="最后登录系统时间"),    Nullable]
		public virtual DateTime? LastLoginTime { get; set; } // datetime

		/// <summary>
		/// 登录的浏览器信息
		/// </summary>
		[Column("UserAgent",          DataType=AntData.ORM.DataType.VarChar,  Length=500, Comment="登录的浏览器信息"),    Nullable]
		public virtual string UserAgent { get; set; } // varchar(500)

		/// <summary>
		/// 创建的角色名称
		/// </summary>
		[Column("CreateRoleName",     DataType=AntData.ORM.DataType.VarChar,  Length=500, Comment="创建的角色名称"),    Nullable]
		public virtual string CreateRoleName { get; set; } // varchar(500)

		/// <summary>
		/// 创建者
		/// </summary>
		[Column("CreateUser",         DataType=AntData.ORM.DataType.VarChar,  Length=50, Comment="创建者"),    Nullable]
		public virtual string CreateUser { get; set; } // varchar(50)

		#endregion

		#region Field

		private DateTime _DataChangeLastTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

		#endregion
	}

	public static partial class TableExtensions
	{
		public static SystemMenu FindByBk(this IQueryable<SystemMenu> table, long Tid)
		{
			return table.FirstOrDefault(t =>
				t.Tid == Tid);
		}

		public static async Task<SystemMenu> FindByBkAsync(this IQueryable<SystemMenu> table, long Tid)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Tid == Tid);
		}

		public static SystemPageAction FindByBk(this IQueryable<SystemPageAction> table, long Tid)
		{
			return table.FirstOrDefault(t =>
				t.Tid == Tid);
		}

		public static async Task<SystemPageAction> FindByBkAsync(this IQueryable<SystemPageAction> table, long Tid)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Tid == Tid);
		}

		public static SystemRole FindByBk(this IQueryable<SystemRole> table, long Tid)
		{
			return table.FirstOrDefault(t =>
				t.Tid == Tid);
		}

		public static async Task<SystemRole> FindByBkAsync(this IQueryable<SystemRole> table, long Tid)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Tid == Tid);
		}

		public static SystemUsers FindByBk(this IQueryable<SystemUsers> table, long Tid)
		{
			return table.FirstOrDefault(t =>
				t.Tid == Tid);
		}

		public static async Task<SystemUsers> FindByBkAsync(this IQueryable<SystemUsers> table, long Tid)
		{
			return await table.FirstOrDefaultAsync(t =>
				t.Tid == Tid);
		}
	}
}
