using QuickCode.Demo.Common.Nswag.Clients.EmailManagerModuleApi.Contracts;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using QuickCode.Demo.Portal.Helpers;

namespace QuickCode.Demo.Portal.Models.EmailManagerModule
{
    public class InfoTypesData
    {
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int NumberOfRecord { get; set; }
        public string SelectedKey { get; set; }
        public InfoTypesObj SelectedItem { get; set; }

        public Dictionary<string, IEnumerable<SelectListItem>> ComboList = new Dictionary<string, IEnumerable<SelectListItem>>();
        public List<InfoTypesObj> List { get; set; }
    }

    public class InfoTypesObj : InfoTypesDto
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