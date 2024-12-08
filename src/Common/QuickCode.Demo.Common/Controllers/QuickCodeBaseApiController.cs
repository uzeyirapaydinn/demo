using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QuickCode.Demo.Common.Filters;

namespace QuickCode.Demo.Common.Controllers;

[ApiExceptionFilter]
[ApiController]
[ApiKey]
[Route("api/[controller]")]
public class QuickCodeBaseApiController : ControllerBase
{

}