

# AntMgr
基于netcore2.0  mvc 开发的 快速搭建具有如下特色的后台管理系统
最新版已升级到net6.0

# 特色：
1. 用户管理
2. 菜单管理
3. 角色管理
4. 权限管理[精确到按钮]）
5. 代码生成器(一键生成CRUD前后端代码 增+删+改+查【分页】导入+导出)
6. SQL工具(智能字段提示)
7. 约定大于配置，开发简单效率高,权限配置友好不复杂
8. 支持mysql和sqlserver两种数据库。切换数据库不用改任何代码

角色支持遗传，比如 ：共有ABCD四个功能权限，分配了 A B 2个权限给 超级管理员角色的用户，支持让超级管理员用户还能自己创建角色。但是只能选择 A B 2个。无法看到 C D！

# 遇到问题可有加群咨询
![image](https://images4.c-ctrip.com/target/zb0k16000000yztm18B20.png)

# 如何使用 wiki:https://github.com/yuzd/AntMgr/wiki
代码克隆到本地 用vs2017或以上版本 打开工程。

### 项目结构如下：

![image](https://images4.c-ctrip.com/target/zb0315000000yq3me600A.png)
![image](https://images4.c-ctrip.com/target/zb0u15000000yoc9r273B.png)

### 找到DbModel下面的初始化db脚本
- mysql数据库的初始化脚本
- sqlserver数据库的初始化脚本（我测试用的是2012）

![image](https://images4.c-ctrip.com/target/zb0m16000000z3fk4624C.png)

### 以下教程采用的是mysql数据库，如果您是用MSSQL的话请看wiki里面有专门介绍注意点。

里面包含4张表的schema和初始化数据，你可以新建一个db，也可以导入到你现有的db

1. system_users           系统用户表
2. system_menu            系统菜单表
3. system_role            系统角色表
4. system_page_action     按钮权限表



![image](https://images4.c-ctrip.com/target/zb0b15000000yqrkk687D.png)


### 导入db成功后在项目配置文件里面修改成正确的db链接字符串

![image](https://images4.c-ctrip.com/target/zb0o15000000yn8889DA6.png)


### F5启动应用 会进入登录页面

![image](https://images4.c-ctrip.com/target/zb0915000000yqsrzD697.png)

如果是按照上面初始化db脚本的话
初始化的登录名 zdyu 密码 123456


登录成功如下图：

### 首页
![image](https://images4.c-ctrip.com/target/zb081e000001fgemwEB45.png)


### 菜单管理
![image](https://images4.c-ctrip.com/target/zb0h1e000001fhsa4146E.png)

### 权限管理
![image](https://images4.c-ctrip.com/target/zb0h1e000001fhsarEC6C.png)

### 角色管理
![image](https://images4.c-ctrip.com/target/zb0c1e000001fiuw76AD4.png)
![image](https://images4.c-ctrip.com/target/zb0f1e000001fg8ec6A05.png)

### 系统用户
![image](https://images4.c-ctrip.com/target/zb091e000001fim7m7EF4.png)

### 代码自动生成
![image](https://images4.c-ctrip.com/target/zb081e000001fgeq1EC8F.png)
![image](https://images4.c-ctrip.com/target/zb0d1e000001feqhn5B6D.png)

### SQL工具(智能字段提示,写sql再也不用担心忘记字段名称了)
![image](https://images4.c-ctrip.com/target/zb0v1e000001fr9j98114.png)


### 权限如何设计的
-  一个菜单(system_menu)对应一个view页面【1：1】
-  一个用户(system_user)只能有一个角色(system_role)【1:1】
-  一个角色(system_role)可以配置多个菜单权限(view页面)
    和view页面里面的功能模块(ru按钮) 【1：N】
-  每个菜单(view页面)下的功能模块(如按钮)配置实际该功能用到的接口【1：N】

### 约定>配置 权限设计其实很简单
权限主要拦截器代码实现在：
![image](https://images4.c-ctrip.com/target/zb0c15000000yr2jg2D37.png)

用户登录态是记录在cookie的 默认的keyName:cbeid
可以在这个里面更改
![image](https://images4.c-ctrip.com/target/zb0t15000000yoh6fE3C8.png) 


#### 约定一
具有权限拦截的Controller类上面统一打上[API("controller作用描述")] 方法上面打上  [API("方法作用描述")]

#### 约定二

在页面里面写需要权限控制的按钮的时候

```
<button type="button" class="authorization" action-id="role-add" action-name="新增角色" style="display: none" >新增角色</button>

```

标签名称 | 含义
---|---
action-id  | 表示这个权限按钮的id 在当前view唯一
action-name  | 权限按钮的作用描述
display  | 默认使用 none 隐藏
class    | 必须有 authorization

只要遵守如上2个约定开发好页面和controller后，按照如下步骤进行配置权限

1. 在菜单管理加入新增的页面
2. 在权限管理配置功能按钮对应用到的接口
3. 在角色管理修改对应的角色


### 其他说明

后台用到了的Orm框架：

https://github.com/yuzd/AntData.ORM

配合ORM框架的vs一键生成dbmodel 插件 [wiki有插件使用教程](https://github.com/yuzd/AntMgr/wiki/4-Dbmodels%E6%8F%92%E4%BB%B6%E7%9B%B8%E5%85%B3)：

https://marketplace.visualstudio.com/items?itemName=nainaigu.AntDataVS

配合ORM框架的rider一键生成dbmodel 插件 [wiki有插件使用教程](https://github.com/yuzd/AntMgr/wiki/4-Dbmodels%E6%8F%92%E4%BB%B6%E7%9B%B8%E5%85%B3)：

https://plugins.jetbrains.com/plugin/14033


### WIKI有更详细的教程
https://github.com/yuzd/AntMgr/wiki
