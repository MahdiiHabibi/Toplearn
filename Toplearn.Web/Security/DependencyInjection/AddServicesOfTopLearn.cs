using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Toplearn.Core.Convertors;
using Toplearn.Core.Services.Implement;
using Toplearn.Core.Services.Implement.SendEmail;
using Toplearn.Core.Services.Implement.Setting;
using Toplearn.Core.Services.Interface;
using Toplearn.Core.Services.Interface.ISendEmail;
using Toplearn.DataLayer.Context;
using WebMarkupMin.AspNetCore8;

namespace Toplearn.Web.Security.DependencyInjection
{
    public static class AddServicesOfTopLearn
    {
        public static WebApplicationBuilder AddIoCs(this WebApplicationBuilder builder)
        {
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
            // Add User Services that we need to do for some Action in User Panel
            builder.Services.AddScoped<IUserPanelService, UserPanelService>();
            // Add Services that we need in everyThing That About Wallet
            builder.Services.AddScoped<IWalletManager, WalletManager>();
            // Add Services that we need in everyThing in Admin Layer
            builder.Services.AddScoped<IAdminServices, AdminServices>();
            // Add Role Services that we need to do for some Action Like : Get Roles || Add 
            builder.Services.AddTransient<IRoleManager, RoleManager>();
            // Add Services Of Email Sender Information For Send
            builder.Services.AddSingleton<ISendEmail, SendEmail>();
            builder.Services.Configure<SendEmailViewModel>(builder.Configuration.GetSection("EmailSenderInformation"));
            // Add Services Of MemoryCache
            builder.Services.AddMemoryCache();
            // Add Services Of App Setting
            builder.Services.AddScoped<IUtilities, Utilities>();
            //
            builder.Services.AddSingleton<IPermissionServices, PermissionServices>();


			#endregion

			#region WebMarkupMin

			builder.Services.AddWebMarkupMin()
				.AddHtmlMinification()
				.AddXhtmlMinification()
				.AddXmlMinification();

			#endregion

			return builder;
        }
	}
}
