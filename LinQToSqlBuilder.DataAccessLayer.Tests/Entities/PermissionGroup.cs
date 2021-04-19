using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LinQToSqlBuilder.DataAccessLayer.Tests.Entities
{
    [Table("PermissionGroups")]
    public class PermissionGroup : TableBase
    {
        public string Name { get; set; }
        public string ResourcePath { get; set; }
        public UserTypeEnum UserType { get; set; }

        public DateTime Date { get; set; }

        public int ID_Perm { get; set; }

        public int? WorkCredit { get; set; }
        public bool? ntf { get; set; }
    }

    public enum UserTypeEnum
    {
        System = 0,
        Banker = 1,
        Administrator = 2,
        Agent = 3,
        Player = 4 //mobile
    }
}
