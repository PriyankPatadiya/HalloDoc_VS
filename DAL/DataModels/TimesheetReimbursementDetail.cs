using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("TimesheetReimbursementDetail")]
public partial class TimesheetReimbursementDetail
{
    [Key]
    public int TimesheetDetailReimbursementId { get; set; }

    public int TimesheetDetailId { get; set; }

    [StringLength(500)]
    public string ItemName { get; set; } = null!;

    public int Amount { get; set; }

    [StringLength(500)]
    public string Bill { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    [StringLength(128)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [StringLength(128)]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("TimesheetReimbursementDetailCreatedByNavigations")]
    public virtual AspNetUser CreatedByNavigation { get; set; } = null!;

    [ForeignKey("ModifiedBy")]
    [InverseProperty("TimesheetReimbursementDetailModifiedByNavigations")]
    public virtual AspNetUser? ModifiedByNavigation { get; set; }

    [ForeignKey("TimesheetDetailId")]
    [InverseProperty("TimesheetReimbursementDetails")]
    public virtual TimesheetDetail TimesheetDetail { get; set; } = null!;
}
