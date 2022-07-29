using AutoMapper;

namespace JoreNoe.AutoMapperExtend
{
    public class ObjectStore:IObjectStore
    {
        public IMapper Mapper;
        public ObjectStore(IMapper Mapper)
        {
            this.Mapper = Mapper;
            GetUseMapper = Mapper;
        }

        public static IMapper GetUseMapper { get; set; }
        public IMapper UseMapper { get => this.Mapper; set { this.Mapper = value; } }
    }
}
