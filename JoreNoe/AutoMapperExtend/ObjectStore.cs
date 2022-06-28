using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.AutoMapperExtend
{
    public class ObjectStore
    {
        public static void ObjectSingleInstance(IMapper Mapper)
        {
            ObjectStore.Mapper = Mapper;
        }

        public static IMapper Mapper;
    }
}
