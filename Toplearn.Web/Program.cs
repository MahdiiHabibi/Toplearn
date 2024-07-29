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
using WebMarkupMin.AspNetCore8;

#region Services


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
// Add Rep Of Maps that we need in Account Actions
builder.Services.AddScoped<IMapperAccount, MapperAccount>();
// Add Rep Of Maps that we need in UserPanel Area
builder.Services.AddScoped<IMapperUserPanel,MapperUserPanel>();

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

#region WebMarkupMin

builder.Services.AddWebMarkupMin()
	.AddHtmlMinification()
	.AddXhtmlMinification()
	.AddXmlMinification();

#endregion

builder.Services.AddElmahIo(o =>
{
	o.ApiKey = "e7df4a60042843fabd1894e263c1debc";
	o.LogId = new Guid("69ab5b1c-5d04-466d-ac52-bfe45c7017bb");
});

#endregion


#region WebApp

var app = builder.Build();


//if (!app.Environment.IsDevelopment())
//{
//	app.UseExceptionHandler("/Home/Error");
//	app.UseHsts();
//}
app.UseDeveloperExceptionPage();
app.UseElmahIo();
app.UseStaticFiles();

app.UseRouting();

app.UseWebMarkupMin();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


#endregion