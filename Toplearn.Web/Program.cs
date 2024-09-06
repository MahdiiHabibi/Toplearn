using IdentitySample.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Toplearn.Core.Convertors;
using Toplearn.Core.Convertors.AutoMapper;
using Toplearn.Core.Services.Implement;
using Toplearn.Core.Services.Implement.Mapper;
using Toplearn.Core.Services.Interface;
using Toplearn.Core.Services.Interface.Mapper;
using Toplearn.DataLayer.Context;
using WebMarkupMin.AspNetCore8;
using Toplearn.Core.Services.Implement.SendEmail;
using Toplearn.Core.Services.Interface.ISendEmail;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection;
using Toplearn.Core.Services.Implement.Setting;
using Toplearn.Web.Security.DependencyInjection;
using Microsoft.AspNetCore.Http.Features;




#region Services


var builder = WebApplication.CreateBuilder(args);

// TODO:
builder.Services.AddControllersWithViews()
	.AddRazorRuntimeCompilation();

builder.Services.AddElmahIo(o =>
{
	o.ApiKey = "e7df4a60042843fabd1894e263c1debc";
	o.LogId = new Guid("69ab5b1c-5d04-466d-ac52-bfe45c7017bb");
});

builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 507374182);
builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 507374182;options.ValueLengthLimit = 507374182; });


#region Ioc

builder.AddIoCs();

#endregion

#region AutoMapper

builder.Services.AddAutoMapper();

#endregion

#region Identity Services 

builder.Services.AddIdentityServices();

#endregion



#endregion


#region WebApp

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
	//app.UseExceptionHandler("/Home/Error");
	//app.UseHsts();

}

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