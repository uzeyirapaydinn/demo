//------------------------------------------------------------------------------  
// <auto-generated>                                                               
// This code was generated by QuickCode.                                          
// Runtime Version:1.0                                                            
//                                                                                
// Changes to this file may cause incorrect behavior and will be lost if          
// the code is regenerated.                                                       
// </auto-generated>                                                              
//------------------------------------------------------------------------------  
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace QuickCode.Demo.UserManagerModule.Application.Dtos
{
    public record AspNetRoleClaimsDto
    {
        public int Id { get; init; }
        public string RoleId { get; init; }
        public string? ClaimType { get; init; }
        public string? ClaimValue { get; init; }
    }
}