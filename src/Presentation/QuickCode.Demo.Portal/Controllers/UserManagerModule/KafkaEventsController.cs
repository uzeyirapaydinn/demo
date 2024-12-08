using System;
using System.Collections.Generic;
using System.Linq;
using QuickCode.Demo.Portal.Models;
using QuickCode.Demo.Portal.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using QuickCode.Demo.Portal.Helpers.Authorization;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using QuickCode.Demo.Common.Helpers;
using QuickCode.Demo.Common.Nswag.Clients.UserManagerModuleApi.Contracts;

namespace QuickCode.Demo.Portal.Controllers.UserManagerModule
{
    [Permission("UserManagerModulePortalPermissions")]
    public partial class KafkaEventsController : BaseController
    {
        [Route("GetKafkaEvents")]
        [HttpGet]
        public async Task<IActionResult> GetKafkaEvents()
        {
            var model = GetModel<GetKafkaEventsData>();
            var kafkaEvents = await pageClient.GetKafkaEventsAsync();
            model.Items = new Dictionary<string, Dictionary<string, List<KafkaEventsGetKafkaEventsResponseDto>>>();
            foreach (var item in kafkaEvents)
            {
                var moduleName = item.Path.Split('/')[2].KebabCaseToPascal("");
                if (item.ControllerName.Equals("AuthenticationController"))
                {
                    moduleName = "UserManagerModule";
                }
                model.Items.TryAdd(moduleName, new Dictionary<string, List<KafkaEventsGetKafkaEventsResponseDto>>());
                model.Items[moduleName].TryAdd(item.ControllerName, []);
                model.Items[moduleName][item.ControllerName].Add(item);
            }

            SetModelBinder(ref model);
            return View("KafkaEvents", model);
        }

        [Route("UpdateKafkaEvent")]
        [HttpPost]
        public async Task<JsonResult> UpdateKafkaEvent(UpdateKafkaEvent request)
        {
            var eventData = await pageClient.KafkaEventsGetAsync(request.Id);
            eventData.IsActive = request.Value == 1;
            
            var result = await pageClient.KafkaEventsPutAsync(request.Id, eventData);
            return Json(result);
        }
    }
}

