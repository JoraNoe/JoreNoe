using Autofac;
using AutoMapper;
using JoreNoe.AutoMapperExtend;
using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.Modules
{
    public class JoreNoeModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            ///注入Mapper
            builder.RegisterType<Mapper>().As<IMapper>().InstancePerLifetimeScope();
            builder.RegisterType<ObjectStore>().As<IObjectStore>().InstancePerLifetimeScope();
        }
    }
}
