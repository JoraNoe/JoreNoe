# JoreNoe Package

安装方法

| Build                                                     | NuGet                                                        | Downloads                                                    |
| --------------------------------------------------------- | ------------------------------------------------------------ | ------------------------------------------------------------ |
| ![](https://img.shields.io/badge/NetCore-3.1-green.svg) | [![](https://img.shields.io/nuget/v/JoreNoe.svg)](https://www.nuget.org/packages/JoreNoe) | <a href="https://www.nuget.org/packages/JoreNoe/" rel="nofollow noreferrer"><img src="https://img.shields.io/nuget/dt/JoreNoe?label=Downloads" alt="NuGet Downloads"></a> |

```C#
Install-Package JoreNoe -Version 7.0.0.4
```



# 文档目录



#### **[ORM使用](#OPT1)**

- **[Dapper教程](#OPT1-1)**
- **[EntityFramework.Core教程](#OPT1-2)**

#### [Redis使用](#OPT2)

#### [发送消息](#OPT3)

#### [帮助扩展方法](#OPT4)

#### [中间件使用](#OPT5)

#### [RabbitMQ](#OPT6)

# ORM使用说明

**JoreNoe包目前支持数据库：Mysql , SqlServer** 

**支持，ORM框架 Dapper，EFCore** 



<a name="OPT1-1"></a>

## 1.Dapper 使用



#### 首先第一步引用

```C#
using JoreNoe.DB.Dapper
```

#### 第二步进行注册 

在您的应用程序启动时，将服务添加到依赖注入容器中。您可以在 Startup.cs 文件中的 ConfigureServices 方法中调用 AddJoreNoeDapper 方法来注册服务。

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddJoreNoeDapper("your_connection_string_here", IDBType.SqlServer);
    // 或者
    // services.AddJoreNoeDapper("your_connection_string_here", IDBType.MySql);
}

-- 最新版本

// 单个模式注入
//builder.Services.AddJoreNoeDapper("your_connection_string_here", IDBType.MySql, true);

// 多个模式注入
//builder.Services.AddJoreNoeDapper(
//     new List<DatabaseSettings>
//        {
//            new DatabaseSettings("your_connection_string_here",IDBType.MySql,true,
//            AvailableTables:new List<string>{
//                "User"// 表名
//            }),
//            new DatabaseSettings("your_connection_string_here",IDBType.MySql,true,
//            AvailableTables:new List<string>{
//                "test"
//            }),

//        }
//);

```

#### **第三步使用服务**

```C#
public class YourService
{
    private readonly IRepository<test> TestRepository;

    public YourService(IRepository<test> TestRepository)
    {
        this.TestRepository = TestRepository;
    }

    public void YourMethod()
    {
        this.TestRepository.Add(new ...);
    }
}
```

#### 属性获取

```C#
public class YourService
{
    private readonly IDatabaseService dataBaseService;

    public YourService(IDatabaseService dataBaseService)
    {
        this.dataBaseService = dataBaseService;
    }

    public IDbConnection GetConnection()
    {
        this.dataBaseService.GetConnection();
    }
    
    public string GetPropValue()
    {
        return this.dataBaseService.DataBaseSettings.connectionString; // 返回链接字符串
        return this.dataBaseService.DataBaseSettings.dbType; // 返回数据库类型
         return this.dataBaseService.DataBaseSettings.mulitInsertBatchcount; // 返回批量插入 一批次数量
        
    }
    
}
```



#### 不使用注入方式

```C#
public class UserController
{
    var database = new Repository<test>(new DatabaseService("your_connection_string_here",默认Mysql,默认20万));
    database.add(new test{...});
}
```



<a name="OPT1-2"></a>

## 2.EntityFramework.Core使用



#### 首先第一步引用

**1.在仓储项目中创建** 

```C#
1.1 RepositoryModule.cs

1.2 IntegratedPlatformSupporRegister.cs  名字可随意 
```

**2.创建上下文** 

```C#
2.1 IntegratedPlatformSupporDBContext.cs 名字随意 
```

#### 第二步具体代码实现

**1.1.RepositoryModule.cs 文件 具体代码实现**

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

**1.2.IntegratedPlatformSupporRegister.cs 文件具体代码实现**

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

**2.1.IntegratedPlatformSupporDBContext.cs 文件具体代码实现**

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

**1.使用AutoFac** 

	在项目中创建Autofac.json 文件 写入配置如下  根据实际情况进行自行调整

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

**2.WebApi 项目中 Program.cs 文件中写入** 

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

**3.StartUp.cs 中加入** 

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

<a name="OPT2"></a>

## 3.Redis 使用说明



#### 如何使用

**1.注入  JoreNoe Redis 中注册上下文**

```C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddJoreNoeRedis("your_connection_string_here", "InstanceName",DefaultDB=0);
}
```

**2.如何使用Redis**

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

##### 	3.直接调用

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

# 发送消息

**目前支持：email  发送**  



## 1.邮箱发送

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

# 帮助扩展方法



**支持：boolean，字典转SQL，映射，实体转字典，Resolve扩展**

**1.bool 扩展方法**

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

##### 	2.映射（AutoMapper）

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

### 3.Resolve扩展

```C#
// 在程序启动时设置容器工厂
AutofacResolver.SetContainerFactory(() => container);

// 在需要解析依赖项的地方使用 AutofacResolver
var service = AutofacResolver.Resolve<IMyService>();

```



#### 4.网络请求HttpClientAPI 

4.1注入方式

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

4.2 直接使用方式

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

#### 5.Swagger暗黑主题

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

# 中间件的使用



#### 1.全局错误日志中间件

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

#### 2.全局运行日志中间件

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


<a name="OPT6"></a>

# RabbitMQ使用



### 1.初始化

```C#
 // 在Program  或者 StrartUp 中 进行初始化
 // 加入RabbitMQ 外部使用 监控使用 特殊用法
 JoreNoe.Queue.RBMQ.Register.RegisterQueue("Ip", "账户", "密码", "/虚机", "队列名称");
 // 例子
 JoreNoe.Queue.RBMQ.Register.RegisterQueue("124.70.12.123", "jorenoe", "jorenoe", "/", "Moitoring");

```



### 注意 如果只推送 不接受按照第一步初始化即可，如果需要接受请按一下配置

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



### 2.使用 推送 和 接受

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
Jorenoe and 










