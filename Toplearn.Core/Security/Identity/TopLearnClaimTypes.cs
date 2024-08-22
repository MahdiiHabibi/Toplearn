using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.Security.Identity
{
	public static class TopLearnClaimTypes
	{   /// <summary>The URI for a claim that specifies the email address of an entity, <c>http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress</c>.</summary>
		public const string Email = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
		/// <summary>The URI for a claim that specifies the name of an entity, <c>http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name</c>.</summary>
		public const string Name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
		/// <summary>The URI for a claim that specifies the name of an entity, <c>http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier</c>.</summary>
		public const string NameIdentifier = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
		/// <summary>
		/// <c>http://schemas.microsoft.com/ws/2008/06/identity/claims/ispersistent</c>.</summary>
		public const string IsPersistent = "http://schemas.microsoft.com/ws/2008/06/identity/claims/ispersistent";

		public const string ImageUrl = nameof(ImageUrl);

		public const string UserName = nameof(UserName);
		
		public const string DateTimeOfRegister = nameof(DateTimeOfRegister);
		
	}
}
