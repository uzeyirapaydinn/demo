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
using QuickCode.Demo.Portal.Models;
using QuickCode.Demo.Portal.Models.UserManagerModule;
using QuickCode.Demo.Portal.Helpers;
using Microsoft.AspNetCore.Mvc;
using UserManagerModuleContracts = QuickCode.Demo.Common.Nswag.Clients.UserManagerModuleApi.Contracts;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using QuickCode.Demo.Portal.Helpers.Authorization;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using AutoRest.Core.Utilities.Collections;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuickCode.Demo.Portal.Controllers.UserManagerModule
{
    [Permission("UserManagerModuleAspNetUserRoles")]
    [Area("UserManagerModule")]
    [Route("UserManagerModuleAspNetUserRoles")]
    public partial class AspNetUserRolesController : BaseController
    {
        private int pageSize = 20;
        private readonly UserManagerModuleContracts.IAspNetUserRolesClient pageClient;
        private readonly UserManagerModuleContracts.IAspNetUsersClient pageAspNetUsersClient;
        private readonly UserManagerModuleContracts.IAspNetRolesClient pageAspNetRolesClient;
        public AspNetUserRolesController(UserManagerModuleContracts.IAspNetUserRolesClient pageClient, UserManagerModuleContracts.IAspNetUsersClient pageAspNetUsersClient, UserManagerModuleContracts.IAspNetRolesClient pageAspNetRolesClient, UserManagerModuleContracts.ITableComboboxSettingsClient tableComboboxSettingsClient, IHttpContextAccessor httpContextAccessor, IMapper mapper, IMemoryCache cache) : base(tableComboboxSettingsClient, httpContextAccessor, mapper, cache)
        {
            this.pageClient = pageClient;
            this.pageAspNetUsersClient = pageAspNetUsersClient;
            this.pageAspNetRolesClient = pageAspNetRolesClient;
        }

        [ResponseCache(VaryByQueryKeys = new[] { "ic" }, Duration = 30)]
        public async Task<IActionResult> GetImage(string ic)
        {
            return await GetImageResult(pageClient, ic);
        }

        [Route("List")]
        public async Task<IActionResult> List()
        {
            var model = GetModel<AspNetUserRolesData>();
            model.PageSize = pageSize;
            model.CurrentPage = 1;
            model.NumberOfRecord = (await pageClient.CountAsync());
            model.TotalPage = (model.NumberOfRecord / model.PageSize) + (model.NumberOfRecord % model.PageSize == 0 ? 0 : 1);
            model.ComboList = await FillPageComboBoxes(model.ComboList);
            var listResponse = (await pageClient.AspNetUserRolesGetAsync(model.CurrentPage, model.PageSize));
            model.List = mapper.Map<List<AspNetUserRolesObj>>(listResponse.ToList());
            SetModelBinder(ref model);
            return View("List", model);
        }

        [Route("List")]
        [HttpPost]
        public async Task<IActionResult> List(AspNetUserRolesData model)
        {
            ModelBinder(ref model);
            model.PageSize = pageSize;
            model.NumberOfRecord = (await pageClient.CountAsync());
            model.TotalPage = (model.NumberOfRecord / model.PageSize) + (model.NumberOfRecord % model.PageSize == 0 ? 0 : 1);
            if (model.CurrentPage == Int32.MaxValue)
            {
                model.CurrentPage = model.TotalPage;
            }

            var listResponse = (await pageClient.AspNetUserRolesGetAsync(model.CurrentPage, model.PageSize));
            model.List = mapper.Map<List<AspNetUserRolesObj>>(listResponse.ToList());
            SetModelBinder(ref model);
            model.SelectedItem = new AspNetUserRolesObj();
            return View("List", model);
        }

        [Route("Insert")]
        [HttpPost]
        public async Task<IActionResult> Insert(AspNetUserRolesData model)
        {
            ModelBinder(ref model);
            var selected = mapper.Map<UserManagerModuleContracts.AspNetUserRolesDto>(model.SelectedItem);
            var result = await pageClient.AspNetUserRolesPostAsync(selected);
            SetModelBinder(ref model);
            return Ok(result);
        }

        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update(AspNetUserRolesData model)
        {
            ModelBinder(ref model);
            var request = mapper.Map<UserManagerModuleContracts.AspNetUserRolesDto>(model.SelectedItem);
            var result = await pageClient.AspNetUserRolesPutAsync(request.UserId, request.RoleId, request);
            SetModelBinder(ref model);
            return Ok(result);
        }

        [Route("Delete")]
        [HttpPost]
        public async Task<IActionResult> Delete(AspNetUserRolesData model)
        {
            ModelBinder(ref model);
            var request = model.SelectedItem;
            var result = await pageClient.AspNetUserRolesDeleteAsync(request.UserId, request.RoleId);
            SetModelBinder(ref model);
            return Ok(result);
        }

        [Route("InsertItem")]
        public IActionResult InsertItem(AspNetUserRolesData model)
        {
            ModelState.Clear();
            ModelBinder(ref model);
            SetModelBinder(ref model);
            model.SelectedItem = new AspNetUserRolesObj();
            return PartialView("Insert", model);
        }

        [Route("DetailItem")]
        public async Task<IActionResult> DetailItem(AspNetUserRolesData model)
        {
            ModelBinder(ref model);
            if (model.List == null)
            {
                model = await FillModel(model);
            }

            model.SelectedItem = model.List.Find(i => i._Key == model.SelectedKey);
            SetModelBinder(ref model);
            return PartialView("Detail", model);
        }

        [Route("UpdateItem")]
        [HttpPost]
        public async Task<IActionResult> UpdateItem(AspNetUserRolesData model)
        {
            ModelState.Clear();
            ModelBinder(ref model);
            if (model.List == null)
            {
                model = await FillModel(model);
            }

            model.SelectedItem = model.List.Find(i => i._Key == model.SelectedKey);
            SetModelBinder(ref model);
            return PartialView("Update", model);
        }

        [Route("DeleteItem")]
        [HttpPost]
        public async Task<IActionResult> DeleteItem(AspNetUserRolesData model)
        {
            ModelBinder(ref model);
            if (model.List == null)
            {
                model = await FillModel(model);
            }

            model.SelectedItem = model.List.Find(i => i._Key == model.SelectedKey);
            SetModelBinder(ref model);
            return PartialView("Delete", model);
        }

        private async Task<AspNetUserRolesData> FillModel(AspNetUserRolesData model)
        {
            model.PageSize = pageSize;
            model.NumberOfRecord = (await pageClient.CountAsync());
            model.TotalPage = (model.NumberOfRecord / model.PageSize) + (model.NumberOfRecord % model.PageSize == 0 ? 0 : 1);
            var listResponse = (await pageClient.AspNetUserRolesGetAsync(model.CurrentPage, model.PageSize));
            model.List = mapper.Map<List<AspNetUserRolesObj>>(listResponse.ToList());
            return model;
        }

        private async Task<Dictionary<string, IEnumerable<SelectListItem>>> FillPageComboBoxes(Dictionary<string, IEnumerable<SelectListItem>> comboBoxList)
        {
            comboBoxList.Clear();
            comboBoxList.AddRange(await FillComboBoxAsync("AspNetUsers", () => pageAspNetUsersClient.AspNetUsersGetAsync()));
            comboBoxList.AddRange(await FillComboBoxAsync("AspNetRoles", () => pageAspNetRolesClient.AspNetRolesGetAsync()));
            return comboBoxList;
        }
    }
}