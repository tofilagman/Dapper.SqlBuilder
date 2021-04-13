using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinQToSqlBuilder.DataAccessLayer.Tests.Entities
{
    [Table("tDailySchedule")]
    public class DailySchedule
    {
        [Key]
        public int Id { get; set; }
        public int ID_DailyScheduleSun { get; set; }
        public int ID_DailyScheduleMon { get; set; }
        public int ID_DailyScheduleTue { get; set; }
        public int ID_DailyScheduleWed { get; set; }
        public int ID_DailyScheduleThu { get; set; }
        public int ID_DailyScheduleFri { get; set; }
        public int ID_DailyScheduleSat { get; set; }

    }
}
