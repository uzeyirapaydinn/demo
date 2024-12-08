using QuickCode.Demo.Common.Nswag.Clients.UserManagerModuleApi.Contracts;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using QuickCode.Demo.Portal.Helpers;

namespace QuickCode.Demo.Portal.Models.UserManagerModule
{
    public class PortalMenusData
    {
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int NumberOfRecord { get; set; }
        public string SelectedKey { get; set; }
        public PortalMenusObj SelectedItem { get; set; }

        public Dictionary<string, IEnumerable<SelectListItem>> ComboList = new Dictionary<string, IEnumerable<SelectListItem>>();
        public List<PortalMenusObj> List { get; set; }
    }

    public class PortalMenusObj : PortalMenusDto
    {
        private object[] KeyValues
        {
            get
            {
                return new object[]
                {
                    Id
                };
            }
        }

        public string _Key
        {
            get
            {
                return String.Join("|", this.KeyValues.Select(i => i.AsString()));
            }
        }
    }
}