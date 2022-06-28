using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.AutoMapperExtend
{
    public static class ObjectToObjectExtension
    {
        public static TTO Map<TFrom, TTO>(this TFrom Source)
        {
            if (Source == null)
                return default;

            return ObjectStore.Mapper.Map<TTO>(Source);
        }

        public static TTO Map<TFrom, TTO>(this TFrom Source, TTO Target)
        {
            if (Source == null || Target == null)
                return default;
            return ObjectStore.Mapper.Map(Source, Target);
        }

        public static TTO Map<TTO>(this object Source)
        {
            if (Source == null)
                return default;

            return ObjectStore.Mapper.Map<TTO>(Source);
        }

        public static IList<TTO> Map<TTO>(this IList<TTO> source)
        {
            if (source == null)
                return default;

            return ObjectStore.Mapper.Map<IList<TTO>>(source);
        }

    }
}
