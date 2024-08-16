using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Toplearn.Core.Security.Identity.CheckIVGAuthotization
{
	public class CheckIVGPolicyRequirment : IAuthorizationRequirement
	{
		public int UserId { get; set; }
	}
}
