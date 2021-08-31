using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.DB.Models
{
    public class BaseModel
    {

        public BaseModel()
        {
            this.Id = Guid.NewGuid();
            this.IsDelete = false;
            this.CreateTime = DateTime.Now;

        }
        public Guid Id { get; set; } = Guid.NewGuid();

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
