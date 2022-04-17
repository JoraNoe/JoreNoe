# JoreNoe

安装方法

| Build                                                     | NuGet                                                        | Downloads                                                    |
| --------------------------------------------------------- | ------------------------------------------------------------ | ------------------------------------------------------------ |
| ![](https://img.shields.io/badge/NetCore-3.1-green.svg) | [![](https://img.shields.io/nuget/v/JoreNoe.svg)](https://www.nuget.org/packages/JoreNoe) | <a href="https://www.nuget.org/packages/JoreNoe/" rel="nofollow noreferrer"><img src="https://img.shields.io/nuget/dt/JoreNoe?label=Downloads" alt="NuGet Downloads"></a>

```C
Install-Package JoreNoe -Version 5.1.2
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

