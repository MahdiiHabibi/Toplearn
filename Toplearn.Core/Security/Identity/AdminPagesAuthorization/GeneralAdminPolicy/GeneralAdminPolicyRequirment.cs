using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Toplearn.Core.Security.Identity.AdminPagesAuthorization.GeneralAdminPolicy
{
    public class GeneralAdminPolicyRequirement : IAuthorizationRequirement
    {
	    public int UserId { get; set; } = 0;
    }
}
