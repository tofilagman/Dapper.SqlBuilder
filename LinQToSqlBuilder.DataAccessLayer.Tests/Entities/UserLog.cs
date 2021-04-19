using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LinQToSqlBuilder.DataAccessLayer.Tests.Entities
{
    [Table("UserLog")]
    public  class UserLog : TableBase
    {
        public int ID_User { get; set; }
        public DateTime DateLog { get; set; }
        public DateTime DateSlide { get; set; }
        public Guid Guid { get; set; }
        public TimeSpan ExpiresIn { get; set; }
    }
}
