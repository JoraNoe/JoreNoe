using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace JoreNoe.AutoMapperExtend
{
    public static class ObjectToObjectExtension
    {

        public static void UseObjectToOBjectExtension(this IApplicationBuilder Builder)
        {
            UseMapper = Builder.ApplicationServices.GetService<IMapper>();
        }

        private static IMapper UseMapper { get; set; }

        public static TDestination Map<TSource, TDestination>(this TSource Source)
            where TDestination : class
            where TSource : class
        {
            if (Source == null) return default(TDestination);

            //var mapper = ConfigurationProvider.CreateMapper();

            return UseMapper.Map<TDestination>(Source);
        }

        public static TDestination Map<TSource, TDestination>(this TSource Source, TDestination Target)
            where TDestination : class
            where TSource : class
        {
            if (Source == null || Target == null)
                return default;


            //var mapper = ConfigurationProvider.CreateMapper();

            return UseMapper.Map(Source, Target);
        }

        public static TDestination Map<TDestination>(this object Source)
            where TDestination : class, new()
        {
            if (Source == null) return default(TDestination);
            var Tde = new TDestination();

            //var mapper = ConfigurationProvider.CreateMapper();

            return UseMapper.Map<TDestination>(Source);
        }

        public static IEnumerable<TDestination> Map<TDestination, TSource>(this IEnumerable<TSource> source)
            where TDestination : class
            where TSource : class
        {
            if (source == null) return new List<TDestination>();
            //var mapper = ConfigurationProvider.CreateMapper();
            return UseMapper.Map<List<TDestination>>(source);
        }

        public static IEnumerable<TDestination> MapList<TDestination>(this object source)
          where TDestination : class, new()
        {
            if (source == null) return new List<TDestination>();

            //var mapper = ConfigurationProvider.CreateMapper();
            return UseMapper.Map<List<TDestination>>(source);
        }


    }
}
