using Autofac;
using AutoMapper;

namespace JoreNoe.Modules
{
    public class JoreNoeModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            ///注入Mapper
            builder.RegisterType<Mapper>().As<IMapper>().InstancePerLifetimeScope();

            var containner = builder.Build();
            using (var scope = containner.BeginLifetimeScope())
            {
                //用生命周期作用域解析获取IDateWriter对应的依赖对象实例
                var writer = scope.Resolve<Mapper>();
            }
        }
    }
}
