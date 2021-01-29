using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LinQToSqlBuilder.DataAccessLayer.Tests.Entities
{
    public abstract class TableBase
    {
        [Key]
        public int ID { get; set; }
    }
}
