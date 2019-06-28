CREATE TABLE `system_menu` (
  `Tid` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'MenuId',
  `DataChangeLastTime` DATETIME NOT NULL COMMENT '最后更新时间',
  `IsActive` TINYINT(1) NOT NULL DEFAULT '1' COMMENT '是否可用',
  `ParentTid` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '父节点Id',
  `Name` VARCHAR(50) CHARACTER SET utf8 DEFAULT NULL COMMENT '名称',
  `Ico` VARCHAR(100) CHARACTER SET utf8 DEFAULT NULL COMMENT '展示的图标',
  `Url` VARCHAR(200) CHARACTER SET utf8 DEFAULT NULL COMMENT '连接地址',
  `OrderRule` INT(11) DEFAULT NULL COMMENT '排序',
  `Level` INT(11) DEFAULT NULL COMMENT '等级',
  `Class` VARCHAR(100) CHARACTER SET utf8 DEFAULT NULL COMMENT '样式',
  PRIMARY KEY (`Tid`),
  KEY `idx_DataChange_LastTime` (`DataChangeLastTime`)
) ENGINE=INNODB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8 COMMENT='系统菜单表'


CREATE TABLE `system_page_action` (
  `Tid` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '主键',
  `DataChangeLastTime` DATETIME NOT NULL COMMENT '最后更新时间',
  `MenuTid` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '访问路径',
  `ActionId` VARCHAR(100) DEFAULT NULL COMMENT 'ActionId',
  `ActionName` VARCHAR(255) DEFAULT NULL COMMENT 'ActionName',
  `ControlName` VARCHAR(255) DEFAULT NULL COMMENT 'ControlName',
  PRIMARY KEY (`Tid`)
) ENGINE=INNODB DEFAULT CHARSET=utf8 COMMENT='菜单按钮'



CREATE TABLE `system_role` (
  `DataChangeLastTime` DATETIME NOT NULL COMMENT '最后更新时间',
  `RoleName` VARCHAR(100) CHARACTER SET utf8 DEFAULT NULL COMMENT '角色名称',
  `Description` VARCHAR(200) CHARACTER SET utf8 DEFAULT NULL COMMENT '描述',
  `IsActive` TINYINT(1) NOT NULL DEFAULT '1' COMMENT '是否可用',
  `MenuRights` VARCHAR(150) CHARACTER SET utf8 DEFAULT NULL COMMENT '菜单权限',
  `Tid` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '主键',
  `ActionList` LONGTEXT COMMENT '按钮等权限',
  `CreateUser` VARCHAR(20) DEFAULT NULL COMMENT '创建者',
  `CreateRoleTid` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '创建者的角色Tid',
  PRIMARY KEY (`Tid`),
  UNIQUE KEY `idx_RoleName` (`RoleName`),
  KEY `idx_DataChange_LastTime` (`DataChangeLastTime`)
) ENGINE=INNODB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COMMENT='角色表'



CREATE TABLE `system_users` (
  `Tid` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '主键',
  `DataChangeLastTime` DATETIME NOT NULL COMMENT '最后更新时间',
  `IsActive` TINYINT(1) NOT NULL DEFAULT '1' COMMENT '是否可用',
  `Eid` VARCHAR(36) CHARACTER SET utf8 DEFAULT NULL COMMENT '登陆名',
  `UserName` VARCHAR(50) CHARACTER SET utf8 DEFAULT NULL COMMENT '用户名',
  `Pwd` VARCHAR(50) CHARACTER SET utf8 DEFAULT NULL COMMENT '密码',
  `Phone` VARCHAR(20) CHARACTER SET utf8 DEFAULT NULL COMMENT '手机号',
  `LoginIp` VARCHAR(30) CHARACTER SET utf8 DEFAULT NULL COMMENT '登陆IP',
  `MenuRights` VARCHAR(150) CHARACTER SET utf8 DEFAULT NULL COMMENT '菜单权限',
  `RoleTid` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '角色Tid(一个人只有一个角色)',
  `LastLoginTime` DATETIME DEFAULT NULL COMMENT '最后登录系统时间',
  `UserAgent` VARCHAR(500) CHARACTER SET utf8 DEFAULT NULL COMMENT '登录的浏览器信息',
  `CreateRoleName` VARCHAR(500) DEFAULT NULL COMMENT '创建的角色名称',
  `CreateUser` VARCHAR(50) DEFAULT NULL COMMENT '创建者',
  PRIMARY KEY (`Tid`),
  UNIQUE KEY `idx_Eid` (`Eid`),
  KEY `idx_roleTid` (`RoleTid`),
  KEY `idx_DataChange_LastTime` (`DataChangeLastTime`)
) ENGINE=INNODB DEFAULT CHARSET=utf8 COMMENT='后台系统用户表'



insert into `system_menu` (`Tid`,`DataChangeLastTime`, `IsActive`, `ParentTid`, `Name`, `Ico`, `Url`, `OrderRule`, `Level`, `Class`)
 values
 (1,now(),'1','0','系统管理',NULL,'#','3','1','glyphicon glyphicon-cog');
 
insert into `system_menu` (`DataChangeLastTime`, `IsActive`, `ParentTid`, `Name`, `Ico`, `Url`, `OrderRule`, `Level`, `Class`)
 values
 (now(),'1','1','菜单管理',NULL,'~/Home/MenuList','1','2',NULL);
 
 insert into `system_menu` (`DataChangeLastTime`, `IsActive`, `ParentTid`, `Name`, `Ico`, `Url`, `OrderRule`, `Level`, `Class`)
 values(now(),'1','1','权限配置',NULL,'~/Role/RoleAction','2','2',NULL);
 
insert into `system_menu` (`DataChangeLastTime`, `IsActive`, `ParentTid`, `Name`, `Ico`, `Url`, `OrderRule`, `Level`, `Class`)
 values(now(),'1','1','角色管理',NULL,'~/Role/RoleList','3','2',NULL);
 
insert into `system_menu` (`DataChangeLastTime`, `IsActive`, `ParentTid`, `Name`, `Ico`, `Url`, `OrderRule`, `Level`, `Class`) 
 values(now(),'1','1','系统用户',NULL,'~/Account/UserList','4','2',NULL);
 
insert into `system_menu` (`DataChangeLastTime`, `IsActive`, `ParentTid`, `Name`, `Ico`, `Url`, `OrderRule`, `Level`, `Class`)
 values(now(),'1','1','代码生成',NULL,'~/Common/CodeGen','5','2',NULL);

INSERT INTO `system_users` 
(`DataChangeLastTime`, `IsActive`, `Eid`, `UserName`, `Pwd`, `Phone`, `LoginIp`, `MenuRights`, `RoleTid`, `LastLoginTime`, `UserAgent`, `CreateRoleName`, `CreateUser`) 
VALUES(NOW(),'1','zdyu','admin','e10adc3949ba59abbe56e057f20f883e','13000000000','114.80.11.236',NULL,'0','2019-05-15 12:22:52','Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36',NULL,'');


