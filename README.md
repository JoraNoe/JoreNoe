# JoreNoe

安装方法

| Build                                                     | NuGet                                                        | Downloads                                                    |
| --------------------------------------------------------- | ------------------------------------------------------------ | ------------------------------------------------------------ |
| ![](https://img.shields.io/badge/NetCore-5.0.9-green.svg) | [![](https://img.shields.io/nuget/v/JoreNoe.svg)](https://www.nuget.org/packages/JoreNoe) | ![](https://img.shields.io/badge/Downloads-2.1K+-green.svg) |

```C
Install-Package JoreNoe -Version 5.0.9 
```

JoreNoe.DB.SqlServer 使用方法 

可在 Startup 文件中 直接使用   

```c#
//使用方法 将EF 上下文 注册进入    new  Your EFContext
Register.SetInitDbContext(MyContext)
```

JoreNoe.DB.Redis 使用方法

Startup 中注册上下文

```C#
//使用此方法进行注册 
//参数 Connection 你的Reids链接地址
//实例名称
//默认数据库（Int 类型 ）
Register.SetInitRedisConfig(YourRedisConnection,InstanceName,defaultDB = 0)
```

