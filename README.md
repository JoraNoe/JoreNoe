# 🎉 JoreNoe Package

[📖 线上文档](https://jorenoe.gitbook.io/jorenoe-docs/getting-started/quickstart)

## 📦 安装方法

| Build                                                        | NuGet                                                        | Downloads                                                    | CreateTime                                                   | Laster  Commit                                               |
| ------------------------------------------------------------ | ------------------------------------------------------------ | ------------------------------------------------------------ | ------------------------------------------------------------ | ------------------------------------------------------------ |
| ![.NET Version](https://img.shields.io/badge/.NET-6.0-purple?logo=.net) | [![](https://img.shields.io/nuget/v/JoreNoe?logo=nuget&label=jorenoe)](https://www.nuget.org/packages/JoreNoe) | <a href="https://www.nuget.org/packages/JoreNoe/" rel="nofollow noreferrer"><img src="https://img.shields.io/nuget/dt/JoreNoe?logo=nuget&label=Downloads" alt="NuGet Downloads"></a> | ![](https://img.shields.io/nuget/v/JoreNoe?logo=nuget&label=2021-08-31) | ![GitHub last commit](https://img.shields.io/github/last-commit/JoraNoe/JoraNoe?logo=github) |

```sh
Install-Package JoreNoe -Version Laster
install-package Jorenoe -version 7.4.6
```

---


# 📂 文档目录

#### **[ORM使用](#OPT1)**

- **[Dapper教程](#OPT1-1)**
- **[EntityFramework.Core教程](#OPT1-2)**

#### [Redis扩展如何使用](#OPT2)

#### [发送Email消息扩展如何使用](#OPT3)

#### [辅助开发帮助类或者函数如何使用](#OPT4)

#### [NetCore中间件使用](#OPT5)

#### [RabbitMQ如何使用](#OPT6)

# ORM使用说明

**JoreNoe包目前支持数据库：Mysql , SqlServer** 

**支持，ORM框架 Dapper，EFCore** 



<a name="OPT1-1"></a>

# 🏗 1. Dapper 使用指南

## 📌 第一步：引用依赖

在代码中引入 **JoreNoe.DB.Dapper**，确保 Dapper 能够正确使用。

```csharp
using JoreNoe.DB.Dapper;
```

---

## 🛠 第二步：注册 Dapper 服务

在应用程序启动时，将 Dapper 添加到 **依赖注入容器**。  
在 **Startup.cs** 文件中的 `ConfigureServices` 方法中，调用 `AddJoreNoeDapper` 进行注册：

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddJoreNoeDapper("your_connection_string_here", IDBType.SqlServer);
    // 或者
    services.AddJoreNoeDapper("your_connection_string_here", IDBType.MySql);
}
```

### 🔹 最新版本支持 **单库 & 多库模式**

#### **单库模式**

```csharp
// 单个模式注入
builder.Services.AddJoreNoeDapper("your_connection_string_here", IDBType.MySql, true);
```

#### **多库模式**

```csharp
// 多个模式注入
builder.Services.AddJoreNoeDapper(
    new List<DatabaseSettings>
    {
        new DatabaseSettings("your_connection_string_here", IDBType.MySql, true,
        AvailableTables: new List<string> { "User" }), // 绑定 User 表

        new DatabaseSettings("your_connection_string_here", IDBType.MySql, true,
        AvailableTables: new List<string> { "test" }) // 绑定 test 表
    }
);
```

---

## 🚀 第三步：使用 Dapper 服务

### **🔹 在业务逻辑中使用 Repository**

```csharp
public class YourService
{
    private readonly IRepository<test> TestRepository;

    public YourService(IRepository<test> TestRepository)
    {
        this.TestRepository = TestRepository;
    }

    public void YourMethod()
    {
        this.TestRepository.Add(new test { ... });
    }
}
```

---

## 🔍 获取数据库属性

```csharp
public class YourService
{
    private readonly IDatabaseService dataBaseService;

    public YourService(IDatabaseService dataBaseService)
    {
        this.dataBaseService = dataBaseService;
    }

    public IDbConnection GetConnection()
    {
        return this.dataBaseService.GetConnection(); // 获取数据库连接
    }
    
    public string GetPropValue()
    {
        return this.dataBaseService.DataBaseSettings.connectionString;  // 数据库连接字符串
        return this.dataBaseService.DataBaseSettings.dbType;            // 数据库类型
        return this.dataBaseService.DataBaseSettings.mulitInsertBatchcount; // 批量插入时每批数量
    }
}
```

---

## ❌ 不使用依赖注入方式（手动创建实例）

如果不想使用 **依赖注入（DI）**，可以手动创建 `Repository` 实例：

```csharp
public class UserController
{
    var database = new Repository<test>(new DatabaseService("your_connection_string_here", IDBType.MySql, 200000));
    database.Add(new test { ... });
}
```

---

## ✅ 总结：

- **支持单库 & 多库模式**
- **支持依赖注入 & 手动实例化**
- **提供数据库连接 & 批量插入配置**
- **便捷的 Repository 操作，提升开发效率**

🚀 **Dapper + JoreNoe 让数据操作更简单！** 🎯



<a name="OPT1-2"></a>

# 🏗 2. EntityFramework.Core 使用指南

## 📌 第一步：项目结构

在 **仓储项目** 中创建以下文件：

```csharp
1. RepositoryModule.cs
2. IntegratedPlatformSupporRegister.cs  // 名称可自定义
```

在 **数据访问层** 中创建 **DbContext** 文件：

```csharp
3. IntegratedPlatformSupporDBContext.cs  // 名称可自定义
```

---

## 🛠 第二步：代码实现

### **1️⃣ RepositoryModule.cs - 依赖注入模块**

```csharp
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

### **2️⃣ IntegratedPlatformSupporRegister.cs - 注册 DbContext**

```csharp
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

### **3️⃣ IntegratedPlatformSupporDBContext.cs - 数据库上下文**

```csharp
using IntegratedPlatformSuppor.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IntegratedPlatformSuppor.Repository
{
    public class IntegratedPlatformSupporDBContext : DbContext
    {
        public IntegratedPlatformSupporDBContext() { }

        public IConfiguration Configuration { set; get; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connStr = this.Configuration.GetConnectionString("DbConnect");
            optionsBuilder.UseSqlServer(string.IsNullOrEmpty(connStr) ? "Server=47.106.198.147;Database=IntegratedPlatformSuppor;Uid=sa;Password=JoreNoe123$%^" : connStr);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Test>().HasQueryFilter(t => t.IsDelete == false);
            modelBuilder.Entity<User>().HasQueryFilter(t => t.IsDelete == false);
            modelBuilder.Entity<MeansCategory>().HasQueryFilter(d => !d.IsDelete);
        }
    }
}
```

---

## 🔹 第三步：注册 AutoFac 依赖

在项目根目录创建 **Autofac.json**，调整模块配置：

```json
{
  "modules": [
    { "type": "IntegratedPlatformSuppor.Repository.RepositoryModule,IntegratedPlatformSuppor.Repository" },
    { "type": "IntegratedPlatformSuppor.API.APIModule,IntegratedPlatformSuppor.API" },
    { "type": "IntegratedPlatformSuppor.DomainService.DomainServiceModule,IntegratedPlatformSuppor.DomainService" }
  ]
}
```

### **1️⃣ WebApi 项目 Program.cs 配置**

```csharp
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
                 builder.SetBasePath(Directory.GetCurrentDirectory())
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

### **2️⃣ 在 Startup.cs 里注册 AutoFac**

```csharp
public void ConfigureContainer(ContainerBuilder builder)
{
    var config = new ConfigurationBuilder();
    config.AddJsonFile("./Configs/Autofac.json");
    builder.RegisterModule(new ConfigurationModule(config.Build()));
}
```

---

## 🚀 实战使用示例

```csharp
public class testDomainService : BaseRepository, ItestDomainService
{
    private readonly IRepository<Guid, Test> _testRepository;

    public testDomainService(IRepository<Guid, Test> testRepository, IUnitOfWork unit) : base(unit)
    {
        _testRepository = testRepository;
    }

    public TestValue GetTest()
    {
        return _testRepository.Single(Guid.NewGuid());
    }
}
```

---

## ✅ 总结

- **支持 AutoFac 依赖注入**，模块化管理依赖
- **统一管理数据库上下文**，支持多数据库配置
- **提供自动查询过滤器**，提升数据查询效率
- **可扩展性强**，适用于企业级应用

🚀 **EFCore + JoreNoe 让数据访问更简单高效！** 🎯


<a name="OPT2"></a>

# 3. 🏗 Redis 使用说明

### 如何使用

#### 1. 注入 JoreNoe Redis 中注册上下文

📌 **步骤 1：注入 Redis**

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddJoreNoeRedis("your_connection_string_here", "InstanceName", DefaultDB = 0);
}

 🔧 **步骤 2: 如何使用Redis**

```C#
using  JoreNoe.Cache.Redis;
public class RedisTest
{
    private readonly JoreNoe.Cache.Redis.IRedisManager ReadisManager;
    public RedisTest(JoreNoe.Cache.Redis.IRedisManager ReadisManager) {
        this.ReadisManager = ReadisManager;
    }

    public void test()
    {
        this.ReadisManager.Add("Test", "test", JoreNoe.Cache.Redis.ExpireModel.LongCache);

        Console.WriteLine(this.ReadisManager.Get("Test"));
    }
}
```

 🔗	**步骤3：手动创建实例并调用**

```C#
JoreNoe.Cache.Redis.JoreNoeRedisBaseService RedisDataBase = new JoreNoe.Cache.Redis.JoreNoeRedisBaseService(new JoreNoe.Cache.Redis.SettingConfigs {
    ConnectionString= "localhost:6379,password=mima",
    DefaultDB=1,
    InstanceName="TestRedis"
});

JoreNoe.Cache.Redis.IRedisManager RedisManager = new JoreNoe.Cache.Redis.RedisManager(RedisDataBase);

RedisManager.Add("Test","test", JoreNoe.Cache.Redis.ExpireModel.LongCache);

Console.WriteLine(RedisManager.Get("Test"));

Console.ReadLine();
```


<a name="OPT3"></a>

# 🏗 4.发送消息

**📧目前支持：Email**  

## 📌1.邮箱发送

**如何使用**

```C#
using JoreNoe.Message;
public class test{
    public void sendtest(){
        // 首先注册 
        var EmailHelper = new EmailMessageAPI(发送者，SMTP地址，SMTP端口，密码（个人是授权码），是否开启SSL认证);
        EmailHelper.Send(收件人，标题，主题内容，是否开启兼容HTML);
    }
}
```



<a name="OPT4"></a>

# 🏗 5.帮助扩展方法

**🚀支持：boolean，字典转SQL，映射，实体转字典，Resolve扩展**

## 🔗1.bool 扩展方法

```C#
using JoreNoe.Extend;

public class test{
    
    public void sendtest(){

         /// <summary>
 /// 可用枚举类型 默认 1
 /// 类型1：IsOrDeny 是 否
 /// 类型2：TrueOrFalse 真 假
 /// 类型3：OnOrOff 开 关 
 /// 类型4：EnableOrDisable 启用 关闭
 /// </summary>
        
        var booltest = false;
         var REsult = booltest.BooleanToString(AvailableType.IsOrDeny);
        // 输出 否
        
    }
}
```

## 🔗2.映射（AutoMapper）

```C#
// 直接使用方式 
var config = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<test, test1>();
    cfg.CreateMap<test1, test>();
});
var mapper = new Mapper(config);
JoreNoe.Extend.JoreNoeObjectToObjectExtension.UseJoreNoeObjectToOBject(mapper);
var test = new test() {
    name = "c",
    age=123
};
var test1 = new test1();
// 将 test 数据 给 test1
var ment = test.Map(test1);
Console.ReadLine();

// NET 使用方式
// StartUp 
 public partial class Startup
    {
        protected void AddAutoMapper(IServiceCollection services)
        {
            services.TryAddSingleton<MapperConfigurationExpression>();
            services.TryAddSingleton(serviceProvider =>
            {
                var mapperConfigurationExpression = serviceProvider.GetRequiredService<MapperConfigurationExpression>();
                var instance = new MapperConfiguration(mapperConfigurationExpression);
                
                instance.AssertConfigurationIsValid();
                return instance;
            });
            services.TryAddSingleton(serviceProvider =>
            {
                var mapperConfiguration = serviceProvider.GetRequiredService<MapperConfiguration>();
                return mapperConfiguration.CreateMapper();
            });
        }
        public void UseAutoMapper(IApplicationBuilder applicationBuilder)
        {
            var config = applicationBuilder.ApplicationServices.GetRequiredService<MapperConfigurationExpression>();
            
            //订单
            config.CreateMap<OrderModel, Order>(MemberList.None);
            config.CreateMap<Order, OrderValue>(MemberList.None);

            //config.CreateMap<User, UserInfo>().ForMember(d => d.names, option => option.MapFrom(d => d.name)).ReverseMap();
        }
     
     
     // Program
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
     
     
     // StartUp Configure 中  
     public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ZerroMovies.API v1"));
            }
            app.UseObjectToOBjectExtension();
        }
```

## 🔗3.Resolve扩展

```C#
// 在程序启动时设置容器工厂
AutofacResolver.SetContainerFactory(() => container);

// 在需要解析依赖项的地方使用 AutofacResolver
var service = AutofacResolver.Resolve<IMyService>();

```

## 🔗4.网络请求HttpClientAPI 

  ### 🔗4.1注入方式

```C#
// 注入 在 StartUp 或者 （NEt6以上在Program中注册）
 services.AddHttpClientApi();

//使用Demo
using Microsoft.AspNetCore.Mvc;
using JoreNoe.JoreHttpClient; // 引入你的命名空间

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly HttpClientApi _httpClientApi;

        public TestController(HttpClientApi httpClientApi)
        {
            _httpClientApi = httpClientApi;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = await _httpClientApi.GetAsync("https://api.example.com/data");
                return Ok(response);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("post")]
        public async Task<IActionResult> Post([FromBody] string content)
        {
            try
            {
                var response = await _httpClientApi.PostAsync("https://api.example.com/data", content);
                return Ok(response);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
```

 ### 🔗4.2 直接使用方式

```C#
 // 创建 HttpClientHandler（可以配置 SSL 验证等）
var handler = new HttpClientHandler
{
    // 例如：禁用 SSL 证书验证
    ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
};

// 创建 HttpClient 实例
var httpClient = new HttpClient(handler)
{
    BaseAddress = new Uri("https://api.example.com/")
};

// 创建 IHttpClientFactory 的模拟实现
var httpClientFactory = new FakeHttpClientFactory(httpClient);

// 创建 HttpClientApi 实例
var httpClientApi = new HttpClientApi(httpClientFactory);

// 使用 HttpClientApi 发送请求
try
{
    var response = await httpClientApi.GetAsync("data");
    Console.WriteLine(response);
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

## 🔗5.Swagger暗黑主题

```C#
app.UseSwagger();
app.UseJoreNoeSwaggerThemeDark(); // 注入
   app.UseSwaggerUI(option =>
   {
       option.InjectStylesheet(SwaggerThemsExtend.DarkTheme); // 注入主题地址
       
       option.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
       option.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");

   });

```



<a name="OPT5"></a>

# 🚀 6.中间件的使用

## 🔗1.全局错误日志中间件

```C#
// 使用方式1 webapi 全局错误日志中间件  直接使用方式
app.UseJoreNoeGlobalErrorHandlingMiddleware(async (ex, context) =>
{
    // 返回错误信息 // 处理自己的数据
    await Console.Out.WriteLineAsync(ex.Message);
});


// 使用方式2 注入 自定义类继承使用方式
builder.Services.AddJoreNoeGlobalErrorHandlingMiddleware<TestErrorMiddleWare>();
app.UseJoreNoeGlobalErrorHandlingMiddleware();
// 使用案例
using JoreNoe.Middleware;

namespace TestNET6Project
{
    public class TestErrorMiddleWare : IJoreNoeGlobalErrorHandling
    {
        public async Task GlobalErrorHandling(Exception Ex)
        {
            await Console.Out.WriteLineAsync(JoreNoeRequestCommonTools.FormatError(Ex));
        }
    }
}

```

## 🔗2.全局运行日志中间件

```C#
// webapi 全局运行日志中间件  直接使用方式
app.UseJoreNoeRequestLoggingMiddleware(info => {
    Console.WriteLine("方法"+info.Method);
    Console.WriteLine("路径" + info.Path);
    Console.WriteLine("开始时间" + info.StartTime);
    Console.WriteLine("总时长" + info.Duration);
    Console.WriteLine("Get请求参数" + info.QueryString);
    Console.WriteLine("BODY请求参数" + info.RequestBody);
    Console.WriteLine("完整路径" + info.FullPathUrl);
    Console.WriteLine("Headers" + info.Headers);
});

// 注入 自定义类继承使用方式
builder.Services.AddJoreNoeRequestLoggingMiddleware<TestMiddleWare>();
app.UseJoreNoeRequestLoggingMiddleware();
// 使用案例
using JoreNoe.Middleware;

namespace TestNET6Project
{
    public class TestMiddleWare : IJorenoeRuningRequestLogging
    {
        public async Task RunningRequestLogging(JorenoeRuningRequestLoggingModel info)
        {
            Console.WriteLine("方法" + info.Method);
            Console.WriteLine("路径" + info.Path);
            Console.WriteLine("开始时间" + info.StartTime);
            Console.WriteLine("总时长" + info.Duration);
            Console.WriteLine("Get请求参数" + info.QueryString);
            Console.WriteLine("BODY请求参数" + info.RequestBody);
            Console.WriteLine("完整路径" + info.FullPathUrl);
            Console.WriteLine("Headers" + info.Headers);
        }
    }
}

```

## 🔗3.全局方法控制中间件

### 📌 中间件说明

该中间件依赖于 **Redis**，在项目启动时，会将所有 **控制器方法** 写入 Redis，默认存储格式如下：

- **Key**：`控制器/方法名`
- Value：
  - `true`：可用状态 ✅（正常访问）
  - `false`：不可用状态 ❌（请求将直接返回预设内容，如 “拒绝访问”）

### ⚙️ 配置项

- **`LocalCacheDurationInMinutes`**：本地缓存有效时间，默认 **30 分钟**，可根据需求自行调整。
- **`ReturnMessage`**：被拒绝后响应的内容，默认为："Access Denied",可自定义设置，支持HTML

```C#
// 使用方式 在 Prgrame 中 
builder.Services.AddJoreNoeJoreNoeIntefaceAccessMiddleware(LocalCacheDurationInMinutes: TimeSpan.FromMinutes(5)
                                                           ,"拒绝请求内容，支持HTML");

app.UseJoreNoeIntefaceAccessMiddleware();
```

## 🔗4.全局IP请求控制中间件

### 📌 中间件说明

该中间件依赖于 **Redis**，在项目启动时，会将 **非法请求的IP** 写入 Redis，默认存储格式如下：

- **Key**：`ProjectBlackLists`
- Value：`[127.0.0.1]`：❌（请求将直接返回预设内容）
- 默认返回内容存储在Reids中key如下
- **Key**:`DeniedReturnMessage`

### ⚙️ 配置项

- **`maxRequestCount`**：每spanTime/ 多少次 ，必填
- **`spanTime`**：spanTime内支持请求多少次，同一个IP，必填
- **`TimeSpanLocalCache`**：本地缓存时间，拒绝访问响应内容和黑名单IP存储
- **`isEnabledRequestLimit`**：是否启用计数限制，如启用将在每spantime=1分钟/maxRequestCount=100次，每分钟请求超过一百次后记录次IP拒绝访问，如不启用将不进行限制，可手动添加黑名单到Reids数组中

```C#
builder.Services.AddJoreNoeSystemIPBlackListMiddleware(300, TimeSpan.FromSeconds(60), TimeSpan.FromMinutes(6), true);
app.UseJoreNoeSystemIPBlackListMiddleware();
```



<a name="OPT6"></a>

# 🚀 7.RabbitMQ使用

## 🔗1.初始化

```C#
 // 在Program  或者 StrartUp 中 进行初始化
 // 加入RabbitMQ 外部使用 监控使用 特殊用法
 JoreNoe.Queue.RBMQ.Register.RegisterQueue("Ip", "账户", "密码", "/虚机", "队列名称");
 // 例子
 JoreNoe.Queue.RBMQ.Register.RegisterQueue("124.70.12.123", "jorenoe", "jorenoe", "/", "Moitoring");

```

#### 📌注意 如果只推送 不接受按照第一步初始化即可，如果需要接受请按一下配置

```C#
JoreNoe.Queue.RBMQ.Register.RegisterQueue("124.70.12.123", "jorenoe", "jorenoe", "/", "Moitoring");
QueueManager.Receive<MoitoringEvent>(new CustomerRabbitMQ(), "Moitoring");// 增加一条接受配置

 public class CustomerRabbitMQ : ICustome<MoitoringEvent>
    {
        public async Task<MoitoringEvent> ConSume(CustomeContent<MoitoringEvent> Context)
        {
            MessageBox.Show(Context.Context.SID);
            return null;
        }
    }
```

## 🔗2.使用 推送 和 接受

```C#
public class MoitoringEvent
{
    /// <summary>
    /// 设备ID
    /// </summary>
    public string SID { get; set; }

    /// <summary>
    /// 上线还是下线
    /// </summary>
    public string Type { get; set; }
}


// 推送
QueueManager.SendPublish<MoitoringEvent>(new MoitoringEvent { SID = SID,Type= Type });

// 接收
 public class CustomerRabbitMQ : ICustome<MoitoringEvent>
    {
        public async Task<MoitoringEvent> ConSume(CustomeContent<MoitoringEvent> Context)
        {
            MessageBox.Show(Context.Context.SID);
            return null;
        }
    }
```

# 📜 版权声明

版权所有 © JoreNoe。保留所有权利。


## 📞 联系方式

📧 **Email**: [jorenoe@163.com](mailto:jorenoe@163.com)  
🌍 **官网**: [JoreNoe 官方网站](https://jorenoe.top)  

---

**JoreNoe** 感谢您的支持！🚀  