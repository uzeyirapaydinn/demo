using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuickCode.Demo.Portal.Models
{
	public class LoginData
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public bool RememberMe { get; set; }
		public string ReturnUrl { get; set; }
		public string ErrorMessage { get; set; }
	}
}

