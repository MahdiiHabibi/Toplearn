using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Toplearn.Core.DTOs.Wallet;
using Toplearn.Core.Services.Interface.Mapper;
using Toplearn.DataLayer.Entities.Wallet;

namespace Toplearn.Core.Services.Implement.Mapper
{
	public class MapperWallet(IMapper _mapper) : IMapperWallet
	{
		public GetPaymentInformationViewModel MapTheGetPaymentInformationViewModelFromWallet(Wallet wallet) =>
			 _mapper.Map<GetPaymentInformationViewModel>(wallet);

	}
}
