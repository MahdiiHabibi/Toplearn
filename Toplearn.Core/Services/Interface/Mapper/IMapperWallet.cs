using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.Core.DTOs.Wallet;
using Toplearn.DataLayer.Entities.Wallet;

namespace Toplearn.Core.Services.Interface.Mapper
{
	public interface IMapperWallet
	{
		public GetPaymentInformationViewModel MapTheGetPaymentInformationViewModelFromWallet(Wallet wallet);
	}
}
