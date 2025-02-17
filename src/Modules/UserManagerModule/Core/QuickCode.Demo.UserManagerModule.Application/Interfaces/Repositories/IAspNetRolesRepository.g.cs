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
using Microsoft.Data.SqlClient;
using QuickCode.Demo.UserManagerModule.Application.Models;
using QuickCode.Demo.UserManagerModule.Domain.Entities;
using QuickCode.Demo.UserManagerModule.Application.Dtos;
using System.Threading.Tasks;

namespace QuickCode.Demo.UserManagerModule.Application.Interfaces.Repositories
{
    public partial interface IAspNetRolesRepository : IBaseRepository<AspNetRoles>
    {
        Task<DLResponse<AspNetRoles>> GetByPkAsync(string id);
        Task<DLResponse<List<AspNetRolesAspNetUserRoles_RESTResponseDto>>> AspNetRolesAspNetUserRoles_RESTAsync(string aspNetRolesId);
        Task<DLResponse<AspNetRolesAspNetUserRoles_KEY_RESTResponseDto>> AspNetRolesAspNetUserRoles_KEY_RESTAsync(string aspNetRolesId, string aspNetUserRolesUserId);
        Task<DLResponse<List<AspNetRolesAspNetRoleClaims_RESTResponseDto>>> AspNetRolesAspNetRoleClaims_RESTAsync(string aspNetRolesId);
        Task<DLResponse<AspNetRolesAspNetRoleClaims_KEY_RESTResponseDto>> AspNetRolesAspNetRoleClaims_KEY_RESTAsync(string aspNetRolesId, int aspNetRoleClaimsId);
    }
}