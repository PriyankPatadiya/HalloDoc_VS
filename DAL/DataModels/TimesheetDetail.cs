using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("TimesheetDetail")]
public partial class TimesheetDetail
{
    [Key]
    public int TimesheetDetailId { get; set; }

    public int TimesheetId { get; set; }

    public DateOnly TimesheetDate { get; set; }

    public decimal? TotalHours { get; set; }

    public bool? IsWeekend { get; set; }

    public int? NumberOfHouseCall { get; set; }

    public int? NumberOfPhoneCall { get; set; }

    [StringLength(128)]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("TimesheetDetails")]
    public virtual AspNetUser? ModifiedByNavigation { get; set; }

    [ForeignKey("TimesheetId")]
    [InverseProperty("TimesheetDetails")]
    public virtual Timesheet Timesheet { get; set; } = null!;

    [InverseProperty("TimesheetDetail")]
    public virtual ICollection<TimesheetReimbursementDetail> TimesheetReimbursementDetails { get; set; } = new List<TimesheetReimbursementDetail>();
}
