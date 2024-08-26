using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

namespace Toplearn.Web.Security.DependencyInjection
{
	public static class AddServicesOfIdentity
	{
		public static IServiceCollection AddIdentityServices(this IServiceCollection service)
		{

			#region Authentication

			service.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

				})
				.AddCookie(options =>
				{
					options.LoginPath = "/Login";
					options.LogoutPath = "/Logout";
					options.ExpireTimeSpan = TimeSpan.FromMinutes(43200);
					options.ReturnUrlParameter = "BackUrl";
					options.AccessDeniedPath = "/AccessDenied";
				});

			// Add Services Of Data Protection For Protect Cookies 
			service.AddDataProtection()
				.UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
				{
					EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
					ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
				});

			// Add Services Of Http_Context_Accessor  
			service.AddHttpContextAccessor();



			#endregion

			#region AddAuthorization 

			

			#endregion

			return service;
		}
	}
}
