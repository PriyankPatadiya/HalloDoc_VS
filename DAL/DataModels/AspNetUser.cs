﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Index("Email", Name = "AspNetUsers_Ukey", IsUnique = true)]
public partial class AspNetUser
{
    [Key]
    [StringLength(128)]
    public string Id { get; set; } = null!;

    [StringLength(256)]
    public string UserName { get; set; } = null!;

    [Column(TypeName = "character varying")]
    public string? PasswordHash { get; set; }

    [StringLength(256)]
    public string? Email { get; set; }

    [Column(TypeName = "character varying")]
    public string? PhoneNumber { get; set; }

    [Column("IP")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? EndDate { get; set; }

    [InverseProperty("AspNetUser")]
    public virtual ICollection<Admin> AdminAspNetUsers { get; set; } = new List<Admin>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Admin> AdminCreatedByNavigations { get; set; } = new List<Admin>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Admin> AdminModifiedByNavigations { get; set; } = new List<Admin>();

    [InverseProperty("User")]
    public virtual AspNetUserRole? AspNetUserRole { get; set; }

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Business> BusinessCreatedByNavigations { get; set; } = new List<Business>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Business> BusinessModifiedByNavigations { get; set; } = new List<Business>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<PayrateByProvider> PayrateByProviderCreatedByNavigations { get; set; } = new List<PayrateByProvider>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<PayrateByProvider> PayrateByProviderModifiedByNavigations { get; set; } = new List<PayrateByProvider>();

    [InverseProperty("AspNetUser")]
    public virtual ICollection<Physician> PhysicianAspNetUsers { get; set; } = new List<Physician>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Physician> PhysicianCreatedByNavigations { get; set; } = new List<Physician>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Physician> PhysicianModifiedByNavigations { get; set; } = new List<Physician>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<ShiftDetail> ShiftDetails { get; set; } = new List<ShiftDetail>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Timesheet> TimesheetCreatedByNavigations { get; set; } = new List<Timesheet>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<TimesheetDetail> TimesheetDetails { get; set; } = new List<TimesheetDetail>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Timesheet> TimesheetModifiedByNavigations { get; set; } = new List<Timesheet>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<TimesheetReimbursementDetail> TimesheetReimbursementDetailCreatedByNavigations { get; set; } = new List<TimesheetReimbursementDetail>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<TimesheetReimbursementDetail> TimesheetReimbursementDetailModifiedByNavigations { get; set; } = new List<TimesheetReimbursementDetail>();

    [InverseProperty("AspNetUser")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
