# JoreNoe Nuget包说明

安装方法

| Build                                   | NuGet                                                        | Downloads                                                    |
| --------------------------------------- | ------------------------------------------------------------ | ------------------------------------------------------------ |
| ![](./assets/NetCore-3.1-green.svg+xml) | [![](./assets/JoreNoe.svg+xml)](https://www.nuget.org/packages/JoreNoe) | <a href="https://www.nuget.org/packages/JoreNoe/" rel="nofollow noreferrer"><img src="./assets/JoreNoe.svg+xml" alt="NuGet Downloads"></a> |

```C
Install-Package JoreNoe -Version 6.7.5
```

### ORM使用说明

##### 1.Dapper 使用

首先第一步引用

```C#
using JoreNoe.DB.Dapper
```

第二步进行注册,写入StartUp 中  

```c#
 // SetInitDbContext
// 参数1：数据库链接字符串
//参数2：数据库类型，目前只支持MySql 和  SQLServer
//参数3：批量插入分页大小 ，默认二十万条 ，根据实际需求进行修改
 Registory.SetInitDbContext("Server=localhost;Database=tempdata;User ID=jorenoe;Password=zeNnwfyD5ue2z81V;", IDBType.MySql);
 Service.AddJoreNoeDapper(); // 注入JoreNoe.Dapper 
```

第三步实战使用

```C#
public class UserController
{
    // 注入Dapper 
     private readonly IRepository<Employees> Dapper;
     public UserController(IRepository<Employees> Dapper)
     {
         this.Dapper = Dapper;
     }
    
    // 测试方法
    [HttpPost("test")]
    public void test()
    {
        // 调用封装好的方法
        this.Dapper.Add(new Employees { });
    }
}
```



#### 2.EntityFramework.Core使用

##### 首先第一步引用

1.在仓储项目中创建 

​	1.1 RepositoryModule.cs

​	1.2 IntegratedPlatformSupporRegister.cs  名字可随意 

2.创建上下文 

​	2.1 IntegratedPlatformSupporDBContext.cs 名字随意 

##### 第二步具体代码实现

1.1.RepositoryModule.cs 文件 具体代码实现

```C#
using Autofac;
using JoreNoe;
using JoreNoe.DB.EntityFrameWork.Core.SqlServer;
namespace IntegratedPlatformSuppor.Repository
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IntegratedPlatformSupporRegister>().As<ICurrencyRegister>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Repository<,>)).As(typeof(IRepository<,>));
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
        }
    }
}
```

1.2.IntegratedPlatformSupporRegister.cs 文件具体代码实现

```C#
using JoreNoe;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace IntegratedPlatformSuppor.Repository
{
    public class IntegratedPlatformSupporRegister : ICurrencyRegister, IDisposable
    {
        private DbContext _dbContext;

        public IntegratedPlatformSupporRegister(IConfiguration Configuration)
        {
            this._dbContext = new IntegratedPlatformSupporDBContext { Configuration = Configuration };
        }

        public DbContext Dbcontext { get => this._dbContext; set { this._dbContext = value; } }

        public void Dispose()
        {
            this._dbContext.Dispose();
        }
    }
}

```

2.1.IntegratedPlatformSupporDBContext.cs 文件具体代码实现

```C#
using IntegratedPlatformSuppor.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IntegratedPlatformSuppor.Repository
{
    public class IntegratedPlatformSupporDBContext : DbContext
    {
        public IntegratedPlatformSupporDBContext()
        {
            //this.Configuration = configuration;
            //如果要访问的数据库存在，则不做操作，如果不存在，会自动创建所有数据表和模式
            //Database.EnsureCreated();

        }

        /// <summary>
        /// 配置
        /// </summary>
        public IConfiguration Configuration { set; get; }

        /// <summary>
        /// 用户
        /// </summary>
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(this.Configuration.GetConnectionString("DbConnect")))
                optionsBuilder.UseSqlServer(this.Configuration.GetConnectionString("DbConnect"));
            else
                optionsBuilder.UseSqlServer("Server=47.106.198.147;Database=IntegratedPlatformSuppor;Uid=sa;Password=JoreNoe123$%^");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Test>().HasQueryFilter(t => t.IsDelete == false);
            modelBuilder.Entity<User>().HasQueryFilter(t => t.IsDelete == false);
            modelBuilder.Entity<MeansCategory>().HasQueryFilter(d => !d.IsDelete); //.HasQueryFilter(t => t.IsDelete == false);
        }
    }
}

```

#### 进行注册

##### 第一步

1.使用AutoFac 

​	在项目中创建Autofac.json 文件 写入配置如下  根据实际情况进行自行调整

```json
{
  "modules": [
    { "type": "IntegratedPlatformSuppor.Repository.RepositoryModule,IntegratedPlatformSuppor.Repository" },
    { "type": "IntegratedPlatformSuppor.API.APIModule,IntegratedPlatformSuppor.API" },
    { "type": "IntegratedPlatformSuppor.DomainService.DomainServiceModule,IntegratedPlatformSuppor.DomainService" }
    //{ "type": "JoreNoe.Modules.JoreNoeModule,JoreNoe" }
  ]
}
```

2.WebApi 项目中 Program.cs 文件中写入 

```C#
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace IntegratedPlatformSuppor.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
             .UseServiceProviderFactory(new AutofacServiceProviderFactory())
             .ConfigureAppConfiguration((appConfiguration, builder) =>
             {
                 builder
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("Configs/Redis.json", optional: false, reloadOnChange: true)
               .AddJsonFile("Configs/Exceptionless.json", optional: false, reloadOnChange: true)
               .AddJsonFile("Configs/WeChatOpenConfig.json", optional: false, reloadOnChange: true)
               .AddEnvironmentVariables().Build();
             })
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder.UseStartup<Startup>();
                 webBuilder.UseUrls("http://*:5000");
             });
    }
}

```

3.StartUp.cs 中加入 

```C#
      public void ConfigureContainer(ContainerBuilder builder)
      {
          var config = new ConfigurationBuilder();
          config.AddJsonFile("./Configs/Autofac.json");
          builder.RegisterModule(new ConfigurationModule(config.Build()));
      }
```

### 实战使用

```C#
public class testDomainService :BaseRepository ,ItestDomainService
{
    private readonly IRepository<Guid, Test> test;
    public testDomainService(
        IRepository<Guid, Test> test,
        IUnitOfWork Unit):base(Unit)
    {
        this.test = test;
    }

    public TestValue k()
    {
        var xss = this.test.Single(Guid.NewGuid());
        return null;
    }

}
```

### 2.Redis 使用说明

JoreNoe.DB.Redis 使用方法

Startup 中注册上下文

```C#
//使用此方法进行注册 
//参数 Connection 你的Reids链接地址
//实例名称   
//默认数据库（Int 类型 ）
Register.SetInitRedisConfig(YourRedisConnection,InstanceName,defaultDB = 0)
```
