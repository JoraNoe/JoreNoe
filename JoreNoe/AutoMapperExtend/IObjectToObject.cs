using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.AutoMapperExtend
{
    public interface IObjectToObject
    {
        TTO Map<TFrom, TTO>(TFrom Source);

        TTO Map<TFrom, TTO>(TFrom Source, TTO Target);
    }
}
