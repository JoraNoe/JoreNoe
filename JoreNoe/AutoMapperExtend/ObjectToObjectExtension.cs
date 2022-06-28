using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.AutoMapperExtend
{
    public static class ObjectToObjectExtension
    {

        private static IConfigurationProvider ConfigurationProvider;

        public static void UseAutoMapperExtend(IConfigurationProvider Configuration)
        {
            ConfigurationProvider = Configuration;
        }

        public static TDestination Map<TSource, TDestination>(this TSource Source)
            where TDestination : class
            where TSource : class
        {
            if (Source == null) return default(TDestination);

            var mapper = ConfigurationProvider.CreateMapper();

            return mapper.Map<TDestination>(Source);
        }

        public static TDestination Map<TSource, TDestination>(this TSource Source, TDestination Target)
            where TDestination : class
            where TSource : class
        {
            if (Source == null || Target == null)
                return default;


            var mapper = ConfigurationProvider.CreateMapper();

            return mapper.Map(Source, Target);
        }

        public static TDestination Map<TDestination>(this object Source)
            where TDestination : class, new()
        {
            if (Source == null) return default(TDestination);
            var Tde = new TDestination();

            var mapper = ConfigurationProvider.CreateMapper();

            return mapper.Map<TDestination>(Source);
        }

        public static IEnumerable<TDestination> Map<TDestination, TSource>(this IEnumerable<TSource> source)
            where TDestination : class
            where TSource : class
        {
            if (source == null) return new List<TDestination>();
            var mapper = ConfigurationProvider.CreateMapper();
            return mapper.Map<List<TDestination>>(source);
        }

        public static IEnumerable<TDestination> MapList<TDestination>(this object source)
          where TDestination : class, new()
        {
            if (source == null) return new List<TDestination>();

            var mapper = ConfigurationProvider.CreateMapper();
            return mapper.Map<List<TDestination>>(source);
        }


    }
}
