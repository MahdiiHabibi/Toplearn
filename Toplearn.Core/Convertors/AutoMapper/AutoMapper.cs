using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TopLearn.Core.Security;
using Toplearn.DataLayer.Entities.User;
using Toplearn.Core.DTOs.Accounts;
using Toplearn.Core.DTOs.UserPanel;

namespace Toplearn.Core.Convertors.AutoMapper
{
    public class AutoMapperUser : Profile
    {
        public AutoMapperUser()
        {
            //Create The User From RegisterViewModel
            CreateMap<RegisterViewModel, User>()
                .ForMember(x => x.ActiveCode, y =>
                    y.MapFrom(d => Generator.StringGenerate.GuidGenerate()))
                .ForMember(x => x.DateTime, y =>
                    y.MapFrom(d => DateTime.Now))
                .ForMember(x => x.Password, y =>
                    y.MapFrom(d => d.Password.EncodePasswordMd5()))
                .ForMember(x => x.ImageUrl, y =>
                    y.MapFrom(d => "/images/pic/Default.png"));

            // Create the SendEmailHtmlViewModel From User
            CreateMap<User, SendEmailHtmlViewModel>()
                .ForMember(x => x.FullName, y => y.MapFrom(d => d.FullName))
                .ForMember(x => x.ActiveCode, y => y.MapFrom(d => d.ActiveCode));


            // Create The UsePanelViewModel From CookiesUser
            CreateMap<IEnumerable<Claim>, UserPanelViewModel>()
                .ForMember(x => x.Email, y => y.MapFrom(d => d.SingleOrDefault(s => s.Type == ClaimTypes.Email)!.Value))
                .ForMember(x => x.DateTime, y => y.MapFrom(d => Convert.ToDateTime(d.SingleOrDefault(s => s.Type == "DateTimeOfRegister")!.Value)))
                .ForMember(x => x.FullName, y => y.MapFrom(d => d.SingleOrDefault(s => s.Type == ClaimTypes.Name)!.Value))
                .ForMember(x => x.UserName, y => y.MapFrom(d => d.SingleOrDefault(s => s.Type == "UserName")!.Value))
                .ForMember(x => x.WalletBalance, y => y.MapFrom(d => 0))
                .ForMember(x => x.ImageUrl, y => y.MapFrom(d => d.SingleOrDefault(s => s.Type == "ImageUrl")!.Value))
                .ForMember(x => x.UserId, y => y.MapFrom(d => Convert.ToInt32(d.SingleOrDefault(s => s.Type == ClaimTypes.NameIdentifier)!.Value)));
        }

    }
}
