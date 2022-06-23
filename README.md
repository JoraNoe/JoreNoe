# JoreNoe

安装方法

| Build                                                     | NuGet                                                        | Downloads                                                    |
| --------------------------------------------------------- | ------------------------------------------------------------ | ------------------------------------------------------------ |
| ![](https://img.shields.io/badge/NetCore-3.1-green.svg) | [![](https://img.shields.io/nuget/v/JoreNoe.svg)](https://www.nuget.org/packages/JoreNoe) | <a href="https://www.nuget.org/packages/JoreNoe/" rel="nofollow noreferrer"><img src="https://img.shields.io/nuget/dt/JoreNoe?label=Downloads" alt="NuGet Downloads"></a>

```C
Install-Package JoreNoe -Version 6.4.9
```

JoreNoe.DB.SqlServer 使用方法 

可在 Startup 文件中 直接使用   

```c#
//使用方法 将EF 上下文 注册进入 
public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ZerroMoviesRegister>().As<ICurrencyRegister>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Repository<,>)).As(typeof(IRepository<,>));
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
        }
    }
```
Register 注册使用方法
```C#
public class ZerroMoviesRegister : ICurrencyRegister, IDisposable
    {
        private DbContext _dbContext;

        public ZerroMoviesRegister(IConfiguration Configuration)
        {
            this._dbContext = new ZerroMoviesDBCntext { Configuration = Configuration };
        }

        public DbContext Dbcontext { get => this._dbContext; set { this._dbContext = value; } }

        public void Dispose()
        {
            this._dbContext.Dispose();
        }
    }
```

注入方式
```C#

    public class MovieDomainService : BaseRepository, IMovieDomainService
    {
        private readonly IRepository<Guid, Movie> Repository;
        public MovieDomainService(
                IRepository<Guid, Movie> MovieRepository,
                IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this.RedisManager = RedisManager;
            this.MovieRepository = MovieRepository;
            this.Mapper = Mapper;
        }
}
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

