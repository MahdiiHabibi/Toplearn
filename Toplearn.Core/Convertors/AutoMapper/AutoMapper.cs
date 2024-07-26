using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TopLearn.Core.Security;
using Toplearn.DataLayer.Entities.User;
using Toplearn.Core.DTOs.Accounts;

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
                    y.MapFrom(d => "~/images/pic/Default.jpg"));

            // Create the From User
            CreateMap<User, SendEmailHtmlViewModel>()
	            .ForMember(x=>x.FullName,y=>y.MapFrom(d=>d.FullName))
	            .ForMember(x=>x.ActiveCode,y=>y.MapFrom(d=>d.ActiveCode));

        }

    }
}
