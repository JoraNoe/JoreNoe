using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.AutoMapperExtend
{
    public interface IObjectStore
    {
        IMapper UseMapper { get; set; }
    }
}
