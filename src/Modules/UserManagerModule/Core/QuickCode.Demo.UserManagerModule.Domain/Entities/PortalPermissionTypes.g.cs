using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using QuickCode.Demo.Common;

namespace QuickCode.Demo.UserManagerModule.Domain.Entities;

[Table("PortalPermissionTypes")]
public partial class PortalPermissionTypes  
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	[Column("Id")]
	public int Id { get; set; }
	
	[Column("Name")]
	[StringLength(50)]
	public string Name { get; set; }
	
	[InverseProperty("PortalPermissionType")]
	public virtual ICollection<PortalPermissionGroups> PortalPermissionGroups { get; } = new List<PortalPermissionGroups>();
}

