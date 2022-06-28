using AutoMapper;

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
