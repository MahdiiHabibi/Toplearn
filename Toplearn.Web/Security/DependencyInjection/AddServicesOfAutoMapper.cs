using Toplearn.Core.Convertors.AutoMapper;
using Toplearn.Core.Services.Implement.Mapper;
using Toplearn.Core.Services.Interface.Mapper;

namespace Toplearn.Web.Security.DependencyInjection
{
	public static class AddServicesOfAutoMapper
	{
		public static IServiceCollection AddAutoMapper(this IServiceCollection service)
		{
			#region AutoMapper

			// Enable AutoMapper To Convert Objet to other Object
			service.AddAutoMapper(typeof(AutoMapperUser));
			// Add Rep Of Maps that we need in Account Actions
			service.AddScoped<IMapperAccount, MapperAccount>();
			// Add Rep Of Maps that we need in UserPanel Area
			service.AddScoped<IMapperUserPanel, MapperUserPanel>();
			// Add Rep Of Maps that we need in UserPanel Area And Wallet Controller
			service.AddScoped<IMapperWallet, MapperWallet>();
			// Add Rep Of Maps that we need in Admin Area 
			service.AddScoped<IMapperAdmin, MapperAdmin>();

			#endregion

			return service;
		}
	}
}
