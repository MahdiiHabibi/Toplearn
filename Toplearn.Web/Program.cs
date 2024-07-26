
#region Services

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Toplearn.Core.Convertors;
using Toplearn.Core.Convertors.AutoMapper;
using Toplearn.Core.Services.Implement;
using Toplearn.Core.Services.Implement.Mapper;
using Toplearn.Core.Services.Interface;
using Toplearn.Core.Services.Interface.Mapper;
using Toplearn.DataLayer.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

#region Ioc
// Add DbContext (TopLearnContext) with SqlServer
builder.Services.AddDbContextFactory<TopLearnContext>(option =>

	option.UseSqlServer("Server=.;Database=Toplearn;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true")
);

// All Works that we can do with Database with dynamic Code
builder.Services.AddScoped(typeof(IContextActions<>), typeof(ContextActions<>));
// Add Services that we need to Convert View to String
builder.Services.AddScoped<IViewRenderService, RenderViewToString>();

// Add User Services that we need to do for some Action Like : Register || Login 
builder.Services.AddScoped<IUserAction, UserAction>();


#endregion

#region AutoMapper

// Enable AutoMapper To Convert Objet to other Object
builder.Services.AddAutoMapper(typeof(AutoMapperUser));
builder.Services.AddScoped<IMapperAccount, MapperAccount>();

#endregion

#region Authentication

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

}).AddCookie(options =>
{
	options.LoginPath = "/Login";
	options.LogoutPath = "/Logout";
	options.ExpireTimeSpan = TimeSpan.FromMinutes(43200);
	options.ReturnUrlParameter = "BackUrl_Url";
});

#endregion

#endregion


#region WebApp

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


#endregion