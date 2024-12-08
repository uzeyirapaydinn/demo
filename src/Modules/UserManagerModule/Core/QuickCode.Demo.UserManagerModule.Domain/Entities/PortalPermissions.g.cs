using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using QuickCode.Demo.Common;

namespace QuickCode.Demo.UserManagerModule.Domain.Entities;

[Table("PortalPermissions")]
public partial class PortalPermissions  : BaseSoftDeletable 
{
	[Key]
	[Column("Id")]
	public int Id { get; set; }
	
	[Column("Name")]
	[StringLength(250)]
	public string Name { get; set; }
	
	[Column("ItemType")]
	[StringLength(1)]
	public string ItemType { get; set; }
	
	[InverseProperty("PortalPermission")]
	public virtual ICollection<PortalPermissionGroups> PortalPermissionGroups { get; } = new List<PortalPermissionGroups>();
}

