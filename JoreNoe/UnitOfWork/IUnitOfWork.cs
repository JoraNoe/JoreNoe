using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe
{
    public interface IUnitOfWork
    {
        void Commit();
    }
}
