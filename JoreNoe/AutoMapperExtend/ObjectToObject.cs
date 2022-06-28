using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.AutoMapperExtend
{
    public class ObjectToObject : IObjectToObject
    {
        protected readonly IMapper Mapper;
        public ObjectToObject(IMapper Mapper)
        {
            this.Mapper = Mapper;
            ObjectStore.Mapper = Mapper;
        }
        public TTO Map<TFrom, TTO>(TFrom Source)
        {
            if (Source == null)
                return default;

            return Mapper.Map<TTO>(Source);
        }

        public TTO Map<TFrom, TTO>(TFrom Source, TTO Target)
        {
            if (Source == null || Target == null)
                return default;
            return Mapper.Map(Source, Target);
        }


    }
}